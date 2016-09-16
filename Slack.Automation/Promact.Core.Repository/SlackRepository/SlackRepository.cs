using Promact.Core.Repository.LeaveRequestRepository;
using Promact.Core.Repository.ProjectUserCall;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Promact.Erp.Util;
using Promact.Core.Repository.Client;
using Promact.Core.Repository.AttachmentRepository;
using System.Linq;
using System.Globalization;
using Promact.Erp.DomainModel.DataRepository;
using System.Net.Mail;

namespace Promact.Core.Repository.SlackRepository
{
    public class SlackRepository : ISlackRepository
    {
        private readonly IProjectUserCallRepository _projectUser;
        private readonly ILeaveRequestRepository _leaveRepository;
        private readonly IClient _client;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IRepository<ApplicationUser> _userManager;
        string replyText = null;
        public SlackRepository(ILeaveRequestRepository leaveRepository, IProjectUserCallRepository projectUser, IClient client, IAttachmentRepository attachmentRepository, IRepository<ApplicationUser> userManager)
        {
            _projectUser = projectUser;
            _leaveRepository = leaveRepository;
            _client = client;
            _attachmentRepository = attachmentRepository;
            _userManager = userManager;
        }

        /// <summary>
        /// Method to apply leave
        /// </summary>
        /// <param name="slackRequest"></param>
        /// <param name="userName"></param>
        /// <returns>leaveRequest</returns>
        private async Task<string> LeaveApply(List<string> slackRequest, SlashCommand leave, string accessToken)
        {
            try
            {
                LeaveType leaveTypeExcepted;
                DateTime dateTime;
                User user = new User();
                // checking whether string can convert to date of indian culture or not, if return true then further process will be conduct
                var dateConvertorResult = DateTime.TryParseExact(slackRequest[3], "dd-MM-yyyy", CultureInfo.CreateSpecificCulture("hi-IN"), DateTimeStyles.None, out dateTime);
                if (dateConvertorResult)
                {
                    // converting string to date of indian culture
                    var startDate = DateTime.ParseExact(slackRequest[3], "dd-MM-yyyy", CultureInfo.CreateSpecificCulture("hi-IN"));
                    LeaveRequest leaveRequest = new LeaveRequest();
                    // checking whether string can convert to leave type or not, if return true then further process will be conduct
                    var leaveTypeConvertor = Enum.TryParse(slackRequest[1], out leaveTypeExcepted);
                    if (leaveTypeConvertor)
                    {
                        // converting string to leave type of indian culture
                        var leaveType = (LeaveType)Enum.Parse(typeof(LeaveType), slackRequest[1]);
                        switch (leaveType)
                        {
                            case LeaveType.cl:
                                {
                                    // checking whether string can convert to date of indian culture or not, if return true then further process will be conduct
                                    var firstDateConvertorResult = DateTime.TryParseExact(slackRequest[4], "dd-MM-yyyy", CultureInfo.CreateSpecificCulture("hi-IN"), DateTimeStyles.None, out dateTime);
                                    var secondDateConvertorResult = DateTime.TryParseExact(slackRequest[5], "dd-MM-yyyy", CultureInfo.CreateSpecificCulture("hi-IN"), DateTimeStyles.None, out dateTime);
                                    if (firstDateConvertorResult && secondDateConvertorResult)
                                    {
                                        // converting string to date time of indian culture
                                        var endDate = DateTime.ParseExact(slackRequest[4], "dd-MM-yyyy", CultureInfo.CreateSpecificCulture("hi-IN"));
                                        var reJoinDate = DateTime.ParseExact(slackRequest[5], "dd-MM-yyyy", CultureInfo.CreateSpecificCulture("hi-IN"));
                                        // get user details from oAuth server
                                        user = await _projectUser.GetUserByUsername(leave.Username, accessToken);
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
                                            leaveRequest.EmployeeId = user.Id;
                                            _leaveRepository.ApplyLeave(leaveRequest);
                                            replyText = _attachmentRepository.ReplyText(leave.Username, leaveRequest);
                                            // method to send slack notification and email to team leaders and management
                                            await _client.SendMessageWithAttachmentIncomingWebhook(leave, leaveRequest, accessToken);
                                        }
                                        else
                                            // if user doesn't exist in OAuth server then user can't apply leave, will get this message
                                            replyText = StringConstant.SorryYouCannotApplyLeave;
                                    }
                                    else
                                        // if date is not proper than date format error message will be send to user
                                        replyText = StringConstant.DateFormatErrorMessage;
                                }
                                break;
                            case LeaveType.sl:
                                {
                                    if (slackRequest.Count > 4)
                                        // get user details from oAuth server for other user
                                        user = await _projectUser.GetUserByUsername(slackRequest[4], accessToken);
                                    else
                                        // get user details from oAuth server for own
                                        user = await _projectUser.GetUserByUsername(leave.Username, accessToken);
                                    leaveRequest.Status = Condition.Approved;
                                    leaveRequest.Type = leaveType;
                                    leaveRequest.Reason = slackRequest[2];
                                    leaveRequest.FromDate = startDate;
                                    leaveRequest.CreatedOn = DateTime.UtcNow;
                                    // if user doesn't exist in OAuth server then user can't apply leave
                                    if (user.Id != null)
                                    {
                                        leaveRequest.EmployeeId = user.Id;
                                        _leaveRepository.ApplyLeave(leaveRequest);
                                        replyText = _attachmentRepository.ReplyTextSick(user.FirstName, leaveRequest);
                                    }
                                    else
                                        // if user doesn't exist in OAuth server then user can't apply leave, will get this message
                                        replyText = StringConstant.SorryYouCannotApplyLeave;
                                }
                                break;
                        }
                    }
                    else
                        // if leave type is not proper than not of leave type format error message will be send to user
                        replyText = StringConstant.NotTypeOfLeave;
                }
                else
                    // if date is not proper than date format error message will be send to user
                    replyText = StringConstant.DateFormatErrorMessage;
            }
            catch (SmtpException)
            {
                // error message will be send to email. But leave will be applied
                replyText = StringConstant.ErrorWhileSendingEmail;
            }
            catch (Exception ex)
            {
                replyText = ex.Message;
            }
            return replyText;
        }

        /// <summary>
        /// Method to get Employee Id from its userName and from its employeeId, to get list of leave
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>replyText as string</returns>
        private async Task<string> LeaveList(string userName, string accessToken)
        {
            // get user details from oAuth server
            var user = await _projectUser.GetUserByUsername(userName, accessToken);
            // get list of leave from user Id
            var leaveList = _leaveRepository.LeaveListByUserId(user.Id);
            // if leave exit then further will get list in string else 
            if (leaveList.Count() != 0)
            {
                foreach (var leave in leaveList)
                {
                    replyText += string.Format("{0} {1} {2} {3} {4} {5}", leave.Id, leave.Reason, leave.FromDate.ToShortDateString(), leave.EndDate.Value.ToShortDateString(), leave.Status, System.Environment.NewLine);
                }
            }
            else
            {
                // if leave doesnot exit for that user
                replyText = StringConstant.SlashCommandLeaveListErrorMessage;
            }
            return replyText;
        }

        /// <summary>
        /// Method to Cancel leave, only allowed to the applier of the leave to cancel the leave
        /// </summary>
        /// <param name="leaveId"></param>
        /// <param name="userName"></param>
        /// <returns>replyText as string</returns>
        private async Task<string> CancelLeave(int leaveId, string userName, string accessToken)
        {
            // get user details from oAuth server
            var user = await _projectUser.GetUserByUsername(userName, accessToken);
            // only authorize user of leave is allowed to cancel there leave
            if (user.Id == _leaveRepository.LeaveById(leaveId).EmployeeId)
            {
                // method to cancel leave
                var leave = _leaveRepository.CancelLeave(leaveId);
                replyText = string.Format("Your leave Id no: {0} From {1} To {2} has been {3}", leave.Id, leave.FromDate.ToShortDateString(), leave.EndDate.Value.ToShortDateString(), leave.Status);
            }
            else
            {
                // if leave doesn't exist or unauthorize trespass try to do
                replyText = string.Format("{0}{1}{2}", StringConstant.LeaveDoesnotExist, StringConstant.OrElseString, StringConstant.CancelLeaveError);
            }
            return replyText;
        }

        /// <summary>
        /// Method to get Employee Id from its userName and from its employeeId, to get last leave status
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>replyText as string</returns>
        private async Task<string> LeaveStatus(string userName, string accessToken)
        {
            // get user details from oAuth server
            var user = await _projectUser.GetUserByUsername(userName, accessToken);
            try
            {
                // get only last leave of the user
                var leave = _leaveRepository.LeaveListStatusByUserId(user.Id);
                replyText = string.Format("Your leave Id no: {0} From {1} To {2} for {3} is {4}", leave.Id, leave.FromDate.ToShortDateString(), leave.EndDate.Value.ToShortDateString(), leave.Reason, leave.Status);
            }
            catch (Exception)
            {
                // if leave doesn't exist 
                replyText = StringConstant.SlashCommandLeaveStatusErrorMessage;
            }
            return replyText;
        }

        /// <summary>
        /// Method to get leave Updated from slack button response
        /// </summary>
        /// <param name="leaveId"></param>
        /// <param name="status"></param>
        /// <returns>replyText</returns>
        public void UpdateLeave(SlashChatUpdateResponse leaveResponse)
        {
            // method to get leave by its id
            var leave = _leaveRepository.LeaveById(leaveResponse.CallbackId);
            // only pending status can be modified
            if (leave.Status == Condition.Pending)
            {
                if (leaveResponse.Actions.Value == StringConstant.Approved)
                {
                    leave.Status = Condition.Approved;
                }
                else
                {
                    leave.Status = Condition.Rejected;
                }
            }
            _leaveRepository.UpdateLeave(leave);

            replyText = string.Format("You had {0} Leave for {1} From {2} To {3} for Reason {4} will re-join by {5}",
                            leave.Status,
                            leaveResponse.User.Name,
                            leave.FromDate.ToShortDateString(),
                            leave.EndDate.Value.ToShortDateString(),
                            leave.Reason,
                            leave.RejoinDate.Value.ToShortDateString());
            _client.UpdateMessage(leaveResponse, replyText);
        }

        /// <summary>
        /// Method to Get Leave List on slack
        /// </summary>
        /// <param name="slackText"></param>
        /// <param name="leave"></param>
        /// <returns>replyText</returns>
        private async Task<string> SlackLeaveList(List<string> slackText, SlashCommand leave, string accessToken)
        {
            // if slackText count is more then 1 then its means that user want to get leave list of someone else
            if (slackText.Count > 1)
            {
                // other user slack user name
                var userName = slackText[1];
                // leave list of other user 
                replyText = await LeaveList(userName, accessToken);
            }
            else
            {
                // leave list of own
                replyText = await LeaveList(leave.Username, accessToken);
            }
            return replyText;
        }

        /// <summary>
        /// Method to cancel leave by its Id from slack
        /// </summary>
        /// <param name="slackText"></param>
        /// <param name="leave"></param>
        /// <returns>replyText</returns>
        private async Task<string> SlackLeaveCancel(List<string> slackText, SlashCommand leave, string accessToken)
        {
            int leaveIdType;
            // checking whether string can be convert to integer or not, true then only further process will be conduct
            var leaveIdResult = int.TryParse(slackText[1], out leaveIdType);
            if (leaveIdResult)
            {
                // converting string to integer
                var leaveId = Convert.ToInt32(slackText[1]);
                // method to cancel leave by its id
                replyText = await CancelLeave(leaveId, leave.Username, accessToken);
            }
            else
                // if string converting to integer return false then user will get cancel command error message
                replyText = StringConstant.SlashCommandLeaveCancelErrorMessage;
            return replyText;
        }

        /// <summary>
        /// Method to get last leave status and details on slack
        /// </summary>
        /// <param name="slackText"></param>
        /// <param name="leave"></param>
        /// <returns>replyText</returns>
        private async Task<string> SlackLeaveStatus(List<string> slackText, SlashCommand leave, string accessToken)
        {
            // if slackText count is more then 1 then its means that user want to get leave list of someone else
            if (slackText.Count > 1)
            {
                // other user slack user name
                var userName = slackText[1];
                // last leave details of other user 
                replyText = await LeaveStatus(userName, accessToken);
            }
            else
            {
                // last leave details of own
                replyText = await LeaveStatus(leave.Username, accessToken);
            }
            return replyText;
        }


        /// <summary>
        /// Method to check leave Balance from slack
        /// </summary>
        /// <param name="leave"></param>
        /// <param name="accessToken"></param>
        /// <returns>replyText</returns>
        private async Task<string> SlackLeaveBalance(SlashCommand leave, string accessToken)
        {
            // get user details from oAuth server
            var user = await _projectUser.GetUserByUsername(leave.Username, accessToken);
            if (user.Id != null)
            {
                // get user leave allowed details from oAuth server
                var allowedLeave = await _projectUser.CasualLeave(leave.Username, accessToken);
                // method to get user's number of leave taken
                var leaveTaken = _leaveRepository.NumberOfLeaveTaken(user.Id);
                var casualLeaveTaken = leaveTaken.CasualLeave;
                var sickLeaveTaken = leaveTaken.SickLeave;
                var casualLeaveLeft = allowedLeave.CasualLeave - casualLeaveTaken;
                var sickLeaveLeft = allowedLeave.SickLeave - sickLeaveTaken;
                replyText = string.Format("You have taken {0} casual leave out of {1}{2}You have casual leave left {3}", casualLeaveTaken, allowedLeave.CasualLeave, Environment.NewLine, casualLeaveLeft);
                replyText += string.Format("{2}You have taken {0} sick leave out of {1}{2}You have sick leave left {3}", casualLeaveTaken, allowedLeave.SickLeave, Environment.NewLine, sickLeaveLeft);
            }
            else
            {
                // if user doesn't exist in Oauth server
                replyText = StringConstant.LeaveNoUserErrorMessage;
            }
            return replyText;
        }

        /// <summary>
        /// Method for gettin help on slack regards Leave slash command
        /// </summary>
        /// <param name="leave"></param>
        private string SlackLeaveHelp(SlashCommand leave)
        {
            var replyText = StringConstant.SlackHelpMessage;
            return replyText;
        }

        public async Task LeaveRequest(SlashCommand leave)
        {
            SlackAction actionType;
            // method to convert slash command to list of string
            var slackText = _attachmentRepository.SlackText(leave.Text);
            // to get user details by slack user name
            var user = _userManager.FirstOrDefault(x => x.SlackUserName == leave.Username);
            if (user != null)
            {
                leave.Text.ToLower();
                // getting access token of user of promact oauth server
                var accessToken = await _attachmentRepository.AccessToken(user.UserName);
                // checking whether string ca convert to Slack Action or not, if true then further process will be conduct
                var actionConvertResult = Enum.TryParse(slackText[0], out actionType);
                if (actionConvertResult)
                {
                    // convert string to slack action type
                    var action = (SlackAction)Enum.Parse(typeof(SlackAction), slackText[0]);
                    switch (action)
                    {
                        case SlackAction.apply:
                            replyText = await LeaveApply(slackText, leave, accessToken);
                            break;
                        case SlackAction.list:
                            replyText = await SlackLeaveList(slackText, leave, accessToken);
                            break;
                        case SlackAction.cancel:
                            replyText = await SlackLeaveCancel(slackText, leave, accessToken);
                            break;
                        case SlackAction.status:
                            replyText = await SlackLeaveStatus(slackText, leave, accessToken);
                            break;
                        case SlackAction.balance:
                            replyText = await SlackLeaveBalance(leave, accessToken);
                            break;
                        case SlackAction.update:
                            await UpdateSickLeave(slackText, user, accessToken);
                            break;
                        default:
                            replyText = SlackLeaveHelp(leave);
                            break;
                    }
                }
                else
                    // if error in converting then user will get message to enter proper action command
                    replyText = StringConstant.RequestToEnterProperAction;
            }
            else
                // if user doesn't exist then will get message of user doesn't exist and ask to externally logic from Oauth server
                replyText = StringConstant.SorryYouCannotApplyLeave;
            _client.SendMessage(leave, replyText);
        }

        /// <summary>
        /// Method to send error message to user od slack
        /// </summary>
        /// <param name="leave"></param>
        public void Error(SlashCommand leave)
        {
            // if something error will happen user will get this message
            var replyText = string.Format("{0}{1}{2}{1}{3}", StringConstant.LeaveNoUserErrorMessage, Environment.NewLine, StringConstant.OrElseString, StringConstant.SlackErrorMessage);
            _client.SendMessage(leave, replyText);
        }

        /// <summary>
        /// Method to update sick leave, only admin allowed
        /// </summary>
        /// <param name="slackText"></param>
        /// <param name="user"></param>
        /// <param name="accessToken"></param>
        /// <returns>replyText</returns>
        private async Task<string> UpdateSickLeave(List<string> slackText, ApplicationUser user, string accessToken)
        {
            // checking from oAuth whether user is Admin or not
            var IsAdmin = await _projectUser.UserIsAdmin(user.UserName, accessToken);
            if (IsAdmin)
            {
                int leaveIdType;
                DateTime dateTime;
                // checking whether string can be convert to integer or not, true then only further process will be conduct
                var leaveIdResult = int.TryParse(slackText[1], out leaveIdType);
                if (leaveIdResult)
                {
                    var leave = _leaveRepository.LeaveById(Convert.ToInt32(slackText[1]));
                    if (leave != null && leave.Type == LeaveType.sl)
                    {
                        // checking whether string can convert to date of indian culture or not, if return true then further process will be conduct
                        var firstDateConvertorResult = DateTime.TryParseExact(slackText[2], "dd-MM-yyyy", CultureInfo.CreateSpecificCulture("hi-IN"), DateTimeStyles.None, out dateTime);
                        var secondDateConvertorResult = DateTime.TryParseExact(slackText[3], "dd-MM-yyyy", CultureInfo.CreateSpecificCulture("hi-IN"), DateTimeStyles.None, out dateTime);
                        if (firstDateConvertorResult && secondDateConvertorResult)
                        {

                            // convert string to date of indian culture
                            leave.EndDate = DateTime.ParseExact(slackText[2], "dd-MM-yyyy", CultureInfo.CreateSpecificCulture("hi-IN"));
                            leave.RejoinDate = DateTime.ParseExact(slackText[3], "dd-MM-yyyy", CultureInfo.CreateSpecificCulture("hi-IN"));
                            _leaveRepository.UpdateLeave(leave);
                            replyText = string.Format("Sick leave of {0} from {1} to {2} for reason {3} has been updated, will rejoin on {4}"
                                , user.SlackUserName, leave.FromDate.ToShortDateString(), leave.EndDate.Value.ToShortDateString(),
                                leave.Reason, leave.RejoinDate.Value.ToShortDateString());
                        }
                        else
                            // if date is not proper than date format error message will be send to user
                            replyText = StringConstant.DateFormatErrorMessage;
                    }
                    else
                        // if sick leave will doesn't exist for leaveId
                        replyText = StringConstant.SickLeaveDoesnotExist;
                }
                else
                    // if string converting to integer return false then user will get cancel command error message
                    replyText = StringConstant.UpdateEnterAValidLeaveId;
            }
            else
                // if user is not admin then this message will be show to user
                replyText = StringConstant.AdminErrorMessage;
            return replyText;
        }
    }
}



