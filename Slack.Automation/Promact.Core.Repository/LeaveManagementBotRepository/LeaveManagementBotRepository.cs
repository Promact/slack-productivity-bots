using AutoMapper;
using Promact.Core.Repository.AttachmentRepository;
using Promact.Core.Repository.BotQuestionRepository;
using Promact.Core.Repository.Client;
using Promact.Core.Repository.LeaveRequestRepository;
using Promact.Core.Repository.OauthCallsRepository;
using Promact.Core.Repository.SlackUserRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.StringLiteral;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Promact.Core.Repository.LeaveManagementBotRepository
{
    public class LeaveManagementBotRepository : ILeaveManagementBotRepository
    {
        #region Private variables
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly ApplicationUserManager _userManager;
        private readonly IOauthCallsRepository _oauthCallRepository;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IMapper _mapper;
        private readonly AppStringLiteral _stringConstant;
        private readonly IRepository<TemporaryLeaveRequestDetail> _temporaryLeaveRequestDetailDataRepository;
        private readonly IBotQuestionRepository _botQuestionRepository;
        private readonly IClient _clientRepository;
        private readonly ISlackUserRepository _slackUserRepository;
        #endregion

        #region Constructor
        public LeaveManagementBotRepository(ILeaveRequestRepository leaveRequestRepository, ApplicationUserManager userManager,
            IOauthCallsRepository oauthCallRepository, IAttachmentRepository attachmentRepository, IMapper mapper,
            ISingletonStringLiteral stringConstant, IRepository<TemporaryLeaveRequestDetail> temporaryLeaveRequestDetailDataRepository,
            IBotQuestionRepository botQuestionRepository, IClient clientRepository, ISlackUserRepository slackUserRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _userManager = userManager;
            _oauthCallRepository = oauthCallRepository;
            _attachmentRepository = attachmentRepository;
            _mapper = mapper;
            _stringConstant = stringConstant.StringConstant;
            _temporaryLeaveRequestDetailDataRepository = temporaryLeaveRequestDetailDataRepository;
            _botQuestionRepository = botQuestionRepository;
            _slackUserRepository = slackUserRepository;
            _clientRepository = clientRepository;
        }
        #endregion

        #region Public Method
        /// <summary>
        /// Method to process leave request - SS
        /// </summary>
        /// <param name="slackUserId">user's slack Id</param>
        /// <param name="answer">text send from user</param>
        /// <returns>reply to be send</returns>
        public async Task<string> ProcessLeaveAsync(string slackUserId, string answer)
        {
            string replyText = string.Empty;
            // Way to break string by spaces only if spaces are not between quotes
            List<string> slackText = _attachmentRepository.SlackText(answer);
            // Way to get user details from oauth server if leave is not in process
            var user = await GetUserDetailsFromOAuthAsync(slackUserId);
            if (user != null)
            {
                // if user is not active in oauth server then this will be null
                if (!string.IsNullOrEmpty(user.Id))
                {
                    // validation if message recieve from user is leave command or reply of question
                    if (slackText[0] == _stringConstant.Leave && slackText.Count > 1)
                    {
                        SlackAction slackAction;
                        // validation of leave command is proper or not
                        if (Enum.TryParse(slackText[1], out slackAction))
                        {
                            switch (slackAction)
                            {
                                // leave apply command
                                case SlackAction.apply:
                                    replyText = await StartLeaveProcessAsync(user.Id, answer);
                                    break;
                                // leave list command
                                case SlackAction.list:
                                    {
                                        // if leave list for other user
                                        if (slackText.Count > 2)
                                            replyText = await GetLeaveListAsync(user.Id, slackText[2]);
                                        // if leave list for user itself
                                        else
                                            replyText = await GetLeaveListAsync(user.Id, null);
                                    }
                                    break;
                                // leave cancel command
                                case SlackAction.cancel:
                                    {
                                        // validation if leave cancel is not proper
                                        if (slackText.Count > 2)
                                            replyText = await LeaveCancelAsync(user.Id, slackText[2]);
                                        else
                                            replyText = _stringConstant.IncorrectLeaveCancelCommandMessage;
                                    }
                                    break;
                                // last leave status command
                                case SlackAction.status:
                                    replyText = GetLeaveStatus(user.Id);
                                    break;
                                // leave balance command
                                case SlackAction.balance:
                                    replyText = await GetUserLeaveBalanceAsync(user.Id);
                                    break;
                                // sick leave update command
                                case SlackAction.update:
                                    replyText = await UpdateSickLeaveByAdminAsync(slackText, user.Id);
                                    break;
                                // leave help or if any other command get parse
                                default:
                                    {
                                        // if user is admin then help command will be contain update command else not
                                        if (await _oauthCallRepository.UserIsAdminAsync(user.Id, (await _attachmentRepository.UserAccessTokenAsync(user.UserName))))
                                            replyText = string.Format(_stringConstant.FirstAndSecondIndexStringFormat,
                                                _stringConstant.LeaveHelpBotCommands, _stringConstant.LeaveUpdateFormatMessage);
                                        else
                                            replyText = _stringConstant.LeaveHelpBotCommands;
                                    }
                                    break;
                            }
                        }
                        // Improper leave action message
                        else
                            replyText = _stringConstant.ProperActionErrorMessage;
                    }
                    // If leave will be not first word of message then goes for leave apply process
                    else
                        replyText = await LeaveApplyProcessAsync(answer, user.Id);
                }
                // User is not active in oauth server error message
                else
                    replyText = _stringConstant.InActiveUserErrorMessage;
            }
            // User details not found error message
            else
                replyText = _stringConstant.SorryYouCannotApplyLeave;
            return replyText;
        }

        /// <summary>
        /// method to convert slack user id to slack user's name - SS
        /// </summary>
        /// <param name="message">message from slack</param>
        /// <param name="userNotFound">if user is not found</param>
        /// <returns>message after conversation</returns>
        public string ProcessToConvertSlackIdToSlackUserName(string message, out bool userNotFound)
        {
            // regex pattern for slack message of user id
            Regex pattern = new Regex(@_stringConstant.UserIdPattern);
            Match match = pattern.Match(message);
            // if userid is contain any user id
            if (match.Length != 0)
            {
                //the slack userId is fetched
                string applicantId = match.Groups[_stringConstant.UserId].Value;
                var applicant = _slackUserRepository.GetByIdAsync(applicantId).Result;
                // if user slack detail is found then id will be replace with name
                if (applicant != null)
                {
                    userNotFound = false;
                    message = message.Replace(match.Value, applicant.Name);
                }
                // if user slack detail is not found then message will be send for user not found
                else
                {
                    userNotFound = true;
                    message = string.Format(_stringConstant.UserNotFoundRequestToAddToSlackOtherUser, match.Value);
                }
            }
            else
                userNotFound = false;
            return message;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Method to get user details from oauth server if leave is not in process - SS
        /// </summary>
        /// <param name="slackUserId">user's slack Id</param>
        /// <returns>user detail</returns>
        private async Task<ApplicationUser> GetUserDetailsFromOAuthAsync(string slackUserId)
        {
            // user details from slack table
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.SlackUserId == slackUserId);
            // if found user detail then will go further
            if (user != null)
            {
                // check if user is process of leave apply or not
                if (!_temporaryLeaveRequestDetailDataRepository.Any(x => x.EmployeeId == user.Id))
                {
                    // if leave is not in not process then will check user details from oauth server
                    var oauthUser = await _oauthCallRepository.GetUserByUserIdAsync(user.Id,
                        (await _attachmentRepository.UserAccessTokenAsync(user.UserName)));
                    // if user is not active then user details will be send else user's Id will be null
                    if (oauthUser.IsActive)
                        user = _mapper.Map<User, ApplicationUser>(oauthUser);
                    else
                        user.Id = null;
                }
            }
            return user;
        }

        /// <summary>
        /// Method to start leave apply process - SS
        /// </summary>
        /// <param name="userId">user's Id</param>
        /// <returns>reply to be send</returns>
        private async Task<string> StartLeaveProcessAsync(string userId, string answer)
        {
            // check if any leave for user is on process
            if (!_temporaryLeaveRequestDetailDataRepository.Any(x => x.EmployeeId == userId))
            {
                // first question of leave application
                var nextQuestion = await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.LeaveType);
                // adding leave detail in temporary table
                var leave = new TemporaryLeaveRequestDetail()
                {
                    CreatedOn = DateTime.UtcNow,
                    EmployeeId = userId,
                    QuestionId = nextQuestion.Id,
                    Status = Condition.Pending
                };
                _temporaryLeaveRequestDetailDataRepository.Insert(leave);
                await _temporaryLeaveRequestDetailDataRepository.SaveChangesAsync();
                return nextQuestion.QuestionStatement;
            }
            // if leave is already in process
            else
                return await LeaveApplyProcessAsync(null, userId);
        }

        /// <summary>
        /// Method to process leave apply - SS
        /// </summary>
        /// <param name="answer">message from slack user</param>
        /// <param name="userId">user's Id</param>
        /// <returns>reply to be send</returns>
        private async Task<string> LeaveApplyProcessAsync(string answer, string userId)
        {
            var replyText = string.Empty;
            // previous leave details if exist or not
            var leave = await _temporaryLeaveRequestDetailDataRepository.FirstOrDefaultAsync(x => x.EmployeeId == userId);
            if (leave != null)
            {
                // if leave exist then get next question detail from leave question Id
                var previousQuestion = await _botQuestionRepository.FindByIdAsync(leave.QuestionId);
                switch (previousQuestion.OrderNumber)
                {
                    // if previous question was leave type question then answer will go here
                    case QuestionOrder.LeaveType:
                        replyText = await AddLeaveTypeDetailsAsync(answer, leave);
                        break;
                    // if previous question was leave reason question then answer will go here
                    case QuestionOrder.Reason:
                        replyText = await AddLeaveReasonDetailsAsync(answer, leave);
                        break;
                    // if previous question was leave from date question then answer will go here
                    case QuestionOrder.FromDate:
                        replyText = await AddLeaveStartDateDetailsAsync(answer, leave);
                        break;
                    // if previous question was leave end date question then answer will go here
                    case QuestionOrder.EndDate:
                        replyText = await AddLeaveEndDateDetailsAsync(answer, leave);
                        break;
                    // if previous question was leave rejoin date question then answer will go here
                    case QuestionOrder.RejoinDate:
                        replyText = await AddLeaveRejoinDateDetailsAsync(answer, leave);
                        break;
                    // if previous question was leave send mail question then answer will go here
                    case QuestionOrder.SendLeaveMail:
                        replyText = await AddLeaveSendMailDetailsAsync(answer, leave);
                        break;
                }
            }
            // if any other message is send and leave is also not in process
            else
                replyText = _stringConstant.LeaveBotDoesNotUnderStandErrorMessage;
            return replyText;
        }

        /// <summary>
        /// Method to update temporary details of leave - SS
        /// </summary>
        /// <param name="temporaryLeaveRequestDetail">leave temporary details</param>
        private async Task UpdateTemporaryLeaveDetailsAsync(TemporaryLeaveRequestDetail temporaryLeaveRequestDetail)
        {
            // update leave detail in temporary leave table
            _temporaryLeaveRequestDetailDataRepository.Update(temporaryLeaveRequestDetail);
            await _temporaryLeaveRequestDetailDataRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Method to get question of leave management by order number - SS
        /// </summary>
        /// <param name="orderNumber">order number</param>
        /// <returns>question details</returns>
        private async Task<Question> GetLeaveQuestionDetailsByOrderAsync(QuestionOrder orderNumber)
        {
            return await _botQuestionRepository.FindByTypeAndOrderNumberAsync((int)orderNumber, (int)BotQuestionType.LeaveManagement);
        }

        /// <summary>
        /// Method to add leave type and get next question for leave apply - SS
        /// </summary>
        /// <param name="answer">reply from slack user</param>
        /// <param name="temporaryLeaveRequestDetail">temporary leave details</param>
        /// <returns>reply to be send to slack user</returns>
        private async Task<string> AddLeaveTypeDetailsAsync(string answer, TemporaryLeaveRequestDetail temporaryLeaveRequestDetail)
        {
            LeaveType leaveType;
            // if user answer is from leave type then goes here
            if (Enum.TryParse(answer, out leaveType))
            {
                temporaryLeaveRequestDetail.Type = leaveType;
                // next question for leave application
                var nextQuestion = await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.Reason);
                temporaryLeaveRequestDetail.QuestionId = nextQuestion.Id;
                await UpdateTemporaryLeaveDetailsAsync(temporaryLeaveRequestDetail);
                return nextQuestion.QuestionStatement;
            }
            // if user answer is not from leave type then goes here
            else
            {
                var previousQuestion = await _botQuestionRepository.FindByIdAsync(temporaryLeaveRequestDetail.QuestionId);
                return string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat, _stringConstant.NotTypeOfLeave,
                    Environment.NewLine, previousQuestion.QuestionStatement);
            }
        }

        /// <summary>
        /// Method to add leave reason and get next question for leave apply - SS
        /// </summary>
        /// <param name="answer">reply from slack user</param>
        /// <param name="temporaryLeaveRequestDetail">temporary leave details</param>
        /// <returns>reply to be send to slack user</returns>
        private async Task<string> AddLeaveReasonDetailsAsync(string answer, TemporaryLeaveRequestDetail temporaryLeaveRequestDetail)
        {
            if (answer != null)
            {
                temporaryLeaveRequestDetail.Reason = answer;
                // next question for leave application
                var nextQuestion = await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.FromDate);
                temporaryLeaveRequestDetail.QuestionId = nextQuestion.Id;
                await UpdateTemporaryLeaveDetailsAsync(temporaryLeaveRequestDetail);
                return nextQuestion.QuestionStatement;
            }
            // if user leaves the leave application here point then fired leave apply then user will get this question again
            else
                return (await _botQuestionRepository.FindByIdAsync(temporaryLeaveRequestDetail.QuestionId)).QuestionStatement;
        }

        /// <summary>
        /// Method to add leave start date and get next question for leave apply - SS
        /// </summary>
        /// <param name="answer">reply from slack user</param>
        /// <param name="temporaryLeaveRequestDetail">temporary leave details</param>
        /// <returns>reply to be send to slack user</returns>
        private async Task<string> AddLeaveStartDateDetailsAsync(string answer, TemporaryLeaveRequestDetail temporaryLeaveRequestDetail)
        {
            DateTime startDate;
            // get date format of current culture
            string dateFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
            // try to parse if answer is as same format of current culture 
            if (DateTime.TryParseExact(answer, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
            {
                // check any leave exist on this date
                if (!DuplicateLeaveExist(startDate, temporaryLeaveRequestDetail.EmployeeId, true, null))
                {
                    // check leave's first date is not beyond toCheckDate. Back date checking
                    if (LeaveDateValid(DateTime.UtcNow, startDate))
                    {
                        // if sick leave then start date will be end date and rejoin date will be next day and ask user to send mail else
                        // for cl next question will be asked
                        Question nextQuestion;
                        temporaryLeaveRequestDetail.FromDate = startDate;
                        if (temporaryLeaveRequestDetail.Type == LeaveType.cl)
                            nextQuestion = await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.EndDate);
                        else
                        {
                            nextQuestion = await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.SendLeaveMail);
                            temporaryLeaveRequestDetail.EndDate = null;
                            temporaryLeaveRequestDetail.RejoinDate = null;
                            temporaryLeaveRequestDetail.Status = Condition.Approved;
                        }
                        temporaryLeaveRequestDetail.QuestionId = nextQuestion.Id;
                        await UpdateTemporaryLeaveDetailsAsync(temporaryLeaveRequestDetail);
                        return nextQuestion.QuestionStatement;
                    }
                    // back date error message
                    else
                        return _stringConstant.BackDateErrorMessage;
                }
                // duplicate date error message
                else
                    return _stringConstant.LeaveAlreadyExistOnSameDate;
            }
            // date format error message
            else
                return string.Format(_stringConstant.DateFormatErrorMessage, dateFormat);
        }

        /// <summary>
        /// Method to add leave end date and get next question for leave apply - SS
        /// </summary>
        /// <param name="answer">reply from slack user</param>
        /// <param name="temporaryLeaveRequestDetail">temporary leave details</param>
        /// <returns>reply to be send to slack user</returns>
        private async Task<string> AddLeaveEndDateDetailsAsync(string answer, TemporaryLeaveRequestDetail temporaryLeaveRequestDetail)
        {
            DateTime endDate;
            // get date format of current culture
            string dateFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
            // try to parse if answer is as same format of current culture 
            if (DateTime.TryParseExact(answer, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
            {
                // check leave's first date is not beyond toCheckDate. Back date checking
                if (LeaveDateValid(temporaryLeaveRequestDetail.FromDate.Value, endDate))
                {
                    // check any leave exist on this date
                    if (!DuplicateLeaveExist(temporaryLeaveRequestDetail.FromDate.Value, temporaryLeaveRequestDetail.EmployeeId, false, endDate))
                    {
                        temporaryLeaveRequestDetail.EndDate = endDate;
                        // next question for leave application
                        var nextQuestion = await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.RejoinDate);
                        temporaryLeaveRequestDetail.QuestionId = nextQuestion.Id;
                        await UpdateTemporaryLeaveDetailsAsync(temporaryLeaveRequestDetail);
                        return nextQuestion.QuestionStatement;
                    }
                    // duplicate date error message
                    else
                        return _stringConstant.LeaveAlreadyExistOnSameDate;
                }
                // back date error message
                else
                    return string.Format(_stringConstant.EndDateBeyondStartDateErrorMessage, Environment.NewLine,
                        (await _botQuestionRepository.FindByIdAsync(temporaryLeaveRequestDetail.QuestionId)).QuestionStatement);
            }
            // date format error message
            else
            {
                var dateErrorMessage = string.Format(_stringConstant.DateFormatErrorMessage, dateFormat);
                return string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat, dateErrorMessage, Environment.NewLine,
                    (await _botQuestionRepository.FindByIdAsync(temporaryLeaveRequestDetail.QuestionId)).QuestionStatement);
            }
        }

        /// <summary>
        /// Method to add leave rejoin date and get next question for leave apply - SS
        /// </summary>
        /// <param name="answer">reply from slack user</param>
        /// <param name="temporaryLeaveRequestDetail">temporary leave details</param>
        /// <returns>reply to be send to slack user</returns>
        private async Task<string> AddLeaveRejoinDateDetailsAsync(string answer, TemporaryLeaveRequestDetail temporaryLeaveRequestDetail)
        {
            DateTime rejoinDate;
            // get date format of current culture
            string dateFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
            // try to parse if answer is as same format of current culture 
            if (DateTime.TryParseExact(answer, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out rejoinDate))
            {
                // check leave's first date is not beyond toCheckDate. Back date checking
                if (LeaveDateValid(temporaryLeaveRequestDetail.EndDate.Value, rejoinDate))
                {
                    temporaryLeaveRequestDetail.RejoinDate = rejoinDate;
                    var nextQuestion = await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.SendLeaveMail);
                    temporaryLeaveRequestDetail.QuestionId = nextQuestion.Id;
                    await UpdateTemporaryLeaveDetailsAsync(temporaryLeaveRequestDetail);
                    return nextQuestion.QuestionStatement;
                }
                // back date error message
                else
                    return string.Format(_stringConstant.RejoinDateBeyondEndDateErrorMessage, Environment.NewLine,
                        (await _botQuestionRepository.FindByIdAsync(temporaryLeaveRequestDetail.QuestionId)).QuestionStatement);
            }
            // date format error message
            else
            {
                var dateErrorMessage = string.Format(_stringConstant.DateFormatErrorMessage, dateFormat);
                return string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat, dateErrorMessage, Environment.NewLine,
                    (await _botQuestionRepository.FindByIdAsync(temporaryLeaveRequestDetail.QuestionId)).QuestionStatement);
            }
        }

        /// <summary>
        /// Method to add leave in leave request table and send mail for leave apply - SS
        /// </summary>
        /// <param name="answer">reply from slack user</param>
        /// <param name="temporaryLeaveRequestDetail">temporary leave details</param>
        /// <returns>reply to be send to slack user</returns>
        private async Task<string> AddLeaveSendMailDetailsAsync(string answer, TemporaryLeaveRequestDetail temporaryLeaveRequestDetail)
        {
            SendEmailConfirmation confirmation;
            // try to parse user answer is SendEmailConfirmation or not
            if (Enum.TryParse(answer, out confirmation))
            {
                switch (confirmation)
                {
                    // if yes then mail and notification will be send
                    case SendEmailConfirmation.yes:
                        {
                            // user details
                            var user = (await _userManager.FindByIdAsync(temporaryLeaveRequestDetail.EmployeeId));
                            // user' slack username
                            var slackUserName = (await _slackUserRepository.GetByIdAsync(user.SlackUserId)).Name;
                            // leave applied and move to leave request table and deleted from temporary table
                            var leave = _mapper.Map<TemporaryLeaveRequestDetail, LeaveRequest>(temporaryLeaveRequestDetail);
                            await _leaveRequestRepository.ApplyLeaveAsync(leave);
                            _temporaryLeaveRequestDetailDataRepository.Delete(temporaryLeaveRequestDetail.Id);
                            await _temporaryLeaveRequestDetailDataRepository.SaveChangesAsync();
                            // message format to be send to slack will be vary for cl and sl
                            if (temporaryLeaveRequestDetail.Type == LeaveType.cl)
                            {
                                var message = _attachmentRepository.ReplyText(slackUserName, leave);
                                // slack message to user, management and TL
                                await _clientRepository.SendMessageWithAttachmentIncomingWebhookAsync(leave,
                                    await _attachmentRepository.UserAccessTokenAsync(user.UserName), message,
                                    temporaryLeaveRequestDetail.EmployeeId);
                            }
                            else
                            {
                                var message = _attachmentRepository.ReplyTextSick(slackUserName, leave);
                                // slack message to user, management and TL
                                await _clientRepository.SendMessageWithoutButtonAttachmentIncomingWebhookAsync(leave,
                                    await _attachmentRepository.UserAccessTokenAsync(user.UserName), message,
                                    temporaryLeaveRequestDetail.EmployeeId);
                            }
                        }
                        break;
                }
                return _stringConstant.ThankYou;
            }
            // if user answer is not in SendEmailConfirmation format
            else
                return string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat,
                    _stringConstant.SendTaskMailConfirmationErrorMessage,
                    Environment.NewLine, (await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.SendLeaveMail)).QuestionStatement);
        }

        /// <summary>
        /// Method to get leave list - SS
        /// </summary>
        /// <param name="userId">user's Id</param>
        /// <param name="empployeeName">employee name who's leave will displayed</param>
        /// <returns>reply to be send</returns>
        private async Task<string> GetLeaveListAsync(string userId, string employeeName)
        {
            string replyText = string.Empty;
            List<LeaveRequest> leaves = new List<LeaveRequest>();
            // if null then user want to get own leave list
            if (employeeName != null)
            {
                // user's username
                var username = (await _userManager.FindByIdAsync(userId)).UserName;
                // check if user is admin or not
                if (await _oauthCallRepository.UserIsAdminAsync(userId, (await _attachmentRepository.UserAccessTokenAsync(username))))
                {
                    // employee slack details whose leave detail user wan to get
                    var employeeSlackDetails = await _slackUserRepository.GetBySlackNameAsync(employeeName);
                    // check if user exist or not
                    if (employeeSlackDetails != null)
                    {
                        // employee slack details whose leave detail user wan to get
                        var employee = await _userManager.Users.FirstOrDefaultAsync(x => x.SlackUserId == employeeSlackDetails.UserId);
                        // check if user exist or not
                        if (employee != null)
                        {
                            // leave list of employee
                            leaves = _leaveRequestRepository.LeaveListByUserId(employee.Id).ToList();
                            // if exist then format the message
                            if (leaves.Any())
                                replyText = GetLeaveListMessageByLeaveList(leaves);
                            // else message will be no record found for employee
                            else
                                replyText = string.Format(_stringConstant.LeaveListForOtherErrorMessage, employeeName);
                        }
                        // message to ask employee to do login with promact oauth server
                        else
                            replyText = _stringConstant.MessageToRequestToAddToSlackOtherUser;
                    }
                    // message to ask employee to do add to slack
                    else
                        replyText = string.Format(_stringConstant.UserNotFoundRequestToAddToSlackOtherUser, employeeName);
                }
                // if user is not admin then unauthorize message will be send
                else
                    replyText = _stringConstant.UserIsNotAllowedToListOtherLeaveDetailsMessage;
            }
            // if user want to get own leave list
            else
            {
                // leave list of user
                leaves = _leaveRequestRepository.LeaveListByUserId(userId).ToList();
                // if exist then format the message
                if (leaves.Any())
                    replyText = GetLeaveListMessageByLeaveList(leaves);
                // else message will be no record found for user
                else
                    replyText = _stringConstant.LeaveDoesNotExistErrorMessage;
            }
            return replyText;
        }

        /// <summary>
        /// Method to cancel leave by id - SS
        /// </summary>
        /// <param name="userId">user's Id</param>
        /// <param name="leaveIdStringValue">leave id in string</param>
        /// <returns>reply text to be send</returns>
        private async Task<string> LeaveCancelAsync(string userId, string leaveIdStringValue)
        {
            int leaveId;
            // try to parse answer to leave Id
            if (int.TryParse(leaveIdStringValue, out leaveId))
            {
                // to get leave details
                var leave = await _leaveRequestRepository.LeaveByIdAsync(leaveId);
                // if do exist
                if (leave != null)
                {
                    // leave applied employee and user want to cancel leave is same or not verification
                    if (leave.EmployeeId == userId)
                    {
                        // if leave status is pending then only can cancel it
                        if (leave.Status == Condition.Pending)
                        {
                            leave.Status = Condition.Cancel;
                            await _leaveRequestRepository.UpdateLeaveAsync(leave);
                            return _stringConstant.LeaveCancelSuccessfulMessage;
                        }
                        // if leave is not pending then message of already update will be send
                        else
                            return string.Format(_stringConstant.LeaveStatusAlreadyUpdatedErrorMessge, leave.Id, leave.Reason, leave.Status);
                    }
                    // leave applied employee and user want to cancel leave is not same
                    else
                        return string.Format(_stringConstant.LeaveCancelUnAuthorizeErrorMessage, leaveId);
                }
                // if leave does not exist
                else
                    return string.Format(_stringConstant.LeaveDoesNotExistErrorMessageWithLeaveIdFormat, leaveId);
            }
            // Improper leave id format error message
            else
                return _stringConstant.LeaveCancelCommandErrorFormatMessage;
        }

        /// <summary>
        /// Method to get leave status of last leave applied - SS
        /// </summary>
        /// <param name="userId">user's Id</param>
        /// <returns>reply to be send</returns>
        private string GetLeaveStatus(string userId)
        {
            string replyText = string.Empty;
            // get last leave of user
            var leave = (_leaveRequestRepository.LeaveListByUserId(userId).ToList()).LastOrDefault();
            if (leave != null)
            {
                // message format for leave
                replyText = string.Format(_stringConstant.ReplyTextForCasualLeaveList, leave.Id,
                            leave.Reason, leave.FromDate.ToShortDateString(),
                            leave.EndDate.Value.ToShortDateString(), leave.Status,
                            Environment.NewLine);
            }
            // if any leave doesnot exist
            else
                replyText = _stringConstant.LeaveDoesNotExistErrorMessage;
            return replyText;
        }

        /// <summary>
        /// Method to get leave balance for user - SS
        /// </summary>
        /// <param name="userId">user's Id</param>
        /// <returns>reply to be send</returns>
        private async Task<string> GetUserLeaveBalanceAsync(string userId)
        {
            // user's username
            var username = (await _userManager.FindByIdAsync(userId)).UserName;
            // leave allowed detail of user from ouath server 
            LeaveAllowed leaveAllowed = await _oauthCallRepository.AllowedLeave(userId, (await _attachmentRepository.UserAccessTokenAsync(username)));
            // number of leave taken
            LeaveAllowed leaveTaken = _leaveRequestRepository.NumberOfLeaveTaken(userId);
            // calcualtion
            double casualLeaveTaken = leaveTaken.CasualLeave;
            double sickLeaveTaken = leaveTaken.SickLeave;
            double casualLeaveLeft = leaveAllowed.CasualLeave - casualLeaveTaken;
            double sickLeaveLeft = leaveAllowed.SickLeave - sickLeaveTaken;
            // leave balance message are formatted
            string replyText = string.Format(_stringConstant.ReplyTextForCasualLeaveBalance, casualLeaveTaken,
                leaveAllowed.CasualLeave, Environment.NewLine, casualLeaveLeft);
            replyText += string.Format(_stringConstant.ReplyTextForSickLeaveBalance, sickLeaveTaken,
                leaveAllowed.SickLeave, Environment.NewLine, sickLeaveLeft);
            return replyText;
        }

        /// <summary>
        /// Method to check leave's first date is not beyond toCheckDate. Back date checking - SS
        /// </summary>
        /// <param name="firstDate">date with which date to be check</param>
        /// <param name="toCheckDate">date to be check</param>
        /// <returns>true or false</returns>
        private bool LeaveDateValid(DateTime firstDate, DateTime toCheckDate)
        {
            var valid = firstDate.Date.CompareTo(toCheckDate.Date);
            if (valid <= 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Method to check any leave exist on this date - SS
        /// </summary>
        /// <param name="startDate">start date to be check</param>
        /// <param name="userId">user's Id</param>
        /// <param name="isStartDate">comparisaion date is start date or end date</param>
        /// <param name="endDate">end date is to compare else will recieve null</param>
        /// <returns>boolean value of comparsion</returns>
        private bool DuplicateLeaveExist(DateTime startDate, string userId, bool isStartDate, DateTime? endDate)
        {
            bool leaveExist = false;
            // if date is start date then goes here
            if (isStartDate)
            {
                // get all leave list of user
                var leaves = _leaveRequestRepository.LeaveListByUserId(userId).ToList();
                foreach (var leave in leaves)
                {
                    if (leave.EndDate.HasValue)
                    {
                        // check date provided by user, in that already leave exist or not
                        if (startDate.Date >= leave.FromDate.Date && startDate.Date <= leave.EndDate.Value.Date)
                        {
                            leaveExist = true;
                            break;
                        }
                    }
                    else
                    {
                        // check date provided by user, in that already leave exist or not
                        if (startDate.Date == leave.FromDate.Date)
                        {
                            leaveExist = true;
                            break;
                        }
                    }
                }
            }
            //if date is end date then goes here
            else
            {
                leaveExist = DuplicateLeaveExistWithStartAndEndDate(startDate, endDate.Value, userId);
            }
            return leaveExist;
        }

        /// <summary>
        /// Method to update sick leave by admin - SS
        /// </summary>
        /// <param name="leaveValue">command send by user to update sick leave</param>
        /// <param name="userId">admin user's Id</param>
        /// <returns>reply to be send</returns>
        private async Task<string> UpdateSickLeaveByAdminAsync(List<string> leaveValue, string userId)
        {
            if (leaveValue.Count == 5)
            {
                // user' username
                var username = (await _userManager.FindByIdAsync(userId)).UserName;
                // check if user is admin then goes here
                if (await _oauthCallRepository.UserIsAdminAsync(userId, (await _attachmentRepository.UserAccessTokenAsync(username))))
                {
                    int leaveId;
                    string replyText = string.Empty;
                    // try to parse user answer to leave id 
                    if (int.TryParse(leaveValue[2], out leaveId))
                    {
                        // check if leave exist or not
                        var leave = await _leaveRequestRepository.LeaveByIdAsync(leaveId);
                        if (leave != null)
                        {
                            // check if leave is sick leave or not
                            if (leave.Type == LeaveType.sl)
                            {
                                DateTime rejoinDate, endDate;
                                // get date format of current culture
                                string dateFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
                                // try to parse if answer is as same format of current culture 
                                if (DateTime.TryParseExact(leaveValue[4], dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out rejoinDate) &&
                                    DateTime.TryParseExact(leaveValue[3], dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
                                {
                                    // check leave's first date is not beyond toCheckDate. Back date checking
                                    if (LeaveDateValid(leave.FromDate, endDate))
                                    {
                                        leave.EndDate = null;
                                        leave.RejoinDate = null;
                                        await _leaveRequestRepository.UpdateLeaveAsync(leave);
                                        // check any leave exist on this date
                                        if (!DuplicateLeaveExist(leave.FromDate, leave.EmployeeId, false, endDate))
                                        {
                                            // check leave's first date is not beyond toCheckDate. Back date checking
                                            if (LeaveDateValid(endDate, rejoinDate))
                                            {
                                                leave.EndDate = endDate;
                                                leave.RejoinDate = rejoinDate;
                                                await _leaveRequestRepository.UpdateLeaveAsync(leave);
                                                var employee = await _userManager.FindByIdAsync(leave.EmployeeId);
                                                // reply to be send in slack to user, management and TL
                                                replyText = string.Format(_stringConstant.ReplyTextForSickLeaveUpdate
                                                    , (await _slackUserRepository.GetByIdAsync(employee.SlackUserId)).Name, leave.FromDate.ToShortDateString(),
                                                    leave.EndDate.Value.ToShortDateString(), leave.Reason, leave.RejoinDate.Value.ToShortDateString());
                                                // send slack message to user, management and TL
                                                await _clientRepository.SendSickLeaveMessageToUserIncomingWebhookAsync(leave, username, replyText,
                                                    _mapper.Map<ApplicationUser, User>(employee));
                                                // reply with leave updated confirmation
                                                return string.Format(_stringConstant.LeaveUpdateMessage, leave.Id, Environment.NewLine);
                                            }
                                            // back date error message
                                            else
                                                return string.Format(_stringConstant.RejoinDateBeyondEndDateErrorMessage, Environment.NewLine,
                                                    _stringConstant.LeaveUpdateFormatMessage);
                                        }
                                        // duplicate date error message
                                        else
                                            return _stringConstant.LeaveAlreadyExistOnSameDate;
                                    }
                                    // end back date error message
                                    else
                                        return string.Format(_stringConstant.EndDateBeyondStartDateErrorMessage, Environment.NewLine,
                                            _stringConstant.LeaveUpdateFormatMessage);
                                }
                                // date format error message
                                else
                                {
                                    var dateErrorMessage = string.Format(_stringConstant.DateFormatErrorMessage, dateFormat);
                                    return string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat, dateErrorMessage, Environment.NewLine,
                                        _stringConstant.LeaveUpdateFormatMessage);
                                }
                            }
                            // sick leave doesnot exist for leave id error message 
                            else
                                return string.Format(_stringConstant.SickLeaveDoesnotExist, leaveId);
                        }
                        // leave doesnot exist error message
                        else
                            return string.Format(_stringConstant.LeaveDoesNotExistErrorMessageWithLeaveIdFormat, leaveId);
                    }
                    // leave id format error message
                    else
                        return string.Format(_stringConstant.LeaveUpdateLeaveIdErrorFormatErrorMessage, Environment.NewLine);
                }
                // user want to update leave is not admin error message
                else
                    return _stringConstant.AdminErrorMessageUpdateSickLeave;
            }
            else
                return string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat, _stringConstant.LeaveUpdateFormatErrorMessage,
                    Environment.NewLine, _stringConstant.LeaveUpdateFormatMessage);
        }

        /// <summary>
        /// Method to tranform leave list to string message - SS
        /// </summary>
        /// <param name="leaves">leave list</param>
        /// <returns>leave list in message format</returns>
        private string GetLeaveListMessageByLeaveList(List<LeaveRequest> leaves)
        {
            string replyText = string.Empty;
            foreach (var leave in leaves)
                // Suffex differnet for leave type
                if (leave.Type == LeaveType.cl)
                    replyText += string.Format(_stringConstant.ReplyTextForCasualLeaveList, leave.Id,
                                leave.Reason, leave.FromDate.ToShortDateString(),
                                leave.EndDate.Value.ToShortDateString(), leave.Status, Environment.NewLine);
                else
                {
                    if (leave.EndDate.HasValue)
                        replyText += string.Format(_stringConstant.ReplyTextForSickLeaveList, leave.Id, leave.Reason,
                            leave.FromDate.ToShortDateString(), leave.EndDate.Value.ToShortDateString(), leave.Status, Environment.NewLine);
                    else
                        replyText += string.Format(_stringConstant.ReplyTextForSickLeaveListWithoutEndDate, leave.Id, leave.Reason,
                            leave.FromDate.ToShortDateString(), leave.Status, Environment.NewLine);
                }
            return replyText;
        }

        /// <summary>
        /// Method to check more than one leave cannot be applied on that date - SS
        /// </summary>
        /// <param name="userId">User's Id</param>
        /// <param name="startDate">leave start date</param>
        /// <param name="endDate">leave end date</param>
        /// <returns>true or false</returns>
        private bool DuplicateLeaveExistWithStartAndEndDate(DateTime startDate, DateTime endDate, string userId)
        {
            int valid = -1;
            bool validIndicator = false;
            LeaveRequest lastLeave = new LeaveRequest();
            IEnumerable<LeaveRequest> allLeave = _leaveRequestRepository.LeaveListByUserIdOnlyApprovedAndPending(userId).Result;
            if (allLeave != null)
            {
                foreach (var leave in allLeave)
                {
                    if (leave.EndDate.HasValue)
                        valid = leave.EndDate.Value.Date.CompareTo(startDate.Date);
                    else
                        valid = leave.FromDate.Date.CompareTo(endDate.Date);
                    switch (valid)
                    {
                        case -1:
                            {
                                validIndicator = false;
                            }
                            break;
                        case 0:
                            {
                                validIndicator = true;
                            }
                            break;
                        case 1:
                            {
                                valid = leave.FromDate.Date.CompareTo(endDate.Date);
                                if (valid == 1)
                                    validIndicator = false;
                                else
                                    validIndicator = true;
                            }
                            break;
                    }
                    if (validIndicator)
                        break;
                }
            }
            return validIndicator;
        }
        #endregion
    }
}