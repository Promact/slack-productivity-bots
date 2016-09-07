﻿using System;

namespace Promact.Erp.Util
{
    public class StringConstant
    {
        public static string EmailSubject = "Leave Application";
        public static string FromDate = "FromDate";
        public static string EndDate = "EndDate";
        public static string Reason = "Reason";
        public static string Type = "Type";
        public static string Status = "Status";
        public static string ReJoinDate = "ReJoinDate";
        public static string CreatedOn = "CreatedOn";
        public static string WebRequestContentType = "application/json; charset=UTF-8";
        public static string DateFormat = "dd,MM,yyyy";
        public static string ResponseTypeEphemeral = "ephemeral";
        public static string Approved = "Approved";
        public static string Button = "button";
        public static string Rejected = "Rejected";
        public static string Fallback = "Leave Applied";
        public static string Color = "#3AA3E3";
        public static string AttachmentType = "default";
        public static string CancelLeaveError = "You are trying with wrong leaveId which not belong to you";
        public static string SlackHelpMessage = string.Format("For leave apply: /leaves apply [Reason] [FromDate: dd-MM-yyyy] [EndDate: dd-MM-yyyy] [LeaveType] [RejoinDate: dd-MM-yyyy]{0}For leave list of Yours : /leaves list{0}For leave list of others : /leaves list [@user]{0}For leave Cancel : /leaves cancel [leave Id number]{0}For leave status of Yours : /leaves status{0}For leave status of others : /leaves status [@user]{0}For leaves balance: /leaves balance", Environment.NewLine);
        public static string SlackErrorMessage = "Sorry, I didn't quite get that. I'm easily confused. Perhaps try the words in a different order. For help : /leaves help";

        public static string ProjectDetailsUrl = "fetchProject/";
        public static string UsersDetailByGroupUrl = "fetchProjectUsers/";
        public static string UserDetailsByIdUrl = "fetchUserById/";
        public static string UserDetailByUserNameUrl = "fetchbyusername/";
        public static string UrlRtmStart = "https://slack.com/api/rtm.start";
        public static string OAuthAuthorizationScopeAndClientId = "?scope=incoming-webhook,commands,bot,users:read,groups:read&client_id=";
        public static string UserDetailsUrl = "userDetails/";
        public static string TeamLeaderDetailsUrl = "teamLeaderDetails/";
        public static string ManagementDetailsUrl = "managementDetails";
        public static string OAuthAcessUrl = "https://slack.com/api/oauth.access";
        public static string ProjectDetailsByUserNameUrl = "projectByUserName/";
        public static string ProjectUsersByTeamLeaderId = "projectUsersById/"; 
        public static string ProjectUserDetailsUrl = "project/userDetails/";
        public static string ProjectTeamLeaderDetailsUrl = "project/teamLeadersDetails/";
        public static string ProjectManagementDetailsUrl = "project/Admin";
        public static string UserDetailUrl = "userDetail/";
        public static string LoginUserDetail = "getByUserName/";
        public static string ThankYou = "Thank You";
        public static string InternalError = "Internal Error";
        public static string SlackUserListUrl = "https://slack.com/api/users.list";
        public static string SlackChannelListUrl = "https://slack.com/api/channels.list";
        public static string SlackGroupListUrl = "https://slack.com/api/groups.list";
        public static string TaskMailBotStatusErrorMessage = "Status should be completed/inprogress/roadblock";
        public static string TaskMailBotHourErrorMessage = "Please enter numeric value. And it should be in range of 0.5 to 8";
        public static string TaskMailDescription = "Descriptions";
        public static string TaskMailHours = "Hours";
        public static string TaskMailComment = "Comment";
        public static string TaskMailStatus = "Status";
        public static string ScrumTime = "scrum time";
        public static string ScrumHelp = "scrum help";
        public static string ScrumHelpMessage = "To automate your stand up meet.\nAdd me to your group.\nType *scrum time* to start your team's stand up meet.\nTeam members will be asked questions and only the person who is asked question must answer it.\n>If a person is on leave and asked question,then any team member can write *leave _team member's name_*.\nThe stand up meet has to be conducted in one go.\n>If it gets interrupted in any circumstances, you can resume it by typing the keyword *scrum time*.I will resume the stand up meet from where it had stopped.\nHope this helped.";
        public static string Leave = "leave";
        public static string ScrumBotToken = "ScrumBotToken";
        public static string ScrumBotName = "ScrumBotName";
        public static string ServerClosed = "Sorry :worried: \nWe cannot process your request due to technical glitches.Please try after some time";
        public static string ErrorGetProject = "Sorry :worried: \nProject could not be fetched";
        public static string ErrorGetEmployees = "Sorry :worried: \nEmployees could not be fetched";
        public static string NoQuestion = "Sorry I have nothing to ask you.";
        public static string NoEmployeeFound = "Sorry. No employees found for this project.";
        public static string NoProjectFound = "No project found for this group.";
        public static string ScrumComplete = "Scrum concluded.\nGood luck team :thumbsup:";
        public static string ScrumNotStarted = "Scrum has not been initiated yet";
        public static string ScrumAlreadyConducted = "Scrum for today has been concluded";
        public static string GoodDay = "Good Day ";
        public static string SendTaskMailConfirmationErrorMessage = "Please enter yes or no";
        public static string RequestToStartTaskMail = "Please start task mail";
        public static string AlreadyMailSend = "You have already sended mail for today. No more task mail for today";
        public static string TaskMailSubject = "Task Mail";
        public static string FirstNameForTest = "siddhartha";
        public static string AccessTokenForTest = "94d56876-fb02-45a9-8b01-c56046482d17";
        public static string EmailForTest = "siddhartha@promactinfo.com";
        public static string StringIdForTest = "13b0f2ca-92f5-4713-a67e-37e50172e148";
        public static string LastNameForTest = "shaw";
        public static string UserDetailsFromOauthServer = "{\"firstName\":\"roshni\",\"lastName\":\"Promact\",\"isActive\":false,\"numberOfCasualLeave\":0.0,\"numberOfSickLeave\":0.0,\"joiningDate\":\"0001-01-01T00:00:00\",\"slackUserName\":null,\"projects\":null,\"createdBy\":null,\"createdDateTime\":\"0001-01-01T00:00:00\",\"updatedBy\":null,\"updatedDateTime\":\"0001-01-01T00:00:00\",\"id\":\"13b0f2ca-92f5-4713-a67e-37e50172e148\",\"userName\":\"roshni@promactinfo.com\",\"normalizedUserName\":null,\"email\":\"roshni@promactinfo.com\",\"normalizedEmail\":null,\"emailConfirmed\":false,\"passwordHash\":null,\"securityStamp\":null,\"concurrencyStamp\":\"a39b2cff-51e2-4f1d-bde9-096cefb17497\",\"phoneNumber\":null,\"phoneNumberConfirmed\":false,\"twoFactorEnabled\":false,\"lockoutEnd\":null,\"lockoutEnabled\":false,\"accessFailedCount\":0,\"roles\":[],\"Role\":\"Admin\",\"claims\":[],\"logins\":[]}";
        public static string SlashCommandText = "Apply Hello 30-09-2016 30-09-2016 Casual 30-09-2016";
        public static string PromactStringName = "Promact";
        public static string QuestionForTest = "On which task you worked on Today?";
        public static string TeamLeaderDetailsFromOauthServer = "[{\"firstName\":null,\"lastName\":null,\"isActive\":false,\"numberOfCasualLeave\":0.0,\"numberOfSickLeave\":0.0,\"joiningDate\":\"0001-01-01T00:00:00\",\"slackUserName\":null,\"projects\":null,\"createdBy\":null,\"createdDateTime\":\"0001-01-01T00:00:00\",\"updatedBy\":null,\"updatedDateTime\":\"0001-01-01T00:00:00\",\"id\":\"24954a25-7af6-488e-9c5f-970958c62eeb\",\"userName\":\"gourav@promactinfo.com\",\"normalizedUserName\":null,\"email\":\"gourav@promactinfo.com\",\"normalizedEmail\":null,\"emailConfirmed\":false,\"passwordHash\":null,\"securityStamp\":null,\"concurrencyStamp\":\"00836465-b3d0-48ee-acb4-f0bff1c1834b\",\"phoneNumber\":null,\"phoneNumberConfirmed\":false,\"twoFactorEnabled\":false,\"lockoutEnd\":null,\"lockoutEnabled\":false,\"accessFailedCount\":0,\"roles\":[],\"claims\":[],\"logins\":[]}]";
        public static string CommentAndDescriptionForTest = "Doing Test Task";
        public static string ManagementDetailsFromOauthServer = "[{\"firstName\":\"roshni\",\"lastName\":null,\"isActive\":false,\"numberOfCasualLeave\":0.0,\"numberOfSickLeave\":0.0,\"joiningDate\":\"0001-01-01T00:00:00\",\"slackUserName\":null,\"projects\":null,\"createdBy\":null,\"createdDateTime\":\"0001-01-01T00:00:00\",\"updatedBy\":null,\"updatedDateTime\":\"0001-01-01T00:00:00\",\"id\":\"5e6d8293-98ad-4249-b4ff-d4baa5da09bb\",\"userName\":null,\"normalizedUserName\":null,\"email\":\"roshni@promactinfo.com\",\"normalizedEmail\":null,\"emailConfirmed\":false,\"passwordHash\":null,\"securityStamp\":null,\"concurrencyStamp\":\"22914d35-4125-4c89-b67f-bb2060ed4247\",\"phoneNumber\":null,\"phoneNumberConfirmed\":false,\"twoFactorEnabled\":false,\"lockoutEnd\":null,\"lockoutEnabled\":false,\"accessFailedCount\":0,\"roles\":[],\"claims\":[],\"logins\":[]}]";
        public static string TaskMailReport = "[{\"userName\":\"roshni@promactinfo.com\", \"name\":\"Admin Promact\", \"role\":\"TeamLeader\"},{\"userName\":\"test@promactinfo.com\",\"name\":\"test test test\",\"role\":\"Admin\"},{\"userName\":\"user2@promactinfo.com\",\"name\":\"user2 user2\",\"role\":\"Admin\"}]";
        public static string TeamLeaderEmailForTest = "gourav@promactinfo.com";
        public static string ManagementFirstForTest = "roshni";
        public static string ManagementEmailForTest = "roshni@promactinfo.com";
        public static string LeaveReasonForTest = "Testing";
        public static string LeaveTypeForTest = "Casual";
        public static string ChannelId = "channel_id";
        public static string ChannelName = "channel_name";
        public static string Command = "command";
        public static string ResponseUrl = "response_url";
        public static string TeamDomain = "team_domain";
        public static string TeamId = "team_id";
        public static string Text = "text";
        public static string Token = "token";
        public static string UserId = "user_id";
        public static string UserName = "user_name";
        public static string LeaveBot = "LeaveBot";
        public static string UnderConstruction = "Still under Construction";
        public static string Hello = "Hello All";
        public static string All = "All";
        public static string StringHourForTest = "4";
        public static string AfterLogIn = "AfterLogIn";
        public static string Home = "Home";
        public static string Index = "Index";
        public static string WebConfig = "~/web.config";
        public static string MailSetting = "system.net/mailSettings";
        public static string SlackChatUpdateUrl = "https://slack.com/api/chat.update";
        public static string ProjectUserUrl = "http://localhost:35716/api/ProjectUser/";
        public static string ProjectUrl = "http://localhost:35716/api/Project/";
        public static string UserUrl = "http://localhost:35716/api/User/";
        public static string OAuthUrl = "http://localhost:35716/OAuth/ExternalLogin";
        public static string ClientReturnUrl = "http://localhost:28182/Home/ExtrenalLoginCallBack";
        public static string LeaveManagementAuthorizationUrl = "https://slack.com/oauth/authorize";
        public static string ChatPostUrl = "https://slack.com/api/chat.postMessage";
        public static string SlashCommandLeaveListErrorMessage = string.Format("Leave doesn't exist for that user. Enter a valid slack username. {0}Example :- /leaves list username{0}/leaves list", Environment.NewLine);
        public static string SlashCommandLeaveCancelErrorMessage = "Please enter a valid leave cancel command. Example :- /leaves cancel 75";
        public static string SlashCommandLeaveStatusErrorMessage = string.Format("Leave doesn't exist for that user. Enter a valid slack username. {0}Example :- /leaves status username{0}/leaves status", Environment.NewLine);
        public static string Host = "Host";
        public static string Port = "Port";
        public static string From = "From";
        public static string Password = "Password";
        public static string EnableSsl = "EnableSsl";
        public static string IncomingWebHookUrl = "IncomingWebHookUrl";
        public static string TaskmailAccessToken = "DailyTaskMailAccessToken";
        public static string SlackOAuthClientId = "SlackOAuthClientId";
        public static string SlackOAuthClientSecret = "SlackOAuthClientSecret";
        public static string PromactOAuthClientId = "PromactOAuthClientId";
        public static string PromactOAuthClientSecret = "PromactOAuthClientSecret";
        public static string LoggerErrorMessageLeaveRequestControllerSlackRequest = "Error in Leave Request Controller-Slack Request";
        public static string LoggerErrorMessageLeaveRequestControllerSlackButtonRequest = "Error in Leave Request Controller-Slack Button Request";
        public static string LoggerErrorMessageHomeControllerExtrenalLogin = "Error in Home Controller-Extrenal Login ";
        public static string LoggerErrorMessageHomeControllerExtrenalLoginCallBack = "Error in Home Controller-Extrenal Login CallBack ";
        public static string LoggerErrorMessageHomeControllerLogoff = "Error in Home Controller-LogOff";
        public static string LoggerErrorMessageOAuthControllerRefreshToken = "Error in OAuth Controller-Refresh Token";
        public static string LoggerErrorMessageOAuthControllerSlackOAuth = "Error in OAuth Controller-Slack OAuth";
        public static string LoggerErrorMessageOAuthControllerSlackEvent = "Error in OAuth Controller-Slack Event";
        public static string LoggerErrorMessageTaskMailBot = "Error in Task Mail Bot";
        public static string SlackBotStringName = "slackbot";
        public static string CasualLeaveUrl = "casual/leave/";
        public static string CasualLeaveResponse = "10.0";

        #region String Constants for Test Cases

        public static string UserNameForTest = "apoorvapatel";
        public static string GroupName = "testbotgroup";
        public static string AnswerStatement = "Completed bot";
        public static string LeaveApplicant = "apoorvapatel";
        public static string ChannelIdForTest = "231asd";
        public static string ScrumQuestionForTest = "What did you do yesterday?";
        public static string ChannelNameForTest = "testbotgroup";
        public static string ProjectDetailsFromOauth = "{\"id\":2,\"name\":\"testbotgroup\",\"slackChannelName\":\"testbotgroup\",\"isActive\":true,\"teamLeaderId\":\"5c84049f-f861-406d-b420-e1bf03c9e06e\",\"createdBy\":\"1bac6614-7a2b-42fa-9f18-b6a19d8e25fb\",\"createdDate\":null,\"updatedBy\":null,\"updatedDate\":null,\"teamLeader\":null,\"applicationUsers\":null}";
        public static string EmployeeDetailsFromOauth = "{\"Id\":\"577696c8-136f-4865-8328-09e7d48ac58d\",\"FirstName\":\"Apoorva\",\"LastName\":\"Promact\",\"IsActive\":true,\"Email\":\"apoorvapatel@promactinfo.com\",\"Password\":null,\"UserName\":\"apoorvapatel\",\"UniqueName\":\"Apoorva-apoorvapatel@promactinfo.com\"}";
        public static string EmployeesListFromOauth = "[{\"Id\":\"577696c8-136f-4865-8328-09e7d48ac58d\",\"FirstName\":\"Apoorva\",\"LastName\":\"Promact\",\"IsActive\":true,\"Email\":\"apoorvapatel@promactinfo.com\",\"Password\":null,\"SlackUserName\":\"apoorvapatel\",\"UserName\":\"apoorvapatel\",\"UniqueName\":\"Apoorva-apoorvapatel@promactinfo.com\"},{\"Id\":\"aac59fbc-7835-4bd7-9080-6b6766302080\",\"FirstName\":\"Pranali\",\"LastName\":\"Promact\",\"IsActive\":true,\"Email\":\"pranali@promactinfo.com\",\"Password\":null,\"SlackUserName\":\"pranali\",\"UserName\":\"pranali\",\"UniqueName\":\"Pranali-pranali@promactinfo.com\"}]";
        public static string UserBySlackUserName = "{\"Id\":\"fce9e5de-0c3e-410f-8602-068e211d5f4d\",\"FirstName\":null,\"LastName\":null,\"IsActive\":false,\"Email\":null,\"Password\":null,\"UserName\":null,\"UniqueName\":\"-\"}";
        public static string UserBySlackUserNameForLeaveApplicant = "{\"Id\":\"fce9e5de-0c3e-410f-8602-068e211d5f4d\",\"FirstName\":null,\"LastName\":null,\"IsActive\":false,\"Email\":null,\"Password\":null,\"UserName\":null,\"UniqueName\":\"-\"}";
        public static string UserIdForTest = "577696c8-136f-4865-8328-09e7d48ac58d";
        public static string TestUser = "pranali";
        public static string scrumAnswerForTest = "Worked on testing";
        public static string ProjectIdForTest = "2";
        public static string TeamLeaderIdForTest = "5c84049f-f861-406d-b420-e1bf03c9e06e";

        
        public static string Admin = "Admin";
        public static string Employee = "Employee";
        public static string TeamLeader = "TeamLeader";
        public static string EmployeeIdForTest = "2d5f21e0-f7e7-4027-85ad-3faf8e1bf8bf";
        public static string TestAccessToken = "05310c2a-3fd3-4fa7-a059-e88bfbfe5f99";
        public static string TestUserName = "roshni@promactinfo.com";
        public static string TestUserNameFalse = "gourav@promactinfo.com";
        public static string FirstUserName = "roshni@promactinfo.com";
        public static string FirstUserNameFalse = "gourav@promactinfo.com";
        public static string ProjectUsers = "[{\"firstName\":\"roshni\",\"lastName\":null,\"isActive\":false,\"numberOfCasualLeave\":0.0,\"numberOfSickLeave\":0.0,\"joiningDate\":\"0001-01-01T00:00:00\",\"slackUserName\":null,\"projects\":null,\"createdBy\":null,\"createdDateTime\":\"0001-01-01T00:00:00\",\"updatedBy\":null,\"updatedDateTime\":\"0001-01-01T00:00:00\",\"id\":\"5e6d8293-98ad-4249-b4ff-d4baa5da09bb\",\"userName\":\"roshni@promactinfo.com\",\"normalizedUserName\":null,\"email\":\"roshni@promactinfo.com\",\"normalizedEmail\":null,\"emailConfirmed\":false,\"passwordHash\":null,\"securityStamp\":null,\"concurrencyStamp\":\"22914d35-4125-4c89-b67f-bb2060ed4247\",\"phoneNumber\":null,\"phoneNumberConfirmed\":false,\"twoFactorEnabled\":false,\"lockoutEnd\":null,\"lockoutEnabled\":false,\"accessFailedCount\":0,\"roles\":[],\"claims\":[],\"logins\":[]}]";
        public static string EmptyString = "";
        #endregion


        public static string ProjectInformationUrl = "featchUserRole/";

        public static string RoleAdmin = "Admin";
        public static string RoleTeamLeader = "TeamLeader";
        public static string RoleEmployee = "Employee";

        

    }
}
