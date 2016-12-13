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
using Promact.Erp.Util;
using Promact.Erp.Util.ExceptionHandler;

namespace Promact.Core.Repository.SlackRepository
{
    public class SlackRepository : ISlackRepository
    {
        #region Private Variable
        private readonly IOauthCallsRepository _oauthCallsRepository;
        private readonly ISlackUserRepository _slackUserRepository;
        private readonly ILeaveRequestRepository _leaveRepository;
        private readonly IClient _client;
        private readonly IStringConstantRepository _stringConstant;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IRepository<ApplicationUser> _userManager;
        string replyText = null;
        private readonly IRepository<IncomingWebHook> _incomingWebHook;
        private readonly IEmailServiceTemplateRepository _emailTemplateRepository;
        private readonly IEmailService _emailService;
        #endregion

        #region Constructor
        public SlackRepository(ILeaveRequestRepository leaveRepository, IOauthCallsRepository oauthCallsRepository,
            ISlackUserRepository slackUserRepository, IClient client, IStringConstantRepository stringConstant,
            IAttachmentRepository attachmentRepository, IRepository<ApplicationUser> userManager,
            IRepository<IncomingWebHook> incomingWebHook, IEmailServiceTemplateRepository emailTemplateRepository,
            IEmailService emailService)
        {
            _oauthCallsRepository = oauthCallsRepository;
            _leaveRepository = leaveRepository;
            _client = client;
            _stringConstant = stringConstant;
            _attachmentRepository = attachmentRepository;
            _userManager = userManager;
            _slackUserRepository = slackUserRepository;
            _incomingWebHook = incomingWebHook;
            _emailTemplateRepository = emailTemplateRepository;
            _emailService = emailService;
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
                // method to get leave by its id
                var leave = await _leaveRepository.LeaveByIdAsync(Convert.ToInt32(leaveResponse.CallbackId));
                var user = await _userManager.FirstOrDefaultAsync(x => x.Id == leave.EmployeeId);
                var slackUser = await _slackUserRepository.GetByIdAsync(user.SlackUserId);
                var updaterUser = await _userManager.FirstOrDefaultAsync(x => x.SlackUserId == leaveResponse.User.Id);
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
                    _leaveRepository.UpdateLeave(leave);
                    replyText = string.Format(_stringConstant.CasualLeaveUpdateMessageForUser,
                                leave.Id, leave.FromDate.ToShortDateString(), leave.EndDate.Value.ToShortDateString(),
                                leave.Reason, leave.Status, leaveResponse.User.Name);
                    var incomingWebHook = await _incomingWebHook.FirstOrDefaultAsync(x => x.UserId == slackUser.UserId);
                    // Used to send slack message to the user about leave updation
                    await _client.UpdateMessageAsync(incomingWebHook.IncomingWebHookUrl, replyText);
                    // Used to send email to the user about leave updation
                    EmailApplication email = new EmailApplication();
                    email.Body = _emailTemplateRepository.EmailServiceTemplateLeaveUpdate(leave);
                    email.From = updaterUser.Email;
                    email.To = user.Email;
                    email.Subject = string.Format(_stringConstant.LeaveUpdateEmailStringFormat, _stringConstant.Leave, leave.Status);
                    replyText = string.Format(_stringConstant.ReplyTextForUpdateLeave, leave.Status, slackUser.Name,
                    leave.FromDate.ToShortDateString(), leave.EndDate.Value.ToShortDateString(), leave.Reason,
                    leave.RejoinDate.Value.ToShortDateString());
                    _emailService.Send(email);
                }
                else
                {
                    replyText = string.Format(_stringConstant.AlreadyUpdatedMessage, leave.Status);
                }
            }
            catch (SmtpException ex)
            {
                replyText += string.Format(_stringConstant.ReplyTextForSMTPExceptionErrorMessage,
                    _stringConstant.ErrorWhileSendingEmail, ex.Message.ToString());
            }
            // updating leave applied text of slack
            await _client.SendMessageAsync(leaveResponse.ResponseUrl, replyText);
        }

        /// <summary>
        /// Method to operate leave slack command
        /// </summary>
        /// <param name="leave">slash command object</param>
        public async Task LeaveRequestAsync(SlashCommand leave)
        {
            SlackAction actionType;
            // method to convert slash command to list of string
            var slackText = _attachmentRepository.SlackText(leave.Text);
            // to get user details by slack user name
            var user = await _userManager.FirstOrDefaultAsync(x => x.SlackUserId == leave.UserId);
            if (user != null)
            {
                var incomingWebHook = await _incomingWebHook.FirstOrDefaultAsync(x => x.UserId == user.SlackUserId);
                if (incomingWebHook != null)
                {
                    leave.Text.ToLower();
                    // getting access token of user of promact oauth server
                    var accessToken = await _attachmentRepository.UserAccessTokenAsync(user.UserName);
                    // checking whether string ca convert to Slack Action or not, if true then further process will be conduct
                    var actionConvertorResult = Enum.TryParse(slackText[0], out actionType);
                    if (actionConvertorResult)
                    {
                        switch (actionType)
                        {
                            case SlackAction.apply:
                                replyText = await LeaveApplyAsync(slackText, leave, accessToken);
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
            await _client.SendMessageAsync(leave.ResponseUrl, replyText);
        }

        /// <summary>
        /// Method to send error message to user od slack
        /// </summary>
        /// <param name="leave">slash command object</param>
        public void Error(SlashCommand leave)
        {
            // if something error will happen user will get this message
            var replyText = _stringConstant.SlashCommandErrorMessage;
            _client.SendMessageAsync(leave.ResponseUrl, replyText);
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
        private async Task<string> LeaveApplyAsync(List<string> slackRequest, SlashCommand leave, string accessToken)
        {
            try
            {
                LeaveType leaveType;
                DateTime startDate, endDate, reJoinDate;
                User user = new User();
                var dateFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
                // checking whether string can convert to date of independent culture or not, if return true then further process will be conduct
                var startDateConvertorResult = DateTime.TryParseExact(slackRequest[3], dateFormat, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out startDate);
                if (startDateConvertorResult)
                {
                    user = await _oauthCallsRepository.GetUserByUserIdAsync(leave.UserId, accessToken);
                    LeaveRequest leaveRequest = new LeaveRequest();
                    // Method to check leave's start date is not beyond today. Back date checking
                    var validStartDate = LeaveStartDateValid(startDate);
                    if (validStartDate)
                    {
                        // checking whether string can convert to leave type or not, if return true then further process will be conduct
                        var leaveTypeConvertorResult = Enum.TryParse(slackRequest[1], out leaveType);
                        if (leaveTypeConvertorResult)
                        {
                            // converting string to leave type of indian culture
                            switch (leaveType)
                            {
                                case LeaveType.cl:
                                    {
                                        // checking whether string can convert to date of indian culture or not, if return true then further process will be conduct
                                        var endDateConvertorResult = DateTime.TryParseExact(slackRequest[4], dateFormat,
                                            CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate);
                                        var reJoinDateConvertorResult = DateTime.TryParseExact(slackRequest[5], dateFormat,
                                            CultureInfo.InvariantCulture, DateTimeStyles.None, out reJoinDate);
                                        if (endDateConvertorResult && reJoinDateConvertorResult)
                                        {
                                            // Method to check leave's end date is not beyond start date and re-join date is not beyond end date
                                            var validDate = ValidDateTimeForLeave(startDate, endDate, reJoinDate);
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
                                                    // Method to check more than one leave cannot be applied on that date
                                                    validDate = LeaveDateDuplicate(user.Id, startDate);
                                                    if (!validDate)
                                                    {
                                                        leaveRequest.EmployeeId = user.Id;
                                                        _leaveRepository.ApplyLeave(leaveRequest);
                                                        replyText = _attachmentRepository.ReplyText(leave.Username, leaveRequest);
                                                        // method to send slack notification and email to team leaders and management
                                                        await _client.SendMessageWithAttachmentIncomingWebhookAsync(leaveRequest,
                                                            accessToken, replyText, leave.UserId);
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
                                            replyText = _stringConstant.DateFormatErrorMessage;
                                    }
                                    break;
                                case LeaveType.sl:
                                    {
                                        bool IsAdmin = false;
                                        User newUser = new User();
                                        if (slackRequest.Count > 4)
                                        {
                                            IsAdmin = await _oauthCallsRepository.UserIsAdminAsync(leave.UserId, accessToken);
                                            if (IsAdmin)
                                            {
                                                var slackUser = await _slackUserRepository.GetBySlackNameAsync(slackRequest[4]);
                                                // get user details from oAuth server for other user
                                                newUser = await _oauthCallsRepository.GetUserByUserIdAsync(slackUser.UserId, accessToken);
                                            }
                                            else
                                                replyText = _stringConstant.AdminErrorMessageApplySickLeave;
                                        }
                                        else
                                        {
                                            // get user details from oAuth server for own
                                            newUser = await _oauthCallsRepository.GetUserByUserIdAsync(leave.UserId, accessToken);
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
                                            var validDate = LeaveDateDuplicate(newUser.Id, startDate);
                                            if (!validDate)
                                            {
                                                leaveRequest.EmployeeId = newUser.Id;
                                                _leaveRepository.ApplyLeave(leaveRequest);
                                                replyText = _attachmentRepository.ReplyTextSick(newUser.FirstName, leaveRequest);
                                                await _client.SendMessageWithoutButtonAttachmentIncomingWebhookAsync(leaveRequest,
                                                    accessToken, replyText, newUser.SlackUserId);
                                                if (IsAdmin)
                                                {
                                                    await _client.SendSickLeaveMessageToUserIncomingWebhookAsync(leaveRequest,
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
                    replyText = _stringConstant.DateFormatErrorMessage;
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
        /// <param name="slackUserId">User's slack used Id</param>
        /// <param name="accessToken">User's access token</param>
        /// <returns>Reply text to be send</returns>
        private async Task<string> LeavesListBySlackUserIdAsync(string slackUserId, string accessToken)
        {
            // get user details from oAuth server
            var user = await _oauthCallsRepository.GetUserByUserIdAsync(slackUserId, accessToken);
            // get list of leave from user Id
            var leaveList = _leaveRepository.LeaveListByUserId(user.Id);
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
                            System.Environment.NewLine);
                    }
                    else
                    {
                        replyText += string.Format(_stringConstant.ReplyTextForSickLeaveList, leave.Id,
                            leave.Reason, leave.FromDate.ToShortDateString(), leave.Status,
                            System.Environment.NewLine);
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
        /// <param name="slackUserId">User's slack user Id</param>
        /// <param name="accessToken">User's access token</param>
        /// <returns>Reply text to be send</returns>
        private async Task<string> LeaveCancelByIdAsync(int leaveId, string slackUserId, string accessToken)
        {
            // get user details from oAuth server
            var user = await _oauthCallsRepository.GetUserByUserIdAsync(slackUserId, accessToken);
            var leave = await _leaveRepository.LeaveByIdAsync(leaveId);
            // only authorize user of leave is allowed to cancel there leave
            if (user.Id == leave.EmployeeId)
            {
                // method to cancel leave
                var updateLeave = _leaveRepository.CancelLeave(leaveId);
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
        /// <param name="slackUserId">User's slack user Id</param>
        /// <param name="accessToken">User's access token</param>
        /// <returns>Reply text to be send</returns>
        private async Task<string> LeaveStatusBySlackUserIdAsync(string slackUserId, string accessToken)
        {
            // get user details from oAuth server
            var user = await _oauthCallsRepository.GetUserByUserIdAsync(slackUserId, accessToken);
            try
            {
                // get only last leave of the user
                var leave = _leaveRepository.LeaveListStatusByUserId(user.Id);
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
                // leave list of other user 
                replyText = await LeavesListBySlackUserIdAsync(slackUser.UserId, accessToken);
            }
            else
            {
                // leave list of own
                replyText = await LeavesListBySlackUserIdAsync(leave.UserId, accessToken);
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
            var leaveIdConvertorResult = int.TryParse(slackText[1], out leaveId);
            if (leaveIdConvertorResult)
            {
                // method to cancel leave by its id
                replyText = await LeaveCancelByIdAsync(leaveId, leave.UserId, accessToken);
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
                // last leave details of other user 
                replyText = await LeaveStatusBySlackUserIdAsync(slackUser.UserId, accessToken);
            }
            else
            {
                // last leave details of own
                replyText = await LeaveStatusBySlackUserIdAsync(leave.UserId, accessToken);
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
            // get user details from oAuth server
            var user = await _oauthCallsRepository.GetUserByUserIdAsync(leave.UserId, accessToken);
            if (user.Id != null)
            {
                // get user leave allowed details from oAuth server
                var allowedLeave = await _oauthCallsRepository.CasualLeaveAsync(leave.UserId, accessToken);
                // method to get user's number of leave taken
                var leaveTaken = _leaveRepository.NumberOfLeaveTaken(user.Id);
                var casualLeaveTaken = leaveTaken.CasualLeave;
                var sickLeaveTaken = leaveTaken.SickLeave;
                var casualLeaveLeft = allowedLeave.CasualLeave - casualLeaveTaken;
                var sickLeaveLeft = allowedLeave.SickLeave - sickLeaveTaken;
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
            var replyText = _stringConstant.SlackHelpMessage;
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
            var IsAdmin = await _oauthCallsRepository.UserIsAdminAsync(user.SlackUserId, accessToken);
            if (IsAdmin)
            {
                int leaveId;
                DateTime endDate, reJoinDate;
                // checking whether string can be convert to integer or not, true then only further process will be conduct
                var leaveIdConvertorResult = int.TryParse(slackText[1], out leaveId);
                if (leaveIdConvertorResult)
                {
                    var leave = await _leaveRepository.LeaveByIdAsync(leaveId);
                    if (leave != null && leave.Type == LeaveType.sl)
                    {
                        var dateFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
                        // checking whether string can convert to date of indian culture or not, if return true then further process will be conduct
                        var endDateConvertorResult = DateTime.TryParseExact(slackText[2], dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate);
                        var reJoinDateConvertorResult = DateTime.TryParseExact(slackText[3], dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out reJoinDate);
                        if (endDateConvertorResult && reJoinDateConvertorResult)
                        {
                            var newUser = await _oauthCallsRepository.GetUserByEmployeeIdAsync(leave.EmployeeId, accessToken);
                            leave.EndDate = endDate;
                            leave.RejoinDate = reJoinDate;
                            // Method to check leave's end date is not beyond start date and re-join date is not beyond end date
                            var validDate = ValidDateTimeForLeave(leave.FromDate, leave.EndDate.Value, leave.RejoinDate.Value);
                            if (validDate)
                            {
                                _leaveRepository.UpdateLeave(leave);
                                replyText = string.Format(_stringConstant.ReplyTextForSickLeaveUpdate
                                    , newUser.FirstName, leave.FromDate.ToShortDateString(), leave.EndDate.Value.ToShortDateString(),
                                    leave.Reason, leave.RejoinDate.Value.ToShortDateString());
                                await _client.SendMessageWithoutButtonAttachmentIncomingWebhookAsync(leave, accessToken, replyText, newUser.SlackUserId);
                                await _client.SendSickLeaveMessageToUserIncomingWebhookAsync(leave, user.Email, replyText, newUser);
                            }
                            else
                                replyText = _stringConstant.InValidDateErrorMessage;
                        }
                        else
                            // if date is not proper than date format error message will be send to user
                            replyText = _stringConstant.DateFormatErrorMessage;
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
        /// <returns>true or false</returns>
        private bool LeaveDateDuplicate(string userId, DateTime startDate)
        {
            int valid = -1;
            bool validIndicator = false;
            LeaveRequest lastLeave = new LeaveRequest();
            var allLeave = _leaveRepository.LeaveListByUserIdOnlyApprovedAndPending(userId);
            foreach (var leave in allLeave)
            {
                if (leave.EndDate.HasValue)
                    valid = leave.EndDate.Value.CompareTo(startDate);
                else
                    valid = leave.FromDate.CompareTo(startDate);
                if (valid >= 0)
                {
                    validIndicator = true;
                    break;
                }
                else
                    validIndicator = false;
            }
            return validIndicator;
        }
        #endregion
    }
}



