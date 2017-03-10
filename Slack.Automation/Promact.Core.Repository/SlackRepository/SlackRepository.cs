using Promact.Core.Repository.LeaveRequestRepository;
using Promact.Core.Repository.OauthCallsRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Promact.Core.Repository.Client;
using Promact.Core.Repository.AttachmentRepository;
using System.Linq;
using System.Globalization;
using Promact.Erp.DomainModel.DataRepository;
using System.Net.Mail;
using Promact.Erp.Util.StringConstants;
using Promact.Core.Repository.SlackUserRepository;
using System.Threading;
using Promact.Core.Repository.EmailServiceTemplateRepository;
using Promact.Erp.Util.Email;
using Promact.Erp.Util.ExceptionHandler;
using NLog;

namespace Promact.Core.Repository.SlackRepository
{
    public class SlackRepository : ISlackRepository
    { 
        #region Private Variable
        private readonly IOauthCallsRepository _oauthCallsRepository;
        private readonly ISlackUserRepository _slackUserRepository;
        private readonly ILeaveRequestRepository _leaveRepository;
        private readonly IClient _clientRepository;
        private readonly IStringConstantRepository _stringConstant;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IRepository<ApplicationUser> _userManagerRepository;
        string replyText = null;
        private readonly IRepository<IncomingWebHook> _incomingWebHookRepository;
        private readonly IEmailServiceTemplateRepository _emailTemplateRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger _logger;
        #endregion

        #region Constructor
        public SlackRepository(ILeaveRequestRepository leaveRepository, IOauthCallsRepository oauthCallsRepository,
            ISlackUserRepository slackUserRepository, IClient clientRepository, IStringConstantRepository stringConstant,
            IAttachmentRepository attachmentRepository, IRepository<ApplicationUser> userManagerRepository,
            IRepository<IncomingWebHook> incomingWebHookRepository, IEmailServiceTemplateRepository emailTemplateRepository,
            IEmailService emailService)
        {
            _oauthCallsRepository = oauthCallsRepository;
            _leaveRepository = leaveRepository;
            _clientRepository = clientRepository;
            _stringConstant = stringConstant;
            _attachmentRepository = attachmentRepository;
            _userManagerRepository = userManagerRepository;
            _slackUserRepository = slackUserRepository;
            _incomingWebHookRepository = incomingWebHookRepository;
            _emailTemplateRepository = emailTemplateRepository;
            _emailService = emailService;
            _logger = LogManager.GetLogger("SlackRepository");
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method to get leave Updated from slack button response
        /// </summary>
        /// <param name="leaveResponse">leave update response from slack</param>
        public async Task UpdateLeaveAsync(SlashChatUpdateResponse leaveResponse)
        {
            try
            {
                _logger.Debug("UpdateLeaveAsync Leave update method");
                // method to get leave by its id
                LeaveRequest leave = await _leaveRepository.LeaveByIdAsync(Convert.ToInt32(leaveResponse.CallbackId));
                _logger.Debug("UpdateLeaveAsync leave applied by : " + leave.EmployeeId);
                ApplicationUser user = await _userManagerRepository.FirstOrDefaultAsync(x => x.Id == leave.EmployeeId);
                _logger.Debug("UpdateLeaveAsync User name : " + user.UserName);
                SlackUserDetailAc slackUser = await _slackUserRepository.GetByIdAsync(user.SlackUserId);
                _logger.Debug("UpdateLeaveAsync User slack name : " + slackUser.Name);
                ApplicationUser updaterUser = await _userManagerRepository.FirstOrDefaultAsync(x => x.SlackUserId == leaveResponse.User.Id);
                if (updaterUser != null)
                {
                    _logger.Debug("UpdateLeaveAsync User want to update the leave : " + updaterUser.UserName);
                    // only pending status can be modified
                    if (leave.Status == Condition.Pending)
                    {
                        if (leaveResponse.Actions[0].Value == _stringConstant.Approved)
                        {
                            leave.Status = Condition.Approved;
                        }
                        else
                        {
                            leave.Status = Condition.Rejected;
                        }
                        await _leaveRepository.UpdateLeaveAsync(leave);
                        _logger.Debug("UpdateLeaveAsync leave updated successfully");
                        replyText = string.Format(_stringConstant.CasualLeaveUpdateMessageForUser,
                                    leave.Id, leave.FromDate.ToShortDateString(), leave.EndDate.Value.ToShortDateString(),
                                    leave.Reason, leave.Status, leaveResponse.User.Name);
                        IncomingWebHook incomingWebHook = await _incomingWebHookRepository.FirstOrDefaultAsync(x => x.UserId == slackUser.UserId);
                        _logger.Debug("UpdateLeaveAsync user incoming webhook is null : " + string.IsNullOrEmpty(incomingWebHook.IncomingWebHookUrl));
                        // Used to send slack message to the user about leave updation
                        _logger.Debug("UpdateLeaveAsync Client repository - UpdateMessageAsync");
                        await _clientRepository.UpdateMessageAsync(incomingWebHook.IncomingWebHookUrl, replyText);
                        // Used to send email to the user about leave updation
                        _logger.Debug("UpdateLeaveAsync Email sending");
                        EmailApplication email = new EmailApplication();
                        email.To = new List<string>();
                        email.Body = _emailTemplateRepository.EmailServiceTemplateLeaveUpdate(leave);
                        email.From = updaterUser.Email;
                        email.To.Add(user.Email);
                        email.Subject = string.Format(_stringConstant.LeaveUpdateEmailStringFormat, _stringConstant.Leave, leave.Status);
                        replyText = string.Format(_stringConstant.ReplyTextForUpdateLeave, leave.Status, slackUser.Name,
                        leave.FromDate.ToShortDateString(), leave.EndDate.Value.ToShortDateString(), leave.Reason,
                        leave.RejoinDate.Value.ToShortDateString());
                        _emailService.Send(email);
                        _logger.Debug("UpdateLeaveAsync Email successfully send");
                    }
                    else
                    {
                        _logger.Debug("UpdateLeaveAsync leave already updated");
                        replyText = string.Format(_stringConstant.AlreadyUpdatedMessage, leave.Status);
                    }
                }
                else
                    replyText = _stringConstant.AdminErrorMessageUpdateSickLeave;
            }
            catch (SmtpException ex)
            {
                replyText += string.Format(_stringConstant.ReplyTextForSMTPExceptionErrorMessage,
                    _stringConstant.ErrorWhileSendingEmail, ex.Message.ToString());
            }
            _logger.Debug("UpdateLeaveAsync Client Repository - SendMessageAsync");
            // updating leave applied text of slack
            await _clientRepository.SendMessageAsync(leaveResponse.ResponseUrl, replyText);
        }

        /// <summary>
        /// Method to operate leave slack command
        /// </summary>
        /// <param name="leave">slash command object</param>
        public async Task LeaveRequestAsync(SlashCommand leave)
        {
            SlackAction actionType;
            // method to convert slash command to list of string
            List<string> slackText = _attachmentRepository.SlackText(leave.Text);
            // to get user details by slack user name
            ApplicationUser user = await _userManagerRepository.FirstOrDefaultAsync(x => x.SlackUserId == leave.UserId);
            if (user != null)
            {
                _logger.Debug("LeaveRequestAsync leave request user name : " + user.UserName);
                IncomingWebHook incomingWebHook = await _incomingWebHookRepository.FirstOrDefaultAsync(x => x.UserId == user.SlackUserId);
                if (incomingWebHook != null)
                {
                    _logger.Debug("LeaveRequestAsync leave request user incoming webhook is null : " + incomingWebHook.IncomingWebHookUrl);
                    leave.Text.ToLower();
                    // getting access token of user of promact oauth server
                    string accessToken = await _attachmentRepository.UserAccessTokenAsync(user.UserName);
                    // checking whether string ca convert to Slack Action or not, if true then further process will be conduct
                    bool actionConvertorResult = Enum.TryParse(slackText[0], out actionType);
                    if (actionConvertorResult)
                    {
                        switch (actionType)
                        {
                            case SlackAction.apply:
                                replyText = await LeaveApplyAsync(slackText, leave, accessToken, user.Id);
                                break;
                            case SlackAction.list:
                                replyText = await SlackLeaveListAsync(slackText, leave, accessToken);
                                break;
                            case SlackAction.cancel:
                                replyText = await SlackLeaveCancelAsync(slackText, leave, accessToken);
                                break;
                            case SlackAction.status:
                                replyText = await SlackLeaveStatusAsync(slackText, leave, accessToken);
                                break;
                            case SlackAction.balance:
                                replyText = await SlackLeaveBalanceAsync(leave, accessToken);
                                break;
                            case SlackAction.update:
                                replyText = await UpdateSickLeaveAsync(slackText, user, accessToken);
                                break;
                            default:
                                replyText = SlackLeaveHelp(leave);
                                break;
                        }
                    }
                    else
                        // if error in converting then user will get message to enter proper action command
                        replyText = _stringConstant.RequestToEnterProperAction;
                }
                else
                    replyText = _stringConstant.RequestToAddSlackApp;
            }
            else
                // if user doesn't exist then will get message of user doesn't exist and ask to externally logic from Oauth server
                replyText = _stringConstant.SorryYouCannotApplyLeave;
            _logger.Debug("LeaveRequestAsync Client Repository - SendMessageAsync");
            await _clientRepository.SendMessageAsync(leave.ResponseUrl, replyText);
        }

        /// <summary>
        /// Method to send error message to user od slack
        /// </summary>
        /// <param name="errorMessage">Message to send</param>
        /// <param name="responseUrl">Incoming webhook url</param>
        public async Task ErrorAsync(string responseUrl, string errorMessage)
        {
            // if something error will happen user will get this message
            var replyText = string.Format(_stringConstant.FirstSecondAndThirdIndexStringFormat, _stringConstant.Star, errorMessage, _stringConstant.Star);
            await _clientRepository.SendMessageAsync(responseUrl, replyText);
        }
        #endregion

        #region Private Methods


        /// <summary>
        /// Method to apply leave
        /// </summary>
        /// <param name="slackRequest">list of string contain leave slash command parameters</param>
        /// <param name="leave">leave slash command</param>
        /// <param name="accessToken">User's access token</param>
        /// <returns>Reply text to be send</returns>
        private async Task<string> LeaveApplyAsync(List<string> slackRequest, SlashCommand leave, string accessToken, string userId)
        {
            try
            {
                LeaveType leaveType;
                DateTime startDate, endDate, reJoinDate;
                User user = new User();
                string dateFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
                _logger.Debug("LeaveApplyAsync Date format of leave command : " + dateFormat);
                _logger.Debug("LeaveApplyAsync Current Culture info : " + Thread.CurrentThread.CurrentCulture.DisplayName);
                // checking whether string can convert to date of independent culture or not, if return true then further process will be conduct
                bool startDateConvertorResult = DateTime.TryParseExact(slackRequest[3], dateFormat, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out startDate);
                if (startDateConvertorResult)
                {
                    user = await _oauthCallsRepository.GetUserByUserIdAsync(userId, accessToken);
                    LeaveRequest leaveRequest = new LeaveRequest();
                    // Method to check leave's start date is not beyond today. Back date checking
                    bool validStartDate = LeaveStartDateValid(startDate);
                    if (validStartDate)
                    {
                        // checking whether string can convert to leave type or not, if return true then further process will be conduct
                        bool leaveTypeConvertorResult = Enum.TryParse(slackRequest[1], out leaveType);
                        if (leaveTypeConvertorResult)
                        {
                            // converting string to leave type of indian culture
                            switch (leaveType)
                            {
                                case LeaveType.cl:
                                    {
                                        _logger.Debug("LeaveApplyAsync Casual leave");
                                        // checking whether string can convert to date of indian culture or not, if return true then further process will be conduct
                                        bool endDateConvertorResult = DateTime.TryParseExact(slackRequest[4], dateFormat,
                                            CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate);
                                        bool reJoinDateConvertorResult = DateTime.TryParseExact(slackRequest[5], dateFormat,
                                            CultureInfo.InvariantCulture, DateTimeStyles.None, out reJoinDate);
                                        if (endDateConvertorResult && reJoinDateConvertorResult)
                                        {
                                            // Method to check leave's end date is not beyond start date and re-join date is not beyond end date
                                            bool validDate = ValidDateTimeForLeave(startDate, endDate, reJoinDate);
                                            if (validDate)
                                            {
                                                // get user details from oAuth server
                                                leaveRequest.EndDate = endDate;
                                                leaveRequest.RejoinDate = reJoinDate;
                                                leaveRequest.Status = Condition.Pending;
                                                leaveRequest.Type = leaveType;
                                                leaveRequest.Reason = slackRequest[2];
                                                leaveRequest.FromDate = startDate;
                                                leaveRequest.CreatedOn = DateTime.UtcNow;
                                                // if user doesn't exist in OAuth server then user can't apply leave
                                                if (user.Id != null)
                                                {
                                                    _logger.Debug("LeaveApplyAsync leave apply user name : " + user.UserName);
                                                    // Method to check more than one leave cannot be applied on that date
                                                    validDate = await LeaveDateDuplicate(user.Id, startDate, endDate);
                                                    if (!validDate)
                                                    {
                                                        leaveRequest.EmployeeId = user.Id;
                                                        await _leaveRepository.ApplyLeaveAsync(leaveRequest);
                                                        _logger.Debug("LeaveApplyAsync Leave applied sucessfully");
                                                        replyText = _attachmentRepository.ReplyText(leave.Username, leaveRequest);
                                                        _logger.Debug("LeaveApplyAsync Client Repository - SendMessageWithAttachmentIncomingWebhookAsync");
                                                        // method to send slack notification and email to team leaders and management
                                                        await _clientRepository.SendMessageWithAttachmentIncomingWebhookAsync(leaveRequest,
                                                            accessToken, replyText, user.Id);
                                                    }
                                                    else
                                                        replyText = _stringConstant.LeaveAlreadyExistOnSameDate;
                                                }
                                                else
                                                    // if user doesn't exist in OAuth server then user can't apply leave, will get this message
                                                    replyText = _stringConstant.SorryYouCannotApplyLeave;
                                            }
                                            else
                                                replyText = _stringConstant.InValidDateErrorMessage;
                                        }
                                        else
                                            // if date is not proper than date format error message will be send to user
                                            replyText = string.Format(_stringConstant.DateFormatErrorMessage, dateFormat);
                                    }
                                    break;
                                case LeaveType.sl:
                                    {
                                        _logger.Debug("LeaveApplyAsync Sick leave");
                                        bool isAdmin = false;
                                        User newUser = new User();
                                        if (slackRequest.Count > 4)
                                        {
                                            isAdmin = await _oauthCallsRepository.UserIsAdminAsync(userId, accessToken);
                                            _logger.Debug("LeaveApplyAsync User is admin : " + isAdmin);
                                            if (isAdmin)
                                            {
                                                _logger.Debug("LeaveApplyAsync Sick Leave applying for other");
                                                SlackUserDetailAc slackUser = await _slackUserRepository.GetBySlackNameAsync(slackRequest[4]);
                                                // get user details from oAuth server for other user
                                                var newUserDetails = await _userManagerRepository.FirstOrDefaultAsync(x => x.SlackUserId == slackUser.UserId);
                                                newUser = await _oauthCallsRepository.GetUserByUserIdAsync(newUserDetails.Id, accessToken);
                                            }
                                            else
                                                replyText = _stringConstant.AdminErrorMessageApplySickLeave;
                                        }
                                        else
                                        {
                                            _logger.Debug("LeaveApplyAsync Sick Leave applying for own");
                                            // get user details from oAuth server for own
                                            newUser = await _oauthCallsRepository.GetUserByUserIdAsync(userId, accessToken);
                                            leaveRequest.EndDate = startDate;
                                            leaveRequest.RejoinDate = startDate.AddDays(1);
                                        }
                                        leaveRequest.Status = Condition.Approved;
                                        leaveRequest.Type = leaveType;
                                        leaveRequest.Reason = slackRequest[2];
                                        leaveRequest.FromDate = startDate;
                                        leaveRequest.CreatedOn = DateTime.UtcNow;
                                        // if user doesn't exist in OAuth server then user can't apply leave
                                        if (newUser.Id != null)
                                        {
                                            
                                            // Method to check more than one leave cannot be applied on that date
                                            bool validDate = await LeaveDateDuplicate(newUser.Id, startDate, null);
                                            if (!validDate)
                                            {
                                                leaveRequest.EmployeeId = newUser.Id;
                                                await _leaveRepository.ApplyLeaveAsync(leaveRequest);
                                                _logger.Debug("LeaveApplyAsync Leave applied successfully");
                                                replyText = _attachmentRepository.ReplyTextSick(newUser.FirstName, leaveRequest);
                                                _logger.Debug("LeaveApplyAsync Client Repository - SendMessageWithoutButtonAttachmentIncomingWebhookAsync");
                                                await _clientRepository.SendMessageWithoutButtonAttachmentIncomingWebhookAsync(leaveRequest,
                                                    accessToken, replyText, newUser.Id);
                                                if (isAdmin)
                                                {
                                                    _logger.Debug("LeaveApplyAsync Client Repository - SendSickLeaveMessageToUserIncomingWebhookAsync");
                                                    await _clientRepository.SendSickLeaveMessageToUserIncomingWebhookAsync(leaveRequest,
                                                        user.Email, replyText, newUser);
                                                }
                                            }
                                            else
                                                replyText = _stringConstant.LeaveAlreadyExistOnSameDate;
                                        }
                                        else
                                            // if user doesn't exist in OAuth server then user can't apply leave, will get this message
                                            replyText += _stringConstant.SorryYouCannotApplyLeave;
                                    }
                                    break;
                            }
                        }
                        else
                            // if leave type is not proper than not of leave type format error message will be send to user
                            replyText = _stringConstant.NotTypeOfLeave;
                    }
                    else
                        replyText = _stringConstant.BackDateErrorMessage;
                }
                else
                    // if date is not proper than date format error message will be send to user
                    replyText = string.Format(_stringConstant.DateFormatErrorMessage, dateFormat);
            }
            catch (SmtpException ex)
            {
                // error message will be send to email. But leave will be applied
                replyText = string.Format(_stringConstant.ReplyTextForSMTPExceptionErrorMessage,
                    _stringConstant.ErrorWhileSendingEmail, ex.Message.ToString());
            }
            return replyText;
        }

        /// <summary>
        /// Method to get Employee Id from its slackUserId and from its employeeId, to get list of leave
        /// </summary>
        /// <param name="userId">User's used Id</param>
        /// <param name="accessToken">User's access token</param>
        /// <returns>Reply text to be send</returns>
        private async Task<string> LeavesListBySlackUserIdAsync(string userId, string accessToken)
        {
            // get user details from oAuth server
            User user = await _oauthCallsRepository.GetUserByUserIdAsync(userId, accessToken);
            _logger.Debug("LeavesListBySlackUserIdAsync user name : " + user.UserName);
            // get list of leave from user Id
            IEnumerable<LeaveRequest> leaveList = _leaveRepository.LeaveListByUserId(user.Id);
            _logger.Debug("LeavesListBySlackUserIdAsync leave list count : " + leaveList.Count());
            // if leave exit then further will get list in string else 
            if (leaveList.Count() != 0)
            {
                foreach (var leave in leaveList)
                {
                    if (leave.Type == LeaveType.cl)
                    {
                        replyText += string.Format(_stringConstant.ReplyTextForCasualLeaveList, leave.Id,
                            leave.Reason, leave.FromDate.ToShortDateString(),
                            leave.EndDate.Value.ToShortDateString(), leave.Status,
                            Environment.NewLine);
                    }
                    else
                    {
                        replyText += string.Format(_stringConstant.ReplyTextForSickLeaveList, leave.Id,
                            leave.Reason, leave.FromDate.ToShortDateString(), leave.Status,
                            Environment.NewLine);
                    }
                }
            }
            else
            {
                // if leave doesnot exit for that user
                replyText = _stringConstant.SlashCommandLeaveStatusErrorMessage;
            }
            return replyText;
        }

        /// <summary>
        /// Method to Cancel leave, only allowed to the applier of the leave to cancel the leave
        /// </summary>
        /// <param name="leaveId">leave request Id</param>
        /// <param name="userId">User's user Id</param>
        /// <param name="accessToken">User's access token</param>
        /// <returns>Reply text to be send</returns>
        private async Task<string> LeaveCancelByIdAsync(int leaveId, string userId, string accessToken)
        {
            // get user details from oAuth server
            User user = await _oauthCallsRepository.GetUserByUserIdAsync(userId, accessToken);
            _logger.Debug("LeaveCancelByIdAsync user name : " + user.UserName);
            LeaveRequest leave = await _leaveRepository.LeaveByIdAsync(leaveId);
            _logger.Debug("LeaveCancelByIdAsync leave employee id : " + leave.EmployeeId);
            // only authorize user of leave is allowed to cancel there leave
            if (user.Id == leave.EmployeeId)
            {
                // method to cancel leave
                LeaveRequest updateLeave = await _leaveRepository.CancelLeaveAsync(leaveId);
                _logger.Debug("LeaveCancelByIdAsync leave cancel sucessfully");
                replyText = string.Format(_stringConstant.ReplyTextForCancelLeave, updateLeave.Id,
                    updateLeave.FromDate.ToShortDateString(), updateLeave.EndDate.Value.ToShortDateString(),
                    updateLeave.Status);
            }
            else
            {
                // if leave doesn't exist or unauthorize trespass try to do
                replyText = _stringConstant.ReplyTextForErrorInCancelLeave;
            }
            return replyText;
        }

        /// <summary>
        /// Method to get Employee Id from its userName and from its employeeId, to get last leave status
        /// </summary>
        /// <param name="userId">User's user Id</param>
        /// <param name="accessToken">User's access token</param>
        /// <returns>Reply text to be send</returns>
        private async Task<string> LeaveStatusBySlackUserIdAsync(string userId, string accessToken)
        {
            // get user details from oAuth server
            User user = await _oauthCallsRepository.GetUserByUserIdAsync(userId, accessToken);
            _logger.Debug("LeaveStatusBySlackUserIdAsync user name : " + user.UserName);
            try
            {
                // get only last leave of the user
                LeaveRequest leave = _leaveRepository.LeaveListStatusByUserId(user.Id);
                if (leave.Type == LeaveType.cl)
                {
                    replyText = string.Format(_stringConstant.ReplyTextForCasualLeaveStatus, leave.Id,
                        leave.FromDate.ToShortDateString(), leave.EndDate.Value.ToShortDateString(),
                        leave.Reason, leave.Status);
                }
                else
                {
                    replyText = string.Format(_stringConstant.ReplyTextForSickLeaveStatus, leave.Id,
                        leave.FromDate.ToShortDateString(), leave.Reason, leave.Status);
                }
            }
            catch (LeaveNotFoundForUser)
            {
                // if leave doesn't exist 
                replyText = _stringConstant.SlashCommandLeaveStatusErrorMessage;
            }
            return replyText;
        }

        /// <summary>
        /// Method to Get Leave List on slack
        /// </summary>
        /// <param name="slackText">list of string contain leave slash command parameter</param>
        /// <param name="leave">leave slash command</param>
        /// <param name="accessToken">User's access token</param>
        /// <returns>Reply text to be send</returns>
        private async Task<string> SlackLeaveListAsync(List<string> slackText, SlashCommand leave, string accessToken)
        {
            // if slackText count is more then 1 then its means that user want to get leave list of someone else
            if (slackText.Count > 1)
            {
                // other user slack user name
                SlackUserDetailAc slackUser = await _slackUserRepository.GetBySlackNameAsync(slackText[1]);
                var user = await _userManagerRepository.FirstOrDefaultAsync(x => x.SlackUserId == slackUser.UserId);
                _logger.Debug("SlackLeaveListAsync user name other : " + user.UserName);
                // leave list of other user 
                replyText = await LeavesListBySlackUserIdAsync(user.Id, accessToken);
            }
            else
            {
                var user = await _userManagerRepository.FirstOrDefaultAsync(x => x.SlackUserId == leave.UserId);
                _logger.Debug("SlackLeaveListAsync user name own : " + user.UserName);
                // leave list of own
                replyText = await LeavesListBySlackUserIdAsync(user.Id, accessToken);
            }
            return replyText;
        }

        /// <summary>
        /// Method to cancel leave by its Id from slack
        /// </summary>
        /// <param name="slackText">list of string contain leave slash command parameter</param>
        /// <param name="leave">leave slash command</param>
        /// <param name="accessToken">User's access token</param>
        /// <returns>Reply text to be send</returns>
        private async Task<string> SlackLeaveCancelAsync(List<string> slackText, SlashCommand leave, string accessToken)
        {
            int leaveId;
            // checking whether string can be convert to integer or not, true then only further process will be conduct
            bool leaveIdConvertorResult = int.TryParse(slackText[1], out leaveId);
            if (leaveIdConvertorResult)
            {
                var user = await _userManagerRepository.FirstOrDefaultAsync(x => x.SlackUserId == leave.UserId);
                _logger.Debug("SlackLeaveCancelAsync user name : " + user.UserName);
                // method to cancel leave by its id
                replyText = await LeaveCancelByIdAsync(leaveId, user.Id, accessToken);
            }
            else
                // if string converting to integer return false then user will get cancel command error message
                replyText = _stringConstant.SlashCommandLeaveCancelErrorMessage;
            return replyText;
        }

        /// <summary>
        /// Method to get last leave status and details on slack
        /// </summary>
        /// <param name="slackText">list of string contain leave slash command parameter</param>
        /// <param name="leave">leave slash command</param>
        /// <param name="accessToken">User's access token</param>
        /// <returns>Reply text to be send</returns>
        private async Task<string> SlackLeaveStatusAsync(List<string> slackText, SlashCommand leave, string accessToken)
        {
            // if slackText count is more then 1 then its means that user want to get leave list of someone else
            if (slackText.Count > 1)
            {
                // other user slack user name
                SlackUserDetailAc slackUser = await _slackUserRepository.GetBySlackNameAsync(slackText[1]);
                var user = await _userManagerRepository.FirstOrDefaultAsync(x => x.SlackUserId == slackUser.UserId);
                _logger.Debug("SlackLeaveStatusAsync user name other : " + user.UserName);
                // last leave details of other user 
                replyText = await LeaveStatusBySlackUserIdAsync(user.Id, accessToken);
            }
            else
            {
                var user = await _userManagerRepository.FirstOrDefaultAsync(x => x.SlackUserId == leave.UserId);
                _logger.Debug("SlackLeaveStatusAsync user name own : " + user.UserName);
                // last leave details of own
                replyText = await LeaveStatusBySlackUserIdAsync(user.Id, accessToken);
            }
            return replyText;
        }


        /// <summary>
        /// Method to check leave Balance from slack
        /// </summary>
        /// <param name="leave">leave slash command</param>
        /// <param name="accessToken">User's access token</param>
        /// <returns>Reply text to be send</returns>
        private async Task<string> SlackLeaveBalanceAsync(SlashCommand leave, string accessToken)
        {
            var userDetails = await _userManagerRepository.FirstOrDefaultAsync(x => x.SlackUserId == leave.UserId);
            // get user details from oAuth server
            User user = await _oauthCallsRepository.GetUserByUserIdAsync(userDetails.Id, accessToken);
            if (user.Id != null)
            {
                _logger.Debug("SlackLeaveBalanceAsync user name : " + user.UserName);
                // get user leave allowed details from oAuth server
                LeaveAllowed allowedLeave = await _oauthCallsRepository.AllowedLeave(user.Id, accessToken);
                // method to get user's number of leave taken
                LeaveAllowed leaveTaken = _leaveRepository.NumberOfLeaveTaken(user.Id);
                double casualLeaveTaken = leaveTaken.CasualLeave;
                double sickLeaveTaken = leaveTaken.SickLeave;
                double casualLeaveLeft = allowedLeave.CasualLeave - casualLeaveTaken;
                double sickLeaveLeft = allowedLeave.SickLeave - sickLeaveTaken;
                replyText = string.Format(_stringConstant.ReplyTextForCasualLeaveBalance, casualLeaveTaken,
                    allowedLeave.CasualLeave, Environment.NewLine, casualLeaveLeft);
                replyText += string.Format(_stringConstant.ReplyTextForSickLeaveBalance, sickLeaveTaken,
                    allowedLeave.SickLeave, Environment.NewLine, sickLeaveLeft);
            }
            else
            {
                // if user doesn't exist in Oauth server
                replyText = _stringConstant.LeaveNoUserErrorMessage;
            }
            return replyText;
        }

        /// <summary>
        /// Method for gettin help on slack regards Leave slash command
        /// </summary>
        /// <param name="leave">list of string contain leave slash command parameter</param>
        /// <returns>Reply text to be send</returns>
        private string SlackLeaveHelp(SlashCommand leave)
        {
            string replyText = _stringConstant.SlackHelpMessage;
            return replyText;
        }

        /// <summary>
        /// Method to update sick leave, only admin allowed
        /// </summary>
        /// <param name="slackText">list of string contain leave slash command parameter</param>
        /// <param name="user">User details</param>
        /// <param name="accessToken">User's access token</param>
        /// <returns>Reply text to be send</returns>
        private async Task<string> UpdateSickLeaveAsync(List<string> slackText, ApplicationUser user, string accessToken)
        {
            // checking from oAuth whether user is Admin or not
            bool isAdmin = await _oauthCallsRepository.UserIsAdminAsync(user.Id, accessToken);
            _logger.Debug("UpdateSickLeaveAsync user is admin : " + isAdmin);
            if (isAdmin)
            {
                int leaveId;
                DateTime endDate, reJoinDate;
                // checking whether string can be convert to integer or not, true then only further process will be conduct
                bool leaveIdConvertorResult = int.TryParse(slackText[1], out leaveId);
                if (leaveIdConvertorResult)
                {
                    LeaveRequest leave = await _leaveRepository.LeaveByIdAsync(leaveId);
                    if (leave != null && leave.Type == LeaveType.sl)
                    {
                        _logger.Debug("UpdateSickLeaveAsync Leave employee id : " + leave.EmployeeId);
                        string dateFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
                        // checking whether string can convert to date of indian culture or not, if return true then further process will be conduct
                        bool endDateConvertorResult = DateTime.TryParseExact(slackText[2], dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate);
                        bool reJoinDateConvertorResult = DateTime.TryParseExact(slackText[3], dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out reJoinDate);
                        if (endDateConvertorResult && reJoinDateConvertorResult)
                        {
                            User newUser = await _oauthCallsRepository.GetUserByUserIdAsync(leave.EmployeeId, accessToken);
                            leave.EndDate = endDate;
                            leave.RejoinDate = reJoinDate;
                            // Method to check leave's end date is not beyond start date and re-join date is not beyond end date
                            bool validDate = ValidDateTimeForLeave(leave.FromDate, leave.EndDate.Value, leave.RejoinDate.Value);
                            if (validDate)
                            {
                                await _leaveRepository.UpdateLeaveAsync(leave);
                                _logger.Debug("UpdateSickLeaveAsync leave updated successfully");
                                replyText = string.Format(_stringConstant.ReplyTextForSickLeaveUpdate
                                    , newUser.FirstName, leave.FromDate.ToShortDateString(), leave.EndDate.Value.ToShortDateString(),
                                    leave.Reason, leave.RejoinDate.Value.ToShortDateString());
                                _logger.Debug("Client Repository - SendMessageWithoutButtonAttachmentIncomingWebhookAsync");
                                await _clientRepository.SendMessageWithoutButtonAttachmentIncomingWebhookAsync(leave, accessToken, replyText, newUser.Id);
                                _logger.Debug("Client Repository - SendSickLeaveMessageToUserIncomingWebhookAsync");
                                await _clientRepository.SendSickLeaveMessageToUserIncomingWebhookAsync(leave, user.Email, replyText, newUser);
                            }
                            else
                                replyText = _stringConstant.InValidDateErrorMessage;
                        }
                        else
                            // if date is not proper than date format error message will be send to user
                            replyText = string.Format(_stringConstant.DateFormatErrorMessage, dateFormat);
                    }
                    else
                        // if sick leave will doesn't exist for leaveId
                        replyText = _stringConstant.SickLeaveDoesnotExist;
                }
                else
                    // if string converting to integer return false then user will get cancel command error message
                    replyText = _stringConstant.UpdateEnterAValidLeaveId;
            }
            else
                // if user is not admin then this message will be show to user
                replyText = _stringConstant.AdminErrorMessageUpdateSickLeave;
            return replyText;
        }

        /// <summary>
        /// Method to check leave's end date is not beyond start date and re-join date is not beyond end date
        /// </summary>
        /// <param name="startDate">leave start date</param>
        /// <param name="endDate">leave end date</param>
        /// <param name="rejoinDate">leave rejoin date</param>
        /// <returns>true or false</returns>
        private bool ValidDateTimeForLeave(DateTime startDate, DateTime endDate, DateTime rejoinDate)
        {
            var valid = startDate.CompareTo(endDate);
            if (valid < 1)
            {
                valid = endDate.CompareTo(rejoinDate);
                if (valid < 0)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// Method to check leave's start date is not beyond today. Back date checking
        /// </summary>
        /// <param name="startDate">leave start date</param>
        /// <returns>true or false</returns>
        private bool LeaveStartDateValid(DateTime startDate)
        {
            var valid = DateTime.UtcNow.Date.CompareTo(startDate.Date);
            if (valid <= 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Method to check more than one leave cannot be applied on that date
        /// </summary>
        /// <param name="userId">User's Id</param>
        /// <param name="startDate">leave start date</param>
        /// <param name="endDate">leave end date</param>
        /// <returns>true or false</returns>
        private async Task<bool> LeaveDateDuplicate(string userId, DateTime startDate, DateTime? endDate)
        {
            int valid = -1;
            bool validIndicator = false;
            LeaveRequest lastLeave = new LeaveRequest();
            IEnumerable<LeaveRequest> allLeave = await _leaveRepository.LeaveListByUserIdOnlyApprovedAndPending(userId);
            if (allLeave != null)
            {
                foreach (var leave in allLeave)
                {
                    _logger.Debug("LeaveDateDuplicate Leave of user in count : " + allLeave.Count());
                    if (leave.EndDate.HasValue)
                        valid = leave.EndDate.Value.CompareTo(startDate);
                    else
                        valid = leave.FromDate.CompareTo(startDate);
                    switch (valid)
                    {
                        case -1:
                            {
                                if (endDate != null)
                                    valid = leave.FromDate.CompareTo(endDate);
                                else
                                    valid = leave.FromDate.CompareTo(startDate);
                                if (valid == -1)
                                    validIndicator = false;
                                else
                                    validIndicator = true;
                            }
                            break;
                        case 0:
                            {
                                validIndicator = true;
                            }
                            break;
                        case 1:
                            {
                                if (endDate != null)
                                    valid = leave.FromDate.CompareTo(endDate);
                                else
                                    valid = leave.FromDate.CompareTo(startDate);
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

        /// <summary>
        /// try to convert string date to DateTime
        /// </summary>
        /// <param name="inputDate">string date</param>
        /// <param name="date">expected date as out</param>
        /// <returns>true or false</returns>
        private bool DateFormatChecker(string inputDate, out DateTime date)
        {
            var dateFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
            return DateTime.TryParseExact(inputDate, dateFormat, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out date);
        }
        #endregion
    }
}




