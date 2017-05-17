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
            answer = answer.Replace('”', '"');
            answer = answer.Replace('“', '"');
            List<string> slackText = _attachmentRepository.SlackText(answer);
            var user = await GetUserDetailsFromOAuthAsync(slackUserId);
            if (user != null)
            {
                if (slackText[0] == _stringConstant.Leave)
                {
                    SlackAction slackAction;
                    if (Enum.TryParse(slackText[1], out slackAction))
                    {
                        switch (slackAction)
                        {
                            case SlackAction.apply:
                                replyText = await StartLeaveProcessAsync(user.Id, answer);
                                break;
                            case SlackAction.list:
                                {
                                    if (slackText.Count > 2)
                                        replyText = await GetLeaveListAsync(user.Id, slackText[2]);
                                    else
                                        replyText = await GetLeaveListAsync(user.Id, null);
                                }
                                break;
                            case SlackAction.cancel:
                                {
                                    if (slackText.Count > 2)
                                        replyText = await LeaveCancelAsync(user.Id, slackText[2]);
                                    else
                                        replyText = _stringConstant.IncorrectLeaveCancelCommandMessage;
                                }
                                break;
                            case SlackAction.status:
                                replyText = GetLeaveStatus(user.Id);
                                break;
                            case SlackAction.balance:
                                replyText = await GetUserLeaveBalanceAsync(user.Id);
                                break;
                            case SlackAction.update:
                                replyText = await UpdateSickLeaveByAdminAsync(slackText, user.Id);
                                break;
                            default:
                                {
                                    if (await _oauthCallRepository.UserIsAdminAsync(user.Id, (await _attachmentRepository.UserAccessTokenAsync(user.UserName))))
                                        replyText = string.Format(_stringConstant.FirstAndSecondIndexStringFormat,
                                            _stringConstant.LeaveHelpBotCommands, _stringConstant.LeaveUpdateFormatMessage);
                                    else
                                        replyText = _stringConstant.LeaveHelpBotCommands;
                                }
                                break;
                        }
                    }
                    else
                        replyText = _stringConstant.ProperActionErrorMessage;
                }
                else
                    replyText = await LeaveApplyProcessAsync(answer, user.Id);
            }
            else
                replyText = _stringConstant.SorryYouCannotApplyLeave;
            return replyText;
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
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.SlackUserId == slackUserId);
            if (user != null)
            {
                if (!_temporaryLeaveRequestDetailDataRepository.Any(x => x.EmployeeId == user.Id))
                    user = _mapper.Map<User, ApplicationUser>(await _oauthCallRepository.GetUserByUserIdAsync(user.Id,
                        (await _attachmentRepository.UserAccessTokenAsync(user.UserName))));
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
            if (!_temporaryLeaveRequestDetailDataRepository.Any(x => x.EmployeeId == userId))
            {
                var nextQuestion = await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.LeaveType);
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
            var leave = await _temporaryLeaveRequestDetailDataRepository.FirstOrDefaultAsync(x => x.EmployeeId == userId);
            if (leave != null)
            {
                var previousQuestion = await _botQuestionRepository.FindByIdAsync(leave.QuestionId);
                switch (previousQuestion.OrderNumber)
                {
                    case QuestionOrder.LeaveType:
                        replyText = await AddLeaveTypeDetailsAsync(answer, leave);
                        break;
                    case QuestionOrder.Reason:
                        replyText = await AddLeaveReasonDetailsAsync(answer, leave);
                        break;
                    case QuestionOrder.FromDate:
                        replyText = await AddLeaveStartDateDetailsAsync(answer, leave);
                        break;
                    case QuestionOrder.EndDate:
                        replyText = await AddLeaveEndDateDetailsAsync(answer, leave);
                        break;
                    case QuestionOrder.RejoinDate:
                        replyText = await AddLeaveRejoinDateDetailsAsync(answer, leave);
                        break;
                    case QuestionOrder.SendLeaveMail:
                        replyText = await AddLeaveSendMailDetailsAsync(answer, leave);
                        break;
                }
            }
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
            if (Enum.TryParse(answer, out leaveType))
            {
                temporaryLeaveRequestDetail.Type = leaveType;
                var nextQuestion = await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.Reason);
                temporaryLeaveRequestDetail.QuestionId = nextQuestion.Id;
                await UpdateTemporaryLeaveDetailsAsync(temporaryLeaveRequestDetail);
                return nextQuestion.QuestionStatement;
            }
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
                var nextQuestion = await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.FromDate);
                temporaryLeaveRequestDetail.QuestionId = nextQuestion.Id;
                await UpdateTemporaryLeaveDetailsAsync(temporaryLeaveRequestDetail);
                return nextQuestion.QuestionStatement;
            }
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
            string dateFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
            if (DateTime.TryParseExact(answer, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
            {
                if (!DuplicateLeaveExist(startDate, temporaryLeaveRequestDetail.EmployeeId))
                {
                    if (LeaveDateValid(DateTime.UtcNow, startDate))
                    {
                        Question nextQuestion;
                        temporaryLeaveRequestDetail.FromDate = startDate;
                        if (temporaryLeaveRequestDetail.Type == LeaveType.cl)
                            nextQuestion = await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.EndDate);
                        else
                        {
                            nextQuestion = await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.SendLeaveMail);
                            temporaryLeaveRequestDetail.EndDate = startDate;
                            temporaryLeaveRequestDetail.RejoinDate = startDate.AddDays(1);
                            temporaryLeaveRequestDetail.Status = Condition.Approved;
                        }
                        temporaryLeaveRequestDetail.QuestionId = nextQuestion.Id;
                        await UpdateTemporaryLeaveDetailsAsync(temporaryLeaveRequestDetail);
                        return nextQuestion.QuestionStatement;
                    }
                    else
                        return _stringConstant.BackDateErrorMessage;
                }
                else
                    return _stringConstant.LeaveAlreadyExistOnSameDate;
            }
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
            string dateFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
            if (DateTime.TryParseExact(answer, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
            {
                if (LeaveDateValid(temporaryLeaveRequestDetail.FromDate.Value, endDate))
                {
                    if (!DuplicateLeaveExist(endDate, temporaryLeaveRequestDetail.EmployeeId))
                    {
                        temporaryLeaveRequestDetail.EndDate = endDate;
                        var nextQuestion = await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.RejoinDate);
                        temporaryLeaveRequestDetail.QuestionId = nextQuestion.Id;
                        await UpdateTemporaryLeaveDetailsAsync(temporaryLeaveRequestDetail);
                        return nextQuestion.QuestionStatement;
                    }
                    else
                        return _stringConstant.LeaveAlreadyExistOnSameDate;
                }
                else
                    return string.Format(_stringConstant.EndDateBeyondStartDateErrorMessage, Environment.NewLine,
                        (await _botQuestionRepository.FindByIdAsync(temporaryLeaveRequestDetail.QuestionId)).QuestionStatement);
            }
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
            string dateFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
            if (DateTime.TryParseExact(answer, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out rejoinDate))
            {
                if (LeaveDateValid(temporaryLeaveRequestDetail.EndDate.Value, rejoinDate))
                {
                    temporaryLeaveRequestDetail.RejoinDate = rejoinDate;
                    var nextQuestion = await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.SendLeaveMail);
                    temporaryLeaveRequestDetail.QuestionId = nextQuestion.Id;
                    await UpdateTemporaryLeaveDetailsAsync(temporaryLeaveRequestDetail);
                    return nextQuestion.QuestionStatement;
                }
                else
                    return string.Format(_stringConstant.RejoinDateBeyondEndDateErrorMessage, Environment.NewLine,
                        (await _botQuestionRepository.FindByIdAsync(temporaryLeaveRequestDetail.QuestionId)).QuestionStatement);
            }
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
            if (Enum.TryParse(answer, out confirmation))
            {
                switch (confirmation)
                {
                    case SendEmailConfirmation.yes:
                        {
                            var user = (await _userManager.FindByIdAsync(temporaryLeaveRequestDetail.EmployeeId));
                            var slackUserName = (await _slackUserRepository.GetByIdAsync(user.SlackUserId)).Name;
                            var leave = _mapper.Map<TemporaryLeaveRequestDetail, LeaveRequest>(temporaryLeaveRequestDetail);
                            await _leaveRequestRepository.ApplyLeaveAsync(leave);
                            _temporaryLeaveRequestDetailDataRepository.Delete(temporaryLeaveRequestDetail.Id);
                            await _temporaryLeaveRequestDetailDataRepository.SaveChangesAsync();
                            if (temporaryLeaveRequestDetail.Type == LeaveType.cl)
                            {
                                var message = _attachmentRepository.ReplyText(slackUserName, leave);
                                await _clientRepository.SendMessageWithAttachmentIncomingWebhookAsync(leave,
                                    await _attachmentRepository.UserAccessTokenAsync(user.UserName), message,
                                    temporaryLeaveRequestDetail.EmployeeId);
                            }
                            else
                            {
                                var message = _attachmentRepository.ReplyTextSick(slackUserName, leave);
                                await _clientRepository.SendMessageWithoutButtonAttachmentIncomingWebhookAsync(leave,
                                    await _attachmentRepository.UserAccessTokenAsync(user.UserName), message,
                                    temporaryLeaveRequestDetail.EmployeeId);
                            }
                        }
                        break;
                }
                return _stringConstant.ThankYou;
            }
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
            if (employeeName != null)
            {
                var username = (await _userManager.FindByIdAsync(userId)).UserName;
                var employeeSlackDetails = await _slackUserRepository.GetBySlackNameAsync(employeeName);
                if (employeeSlackDetails != null)
                {
                    var employee = await _userManager.Users.FirstOrDefaultAsync(x => x.SlackUserId == employeeSlackDetails.UserId);
                    if (employee != null)
                    {
                        if (await _oauthCallRepository.UserIsAdminAsync(userId, (await _attachmentRepository.UserAccessTokenAsync(username))))
                        {
                            leaves = _leaveRequestRepository.LeaveListByUserId(employee.Id).ToList();
                            if (leaves.Any())
                                replyText = GetLeaveListMessageByLeaveList(leaves);
                            else
                                replyText = string.Format(_stringConstant.LeaveListForOtherErrorMessage, employeeName);
                        }
                        else
                            replyText = _stringConstant.UserIsNotAllowedToListOtherLeaveDetailsMessage;
                    }
                    else
                        replyText = _stringConstant.MessageToRequestToAddToSlackOtherUser;
                }
                else
                    replyText = string.Format(_stringConstant.UserNotFoundRequestToAddToSlackOtherUser, employeeName);
            }
            else
            {
                leaves = _leaveRequestRepository.LeaveListByUserId(userId).ToList();
                if (leaves.Any())
                    replyText = GetLeaveListMessageByLeaveList(leaves);
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
            if (int.TryParse(leaveIdStringValue, out leaveId))
            {
                var leave = await _leaveRequestRepository.LeaveByIdAsync(leaveId);
                if (leave != null)
                {
                    if (leave.EmployeeId == userId)
                    {
                        if (leave.Status == Condition.Pending)
                        {
                            leave.Status = Condition.Cancel;
                            await _leaveRequestRepository.UpdateLeaveAsync(leave);
                            return _stringConstant.LeaveCancelSuccessfulMessage;
                        }
                        else
                            return string.Format(_stringConstant.LeaveStatusAlreadyUpdatedErrorMessge, leave.Id, leave.Reason, leave.Status);
                    }
                    else
                        return string.Format(_stringConstant.LeaveCancelUnAuthorizeErrorMessage, leaveId);
                }
                else
                    return string.Format(_stringConstant.LeaveDoesNotExistErrorMessageWithLeaveIdFormat, leaveId);
            }
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
            var leave = (_leaveRequestRepository.LeaveListByUserId(userId).ToList()).LastOrDefault();
            if (leave != null)
            {
                replyText = string.Format(_stringConstant.ReplyTextForCasualLeaveList, leave.Id,
                            leave.Reason, leave.FromDate.ToShortDateString(),
                            leave.EndDate.Value.ToShortDateString(), leave.Status,
                            Environment.NewLine);
            }
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
            var username = (await _userManager.FindByIdAsync(userId)).UserName;
            LeaveAllowed leaveAllowed = await _oauthCallRepository.AllowedLeave(userId, (await _attachmentRepository.UserAccessTokenAsync(username)));
            LeaveAllowed leaveTaken = _leaveRequestRepository.NumberOfLeaveTaken(userId);
            double casualLeaveTaken = leaveTaken.CasualLeave;
            double sickLeaveTaken = leaveTaken.SickLeave;
            double casualLeaveLeft = leaveAllowed.CasualLeave - casualLeaveTaken;
            double sickLeaveLeft = leaveAllowed.SickLeave - sickLeaveTaken;
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
        /// <param name="date">date to be check</param>
        /// <param name="userId">user's Id</param>
        /// <returns>boolean value of comparsion</returns>
        private bool DuplicateLeaveExist(DateTime date, string userId)
        {
            bool leaveExist = false;
            var leaves = _leaveRequestRepository.LeaveListByUserId(userId).ToList();
            foreach (var leave in leaves)
            {
                var res = leave.FromDate.CompareTo(date);
                if (date.Date >= leave.FromDate.Date && date.Date <= leave.EndDate.Value.Date)
                {
                    leaveExist = true;
                    break;
                }
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
            var username = (await _userManager.FindByIdAsync(userId)).UserName;
            if (await _oauthCallRepository.UserIsAdminAsync(userId, (await _attachmentRepository.UserAccessTokenAsync(username))))
            {
                int leaveId;
                string replyText = string.Empty;
                if (int.TryParse(leaveValue[2], out leaveId))
                {
                    var leave = await _leaveRequestRepository.LeaveByIdAsync(leaveId);
                    if (leave != null)
                    {
                        if (leave.Type == LeaveType.sl)
                        {
                            var leaveTemporaryData = _mapper.Map<LeaveRequest, TemporaryLeaveRequestDetail>(leave);
                            leaveTemporaryData.QuestionId = (await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.EndDate)).Id;
                            _temporaryLeaveRequestDetailDataRepository.Insert(leaveTemporaryData);
                            await _temporaryLeaveRequestDetailDataRepository.SaveChangesAsync();
                            var nextQuestion = await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.RejoinDate);
                            replyText = await AddLeaveEndDateDetailsAsync(leaveValue[3], leaveTemporaryData);
                            if (replyText == nextQuestion.QuestionStatement)
                            {
                                leaveTemporaryData.QuestionId = (await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.EndDate)).Id;
                                nextQuestion = await GetLeaveQuestionDetailsByOrderAsync(QuestionOrder.SendLeaveMail);
                                replyText = await AddLeaveRejoinDateDetailsAsync(leaveValue[4], leaveTemporaryData);
                                if (replyText == nextQuestion.QuestionStatement)
                                {
                                    leave.EndDate = leaveTemporaryData.EndDate.Value;
                                    leave.RejoinDate = leaveTemporaryData.RejoinDate.Value;
                                    await _leaveRequestRepository.UpdateLeaveAsync(leave);
                                    var employee = await _userManager.FindByIdAsync(leave.EmployeeId);
                                    replyText = string.Format(_stringConstant.ReplyTextForSickLeaveUpdate
                                        , (await _slackUserRepository.GetByIdAsync(employee.SlackUserId)).Name, leave.FromDate.ToShortDateString(),
                                        leave.EndDate.Value.ToShortDateString(), leave.Reason, leave.RejoinDate.Value.ToShortDateString());
                                    await _clientRepository.SendSickLeaveMessageToUserIncomingWebhookAsync(leave, username, replyText,
                                        _mapper.Map<ApplicationUser, User>(employee));
                                    return string.Format(_stringConstant.LeaveUpdateMessage, leaveTemporaryData.Id, Environment.NewLine);
                                }
                                else if (replyText.Contains(_stringConstant.RejoinDateMessage))
                                {
                                    replyText = string.Format(_stringConstant.RejoinDateBeyondEndDateErrorMessage, Environment.NewLine,
                                        _stringConstant.LeaveUpdateFormatMessage);
                                    return replyText;
                                }
                                else
                                {
                                    string dateFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
                                    var dateErrorMessage = string.Format(_stringConstant.DateFormatErrorMessage, dateFormat);
                                    replyText = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat, dateErrorMessage,
                                        Environment.NewLine, _stringConstant.LeaveUpdateFormatMessage);
                                    return replyText;
                                }
                            }
                            else if (replyText.Contains(_stringConstant.EndDateMessage))
                            {
                                replyText = string.Format(_stringConstant.EndDateBeyondStartDateErrorMessage, Environment.NewLine,
                                    _stringConstant.LeaveUpdateFormatMessage);
                                return replyText;
                            }
                            else if (replyText.Contains(_stringConstant.DateFormatError))
                            {
                                string dateFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
                                var dateErrorMessage = string.Format(_stringConstant.DateFormatErrorMessage, dateFormat);
                                replyText = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat, dateErrorMessage,
                                    Environment.NewLine, _stringConstant.LeaveUpdateFormatMessage);
                                return replyText;
                            }
                            else
                                return replyText;
                        }
                        else
                            return string.Format(_stringConstant.SickLeaveDoesnotExist, leaveId);
                    }
                    else
                        return string.Format(_stringConstant.LeaveDoesNotExistErrorMessageWithLeaveIdFormat, leaveId);
                }
                else
                    return string.Format(_stringConstant.LeaveUpdateLeaveIdErrorFormatErrorMessage, Environment.NewLine);
            }
            else
                return _stringConstant.AdminErrorMessageUpdateSickLeave;
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
                if (leave.Type == LeaveType.cl)
                    replyText += string.Format(_stringConstant.ReplyTextForCasualLeaveList, leave.Id,
                                leave.Reason, leave.FromDate.ToShortDateString(),
                                leave.EndDate.Value.ToShortDateString(), leave.Status, Environment.NewLine);
                else
                    replyText += string.Format(_stringConstant.ReplyTextForSickLeaveList, leave.Id, leave.Reason,
                        leave.FromDate.ToShortDateString(), leave.EndDate.Value.ToShortDateString(), leave.Status, Environment.NewLine);
            return replyText;
        }
        #endregion
    }
}