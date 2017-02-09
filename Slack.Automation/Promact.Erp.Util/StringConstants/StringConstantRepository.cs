using System;

namespace Promact.Erp.Util.StringConstants
{
    public class StringConstantRepository : IStringConstantRepository
    {
        public StringConstantRepository()
        {

        }


        public string EmailSubject
        {
            get
            {
                return "Leave Application";
            }
        }
        public string FromDate
        {
            get
            {
                return "FromDate";
            }
        }
        public string EndDate
        {
            get
            {
                return "EndDate";
            }
        }
        public string Reason
        {
            get
            {
                return "Reason";
            }
        }
        public string Type
        {
            get
            {
                return "Type";
            }
        }
        public string Status
        {
            get
            {
                return "Status";
            }
        }
        public string ReJoinDate
        {
            get
            {
                return "ReJoinDate";
            }
        }
        public string CreatedOn
        {
            get
            {
                return "CreatedOn";
            }
        }
        public string WebRequestContentType
        {
            get
            {
                return "application/json; charset=UTF-8";
            }
        }
        public string DateFormat
        {
            get
            {
                return "dd-MM-yyyy";
            }
        }
        public string ResponseTypeEphemeral
        {
            get
            {
                return "ephemeral";
            }
        }
        public string Approved
        {
            get
            {
                return "Approved";
            }
        }
        public string Button
        {
            get
            {
                return "button";
            }
        }
        public string Rejected
        {
            get
            {
                return "Rejected";
            }
        }
        public string Fallback
        {
            get
            {
                return "Leave Applied";
            }
        }
        public string Color
        {
            get
            {
                return "#3AA3E3";
            }
        }
        public string AttachmentType
        {
            get
            {
                return "default";
            }
        }
        public string CancelLeaveError
        {
            get
            {
                return "You are trying with wrong leave id, which does not belong to you";
            }
        }
        public string SlackHelpMessage
        {
            get
            {
                return string.Format(
                    "To apply casual leave: /leaves apply cl [Reason(for long reason write in \" \")] [FromDate] [EndDate] [RejoinDate]"+
                    "{0}To apply sick leave: /leaves apply sl [Reason] [FromDate]"+
                    "{0}To apply sick leave by admin for other: /leaves apply sl [Reason] [FromDate] [username]" +
                    "{0}For leaves list of yours : /leaves list" +
                    "{0}For leaves list of others : /leaves list [username]"+
                    "{0}For leave cancel : /leaves cancel [leave Id number]" +
                    "{0}For sick leave update(only by admin): / leaves update [leave id] [EndDate] [RejoinDate]" +
                    "{0}For leave status of yours : /leaves status"+
                    "{0}For leave status of others : /leaves status [username]" +
                    "{0}For leaves balance: /leaves balance", Environment.NewLine);
            }
        }
        public string SlackErrorMessage
        {
            get
            {
                return "I didn't quite get that. I'm easily confused. Perhaps try the words in a different order."+
                    " For help : /leaves help";
            }
        }
        public string UsersDetailByChannelNameUrl
        {
            get
            {
                return "slackChannel/";
            }
        }

        public string UserDetailsByIdUrl
        {
            get
            {
                return "fetchUserById/";
            }
        }
        public string UrlRtmStart
        {
            get
            {
                return "https://slack.com/api/rtm.start";
            }
        }
        public string UserCouldNotBeAdded
        {
            get
            {
                return "User could not be added";
            }
        }
        public string OAuthAuthorizationScopeAndClientId
        {
            get
            {
                return "?scope=incoming-webhook,commands,bot,users:read,groups:read,channels:read,chat:write:bot,chat:write:user&client_id=";
            }
        }
        public string SlackAuthorize
        {
            get
            {
                return "SlackAuthorize";
            }
        }
        public string UserDetailsUrl
        {
            get
            {
                return "user/";
            }
        }
        public string TeamLeaderDetailsUrl
        {
            get
            {
                return "teamLeaders/";
            }
        }
        public string ManagementDetailsUrl
        {
            get
            {
                return "managements";
            }
        }
        public string OAuthAcessUrl
        {
            get
            {
                return "https://slack.com/api/oauth.access";
            }
        }
        public string ProjectDetailsByUserNameUrl
        {
            get
            {
                return "projectByUserName/";
            }
        }
        public string ProjectUsersByTeamLeaderId
        {
            get
            {
                return "/project";
            }
        }
        public string ProjectUserDetailsUrl
        {
            get
            {
                return "project/userDetails/";
            }
        }
        public string ProjectTeamLeaderDetailsUrl
        {
            get
            {
                return "project/teamLeadersDetails/";
            }
        }
        public string ProjectManagementDetailsUrl
        {
            get
            {
                return "project/Admin";
            }
        }
        public string UserDetailUrl
        {
            get
            {
                return "/detail";
            }
        }

        public string ThankYou
        {
            get
            {
                return "Thank You";
            }
        }
        public string InternalError
        {
            get
            {
                return "Internal Error";
            }
        }
        public string SlackUserListUrl
        {
            get
            {
                return "https://slack.com/api/users.list";
            }
        }
        public string SlackChannelListUrl
        {
            get
            {
                return "https://slack.com/api/channels.list";
            }
        }
        public string SlackGroupListUrl
        {
            get
            {
                return "https://slack.com/api/groups.list";
            }
        }
        public string TaskMailBotStatusErrorMessage
        {
            get
            {
                return "Status should be completed/inprogress/roadblock";
            }
        }
        public string TaskMailBotHourErrorMessage
        {
            get
            {
                return "Please enter a numeric value. And it should be in the range of 0.5 to 8";
            }
        }
        public string ScrumInProgress
        {
            get
            {
                return "Scrum is in progress";
            }
        }
        public string SlackUserNotFound
        {
            get
            {
                return "Slack details of user have not been found.";
            }
        }
        public string TaskMailDescription
        {
            get
            {
                return "Descriptions";
            }
        }
        public string TaskMailHours
        {
            get
            {
                return "Hours";
            }
        }
        public string TaskMailComment
        {
            get
            {
                return "Comment";
            }
        }
        public string TaskMailStatus
        {
            get
            {
                return "Status";
            }
        }
        public string StartBot
        {
            get
            {
                return "start <@ScrumBotName>";
            }
        }
        public string ScrumHalt
        {
            get
            {
                return "scrum halt";
            }
        }
        public string ChannelAddSuccess
        {
            get
            {
                return "Channel details have been added";
            }
        }
        public string Add
        {
            get
            {
                return "add";
            }
        }

        public string ChannelAddInstruction
        {
            get
            {
                return "The channel is not in our database. Please write the command *add channel _channelname_* , if it is added as a project in OAuth server.";
            }
        }
        public string ProjectNotInOAuth
        {
            get
            {
                return "This channel is not registered as Project in OAuth. Please add it to OAuth first";
            }
        }
        public string UserNotInOAuthOrProject
        {
            get
            {
                return "User not in OAuth or not a user in the project";
            }
        }
        public string GroupNameStartsWith
        {
            get
            {
                return "G";
            }
        }
        public string OnlyPrivateChannel
        {
            get
            {
                return "Only private channels can be added manually.";
            }
        }

        public string ScrumHalted
        {
            get
            {
                return "Scrum has been halted";
            }
        }
        public string ScrumIsHalted
        {
            get
            {
                return "Scrum is halted\n";
            }
        }
        public string ScrumAlreadyHalted
        {
            get
            {
                return "Scrum is already halted. Enter *scrum resume* to resume scrum";
            }
        }
        public string ScrumResume
        {
            get
            {
                return "scrum resume";
            }
        }
        public string Scrum
        {
            get
            {
                return "scrum";
            }
        }
        public string ScrumResumed
        {
            get
            {
                return "Scrum has been resumed\n";
            }
        }
        public string AnswerToday
        {
            get
            {
                return "*Please answer the following questions today*";
            }
        }
        public string ScrumNotHalted
        {
            get
            {
                return "Scrum was not halted\n";
            }
        }
        public string InValidStartCommand
        {
            get
            {
                return "Invalid command . Try *_start <@{0}>_*";
            }
        }
        public string ScrumHelp
        {
            get
            {
                return "scrum help";
            }
        }
        public string ScrumHelpMessage
        {
            get
            {
                return "To automate your stand up meet.\nLogin with Promact and add me to your channel.\nType *start <@{0}>* to start your team's stand up meet.\nTeam members will be asked questions and only the person who is asked question must answer it.\n>If a person is on leave and asked question,then any team member can write *leave _@team member's name_*.\n>Members who are marked as in-active in OAuth will not be asked questions.\nThe stand up meet has to be conducted in one go.\n>If it gets interrupted in any circumstances, you can start from where you left by typing the keyword *start <@{0}>*.\nScrum can be halted by writing *scrum halt* and it can be resumed by *scrum resume*. \nHope this helped.\n\n_P.S. If these instructions are not followed, I might misbehave_.\n_My apologies in advance :wink:_";
            }
        }
        public string NotAUser
        {
            get
            {
                return "Please login with Promact first.";
            }
        }
        public string NotAProject
        {
            get
            {
                return "Please add Project details of the channel on Promact";
            }
        }
        public string Leave
        {
            get
            {
                return "leave";
            }
        }
        public string Later
        {
            get
            {
                return "later";
            }
        }
        public string LeaveError
        {
            get
            {
                return "How can you mark yourself on leave ? :joy:";
            }
        }
        public string ResumeScrum
        {
            get
            {
                return "Please resume the scrum by writing *scrum resume*";
            }
        }
        public string PreviousDayStatus
        {
            get
            {
                return "*Your scrum status of {0} is :*\n";
            }
        }
        public string ScrumBotToken
        {
            get
            {
                return "ScrumBotToken";
            }
        }
        public string ScrumBotName
        {
            get
            {
                return "ScrumBotName";
            }
        }
        public string LoggerScrumBot
        {
            get
            {
                return "Error in Scrum Bot : ";
            }
        }
        public string LoggerTaskMailBot
        {
            get
            {
                return "Error in Tasl Mail Bot : ";
            }
        }
        public string ScrumLaterDone
        {
            get
            {
                return "Good luck <@{0}> ! You have answered all scrum questions.";
            }
        }
        public string AlreadyAnswered
        {
            get
            {
                return "But,<@{0}>'s answers have been recorded today :worried:\n";
            }
        }
        public string NotExpected
        {
            get
            {
                return "Sorry. <@{0}> is not expected to answer now.\n";
            }
        }

        public string NoQuestion
        {
            get
            {
                return "Sorry I have nothing to ask you.";
            }
        }
        public string NoEmployeeFound
        {
            get
            {
                return "Sorry. No active employee found for this project.";
            }
        }

        public string Unrecognized
        {
            get
            {
                return "Sorry :worried: I don't know who you are. Please contact your administrator";
            }
        }

        public string NoProjectFound
        {
            get
            {
                return "No active project found for this channel.";
            }
        }
        public string ScrumComplete
        {
            get
            {
                return "Scrum concluded.\nGood luck team :thumbsup:";
            }
        }
        public string ScrumNotStarted
        {
            get
            {
                return "Scrum has not been initiated yet";
            }
        }

        public string NameFormat
        {
            get
            {
                return "<@{0}> ";
            }
        }
        public string AnswerNotRecorded
        {
            get
            {
                return "Answer could not be recorded";
            }
        }

        public string PreviousDayScrumAnswer
        {
            get
            {
                return "*_Q_*: {0}\r\n*_A_*: _{1}_\r\n";
            }
        }

        public string ScrumCannotBeHalted
        {
            get
            {
                return " Scrum cannot be halted.";
            }
        }

        public string ScrumCannotBeResumed
        {
            get
            {
                return "Scrum cannot be resumed.";
            }
        }

        public string ProjectInActive
        {
            get
            {
                return "Project is marked as Inactive in Promact OAuth.\n";
            }
        }

        public string ScrumAlreadyConducted
        {
            get
            {
                return "Scrum for today has been concluded";
            }
        }
        public string NoSlackDetails
        {
            get
            {
                return string.Format("Sorry we do not have your slack details."+
                    " Click here {0}", AppSettingUtil.PromactErpUrl);
            }
        }
        public string MarkedInActive
        {
            get
            {
                return "<@{0}>,you were marked as In-active or not in OAuth before or did not answer this answer before due to technical glitches.\n Please answer ";
            }
        }
        public string Channel
        {
            get
            {
                return "channel";
            }
        }
        public string GoodDay
        {
            get
            {
                return "Good Day <@{0}>!\n";
            }
        }
        public string Time
        {
            get
            {
                return "time";
            }
        }
        public string UserNotInSlack
        {
            get
            {
                return "User is not in Slack or has not added app to slack yet\n";
            }
        }
        public string NotInSlackOrNotExpectedUser
        {
            get
            {
                return "User with email id {0} has not logged in with promact yet. Please authenticate with Promact OAuth server to add app to Slack";
            }
        }
        public string PleaseAnswer
        {
            get
            {
                return "I am expecting <@{0}> to answer.";
            }
        }
        public string ScrumConcludedButLater
        {
            get
            {
                return "Scrum wound up for now.\nGood luck team :thumbsup:. \nTo conduct scrum of employees marked as _later_ type *_scrum @username_*";
            }
        }
        public string AllAnswerRecorded
        {
            get
            {
                return "No questions are marked to be asked later";
            }
        }
        public string NotLaterYet
        {
            get
            {
                return "<@{0}> has not been marked to answer later yet.\n";
            }
        }
        public string SendTaskMailConfirmationErrorMessage
        {
            get
            {
                return "Please enter yes or no";
            }
        }
        public string RequestToStartTaskMail
        {
            get
            {
                return "To add task please start, write *task mail*";
            }
        }
        public string AlreadyMailSend
        {
            get
            {
                return "You have already send mail for today. No more task mail for today";
            }
        }
        public string TaskMailSubject
        {
            get
            {
                return "Task Mail";
            }
        }
        public string FirstNameForTest
        {
            get
            {
                return "siddhartha";
            }
        }
        public string AccessTokenForTest
        {
            get
            {
                return "94d56876-fb02-45a9-8b01-c56046482d17";
            }
        }
        public string EmailForTest
        {
            get
            {
                return "siddhartha@promactinfo.com";
            }
        }
        public string UserNotFound
        {
            get
            {
                return "User Not Found";
            }
        }
        public string BotNotFound
        {
            get
            {
                return "Bot Not Found";
            }
        }
        public string StringIdForTest
        {
            get
            {
                return "13b0f2ca-92f5-4713-a67e-37e50172e148";
            }
        }
        public string LastNameForTest
        {
            get
            {
                return "shaw";
            }
        }
        public string UserChange
        {
            get
            {
                return "user_change";
            }
        }
        public string ChannelRename
        {
            get
            {
                return "channel_rename";
            }
        }
        public string GroupRename
        {
            get
            {
                return "group_rename";
            }
        }
        public string ChannelCreated
        {
            get
            {
                return "channel_created";
            }
        }
        public string ChannelArchive
        {
            get
            {
                return "channel_archive";
            }
        }
        public string GroupArchive
        {
            get
            {
                return "group_archive";
            }
        }
        public string UserDetailsFromOauthServer
        {
            get
            {
                return "{\"firstName\":\"siddhartha\",\"lastName\":\"Promact\",\"isActive\":false,\"numberOfCasualLeave\":10.0,\"numberOfSickLeave\":5.0,\"joiningDate\":\"0001-01-01T00:00:00\",\"slackUserId\":\"U0HJ49KJ4\",\"slackUserName\":\"siddhartha\",\"slackUserId\":\"U0HJ49KJ4\",\"projects\":null,\"createdBy\":null,\"createdDateTime\":\"0001-01-01T00:00:00\",\"updatedBy\":null,\"updatedDateTime\":\"0001-01-01T00:00:00\",\"id\":\"13b0f2ca-92f5-4713-a67e-37e50172e148\",\"userName\":\"roshni@promactinfo.com\",\"normalizedUserName\":null,\"email\":\"roshni@promactinfo.com\",\"normalizedEmail\":null,\"emailConfirmed\":false,\"passwordHash\":null,\"securityStamp\":null,\"concurrencyStamp\":\"a39b2cff-51e2-4f1d-bde9-096cefb17497\",\"phoneNumber\":null,\"phoneNumberConfirmed\":false,\"twoFactorEnabled\":false,\"lockoutEnd\":null,\"lockoutEnabled\":false,\"accessFailedCount\":0,\"roles\":[],\"Role\":\"Admin\",\"claims\":[],\"logins\":[]}";
            }
        }

        public string UserDetailsFromOauthServerFalse
        {
            get
            {
                return "{\"firstName\":\"siddhartha\",\"lastName\":\"Promact\",\"isActive\":false,\"numberOfCasualLeave\":10.0,\"numberOfSickLeave\":5.0,\"joiningDate\":\"0001-01-01T00:00:00\",\"slackUserId\":\"U0HJ49KJ4\",\"slackUserName\":\"siddhartha\",\"slackUserId\":\"U0HJ49KJ4\",\"projects\":null,\"createdBy\":null,\"createdDateTime\":\"0001-01-01T00:00:00\",\"updatedBy\":null,\"updatedDateTime\":\"0001-01-01T00:00:00\",\"id\":\"13b0f2ca-92f5-4713-a67e-37e50172e148\",\"userName\":\"roshni@promactinfo.com\",\"normalizedUserName\":null,\"email\":\"roshni@promactinfo.com\",\"normalizedEmail\":null,\"emailConfirmed\":false,\"passwordHash\":null,\"securityStamp\":null,\"concurrencyStamp\":\"a39b2cff-51e2-4f1d-bde9-096cefb17497\",\"phoneNumber\":null,\"phoneNumberConfirmed\":false,\"twoFactorEnabled\":false,\"lockoutEnd\":null,\"lockoutEnabled\":false,\"accessFailedCount\":0,\"roles\":[],\"Role\":\"Management\",\"claims\":[],\"logins\":[]}";
            }
        }

        public string SlashCommandText
        {
            get
            {
                return string.Format("apply cl Testing {0} {0} {1}",
                    DateTime.UtcNow.ToShortDateString(), DateTime.UtcNow.AddDays(1).ToShortDateString());
            }
        }
        public string PromactStringName
        {
            get
            {
                return "Promact";
            }
        }
        public string FirstQuestionForTest
        {
            get
            {
                return "On which task did you work today?";
            }
        }
        public string TeamLeaderDetailsFromOauthServer
        {
            get
            {
                return "[{ \"firstName\":null,\"lastName\":null,\"isActive\":false,\"numberOfCasualLeave\":0.0,\"numberOfSickLeave\":0.0,\"joiningDate\":\"0001-01-01T00:00:00\",\"slackUserId\":\"U0HJ34YU9\",\"slackUserName\":null,\"projects\":null,\"createdBy\":null,\"createdDateTime\":\"0001-01-01T00:00:00\",\"updatedBy\":null,\"updatedDateTime\":\"0001-01-01T00:00:00\",\"id\":\"13b0f2ca-92f5-4713-a67e-37e50172e148\",\"userName\":\"gourav@promactinfo.com\",\"normalizedUserName\":null,\"email\":\"gourav@promactinfo.com\",\"normalizedEmail\":null,\"emailConfirmed\":false,\"passwordHash\":null,\"securityStamp\":null,\"concurrencyStamp\":\"be27bae7-b292-4c57-917e-abb88793b8c1\",\"phoneNumber\":null,\"phoneNumberConfirmed\":false,\"twoFactorEnabled\":false,\"lockoutEnd\":null,\"lockoutEnabled\":false,\"accessFailedCount\":0,\"roles\":[],\"claims\":[],\"logins\":[]}]";
            }
        }
        public string CommentAndDescriptionForTest
        {
            get
            {
                return "Doing Test Task";
            }
        }
        public string ManagementDetailsFromOauthServer
        {
            get
            {
                return "[{\"firstName\":\"roshni\",\"lastName\":null,\"isActive\":false,\"numberOfCasualLeave\":0.0,\"numberOfSickLeave\":0.0,\"joiningDate\":\"0001-01-01T00:00:00\",\"slackUserId\":\"U0525LCJR\",\"slackUserName\":null,\"projects\":null,\"createdBy\":null,\"createdDateTime\":\"0001-01-01T00:00:00\",\"updatedBy\":null,\"updatedDateTime\":\"0001-01-01T00:00:00\",\"id\":\"13b0f2ca-92f5-4713-a67e-37e50172e148\",\"userName\":null,\"normalizedUserName\":null,\"email\":\"roshni@promactinfo.com\",\"normalizedEmail\":null,\"emailConfirmed\":false,\"passwordHash\":null,\"securityStamp\":null,\"concurrencyStamp\":\"22914d35-4125-4c89-b67f-bb2060ed4247\",\"phoneNumber\":null,\"phoneNumberConfirmed\":false,\"twoFactorEnabled\":false,\"lockoutEnd\":null,\"lockoutEnabled\":false,\"accessFailedCount\":0,\"roles\":[],\"claims\":[],\"logins\":[]}]";
            }
        }
        public string TaskMailReport
        {
            get
            {
                return "[{\"userId\":\"null\",\"userName\":\"roshni@promactinfo.com\", \"name\":\"Admin Promact\", \"role\":\"TeamLeader\"},{\"userId\":\"null\",\"userName\":\"siddhartha@promactinfo.com\",\"name\":\"test test test\",\"role\":\"Employee\"},{\"userId\":\"null\",\"userName\":\"user2@promactinfo.com\",\"name\":\"user2 user2\",\"role\":\"Employee\"}]";
            }
        }
        public string ListOfEmployeeForTeamLeader
        {
            get
            {
                return "[{\"userId\":\"null\",\"userName\":\"roshni@promactinfo.com\", \"name\":\"Promact\", \"role\":\"Employee\"},{\"userId\":\"null\",\"userName\":\"siddhartha@promactinfo.com\",\"name\":\"test test test\",\"role\":\"TeamLeader\"},{\"userId\":\"null\",\"userName\":\"user2@promactinfo.com\",\"name\":\"user2 user2\",\"role\":\"Employee\"}]";
            }
        }
        public string EmployeeInformation
        {
            get
            {
                return "[{\"userId\":\"null\",\"userName\":\"siddhartha@promactinfo.com\", \"name\":\"Promact\", \"role\":\"Employee\"}]";
            }
        }
        public string TaskMailReportTeamLeader
        {
            get
            {
                return "[{\"userId\":\"1\",\"userName\":\"roshni@promactinfo.com\", \"name\":\"Admin Promact\", \"role\":\"TeamLeader\"},{\"userId\":\"null\",\"userName\":\"test@promactinfo.com\",\"name\":\"test test test\",\"role\":\"Admin\"},{\"userId\":\"null\",\"userName\":\"user2@promactinfo.com\",\"name\":\"user2 user2\",\"role\":\"Admin\"}]";
            }
        }
        public string TeamLeaderEmailForTest
        {
            get
            {
                return "gourav@promactinfo.com";
            }
        }
        public string ManagementFirstForTest
        {
            get
            {
                return "roshni";
            }
        }
        public string ManagementEmailForTest
        {
            get
            {
                return "roshni@promactinfo.com";
            }
        }
        public string LeaveReasonForTest
        {
            get
            {
                return "Testing";
            }
        }
        public string LeaveTypeForTest
        {
            get
            {
                return "Casual";
            }
        }
        public string ChannelId
        {
            get
            {
                return "channel_id";
            }
        }
        public string ChannelName
        {
            get
            {
                return "channel_name";
            }
        }
        public string Command
        {
            get
            {
                return "command";
            }
        }
        public string ResponseUrl
        {
            get
            {
                return "response_url";
            }
        }
        public string TeamDomain
        {
            get
            {
                return "team_domain";
            }
        }
        public string TeamId
        {
            get
            {
                return "team_id";
            }
        }
        public string Text
        {
            get
            {
                return "text";
            }
        }
        public string Token
        {
            get
            {
                return "token";
            }
        }
        public string UserId
        {
            get
            {
                return "user_id";
            }
        }
        public string UserName
        {
            get
            {
                return "user_name";
            }
        }
        public string LeaveBot
        {
            get
            {
                return "LeaveBot";
            }
        }
        public string UnderConstruction
        {
            get
            {
                return "Still under Construction";
            }
        }
        public string Hello
        {
            get
            {
                return "Hello All";
            }
        }
        public string All
        {
            get
            {
                return "All";
            }
        }
        public string StringHourForTest
        {
            get
            {
                return "4";
            }
        }
        public string AfterLogIn
        {
            get
            {
                return "AfterLogIn";
            }
        }
        public string Home
        {
            get
            {
                return "Home";
            }
        }
        public string Index
        {
            get
            {
                return "Index";
            }
        }
        public string WebConfig
        {
            get
            {
                return "~/web.config";
            }
        }
        public string MailSetting
        {
            get
            {
                return "system.net/mailSettings";
            }
        }
        public string SlackChatUpdateUrl
        {
            get
            {
                return "https://slack.com/api/chat.update";
            }
        }
        public string ProjectUserUrl
        {
            get
            {
                return string.Format("{0}api/users/", AppSettingUtil.OAuthUrl);
            }
        }
        public string ProjectUrl
        {
            get
            {
                return string.Format("{0}api/project/", AppSettingUtil.OAuthUrl);
            }
        }
        public string UserUrl
        {
            get
            {
                return string.Format("{0}api/users/", AppSettingUtil.OAuthUrl);
            }
        }
        public string OAuthUrl
        {
            get
            {
                return string.Format("{0}OAuth/ExternalLogin", AppSettingUtil.OAuthUrl);
            }
        }
        public string ClientReturnUrl
        {
            get
            {
                return string.Format("{0}Home/ExtrenalLoginCallBack", AppSettingUtil.PromactErpUrl);
            }
        }
        public string LeaveManagementAuthorizationUrl
        {
            get
            {
                return "https://slack.com/oauth/authorize";
            }
        }
        public string ChatPostUrl
        {
            get
            {
                return "https://slack.com/api/chat.postMessage";
            }
        }
        public string SlashCommandLeaveListErrorMessage
        {
            get
            {
                return string.Format("Either leave doesn't exist for that user or enter a valid slack username." +
                    "{0}Example :-{0}/leaves list username - for other user{0}/leaves list - for own", Environment.NewLine);
            }
        }
        public string SlashCommandLeaveCancelErrorMessage
        {
            get
            {
                return "Please enter a valid leave cancel command. Example :- /leaves cancel 75";
            }
        }
        public string SlashCommandLeaveStatusErrorMessage
        {
            get
            {
                return string.Format("Either leave doesn't exist for that user or enter a valid slack username."+
                    " {0}Example :-{0}/leaves status username - for other user{0}/leaves status - for own", Environment.NewLine);
            }
        }
        public string Host
        {
            get
            {
                return "Host";
            }
        }
        public string Port
        {
            get
            {
                return "Port";
            }
        }
        public string TokenEmpty
        {
            get
            {
                return "Could not obtain Bot Token";
            }
        }
        public string ErrorMsg
        {
            get
            {
                return "Sorry. Something bad happened :face_with_head_bandage: . Please try after sometime.";
            }
        }
        public string From
        {
            get
            {
                return "From";
            }
        }
        public string Password
        {
            get
            {
                return "Password";
            }
        }
        public string EnableSsl
        {
            get
            {
                return "EnableSsl";
            }
        }
        public string IncomingWebHookUrl
        {
            get
            {
                return "IncomingWebHookUrl";
            }
        }
        public string TaskmailAccessToken
        {
            get
            {
                return "DailyTaskMailAccessToken";
            }
        }
        public string SlackOAuthClientId
        {
            get
            {
                return "SlackOAuthClientId";
            }
        }
        public string SlackOAuthClientSecret
        {
            get
            {
                return "SlackOAuthClientSecret";
            }
        }
        public string PromactOAuthClientId
        {
            get
            {
                return "PromactOAuthClientId";
            }
        }
        public string PromactOAuthClientSecret
        {
            get
            {
                return "PromactOAuthClientSecret";
            }
        }
        public string LoggerErrorMessageLeaveRequestControllerSlackRequest
        {
            get
            {
                return "Error in Leave Request Controller-Slack Request";
            }
        }
        public string LoggerErrorMessageLeaveRequestControllerSlackButtonRequest
        {
            get
            {
                return "Error in Leave Request Controller-Slack Button Request";
            }
        }
        public string SlackAppAdded
        {
            get
            {
                return "Promact Slack app has been added successfully";
            }
        }
        public string LoggerErrorMessageHomeControllerAuthorizeStatusPage
        {
            get
            {
                return "Error in Home Controller-Status Page after Authorize ";
            }
        }
        public string LoggerErrorMessageOAuthControllerSlackDetailsAdd
        {
            get
            {
                return "Error in OAuth Controller-Slack Details Add";
            }
        }
        public string SlackAuthError
        {
            get
            {
                return "There is something wrong internally.The response from Slack is : ";
            }
        }
        public string SlackAppError
        {
            get
            {
                return "Promact Slack app was not added successfully. ";
            }
        }
        public string LoggerErrorMessageHomeControllerExtrenalLogin
        {
            get
            {
                return "Error in Home Controller-Extrenal Login ";
            }
        }
        public string LoggerErrorMessageHomeControllerExtrenalLoginCallBack
        {
            get
            {
                return "Error in Home Controller-Extrenal Login CallBack ";
            }
        }
        public string LoggerErrorMessageHomeControllerSlackOAuthAuthorization
        {
            get
            {
                return "Error in Home Controller-Slack OAuth Authorization ";
            }
        }
        public string LoggerErrorMessageHomeControllerLogoff
        {
            get
            {
                return "Error in Home Controller-LogOff";
            }
        }
        public string LoggerErrorMessageOAuthControllerRefreshToken
        {
            get
            {
                return "Error in OAuth Controller-Refresh Token";
            }
        }
        public string LoggerErrorMessageOAuthControllerSlackOAuth
        {
            get
            {
                return "Error in OAuth Controller-Slack OAuth";
            }
        }
        public string LoggerErrorMessageOAuthControllerSlackEvent
        {
            get
            {
                return "Error in OAuth Controller-Slack Event";
            }
        }
        public string LoggerErrorMessageTaskMailBot
        {
            get
            {
                return "Error in Task Mail Bot";
            }
        }
        public string SlackBotName
        {
            get
            {
                return "slackbot";
            }
        }
        public string CasualLeaveUrl
        {
            get
            {
                return "leaveAllowed/";
            }
        }
        public string CasualLeaveResponse
        {
            get
            {
                return "{\"casualLeave\":10.0,\"sickLeave\":5.0}";
            }
        }
        public string SlackChannelIdForTest
        {
            get
            {
                return "DA4ADFD44";
            }
        }
        public string MessageTsForTest
        {
            get
            {
                return "5212201241.15452124";
            }
        }
        public string SorryYouCannotApplyLeave
        {
            get
            {
                return string.Format("Sorry you cannot use leave slash command."+
                    " Either user is not in Promact OAuth or yet u haven't login in promact-slack server."+
                    " Click here {0}", AppSettingUtil.PromactErpUrl);
            }
        }
        public string LeaveListCommandForTest
        {
            get
            {
                return "list siddhartha";
            }
        }
        public string LeaveCancelCommandForTest
        {
            get
            {
                return "cancel 2";
            }
        }
        public string Cancel
        {
            get
            {
                return "Cancel";
            }
        }
        public string FalseStringNameForTest
        {
            get
            {
                return "Tester";
            }
        }
        public string LeaveNoUserErrorMessage
        {
            get
            {
                return "Either you are not in Promact OAuth or yet u haven't login in promact-slack server";
            }
        }
        public string LeaveBalanceReplyTextForTest
        {
            get
            {
                return "You have taken 0 casual leave out of 10\r\nYou are left with 10 casual leave";
            }
        }
        public string OrElseString
        {
            get
            {
                return "Or Else";
            }
        }
        public string UserNotInProject
        {
            get
            {
                return "<@{0}> is not included in the project(of this group in OAuth)\n";
            }
        }
        public string Admin
        {
            get
            {
                return "Admin";
            }
        }
        public string Employee
        {
            get
            {
                return "Employee";
            }
        }
        public string TeamLeader
        {
            get
            {
                return "TeamLeader";
            }
        }
        public string EmptyString
        {
            get
            {
                return "";
            }
        }
        public string VerificationUrl
        {
            get
            {
                return "url_verification";
            }
        }
        public string TeamJoin
        {
            get
            {
                return "team_join";
            }
        }
        public string SlackOAuthResponseText
        {
            get
            {
                return "{\n    \"ok\": true,\n    \"access_token\": \"adfkajfklasjksejflkasmadsfjbajfbjak\",\n    \"scope\": \"identify,bot,commands,incoming-webhook,channels:read,groups:read,users:read\",\n    \"user_id\": \"U0HJ49KJ4\",\n    \"team_name\": \"Promact\",\n    \"team_id\": \"T04K6NL66\",\n    \"incoming_webhook\": {\n        \"channel\": \"@slackbot\",\n        \"channel_id\": \"D0HHZPADB\",\n        \"configuration_url\": \"https:\\/\\/promact.slack.com\\/services\\/B2903B812\",\n        \"url\": \"https:\\/\\/hooks.slack.com\\/services\\/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\"\n    },\n    \"bot\": {\n        \"bot_user_id\": \"U257ZD738\",\n        \"bot_access_token\": \"xoxb-73271449110-VS4o6LS0U0x48o8qSOWLvTm7\"\n    }\n}\n";
            }
        }
        public string AccessTokenSlack
        {
            get
            {
                return "adfkajfklasjksejflkasmadsfjbajfbjak";
            }
        }
        public string UserDetailsResponseText
        {
            get
            {
                return "{\n    \"ok\": true,\n    \"members\": [\n        {\n            \"id\": \"U0J4V5EEQ\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"amritha\",\n            \"deleted\": true,\n            \"profile\": {\n                \"avatar_hash\": \"gc6315f20ced\",\n                \"real_name\": \"\",\n                \"real_name_normalized\": \"\",\n                \"email\": \"amritha@promactinfo.com\",\n                \"image_24\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/c6315f20ced3719a347f58858d819259.jpg?s=24&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0009-24.png\",\n                \"image_32\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/c6315f20ced3719a347f58858d819259.jpg?s=32&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0009-32.png\",\n                \"image_48\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/c6315f20ced3719a347f58858d819259.jpg?s=48&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0009-48.png\",\n                \"image_72\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/c6315f20ced3719a347f58858d819259.jpg?s=72&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0009-72.png\",\n                \"image_192\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/c6315f20ced3719a347f58858d819259.jpg?s=192&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0009-192.png\",\n                \"image_512\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/c6315f20ced3719a347f58858d819259.jpg?s=512&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0009-512.png\"\n            }\n        },\n        {\n            \"id\": \"U0545BH7Q\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"anitha\",\n            \"deleted\": true,\n            \"profile\": {\n                \"first_name\": \"Anitha\",\n                \"last_name\": \"Aloysius\",\n                \"title\": \"\",\n                \"skype\": \"\",\n                \"phone\": \"\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2015-08-01\\/8517493251_ddff49947c31bca2eee3_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2015-08-01\\/8517493251_ddff49947c31bca2eee3_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2015-08-01\\/8517493251_ddff49947c31bca2eee3_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2015-08-01\\/8517493251_ddff49947c31bca2eee3_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2015-08-01\\/8517493251_ddff49947c31bca2eee3_192.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2015-08-01\\/8517493251_ddff49947c31bca2eee3_original.jpg\",\n                \"avatar_hash\": \"ddff49947c31\",\n                \"real_name\": \"Anitha Aloysius\",\n                \"real_name_normalized\": \"Anitha Aloysius\",\n                \"email\": \"anitha@promactinfo.com\"\n            }\n        },\n        {\n            \"id\": \"U0526N37K\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"ankit\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"84b22f\",\n            \"real_name\": \"Ankit Bhanvadia\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57553279616_16ee7dc121c31f341572_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57553279616_16ee7dc121c31f341572_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57553279616_16ee7dc121c31f341572_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57553279616_16ee7dc121c31f341572_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57553279616_16ee7dc121c31f341572_192.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57553279616_16ee7dc121c31f341572_original.jpg\",\n                \"first_name\": \"Ankit\",\n                \"last_name\": \"Bhanvadia\",\n                \"title\": \"Software Developer\",\n                \"skype\": \"\",\n                \"phone\": \"7405860836\",\n                \"avatar_hash\": \"16ee7dc121c3\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57553279616_16ee7dc121c31f341572_192.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57553279616_16ee7dc121c31f341572_192.jpg\",\n                \"real_name\": \"Ankit Bhanvadia\",\n                \"real_name_normalized\": \"Ankit Bhanvadia\",\n                \"email\": \"ankit@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U1JBA2Z5G\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"apoorvapatel\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"d58247\",\n            \"real_name\": \"Apoorva Patel\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"avatar_hash\": \"51809e1af170\",\n                \"first_name\": \"Apoorva\",\n                \"last_name\": \"Patel\",\n                \"title\": \"QA (Trainee)\",\n                \"phone\": \"\",\n                \"skype\": \"\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-11\\/58419737907_51809e1af170a1df91d3_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-11\\/58419737907_51809e1af170a1df91d3_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-11\\/58419737907_51809e1af170a1df91d3_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-11\\/58419737907_51809e1af170a1df91d3_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-11\\/58419737907_51809e1af170a1df91d3_192.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-11\\/58419737907_51809e1af170a1df91d3_512.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-11\\/58419737907_51809e1af170a1df91d3_512.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-11\\/58419737907_51809e1af170a1df91d3_original.jpg\",\n                \"real_name\": \"Apoorva Patel\",\n                \"real_name_normalized\": \"Apoorva Patel\",\n                \"email\": \"apoorva.patel@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U0525MFND\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"apurva\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"9e3997\",\n            \"real_name\": \"Apurva Parikh\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"first_name\": \"Apurva\",\n                \"last_name\": \"Parikh\",\n                \"avatar_hash\": \"5e15bf36ca3c\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-06\\/57169185523_5e15bf36ca3ce90a400f_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-06\\/57169185523_5e15bf36ca3ce90a400f_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-06\\/57169185523_5e15bf36ca3ce90a400f_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-06\\/57169185523_5e15bf36ca3ce90a400f_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-06\\/57169185523_5e15bf36ca3ce90a400f_192.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-06\\/57169185523_5e15bf36ca3ce90a400f_512.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-06\\/57169185523_5e15bf36ca3ce90a400f_512.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-06\\/57169185523_5e15bf36ca3ce90a400f_original.jpg\",\n                \"title\": \"QA\",\n                \"phone\": \"\",\n                \"skype\": \"\",\n                \"real_name\": \"Apurva Parikh\",\n                \"real_name_normalized\": \"Apurva Parikh\",\n                \"email\": \"apurva@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U0525L7NT\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"ashik\",\n            \"deleted\": true,\n            \"profile\": {\n                \"first_name\": \"Ashik\",\n                \"last_name\": \"Patel\",\n                \"title\": \"Don't trust in your memory. Trust your comments;\",\n                \"skype\": \"ashik.promact\",\n                \"phone\": \"9687348466\",\n                \"avatar_hash\": \"gbe2b3b1878a\",\n                \"real_name\": \"Ashik Patel\",\n                \"real_name_normalized\": \"Ashik Patel\",\n                \"email\": \"ashik@promactinfo.com\",\n                \"image_24\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/be2b3b1878a3a99e00d16a91b4edb456.jpg?s=24&d=https%3A%2F%2Fa.slack-edge.com%2F0180%2Fimg%2Favatars%2Fava_0004-24.png\",\n                \"image_32\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/be2b3b1878a3a99e00d16a91b4edb456.jpg?s=32&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0004-32.png\",\n                \"image_48\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/be2b3b1878a3a99e00d16a91b4edb456.jpg?s=48&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0004-48.png\",\n                \"image_72\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/be2b3b1878a3a99e00d16a91b4edb456.jpg?s=72&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0004-72.png\",\n                \"image_192\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/be2b3b1878a3a99e00d16a91b4edb456.jpg?s=192&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0004-192.png\",\n                \"image_512\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/be2b3b1878a3a99e00d16a91b4edb456.jpg?s=512&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0004-512.png\"\n            }\n        },\n        {\n            \"id\": \"U051J17PA\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"ashwini\",\n            \"deleted\": true,\n            \"profile\": {\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5052212516_b4f17b66027995531470_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5052212516_b4f17b66027995531470_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5052212516_b4f17b66027995531470_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5052212516_b4f17b66027995531470_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5052212516_b4f17b66027995531470_192.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5052212516_b4f17b66027995531470_original.jpg\",\n                \"avatar_hash\": \"b4f17b660279\",\n                \"real_name\": \"\",\n                \"real_name_normalized\": \"\",\n                \"email\": \"ashwini@promactinfo.com\"\n            }\n        },\n        {\n            \"id\": \"U05263AL9\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"asrakhan\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"827327\",\n            \"real_name\": \"Asra Khan\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"first_name\": \"Asra\",\n                \"last_name\": \"Khan\",\n                \"avatar_hash\": \"d84eb27c00aa\",\n                \"title\": \"Web Designer\",\n                \"phone\": \"\",\n                \"skype\": \"\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-11\\/58420802947_d84eb27c00aa40a63143_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-11\\/58420802947_d84eb27c00aa40a63143_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-11\\/58420802947_d84eb27c00aa40a63143_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-11\\/58420802947_d84eb27c00aa40a63143_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-11\\/58420802947_d84eb27c00aa40a63143_192.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-11\\/58420802947_d84eb27c00aa40a63143_512.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-11\\/58420802947_d84eb27c00aa40a63143_512.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-11\\/58420802947_d84eb27c00aa40a63143_original.jpg\",\n                \"real_name\": \"Asra Khan\",\n                \"real_name_normalized\": \"Asra Khan\",\n                \"email\": \"asra@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U09G8CUJ3\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"ayushi\",\n            \"deleted\": true,\n            \"profile\": {\n                \"avatar_hash\": \"g1b214344e3d\",\n                \"real_name\": \"\",\n                \"real_name_normalized\": \"\",\n                \"email\": \"ayushi@promactinfo.com\",\n                \"image_24\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/1b214344e3d6dadcbcec32d80bbc1f28.jpg?s=24&d=https%3A%2F%2Fa.slack-edge.com%2F0180%2Fimg%2Favatars%2Fava_0025-24.png\",\n                \"image_32\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/1b214344e3d6dadcbcec32d80bbc1f28.jpg?s=32&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0025-32.png\",\n                \"image_48\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/1b214344e3d6dadcbcec32d80bbc1f28.jpg?s=48&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0025-48.png\",\n                \"image_72\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/1b214344e3d6dadcbcec32d80bbc1f28.jpg?s=72&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0025-72.png\",\n                \"image_192\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/1b214344e3d6dadcbcec32d80bbc1f28.jpg?s=192&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0025-192.png\",\n                \"image_512\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/1b214344e3d6dadcbcec32d80bbc1f28.jpg?s=512&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0025-512.png\"\n            }\n        },\n        {\n            \"id\": \"U051KDCAL\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"bhavin\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"4ec0d6\",\n            \"real_name\": \"Bhavin Khatri\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"first_name\": \"Bhavin\",\n                \"last_name\": \"Khatri\",\n                \"avatar_hash\": \"gab1c6c6b207\",\n                \"title\": \"Software Developer\",\n                \"phone\": \"\",\n                \"skype\": \"\",\n                \"real_name\": \"Bhavin Khatri\",\n                \"real_name_normalized\": \"Bhavin Khatri\",\n                \"email\": \"bhavin@promactinfo.com\",\n                \"image_24\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/ab1c6c6b2078fd53aeaa40e00cd554be.jpg?s=24&d=https%3A%2F%2Fa.slack-edge.com%2F0180%2Fimg%2Favatars%2Fava_0004-24.png\",\n                \"image_32\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/ab1c6c6b2078fd53aeaa40e00cd554be.jpg?s=32&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0004-32.png\",\n                \"image_48\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/ab1c6c6b2078fd53aeaa40e00cd554be.jpg?s=48&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0004-48.png\",\n                \"image_72\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/ab1c6c6b2078fd53aeaa40e00cd554be.jpg?s=72&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0004-72.png\",\n                \"image_192\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/ab1c6c6b2078fd53aeaa40e00cd554be.jpg?s=192&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0004-192.png\",\n                \"image_512\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/ab1c6c6b2078fd53aeaa40e00cd554be.jpg?s=512&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0004-512.png\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U07B75H0T\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"bot_test\",\n            \"deleted\": true,\n            \"profile\": {\n                \"bot_id\": \"B07B6UT70\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2015-07-08\\/7380925298_c485b6c4fedad6687b81_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2015-07-08\\/7380925298_c485b6c4fedad6687b81_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2015-07-08\\/7380925298_c485b6c4fedad6687b81_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2015-07-08\\/7380925298_c485b6c4fedad6687b81_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2015-07-08\\/7380925298_c485b6c4fedad6687b81_192.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2015-07-08\\/7380925298_c485b6c4fedad6687b81_original.jpg\",\n                \"avatar_hash\": \"c485b6c4feda\",\n                \"real_name\": \"\",\n                \"real_name_normalized\": \"\"\n            },\n            \"is_bot\": true\n        },\n        {\n            \"id\": \"U04KUAFSH\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"chintan\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"4bbe2e\",\n            \"real_name\": \"Chintan Shah\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"first_name\": \"Chintan\",\n                \"last_name\": \"Shah\",\n                \"title\": \"The Boss\",\n                \"skype\": \"chintan.promact\",\n                \"phone\": \"9898306658\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5079011985_3bb791320c98772d295a_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5079011985_3bb791320c98772d295a_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5079011985_3bb791320c98772d295a_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5079011985_3bb791320c98772d295a_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5079011985_3bb791320c98772d295a_192.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5079011985_3bb791320c98772d295a_original.jpg\",\n                \"avatar_hash\": \"3bb791320c98\",\n                \"real_name\": \"Chintan Shah\",\n                \"real_name_normalized\": \"Chintan Shah\",\n                \"email\": \"chintanshah@promactinfo.com\"\n            },\n            \"is_admin\": true,\n            \"is_owner\": true,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U26NWUT4H\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"dailytaskmail\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"7d414c\",\n            \"real_name\": \"Task Mail\",\n            \"tz\": null,\n            \"tz_label\": \"Pacific Daylight Time\",\n            \"tz_offset\": -25200,\n            \"profile\": {\n                \"bot_id\": \"B26P38LLS\",\n                \"api_app_id\": \"\",\n                \"avatar_hash\": \"70b5896b8e9b\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-31\\/74777392085_70b5896b8e9b55e67eef_24.png\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-31\\/74777392085_70b5896b8e9b55e67eef_32.png\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-31\\/74777392085_70b5896b8e9b55e67eef_48.png\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-31\\/74777392085_70b5896b8e9b55e67eef_72.png\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-31\\/74777392085_70b5896b8e9b55e67eef_192.png\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-31\\/74777392085_70b5896b8e9b55e67eef_192.png\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-31\\/74777392085_70b5896b8e9b55e67eef_192.png\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-31\\/74777392085_70b5896b8e9b55e67eef_original.png\",\n                \"first_name\": \"Task\",\n                \"last_name\": \"Mail\",\n                \"title\": \"Daily Task Mail Report\",\n                \"real_name\": \"Task Mail\",\n                \"real_name_normalized\": \"Task Mail\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": true\n        },\n        {\n            \"id\": \"U0525QHQ1\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"devashreepol\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"902d59\",\n            \"real_name\": \"Devashree Pol\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"first_name\": \"Devashree\",\n                \"last_name\": \"Pol\",\n                \"title\": \"QA\",\n                \"skype\": \"\",\n                \"phone\": \"\",\n                \"avatar_hash\": \"e69fb9e827aa\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57910020592_e69fb9e827aa6ae06192_24.png\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57910020592_e69fb9e827aa6ae06192_32.png\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57910020592_e69fb9e827aa6ae06192_48.png\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57910020592_e69fb9e827aa6ae06192_72.png\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57910020592_e69fb9e827aa6ae06192_192.png\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57910020592_e69fb9e827aa6ae06192_192.png\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57910020592_e69fb9e827aa6ae06192_192.png\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57910020592_e69fb9e827aa6ae06192_original.png\",\n                \"real_name\": \"Devashree Pol\",\n                \"real_name_normalized\": \"Devashree Pol\",\n                \"email\": \"devashree@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U1MVA0H3M\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"dharakraj\",\n            \"deleted\": true,\n            \"profile\": {\n                \"avatar_hash\": \"2235f5535c11\",\n                \"first_name\": \"Raj\",\n                \"last_name\": \"Dharak\",\n                \"title\": \"Software Developer Trainee\",\n                \"phone\": \"7069973499\",\n                \"skype\": \"\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57555903923_2235f5535c1149279742_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57555903923_2235f5535c1149279742_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57555903923_2235f5535c1149279742_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57555903923_2235f5535c1149279742_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57555903923_2235f5535c1149279742_192.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57555903923_2235f5535c1149279742_512.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57555903923_2235f5535c1149279742_512.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57555903923_2235f5535c1149279742_original.jpg\",\n                \"real_name\": \"Raj Dharak\",\n                \"real_name_normalized\": \"Raj Dharak\",\n                \"email\": \"raj@promactinfo.com\"\n            }\n        },\n        {\n            \"id\": \"U0525NC63\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"dharmesh\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"50a0cf\",\n            \"real_name\": \"Dharmesh Pipariya\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-06\\/57232190503_2df4202a7dc11b53c704_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-06\\/57232190503_2df4202a7dc11b53c704_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-06\\/57232190503_2df4202a7dc11b53c704_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-06\\/57232190503_2df4202a7dc11b53c704_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-06\\/57232190503_2df4202a7dc11b53c704_192.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-06\\/57232190503_2df4202a7dc11b53c704_original.jpg\",\n                \"first_name\": \"Dharmesh\",\n                \"last_name\": \"Pipariya\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-06\\/57232190503_2df4202a7dc11b53c704_192.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-06\\/57232190503_2df4202a7dc11b53c704_192.jpg\",\n                \"avatar_hash\": \"2df4202a7dc1\",\n                \"title\": \"Sr. Web Designer\",\n                \"phone\": \"\",\n                \"skype\": \"\",\n                \"real_name\": \"Dharmesh Pipariya\",\n                \"real_name_normalized\": \"Dharmesh Pipariya\",\n                \"email\": \"dharmesh@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U051HV7DA\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"dhruti\",\n            \"deleted\": false,\n            \"status\": \"?\",\n            \"color\": \"2b6836\",\n            \"real_name\": \"Dhruti Shah\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"first_name\": \"Dhruti\",\n                \"last_name\": \"Shah\",\n                \"title\": \"Team Lead\",\n                \"skype\": \"dhruti1.promact\",\n                \"phone\": \"\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-12\\/58853983600_fd7c9a6efb4346884f36_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-12\\/58853983600_fd7c9a6efb4346884f36_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-12\\/58853983600_fd7c9a6efb4346884f36_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-12\\/58853983600_fd7c9a6efb4346884f36_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-12\\/58853983600_fd7c9a6efb4346884f36_192.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-12\\/58853983600_fd7c9a6efb4346884f36_original.jpg\",\n                \"avatar_hash\": \"fd7c9a6efb43\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-12\\/58853983600_fd7c9a6efb4346884f36_192.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-12\\/58853983600_fd7c9a6efb4346884f36_192.jpg\",\n                \"real_name\": \"Dhruti Shah\",\n                \"real_name_normalized\": \"Dhruti Shah\",\n                \"email\": \"dhruti@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U05256USX\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"divyangibhatt\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"e7392d\",\n            \"real_name\": \"Divyangi Bhatt\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2015-11-26\\/15370097249_9ed3c4a550a2def99211_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2015-11-26\\/15370097249_9ed3c4a550a2def99211_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2015-11-26\\/15370097249_9ed3c4a550a2def99211_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2015-11-26\\/15370097249_9ed3c4a550a2def99211_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2015-11-26\\/15370097249_9ed3c4a550a2def99211_192.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2015-11-26\\/15370097249_9ed3c4a550a2def99211_original.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2015-11-26\\/15370097249_9ed3c4a550a2def99211_192.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2015-11-26\\/15370097249_9ed3c4a550a2def99211_192.jpg\",\n                \"avatar_hash\": \"9ed3c4a550a2\",\n                \"first_name\": \"Divyangi\",\n                \"last_name\": \"Bhatt\",\n                \"title\": \"HR & Admin Manager\",\n                \"phone\": \"+919925036216\",\n                \"skype\": \"divyangi.promact\",\n                \"real_name\": \"Divyangi Bhatt\",\n                \"real_name_normalized\": \"Divyangi Bhatt\",\n                \"email\": \"divyangi@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U0HJ34YU9\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"gourav\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"e7392d\",\n            \"real_name\": \"Gourav Agarwal\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"avatar_hash\": \"3448eda38866\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-09-04\\/76105835728_3448eda38866bb832398_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-09-04\\/76105835728_3448eda38866bb832398_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-09-04\\/76105835728_3448eda38866bb832398_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-09-04\\/76105835728_3448eda38866bb832398_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-09-04\\/76105835728_3448eda38866bb832398_192.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-09-04\\/76105835728_3448eda38866bb832398_512.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-09-04\\/76105835728_3448eda38866bb832398_1024.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-09-04\\/76105835728_3448eda38866bb832398_original.jpg\",\n                \"first_name\": \"Gourav\",\n                \"last_name\": \"Agarwal\",\n                \"title\": \"Junior .net Developer\",\n                \"phone\": \"8000399639\",\n                \"skype\": \"\",\n                \"real_name\": \"Gourav Agarwal\",\n                \"real_name_normalized\": \"Gourav Agarwal\",\n                \"email\": \"gourav@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U1M83P1U0\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"grishma_ukani\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"9e3997\",\n            \"real_name\": \"Grishma Ukani\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"avatar_hash\": \"2a2fac197c60\",\n                \"first_name\": \"Grishma\",\n                \"last_name\": \"Ukani\",\n                \"title\": \"Android_Developer Trainee\",\n                \"phone\": \"7405588239\",\n                \"skype\": \"\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-16\\/60391733552_2a2fac197c6092a4b6a7_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-16\\/60391733552_2a2fac197c6092a4b6a7_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-16\\/60391733552_2a2fac197c6092a4b6a7_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-16\\/60391733552_2a2fac197c6092a4b6a7_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-16\\/60391733552_2a2fac197c6092a4b6a7_192.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-16\\/60391733552_2a2fac197c6092a4b6a7_512.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-16\\/60391733552_2a2fac197c6092a4b6a7_512.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-16\\/60391733552_2a2fac197c6092a4b6a7_original.jpg\",\n                \"real_name\": \"Grishma Ukani\",\n                \"real_name_normalized\": \"Grishma Ukani\",\n                \"email\": \"grishma@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U0H7R5MPS\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"hardikbhatt\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"73769d\",\n            \"real_name\": \"Hardik Bhatt\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"first_name\": \"Hardik\",\n                \"last_name\": \"Bhatt\",\n                \"avatar_hash\": \"71dde8f067e2\",\n                \"title\": \"Quality Analyst\",\n                \"phone\": \"7572900750\",\n                \"skype\": \"hardikbhatt1792\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-23\\/62482714388_71dde8f067e240633384_24.png\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-23\\/62482714388_71dde8f067e240633384_32.png\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-23\\/62482714388_71dde8f067e240633384_48.png\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-23\\/62482714388_71dde8f067e240633384_72.png\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-23\\/62482714388_71dde8f067e240633384_192.png\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-23\\/62482714388_71dde8f067e240633384_512.png\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-23\\/62482714388_71dde8f067e240633384_1024.png\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-23\\/62482714388_71dde8f067e240633384_original.png\",\n                \"real_name\": \"Hardik Bhatt\",\n                \"real_name_normalized\": \"Hardik Bhatt\",\n                \"email\": \"hardik@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U0525MXL3\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"hardini\",\n            \"deleted\": true,\n            \"profile\": {\n                \"first_name\": \"Hardini\",\n                \"last_name\": \"Thakkar\",\n                \"title\": \"\",\n                \"skype\": \"\",\n                \"phone\": \"\",\n                \"avatar_hash\": \"gc11670b4452\",\n                \"real_name\": \"Hardini Thakkar\",\n                \"real_name_normalized\": \"Hardini Thakkar\",\n                \"email\": \"hardini@promactinfo.com\",\n                \"image_24\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/c11670b4452d7294f1a57566c642a168.jpg?s=24&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0024-24.png\",\n                \"image_32\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/c11670b4452d7294f1a57566c642a168.jpg?s=32&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0024-32.png\",\n                \"image_48\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/c11670b4452d7294f1a57566c642a168.jpg?s=48&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0024-48.png\",\n                \"image_72\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/c11670b4452d7294f1a57566c642a168.jpg?s=72&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0024-72.png\",\n                \"image_192\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/c11670b4452d7294f1a57566c642a168.jpg?s=192&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0024-192.png\",\n                \"image_512\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/c11670b4452d7294f1a57566c642a168.jpg?s=512&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0024-512.png\"\n            }\n        },\n        {\n            \"id\": \"U082PJG72\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"hetul\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"4d5e26\",\n            \"real_name\": \"Hetul Soni\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"first_name\": \"Hetul\",\n                \"last_name\": \"Soni\",\n                \"title\": \"iOS Developer\",\n                \"phone\": \"9033562753\",\n                \"skype\": \"\",\n                \"avatar_hash\": \"80ce0221d3a0\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56867565045_80ce0221d3a0e6ee2f3b_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56867565045_80ce0221d3a0e6ee2f3b_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56867565045_80ce0221d3a0e6ee2f3b_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56867565045_80ce0221d3a0e6ee2f3b_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56867565045_80ce0221d3a0e6ee2f3b_192.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56867565045_80ce0221d3a0e6ee2f3b_512.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56867565045_80ce0221d3a0e6ee2f3b_1024.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56867565045_80ce0221d3a0e6ee2f3b_original.jpg\",\n                \"real_name\": \"Hetul Soni\",\n                \"real_name_normalized\": \"Hetul Soni\",\n                \"email\": \"hetul@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U08KM481G\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"hitendra\",\n            \"deleted\": true,\n            \"profile\": {\n                \"avatar_hash\": \"gdb19b809e48\",\n                \"real_name\": \"\",\n                \"real_name_normalized\": \"\",\n                \"email\": \"hitendra@promactinfo.com\",\n                \"image_24\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/db19b809e485c84250d664233ef2b2a2.jpg?s=24&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0005-24.png\",\n                \"image_32\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/db19b809e485c84250d664233ef2b2a2.jpg?s=32&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0005-32.png\",\n                \"image_48\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/db19b809e485c84250d664233ef2b2a2.jpg?s=48&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0005-48.png\",\n                \"image_72\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/db19b809e485c84250d664233ef2b2a2.jpg?s=72&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0005-72.png\",\n                \"image_192\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/db19b809e485c84250d664233ef2b2a2.jpg?s=192&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0005-192.png\",\n                \"image_512\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/db19b809e485c84250d664233ef2b2a2.jpg?s=512&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0005-512.png\"\n            }\n        },\n        {\n            \"id\": \"U1TBJKNBE\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"iop\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"8f4a2b\",\n            \"real_name\": \"\",\n            \"tz\": null,\n            \"tz_label\": \"Pacific Daylight Time\",\n            \"tz_offset\": -25200,\n            \"profile\": {\n                \"bot_id\": \"B1TBJKN5S\",\n                \"api_app_id\": \"\",\n                \"avatar_hash\": \"7bc030894111\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61348525107_7bc030894111f5c17d29_24.png\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61348525107_7bc030894111f5c17d29_32.png\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61348525107_7bc030894111f5c17d29_48.png\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61348525107_7bc030894111f5c17d29_72.png\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61348525107_7bc030894111f5c17d29_192.png\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61348525107_7bc030894111f5c17d29_512.png\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61348525107_7bc030894111f5c17d29_512.png\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61348525107_7bc030894111f5c17d29_original.png\",\n                \"real_name\": \"\",\n                \"real_name_normalized\": \"\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": true\n        },\n        {\n            \"id\": \"U1WTH2LPM\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"jaymindongre\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"84b22f\",\n            \"real_name\": \"Jaymin Dongre\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"avatar_hash\": \"73febf48f28c\",\n                \"first_name\": \"Jaymin\",\n                \"last_name\": \"Dongre\",\n                \"title\": \"\",\n                \"phone\": \"9712867122\",\n                \"skype\": \"\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-09\\/67590706437_73febf48f28c66d5d0b2_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-09\\/67590706437_73febf48f28c66d5d0b2_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-09\\/67590706437_73febf48f28c66d5d0b2_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-09\\/67590706437_73febf48f28c66d5d0b2_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-09\\/67590706437_73febf48f28c66d5d0b2_192.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-09\\/67590706437_73febf48f28c66d5d0b2_512.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-09\\/67590706437_73febf48f28c66d5d0b2_1024.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-09\\/67590706437_73febf48f28c66d5d0b2_original.jpg\",\n                \"real_name\": \"Jaymin Dongre\",\n                \"real_name_normalized\": \"Jaymin Dongre\",\n                \"email\": \"jaymin@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U1MVBJE2X\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"jaynika_kachhela\",\n            \"deleted\": true,\n            \"profile\": {\n                \"avatar_hash\": \"4f8138bad33a\",\n                \"first_name\": \"Jaynika\",\n                \"last_name\": \"Kachhela\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56861983830_4f8138bad33a7e6fea46_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56861983830_4f8138bad33a7e6fea46_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56861983830_4f8138bad33a7e6fea46_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56861983830_4f8138bad33a7e6fea46_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56861983830_4f8138bad33a7e6fea46_192.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56861983830_4f8138bad33a7e6fea46_512.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56861983830_4f8138bad33a7e6fea46_512.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56861983830_4f8138bad33a7e6fea46_original.jpg\",\n                \"title\": \"Software Developer- trainee\",\n                \"phone\": \"8980905256\",\n                \"skype\": \"\",\n                \"real_name\": \"Jaynika Kachhela\",\n                \"real_name_normalized\": \"Jaynika Kachhela\",\n                \"email\": \"jaynika@promactinfo.com\"\n            }\n        },\n        {\n            \"id\": \"U085QD3QB\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"jimit\",\n            \"deleted\": true,\n            \"profile\": {\n                \"first_name\": \"Jimit\",\n                \"last_name\": \"Shah\",\n                \"avatar_hash\": \"g391c4c9f006\",\n                \"real_name\": \"Jimit Shah\",\n                \"real_name_normalized\": \"Jimit Shah\",\n                \"email\": \"jimit@outlook.com\",\n                \"image_24\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/391c4c9f00633425375d6e8da080017a.jpg?s=24&d=https%3A%2F%2Fa.slack-edge.com%2F0180%2Fimg%2Favatars%2Fava_0003-24.png\",\n                \"image_32\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/391c4c9f00633425375d6e8da080017a.jpg?s=32&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0003-32.png\",\n                \"image_48\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/391c4c9f00633425375d6e8da080017a.jpg?s=48&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0003-48.png\",\n                \"image_72\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/391c4c9f00633425375d6e8da080017a.jpg?s=72&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0003-72.png\",\n                \"image_192\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/391c4c9f00633425375d6e8da080017a.jpg?s=192&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0003-192.png\",\n                \"image_512\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/391c4c9f00633425375d6e8da080017a.jpg?s=512&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0003-512.png\"\n            }\n        },\n        {\n            \"id\": \"U06NVGLPQ\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"julie\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"7d414c\",\n            \"real_name\": \"Julie John\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"first_name\": \"Julie\",\n                \"last_name\": \"John\",\n                \"avatar_hash\": \"6b20676ab62b\",\n                \"title\": \"Software Developer\",\n                \"phone\": \"8401345388\",\n                \"skype\": \"\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57489634115_6b20676ab62bdbaed72f_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57489634115_6b20676ab62bdbaed72f_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57489634115_6b20676ab62bdbaed72f_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57489634115_6b20676ab62bdbaed72f_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57489634115_6b20676ab62bdbaed72f_192.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57489634115_6b20676ab62bdbaed72f_512.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57489634115_6b20676ab62bdbaed72f_512.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57489634115_6b20676ab62bdbaed72f_original.jpg\",\n                \"real_name\": \"Julie John\",\n                \"real_name_normalized\": \"Julie John\",\n                \"email\": \"julie@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U051JE2JW\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"kanan\",\n            \"deleted\": true,\n            \"profile\": {\n                \"first_name\": \"Kanan\",\n                \"last_name\": \"Shah\",\n                \"title\": \"Team Lead\",\n                \"skype\": \"kanan.promact\",\n                \"phone\": \"\",\n                \"avatar_hash\": \"g9b940af7d37\",\n                \"real_name\": \"Kanan Shah\",\n                \"real_name_normalized\": \"Kanan Shah\",\n                \"email\": \"kanan@promactinfo.com\",\n                \"image_24\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/9b940af7d37d9a2a5bb72e785870e282.jpg?s=24&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0013-24.png\",\n                \"image_32\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/9b940af7d37d9a2a5bb72e785870e282.jpg?s=32&d=https%3A%2F%2Fa.slack-edge.com%2F0180%2Fimg%2Favatars%2Fava_0013-32.png\",\n                \"image_48\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/9b940af7d37d9a2a5bb72e785870e282.jpg?s=48&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0013-48.png\",\n                \"image_72\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/9b940af7d37d9a2a5bb72e785870e282.jpg?s=72&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0013-72.png\",\n                \"image_192\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/9b940af7d37d9a2a5bb72e785870e282.jpg?s=192&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0013-192.png\",\n                \"image_512\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/9b940af7d37d9a2a5bb72e785870e282.jpg?s=512&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0013-512.png\"\n            }\n        },\n        {\n            \"id\": \"U0525LF29\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"karan\",\n            \"deleted\": true,\n            \"profile\": {\n                \"first_name\": \"Karan\",\n                \"last_name\": \"Kumar\",\n                \"title\": \"Brings Music To The Earth.\",\n                \"skype\": \"\",\n                \"phone\": \"\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2015-10-02\\/11764877569_b1b22cd7cb32f1fcac2e_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2015-10-02\\/11764877569_b1b22cd7cb32f1fcac2e_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2015-10-02\\/11764877569_b1b22cd7cb32f1fcac2e_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2015-10-02\\/11764877569_b1b22cd7cb32f1fcac2e_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2015-10-02\\/11764877569_b1b22cd7cb32f1fcac2e_192.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2015-10-02\\/11764877569_b1b22cd7cb32f1fcac2e_512.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2015-10-02\\/11764877569_b1b22cd7cb32f1fcac2e_1024.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2015-10-02\\/11764877569_b1b22cd7cb32f1fcac2e_original.jpg\",\n                \"avatar_hash\": \"b1b22cd7cb32\",\n                \"real_name\": \"Karan Kumar\",\n                \"real_name_normalized\": \"Karan Kumar\",\n                \"email\": \"karan@promactinfo.com\"\n            }\n        },\n        {\n            \"id\": \"U1N1CA4C8\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"karandesai\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"385a86\",\n            \"real_name\": \"Karan Desai\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"avatar_hash\": \"0109598dfc74\",\n                \"first_name\": \"Karan\",\n                \"last_name\": \"Desai\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56812239315_0109598dfc74bd790722_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56812239315_0109598dfc74bd790722_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56812239315_0109598dfc74bd790722_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56812239315_0109598dfc74bd790722_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56812239315_0109598dfc74bd790722_192.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56812239315_0109598dfc74bd790722_192.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56812239315_0109598dfc74bd790722_192.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56812239315_0109598dfc74bd790722_original.jpg\",\n                \"title\": \"Software Developer Trainee\",\n                \"phone\": \"9924815850\",\n                \"skype\": \"\",\n                \"real_name\": \"Karan Desai\",\n                \"real_name_normalized\": \"Karan Desai\",\n                \"email\": \"karan.desai@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U07B774M8\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"karan_\",\n            \"deleted\": true,\n            \"profile\": {\n                \"bot_id\": \"B07B6R332\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2015-07-08\\/7381226167_3f23c52c045a20139899_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2015-07-08\\/7381226167_3f23c52c045a20139899_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2015-07-08\\/7381226167_3f23c52c045a20139899_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2015-07-08\\/7381226167_3f23c52c045a20139899_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2015-07-08\\/7381226167_3f23c52c045a20139899_192.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2015-07-08\\/7381226167_3f23c52c045a20139899_original.jpg\",\n                \"avatar_hash\": \"3f23c52c045a\",\n                \"real_name\": \"\",\n                \"real_name_normalized\": \"\"\n            },\n            \"is_bot\": true\n        },\n        {\n            \"id\": \"U051J2RTW\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"khyati\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"43761b\",\n            \"real_name\": \"Khyati Shah\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"first_name\": \"Khyati\",\n                \"last_name\": \"Shah\",\n                \"title\": \"Software Developer\",\n                \"skype\": \"\",\n                \"phone\": \"\",\n                \"avatar_hash\": \"00ffc44195a5\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57945102770_00ffc44195a519ba6b3d_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57945102770_00ffc44195a519ba6b3d_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57945102770_00ffc44195a519ba6b3d_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57945102770_00ffc44195a519ba6b3d_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57945102770_00ffc44195a519ba6b3d_192.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57945102770_00ffc44195a519ba6b3d_512.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57945102770_00ffc44195a519ba6b3d_512.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57945102770_00ffc44195a519ba6b3d_original.jpg\",\n                \"real_name\": \"Khyati Shah\",\n                \"real_name_normalized\": \"Khyati Shah\",\n                \"email\": \"khyati@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U051HV8N8\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"kinjal\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"4cc091\",\n            \"real_name\": \"Kinjal Gor\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"first_name\": \"Kinjal\",\n                \"last_name\": \"Gor\",\n                \"avatar_hash\": \"2e984caf4195\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56880194978_2e984caf4195f72db81c_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56880194978_2e984caf4195f72db81c_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56880194978_2e984caf4195f72db81c_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56880194978_2e984caf4195f72db81c_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56880194978_2e984caf4195f72db81c_192.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56880194978_2e984caf4195f72db81c_192.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56880194978_2e984caf4195f72db81c_192.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56880194978_2e984caf4195f72db81c_original.jpg\",\n                \"title\": \"Web Developer\",\n                \"phone\": \"9428471889\",\n                \"skype\": \"kinjal.promact\",\n                \"real_name\": \"Kinjal Gor\",\n                \"real_name_normalized\": \"Kinjal Gor\",\n                \"email\": \"kinjal@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U1KG0DZFS\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"kip\",\n            \"deleted\": true,\n            \"profile\": {\n                \"bot_id\": \"B1KGAN1UH\",\n                \"api_app_id\": \"A0ELU5TDH\",\n                \"first_name\": \"Kip\",\n                \"avatar_hash\": \"065605ef2307\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-22\\/53554751585_065605ef2307820c5adb_24.png\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-22\\/53554751585_065605ef2307820c5adb_32.png\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-22\\/53554751585_065605ef2307820c5adb_48.png\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-22\\/53554751585_065605ef2307820c5adb_72.png\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-22\\/53554751585_065605ef2307820c5adb_192.png\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-22\\/53554751585_065605ef2307820c5adb_512.png\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-22\\/53554751585_065605ef2307820c5adb_512.png\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-22\\/53554751585_065605ef2307820c5adb_original.png\",\n                \"real_name\": \"Kip\",\n                \"real_name_normalized\": \"Kip\"\n            },\n            \"is_bot\": true\n        },\n        {\n            \"id\": \"U0VR8DA0Y\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"krunal\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"5b89d5\",\n            \"real_name\": \"Krunal Yadav\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"first_name\": \"Krunal\",\n                \"last_name\": \"Yadav\",\n                \"avatar_hash\": \"75592fee438c\",\n                \"title\": \"Dot Net Developer\",\n                \"phone\": \"9722216086\",\n                \"skype\": \"\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57521613428_75592fee438cef7b84d6_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57521613428_75592fee438cef7b84d6_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57521613428_75592fee438cef7b84d6_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57521613428_75592fee438cef7b84d6_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57521613428_75592fee438cef7b84d6_192.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57521613428_75592fee438cef7b84d6_512.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57521613428_75592fee438cef7b84d6_512.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57521613428_75592fee438cef7b84d6_original.jpg\",\n                \"real_name\": \"Krunal Yadav\",\n                \"real_name_normalized\": \"Krunal Yadav\",\n                \"email\": \"kunal@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U051J0GN4\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"krupa\",\n            \"deleted\": false,\n            \"status\": \"?\\/\\/\",\n            \"color\": \"db3150\",\n            \"real_name\": \"Krupa Patel\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57533416855_e460ce9484520ced9975_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57533416855_e460ce9484520ced9975_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57533416855_e460ce9484520ced9975_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57533416855_e460ce9484520ced9975_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57533416855_e460ce9484520ced9975_192.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57533416855_e460ce9484520ced9975_original.jpg\",\n                \"first_name\": \"Krupa\",\n                \"last_name\": \"Patel\",\n                \"avatar_hash\": \"e460ce948452\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57533416855_e460ce9484520ced9975_192.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57533416855_e460ce9484520ced9975_192.jpg\",\n                \"title\": \"Sr. Web Designer\",\n                \"phone\": \"\",\n                \"skype\": \"\",\n                \"real_name\": \"Krupa Patel\",\n                \"real_name_normalized\": \"Krupa Patel\",\n                \"email\": \"krupa@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U09UNVC20\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"krutikathakkar\",\n            \"deleted\": true,\n            \"profile\": {\n                \"first_name\": \"Krutika\",\n                \"last_name\": \"Thakkar\",\n                \"avatar_hash\": \"gcd96286efc8\",\n                \"real_name\": \"Krutika Thakkar\",\n                \"real_name_normalized\": \"Krutika Thakkar\",\n                \"email\": \"krutika@promactinfo.com\",\n                \"image_24\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/cd96286efc8ad1fc84d0c1dc6b579b60.jpg?s=24&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0013-24.png\",\n                \"image_32\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/cd96286efc8ad1fc84d0c1dc6b579b60.jpg?s=32&d=https%3A%2F%2Fa.slack-edge.com%2F0180%2Fimg%2Favatars%2Fava_0013-32.png\",\n                \"image_48\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/cd96286efc8ad1fc84d0c1dc6b579b60.jpg?s=48&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0013-48.png\",\n                \"image_72\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/cd96286efc8ad1fc84d0c1dc6b579b60.jpg?s=72&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0013-72.png\",\n                \"image_192\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/cd96286efc8ad1fc84d0c1dc6b579b60.jpg?s=192&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0013-192.png\",\n                \"image_512\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/cd96286efc8ad1fc84d0c1dc6b579b60.jpg?s=512&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0013-512.png\"\n            }\n        },\n        {\n            \"id\": \"U0525LH1B\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"kuntal\",\n            \"deleted\": true,\n            \"profile\": {\n                \"first_name\": \"Kuntal\",\n                \"last_name\": \"Mehta\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5073848023_dfcfee3d4e98633aa9f9_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5073848023_dfcfee3d4e98633aa9f9_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5073848023_dfcfee3d4e98633aa9f9_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5073848023_dfcfee3d4e98633aa9f9_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5073848023_dfcfee3d4e98633aa9f9_192.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5073848023_dfcfee3d4e98633aa9f9_original.jpg\",\n                \"title\": \"Software Developer\",\n                \"skype\": \"\",\n                \"phone\": \"9409642308\",\n                \"avatar_hash\": \"dfcfee3d4e98\",\n                \"real_name\": \"Kuntal Mehta\",\n                \"real_name_normalized\": \"Kuntal Mehta\",\n                \"email\": \"kuntal@promactinfo.com\"\n            }\n        },\n        {\n            \"id\": \"U1WA3HL4U\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"leavebot\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"8d4b84\",\n            \"real_name\": \"\",\n            \"tz\": null,\n            \"tz_label\": \"Pacific Daylight Time\",\n            \"tz_offset\": -25200,\n            \"profile\": {\n                \"bot_id\": \"B1WA6N0NL\",\n                \"api_app_id\": \"\",\n                \"avatar_hash\": \"7198bb25bef3\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-29\\/64343601060_7198bb25bef37e393dbb_24.png\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-29\\/64343601060_7198bb25bef37e393dbb_32.png\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-29\\/64343601060_7198bb25bef37e393dbb_48.png\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-29\\/64343601060_7198bb25bef37e393dbb_72.png\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-29\\/64343601060_7198bb25bef37e393dbb_192.png\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-29\\/64343601060_7198bb25bef37e393dbb_512.png\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-29\\/64343601060_7198bb25bef37e393dbb_512.png\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-29\\/64343601060_7198bb25bef37e393dbb_original.png\",\n                \"real_name\": \"\",\n                \"real_name_normalized\": \"\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": true\n        },\n        {\n            \"id\": \"U1W1P9P6W\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"leavesbot\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"3c8c69\",\n            \"real_name\": \"Leave Bot\",\n            \"tz\": null,\n            \"tz_label\": \"Pacific Daylight Time\",\n            \"tz_offset\": -25200,\n            \"profile\": {\n                \"bot_id\": \"B1W1DG1PZ\",\n                \"api_app_id\": \"\",\n                \"avatar_hash\": \"6c9ba0f7d171\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-28\\/64048021143_6c9ba0f7d1712a46290a_24.png\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-28\\/64048021143_6c9ba0f7d1712a46290a_32.png\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-28\\/64048021143_6c9ba0f7d1712a46290a_48.png\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-28\\/64048021143_6c9ba0f7d1712a46290a_48.png\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-28\\/64048021143_6c9ba0f7d1712a46290a_48.png\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-28\\/64048021143_6c9ba0f7d1712a46290a_48.png\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-28\\/64048021143_6c9ba0f7d1712a46290a_48.png\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-28\\/64048021143_6c9ba0f7d1712a46290a_original.png\",\n                \"first_name\": \"Leave\",\n                \"last_name\": \"Bot\",\n                \"title\": \"Leaves Details\",\n                \"real_name\": \"Leave Bot\",\n                \"real_name_normalized\": \"Leave Bot\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": true\n        },\n        {\n            \"id\": \"U1KG64NA1\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"leo_officevibe_bot\",\n            \"deleted\": true,\n            \"profile\": {\n                \"bot_id\": \"B1KG62NR5\",\n                \"api_app_id\": \"A0GU27WR1\",\n                \"first_name\": \"Leo (Officevibe Bot)\",\n                \"avatar_hash\": \"4cdbab3d3092\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-22\\/53550096503_4cdbab3d309291724383_24.png\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-22\\/53550096503_4cdbab3d309291724383_32.png\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-22\\/53550096503_4cdbab3d309291724383_48.png\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-22\\/53550096503_4cdbab3d309291724383_72.png\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-22\\/53550096503_4cdbab3d309291724383_192.png\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-22\\/53550096503_4cdbab3d309291724383_512.png\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-22\\/53550096503_4cdbab3d309291724383_512.png\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-22\\/53550096503_4cdbab3d309291724383_original.png\",\n                \"real_name\": \"Leo (Officevibe Bot)\",\n                \"real_name_normalized\": \"Leo (Officevibe Bot)\"\n            },\n            \"is_bot\": true\n        },\n        {\n            \"id\": \"U1TB1RQGP\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"lunchbreak\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"e06b56\",\n            \"real_name\": \"Lunch Break\",\n            \"tz\": null,\n            \"tz_label\": \"Pacific Daylight Time\",\n            \"tz_offset\": -25200,\n            \"profile\": {\n                \"bot_id\": \"B1TAVTJHE\",\n                \"api_app_id\": \"\",\n                \"avatar_hash\": \"b6f07324274e\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61386823554_b6f07324274ed2ae1754_24.png\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61386823554_b6f07324274ed2ae1754_32.png\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61386823554_b6f07324274ed2ae1754_48.png\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61386823554_b6f07324274ed2ae1754_48.png\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61386823554_b6f07324274ed2ae1754_48.png\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61386823554_b6f07324274ed2ae1754_48.png\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61386823554_b6f07324274ed2ae1754_48.png\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61386823554_b6f07324274ed2ae1754_original.png\",\n                \"first_name\": \"Lunch\",\n                \"last_name\": \"Break\",\n                \"title\": \"Calls everyone for lunch\",\n                \"real_name\": \"Lunch Break\",\n                \"real_name_normalized\": \"Lunch Break\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": true\n        },\n        {\n            \"id\": \"U0525LW55\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"maishidh\",\n            \"deleted\": true,\n            \"profile\": {\n                \"first_name\": \"\",\n                \"last_name\": \"\",\n                \"title\": \"\",\n                \"skype\": \"\",\n                \"phone\": \"9978440245\",\n                \"avatar_hash\": \"gd19f784cf2b\",\n                \"real_name\": \"\",\n                \"real_name_normalized\": \"\",\n                \"email\": \"maishidh@promactinfo.com\",\n                \"image_24\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/d19f784cf2b40fcfced04d87c1be216b.jpg?s=24&d=https%3A%2F%2Fa.slack-edge.com%2F0180%2Fimg%2Favatars%2Fava_0021-24.png\",\n                \"image_32\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/d19f784cf2b40fcfced04d87c1be216b.jpg?s=32&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0021-32.png\",\n                \"image_48\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/d19f784cf2b40fcfced04d87c1be216b.jpg?s=48&d=https%3A%2F%2Fa.slack-edge.com%2F3654%2Fimg%2Favatars%2Fava_0021-48.png\",\n                \"image_72\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/d19f784cf2b40fcfced04d87c1be216b.jpg?s=72&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0021-72.png\",\n                \"image_192\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/d19f784cf2b40fcfced04d87c1be216b.jpg?s=192&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0021-192.png\",\n                \"image_512\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/d19f784cf2b40fcfced04d87c1be216b.jpg?s=512&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0021-512.png\"\n            }\n        },\n        {\n            \"id\": \"U051HV9KY\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"malkesh\",\n            \"deleted\": true,\n            \"profile\": {\n                \"avatar_hash\": \"g14c792c3809\",\n                \"real_name\": \"\",\n                \"real_name_normalized\": \"\",\n                \"email\": \"malkesh@promactinfo.com\",\n                \"image_24\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/14c792c38098ec7cf0055458811652d0.jpg?s=24&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0026-24.png\",\n                \"image_32\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/14c792c38098ec7cf0055458811652d0.jpg?s=32&d=https%3A%2F%2Fa.slack-edge.com%2F0180%2Fimg%2Favatars%2Fava_0026-32.png\",\n                \"image_48\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/14c792c38098ec7cf0055458811652d0.jpg?s=48&d=https%3A%2F%2Fa.slack-edge.com%2F272a%2Fimg%2Favatars%2Fava_0026-48.png\",\n                \"image_72\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/14c792c38098ec7cf0055458811652d0.jpg?s=72&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0026-72.png\",\n                \"image_192\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/14c792c38098ec7cf0055458811652d0.jpg?s=192&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0026-192.png\",\n                \"image_512\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/14c792c38098ec7cf0055458811652d0.jpg?s=512&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0026-512.png\"\n            }\n        },\n        {\n            \"id\": \"U0J15RC5V\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"masteryoda\",\n            \"deleted\": true,\n            \"profile\": {\n                \"bot_id\": \"B0J15NBTL\",\n                \"api_app_id\": \"\",\n                \"avatar_hash\": \"24ba67ec119f\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-01-08\\/18040198162_24ba67ec119fb7632627_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-01-08\\/18040198162_24ba67ec119fb7632627_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-01-08\\/18040198162_24ba67ec119fb7632627_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-01-08\\/18040198162_24ba67ec119fb7632627_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-01-08\\/18040198162_24ba67ec119fb7632627_192.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-01-08\\/18040198162_24ba67ec119fb7632627_512.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-01-08\\/18040198162_24ba67ec119fb7632627_512.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-01-08\\/18040198162_24ba67ec119fb7632627_original.jpg\",\n                \"first_name\": \"Master\",\n                \"last_name\": \"Yoda\",\n                \"title\": \"Master Yoda will help you guide with daily task\",\n                \"real_name\": \"Master Yoda\",\n                \"real_name_normalized\": \"Master Yoda\"\n            },\n            \"is_bot\": true\n        },\n        {\n            \"id\": \"U1M9Z5BB9\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"mihir\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"53b759\",\n            \"real_name\": \"Mihir Mehta\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"first_name\": \"Mihir\",\n                \"last_name\": \"Mehta\",\n                \"avatar_hash\": \"g3f830305e4e\",\n                \"real_name\": \"Mihir Mehta\",\n                \"real_name_normalized\": \"Mihir Mehta\",\n                \"email\": \"mihir@wotu.in\",\n                \"image_24\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/3f830305e4ef50d5839d499a181c1d59.jpg?s=24&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0024-24.png\",\n                \"image_32\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/3f830305e4ef50d5839d499a181c1d59.jpg?s=32&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0024-32.png\",\n                \"image_48\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/3f830305e4ef50d5839d499a181c1d59.jpg?s=48&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0024-48.png\",\n                \"image_72\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/3f830305e4ef50d5839d499a181c1d59.jpg?s=72&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0024-72.png\",\n                \"image_192\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/3f830305e4ef50d5839d499a181c1d59.jpg?s=192&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0024-192.png\",\n                \"image_512\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/3f830305e4ef50d5839d499a181c1d59.jpg?s=512&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0024-512.png\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": true,\n            \"is_ultra_restricted\": true,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U0N3WUJR4\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"mohini\",\n            \"deleted\": true,\n            \"profile\": {\n                \"avatar_hash\": \"g3eb04885ebb\",\n                \"real_name\": \"\",\n                \"real_name_normalized\": \"\",\n                \"email\": \"mohini@promactinfo.com\",\n                \"image_24\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/3eb04885ebb3a66f8a3a9c7b2165fe19.jpg?s=24&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0015-24.png\",\n                \"image_32\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/3eb04885ebb3a66f8a3a9c7b2165fe19.jpg?s=32&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0015-32.png\",\n                \"image_48\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/3eb04885ebb3a66f8a3a9c7b2165fe19.jpg?s=48&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0015-48.png\",\n                \"image_72\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/3eb04885ebb3a66f8a3a9c7b2165fe19.jpg?s=72&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0015-72.png\",\n                \"image_192\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/3eb04885ebb3a66f8a3a9c7b2165fe19.jpg?s=192&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0015-192.png\",\n                \"image_512\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/3eb04885ebb3a66f8a3a9c7b2165fe19.jpg?s=512&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0015-512.png\"\n            }\n        },\n        {\n            \"id\": \"U0525N6NB\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"nehal\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"ea2977\",\n            \"real_name\": \"Nehal Vegda\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"avatar_hash\": \"1143e6fd1035\",\n                \"first_name\": \"Nehal\",\n                \"last_name\": \"Vegda\",\n                \"title\": \"PHP Developer\",\n                \"phone\": \"7041194520\",\n                \"skype\": \"\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57908456326_1143e6fd1035a70fa4b3_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57908456326_1143e6fd1035a70fa4b3_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57908456326_1143e6fd1035a70fa4b3_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57908456326_1143e6fd1035a70fa4b3_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57908456326_1143e6fd1035a70fa4b3_192.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57908456326_1143e6fd1035a70fa4b3_512.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57908456326_1143e6fd1035a70fa4b3_1024.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57908456326_1143e6fd1035a70fa4b3_original.jpg\",\n                \"real_name\": \"Nehal Vegda\",\n                \"real_name_normalized\": \"Nehal Vegda\",\n                \"email\": \"nehal@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U0ZH5UYSJ\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"nidhi\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"99a949\",\n            \"real_name\": \"Nidhi Gandhi\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"first_name\": \"Nidhi\",\n                \"last_name\": \"Gandhi\",\n                \"avatar_hash\": \"c7d78495ca88\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57648108306_c7d78495ca88a5f30c69_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57648108306_c7d78495ca88a5f30c69_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57648108306_c7d78495ca88a5f30c69_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57648108306_c7d78495ca88a5f30c69_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57648108306_c7d78495ca88a5f30c69_192.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57648108306_c7d78495ca88a5f30c69_512.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57648108306_c7d78495ca88a5f30c69_512.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57648108306_c7d78495ca88a5f30c69_original.jpg\",\n                \"title\": \"working as a sr.php developer\",\n                \"phone\": \"9925860684\",\n                \"skype\": \"\",\n                \"real_name\": \"Nidhi Gandhi\",\n                \"real_name_normalized\": \"Nidhi Gandhi\",\n                \"email\": \"nidhi@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U1T9DR491\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"nikunj\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"d55aef\",\n            \"real_name\": \"\",\n            \"tz\": null,\n            \"tz_label\": \"Pacific Daylight Time\",\n            \"tz_offset\": -25200,\n            \"profile\": {\n                \"bot_id\": \"B1TAQ0VLL\",\n                \"api_app_id\": \"\",\n                \"avatar_hash\": \"985c14b1c4ee\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61364030596_985c14b1c4ee6255a2ce_24.png\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61364030596_985c14b1c4ee6255a2ce_32.png\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61364030596_985c14b1c4ee6255a2ce_48.png\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61364030596_985c14b1c4ee6255a2ce_72.png\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61364030596_985c14b1c4ee6255a2ce_192.png\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61364030596_985c14b1c4ee6255a2ce_512.png\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61364030596_985c14b1c4ee6255a2ce_512.png\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61364030596_985c14b1c4ee6255a2ce_original.png\",\n                \"real_name\": \"\",\n                \"real_name_normalized\": \"\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": true\n        },\n        {\n            \"id\": \"U0525PKAK\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"nimesh\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"e06b56\",\n            \"real_name\": \"Nimesh Bhatt\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"avatar_hash\": \"b80a616b042d\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56888149522_b80a616b042d959ff457_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56888149522_b80a616b042d959ff457_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56888149522_b80a616b042d959ff457_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56888149522_b80a616b042d959ff457_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56888149522_b80a616b042d959ff457_192.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56888149522_b80a616b042d959ff457_192.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56888149522_b80a616b042d959ff457_192.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56888149522_b80a616b042d959ff457_original.jpg\",\n                \"first_name\": \"Nimesh\",\n                \"last_name\": \"Bhatt\",\n                \"title\": \"QA - Software Tester\",\n                \"phone\": \"\",\n                \"skype\": \"nimesh.promact\",\n                \"real_name\": \"Nimesh Bhatt\",\n                \"real_name_normalized\": \"Nimesh Bhatt\",\n                \"email\": \"nimesh@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U051J44DE\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"nipun\",\n            \"deleted\": true,\n            \"profile\": {\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/58000303254_57ce16fada280144a00d_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/58000303254_57ce16fada280144a00d_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/58000303254_57ce16fada280144a00d_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/58000303254_57ce16fada280144a00d_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/58000303254_57ce16fada280144a00d_192.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/58000303254_57ce16fada280144a00d_original.jpg\",\n                \"first_name\": \"Nipun\",\n                \"last_name\": \"Desai\",\n                \"avatar_hash\": \"57ce16fada28\",\n                \"title\": \"Software Developer\",\n                \"phone\": \"7600358959\",\n                \"skype\": \"\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/58000303254_57ce16fada280144a00d_512.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/58000303254_57ce16fada280144a00d_1024.jpg\",\n                \"real_name\": \"Nipun Desai\",\n                \"real_name_normalized\": \"Nipun Desai\",\n                \"email\": \"nipun@promactinfo.com\"\n            }\n        },\n        {\n            \"id\": \"U0FAVB57W\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"nupur\",\n            \"deleted\": true,\n            \"profile\": {\n                \"first_name\": \"Nupur\",\n                \"last_name\": \"Shah\",\n                \"avatar_hash\": \"g8a0dd3270e9\",\n                \"real_name\": \"Nupur Shah\",\n                \"real_name_normalized\": \"Nupur Shah\",\n                \"email\": \"nupur91shah@gmail.com\",\n                \"image_24\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/8a0dd3270e9d590fbfd72036f1bfdf9f.jpg?s=24&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0026-24.png\",\n                \"image_32\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/8a0dd3270e9d590fbfd72036f1bfdf9f.jpg?s=32&d=https%3A%2F%2Fa.slack-edge.com%2F0180%2Fimg%2Favatars%2Fava_0026-32.png\",\n                \"image_48\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/8a0dd3270e9d590fbfd72036f1bfdf9f.jpg?s=48&d=https%3A%2F%2Fa.slack-edge.com%2F272a%2Fimg%2Favatars%2Fava_0026-48.png\",\n                \"image_72\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/8a0dd3270e9d590fbfd72036f1bfdf9f.jpg?s=72&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0026-72.png\",\n                \"image_192\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/8a0dd3270e9d590fbfd72036f1bfdf9f.jpg?s=192&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0026-192.png\",\n                \"image_512\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/8a0dd3270e9d590fbfd72036f1bfdf9f.jpg?s=512&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0026-512.png\"\n            }\n        },\n        {\n            \"id\": \"U051J0M2A\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"panktishah\",\n            \"deleted\": true,\n            \"profile\": {\n                \"avatar_hash\": \"gf8a5fdb5a2b\",\n                \"real_name\": \"\",\n                \"real_name_normalized\": \"\",\n                \"email\": \"pankti@promactinfo.com\",\n                \"image_24\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/f8a5fdb5a2b6407e9464d1646f6dcb34.jpg?s=24&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0018-24.png\",\n                \"image_32\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/f8a5fdb5a2b6407e9464d1646f6dcb34.jpg?s=32&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0018-32.png\",\n                \"image_48\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/f8a5fdb5a2b6407e9464d1646f6dcb34.jpg?s=48&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0018-48.png\",\n                \"image_72\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/f8a5fdb5a2b6407e9464d1646f6dcb34.jpg?s=72&d=https%3A%2F%2Fa.slack-edge.com%2F3654%2Fimg%2Favatars%2Fava_0018-72.png\",\n                \"image_192\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/f8a5fdb5a2b6407e9464d1646f6dcb34.jpg?s=192&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0018-192.png\",\n                \"image_512\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/f8a5fdb5a2b6407e9464d1646f6dcb34.jpg?s=512&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0018-512.png\"\n            }\n        },\n        {\n            \"id\": \"U0FQH9DB9\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"parth\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"8469bc\",\n            \"real_name\": \"Parth joshi\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"first_name\": \"Parth\",\n                \"last_name\": \"joshi\",\n                \"avatar_hash\": \"7566c7909870\",\n                \"title\": \"Jr .Net Developer\",\n                \"skype\": \"parth.joshi101@gmail.com\",\n                \"phone\": \"9979859440\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-11\\/58597611073_7566c7909870f8d1949c_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-11\\/58597611073_7566c7909870f8d1949c_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-11\\/58597611073_7566c7909870f8d1949c_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-11\\/58597611073_7566c7909870f8d1949c_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-11\\/58597611073_7566c7909870f8d1949c_192.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-11\\/58597611073_7566c7909870f8d1949c_512.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-11\\/58597611073_7566c7909870f8d1949c_512.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-11\\/58597611073_7566c7909870f8d1949c_original.jpg\",\n                \"real_name\": \"Parth joshi\",\n                \"real_name_normalized\": \"Parth joshi\",\n                \"email\": \"parth@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U1U4H0K4G\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"pinne\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"a2a5dc\",\n            \"real_name\": \"\",\n            \"tz\": null,\n            \"tz_label\": \"Pacific Daylight Time\",\n            \"tz_offset\": -25200,\n            \"profile\": {\n                \"bot_id\": \"B1U4NTFNH\",\n                \"api_app_id\": \"\",\n                \"avatar_hash\": \"255aad7ce525\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-22\\/62104388339_255aad7ce525bfa82295_24.png\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-22\\/62104388339_255aad7ce525bfa82295_32.png\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-22\\/62104388339_255aad7ce525bfa82295_48.png\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-22\\/62104388339_255aad7ce525bfa82295_72.png\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-22\\/62104388339_255aad7ce525bfa82295_192.png\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-22\\/62104388339_255aad7ce525bfa82295_512.png\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-22\\/62104388339_255aad7ce525bfa82295_512.png\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-22\\/62104388339_255aad7ce525bfa82295_original.png\",\n                \"real_name\": \"\",\n                \"real_name_normalized\": \"\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": true\n        },\n        {\n            \"id\": \"U0526JFLM\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"piyush\",\n            \"deleted\": true,\n            \"profile\": {\n                \"first_name\": \"Piyush\",\n                \"last_name\": \"Siddhapura\",\n                \"title\": \"Sr. Software Developer\",\n                \"skype\": \"\",\n                \"phone\": \"\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5075260799_1dc4c91a65351fe930b6_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5075260799_1dc4c91a65351fe930b6_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5075260799_1dc4c91a65351fe930b6_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5075260799_1dc4c91a65351fe930b6_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5075260799_1dc4c91a65351fe930b6_192.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5075260799_1dc4c91a65351fe930b6_original.jpg\",\n                \"avatar_hash\": \"1dc4c91a6535\",\n                \"real_name\": \"Piyush Siddhapura\",\n                \"real_name_normalized\": \"Piyush Siddhapura\",\n                \"email\": \"piyush@promactinfo.com\"\n            }\n        },\n        {\n            \"id\": \"U0525LLBZ\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"pooja\",\n            \"deleted\": true,\n            \"profile\": {\n                \"first_name\": \"Pooja\",\n                \"last_name\": \"Shah\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5073818963_399443a951dc49e7f076_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5073818963_399443a951dc49e7f076_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5073818963_399443a951dc49e7f076_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5073818963_399443a951dc49e7f076_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5073818963_399443a951dc49e7f076_72.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5073818963_399443a951dc49e7f076_original.jpg\",\n                \"title\": \"Software Developer\",\n                \"skype\": \"\",\n                \"phone\": \"\",\n                \"avatar_hash\": \"399443a951dc\",\n                \"real_name\": \"Pooja Shah\",\n                \"real_name_normalized\": \"Pooja Shah\",\n                \"email\": \"pooja@promactinfo.com\"\n            }\n        },\n        {\n            \"id\": \"U0U36HKV1\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"pranali\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"684b6c\",\n            \"real_name\": \"Pranali Jadhav\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"first_name\": \"Pranali\",\n                \"last_name\": \"Jadhav\",\n                \"avatar_hash\": \"97a4df7c2894\",\n                \"title\": \"Jr. php developer\",\n                \"phone\": \"\",\n                \"skype\": \"\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57525899142_97a4df7c289471065106_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57525899142_97a4df7c289471065106_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57525899142_97a4df7c289471065106_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57525899142_97a4df7c289471065106_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57525899142_97a4df7c289471065106_192.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57525899142_97a4df7c289471065106_512.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57525899142_97a4df7c289471065106_1024.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57525899142_97a4df7c289471065106_original.jpg\",\n                \"real_name\": \"Pranali Jadhav\",\n                \"real_name_normalized\": \"Pranali Jadhav\",\n                \"email\": \"pranali@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U08GCPE0J\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"pratikthakkar\",\n            \"deleted\": true,\n            \"profile\": {\n                \"first_name\": \"Pratik\",\n                \"last_name\": \"Thakkar\",\n                \"avatar_hash\": \"ge718a5a6573\",\n                \"real_name\": \"Pratik Thakkar\",\n                \"real_name_normalized\": \"Pratik Thakkar\",\n                \"email\": \"pratik@promactinfo.com\",\n                \"image_24\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/e718a5a6573dd89ab31fff3742c96a0b.jpg?s=24&d=https%3A%2F%2Fa.slack-edge.com%2F0180%2Fimg%2Favatars%2Fava_0004-24.png\",\n                \"image_32\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/e718a5a6573dd89ab31fff3742c96a0b.jpg?s=32&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0004-32.png\",\n                \"image_48\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/e718a5a6573dd89ab31fff3742c96a0b.jpg?s=48&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0004-48.png\",\n                \"image_72\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/e718a5a6573dd89ab31fff3742c96a0b.jpg?s=72&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0004-72.png\",\n                \"image_192\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/e718a5a6573dd89ab31fff3742c96a0b.jpg?s=192&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0004-192.png\",\n                \"image_512\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/e718a5a6573dd89ab31fff3742c96a0b.jpg?s=512&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0004-512.png\"\n            }\n        },\n        {\n            \"id\": \"U1C74JGTU\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"prince\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"4cc091\",\n            \"real_name\": \"prince thakkar\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"avatar_hash\": \"8360907925fa\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57481108035_8360907925fa7acbe33b_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57481108035_8360907925fa7acbe33b_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57481108035_8360907925fa7acbe33b_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57481108035_8360907925fa7acbe33b_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57481108035_8360907925fa7acbe33b_192.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57481108035_8360907925fa7acbe33b_512.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57481108035_8360907925fa7acbe33b_512.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57481108035_8360907925fa7acbe33b_original.jpg\",\n                \"first_name\": \"prince\",\n                \"last_name\": \"thakkar\",\n                \"title\": \"trainee software developer\",\n                \"phone\": \"9601057203\",\n                \"skype\": \"\",\n                \"real_name\": \"prince thakkar\",\n                \"real_name_normalized\": \"prince thakkar\",\n                \"email\": \"prince@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U1267JY10\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"priteshsolanki\",\n            \"deleted\": true,\n            \"profile\": {\n                \"first_name\": \"Pritesh\",\n                \"last_name\": \"Solanki\",\n                \"avatar_hash\": \"g8c7fd5eb764\",\n                \"real_name\": \"Pritesh Solanki\",\n                \"real_name_normalized\": \"Pritesh Solanki\",\n                \"email\": \"pritesh@promactinfo.com\",\n                \"image_24\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/8c7fd5eb76407184b8b9a6437b3a9367.jpg?s=24&d=https%3A%2F%2Fa.slack-edge.com%2F0180%2Fimg%2Favatars%2Fava_0003-24.png\",\n                \"image_32\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/8c7fd5eb76407184b8b9a6437b3a9367.jpg?s=32&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0003-32.png\",\n                \"image_48\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/8c7fd5eb76407184b8b9a6437b3a9367.jpg?s=48&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0003-48.png\",\n                \"image_72\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/8c7fd5eb76407184b8b9a6437b3a9367.jpg?s=72&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0003-72.png\",\n                \"image_192\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/8c7fd5eb76407184b8b9a6437b3a9367.jpg?s=192&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0003-192.png\",\n                \"image_512\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/8c7fd5eb76407184b8b9a6437b3a9367.jpg?s=512&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0003-512.png\"\n            }\n        },\n        {\n            \"id\": \"U1N1P8X89\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"priyaa\",\n            \"deleted\": true,\n            \"profile\": {\n                \"avatar_hash\": \"g894b1ec8a52\",\n                \"first_name\": \"Priya\",\n                \"last_name\": \"Pancholi\",\n                \"title\": \"software developer_trainee\",\n                \"phone\": \"\",\n                \"skype\": \"\",\n                \"real_name\": \"Priya Pancholi\",\n                \"real_name_normalized\": \"Priya Pancholi\",\n                \"email\": \"priya.pancholi@promactinfo.com\",\n                \"image_24\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/894b1ec8a5230e718cdaaff55d4d2a94.jpg?s=24&d=https%3A%2F%2Fa.slack-edge.com%2F0180%2Fimg%2Favatars%2Fava_0017-24.png\",\n                \"image_32\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/894b1ec8a5230e718cdaaff55d4d2a94.jpg?s=32&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0017-32.png\",\n                \"image_48\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/894b1ec8a5230e718cdaaff55d4d2a94.jpg?s=48&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0017-48.png\",\n                \"image_72\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/894b1ec8a5230e718cdaaff55d4d2a94.jpg?s=72&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0017-72.png\",\n                \"image_192\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/894b1ec8a5230e718cdaaff55d4d2a94.jpg?s=192&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0017-192.png\",\n                \"image_512\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/894b1ec8a5230e718cdaaff55d4d2a94.jpg?s=512&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0017-512.png\"\n            }\n        },\n        {\n            \"id\": \"U0525LFLT\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"priyanka\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"684b6c\",\n            \"real_name\": \"Priyanka Soni\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"first_name\": \"Priyanka\",\n                \"last_name\": \"Soni\",\n                \"avatar_hash\": \"f977c5712d24\",\n                \"title\": \"Sr.\\u00a0Software\\u00a0Developer\",\n                \"phone\": \"\",\n                \"skype\": \"priyanka.promact\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57900010595_f977c5712d244efc0045_24.png\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57900010595_f977c5712d244efc0045_32.png\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57900010595_f977c5712d244efc0045_48.png\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57900010595_f977c5712d244efc0045_72.png\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57900010595_f977c5712d244efc0045_192.png\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57900010595_f977c5712d244efc0045_512.png\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57900010595_f977c5712d244efc0045_512.png\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57900010595_f977c5712d244efc0045_original.png\",\n                \"real_name\": \"Priyanka Soni\",\n                \"real_name_normalized\": \"Priyanka Soni\",\n                \"email\": \"priyanka@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U0525N6FF\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"priyarvyas\",\n            \"deleted\": true,\n            \"profile\": {\n                \"first_name\": \"priya\",\n                \"last_name\": \"vyas\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2015-10-30\\/13558168791_006a31ccdb650c7c8889_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2015-10-30\\/13558168791_006a31ccdb650c7c8889_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2015-10-30\\/13558168791_006a31ccdb650c7c8889_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2015-10-30\\/13558168791_006a31ccdb650c7c8889_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2015-10-30\\/13558168791_006a31ccdb650c7c8889_192.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2015-10-30\\/13558168791_006a31ccdb650c7c8889_192.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2015-10-30\\/13558168791_006a31ccdb650c7c8889_192.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2015-10-30\\/13558168791_006a31ccdb650c7c8889_original.jpg\",\n                \"avatar_hash\": \"006a31ccdb65\",\n                \"real_name\": \"priya vyas\",\n                \"real_name_normalized\": \"priya vyas\",\n                \"email\": \"priya@promactinfo.com\"\n            }\n        },\n        {\n            \"id\": \"U23UZFLN7\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"promacttaskmail\",\n            \"deleted\": true,\n            \"profile\": {\n                \"bot_id\": \"B240WTXL4\",\n                \"api_app_id\": \"\",\n                \"avatar_hash\": \"bdb24d792533\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-23\\/72033762896_bdb24d792533e5c11746_24.png\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-23\\/72033762896_bdb24d792533e5c11746_32.png\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-23\\/72033762896_bdb24d792533e5c11746_48.png\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-23\\/72033762896_bdb24d792533e5c11746_72.png\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-23\\/72033762896_bdb24d792533e5c11746_192.png\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-23\\/72033762896_bdb24d792533e5c11746_512.png\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-23\\/72033762896_bdb24d792533e5c11746_512.png\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-23\\/72033762896_bdb24d792533e5c11746_original.png\",\n                \"real_name\": \"\",\n                \"real_name_normalized\": \"\"\n            },\n            \"is_bot\": true\n        },\n        {\n            \"id\": \"U0HJ33E49\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"rahul\",\n            \"deleted\": true,\n            \"profile\": {\n                \"avatar_hash\": \"cd9452cdf8b7\",\n                \"first_name\": \"\",\n                \"last_name\": \"\",\n                \"title\": \"\",\n                \"phone\": \"\",\n                \"skype\": \"\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-01-11\\/18179010069_cd9452cdf8b75b8f8855_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-01-11\\/18179010069_cd9452cdf8b75b8f8855_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-01-11\\/18179010069_cd9452cdf8b75b8f8855_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-01-11\\/18179010069_cd9452cdf8b75b8f8855_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-01-11\\/18179010069_cd9452cdf8b75b8f8855_192.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-01-11\\/18179010069_cd9452cdf8b75b8f8855_512.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-01-11\\/18179010069_cd9452cdf8b75b8f8855_512.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-01-11\\/18179010069_cd9452cdf8b75b8f8855_original.jpg\",\n                \"real_name\": \"\",\n                \"real_name_normalized\": \"\",\n                \"email\": \"rahul@promactinfo.com\"\n            }\n        },\n        {\n            \"id\": \"U0HJ48P4Y\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"rajdeep\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"b14cbc\",\n            \"real_name\": \"Rajdeep Sen Gupta\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"avatar_hash\": \"615acadbe845\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-01-06\\/17852203716_615acadbe84561d53240_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-01-06\\/17852203716_615acadbe84561d53240_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-01-06\\/17852203716_615acadbe84561d53240_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-01-06\\/17852203716_615acadbe84561d53240_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-01-06\\/17852203716_615acadbe84561d53240_192.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-01-06\\/17852203716_615acadbe84561d53240_512.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-01-06\\/17852203716_615acadbe84561d53240_512.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-01-06\\/17852203716_615acadbe84561d53240_original.jpg\",\n                \"first_name\": \"Rajdeep\",\n                \"last_name\": \"Sen Gupta\",\n                \"title\": \"Software developer\",\n                \"skype\": \"\",\n                \"phone\": \"\",\n                \"real_name\": \"Rajdeep Sen Gupta\",\n                \"real_name_normalized\": \"Rajdeep Sen Gupta\",\n                \"email\": \"rajdeep@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U1TARKT5L\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"right\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"50a0cf\",\n            \"real_name\": \"\",\n            \"tz\": null,\n            \"tz_label\": \"Pacific Daylight Time\",\n            \"tz_offset\": -25200,\n            \"profile\": {\n                \"bot_id\": \"B1TB61RND\",\n                \"api_app_id\": \"\",\n                \"avatar_hash\": \"b8721312a6b7\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61380060833_b8721312a6b7171c7d6d_24.png\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61380060833_b8721312a6b7171c7d6d_32.png\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61380060833_b8721312a6b7171c7d6d_48.png\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61380060833_b8721312a6b7171c7d6d_72.png\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61380060833_b8721312a6b7171c7d6d_192.png\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61380060833_b8721312a6b7171c7d6d_512.png\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61380060833_b8721312a6b7171c7d6d_512.png\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61380060833_b8721312a6b7171c7d6d_original.png\",\n                \"real_name\": \"\",\n                \"real_name_normalized\": \"\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": true\n        },\n        {\n            \"id\": \"U0525MNTH\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"rinkesh\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"53b759\",\n            \"real_name\": \"Rinkesh Parekh\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5073773299_27c0691212a838db9074_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5073773299_27c0691212a838db9074_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5073773299_27c0691212a838db9074_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5073773299_27c0691212a838db9074_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5073773299_27c0691212a838db9074_192.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5073773299_27c0691212a838db9074_original.jpg\",\n                \"first_name\": \"Rinkesh\",\n                \"last_name\": \"Parekh\",\n                \"avatar_hash\": \"27c0691212a8\",\n                \"title\": \"Business & Operation Management\",\n                \"phone\": \"91 9979928386\",\n                \"skype\": \"rinkesh.promact\",\n                \"real_name\": \"Rinkesh Parekh\",\n                \"real_name_normalized\": \"Rinkesh Parekh\",\n                \"email\": \"rinkesh@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U051J094A\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"ripal\",\n            \"deleted\": true,\n            \"profile\": {\n                \"avatar_hash\": \"g748c6e6b48f\",\n                \"real_name\": \"\",\n                \"real_name_normalized\": \"\",\n                \"email\": \"ripal@promactinfo.com\",\n                \"image_24\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/748c6e6b48fff6dc56d11f88139fdb0b.jpg?s=24&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0020-24.png\",\n                \"image_32\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/748c6e6b48fff6dc56d11f88139fdb0b.jpg?s=32&d=https%3A%2F%2Fa.slack-edge.com%2F0180%2Fimg%2Favatars%2Fava_0020-32.png\",\n                \"image_48\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/748c6e6b48fff6dc56d11f88139fdb0b.jpg?s=48&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0020-48.png\",\n                \"image_72\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/748c6e6b48fff6dc56d11f88139fdb0b.jpg?s=72&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0020-72.png\",\n                \"image_192\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/748c6e6b48fff6dc56d11f88139fdb0b.jpg?s=192&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0020-192.png\",\n                \"image_512\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/748c6e6b48fff6dc56d11f88139fdb0b.jpg?s=512&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0020-512.png\"\n            }\n        },\n        {\n            \"id\": \"U051HV8JE\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"riya\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"df3dc0\",\n            \"real_name\": \"Riya Shah\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57547153441_6113d6cb51e0d8de6393_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57547153441_6113d6cb51e0d8de6393_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57547153441_6113d6cb51e0d8de6393_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57547153441_6113d6cb51e0d8de6393_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57547153441_6113d6cb51e0d8de6393_192.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57547153441_6113d6cb51e0d8de6393_original.jpg\",\n                \"first_name\": \"Riya\",\n                \"last_name\": \"Shah\",\n                \"title\": \"Team Lead\",\n                \"avatar_hash\": \"6113d6cb51e0\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57547153441_6113d6cb51e0d8de6393_512.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57547153441_6113d6cb51e0d8de6393_1024.jpg\",\n                \"phone\": \"\",\n                \"skype\": \"\",\n                \"real_name\": \"Riya Shah\",\n                \"real_name_normalized\": \"Riya Shah\",\n                \"email\": \"riya@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U06CU2VB9\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"ronak\",\n            \"deleted\": true,\n            \"profile\": {\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2015-06-26\\/6887884871_cb032b6682fe577c2278_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2015-06-26\\/6887884871_cb032b6682fe577c2278_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2015-06-26\\/6887884871_cb032b6682fe577c2278_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2015-06-26\\/6887884871_cb032b6682fe577c2278_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2015-06-26\\/6887884871_cb032b6682fe577c2278_192.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2015-06-26\\/6887884871_cb032b6682fe577c2278_original.jpg\",\n                \"first_name\": \"Ronak\",\n                \"last_name\": \"Prajapati\",\n                \"title\": \"\",\n                \"skype\": \"\",\n                \"phone\": \"\",\n                \"avatar_hash\": \"cb032b6682fe\",\n                \"real_name\": \"Ronak Prajapati\",\n                \"real_name_normalized\": \"Ronak Prajapati\",\n                \"email\": \"ronak@promactinfo.com\"\n            }\n        },\n        {\n            \"id\": \"U1M8GSPPC\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"ronak.shah\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"235e5b\",\n            \"real_name\": \"Ronak Shah\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"avatar_hash\": \"596d585a01d1\",\n                \"first_name\": \"Ronak\",\n                \"last_name\": \"Shah\",\n                \"title\": \"Software Developer\",\n                \"phone\": \"9428585453\",\n                \"skype\": \"\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-06\\/57149228324_596d585a01d105471730_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-06\\/57149228324_596d585a01d105471730_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-06\\/57149228324_596d585a01d105471730_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-06\\/57149228324_596d585a01d105471730_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-06\\/57149228324_596d585a01d105471730_192.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-06\\/57149228324_596d585a01d105471730_512.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-06\\/57149228324_596d585a01d105471730_1024.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-06\\/57149228324_596d585a01d105471730_original.jpg\",\n                \"real_name\": \"Ronak Shah\",\n                \"real_name_normalized\": \"Ronak Shah\",\n                \"email\": \"ronak.shah@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U23EZH2TW\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"ronak.sharma\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"e23f99\",\n            \"real_name\": \"Ronak Sharma\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"avatar_hash\": \"c52f994d7860\",\n                \"first_name\": \"Ronak\",\n                \"last_name\": \"Sharma\",\n                \"title\": \"Software Developer\",\n                \"phone\": \"9601040960\",\n                \"skype\": \"\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-24\\/72384620961_c52f994d786044717a86_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-24\\/72384620961_c52f994d786044717a86_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-24\\/72384620961_c52f994d786044717a86_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-24\\/72384620961_c52f994d786044717a86_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-24\\/72384620961_c52f994d786044717a86_192.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-24\\/72384620961_c52f994d786044717a86_512.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-24\\/72384620961_c52f994d786044717a86_1024.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-24\\/72384620961_c52f994d786044717a86_original.jpg\",\n                \"real_name\": \"Ronak Sharma\",\n                \"real_name_normalized\": \"Ronak Sharma\",\n                \"email\": \"ronak.sharma@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U0525LCJR\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"roshni\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"674b1b\",\n            \"real_name\": \"Roshni Shah\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"first_name\": \"Roshni\",\n                \"last_name\": \"Shah\",\n                \"title\": \"Team Lead\",\n                \"skype\": \"roshni.promact\",\n                \"phone\": \"09586700841\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5052405908_a9185f85ab388c5827f6_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5052405908_a9185f85ab388c5827f6_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5052405908_a9185f85ab388c5827f6_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5052405908_a9185f85ab388c5827f6_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5052405908_a9185f85ab388c5827f6_192.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2015-05-26\\/5052405908_a9185f85ab388c5827f6_original.jpg\",\n                \"avatar_hash\": \"a9185f85ab38\",\n                \"real_name\": \"Roshni Shah\",\n                \"real_name_normalized\": \"Roshni Shah\",\n                \"email\": \"roshni@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U0525M2P1\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"rushi\",\n            \"deleted\": false,\n            \"status\": \"?\",\n            \"color\": \"5a4592\",\n            \"real_name\": \"Rushi Soni\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"first_name\": \"Rushi\",\n                \"last_name\": \"Soni\",\n                \"title\": \"Software Developer\",\n                \"skype\": \"rushisoni.promact\",\n                \"phone\": \"\",\n                \"avatar_hash\": \"c3ee5d56dbfa\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57981161330_c3ee5d56dbfaea8cf793_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57981161330_c3ee5d56dbfaea8cf793_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57981161330_c3ee5d56dbfaea8cf793_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57981161330_c3ee5d56dbfaea8cf793_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57981161330_c3ee5d56dbfaea8cf793_192.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57981161330_c3ee5d56dbfaea8cf793_192.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57981161330_c3ee5d56dbfaea8cf793_192.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-08\\/57981161330_c3ee5d56dbfaea8cf793_original.jpg\",\n                \"real_name\": \"Rushi Soni\",\n                \"real_name_normalized\": \"Rushi Soni\",\n                \"email\": \"rushi@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U1D6AU01F\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"sakshi\",\n            \"deleted\": true,\n            \"profile\": {\n                \"avatar_hash\": \"g7498d009258\",\n                \"real_name\": \"\",\n                \"real_name_normalized\": \"\",\n                \"email\": \"sakshi@promactinfo.com\",\n                \"image_24\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/7498d009258e42823eb2c85b9e329184.jpg?s=24&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0002-24.png\",\n                \"image_32\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/7498d009258e42823eb2c85b9e329184.jpg?s=32&d=https%3A%2F%2Fa.slack-edge.com%2F0180%2Fimg%2Favatars%2Fava_0002-32.png\",\n                \"image_48\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/7498d009258e42823eb2c85b9e329184.jpg?s=48&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0002-48.png\",\n                \"image_72\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/7498d009258e42823eb2c85b9e329184.jpg?s=72&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0002-72.png\",\n                \"image_192\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/7498d009258e42823eb2c85b9e329184.jpg?s=192&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0002-192.png\",\n                \"image_512\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/7498d009258e42823eb2c85b9e329184.jpg?s=512&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0002-512.png\"\n            }\n        },\n        {\n            \"id\": \"U1N2DFVBM\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"scrumbot\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"ea2977\",\n            \"real_name\": \"Scrum Bot\",\n            \"tz\": null,\n            \"tz_label\": \"Pacific Daylight Time\",\n            \"tz_offset\": -25200,\n            \"profile\": {\n                \"bot_id\": \"B1N49A1PU\",\n                \"api_app_id\": \"\",\n                \"avatar_hash\": \"33ba89d55d0e\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-01\\/56141305665_33ba89d55d0e8f8bf1a7_24.png\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-01\\/56141305665_33ba89d55d0e8f8bf1a7_32.png\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-01\\/56141305665_33ba89d55d0e8f8bf1a7_48.png\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-01\\/56141305665_33ba89d55d0e8f8bf1a7_72.png\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-01\\/56141305665_33ba89d55d0e8f8bf1a7_192.png\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-01\\/56141305665_33ba89d55d0e8f8bf1a7_512.png\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-01\\/56141305665_33ba89d55d0e8f8bf1a7_512.png\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-01\\/56141305665_33ba89d55d0e8f8bf1a7_original.png\",\n                \"first_name\": \"Scrum\",\n                \"last_name\": \"Bot\",\n                \"title\": \"Conducts scrum meeting regularly. \",\n                \"real_name\": \"Scrum Bot\",\n                \"real_name_normalized\": \"Scrum Bot\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": true\n        },\n        {\n            \"id\": \"U24QNPAH0\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"scrummeeting\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"619a4f\",\n            \"real_name\": \"\",\n            \"tz\": null,\n            \"tz_label\": \"Pacific Daylight Time\",\n            \"tz_offset\": -25200,\n            \"profile\": {\n                \"bot_id\": \"B24QF4MU7\",\n                \"api_app_id\": \"\",\n                \"avatar_hash\": \"81b481197892\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-25\\/72832416561_81b4811978924d1d7e35_24.png\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-25\\/72832416561_81b4811978924d1d7e35_32.png\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-25\\/72832416561_81b4811978924d1d7e35_48.png\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-25\\/72832416561_81b4811978924d1d7e35_72.png\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-25\\/72832416561_81b4811978924d1d7e35_192.png\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-25\\/72832416561_81b4811978924d1d7e35_512.png\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-25\\/72832416561_81b4811978924d1d7e35_512.png\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-25\\/72832416561_81b4811978924d1d7e35_original.png\",\n                \"real_name\": \"\",\n                \"real_name_normalized\": \"\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": true\n        },\n        {\n            \"id\": \"U1VV3V3CL\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"scrumtime\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"827327\",\n            \"real_name\": \"Scrum Time\",\n            \"tz\": null,\n            \"tz_label\": \"Pacific Daylight Time\",\n            \"tz_offset\": -25200,\n            \"profile\": {\n                \"bot_id\": \"B1W0A837C\",\n                \"api_app_id\": \"\",\n                \"avatar_hash\": \"8720c4457f2d\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-28\\/63946007859_8720c4457f2da9aa682a_24.png\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-28\\/63946007859_8720c4457f2da9aa682a_32.png\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-28\\/63946007859_8720c4457f2da9aa682a_48.png\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-28\\/63946007859_8720c4457f2da9aa682a_48.png\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-28\\/63946007859_8720c4457f2da9aa682a_48.png\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-28\\/63946007859_8720c4457f2da9aa682a_48.png\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-28\\/63946007859_8720c4457f2da9aa682a_48.png\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-28\\/63946007859_8720c4457f2da9aa682a_original.png\",\n                \"first_name\": \"Scrum\",\n                \"last_name\": \"Time\",\n                \"title\": \"Conducts Scrum Meeting\",\n                \"real_name\": \"Scrum Time\",\n                \"real_name_normalized\": \"Scrum Time\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": true\n        },\n        {\n            \"id\": \"U051J5H4G\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"shagun\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"a2a5dc\",\n            \"real_name\": \"Shagun Parikh\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"first_name\": \"Shagun\",\n                \"last_name\": \"Parikh\",\n                \"title\": \"Android Developer\",\n                \"skype\": \"Shagun\",\n                \"phone\": \"\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2015-09-09\\/10406902197_f495df89690b1c792a67_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2015-09-09\\/10406902197_f495df89690b1c792a67_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2015-09-09\\/10406902197_f495df89690b1c792a67_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2015-09-09\\/10406902197_f495df89690b1c792a67_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2015-09-09\\/10406902197_f495df89690b1c792a67_192.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2015-09-09\\/10406902197_f495df89690b1c792a67_512.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2015-09-09\\/10406902197_f495df89690b1c792a67_512.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2015-09-09\\/10406902197_f495df89690b1c792a67_original.jpg\",\n                \"avatar_hash\": \"f495df89690b\",\n                \"real_name\": \"Shagun Parikh\",\n                \"real_name_normalized\": \"Shagun Parikh\",\n                \"email\": \"shagun@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U051J13Q8\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"shailee\",\n            \"deleted\": false,\n            \"status\": \"?\",\n            \"color\": \"c386df\",\n            \"real_name\": \"Shailee Shah\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"first_name\": \"Shailee\",\n                \"last_name\": \"Shah\",\n                \"title\": \"Software Developer\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57537044007_721e28a498f3ccb45a92_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57537044007_721e28a498f3ccb45a92_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57537044007_721e28a498f3ccb45a92_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57537044007_721e28a498f3ccb45a92_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57537044007_721e28a498f3ccb45a92_192.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57537044007_721e28a498f3ccb45a92_original.jpg\",\n                \"avatar_hash\": \"721e28a498f3\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57537044007_721e28a498f3ccb45a92_512.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-07\\/57537044007_721e28a498f3ccb45a92_1024.jpg\",\n                \"phone\": \"\",\n                \"skype\": \"\",\n                \"real_name\": \"Shailee Shah\",\n                \"real_name_normalized\": \"Shailee Shah\",\n                \"email\": \"shailee@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U051ZTH8J\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"shreel\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"e23f99\",\n            \"real_name\": \"Shreel Bhatt\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"first_name\": \"Shreel\",\n                \"last_name\": \"Bhatt\",\n                \"title\": \"Web Designer\",\n                \"avatar_hash\": \"096c0db622cf\",\n                \"phone\": \"+91 9427784040\",\n                \"skype\": \"\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56877163474_096c0db622cf6f9fbfd3_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56877163474_096c0db622cf6f9fbfd3_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56877163474_096c0db622cf6f9fbfd3_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56877163474_096c0db622cf6f9fbfd3_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56877163474_096c0db622cf6f9fbfd3_72.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56877163474_096c0db622cf6f9fbfd3_72.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56877163474_096c0db622cf6f9fbfd3_72.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56877163474_096c0db622cf6f9fbfd3_original.jpg\",\n                \"real_name\": \"Shreel Bhatt\",\n                \"real_name_normalized\": \"Shreel Bhatt\",\n                \"email\": \"shreel@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U0525LDMF\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"shreyash\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"e96699\",\n            \"real_name\": \"Shreyash Mahajan\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56812536403_40a0bbd0877e0a03b5ca_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56812536403_40a0bbd0877e0a03b5ca_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56812536403_40a0bbd0877e0a03b5ca_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56812536403_40a0bbd0877e0a03b5ca_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56812536403_40a0bbd0877e0a03b5ca_192.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56812536403_40a0bbd0877e0a03b5ca_original.jpg\",\n                \"first_name\": \"Shreyash\",\n                \"last_name\": \"Mahajan\",\n                \"avatar_hash\": \"40a0bbd0877e\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56812536403_40a0bbd0877e0a03b5ca_192.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-05\\/56812536403_40a0bbd0877e0a03b5ca_192.jpg\",\n                \"title\": \"Team Leader\",\n                \"phone\": \"\",\n                \"skype\": \"shreyash.promact\",\n                \"real_name\": \"Shreyash Mahajan\",\n                \"real_name_normalized\": \"Shreyash Mahajan\",\n                \"email\": \"shreyash@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U0HJ49KJ4\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"siddhartha\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"9f69e7\",\n            \"real_name\": \"Siddhartha Shaw\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"avatar_hash\": \"f15fd61e96dc\",\n                \"first_name\": \"Siddhartha\",\n                \"last_name\": \"Shaw\",\n                \"title\": \"Software Developer \",\n                \"phone\": \"9432180440\",\n                \"skype\": \"\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-09-06\\/76445800530_f15fd61e96dc9e051eb5_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-09-06\\/76445800530_f15fd61e96dc9e051eb5_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-09-06\\/76445800530_f15fd61e96dc9e051eb5_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-09-06\\/76445800530_f15fd61e96dc9e051eb5_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-09-06\\/76445800530_f15fd61e96dc9e051eb5_192.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-09-06\\/76445800530_f15fd61e96dc9e051eb5_512.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-09-06\\/76445800530_f15fd61e96dc9e051eb5_1024.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-09-06\\/76445800530_f15fd61e96dc9e051eb5_original.jpg\",\n                \"real_name\": \"Siddhartha Shaw\",\n                \"real_name_normalized\": \"Siddhartha Shaw\",\n                \"email\": \"siddhartha@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false,\n            \"has_2fa\": false\n        },\n        {\n            \"id\": \"U0525NTFZ\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"sneha\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"d55aef\",\n            \"real_name\": \"Sneha Jaiswal\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"first_name\": \"Sneha\",\n                \"last_name\": \"Jaiswal\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-03-04\\/24338919185_9b4e396fbbb8c8988d82_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-03-04\\/24338919185_9b4e396fbbb8c8988d82_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-03-04\\/24338919185_9b4e396fbbb8c8988d82_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-03-04\\/24338919185_9b4e396fbbb8c8988d82_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-03-04\\/24338919185_9b4e396fbbb8c8988d82_192.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-03-04\\/24338919185_9b4e396fbbb8c8988d82_512.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-03-04\\/24338919185_9b4e396fbbb8c8988d82_1024.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-03-04\\/24338919185_9b4e396fbbb8c8988d82_original.jpg\",\n                \"avatar_hash\": \"9b4e396fbbb8\",\n                \"title\": \"QA Software Tester\",\n                \"phone\": \"9998129006\",\n                \"skype\": \"sneha@promactinfo.com\",\n                \"real_name\": \"Sneha Jaiswal\",\n                \"real_name_normalized\": \"Sneha Jaiswal\",\n                \"email\": \"sneha@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U0LH9R9V4\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"subcurrent\",\n            \"deleted\": true,\n            \"profile\": {\n                \"bot_id\": \"B0LH9G6F9\",\n                \"api_app_id\": \"A04E6JX41\",\n                \"first_name\": \"Polls by Subcurrent\",\n                \"avatar_hash\": \"1184171918f3\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-03-10\\/25942073984_1184171918f34752628f_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-03-10\\/25942073984_1184171918f34752628f_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-03-10\\/25942073984_1184171918f34752628f_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-03-10\\/25942073984_1184171918f34752628f_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-03-10\\/25942073984_1184171918f34752628f_192.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-03-10\\/25942073984_1184171918f34752628f_192.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-03-10\\/25942073984_1184171918f34752628f_192.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-03-10\\/25942073984_1184171918f34752628f_original.jpg\",\n                \"real_name\": \"Polls by Subcurrent\",\n                \"real_name_normalized\": \"Polls by Subcurrent\"\n            },\n            \"is_bot\": true\n        },\n        {\n            \"id\": \"U04K6NL6A\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"swaty\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"9f69e7\",\n            \"real_name\": \"Swaty Shah\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"first_name\": \"Swaty\",\n                \"last_name\": \"Shah\",\n                \"skype\": \"swaty.promact\",\n                \"title\": \"CEO & Creative Head\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-04-08\\/33149441602_cad676972c3196835110_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-04-08\\/33149441602_cad676972c3196835110_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-04-08\\/33149441602_cad676972c3196835110_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-04-08\\/33149441602_cad676972c3196835110_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-04-08\\/33149441602_cad676972c3196835110_192.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-04-08\\/33149441602_cad676972c3196835110_original.jpg\",\n                \"avatar_hash\": \"cad676972c31\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-04-08\\/33149441602_cad676972c3196835110_512.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-04-08\\/33149441602_cad676972c3196835110_1024.jpg\",\n                \"phone\": \"\",\n                \"real_name\": \"Swaty Shah\",\n                \"real_name_normalized\": \"Swaty Shah\",\n                \"email\": \"swaty@promactinfo.com\"\n            },\n            \"is_admin\": true,\n            \"is_owner\": true,\n            \"is_primary_owner\": true,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U1Z3M1VNJ\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"tamanna\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"4ec0d6\",\n            \"real_name\": \"Tamanna Bhatt\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"avatar_hash\": \"43550e07840f\",\n                \"first_name\": \"Tamanna\",\n                \"last_name\": \"Bhatt\",\n                \"title\": \"traning\",\n                \"phone\": \"7600611633\",\n                \"skype\": \"tamanna_bhatt\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-16\\/69730866101_43550e07840f8cbcec83_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-16\\/69730866101_43550e07840f8cbcec83_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-16\\/69730866101_43550e07840f8cbcec83_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-16\\/69730866101_43550e07840f8cbcec83_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-16\\/69730866101_43550e07840f8cbcec83_192.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-16\\/69730866101_43550e07840f8cbcec83_512.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-16\\/69730866101_43550e07840f8cbcec83_1024.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-16\\/69730866101_43550e07840f8cbcec83_original.jpg\",\n                \"real_name\": \"Tamanna Bhatt\",\n                \"real_name_normalized\": \"Tamanna Bhatt\",\n                \"email\": \"tamanna@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U1TNNLV0R\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"task\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"de5f24\",\n            \"real_name\": \"\",\n            \"tz\": null,\n            \"tz_label\": \"Pacific Daylight Time\",\n            \"tz_offset\": -25200,\n            \"profile\": {\n                \"bot_id\": \"B1TNGSKFX\",\n                \"api_app_id\": \"\",\n                \"avatar_hash\": \"d0d83c888460\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-21\\/61770710033_d0d83c888460e1f437ad_24.png\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-21\\/61770710033_d0d83c888460e1f437ad_32.png\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-21\\/61770710033_d0d83c888460e1f437ad_48.png\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-21\\/61770710033_d0d83c888460e1f437ad_72.png\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-21\\/61770710033_d0d83c888460e1f437ad_192.png\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-21\\/61770710033_d0d83c888460e1f437ad_512.png\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-21\\/61770710033_d0d83c888460e1f437ad_512.png\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-21\\/61770710033_d0d83c888460e1f437ad_original.png\",\n                \"real_name\": \"\",\n                \"real_name_normalized\": \"\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": true\n        },\n        {\n            \"id\": \"U1TAQGYDS\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"taskmail\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"d1707d\",\n            \"real_name\": \"\",\n            \"tz\": null,\n            \"tz_label\": \"Pacific Daylight Time\",\n            \"tz_offset\": -25200,\n            \"profile\": {\n                \"bot_id\": \"B1TAUS60G\",\n                \"api_app_id\": \"\",\n                \"avatar_hash\": \"aa3d08cc1025\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61366256534_aa3d08cc10257c4bee7c_24.png\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61366256534_aa3d08cc10257c4bee7c_32.png\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61366256534_aa3d08cc10257c4bee7c_48.png\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61366256534_aa3d08cc10257c4bee7c_72.png\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61366256534_aa3d08cc10257c4bee7c_192.png\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61366256534_aa3d08cc10257c4bee7c_512.png\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61366256534_aa3d08cc10257c4bee7c_512.png\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61366256534_aa3d08cc10257c4bee7c_original.png\",\n                \"real_name\": \"\",\n                \"real_name_normalized\": \"\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": true\n        },\n        {\n            \"id\": \"U257ZD738\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"taskmail2\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"a72f79\",\n            \"real_name\": \"Promact\",\n            \"tz\": null,\n            \"tz_label\": \"Pacific Daylight Time\",\n            \"tz_offset\": -25200,\n            \"profile\": {\n                \"bot_id\": \"B25870W05\",\n                \"api_app_id\": \"A1WCQ9A3S\",\n                \"first_name\": \"Promact\",\n                \"avatar_hash\": \"c48a498a9316\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-31\\/74776871392_c48a498a93161f47f385_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-31\\/74776871392_c48a498a93161f47f385_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-31\\/74776871392_c48a498a93161f47f385_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-31\\/74776871392_c48a498a93161f47f385_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-31\\/74776871392_c48a498a93161f47f385_192.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-31\\/74776871392_c48a498a93161f47f385_512.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-31\\/74776871392_c48a498a93161f47f385_512.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-08-31\\/74776871392_c48a498a93161f47f385_original.jpg\",\n                \"real_name\": \"Promact\",\n                \"real_name_normalized\": \"Promact\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": true\n        },\n        {\n            \"id\": \"U063WN339\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"test1\",\n            \"deleted\": true,\n            \"profile\": {\n                \"bot_id\": \"B063WLD5K\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2015-06-09\\/6132760450_ec8c787fe2d5f106e62f_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2015-06-09\\/6132760450_ec8c787fe2d5f106e62f_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2015-06-09\\/6132760450_ec8c787fe2d5f106e62f_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2015-06-09\\/6132760450_ec8c787fe2d5f106e62f_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2015-06-09\\/6132760450_ec8c787fe2d5f106e62f_192.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2015-06-09\\/6132760450_ec8c787fe2d5f106e62f_original.jpg\",\n                \"avatar_hash\": \"ec8c787fe2d5\",\n                \"real_name\": \"\",\n                \"real_name_normalized\": \"\"\n            },\n            \"is_bot\": true\n        },\n        {\n            \"id\": \"U1TCGC6H5\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"testbot\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"902d59\",\n            \"real_name\": \"\",\n            \"tz\": null,\n            \"tz_label\": \"Pacific Daylight Time\",\n            \"tz_offset\": -25200,\n            \"profile\": {\n                \"bot_id\": \"B1TCGC6AK\",\n                \"api_app_id\": \"\",\n                \"avatar_hash\": \"7028b523a777\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61424282837_7028b523a777e51933bd_24.png\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61424282837_7028b523a777e51933bd_32.png\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61424282837_7028b523a777e51933bd_48.png\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61424282837_7028b523a777e51933bd_72.png\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61424282837_7028b523a777e51933bd_192.png\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61424282837_7028b523a777e51933bd_512.png\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61424282837_7028b523a777e51933bd_512.png\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61424282837_7028b523a777e51933bd_original.png\",\n                \"real_name\": \"\",\n                \"real_name_normalized\": \"\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": true\n        },\n        {\n            \"id\": \"U1L0KHJQN\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"trello\",\n            \"deleted\": true,\n            \"profile\": {\n                \"bot_id\": \"B1L0ZECKB\",\n                \"api_app_id\": \"A074YH40Z\",\n                \"first_name\": \"Trello\",\n                \"avatar_hash\": \"2afa1c026330\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-24\\/54033491569_2afa1c026330ace08515_24.png\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-24\\/54033491569_2afa1c026330ace08515_32.png\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-24\\/54033491569_2afa1c026330ace08515_48.png\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-24\\/54033491569_2afa1c026330ace08515_72.png\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-24\\/54033491569_2afa1c026330ace08515_192.png\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-24\\/54033491569_2afa1c026330ace08515_512.png\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-24\\/54033491569_2afa1c026330ace08515_512.png\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-24\\/54033491569_2afa1c026330ace08515_original.png\",\n                \"real_name\": \"Trello\",\n                \"real_name_normalized\": \"Trello\"\n            },\n            \"is_bot\": true\n        },\n        {\n            \"id\": \"U1TB1EN87\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"tsakmail\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"43761b\",\n            \"real_name\": \"Task  Mail\",\n            \"tz\": null,\n            \"tz_label\": \"Pacific Daylight Time\",\n            \"tz_offset\": -25200,\n            \"profile\": {\n                \"bot_id\": \"B1TB79DJ5\",\n                \"api_app_id\": \"\",\n                \"avatar_hash\": \"d779b1f2ccad\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61375618199_d779b1f2ccadebc1ab40_24.png\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61375618199_d779b1f2ccadebc1ab40_32.png\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61375618199_d779b1f2ccadebc1ab40_48.png\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61375618199_d779b1f2ccadebc1ab40_48.png\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61375618199_d779b1f2ccadebc1ab40_48.png\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61375618199_d779b1f2ccadebc1ab40_48.png\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61375618199_d779b1f2ccadebc1ab40_48.png\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-07-20\\/61375618199_d779b1f2ccadebc1ab40_original.png\",\n                \"first_name\": \"Task\",\n                \"last_name\": \" Mail\",\n                \"title\": \"send daily task \",\n                \"real_name\": \"Task  Mail\",\n                \"real_name_normalized\": \"Task  Mail\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": true\n        },\n        {\n            \"id\": \"U051J3RQL\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"vishal_the_employee\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"8f4a2b\",\n            \"real_name\": \"Vishal Kumar Mitra\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"first_name\": \"Vishal\",\n                \"last_name\": \"Kumar Mitra\",\n                \"avatar_hash\": \"g9fb9160651a\",\n                \"title\": \"Develop web application, mobile applications and front side development with the power of dotnet technology\",\n                \"phone\": \"8758833454\",\n                \"skype\": \"\",\n                \"real_name\": \"Vishal Kumar Mitra\",\n                \"real_name_normalized\": \"Vishal Kumar Mitra\",\n                \"email\": \"vishal@promactinfo.com\",\n                \"image_24\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/9fb9160651a32c8e11e5828012a11792.jpg?s=24&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0018-24.png\",\n                \"image_32\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/9fb9160651a32c8e11e5828012a11792.jpg?s=32&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0018-32.png\",\n                \"image_48\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/9fb9160651a32c8e11e5828012a11792.jpg?s=48&d=https%3A%2F%2Fa.slack-edge.com%2F66f9%2Fimg%2Favatars%2Fava_0018-48.png\",\n                \"image_72\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/9fb9160651a32c8e11e5828012a11792.jpg?s=72&d=https%3A%2F%2Fa.slack-edge.com%2F3654%2Fimg%2Favatars%2Fava_0018-72.png\",\n                \"image_192\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/9fb9160651a32c8e11e5828012a11792.jpg?s=192&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0018-192.png\",\n                \"image_512\": \"https:\\/\\/secure.gravatar.com\\/avatar\\/9fb9160651a32c8e11e5828012a11792.jpg?s=512&d=https%3A%2F%2Fa.slack-edge.com%2F7fa9%2Fimg%2Favatars%2Fava_0018-512.png\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U051J2QKG\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"vivekmitra\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"d1707d\",\n            \"real_name\": \"Vivek Mitra\",\n            \"tz\": \"Asia\\/Kolkata\",\n            \"tz_label\": \"India Standard Time\",\n            \"tz_offset\": 19800,\n            \"profile\": {\n                \"first_name\": \"Vivek\",\n                \"last_name\": \"Mitra\",\n                \"avatar_hash\": \"762f356b8ed5\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-24\\/53960066869_762f356b8ed5768f4f87_24.jpg\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-24\\/53960066869_762f356b8ed5768f4f87_32.jpg\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-24\\/53960066869_762f356b8ed5768f4f87_48.jpg\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-24\\/53960066869_762f356b8ed5768f4f87_72.jpg\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-24\\/53960066869_762f356b8ed5768f4f87_192.jpg\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-24\\/53960066869_762f356b8ed5768f4f87_512.jpg\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-24\\/53960066869_762f356b8ed5768f4f87_1024.jpg\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-24\\/53960066869_762f356b8ed5768f4f87_original.jpg\",\n                \"title\": \"\",\n                \"phone\": \"\",\n                \"skype\": \"\",\n                \"real_name\": \"Vivek Mitra\",\n                \"real_name_normalized\": \"Vivek Mitra\",\n                \"email\": \"kumarvivek@promactinfo.com\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        },\n        {\n            \"id\": \"U0Y1E5MC4\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"workbot\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"2b6836\",\n            \"real_name\": \"Workbot\",\n            \"tz\": null,\n            \"tz_label\": \"Pacific Daylight Time\",\n            \"tz_offset\": -25200,\n            \"profile\": {\n                \"bot_id\": \"B0Y1V715X\",\n                \"api_app_id\": \"A0HVDHQ8Z\",\n                \"first_name\": \"Workbot\",\n                \"avatar_hash\": \"4fb09ead09f8\",\n                \"image_24\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-22\\/53166464497_4fb09ead09f894d73490_24.png\",\n                \"image_32\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-22\\/53166464497_4fb09ead09f894d73490_32.png\",\n                \"image_48\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-22\\/53166464497_4fb09ead09f894d73490_48.png\",\n                \"image_72\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-22\\/53166464497_4fb09ead09f894d73490_72.png\",\n                \"image_192\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-22\\/53166464497_4fb09ead09f894d73490_192.png\",\n                \"image_512\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-22\\/53166464497_4fb09ead09f894d73490_512.png\",\n                \"image_1024\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-22\\/53166464497_4fb09ead09f894d73490_512.png\",\n                \"image_original\": \"https:\\/\\/avatars.slack-edge.com\\/2016-06-22\\/53166464497_4fb09ead09f894d73490_original.png\",\n                \"real_name\": \"Workbot\",\n                \"real_name_normalized\": \"Workbot\"\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": true\n        },\n        {\n            \"id\": \"USLACKBOT\",\n            \"team_id\": \"T04K6NL66\",\n            \"name\": \"slackbot\",\n            \"deleted\": false,\n            \"status\": null,\n            \"color\": \"757575\",\n            \"real_name\": \"slackbot\",\n            \"tz\": null,\n            \"tz_label\": \"Pacific Daylight Time\",\n            \"tz_offset\": -25200,\n            \"profile\": {\n                \"first_name\": \"slackbot\",\n                \"last_name\": \"\",\n                \"image_24\": \"https:\\/\\/a.slack-edge.com\\/0180\\/img\\/slackbot_24.png\",\n                \"image_32\": \"https:\\/\\/a.slack-edge.com\\/2fac\\/plugins\\/slackbot\\/assets\\/service_32.png\",\n                \"image_48\": \"https:\\/\\/a.slack-edge.com\\/2fac\\/plugins\\/slackbot\\/assets\\/service_48.png\",\n                \"image_72\": \"https:\\/\\/a.slack-edge.com\\/0180\\/img\\/slackbot_72.png\",\n                \"image_192\": \"https:\\/\\/a.slack-edge.com\\/66f9\\/img\\/slackbot_192.png\",\n                \"image_512\": \"https:\\/\\/a.slack-edge.com\\/1801\\/img\\/slackbot_512.png\",\n                \"avatar_hash\": \"sv1444671949\",\n                \"real_name\": \"slackbot\",\n                \"real_name_normalized\": \"slackbot\",\n                \"email\": null,\n                \"fields\": null\n            },\n            \"is_admin\": false,\n            \"is_owner\": false,\n            \"is_primary_owner\": false,\n            \"is_restricted\": false,\n            \"is_ultra_restricted\": false,\n            \"is_bot\": false\n        }\n    ],\n    \"cache_ts\": 1473237231\n}\n";
            }
        }
        public string ChannelDetailsResponseText
        {
            get
            {
                return "{\n    \"ok\": true,\n    \"channels\": [\n        {\n            \"id\": \"C0L1RME2Y\",\n            \"name\": \"2016fundays-committee\",\n            \"is_channel\": true,\n            \"created\": 1454396958,\n            \"creator\": \"U05256USX\",\n            \"is_archived\": false,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [\n                \"U04K6NL6A\",\n                \"U051HV8JE\",\n                \"U051J0GN4\",\n                \"U051J2QKG\",\n                \"U051J3RQL\",\n                \"U051ZTH8J\",\n                \"U05256USX\",\n                \"U0525LDMF\",\n                \"U0525MNTH\",\n                \"U0525NC63\",\n                \"U0525NTFZ\",\n                \"U0526N37K\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 12\n        },\n        {\n            \"id\": \"C1U5ZHVND\",\n            \"name\": \"56ty\",\n            \"is_channel\": true,\n            \"created\": 1469176011,\n            \"creator\": \"U06NVGLPQ\",\n            \"is_archived\": false,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [\n                \"U06NVGLPQ\",\n                \"U24QNPAH0\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"fdsfdsf\",\n                \"creator\": \"U06NVGLPQ\",\n                \"last_set\": 1469176013\n            },\n            \"num_members\": 2\n        },\n        {\n            \"id\": \"C0FSXU7FT\",\n            \"name\": \"bhailu\",\n            \"is_channel\": true,\n            \"created\": 1449203999,\n            \"creator\": \"U051J3RQL\",\n            \"is_archived\": true,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"Tympass karvanu majani life\",\n                \"creator\": \"U051J3RQL\",\n                \"last_set\": 1449204083\n            },\n            \"num_members\": 0\n        },\n        {\n            \"id\": \"C289EMX28\",\n            \"name\": \"breakers\",\n            \"is_channel\": true,\n            \"created\": 1473074691,\n            \"creator\": \"U051J3RQL\",\n            \"is_archived\": true,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 0\n        },\n        {\n            \"id\": \"C0YATJRPZ\",\n            \"name\": \"checkabcsa\",\n            \"is_channel\": true,\n            \"created\": 1459923176,\n            \"creator\": \"U051J3RQL\",\n            \"is_archived\": false,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 0\n        },\n        {\n            \"id\": \"C0H9JJV8U\",\n            \"name\": \"christmas-celebration\",\n            \"is_channel\": true,\n            \"created\": 1450936828,\n            \"creator\": \"U04K6NL6A\",\n            \"is_archived\": true,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 0\n        },\n        {\n            \"id\": \"C0REM6H61\",\n            \"name\": \"creativecellcore\",\n            \"is_channel\": true,\n            \"created\": 1457513091,\n            \"creator\": \"U051J3RQL\",\n            \"is_archived\": true,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 0\n        },\n        {\n            \"id\": \"C21M7UUDC\",\n            \"name\": \"dailyupdates\",\n            \"is_channel\": true,\n            \"created\": 1471322010,\n            \"creator\": \"U1MVA0H3M\",\n            \"is_archived\": false,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [\n                \"U0525LCJR\",\n                \"U0HJ48P4Y\",\n                \"U0VR8DA0Y\",\n                \"U1MVA0H3M\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 3\n        },\n        {\n            \"id\": \"C1MASD60Z\",\n            \"name\": \"dotnetdevs\",\n            \"is_channel\": true,\n            \"created\": 1467202077,\n            \"creator\": \"U04KUAFSH\",\n            \"is_archived\": false,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [\n                \"U04KUAFSH\",\n                \"U051HV7DA\",\n                \"U051HV8JE\",\n                \"U051HV8N8\",\n                \"U051J13Q8\",\n                \"U051J2RTW\",\n                \"U051J3RQL\",\n                \"U051J44DE\",\n                \"U051KDCAL\",\n                \"U0525LCJR\",\n                \"U0525LFLT\",\n                \"U0525M2P1\",\n                \"U0526N37K\",\n                \"U06NVGLPQ\",\n                \"U09UNVC20\",\n                \"U0FQH9DB9\",\n                \"U0VR8DA0Y\",\n                \"U1C74JGTU\",\n                \"U1D6AU01F\",\n                \"U1M8GSPPC\",\n                \"U1MVA0H3M\",\n                \"U1MVBJE2X\",\n                \"U1N1CA4C8\",\n                \"U1N1P8X89\",\n                \"U1Z3M1VNJ\",\n                \"U23EZH2TW\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"Discussion channel for .Net devs\",\n                \"creator\": \"U04KUAFSH\",\n                \"last_set\": 1467202079\n            },\n            \"num_members\": 20\n        },\n        {\n            \"id\": \"C0L2NG9DY\",\n            \"name\": \"fundays-team\",\n            \"is_channel\": true,\n            \"created\": 1454416405,\n            \"creator\": \"U051ZTH8J\",\n            \"is_archived\": true,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 0\n        },\n        {\n            \"id\": \"C0GD9QPB2\",\n            \"name\": \"game-practice\",\n            \"is_channel\": true,\n            \"created\": 1449825281,\n            \"creator\": \"U0526N37K\",\n            \"is_archived\": true,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 0\n        },\n        {\n            \"id\": \"C0L2GGJBW\",\n            \"name\": \"garageband\",\n            \"is_channel\": true,\n            \"created\": 1454416621,\n            \"creator\": \"U051J5H4G\",\n            \"is_archived\": true,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 0\n        },\n        {\n            \"id\": \"C04K6NL6N\",\n            \"name\": \"general\",\n            \"is_channel\": true,\n            \"created\": 1430302677,\n            \"creator\": \"U04K6NL6A\",\n            \"is_archived\": false,\n            \"is_general\": true,\n            \"is_member\": true,\n            \"members\": [\n                \"U04K6NL6A\",\n                \"U04KUAFSH\",\n                \"U051HV7DA\",\n                \"U051HV8JE\",\n                \"U051HV8N8\",\n                \"U051HV9KY\",\n                \"U051J094A\",\n                \"U051J0GN4\",\n                \"U051J0M2A\",\n                \"U051J13Q8\",\n                \"U051J17PA\",\n                \"U051J2QKG\",\n                \"U051J2RTW\",\n                \"U051J3RQL\",\n                \"U051J44DE\",\n                \"U051J5H4G\",\n                \"U051JE2JW\",\n                \"U051KDCAL\",\n                \"U051ZTH8J\",\n                \"U05256USX\",\n                \"U0525L7NT\",\n                \"U0525LCJR\",\n                \"U0525LDMF\",\n                \"U0525LF29\",\n                \"U0525LFLT\",\n                \"U0525LH1B\",\n                \"U0525LLBZ\",\n                \"U0525LW55\",\n                \"U0525M2P1\",\n                \"U0525MFND\",\n                \"U0525MNTH\",\n                \"U0525MXL3\",\n                \"U0525N6FF\",\n                \"U0525N6NB\",\n                \"U0525NC63\",\n                \"U0525NTFZ\",\n                \"U0525PKAK\",\n                \"U0525QHQ1\",\n                \"U05263AL9\",\n                \"U0526JFLM\",\n                \"U0526N37K\",\n                \"U0545BH7Q\",\n                \"U063WN339\",\n                \"U06CU2VB9\",\n                \"U06NVGLPQ\",\n                \"U07B75H0T\",\n                \"U07B774M8\",\n                \"U082PJG72\",\n                \"U085QD3QB\",\n                \"U08GCPE0J\",\n                \"U08KM481G\",\n                \"U09G8CUJ3\",\n                \"U09UNVC20\",\n                \"U0FAVB57W\",\n                \"U0FQH9DB9\",\n                \"U0H7R5MPS\",\n                \"U0HJ33E49\",\n                \"U0HJ34YU9\",\n                \"U0HJ48P4Y\",\n                \"U0HJ49KJ4\",\n                \"U0J4V5EEQ\",\n                \"U0N3WUJR4\",\n                \"U0U36HKV1\",\n                \"U0VR8DA0Y\",\n                \"U0ZH5UYSJ\",\n                \"U1267JY10\",\n                \"U1C74JGTU\",\n                \"U1D6AU01F\",\n                \"U1JBA2Z5G\",\n                \"U1M83P1U0\",\n                \"U1M8GSPPC\",\n                \"U1MVA0H3M\",\n                \"U1MVBJE2X\",\n                \"U1N1CA4C8\",\n                \"U1N1P8X89\",\n                \"U1WTH2LPM\",\n                \"U1Z3M1VNJ\",\n                \"U23EZH2TW\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"This channel is for team-wide communication and announcements. All team members are in this channel.\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 45\n        },\n        {\n            \"id\": \"C1U69P81Z\",\n            \"name\": \"girlssss\",\n            \"is_channel\": true,\n            \"created\": 1469193785,\n            \"creator\": \"U1N1P8X89\",\n            \"is_archived\": true,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 0\n        },\n        {\n            \"id\": \"C0G4VDCCD\",\n            \"name\": \"group3\",\n            \"is_channel\": true,\n            \"created\": 1449570805,\n            \"creator\": \"U05256USX\",\n            \"is_archived\": false,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [\n                \"U051J3RQL\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 1\n        },\n        {\n            \"id\": \"C0HT38E4V\",\n            \"name\": \"guidelines\",\n            \"is_channel\": true,\n            \"created\": 1452149726,\n            \"creator\": \"U04KUAFSH\",\n            \"is_archived\": false,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [\n                \"U04KUAFSH\",\n                \"U051HV8JE\",\n                \"U051J2QKG\",\n                \"U0525L7NT\",\n                \"U0525LCJR\",\n                \"U0525M2P1\",\n                \"U0525NC63\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 6\n        },\n        {\n            \"id\": \"C0QPX6R55\",\n            \"name\": \"hetul_marriage_gift\",\n            \"is_channel\": true,\n            \"created\": 1457333161,\n            \"creator\": \"U051J2QKG\",\n            \"is_archived\": false,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [\n                \"U051J2QKG\",\n                \"U0525LDMF\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 2\n        },\n        {\n            \"id\": \"C0DJ77J75\",\n            \"name\": \"infoatone\",\n            \"is_channel\": true,\n            \"created\": 1446290050,\n            \"creator\": \"U0525LFLT\",\n            \"is_archived\": true,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 0\n        },\n        {\n            \"id\": \"C0ESC669F\",\n            \"name\": \"ionic-discussion\",\n            \"is_channel\": true,\n            \"created\": 1447910532,\n            \"creator\": \"U0525NC63\",\n            \"is_archived\": false,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"U0525NC63\",\n                \"last_set\": 1447995130\n            },\n            \"num_members\": 0\n        },\n        {\n            \"id\": \"C0DQGMZHS\",\n            \"name\": \"jf-ionic\",\n            \"is_channel\": true,\n            \"created\": 1446615575,\n            \"creator\": \"U051J5H4G\",\n            \"is_archived\": true,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 0\n        },\n        {\n            \"id\": \"C0525MXM3\",\n            \"name\": \"justfollow\",\n            \"is_channel\": true,\n            \"created\": 1432634223,\n            \"creator\": \"U04KUAFSH\",\n            \"is_archived\": true,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"Discussion about ideas in JustFollow\",\n                \"creator\": \"U04KUAFSH\",\n                \"last_set\": 1432634225\n            },\n            \"num_members\": 0\n        },\n        {\n            \"id\": \"C0DG1K4RZ\",\n            \"name\": \"justfollow_tl\",\n            \"is_channel\": true,\n            \"created\": 1446189948,\n            \"creator\": \"U051J5H4G\",\n            \"is_archived\": false,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [\n                \"U04KUAFSH\",\n                \"U051HV7DA\",\n                \"U051J5H4G\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 3\n        },\n        {\n            \"id\": \"C259V81T6\",\n            \"name\": \"mark\",\n            \"is_channel\": true,\n            \"created\": 1472214922,\n            \"creator\": \"U06NVGLPQ\",\n            \"is_archived\": false,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [\n                \"U06NVGLPQ\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 1\n        },\n        {\n            \"id\": \"C0FK1UXU3\",\n            \"name\": \"new-tea-group-pals\",\n            \"is_channel\": true,\n            \"created\": 1448966245,\n            \"creator\": \"U0525NTFZ\",\n            \"is_archived\": false,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [\n                \"U051HV8JE\",\n                \"U051J5H4G\",\n                \"U051ZTH8J\",\n                \"U05256USX\",\n                \"U0525LCJR\",\n                \"U0525NTFZ\",\n                \"U1D6AU01F\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 6\n        },\n        {\n            \"id\": \"C0G9V0UA0\",\n            \"name\": \"olympic-game-equipmen\",\n            \"is_channel\": true,\n            \"created\": 1449722693,\n            \"creator\": \"U051J5H4G\",\n            \"is_archived\": false,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [\n                \"U051J2QKG\",\n                \"U051J3RQL\",\n                \"U0525M2P1\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 3\n        },\n        {\n            \"id\": \"C1MAD1EKY\",\n            \"name\": \"phpdevs\",\n            \"is_channel\": true,\n            \"created\": 1467202367,\n            \"creator\": \"U04KUAFSH\",\n            \"is_archived\": false,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [\n                \"U04KUAFSH\",\n                \"U0525N6NB\",\n                \"U0U36HKV1\",\n                \"U0ZH5UYSJ\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"PHP development talks\",\n                \"creator\": \"U04KUAFSH\",\n                \"last_set\": 1467202369\n            },\n            \"num_members\": 4\n        },\n        {\n            \"id\": \"C0LU479UZ\",\n            \"name\": \"pictionarytiranga\",\n            \"is_channel\": true,\n            \"created\": 1455181062,\n            \"creator\": \"U051J3RQL\",\n            \"is_archived\": false,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [\n                \"U051J0GN4\",\n                \"U0HJ33E49\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 1\n        },\n        {\n            \"id\": \"C0MBHJHB9\",\n            \"name\": \"poetry\",\n            \"is_channel\": true,\n            \"created\": 1455525375,\n            \"creator\": \"U051J2QKG\",\n            \"is_archived\": false,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [\n                \"U04KUAFSH\",\n                \"U051J2QKG\",\n                \"U0525PKAK\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 3\n        },\n        {\n            \"id\": \"C0JAQG43H\",\n            \"name\": \"practicia\",\n            \"is_channel\": true,\n            \"created\": 1452664921,\n            \"creator\": \"U04K6NL6A\",\n            \"is_archived\": true,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 0\n        },\n        {\n            \"id\": \"C0HQE8SP6\",\n            \"name\": \"promact-cricket-team\",\n            \"is_channel\": true,\n            \"created\": 1452063350,\n            \"creator\": \"U051J3RQL\",\n            \"is_archived\": true,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 0\n        },\n        {\n            \"id\": \"C1NAHPZAB\",\n            \"name\": \"promact_closed\",\n            \"is_channel\": true,\n            \"created\": 1467435788,\n            \"creator\": \"U1M83P1U0\",\n            \"is_archived\": true,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 0\n        },\n        {\n            \"id\": \"C0EQ7FESU\",\n            \"name\": \"promactgirls\",\n            \"is_channel\": true,\n            \"created\": 1447845770,\n            \"creator\": \"U051J5H4G\",\n            \"is_archived\": false,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [\n                \"U04K6NL6A\",\n                \"U051HV7DA\",\n                \"U051HV8JE\",\n                \"U051J094A\",\n                \"U051J0GN4\",\n                \"U051J13Q8\",\n                \"U051J2RTW\",\n                \"U051J5H4G\",\n                \"U051ZTH8J\",\n                \"U05256USX\",\n                \"U0525LFLT\",\n                \"U0525MFND\",\n                \"U0525MXL3\",\n                \"U0525N6FF\",\n                \"U0525N6NB\",\n                \"U0525NTFZ\",\n                \"U0525QHQ1\",\n                \"U05263AL9\",\n                \"U0545BH7Q\",\n                \"U06NVGLPQ\",\n                \"U09G8CUJ3\",\n                \"U09UNVC20\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 16\n        },\n        {\n            \"id\": \"C1WM0NQ81\",\n            \"name\": \"promgirls\",\n            \"is_channel\": true,\n            \"created\": 1469869812,\n            \"creator\": \"U1N1P8X89\",\n            \"is_archived\": false,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [\n                \"U1MVBJE2X\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 0\n        },\n        {\n            \"id\": \"C04K6NL6Q\",\n            \"name\": \"random\",\n            \"is_channel\": true,\n            \"created\": 1430302677,\n            \"creator\": \"U04K6NL6A\",\n            \"is_archived\": false,\n            \"is_general\": false,\n            \"is_member\": true,\n            \"members\": [\n                \"U04K6NL6A\",\n                \"U04KUAFSH\",\n                \"U051HV7DA\",\n                \"U051HV8JE\",\n                \"U051HV8N8\",\n                \"U051HV9KY\",\n                \"U051J094A\",\n                \"U051J0GN4\",\n                \"U051J0M2A\",\n                \"U051J13Q8\",\n                \"U051J17PA\",\n                \"U051J2QKG\",\n                \"U051J2RTW\",\n                \"U051J3RQL\",\n                \"U051J44DE\",\n                \"U051J5H4G\",\n                \"U051JE2JW\",\n                \"U051KDCAL\",\n                \"U051ZTH8J\",\n                \"U05256USX\",\n                \"U0525L7NT\",\n                \"U0525LCJR\",\n                \"U0525LDMF\",\n                \"U0525LF29\",\n                \"U0525LFLT\",\n                \"U0525LH1B\",\n                \"U0525LLBZ\",\n                \"U0525LW55\",\n                \"U0525M2P1\",\n                \"U0525MFND\",\n                \"U0525MNTH\",\n                \"U0525MXL3\",\n                \"U0525N6FF\",\n                \"U0525N6NB\",\n                \"U0525NC63\",\n                \"U0525NTFZ\",\n                \"U0525PKAK\",\n                \"U0525QHQ1\",\n                \"U05263AL9\",\n                \"U0526JFLM\",\n                \"U0526N37K\",\n                \"U0545BH7Q\",\n                \"U06CU2VB9\",\n                \"U06NVGLPQ\",\n                \"U082PJG72\",\n                \"U085QD3QB\",\n                \"U08GCPE0J\",\n                \"U08KM481G\",\n                \"U09G8CUJ3\",\n                \"U09UNVC20\",\n                \"U0FAVB57W\",\n                \"U0FQH9DB9\",\n                \"U0H7R5MPS\",\n                \"U0HJ33E49\",\n                \"U0HJ34YU9\",\n                \"U0HJ48P4Y\",\n                \"U0HJ49KJ4\",\n                \"U0J4V5EEQ\",\n                \"U0N3WUJR4\",\n                \"U0U36HKV1\",\n                \"U0VR8DA0Y\",\n                \"U0ZH5UYSJ\",\n                \"U1267JY10\",\n                \"U1C74JGTU\",\n                \"U1D6AU01F\",\n                \"U1JBA2Z5G\",\n                \"U1M83P1U0\",\n                \"U1M8GSPPC\",\n                \"U1MVA0H3M\",\n                \"U1MVBJE2X\",\n                \"U1N1CA4C8\",\n                \"U1N1P8X89\",\n                \"U1WTH2LPM\",\n                \"U1Z3M1VNJ\",\n                \"U23EZH2TW\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"A place for non-work-related flimflam, faffing, hodge-podge or jibber-jabber you'd prefer to keep out of more focused work-related channels.\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 45\n        },\n        {\n            \"id\": \"C0DQZ2QET\",\n            \"name\": \"rangoli\",\n            \"is_channel\": true,\n            \"created\": 1446630551,\n            \"creator\": \"U0525NC63\",\n            \"is_archived\": true,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"Rangoli Competition on November 10th, 2015.\",\n                \"creator\": \"U0525NC63\",\n                \"last_set\": 1446630551\n            },\n            \"num_members\": 0\n        },\n        {\n            \"id\": \"C0DQV6T0D\",\n            \"name\": \"rangoli-group\",\n            \"is_channel\": true,\n            \"created\": 1446630675,\n            \"creator\": \"U051J13Q8\",\n            \"is_archived\": true,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 0\n        },\n        {\n            \"id\": \"C0DR03HK4\",\n            \"name\": \"rangoli-promact\",\n            \"is_channel\": true,\n            \"created\": 1446630765,\n            \"creator\": \"U051J3RQL\",\n            \"is_archived\": false,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [\n                \"U051J2QKG\",\n                \"U051J3RQL\",\n                \"U0525MXL3\",\n                \"U0545BH7Q\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"We need to win\",\n                \"creator\": \"U051J3RQL\",\n                \"last_set\": 1446630766\n            },\n            \"num_members\": 2\n        },\n        {\n            \"id\": \"C0DT63DR8\",\n            \"name\": \"rangoligroup\",\n            \"is_channel\": true,\n            \"created\": 1446698294,\n            \"creator\": \"U0526JFLM\",\n            \"is_archived\": true,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 0\n        },\n        {\n            \"id\": \"C0L2NQQDD\",\n            \"name\": \"skit-group\",\n            \"is_channel\": true,\n            \"created\": 1454416774,\n            \"creator\": \"U0525MNTH\",\n            \"is_archived\": true,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 0\n        },\n        {\n            \"id\": \"C1PFYNB3P\",\n            \"name\": \"slackbothchek\",\n            \"is_channel\": true,\n            \"created\": 1467870987,\n            \"creator\": \"U051J3RQL\",\n            \"is_archived\": false,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [\n                \"U051J3RQL\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 1\n        },\n        {\n            \"id\": \"C1UP563HV\",\n            \"name\": \"slash-command-testing\",\n            \"is_channel\": true,\n            \"created\": 1469441678,\n            \"creator\": \"U0HJ49KJ4\",\n            \"is_archived\": true,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 0\n        },\n        {\n            \"id\": \"C25R7RHAS\",\n            \"name\": \"u23ezh2tw\",\n            \"is_channel\": true,\n            \"created\": 1472447033,\n            \"creator\": \"U06NVGLPQ\",\n            \"is_archived\": false,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 0\n        },\n        {\n            \"id\": \"C0CQC7U8M\",\n            \"name\": \"whiteboard-devs\",\n            \"is_channel\": true,\n            \"created\": 1445319858,\n            \"creator\": \"U051HV8JE\",\n            \"is_archived\": false,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 0\n        },\n        {\n            \"id\": \"C0S4YTVAB\",\n            \"name\": \"whiteboard-undoredo\",\n            \"is_channel\": true,\n            \"created\": 1457697919,\n            \"creator\": \"U051HV8JE\",\n            \"is_archived\": false,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [\n                \"U051HV8JE\",\n                \"U051J13Q8\",\n                \"U051J2RTW\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 3\n        },\n        {\n            \"id\": \"C0L94A894\",\n            \"name\": \"wrapper-designer\",\n            \"is_channel\": true,\n            \"created\": 1454583500,\n            \"creator\": \"U051HV8JE\",\n            \"is_archived\": false,\n            \"is_general\": false,\n            \"is_member\": false,\n            \"members\": [\n                \"U04KUAFSH\",\n                \"U051HV8JE\",\n                \"U051J0GN4\",\n                \"U0525NC63\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"num_members\": 4\n        }\n    ]\n}\n";
            }
        }
        public string GroupDetailsResponseText
        {
            get
            {
                return "{\n    \"ok\": true,\n    \"groups\": [\n        {\n            \"id\": \"G0HJ0SB9R\",\n            \"name\": \"bbit-training-group\",\n            \"is_group\": true,\n            \"created\": 1451628546,\n            \"creator\": \"U04KUAFSH\",\n            \"is_archived\": true,\n            \"is_mpim\": false,\n            \"members\": [\n                \"U04KUAFSH\",\n                \"U051J2RTW\",\n                \"U051J5H4G\",\n                \"U051ZTH8J\",\n                \"U0525M2P1\",\n                \"U0HJ33E49\",\n                \"U0HJ34YU9\",\n                \"U0HJ48P4Y\",\n                \"U0HJ49KJ4\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            }\n        },\n        {\n            \"id\": \"G0RF4FM09\",\n            \"name\": \"creativecell\",\n            \"is_group\": true,\n            \"created\": 1457515586,\n            \"creator\": \"U051J3RQL\",\n            \"is_archived\": false,\n            \"is_mpim\": false,\n            \"members\": [\n                \"U04K6NL6A\",\n                \"U04KUAFSH\",\n                \"U051HV7DA\",\n                \"U051HV8JE\",\n                \"U051J3RQL\",\n                \"U051ZTH8J\",\n                \"U05256USX\",\n                \"U0525LCJR\",\n                \"U0525LFLT\",\n                \"U0525MNTH\",\n                \"U0525NTFZ\",\n                \"U0525PKAK\",\n                \"U05263AL9\",\n                \"U06NVGLPQ\",\n                \"U08GCPE0J\",\n                \"U09UNVC20\",\n                \"U0FQH9DB9\",\n                \"U0H7R5MPS\",\n                \"U0HJ34YU9\",\n                \"U0HJ49KJ4\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            }\n        },\n        {\n            \"id\": \"G0MH2SCTG\",\n            \"name\": \"crozi-internal-beta\",\n            \"is_group\": true,\n            \"created\": 1455627169,\n            \"creator\": \"U04K6NL6A\",\n            \"is_archived\": false,\n            \"is_mpim\": false,\n            \"members\": [\n                \"U04K6NL6A\",\n                \"U051HV8JE\",\n                \"U051J13Q8\",\n                \"U051J2QKG\",\n                \"U051J2RTW\",\n                \"U051J3RQL\",\n                \"U051J5H4G\",\n                \"U0525LDMF\",\n                \"U0525M2P1\",\n                \"U0525MNTH\",\n                \"U0525PKAK\",\n                \"U0525QHQ1\",\n                \"U05263AL9\",\n                \"U09UNVC20\",\n                \"U0FQH9DB9\",\n                \"U0HJ33E49\",\n                \"U0HJ34YU9\",\n                \"U0HJ48P4Y\",\n                \"U0HJ49KJ4\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            }\n        },\n        {\n            \"id\": \"G0L2PBH9N\",\n            \"name\": \"fun-day-2016\",\n            \"is_group\": true,\n            \"created\": 1454416384,\n            \"creator\": \"U0HJ49KJ4\",\n            \"is_archived\": false,\n            \"is_mpim\": false,\n            \"members\": [\n                \"U051HV8JE\",\n                \"U051J0GN4\",\n                \"U0525LDMF\",\n                \"U082PJG72\",\n                \"U0HJ48P4Y\",\n                \"U0HJ49KJ4\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            }\n        },\n        {\n            \"id\": \"G0FG737TL\",\n            \"name\": \"house-martell\",\n            \"is_group\": true,\n            \"created\": 1448869272,\n            \"creator\": \"U082PJG72\",\n            \"is_archived\": false,\n            \"is_mpim\": false,\n            \"members\": [\n                \"U051J2RTW\",\n                \"U0525LDMF\",\n                \"U0525MFND\",\n                \"U0545BH7Q\",\n                \"U082PJG72\",\n                \"U08GCPE0J\",\n                \"U0HJ49KJ4\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"discussion about olympic 2016\",\n                \"creator\": \"U082PJG72\",\n                \"last_set\": 1448869275\n            }\n        },\n        {\n            \"id\": \"G0Q387150\",\n            \"name\": \"mostcrowdedcube\",\n            \"is_group\": true,\n            \"created\": 1457006375,\n            \"creator\": \"U0HJ49KJ4\",\n            \"is_archived\": false,\n            \"is_mpim\": false,\n            \"members\": [\n                \"U0FQH9DB9\",\n                \"U0HJ33E49\",\n                \"U0HJ34YU9\",\n                \"U0HJ48P4Y\",\n                \"U0HJ49KJ4\",\n                \"U0J4V5EEQ\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            }\n        },\n        {\n            \"id\": \"G20P4DY8N\",\n            \"name\": \"mpdm-ankit--gourav--rajdeep--siddhartha--ronak.shah-1\",\n            \"is_group\": true,\n            \"created\": 1470987087,\n            \"creator\": \"U0HJ49KJ4\",\n            \"is_archived\": false,\n            \"is_mpim\": true,\n            \"members\": [\n                \"U0HJ49KJ4\",\n                \"U0526N37K\",\n                \"U0HJ48P4Y\",\n                \"U1M8GSPPC\",\n                \"U0HJ34YU9\"\n            ],\n            \"topic\": {\n                \"value\": \"Group messaging\",\n                \"creator\": \"U0HJ49KJ4\",\n                \"last_set\": 1470987087\n            },\n            \"purpose\": {\n                \"value\": \"Group messaging with: @ankit @gourav @rajdeep @siddhartha @ronak.shah\",\n                \"creator\": \"U0HJ49KJ4\",\n                \"last_set\": 1470987087\n            }\n        },\n        {\n            \"id\": \"G2006M214\",\n            \"name\": \"mpdm-ankit--rajdeep--siddhartha--ronak.shah-1\",\n            \"is_group\": true,\n            \"created\": 1470822352,\n            \"creator\": \"U0526N37K\",\n            \"is_archived\": false,\n            \"is_mpim\": true,\n            \"members\": [\n                \"U0526N37K\",\n                \"U0HJ48P4Y\",\n                \"U1M8GSPPC\",\n                \"U0HJ49KJ4\"\n            ],\n            \"topic\": {\n                \"value\": \"Group messaging\",\n                \"creator\": \"U0526N37K\",\n                \"last_set\": 1470822352\n            },\n            \"purpose\": {\n                \"value\": \"Group messaging with: @ankit @rajdeep @siddhartha @ronak.shah\",\n                \"creator\": \"U0526N37K\",\n                \"last_set\": 1470822352\n            }\n        },\n        {\n            \"id\": \"G1TPX59C0\",\n            \"name\": \"mpdm-gourav--rajdeep--siddhartha-1\",\n            \"is_group\": true,\n            \"created\": 1469092626,\n            \"creator\": \"U0HJ48P4Y\",\n            \"is_archived\": false,\n            \"is_mpim\": true,\n            \"members\": [\n                \"U0HJ48P4Y\",\n                \"U0HJ49KJ4\",\n                \"U0HJ34YU9\"\n            ],\n            \"topic\": {\n                \"value\": \"Group messaging\",\n                \"creator\": \"U0HJ48P4Y\",\n                \"last_set\": 1469092626\n            },\n            \"purpose\": {\n                \"value\": \"Group messaging with: @gourav @rajdeep @siddhartha\",\n                \"creator\": \"U0HJ48P4Y\",\n                \"last_set\": 1469092626\n            }\n        },\n        {\n            \"id\": \"G0HLD1MBQ\",\n            \"name\": \"mpdm-parth--rahul--gourav--rajdeep--siddhartha-1\",\n            \"is_group\": true,\n            \"created\": 1451912292,\n            \"creator\": \"U0FQH9DB9\",\n            \"is_archived\": false,\n            \"is_mpim\": true,\n            \"members\": [\n                \"U0FQH9DB9\",\n                \"U0HJ33E49\",\n                \"U0HJ48P4Y\",\n                \"U0HJ49KJ4\",\n                \"U0HJ34YU9\"\n            ],\n            \"topic\": {\n                \"value\": \"Group messaging\",\n                \"creator\": \"U0FQH9DB9\",\n                \"last_set\": 1451912292\n            },\n            \"purpose\": {\n                \"value\": \"Group messaging with: @parth @rahul @gourav @rajdeep @siddhartha\",\n                \"creator\": \"U0FQH9DB9\",\n                \"last_set\": 1451912292\n            }\n        },\n        {\n            \"id\": \"G0J2SMS4B\",\n            \"name\": \"mpdm-rahul--gourav--rajdeep--siddhartha-1\",\n            \"is_group\": true,\n            \"created\": 1452315477,\n            \"creator\": \"U0HJ48P4Y\",\n            \"is_archived\": false,\n            \"is_mpim\": true,\n            \"members\": [\n                \"U0HJ48P4Y\",\n                \"U0HJ49KJ4\",\n                \"U0HJ33E49\",\n                \"U0HJ34YU9\"\n            ],\n            \"topic\": {\n                \"value\": \"Group messaging\",\n                \"creator\": \"U0HJ48P4Y\",\n                \"last_set\": 1452315477\n            },\n            \"purpose\": {\n                \"value\": \"Group messaging with: @rahul @gourav @rajdeep @siddhartha\",\n                \"creator\": \"U0HJ48P4Y\",\n                \"last_set\": 1452315477\n            }\n        },\n        {\n            \"id\": \"G24PWMYJ2\",\n            \"name\": \"mpdm-roshni--julie--siddhartha-1\",\n            \"is_group\": true,\n            \"created\": 1472105114,\n            \"creator\": \"U0HJ49KJ4\",\n            \"is_archived\": false,\n            \"is_mpim\": true,\n            \"members\": [\n                \"U0HJ49KJ4\",\n                \"U06NVGLPQ\",\n                \"U0525LCJR\"\n            ],\n            \"topic\": {\n                \"value\": \"Group messaging\",\n                \"creator\": \"U0HJ49KJ4\",\n                \"last_set\": 1472105114\n            },\n            \"purpose\": {\n                \"value\": \"Group messaging with: @roshni @julie @siddhartha\",\n                \"creator\": \"U0HJ49KJ4\",\n                \"last_set\": 1472105114\n            }\n        },\n        {\n            \"id\": \"G1TQRH34H\",\n            \"name\": \"promact-erp-dev\",\n            \"is_group\": true,\n            \"created\": 1469096642,\n            \"creator\": \"U0525LCJR\",\n            \"is_archived\": false,\n            \"is_mpim\": false,\n            \"members\": [\n                \"U0525LCJR\",\n                \"U0526N37K\",\n                \"U06NVGLPQ\",\n                \"U0HJ34YU9\",\n                \"U0HJ48P4Y\",\n                \"U0HJ49KJ4\",\n                \"U1M8GSPPC\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"only developers\",\n                \"creator\": \"U0525LCJR\",\n                \"last_set\": 1469096645\n            }\n        },\n        {\n            \"id\": \"G05207LJY\",\n            \"name\": \"promact-team\",\n            \"is_group\": true,\n            \"created\": 1432703647,\n            \"creator\": \"U05256USX\",\n            \"is_archived\": false,\n            \"is_mpim\": false,\n            \"members\": [\n                \"U04K6NL6A\",\n                \"U04KUAFSH\",\n                \"U051HV7DA\",\n                \"U051HV8JE\",\n                \"U051HV8N8\",\n                \"U051HV9KY\",\n                \"U051J094A\",\n                \"U051J0GN4\",\n                \"U051J0M2A\",\n                \"U051J13Q8\",\n                \"U051J17PA\",\n                \"U051J2QKG\",\n                \"U051J2RTW\",\n                \"U051J3RQL\",\n                \"U051J44DE\",\n                \"U051J5H4G\",\n                \"U051JE2JW\",\n                \"U051KDCAL\",\n                \"U051ZTH8J\",\n                \"U05256USX\",\n                \"U0525LCJR\",\n                \"U0525LDMF\",\n                \"U0525LF29\",\n                \"U0525LFLT\",\n                \"U0525LH1B\",\n                \"U0525LLBZ\",\n                \"U0525LW55\",\n                \"U0525M2P1\",\n                \"U0525MFND\",\n                \"U0525MNTH\",\n                \"U0525MXL3\",\n                \"U0525N6FF\",\n                \"U0525N6NB\",\n                \"U0525NC63\",\n                \"U0525NTFZ\",\n                \"U0525PKAK\",\n                \"U0525QHQ1\",\n                \"U05263AL9\",\n                \"U0526JFLM\",\n                \"U0526N37K\",\n                \"U0545BH7Q\",\n                \"U06CU2VB9\",\n                \"U06NVGLPQ\",\n                \"U082PJG72\",\n                \"U08GCPE0J\",\n                \"U08KM481G\",\n                \"U09G8CUJ3\",\n                \"U09UNVC20\",\n                \"U0FQH9DB9\",\n                \"U0H7R5MPS\",\n                \"U0HJ33E49\",\n                \"U0HJ34YU9\",\n                \"U0HJ48P4Y\",\n                \"U0HJ49KJ4\",\n                \"U0J4V5EEQ\",\n                \"U0U36HKV1\",\n                \"U0VR8DA0Y\",\n                \"U0ZH5UYSJ\",\n                \"U1C74JGTU\",\n                \"U1D6AU01F\",\n                \"U1JBA2Z5G\",\n                \"U1M83P1U0\",\n                \"U1M8GSPPC\",\n                \"U1MVA0H3M\",\n                \"U1MVBJE2X\",\n                \"U1N1CA4C8\",\n                \"U1N1P8X89\",\n                \"U1WTH2LPM\",\n                \"U1Z3M1VNJ\",\n                \"U23EZH2TW\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            }\n        },\n        {\n            \"id\": \"G1T01U11P\",\n            \"name\": \"sci-oauth-dev\",\n            \"is_group\": true,\n            \"created\": 1468914520,\n            \"creator\": \"U0525LCJR\",\n            \"is_archived\": false,\n            \"is_mpim\": false,\n            \"members\": [\n                \"U04KUAFSH\",\n                \"U0525LCJR\",\n                \"U05263AL9\",\n                \"U0526N37K\",\n                \"U06NVGLPQ\",\n                \"U0H7R5MPS\",\n                \"U0HJ34YU9\",\n                \"U0HJ49KJ4\",\n                \"U1M8GSPPC\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"Team for Slack Custom Integrations + Making OAuth server\",\n                \"creator\": \"U0525LCJR\",\n                \"last_set\": 1468914522\n            }\n        },\n        {\n            \"id\": \"G2755U4P6\",\n            \"name\": \"slack-automation-mvc\",\n            \"is_group\": true,\n            \"created\": 1472709560,\n            \"creator\": \"U04KUAFSH\",\n            \"is_archived\": false,\n            \"is_mpim\": false,\n            \"members\": [\n                \"U04KUAFSH\",\n                \"U0525LCJR\",\n                \"U05263AL9\",\n                \"U0526N37K\",\n                \"U06NVGLPQ\",\n                \"U0H7R5MPS\",\n                \"U0HJ34YU9\",\n                \"U0HJ49KJ4\",\n                \"U1M8GSPPC\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"This will show build status and git commits. not meant for discussing anything.\",\n                \"creator\": \"U04KUAFSH\",\n                \"last_set\": 1472709563\n            }\n        },\n        {\n            \"id\": \"G25RU420H\",\n            \"name\": \"slack-integration\",\n            \"is_group\": true,\n            \"created\": 1472451083,\n            \"creator\": \"U0526N37K\",\n            \"is_archived\": false,\n            \"is_mpim\": false,\n            \"members\": [\n                \"U05263AL9\",\n                \"U0526N37K\",\n                \"U06NVGLPQ\",\n                \"U0HJ34YU9\",\n                \"U0HJ49KJ4\",\n                \"U1M8GSPPC\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            }\n        },\n        {\n            \"id\": \"G1UPC4057\",\n            \"name\": \"slash-command\",\n            \"is_group\": true,\n            \"created\": 1469441963,\n            \"creator\": \"U0HJ49KJ4\",\n            \"is_archived\": false,\n            \"is_mpim\": false,\n            \"members\": [\n                \"U0HJ49KJ4\",\n                \"U24QNPAH0\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            }\n        }\n    ]\n}\n";
            }
        }
        public string BasicUserDetailsResponseText
        {
            get
            {
                return "{\n    \"ok\": true,\n    \"groups\": [\n        {\n            \"id\": \"G0HJ0SB9R\",\n            \"name\": \"bbit-training-group\",\n            \"is_group\": true,\n            \"created\": 1451628546,\n            \"creator\": \"U04KUAFSH\",\n            \"is_archived\": true,\n            \"is_mpim\": false,\n            \"members\": [\n                \"U04KUAFSH\",\n                \"U051J2RTW\",\n                \"U051J5H4G\",\n                \"U051ZTH8J\",\n                \"U0525M2P1\",\n                \"U0HJ33E49\",\n                \"U0HJ34YU9\",\n                \"U0HJ48P4Y\",\n                \"U0HJ49KJ4\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            }\n        },\n        {\n            \"id\": \"G0RF4FM09\",\n            \"name\": \"creativecell\",\n            \"is_group\": true,\n            \"created\": 1457515586,\n            \"creator\": \"U051J3RQL\",\n            \"is_archived\": false,\n            \"is_mpim\": false,\n            \"members\": [\n                \"U04K6NL6A\",\n                \"U04KUAFSH\",\n                \"U051HV7DA\",\n                \"U051HV8JE\",\n                \"U051J3RQL\",\n                \"U051ZTH8J\",\n                \"U05256USX\",\n                \"U0525LCJR\",\n                \"U0525LFLT\",\n                \"U0525MNTH\",\n                \"U0525NTFZ\",\n                \"U0525PKAK\",\n                \"U05263AL9\",\n                \"U06NVGLPQ\",\n                \"U08GCPE0J\",\n                \"U09UNVC20\",\n                \"U0FQH9DB9\",\n                \"U0H7R5MPS\",\n                \"U0HJ34YU9\",\n                \"U0HJ49KJ4\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            }\n        },\n        {\n            \"id\": \"G0MH2SCTG\",\n            \"name\": \"crozi-internal-beta\",\n            \"is_group\": true,\n            \"created\": 1455627169,\n            \"creator\": \"U04K6NL6A\",\n            \"is_archived\": false,\n            \"is_mpim\": false,\n            \"members\": [\n                \"U04K6NL6A\",\n                \"U051HV8JE\",\n                \"U051J13Q8\",\n                \"U051J2QKG\",\n                \"U051J2RTW\",\n                \"U051J3RQL\",\n                \"U051J5H4G\",\n                \"U0525LDMF\",\n                \"U0525M2P1\",\n                \"U0525MNTH\",\n                \"U0525PKAK\",\n                \"U0525QHQ1\",\n                \"U05263AL9\",\n                \"U09UNVC20\",\n                \"U0FQH9DB9\",\n                \"U0HJ33E49\",\n                \"U0HJ34YU9\",\n                \"U0HJ48P4Y\",\n                \"U0HJ49KJ4\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            }\n        },\n        {\n            \"id\": \"G0L2PBH9N\",\n            \"name\": \"fun-day-2016\",\n            \"is_group\": true,\n            \"created\": 1454416384,\n            \"creator\": \"U0HJ49KJ4\",\n            \"is_archived\": false,\n            \"is_mpim\": false,\n            \"members\": [\n                \"U051HV8JE\",\n                \"U051J0GN4\",\n                \"U0525LDMF\",\n                \"U082PJG72\",\n                \"U0HJ48P4Y\",\n                \"U0HJ49KJ4\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            }\n        },\n        {\n            \"id\": \"G0FG737TL\",\n            \"name\": \"house-martell\",\n            \"is_group\": true,\n            \"created\": 1448869272,\n            \"creator\": \"U082PJG72\",\n            \"is_archived\": false,\n            \"is_mpim\": false,\n            \"members\": [\n                \"U051J2RTW\",\n                \"U0525LDMF\",\n                \"U0525MFND\",\n                \"U0545BH7Q\",\n                \"U082PJG72\",\n                \"U08GCPE0J\",\n                \"U0HJ49KJ4\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"discussion about olympic 2016\",\n                \"creator\": \"U082PJG72\",\n                \"last_set\": 1448869275\n            }\n        },\n        {\n            \"id\": \"G0Q387150\",\n            \"name\": \"mostcrowdedcube\",\n            \"is_group\": true,\n            \"created\": 1457006375,\n            \"creator\": \"U0HJ49KJ4\",\n            \"is_archived\": false,\n            \"is_mpim\": false,\n            \"members\": [\n                \"U0FQH9DB9\",\n                \"U0HJ33E49\",\n                \"U0HJ34YU9\",\n                \"U0HJ48P4Y\",\n                \"U0HJ49KJ4\",\n                \"U0J4V5EEQ\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            }\n        },\n        {\n            \"id\": \"G20P4DY8N\",\n            \"name\": \"mpdm-ankit--gourav--rajdeep--siddhartha--ronak.shah-1\",\n            \"is_group\": true,\n            \"created\": 1470987087,\n            \"creator\": \"U0HJ49KJ4\",\n            \"is_archived\": false,\n            \"is_mpim\": true,\n            \"members\": [\n                \"U0HJ49KJ4\",\n                \"U0526N37K\",\n                \"U0HJ48P4Y\",\n                \"U1M8GSPPC\",\n                \"U0HJ34YU9\"\n            ],\n            \"topic\": {\n                \"value\": \"Group messaging\",\n                \"creator\": \"U0HJ49KJ4\",\n                \"last_set\": 1470987087\n            },\n            \"purpose\": {\n                \"value\": \"Group messaging with: @ankit @gourav @rajdeep @siddhartha @ronak.shah\",\n                \"creator\": \"U0HJ49KJ4\",\n                \"last_set\": 1470987087\n            }\n        },\n        {\n            \"id\": \"G2006M214\",\n            \"name\": \"mpdm-ankit--rajdeep--siddhartha--ronak.shah-1\",\n            \"is_group\": true,\n            \"created\": 1470822352,\n            \"creator\": \"U0526N37K\",\n            \"is_archived\": false,\n            \"is_mpim\": true,\n            \"members\": [\n                \"U0526N37K\",\n                \"U0HJ48P4Y\",\n                \"U1M8GSPPC\",\n                \"U0HJ49KJ4\"\n            ],\n            \"topic\": {\n                \"value\": \"Group messaging\",\n                \"creator\": \"U0526N37K\",\n                \"last_set\": 1470822352\n            },\n            \"purpose\": {\n                \"value\": \"Group messaging with: @ankit @rajdeep @siddhartha @ronak.shah\",\n                \"creator\": \"U0526N37K\",\n                \"last_set\": 1470822352\n            }\n        },\n        {\n            \"id\": \"G1TPX59C0\",\n            \"name\": \"mpdm-gourav--rajdeep--siddhartha-1\",\n            \"is_group\": true,\n            \"created\": 1469092626,\n            \"creator\": \"U0HJ48P4Y\",\n            \"is_archived\": false,\n            \"is_mpim\": true,\n            \"members\": [\n                \"U0HJ48P4Y\",\n                \"U0HJ49KJ4\",\n                \"U0HJ34YU9\"\n            ],\n            \"topic\": {\n                \"value\": \"Group messaging\",\n                \"creator\": \"U0HJ48P4Y\",\n                \"last_set\": 1469092626\n            },\n            \"purpose\": {\n                \"value\": \"Group messaging with: @gourav @rajdeep @siddhartha\",\n                \"creator\": \"U0HJ48P4Y\",\n                \"last_set\": 1469092626\n            }\n        },\n        {\n            \"id\": \"G0HLD1MBQ\",\n            \"name\": \"mpdm-parth--rahul--gourav--rajdeep--siddhartha-1\",\n            \"is_group\": true,\n            \"created\": 1451912292,\n            \"creator\": \"U0FQH9DB9\",\n            \"is_archived\": false,\n            \"is_mpim\": true,\n            \"members\": [\n                \"U0FQH9DB9\",\n                \"U0HJ33E49\",\n                \"U0HJ48P4Y\",\n                \"U0HJ49KJ4\",\n                \"U0HJ34YU9\"\n            ],\n            \"topic\": {\n                \"value\": \"Group messaging\",\n                \"creator\": \"U0FQH9DB9\",\n                \"last_set\": 1451912292\n            },\n            \"purpose\": {\n                \"value\": \"Group messaging with: @parth @rahul @gourav @rajdeep @siddhartha\",\n                \"creator\": \"U0FQH9DB9\",\n                \"last_set\": 1451912292\n            }\n        },\n        {\n            \"id\": \"G0J2SMS4B\",\n            \"name\": \"mpdm-rahul--gourav--rajdeep--siddhartha-1\",\n            \"is_group\": true,\n            \"created\": 1452315477,\n            \"creator\": \"U0HJ48P4Y\",\n            \"is_archived\": false,\n            \"is_mpim\": true,\n            \"members\": [\n                \"U0HJ48P4Y\",\n                \"U0HJ49KJ4\",\n                \"U0HJ33E49\",\n                \"U0HJ34YU9\"\n            ],\n            \"topic\": {\n                \"value\": \"Group messaging\",\n                \"creator\": \"U0HJ48P4Y\",\n                \"last_set\": 1452315477\n            },\n            \"purpose\": {\n                \"value\": \"Group messaging with: @rahul @gourav @rajdeep @siddhartha\",\n                \"creator\": \"U0HJ48P4Y\",\n                \"last_set\": 1452315477\n            }\n        },\n        {\n            \"id\": \"G24PWMYJ2\",\n            \"name\": \"mpdm-roshni--julie--siddhartha-1\",\n            \"is_group\": true,\n            \"created\": 1472105114,\n            \"creator\": \"U0HJ49KJ4\",\n            \"is_archived\": false,\n            \"is_mpim\": true,\n            \"members\": [\n                \"U0HJ49KJ4\",\n                \"U06NVGLPQ\",\n                \"U0525LCJR\"\n            ],\n            \"topic\": {\n                \"value\": \"Group messaging\",\n                \"creator\": \"U0HJ49KJ4\",\n                \"last_set\": 1472105114\n            },\n            \"purpose\": {\n                \"value\": \"Group messaging with: @roshni @julie @siddhartha\",\n                \"creator\": \"U0HJ49KJ4\",\n                \"last_set\": 1472105114\n            }\n        },\n        {\n            \"id\": \"G1TQRH34H\",\n            \"name\": \"promact-erp-dev\",\n            \"is_group\": true,\n            \"created\": 1469096642,\n            \"creator\": \"U0525LCJR\",\n            \"is_archived\": false,\n            \"is_mpim\": false,\n            \"members\": [\n                \"U0525LCJR\",\n                \"U0526N37K\",\n                \"U06NVGLPQ\",\n                \"U0HJ34YU9\",\n                \"U0HJ48P4Y\",\n                \"U0HJ49KJ4\",\n                \"U1M8GSPPC\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"only developers\",\n                \"creator\": \"U0525LCJR\",\n                \"last_set\": 1469096645\n            }\n        },\n        {\n            \"id\": \"G05207LJY\",\n            \"name\": \"promact-team\",\n            \"is_group\": true,\n            \"created\": 1432703647,\n            \"creator\": \"U05256USX\",\n            \"is_archived\": false,\n            \"is_mpim\": false,\n            \"members\": [\n                \"U04K6NL6A\",\n                \"U04KUAFSH\",\n                \"U051HV7DA\",\n                \"U051HV8JE\",\n                \"U051HV8N8\",\n                \"U051HV9KY\",\n                \"U051J094A\",\n                \"U051J0GN4\",\n                \"U051J0M2A\",\n                \"U051J13Q8\",\n                \"U051J17PA\",\n                \"U051J2QKG\",\n                \"U051J2RTW\",\n                \"U051J3RQL\",\n                \"U051J44DE\",\n                \"U051J5H4G\",\n                \"U051JE2JW\",\n                \"U051KDCAL\",\n                \"U051ZTH8J\",\n                \"U05256USX\",\n                \"U0525LCJR\",\n                \"U0525LDMF\",\n                \"U0525LF29\",\n                \"U0525LFLT\",\n                \"U0525LH1B\",\n                \"U0525LLBZ\",\n                \"U0525LW55\",\n                \"U0525M2P1\",\n                \"U0525MFND\",\n                \"U0525MNTH\",\n                \"U0525MXL3\",\n                \"U0525N6FF\",\n                \"U0525N6NB\",\n                \"U0525NC63\",\n                \"U0525NTFZ\",\n                \"U0525PKAK\",\n                \"U0525QHQ1\",\n                \"U05263AL9\",\n                \"U0526JFLM\",\n                \"U0526N37K\",\n                \"U0545BH7Q\",\n                \"U06CU2VB9\",\n                \"U06NVGLPQ\",\n                \"U082PJG72\",\n                \"U08GCPE0J\",\n                \"U08KM481G\",\n                \"U09G8CUJ3\",\n                \"U09UNVC20\",\n                \"U0FQH9DB9\",\n                \"U0H7R5MPS\",\n                \"U0HJ33E49\",\n                \"U0HJ34YU9\",\n                \"U0HJ48P4Y\",\n                \"U0HJ49KJ4\",\n                \"U0J4V5EEQ\",\n                \"U0U36HKV1\",\n                \"U0VR8DA0Y\",\n                \"U0ZH5UYSJ\",\n                \"U1C74JGTU\",\n                \"U1D6AU01F\",\n                \"U1JBA2Z5G\",\n                \"U1M83P1U0\",\n                \"U1M8GSPPC\",\n                \"U1MVA0H3M\",\n                \"U1MVBJE2X\",\n                \"U1N1CA4C8\",\n                \"U1N1P8X89\",\n                \"U1WTH2LPM\",\n                \"U1Z3M1VNJ\",\n                \"U23EZH2TW\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            }\n        },\n        {\n            \"id\": \"G1T01U11P\",\n            \"name\": \"sci-oauth-dev\",\n            \"is_group\": true,\n            \"created\": 1468914520,\n            \"creator\": \"U0525LCJR\",\n            \"is_archived\": false,\n            \"is_mpim\": false,\n            \"members\": [\n                \"U04KUAFSH\",\n                \"U0525LCJR\",\n                \"U05263AL9\",\n                \"U0526N37K\",\n                \"U06NVGLPQ\",\n                \"U0H7R5MPS\",\n                \"U0HJ34YU9\",\n                \"U0HJ49KJ4\",\n                \"U1M8GSPPC\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"Team for Slack Custom Integrations + Making OAuth server\",\n                \"creator\": \"U0525LCJR\",\n                \"last_set\": 1468914522\n            }\n        },\n        {\n            \"id\": \"G2755U4P6\",\n            \"name\": \"slack-automation-mvc\",\n            \"is_group\": true,\n            \"created\": 1472709560,\n            \"creator\": \"U04KUAFSH\",\n            \"is_archived\": false,\n            \"is_mpim\": false,\n            \"members\": [\n                \"U04KUAFSH\",\n                \"U0525LCJR\",\n                \"U05263AL9\",\n                \"U0526N37K\",\n                \"U06NVGLPQ\",\n                \"U0H7R5MPS\",\n                \"U0HJ34YU9\",\n                \"U0HJ49KJ4\",\n                \"U1M8GSPPC\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"This will show build status and git commits. not meant for discussing anything.\",\n                \"creator\": \"U04KUAFSH\",\n                \"last_set\": 1472709563\n            }\n        },\n        {\n            \"id\": \"G25RU420H\",\n            \"name\": \"slack-integration\",\n            \"is_group\": true,\n            \"created\": 1472451083,\n            \"creator\": \"U0526N37K\",\n            \"is_archived\": false,\n            \"is_mpim\": false,\n            \"members\": [\n                \"U05263AL9\",\n                \"U0526N37K\",\n                \"U06NVGLPQ\",\n                \"U0HJ34YU9\",\n                \"U0HJ49KJ4\",\n                \"U1M8GSPPC\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            }\n        },\n        {\n            \"id\": \"G1UPC4057\",\n            \"name\": \"slash-command\",\n            \"is_group\": true,\n            \"created\": 1469441963,\n            \"creator\": \"U0HJ49KJ4\",\n            \"is_archived\": false,\n            \"is_mpim\": false,\n            \"members\": [\n                \"U0HJ49KJ4\",\n                \"U24QNPAH0\"\n            ],\n            \"topic\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            },\n            \"purpose\": {\n                \"value\": \"\",\n                \"creator\": \"\",\n                \"last_set\": 0\n            }\n        }\n    ]\n}\n";
            }
        }
        public string LeaveListTestForOwn
        {
            get
            {
                return "list";
            }
        }
        public string WrongLeaveCancelCommandForTest
        {
            get
            {
                return "cancel aaa";
            }
        }
        public string LeaveStatusTestForOwn
        {
            get
            {
                return "status";
            }
        }
        public string SecondQuestionForTest
        {
            get
            {
                return "How many hours did you spend on this task?";
            }
        }
        public string ThirdQuestionForTest
        {
            get
            {
                return "What is the Status of this task?";
            }
        }
        public string ForthQuestionForTest
        {
            get
            {
                return "Mention if any Comment/Roadblock.";
            }
        }
        public string FifthQuestionForTest
        {
            get
            {
                return "Do you want to send your task mail?";
            }
        }
        public string SixthQuestionForTest
        {
            get
            {
                return "Are you sure to send mail? After sending email you won't be able to add any tak for today.";
            }
        }
        public string SeventhQuestionForTest
        {
            get
            {
                return "Task Mail Complete";
            }
        }
        public string YouAreNotInExistInOAuthServer
        {
            get
            {
                return string.Format("Either you are not in Promact OAuth or you haven't logged in with Promact OAuth."+
                    " Click here {0}", AppSettingUtil.PromactErpUrl);
            }
        }
        public string HourSpentForTest
        {
            get
            {
                return "4";
            }
        }
        public string StatusOfWorkForTest
        {
            get
            {
                return "completed";
            }
        }
        public string SendEmailYesForTest
        {
            get
            {
                return "yes";
            }
        }
        public string SendEmailNoForTest
        {
            get
            {
                return "no";
            }
        }
        public string NotTypeOfLeave
        {
            get
            {
                return "Please enter a valid Leave Type";
            }
        }
        public string DateFormatErrorMessage
        {
            get
            {
                return "Date Format Error. Date format should be of your culture. Format :- {0}";
            }
        }
        public string ErrorWhileSendingEmail
        {
            get
            {
                return "An error occur while sending email but your leave has been applied";
            }
        }
        public string UserIsAdmin
        {
            get
            {
                return "userIsAdmin/";
            }
        }
        public string LeaveDoesnotExist
        {
            get
            {
                return "Leave doesn't exist for this leave Id";
            }
        }
        public string AdminErrorMessageUpdateSickLeave
        {
            get
            {
                return "You are not authorize to update leave. Only Admin can update sick leave. You are not admin";
            }
        }
        public string True
        {
            get
            {
                return "true";
            }
        }
        public string LeaveStatusCommandForTest
        {
            get
            {
                return "status siddhartha";
            }
        }
        public string LeaveBalanceTestForOwn
        {
            get
            {
                return "balance";
            }
        }
        public string LeaveHelpTestForOwn
        {
            get
            {
                return "help";
            }
        }
        public string LeaveBalanceSickReplyTextForTest
        {
            get
            {
                return "You have taken 0 sick leave out of 5\r\nYou are left with 5 sick leave";
            }
        }
        public string SlashCommandTextSick
        {
            get
            {
                return string.Format("apply sl Testing {0}", DateTime.UtcNow.ToShortDateString());
            }
        }
        public string SlashCommandTextSickForUser
        {
            get
            {
                return string.Format("apply sl Testing {0} siddhartha", DateTime.UtcNow.ToShortDateString());
            }
        }
        public string NameForTest
        {
            get
            {
                return "roshni";
            }
        }
        public string RequestToEnterProperAction
        {
            get
            {
                return "Please enter a proper action for leave";
            }
        }
        public string SlashCommandTextErrorLeaveType
        {
            get
            {
                return string.Format("apply kl Testing {0} siddhartha", DateTime.UtcNow.ToShortDateString());
            }
        }
        public string SlashCommandTextErrorDateFormatSick
        {
            get
            {
                return "apply sl Testing 05/2016/12 siddhartha";
            }
        }
        public string SlashCommandTextErrorDateFormatCasual
        {
            get
            {
                return string.Format("apply cl Testing {0} 05/2016/12 05/2016/12", DateTime.UtcNow.ToShortDateString());
            }
        }
        public string SlashCommandTextCasual
        {
            get
            {
                return string.Format("apply cl Testing {0} {0} {1}", DateTime.UtcNow.ToShortDateString(), 
                    DateTime.UtcNow.AddDays(1).ToShortDateString());
            }
        }
        public string SlashCommandUpdate
        {
            get
            {
                return string.Format("update {0} {1} {2}", 1, DateTime.UtcNow.ToShortDateString(), 
                    DateTime.UtcNow.AddDays(1).ToShortDateString());
            }
        }
        public string SlashCommandUpdateDateError
        {
            get
            {
                return string.Format("update {0} 30/09/2016 {1}", 1, DateTime.UtcNow.AddDays(1).ToShortDateString());
            }
        }
        public string SlashCommandUpdateWrongId
        {
            get
            {
                return string.Format("update {0} {1} {2}", 10, DateTime.UtcNow.ToShortDateString(), 
                    DateTime.UtcNow.AddDays(1).ToShortDateString());
            }
        }
        public string SickLeaveDoesnotExist
        {
            get
            {
                return "Sick leave doesn't exist for this leave Id";
            }
        }
        public string UpdateEnterAValidLeaveId
        {
            get
            {
                return "Please enter a valid leave Id to update";
            }
        }
        public string SlashCommandUpdateInValidId
        {
            get
            {
                return string.Format("update abc {0}", DateTime.UtcNow.ToShortDateString());
            }
        }
        public string ErrorOfEmailServiceFailureTaskMail
        {
            get
            {
                return "An error occur while sending email";
            }
        }
        public string AdminErrorMessageApplySickLeave
        {
            get
            {
                return "You are not authorize to apply leave. Only Admin can apply sick leave for other."+
                    " You are not admin. Else ";
            }
        }

        #region String Constants for Test Cases

        public string UserNameForTest
        {
            get
            {
                return "apoorvapatel";
            }
        }
        public string GroupName
        {
            get
            {
                return "testbotgroup";
            }
        }
        public string AnswerStatement
        {
            get
            {
                return "Completed bot";
            }
        }

        public string ChannelIdForTest
        {
            get
            {
                return "231asd";
            }
        }
        public string PhoneForTest
        {
            get
            {
                return "5845155745451";
            }
        }
        public string ScrumQuestionForTest
        {
            get
            {
                return "What did you do yesterday?";
            }
        }
        public string ChannelNameForTest
        {
            get
            {
                return "testbotgroup";
            }
        }
        public string ProjectDetailsFromOauth
        {
            get
            {
                return "{\"id\":2,\"name\":\"testbotgroup\",\"slackChannelName\":\"testbotgroup\",\"isActive\":true,\"teamLeaderId\":\"5c84049f-f861-406d-b420-e1bf03c9e06e\",\"createdBy\":\"1bac6614-7a2b-42fa-9f18-b6a19d8e25fb\",\"createdDate\":null,\"updatedBy\":null,\"updatedDate\":null,\"teamLeader\":null,\"applicationUsers\":null}";
            }
        }

        public string InActiveProjectDetailsFromOauth
        {
            get
            {
                return "{\"id\":2,\"name\":\"testbotgroup\",\"slackChannelName\":\"testbotgroup\",\"isActive\":false,\"teamLeaderId\":\"5c84049f-f861-406d-b420-e1bf03c9e06e\",\"createdBy\":\"1bac6614-7a2b-42fa-9f18-b6a19d8e25fb\",\"createdDate\":null,\"updatedBy\":null,\"updatedDate\":null,\"teamLeader\":null,\"applicationUsers\":null}";
            }
        }

        public string EmployeeDetailsFromOauth
        {
            get
            {
                return "{\"Id\":\"577696c8-136f-4865-8328-09e7d48ac58d\",\"FirstName\":\"Apoorva\",\"LastName\":\"Promact\",\"IsActive\":true,\"Email\":\"apoorvapatel@promactinfo.com\",\"Password\":null,\"UserName\":\"apoorvapatel\",\"UniqueName\":\"Apoorva-apoorvapatel@promactinfo.com\"}";
            }
        }
        public string EmployeesListFromOauth
        {
            get
            {
                return "[{\"Id\":\"13b0f2ca-92f5-4713-a67e-37e50172e148f\",\"FirstName\":\"Apoorva\",\"LastName\":\"Promact\",\"IsActive\":true,\"Email\":\"apoorvapatel@promactinfo.com\",\"Password\":null,\"UserName\":\"apoorvapatel\",\"UniqueName\":\"Apoorva-apoorvapatel@promactinfo.com\"},{\"Id\":\"13b0f2ca-92f5-4713-a67e-37e50172e148\",\"FirstName\":\"Pranali\",\"LastName\":\"Promact\",\"IsActive\":true,\"Email\":\"pranali@promactinfo.com\",\"Password\":null,\"UserName\":\"pranali\",\"UniqueName\":\"Pranali-pranali@promactinfo.com\"}]";
            }
        }
        public string EmployeesListFromOauthOneEmployee
        {
            get
            {
                return "[{\"Id\":\"13b0f2ca-92f5-4713-a67e-37e50172e148\",\"FirstName\":\"Apoorva\",\"LastName\":\"Promact\",\"IsActive\":true,\"Email\":\"apoorvapatel@promactinfo.com\",\"Password\":null,\"UserName\":\"apoorvapatel\",\"UniqueName\":\"Apoorva-apoorvapatel@promactinfo.com\"}]";
            }
        }
        public string EmployeesListFromOauthThreeEmployees
        {
            get
            {
                return "[{\"Id\":\"13b0f2ca-92f5-4713-a67e-37e50172e148f\",\"FirstName\":\"Apoorva\",\"LastName\":\"Promact\",\"IsActive\":true,\"Email\":\"apoorvapatel@promactinfo.com\",\"Password\":null,\"UserName\":\"apoorvapatel\",\"UniqueName\":\"Apoorva-apoorvapatel@promactinfo.com\"},{\"Id\":\"aac59fbc-7835-4bd7-9080-6b6766302\",\"FirstName\":\"Nehal\",\"LastName\":\"Promact\",\"IsActive\":true,\"Email\":\"nehal@promactinfo.com\",\"Password\":null,\"UserName\":\"nehal\",\"UniqueName\":\"Pranali-pranali@promactinfo.com\"},{\"Id\":\"13b0f2ca-92f5-4713-a67e-37e50172e148\",\"FirstName\":\"Pranali\",\"LastName\":\"Promact\",\"IsActive\":true,\"Email\":\"pranali@promactinfo.com\",\"Password\":null,\"UserName\":\"pranali\",\"UniqueName\":\"Pranali-pranali@promactinfo.com\"}]";
            }
        }
        public string EmployeesListFromOauthThreeEmployeesInActive
        {
            get
            {
                return "[{\"Id\":\"13b0f2ca-92f5-4713-a67e-37e50172e148f\",\"FirstName\":\"Apoorva\",\"LastName\":\"Promact\",\"IsActive\":true,\"Email\":\"apoorvapatel@promactinfo.com\",\"Password\":null,\"UserName\":\"apoorvapatel\",\"UniqueName\":\"Apoorva-apoorvapatel@promactinfo.com\"},{\"Id\":\"aac59fbc-7835-4bd7-9080-6b6766302\",\"FirstName\":\"Nehal\",\"LastName\":\"Promact\",\"IsActive\":false,\"Email\":\"nehal@promactinfo.com\",\"Password\":null,\"UserName\":\"nehal\",\"UniqueName\":\"Pranali-pranali@promactinfo.com\"},{\"Id\":\"13b0f2ca-92f5-4713-a67e-37e50172e148\",\"FirstName\":\"Pranali\",\"LastName\":\"Promact\",\"IsActive\":true,\"Email\":\"pranali@promactinfo.com\",\"Password\":null,\"UserName\":\"pranali\",\"UniqueName\":\"Pranali-pranali@promactinfo.com\"}]";
            }
        }
        public string ThirdUserSlackUserId
        {
            get
            {
                return "13b0f2c2f5-4713-a67e-37e50172e148n";
            }
        }

        public string EmployeesListFromOauthInValid
        {
            get
            {
                return "[{\"Id\":\"13b0f2ca-92f5-4713-a67e-37e50172e148f\",\"FirstName\":\"Apoorva\",\"LastName\":\"Promact\",\"IsActive\":true,\"Email\":\"apoorvapatel@promactinfo.com\",\"Password\":null,\"UserName\":\"apoorvapatel\",\"UniqueName\":\"Apoorva-apoorvapatel@promactinfo.com\"},{\"Id\":\"13b0f2ca-92f5-4713-a67e-37e50172e148\",\"FirstName\":\"Pranali\",\"LastName\":\"Promact\",\"IsActive\":false,\"Email\":\"pranali@promactinfo.com\",\"Password\":null,\"UserName\":\"pranali\",\"UniqueName\":\"Pranali-pranali@promactinfo.com\"}]";
            }
        }
        public string EmployeesListInValid
        {
            get
            {
                return "[{\"Id\":\"13b0f2ca-92f5-4713-a67e-37e50172e148f\",\"FirstName\":\"Apoorva\",\"LastName\":\"Promact\",\"IsActive\":false,\"Email\":\"apoorvapatel@promactinfo.com\",\"Password\":null,\"UserName\":\"apoorvapatel\",\"UniqueName\":\"Apoorva-apoorvapatel@promactinfo.com\"},{\"Id\":\"13b0f2ca-92f5-4713-a67e-37e50172e148\",\"FirstName\":\"Pranali\",\"LastName\":\"Promact\",\"IsActive\":true,\"Email\":\"pranali@promactinfo.com\",\"Password\":null,\"UserName\":\"pranali\",\"UniqueName\":\"Pranali-pranali@promactinfo.com\"}]";
            }
        }
        public string InActiveEmployeesList
        {
            get
            {
                return "[{\"Id\":\"13b0f2ca-92f5-4713-a67e-37e50172e148f\",\"FirstName\":\"Apoorva\",\"LastName\":\"Promact\",\"IsActive\":false,\"Email\":\"apoorvapatel@promactinfo.com\",\"Password\":null,\"SlackUserName\":\"apoorvapatel\",\"SlackUserId\":\"13b0f2ca-92f5-4713-a67e-37e50172e148\",\"UserName\":\"apoorvapatel\",\"UniqueName\":\"Apoorva-apoorvapatel@promactinfo.com\"},{\"Id\":\"aac59fbc-7835-4bd7-9080-6b6766302080\",\"FirstName\":\"Pranali\",\"LastName\":\"Promact\",\"IsActive\":false,\"Email\":\"pranali@promactinfo.com\",\"Password\":null,\"SlackUserName\":\"pranali\",\"SlackUserId\":\"13b0f2ca-92f5-4713-a67e-37e50172e148f\",\"UserName\":\"pranali\",\"UniqueName\":\"Pranali-pranali@promactinfo.com\"}]";
            }
        }
        public string InValidOAuthUsers
        {
            get
            {
                return "[{\"Id\":\"13b0f2ca-92f5-4713-a67e-37e50172e148f\",\"FirstName\":\"Apoorva\",\"LastName\":\"Promact\",\"IsActive\":false,\"Email\":\"apoorvapatel@promactinfo.com\",\"Password\":null,\"SlackUserName\":\"apoorvapatel\",\"SlackUserId\":\"13b0f2ca-92f5-4713-a67e-37e50172e148\",\"UserName\":\"apoorvapatel\",\"UniqueName\":\"Apoorva-apoorvapatel@promactinfo.com\"},{\"Id\":\"aac59fbc-7835-4bd7-9080-6b6766302080\",\"FirstName\":\"Pranali\",\"LastName\":\"Promact\",\"IsActive\":false,\"Email\":\"pranali@promactinfo.com\",\"Password\":null,\"SlackUserName\":\"pranali\",\"SlackUserId\":\"13b0f2ca-92f5-4713-a67e-37e50172e148f\",\"UserName\":\"pranali\",\"UniqueName\":\"Pranali-pranali@promactinfo.com\"}]";
            }
        }
        public string OAuthUserDetails
        {
            get
            {
                return "{\"firstName\":\"pranali\",\"lastName\":\"Promact\",\"isActive\":true,\"numberOfCasualLeave\":0.0,\"numberOfSickLeave\":0.0,\"joiningDate\":\"0001-01-01T00:00:00\",\"slackUserName\":\"pranali\",\"slackUserId\":\"13b0f2ca-92f5-4713-a67e-37e50172e148f\",\"projects\":null,\"createdBy\":null,\"createdDateTime\":\"0001-01-01T00:00:00\",\"updatedBy\":null,\"updatedDateTime\":\"0001-01-01T00:00:00\",\"id\":\"aac59fbc-7835-4bd7-9080-6b6766302080\",\"userName\":\"pranali@promactinfo.com\",\"normalizedUserName\":null,\"email\":\"pranali@promactinfo.com\",\"normalizedEmail\":null,\"emailConfirmed\":false,\"passwordHash\":null,\"securityStamp\":null,\"concurrencyStamp\":\"a39b2cff-51e2-4f1d-bde9-096cefb17497\",\"phoneNumber\":null,\"phoneNumberConfirmed\":false,\"twoFactorEnabled\":false,\"lockoutEnd\":null,\"lockoutEnabled\":false,\"accessFailedCount\":0,\"roles\":[],\"Role\":\"Admin\",\"claims\":[],\"logins\":[]}";
            }
        }
        public string UnExpectedInActiveUser
        {
            get
            {
                return "<@pranali> is marked as In-active or not in OAuth server. Please contact your system administrator.\n \r\n<@apoorvapatel> What did you do yesterday?\r\nI am expecting <@apoorvapatel> to answer.";
            }
        }
        public string UserBySlackUserName
        {
            get
            {
                return "{\"Id\":\"fce9e5de-0c3e-410f-8602-068e211d5f4d\",\"FirstName\":null,\"LastName\":null,\"IsActive\":false,\"Email\":null,\"Password\":null,\"UserName\":null,\"UniqueName\":\"-\"}";
            }
        }
        public string UserBySlackUserNameForLeaveApplicant
        {
            get
            {
                return "{\"Id\":\"fce9e5de-0c3e-410f-8602-068e211d5f4d\",\"FirstName\":null,\"LastName\":null,\"IsActive\":false,\"Email\":null,\"Password\":null,\"UserName\":null,\"UniqueName\":\"-\"}";
            }
        }
        public string UserIdForTest
        {
            get
            {
                return "577696c8-136f-4865-8328-09e7d48ac58d";
            }
        }
        public string TestUser
        {
            get
            {
                return "pranali";
            }
        }
        public string scrumAnswerForTest
        {
            get
            {
                return "Worked on testing";
            }
        }
        public int ProjectIdForTest
        {
            get
            {
                return 2;
            }
        }
        public string Halt
        {
            get
            {
                return "halt";
            }
        }
        public string Resume
        {
            get
            {
                return "resume";
            }
        }
        public string AlreadyMarkedAsAnswered
        {
            get
            {
                return "Already marked as later or answered earlier\n";
            }
        }
        public string TeamLeaderIdForTest
        {
            get
            {
                return "5c84049f-f861-406d-b420-e1bf03c9e06e";
            }
        }
        public string EmployeeIdForTest
        {
            get
            {
                return "2d5f21e0-f7e7-4027-85ad-3faf8e1bf8bf";
            }
        }
        public string TestAccessToken
        {
            get
            {
                return "05310c2a-3fd3-4fa7-a059-e88bfbfe5f99";
            }
        }
        public string TestUserName
        {
            get
            {
                return "roshni@promactinfo.com";
            }
        }
        public string TestUserNameFalse
        {
            get
            {
                return "gourav@promactinfo.com";
            }
        }
        public string FirstUserName
        {
            get
            {
                return "roshni@promactinfo.com";
            }
        }
        public string FirstUserNameFalse
        {
            get
            {
                return "gourav@promactinfo.com";
            }
        }
        public string ProjectUsers
        {
            get
            {
                return "[{\"firstName\":\"roshni\",\"lastName\":null,\"isActive\":true,\"numberOfCasualLeave\":0.0,\"numberOfSickLeave\":0.0,\"joiningDate\":\"0001-01-01T00:00:00\",\"slackUserName\":null,\"projects\":null,\"createdBy\":null,\"createdDateTime\":\"0001-01-01T00:00:00\",\"updatedBy\":null,\"updatedDateTime\":\"0001-01-01T00:00:00\",\"id\":\"2d5f21e0-f7e7-4027-85ad-3faf8e1bf8bf\",\"userName\":\"roshni@promactinfo.com\",\"normalizedUserName\":null,\"email\":\"roshni@promactinfo.com\",\"normalizedEmail\":null,\"emailConfirmed\":false,\"passwordHash\":null,\"securityStamp\":null,\"concurrencyStamp\":\"22914d35-4125-4c89-b67f-bb2060ed4247\",\"phoneNumber\":null,\"phoneNumberConfirmed\":false,\"twoFactorEnabled\":false,\"lockoutEnd\":null,\"lockoutEnabled\":false,\"accessFailedCount\":0,\"roles\":[],\"Role\":\"TeamLeader\",\"claims\":[],\"logins\":[]}]";
            }
        }
        public string TeamLeaderDetailFromOauthServer
        {
            get
            {
                return "{\"firstName\":\"roshni\",\"lastName\":\"Promact\",\"isActive\":false,\"numberOfCasualLeave\":0.0,\"numberOfSickLeave\":0.0,\"joiningDate\":\"0001-01-01T00:00:00\",\"slackUserName\":null,\"projects\":null,\"createdBy\":null,\"createdDateTime\":\"0001-01-01T00:00:00\",\"updatedBy\":null,\"updatedDateTime\":\"0001-01-01T00:00:00\",\"id\":\"2d5f21e0-f7e7-4027-85ad-3faf8e1bf8bf\",\"userName\":\"roshni@promactinfo.com\",\"normalizedUserName\":null,\"email\":\"roshni@promactinfo.com\",\"normalizedEmail\":null,\"emailConfirmed\":false,\"passwordHash\":null,\"securityStamp\":null,\"concurrencyStamp\":\"a39b2cff-51e2-4f1d-bde9-096cefb17497\",\"phoneNumber\":null,\"phoneNumberConfirmed\":false,\"twoFactorEnabled\":false,\"lockoutEnd\":null,\"lockoutEnabled\":false,\"accessFailedCount\":0,\"roles\":[],\"Role\":\"TeamLeader\",\"claims\":[],\"logins\":[]}";
            }
        }
        public string EmployeeDetailFromOauthServer
        {
            get
            {
                return "{\"firstName\":\"roshni\",\"lastName\":\"Promact\",\"isActive\":false,\"numberOfCasualLeave\":0.0,\"numberOfSickLeave\":0.0,\"joiningDate\":\"0001-01-01T00:00:00\",\"slackUserName\":null,\"projects\":null,\"createdBy\":null,\"createdDateTime\":\"0001-01-01T00:00:00\",\"updatedBy\":null,\"updatedDateTime\":\"0001-01-01T00:00:00\",\"id\":\"2d5f21e0-f7e7-4027-85ad-3faf8e1bf8bf\",\"userName\":\"roshni@promactinfo.com\",\"normalizedUserName\":null,\"email\":\"roshni@promactinfo.com\",\"normalizedEmail\":null,\"emailConfirmed\":false,\"passwordHash\":null,\"securityStamp\":null,\"concurrencyStamp\":\"a39b2cff-51e2-4f1d-bde9-096cefb17497\",\"phoneNumber\":null,\"phoneNumberConfirmed\":false,\"twoFactorEnabled\":false,\"lockoutEnd\":null,\"lockoutEnabled\":false,\"accessFailedCount\":0,\"roles\":[],\"Role\":\"Employee\",\"claims\":[],\"logins\":[]}";
            }
        }
        public string IdForTest
        {
            get
            {
                return "13b0f2ca-92f5-4713-a67e-37e50172e148f";
            }
        }

        public string TestUserId
        {
            get
            {
                return "aac59fbc-7835-4bd7-9080-6b6766302080";
            }
        }
        public string NextQuestion
        {
            get
            {
                return "<@pranali> What did you do yesterday?";
            }
        }
        public string QuestionToNextEmployee
        {
            get
            {
                return "Good Day <@{0}>!\nWhat did you do yesterday?";
            }
        }
        public string PreviousDayStatusForTest
        {
            get
            {
                return "Scrum has been resumed\nGood Day <@apoorvapatel>!\n\r\n*Your previous day's status is :*\n\r\n*_Q_*: What did you do yesterday?\r\n*_A_*: _Sorry I have nothing to ask you._\r\n\r\n*Please answer the following questions today*\r\n\r\nWhat did you do yesterday?";
            }
        }

        #endregion

        public string RoleAdmin
        {
            get
            {
                return "Admin";
            }
        }
        public string RoleTeamLeader
        {
            get
            {
                return "TeamLeader";
            }
        }
        public string RoleEmployee
        {
            get
            {
                return "Employee";
            }
        }
        public string TeamMembersUrl
        {
            get
            {
                return "/teammembers";
            }
        }
        public string UserRoleUrl
        {
            get
            {
                return "/role";
            }
        }

        public string AllProjectUrl
        {
            get
            {
                return "list";
            }
        }
        public string GetProjectDetails
        {
            get
            {
                return "/detail";
            }
        }
        public string Start
        {
            get
            {
                return "start";
            }
        }
        public string PersonNotAvailable
        {
            get
            {
                return "Person Not Available on {0}";
            }
        }
        public string FormatForDate
        {
            get
            {
                return "MMM dd,yyyy";
            }
        }
        public string ScrumFirstQuestion
        {
            get
            {
                return "What did you do to change the world yesterday?";
            }
        }
        public string ScrumSecondQuestion
        {
            get
            {
                return "How you are going to rock it today?";
            }
        }
        public string ScrumThirdQuestion
        {
            get
            {
                return "Are there any obstacles unfortunate enough to be standing in your way?";
            }
        }
        public string NotAvailable
        {
            get
            {
                return "Not Available";
            }
        }
        public string NextPage
        {
            get
            {
                return "Next";
            }
        }
        public string Previouspage
        {
            get
            {
                return "Previous";
            }
        }
        public string AdminLogin
        {
            get
            {
                return "{\"Id\":\"2d5f21e0-f7e7-4027-85ad-3faf8e1bf8bf\",\"FirstName\":\"Admin\",\"LastName\":\"Promact\",\"IsActive\":true,\"Role\":\"Admin\",\"NumberOfCasualLeave\":0.0,\"NumberOfSickLeave\":0.0,\"JoiningDate\":\"0001-01-01T00:00:00\",\"SlackUserName\":\"roshni\",\"Email\":\"roshni@promactinfo.com\",\"Password\":null,\"UserName\":\"roshni@promactinfo.com\",\"UniqueName\":\"Admin-roshni@promactinfo.com\",\"RoleName\":null}";
            }
        }
        public string TestQuestion
        {
            get
            {
                return "Good Day <@pranali>!\n\r\n*Your scrum status of {0} is :*\n\r\n*_Q_*: What did you do yesterday?\r\n*_A_*: _Sorry I have nothing to ask you._\r\n\r\n*Please answer the following questions today*\r\n\r\nWhat did you do yesterday?";
            }
        }
        public string InActiveInOAuth
        {
            get
            {
                return "<@{0}> is marked as In-active,not added as a user in the project(in OAuth server) or not in OAuth server. Please contact your system administrator.\n ";
            }
        }
        public string TeamLeaderLogin
        {
            get
            {
                return "{\"Id\":\"2d5f21e0-f7e7-4027-85ad-3faf8e1bf8bf\",\"FirstName\":\"Admin\",\"LastName\":\"Promact\",\"IsActive\":true,\"Role\":\"TeamLeader\",\"NumberOfCasualLeave\":0.0,\"NumberOfSickLeave\":0.0,\"JoiningDate\":\"0001-01-01T00:00:00\",\"SlackUserName\":\"roshni\",\"Email\":\"roshni@promactinfo.com\",\"Password\":null,\"UserName\":\"roshni@promactinfo.com\",\"UniqueName\":\"Admin-roshni@promactinfo.com\",\"RoleName\":null}";
            }
        }
        public string EmployeeLogin
        {
            get
            {
                return "{\"Id\":\"2d5f21e0-f7e7-4027-85ad-3faf8e1bf8bf\",\"FirstName\":\"Admin\",\"LastName\":\"Promact\",\"IsActive\":true,\"Role\":\"Employee\",\"NumberOfCasualLeave\":0.0,\"NumberOfSickLeave\":0.0,\"JoiningDate\":\"0001-01-01T00:00:00\",\"SlackUserName\":\"roshni\",\"Email\":\"roshni@promactinfo.com\",\"Password\":null,\"UserName\":\"roshni@promactinfo.com\",\"UniqueName\":\"Admin-roshni@promactinfo.com\",\"RoleName\":null}";
            }
        }
        public string TeamLeaderLoginDetails
        {
            get
            {
                return "{\"Id\":\"01d5e634-f073-49ca-b1b6-c0b04508577b\",\"FirstName\":\"Admin\",\"LastName\":\"Promact\",\"IsActive\":true,\"Role\":\"TeamLeader\",\"NumberOfCasualLeave\":0.0,\"NumberOfSickLeave\":0.0,\"JoiningDate\":\"0001-01-01T00:00:00\",\"SlackUserName\":\"roshni\",\"Email\":\"roshni@promactinfo.com\",\"Password\":null,\"UserName\":\"roshni@promactinfo.com\",\"UniqueName\":\"Admin-roshni@promactinfo.com\",\"RoleName\":null}";
            }
        }
        public string ProjectDetail
        {
            get
            {
                return "{\"id\":1012,\"name\":\"xvz\",\"slackChannelName\":\"xvz\",\"isActive\":true,\"teamLeaderId\":\"01d5e634-f073-49ca-b1b6-c0b04508577b\",\"createdBy\":\"2d5f21e0-f7e7-4027-85ad-3faf8e1bf8bf\",\"createdDate\":\"2016-09-12\",\"updatedBy\":null,\"updatedDate\":null,\"teamLeader\":{\"Id\":\"01d5e634-f073-49ca-b1b6-c0b04508577b\",\"FirstName\":\"abc\",\"LastName\":\"abc\",\"IsActive\":true,\"Role\":\"TeamLeader\",\"NumberOfCasualLeave\":9.0,\"NumberOfSickLeave\":5.0,\"JoiningDate\":\"2016-07-20T00:00:00\",\"SlackUserName\":\"abc\",\"Email\":\"abc@promactinfo.com\",\"Password\":null,\"UserName\":\"abc@promactinfo.com\",\"UniqueName\":\"abc-abc@promactinfo.com\",\"RoleName\":null},\"applicationUsers\":[{\"Id\":\"2300b67f-69a1-4388-bc3e-56d638a80aaf\",\"FirstName\":\"xyz\",\"LastName\":\"XYZ\",\"IsActive\":true,\"Role\":\"Employee\",\"NumberOfCasualLeave\":14.0,\"NumberOfSickLeave\":7.0,\"JoiningDate\":\"2015-01-06T00:00:00\",\"SlackUserName\":\"xyz\",\"Email\":\"xyz@promactinfo.com\",\"Password\":null,\"UserName\":\"xyz@promactinfo.com\",\"UniqueName\":\"xyz-xyz@promactinfo.com\",\"RoleName\":null},{\"Id\":\"2d5f21e0-f7e7-4027-85ad-3faf8e1bf8bf\",\"FirstName\":\"Admin\",\"LastName\":\"Promact\",\"IsActive\":true,\"Role\":\"Employee\",\"NumberOfCasualLeave\":0.0,\"NumberOfSickLeave\":0.0,\"JoiningDate\":\"0001-01-01T00:00:00\",\"SlackUserName\":\"roshni\",\"Email\":\"roshni@promactinfo.com\",\"Password\":null,\"UserName\":\"roshni@promactinfo.com\",\"UniqueName\":\"Admin-roshni@promactinfo.com\",\"RoleName\":null}]}";
            }
        }
        public string ProjectDetailsForAdminFromOauth
        {
            get
            {
                return "[{\"id\":1011,\"name\":\"abc\",\"slackChannelName\":\"abc\",\"isActive\":true,\"teamLeaderId\":\"01d5e634-f073-49ca-b1b6-c0b04508577b\",\"createdBy\":\"2d5f21e0-f7e7-4027-85ad-3faf8e1bf8bf\",\"createdDate\":\"2016-09-12\",\"updatedBy\":null,\"updatedDate\":null,\"teamLeader\":{\"Id\":\"01d5e634-f073-49ca-b1b6-c0b04508577b\",\"FirstName\":\"abc\",\"LastName\":\"abc\",\"IsActive\":true,\"Role\":\"TeamLeader\",\"NumberOfCasualLeave\":9.0,\"NumberOfSickLeave\":5.0,\"JoiningDate\":\"2016-07-20T00:00:00\",\"SlackUserName\":\"abc\",\"Email\":\"abc@promactinfo.com\",\"Password\":null,\"UserName\":\"abc@promactinfo.com\",\"UniqueName\":\"abc-abc@promactinfo.com\",\"RoleName\":null},\"applicationUsers\":[{\"Id\":\"2d5f21e0-f7e7-4027-85ad-3faf8e1bf8bf\",\"FirstName\":\"Admin\",\"LastName\":\"Promact\",\"IsActive\":true,\"Role\":\"Employee\",\"NumberOfCasualLeave\":0.0,\"NumberOfSickLeave\":0.0,\"JoiningDate\":\"0001-01-01T00:00:00\",\"SlackUserName\":\"roshni\",\"Email\":\"roshni@promactinfo.com\",\"Password\":null,\"UserName\":\"roshni@promactinfo.com\",\"UniqueName\":\"Admin-roshni@promactinfo.com\",\"RoleName\":null},{\"Id\":\"f83860c3-e0b8-4f80-9aa7-82c71eaf50d7\",\"FirstName\":\"grv\",\"LastName\":\"ase=\",\"IsActive\":true,\"Role\":\"Employee\",\"NumberOfCasualLeave\":4.0,\"NumberOfSickLeave\":2.0,\"JoiningDate\":\"2016-01-06T00:00:00\",\"SlackUserName\":\"grv\",\"Email\":\"grv@promactinfo.com\",\"Password\":null,\"UserName\":\"grv@promactinfo.com\",\"UniqueName\":\"grv-grv@promactinfo.com\",\"RoleName\":null}]}]";
            }
        }
        public string ProjectDetailsForTeamLeaderFromOauth
        {
            get
            {
                return "[{\"id\":1013,\"name\":\"rew\",\"slackChannelName\":\"rew\",\"isActive\":true,\"teamLeaderId\":\"2d5f21e0-f7e7-4027-85ad-3faf8e1bf8bf\",\"createdBy\":\"2d5f21e0-f7e7-4027-85ad-3faf8e1bf8bf\",\"createdDate\":\"2016-01-01\",\"updatedBy\":null,\"updatedDate\":null,\"teamLeader\":{\"Id\":\"2d5f21e0-f7e7-4027-85ad-3faf8e1bf8bf\",\"FirstName\":\"Admin\",\"LastName\":\"Promact\",\"IsActive\":true,\"Role\":\"TeamLeader\",\"NumberOfCasualLeave\":0.0,\"NumberOfSickLeave\":0.0,\"JoiningDate\":\"0001-01-01T00:00:00\",\"SlackUserName\":\"roshni\",\"Email\":\"roshni@promactinfo.com\",\"Password\":null,\"UserName\":\"roshni@promactinfo.com\",\"UniqueName\":\"Admin-roshni@promactinfo.com\",\"RoleName\":null},\"applicationUsers\":[{\"Id\":\"01d5e634-f073-49ca-b1b6-c0b04508577b\",\"FirstName\":\"abc\",\"LastName\":\"abc\",\"IsActive\":true,\"Role\":\"Employee\",\"NumberOfCasualLeave\":9.0,\"NumberOfSickLeave\":5.0,\"JoiningDate\":\"2016-07-20T00:00:00\",\"SlackUserName\":\"abc\",\"Email\":\"abc@promactinfo.com\",\"Password\":null,\"UserName\":\"abc@promactinfo.com\",\"UniqueName\":\"abc-abc@promactinfo.com\",\"RoleName\":null},{\"Id\":\"2300b67f-69a1-4388-bc3e-56d638a80aaf\",\"FirstName\":\"xyz\",\"LastName\":\"XYZ\",\"IsActive\":true,\"Role\":\"Employee\",\"NumberOfCasualLeave\":14.0,\"NumberOfSickLeave\":7.0,\"JoiningDate\":\"2015-01-06T00:00:00\",\"SlackUserName\":\"xyz\",\"Email\":\"xyz@promactinfo.com\",\"Password\":null,\"UserName\":\"xyz@promactinfo.com\",\"UniqueName\":\"xyz-xyz@promactinfo.com\",\"RoleName\":null}]}]";
            }
        }
        public string ProjectDetailsForEmployeeFromOauth
        {
            get
            {
                return "[{\"id\":1011,\"name\":\"abc\",\"slackChannelName\":\"abc\",\"isActive\":true,\"teamLeaderId\":\"01d5e634-f073-49ca-b1b6-c0b04508577b\",\"createdBy\":\"2d5f21e0-f7e7-4027-85ad-3faf8e1bf8bf\",\"createdDate\":\"2016-09-12\",\"updatedBy\":null,\"updatedDate\":null,\"teamLeader\":{\"Id\":\"01d5e634-f073-49ca-b1b6-c0b04508577b\",\"FirstName\":\"abc\",\"LastName\":\"abc\",\"IsActive\":true,\"Role\":\"TeamLeader\",\"NumberOfCasualLeave\":9.0,\"NumberOfSickLeave\":5.0,\"JoiningDate\":\"2016-07-20T00:00:00\",\"SlackUserName\":\"abc\",\"Email\":\"abc@promactinfo.com\",\"Password\":null,\"UserName\":\"abc@promactinfo.com\",\"UniqueName\":\"abc-abc@promactinfo.com\",\"RoleName\":null},\"applicationUsers\":[{\"Id\":\"2d5f21e0-f7e7-4027-85ad-3faf8e1bf8bf\",\"FirstName\":\"Admin\",\"LastName\":\"Promact\",\"IsActive\":true,\"Role\":\"Employee\",\"NumberOfCasualLeave\":0.0,\"NumberOfSickLeave\":0.0,\"JoiningDate\":\"0001-01-01T00:00:00\",\"SlackUserName\":\"roshni\",\"Email\":\"roshni@promactinfo.com\",\"Password\":null,\"UserName\":\"roshni@promactinfo.com\",\"UniqueName\":\"Admin-roshni@promactinfo.com\",\"RoleName\":null},{\"Id\":\"f83860c3-e0b8-4f80-9aa7-82c71eaf50d7\",\"FirstName\":\"grv\",\"LastName\":\"ase=\",\"IsActive\":true,\"Role\":\"Employee\",\"NumberOfCasualLeave\":4.0,\"NumberOfSickLeave\":2.0,\"JoiningDate\":\"2016-01-06T00:00:00\",\"SlackUserName\":\"grv\",\"Email\":\"grv@promactinfo.com\",\"Password\":null,\"UserName\":\"grv@promactinfo.com\",\"UniqueName\":\"grv-grv@promactinfo.com\",\"RoleName\":null}]}]";
            }
        }
        public string TestGroupName
        {
            get
            {
                return "xyz";
            }
        }

        public string TestId
        {
            get
            {
                return "01d5e634-f073-49ca-b1b6-c0b04508577b";
            }
        }
        public string TestAnswer
        {
            get
            {
                return "angular";
            }
        }
        public string HourLimitExceed
        {
            get
            {
                return string.Format("Your daily limit of task is {0} hour. For today you can't add task. Your working hour is exceeded", TaskMailHours);
            }
        }
        public string StartWorking
        {
            get
            {
                return "Just start working";
            }
        }
        public string HourSpentForTesting
        {
            get
            {
                return "5";
            }
        }
        public string HourSpentExceeded
        {
            get
            {
                return "9";
            }
        }
        public string WrongActionSlashCommand
        {
            get
            {
                return "applied cl Testing 14-09-2016 14-09-2016 14-09-2016";
            }
        }
        public string BackDateErrorMessage
        {
            get
            {
                return "Sorry! You cannot apply leave on back date.";
            }
        }
        public string InValidDateErrorMessage
        {
            get
            {
                return "Please check end date and re-join date. End date cannot beyond start date and"+
                    " rejoin date cannot beyond and same as end date";
            }
        }
        public string LeaveWrongCommandForBackDateCL
        {
            get
            {
                return string.Format("apply cl Testing {0} {0} {0}", DateTime.UtcNow.AddDays(-5).ToShortDateString());
            }
        }
        public string LeaveWrongCommandForBeyondDateFirstExample
        {
            get
            {
                return string.Format("apply cl Testing {0} {1} {2}", DateTime.UtcNow.ToShortDateString(), 
                    DateTime.UtcNow.AddDays(-3).ToShortDateString(), DateTime.UtcNow.AddDays(1).ToShortDateString());
            }
        }
        public string LeaveWrongCommandForBeyondDateSecondExample
        {
            get
            {
                return string.Format("apply cl Testing {0} {0} {1}", DateTime.UtcNow.ToShortDateString(),
                    DateTime.UtcNow.AddDays(-3).ToShortDateString());
            }
        }
        public string LeaveWrongCommandForBackDateSL
        {
            get
            {
                return string.Format("apply sl Testing {0}", DateTime.UtcNow.AddDays(-2).ToShortDateString());
            }
        }
        public string SlashCommandUpdateForBeyondStartDateFirstExample
        {
            get
            {
                return string.Format("update {0} {1} {2}", 1, DateTime.UtcNow.AddDays(-5).ToShortDateString(), 
                    DateTime.UtcNow.AddDays(1).ToShortDateString());
            }
        }

        public string SlashCommandUpdateForBeyondStartDateSecondExample
        {
            get
            {
                return string.Format("update {0} {1} {2}", 1, DateTime.UtcNow.ToShortDateString(), 
                    DateTime.UtcNow.AddDays(-3).ToShortDateString());
            }
        }
        public string JsonContentString
        {
            get
            {
                return "application/json";
            }
        }

        public string UserSlackId
        {
            get
            {
                return "U0HJ49KJ4";
            }
        }
        public string TeamLeaderSlackId
        {
            get
            {
                return "U0HJ34YU9";
            }
        }

        public string ManagementSlackId
        {
            get
            {
                return "U0525LCJR";
            }
        }
        public string Ok
        {
            get
            {
                return "ok";
            }
        }

        public string LeaveAllowed
        {
            get
            {
                return "{\"casualLeave\":10.0,\"sickLeave\":5.0}";
            }
        }

        public string LeaveAlreadyExistOnSameDate
        {
            get
            {
                return "Leave Already exist on the date you have entered for new leave.";
            }
        }

        public string ReplyTextForUpdateLeave
        {
            get
            {
                return "You have {0} Leave for {1} From {2} To {3} for Reason {4} will re-join by {5}";
            }
        }

        public string ReplyTextForCasualLeaveList
        {
            get
            {
                return "Casual leave {0} {1} {2} {3} {4} {5}";
            }
        }

        public string ReplyTextForSickLeaveList
        {
            get
            {
                return "Sick leave {0} {1} {2} {3} {4}";
            }
        }

        public string ReplyTextForCancelLeave
        {
            get
            {
                return "Your leave Id no: {0} From {1} To {2} has been {3}";
            }
        }

        public string ReplyTextForErrorInCancelLeave
        {
            get
            {
                return string.Format("{0}{1}{2}", LeaveDoesnotExist, OrElseString, CancelLeaveError);
            }
        }

        public string ReplyTextForCasualLeaveStatus
        {
            get
            {
                return "Casual leave Id no: {0} From {1} To {2} for {3} is {4}";
            }
        }

        public string ReplyTextForSickLeaveStatus
        {
            get
            {
                return "Sick leave Id no: {0} From {1} for {2} is {3}";
            }
        }

        public string ReplyTextForCasualLeaveBalance
        {
            get
            {
                return "You have taken {0} casual leave out of {1}{2}You are left with {3} casual leave";
            }
        }

        public string ReplyTextForSickLeaveBalance
        {
            get
            {
                return "{2}You have taken {0} sick leave out of {1}{2}You are left with {3} sick leave";
            }
        }

        public string ReplyTextForSickLeaveUpdate
        {
            get
            {
                return "Sick leave of {0} from {1} to {2} for reason {3} has been updated, will rejoin on {4}";
            }
        }

        public string ReplyTextForSMTPExceptionErrorMessage
        {
            get
            {
                return "{0}. {1}";
            }
        }

        public string SlashCommandErrorMessage
        {
            get
            {
                return string.Format("{0}{1}{2}{1}{3}", LeaveNoUserErrorMessage, Environment.NewLine,
                    OrElseString, SlackErrorMessage);
            }
        }

        public string ReplyTextForCasualLeaveApplied
        {
            get
            {
                return "Leave has been applied by {0} From {1} To {2} for Reason {3} will re-join by {4}";
            }
        }

        public string ReplyTextForSickLeaveApplied
        {
            get
            {
                return "Sick leave has been applied for {0} from {1} for reason {2}";
            }
        }

        public string UpdateMessageUrl
        {
            get
            {
                return "?token={0}&ts={1}&channel={2}&text={3}&pretty=1";
            }
        }

        public string AtTheRate
        {
            get
            {
                return "@";
            }
        }

        public string SlackOauthRequestUrl
        {
            get
            {
                return "?client_id={0}&client_secret={1}&code={2}&pretty=1";
            }
        }

        public string SlackUserDetailsUrl
        {
            get
            {
                return "?token={0}&pretty=1";
            }
        }

        public string EmployeeFirstLastNameFormat
        {
            get
            {
                return "{0} {1}";
            }
        }

        public string FirstAndSecondIndexStringFormat
        {
            get
            {
                return "{0}{1}";
            }
        }

        public string FirstSecondAndThirdIndexStringFormat
        {
            get
            {
                return "{0}{1}{2}";
            }
        }

        public string StringValueOneForTest
        {
            get
            {
                return "1";
            }
        }

        public string StringValueFiftyFiveForTest
        {
            get
            {
                return "55";
            }
        }

        public string ControllerErrorMessageStringFormat
        {
            get
            {
                return "{0}. Error -> {1}";
            }
        }

        public string ExternalLoginUrl
        {
            get
            {
                return "{0}?clientId={1}";
            }
        }

        public string SlackAuthorizeAction
        {
            get
            {
                return "SlackAuthorize";
            }
        }

        public string Default
        {
            get
            {
                return "Default";
            }
        }

        public string CasualLeaveUpdateMessageForUser
        {
            get
            {
                return "Your leave Id {0} from {1} to {2} for reason {3} has been {4} by {5}";
            }
        }

        public string AlreadyUpdatedMessage
        {
            get
            {
                return "Leave has already been {0}";
            }
        }

        public string LeaveUpdateResponseJsonString
        {
            get
            {
                return "{\"actions\":[{\"name\":\"Approved\",\"value\":\"Approved\"}],\"callback_id\":\"3\",\"team\":{\"id\":\"T04K6NL66\",\"domain\":\"promact\"},\"channel\":{\"id\":\"D0HHZPADB\",\"name\":\"directmessage\"},\"user\":{\"id\":\"U0HJ49KJ4\",\"name\":\"siddhartha\"},\"action_ts\":\"1481194632.880940\",\"message_ts\":\"1481194612.000007\",\"attachment_id\":\"1\",\"token\":\"oQ7HPOZziax3MR4pzuImuQBR\",\"original_message\":{\"text\":\"\",\"bot_id\":\"B3C8ARBV2\",\"attachments\":[{\"callback_id\":\"3\",\"fallback\":\"Leave Applied\",\"title\":\"Leave has been applied by prince From 16-12-2016 To 16-12-2016 for Reason hgkjiuyyuiyuiyui will re-join by 17-12-2016\",\"id\":1,\"color\":\"3AA3E3\",\"actions\":[{\"id\":\"1\",\"name\":\"Approved\",\"text\":\"Approved\",\"type\":\"button\",\"value\":\"Approved\",\"style\":\"\"},{\"id\":\"2\",\"name\":\"Rejected\",\"text\":\"Rejected\",\"type\":\"button\",\"value\":\"Rejected\",\"style\":\"\"}]}],\"type\":\"message\",\"subtype\":\"bot_message\",\"ts\":\"1481194612.000007\"},\"response_url\":\"https:\\/\\/hooks.slack.com\\/actions\\/T04K6NL66\\/114318064436\\/QoOBNI8kW8w3prmwxe7ONgA7\"}";
            }
        }

        public string Payload
        {
            get
            {
                return "payload";
            }
        }
        public string LeaveUpdateEmailStringFormat
        {
            get
            {
                return "{0} {1}";
            }
        }
        public string RequestToAddSlackApp
        {
            get
            {
                return string.Format("Please add our slack app to your slack slackbot channel. Click here {0}", 
                    AppSettingUtil.PromactErpUrl);
            }
        }

        public string Star
        {
            get
            {
                return "*";
            }
        }

        public string HttpRequestExceptionErrorMessage
        {
            get
            {
                return "An error occurred while sending the request to other server";
            }
        }
        public string Space
        {
            get
            {
                return " ";
            }
        }
        public string TaskMailMaximumTime
        {
            get
            {
                return "8";
            }
        }
        public string Scopes
        {
            get
            { return "Scopes"; }

        }
        public string AuthenticationType
        {
            get
            {
                return "Cookies";
            }
        }
        public string AuthenticationTypeOidc
        {
            get
            {
                return "oidc";
            }
        }

        public string ResponseType
        {
            get
            {
                return "code id_token token";
            }
        }

        public string Scope { get { return "openid offline_access email profile user_read project_read"; } }

        public string RedirectUrl { get { return "signin-oidc"; } }
        public string Sub { get { return "sub"; } }
        public string Email { get { return "email"; } }
        public string SlackUserID { get { return "slack_user_id"; } }
        public string RoleClaimType { get { return "role"; } }
        public string NameClaimType { get { return "name"; } }
        public string Bearer { get { return "Bearer"; } }
    }
}