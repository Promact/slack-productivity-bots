using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Core.Test
{
    public class ScrumRepositoryTest
    {

        //#region Private Variables


        //private readonly IComponentContext _componentContext;
        //private readonly Mock<IHttpClientService> _mockHttpClient;
        //private readonly ApplicationUserManager _userManager;

        //private readonly IScrumSetUpRepository _scrumSetUpRepository;
        //private readonly ISlackUserRepository _slackUserRepository;
        //private readonly ISlackChannelRepository _slackChannelReposiroty;
        //private readonly IStringConstantRepository _stringConstant;
        //private readonly Mock<IServiceRepository> _mockServiceRepository;



        //private SlackProfile profile = new SlackProfile();

        //private SlackUserDetails slackUserDetails = new SlackUserDetails();
        //private SlackUserDetails testSlackUserDetails = new SlackUserDetails();
        //private SlackChannelDetails slackChannelDetails = new SlackChannelDetails();
        //private ApplicationUser user = new ApplicationUser();


        //#endregion


        //#region Constructor


        //public ScrumSetUpRepositoryTest()
        //{
        //    _componentContext = AutofacConfig.RegisterDependancies();

        //    _scrumSetUpRepository = _componentContext.Resolve<IScrumSetUpRepository>();
        //    _mockHttpClient = _componentContext.Resolve<Mock<IHttpClientService>>();
        //    _userManager = _componentContext.Resolve<ApplicationUserManager>();
        //    _slackUserRepository = _componentContext.Resolve<ISlackUserRepository>();
        //    _slackChannelReposiroty = _componentContext.Resolve<ISlackChannelRepository>();
        //    _stringConstant = _componentContext.Resolve<IStringConstantRepository>();
        //    _mockServiceRepository = _componentContext.Resolve<Mock<IServiceRepository>>();
        //    Initialization();

        //}
        //#endregion


        //#region Initialization


        ///// <summary>
        ///// A method is used to initialize variables which are repetitively used
        ///// </summary>
        //public void Initialization()
        //{
        //    user.Id = _stringConstant.StringIdForTest;
        //    user.Email = _stringConstant.EmailForTest;
        //    user.UserName = _stringConstant.EmailForTest;
        //    user.SlackUserId = _stringConstant.StringIdForTest;

        //    profile.Skype = _stringConstant.TestUserId;
        //    profile.Email = _stringConstant.EmailForTest;
        //    profile.FirstName = _stringConstant.UserNameForTest;
        //    profile.LastName = _stringConstant.TestUser;
        //    profile.Phone = _stringConstant.PhoneForTest;
        //    profile.Title = _stringConstant.UserNameForTest;

        //    slackUserDetails.UserId = _stringConstant.StringIdForTest;
        //    slackUserDetails.Name = _stringConstant.TestUser;
        //    slackUserDetails.TeamId = _stringConstant.PromactStringName;
        //    slackUserDetails.CreatedOn = DateTime.UtcNow;
        //    slackUserDetails.Deleted = false;
        //    slackUserDetails.IsAdmin = false;
        //    slackUserDetails.IsBot = false;
        //    slackUserDetails.IsPrimaryOwner = false;
        //    slackUserDetails.IsOwner = false;
        //    slackUserDetails.IsRestrictedUser = false;
        //    slackUserDetails.IsUltraRestrictedUser = false;
        //    slackUserDetails.Profile = profile;
        //    slackUserDetails.RealName = _stringConstant.TestUser + _stringConstant.TestUser;

        //    testSlackUserDetails.UserId = _stringConstant.IdForTest;
        //    testSlackUserDetails.Name = _stringConstant.UserNameForTest;
        //    testSlackUserDetails.TeamId = _stringConstant.PromactStringName;
        //    testSlackUserDetails.Profile = profile;

        //    slackChannelDetails.ChannelId = _stringConstant.SlackChannelIdForTest;
        //    slackChannelDetails.Deleted = false;
        //    slackChannelDetails.CreatedOn = DateTime.UtcNow;
        //    slackChannelDetails.Name = _stringConstant.GroupName;
        //    slackChannelDetails.ProjectId = 1;

        //    var accessTokenForTest = Task.FromResult(_stringConstant.AccessTokenForTest);
        //    _mockServiceRepository.Setup(x => x.GerAccessTokenByRefreshToken(_stringConstant.AccessTokenForTest)).Returns(accessTokenForTest);
        //}


        //private async Task AddChannelUserAsync()
        //{
        //    await _slackChannelReposiroty.AddSlackChannelAsync(slackChannelDetails);
        //    await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
        //    await _slackUserRepository.AddSlackUserAsync(testSlackUserDetails);
        //}

        //private async Task InActiveUserProjectSetup()
        //{
        //    UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
        //    await _userManager.CreateAsync(user);
        //    await _userManager.AddLoginAsync(user.Id, info);

        //    var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauthInValidUser);
        //    string projectRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.ProjectDetailUrl, _stringConstant.StringValueOneForTest);
        //    _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);
        //}

        //private async Task UserProjectSetup()
        //{
        //    UserLoginInfo info = new UserLoginInfo(_stringConstant.PromactStringName, _stringConstant.AccessTokenForTest);
        //    await _userManager.CreateAsync(user);
        //    await _userManager.AddLoginAsync(user.Id, info);

        //    var projectResponse = Task.FromResult(_stringConstant.ProjectDetailsFromOauth);
        //    string projectRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.ProjectDetailUrl, _stringConstant.StringValueOneForTest);
        //    _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);
        //}


        //#endregion


        //#region Test Cases


        //#region ProcessSetUpMessagesAsync Test Cases


        ///// <summary>
        ///// Method ProcessSetUpMessagesAsync Testing for list links
        ///// </summary>
        //[Fact, Trait("Category", "Required")]
        //public async Task ListLinksNotAUser()
        //{
        //    string actual = await _scrumSetUpRepository.ProcessSetUpMessagesAsync(_stringConstant.StringIdForTest, slackChannelDetails, _stringConstant.ListLinks);
        //    Assert.Equal(_stringConstant.YouAreNotInExistInOAuthServer, actual);
        //}


        ///// <summary>
        ///// Method ProcessSetUpMessagesAsync Testing for no list of links
        ///// </summary>
        //[Fact, Trait("Category", "Required")]
        //public async Task ListLinksNoLinks()
        //{
        //    await UserProjectSetup();
        //    await _slackChannelReposiroty.AddSlackChannelAsync(slackChannelDetails);
        //    await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
        //    var projectResponse = Task.FromResult(_stringConstant.ProjectAndTeamLeaderDetail);
        //    string projectRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.DetailsAndSlashForUrl, _stringConstant.StringIdForTest);
        //    _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);

        //    string actual = await _scrumSetUpRepository.ProcessSetUpMessagesAsync(_stringConstant.StringIdForTest, slackChannelDetails, _stringConstant.ListLinks);
        //    Assert.Equal(_stringConstant.NoLinks, actual);
        //}

        ///// <summary>
        ///// Method ProcessSetUpMessagesAsync Testing for list of links
        ///// </summary>
        //[Fact, Trait("Category", "Required")]
        //public async Task ListLinks()
        //{
        //    await UserProjectSetup();
        //    slackChannelDetails.ProjectId = 2;
        //    await _slackChannelReposiroty.AddSlackChannelAsync(slackChannelDetails);
        //    await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
        //    var projectResponse = Task.FromResult(_stringConstant.ProjectTeamLeaderDetail);
        //    string projectRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.DetailsAndSlashForUrl, _stringConstant.StringIdForTest);
        //    _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);
        //    string actual = await _scrumSetUpRepository.ProcessSetUpMessagesAsync(_stringConstant.StringIdForTest, slackChannelDetails, _stringConstant.ListLinks);
        //    string expected = _stringConstant.Links + Environment.NewLine + _stringConstant.ListOfLinks;
        //    Assert.Equal(expected, actual);
        //}


        ///// <summary>
        ///// Method ProcessSetUpMessagesAsync Testing linking channel
        ///// </summary>
        //[Fact, Trait("Category", "Required")]
        //public async Task LinkChannel()
        //{
        //    await UserProjectSetup();
        //    slackChannelDetails.ChannelId = _stringConstant.GroupNameStartsWith + _stringConstant.SlackChannelIdForTest;
        //    slackChannelDetails.ProjectId = null;
        //    await _slackChannelReposiroty.AddSlackChannelAsync(slackChannelDetails);
        //    await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
        //    var projectResponse = Task.FromResult(_stringConstant.ProjectTeamLeaderDetail);
        //    string projectRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.DetailsAndSlashForUrl, _stringConstant.StringIdForTest);
        //    _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);
        //    string actual = await _scrumSetUpRepository.ProcessSetUpMessagesAsync(_stringConstant.StringIdForTest, slackChannelDetails, _stringConstant.LinkTest);
        //    string expected = string.Format(_stringConstant.ProjectLinked, _stringConstant.OAuthProjectName, _stringConstant.GroupName);
        //    Assert.Equal(expected, actual);
        //}


        ///// <summary>
        ///// Method ProcessSetUpMessagesAsync Testing linking channel but not channel
        ///// </summary>
        //[Fact, Trait("Category", "Required")]
        //public async Task LinkChannelNotChannel()
        //{
        //    await UserProjectSetup();
        //    slackChannelDetails.ProjectId = null;
        //    await _slackChannelReposiroty.AddSlackChannelAsync(slackChannelDetails);
        //    await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
        //    var projectResponse = Task.FromResult(_stringConstant.ProjectTeamLeaderDetail);
        //    string projectRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.DetailsAndSlashForUrl, _stringConstant.StringIdForTest);
        //    _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);
        //    string actual = await _scrumSetUpRepository.ProcessSetUpMessagesAsync(_stringConstant.StringIdForTest, slackChannelDetails, _stringConstant.LinkTest);
        //    Assert.Equal(_stringConstant.OnlyPrivateChannel, actual);
        //}


        ///// <summary>
        ///// Method ProcessSetUpMessagesAsync Testing linking channel but unlink first
        ///// </summary>
        //[Fact, Trait("Category", "Required")]
        //public async Task LinkChannelUnLinkFirst()
        //{
        //    await UserProjectSetup();
        //    slackChannelDetails.ChannelId = _stringConstant.GroupNameStartsWith + _stringConstant.SlackChannelIdForTest;
        //    await _slackChannelReposiroty.AddSlackChannelAsync(slackChannelDetails);
        //    await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
        //    var projectResponse = Task.FromResult(_stringConstant.ProjectTeamLeaderDetail);
        //    string projectRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.DetailsAndSlashForUrl, _stringConstant.StringIdForTest);
        //    _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);
        //    string actual = await _scrumSetUpRepository.ProcessSetUpMessagesAsync(_stringConstant.StringIdForTest, slackChannelDetails, _stringConstant.LinkTest);
        //    string expected = string.Format(_stringConstant.UnLinkFirst, _stringConstant.OAuthProjectName);
        //    Assert.Equal(expected, actual);
        //}


        ///// <summary>
        ///// Method ProcessSetUpMessagesAsync Testing linking channel but already linked
        ///// </summary>
        //[Fact, Trait("Category", "Required")]
        //public async Task LinkChannelAlreadyLinked()
        //{
        //    await UserProjectSetup();
        //    slackChannelDetails.ChannelId = _stringConstant.GroupNameStartsWith + _stringConstant.SlackChannelIdForTest;
        //    slackChannelDetails.ProjectId = 2;
        //    await _slackChannelReposiroty.AddSlackChannelAsync(slackChannelDetails);
        //    await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
        //    var projectResponse = Task.FromResult(_stringConstant.ProjectTeamLeaderDetail);
        //    string projectRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.DetailsAndSlashForUrl, _stringConstant.StringIdForTest);
        //    _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);
        //    string actual = await _scrumSetUpRepository.ProcessSetUpMessagesAsync(_stringConstant.StringIdForTest, slackChannelDetails, _stringConstant.LinkTest);
        //    Assert.Equal(_stringConstant.AlreadyLinked, actual);
        //}


        ///// <summary>
        ///// Method ProcessSetUpMessagesAsync Testing linking channel but in active project
        ///// </summary>
        //[Fact, Trait("Category", "Required")]
        //public async Task LinkChannelProjectNotActive()
        //{
        //    await UserProjectSetup();
        //    slackChannelDetails.ChannelId = _stringConstant.GroupNameStartsWith + _stringConstant.SlackChannelIdForTest;
        //    slackChannelDetails.ProjectId = 2;
        //    await _slackChannelReposiroty.AddSlackChannelAsync(slackChannelDetails);
        //    await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
        //    var projectResponse = Task.FromResult(_stringConstant.InActiveProjectTeamLeaderDetail);
        //    string projectRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.DetailsAndSlashForUrl, _stringConstant.StringIdForTest);
        //    _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);
        //    string actual = await _scrumSetUpRepository.ProcessSetUpMessagesAsync(_stringConstant.StringIdForTest, slackChannelDetails, _stringConstant.LinkTest);
        //    Assert.Equal(_stringConstant.InActiveProject, actual);
        //}


        ///// <summary>
        ///// Method ProcessSetUpMessagesAsync Testing linking channel but no name
        ///// </summary>
        //[Fact, Trait("Category", "Required")]
        //public async Task LinkChannelNoName()
        //{
        //    string actual = await _scrumSetUpRepository.ProcessSetUpMessagesAsync(_stringConstant.StringIdForTest, slackChannelDetails, _stringConstant.LinkChannelNoName);
        //    Assert.Null(actual);
        //}


        ///// <summary>
        ///// Method ProcessSetUpMessagesAsync Testing linking channel but wrong command
        ///// </summary>
        //[Fact, Trait("Category", "Required")]
        //public async Task LinkChannelWrongCommand()
        //{

        //    slackChannelDetails.ChannelId = _stringConstant.GroupNameStartsWith + _stringConstant.SlackChannelIdForTest;
        //    slackChannelDetails.ProjectId = 2;
        //    await _slackChannelReposiroty.AddSlackChannelAsync(slackChannelDetails);
        //    string actual = await _scrumSetUpRepository.ProcessSetUpMessagesAsync(_stringConstant.StringIdForTest, slackChannelDetails, _stringConstant.LinkTest + _stringConstant.NameClaimType);
        //    Assert.Null(actual);
        //}


        ///// <summary>
        ///// Method ProcessSetUpMessagesAsync Testing linking channel but wrong command
        ///// </summary>
        //[Fact, Trait("Category", "Required")]
        //public async Task LinkChannelInCorrectCommand()
        //{
        //    slackChannelDetails.ChannelId = _stringConstant.GroupNameStartsWith + _stringConstant.SlackChannelIdForTest;
        //    slackChannelDetails.ProjectId = null;
        //    await _slackChannelReposiroty.AddSlackChannelAsync(slackChannelDetails);
        //    string actual = await _scrumSetUpRepository.ProcessSetUpMessagesAsync(_stringConstant.StringIdForTest, slackChannelDetails, _stringConstant.LinkTest + _stringConstant.NameClaimType);
        //    Assert.Equal(_stringConstant.ProjectChannelNotLinked, actual);
        //}



        ///// <summary>
        ///// Method ProcessSetUpMessagesAsync Testing unlinking channel but in active team leader
        ///// </summary>
        //[Fact, Trait("Category", "Required")]
        //public async Task UnLinkChannelTeamleaderNotActive()
        //{
        //    await UserProjectSetup();
        //    slackChannelDetails.ChannelId = _stringConstant.GroupNameStartsWith + _stringConstant.SlackChannelIdForTest;
        //    slackChannelDetails.ProjectId = 2;
        //    await _slackChannelReposiroty.AddSlackChannelAsync(slackChannelDetails);
        //    await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
        //    var projectResponse = Task.FromResult(_stringConstant.ProjectTeamLeaderInActiveDetail);
        //    string projectRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.DetailsAndSlashForUrl, _stringConstant.StringIdForTest);
        //    _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);
        //    string actual = await _scrumSetUpRepository.ProcessSetUpMessagesAsync(_stringConstant.StringIdForTest, slackChannelDetails, _stringConstant.UnlinkCommand);
        //    string expected = string.Format(_stringConstant.NotActiveUser, _stringConstant.StringIdForTest);
        //    Assert.Equal(expected, actual);
        //}


        ///// <summary>
        ///// Method ProcessSetUpMessagesAsync Testing unlinking channel but not project
        ///// </summary>
        //[Fact, Trait("Category", "Required")]
        //public async Task UnLinkChannelNoProject()
        //{
        //    await UserProjectSetup();
        //    slackChannelDetails.ChannelId = _stringConstant.GroupNameStartsWith + _stringConstant.SlackChannelIdForTest;
        //    slackChannelDetails.ProjectId = 1;
        //    await _slackChannelReposiroty.AddSlackChannelAsync(slackChannelDetails);
        //    await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
        //    var projectResponse = Task.FromResult(_stringConstant.ProjectTeamLeaderInActiveDetail);
        //    string projectRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.DetailsAndSlashForUrl, _stringConstant.StringIdForTest);
        //    _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);
        //    string actual = await _scrumSetUpRepository.ProcessSetUpMessagesAsync(_stringConstant.StringIdForTest, slackChannelDetails, _stringConstant.UnlinkTest);
        //    string expected = string.Format(_stringConstant.NotTeamLeader, _stringConstant.StringIdForTest);
        //    Assert.Equal(expected, actual);
        //}


        ///// <summary>
        ///// Method ProcessSetUpMessagesAsync Testing unlinking channel but not user
        ///// </summary>
        //[Fact, Trait("Category", "Required")]
        //public async Task UnLinkChannelNotUser()
        //{
        //    slackChannelDetails.ChannelId = _stringConstant.GroupNameStartsWith + _stringConstant.SlackChannelIdForTest;
        //    await _slackChannelReposiroty.AddSlackChannelAsync(slackChannelDetails);
        //    string actual = await _scrumSetUpRepository.ProcessSetUpMessagesAsync(_stringConstant.StringIdForTest, slackChannelDetails, _stringConstant.UnlinkTest);
        //    Assert.Equal(_stringConstant.YouAreNotInExistInOAuthServer, actual);
        //}


        ///// <summary>
        ///// Method ProcessSetUpMessagesAsync Testing unlinking channel
        ///// </summary>
        //[Fact, Trait("Category", "Required")]
        //public async Task UnLinkChannel()
        //{
        //    await UserProjectSetup();
        //    slackChannelDetails.ChannelId = _stringConstant.GroupNameStartsWith + _stringConstant.SlackChannelIdForTest;
        //    slackChannelDetails.ProjectId = 2;
        //    await _slackChannelReposiroty.AddSlackChannelAsync(slackChannelDetails);
        //    await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
        //    var projectResponse = Task.FromResult(_stringConstant.ProjectTeamLeaderDetail);
        //    string projectRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.DetailsAndSlashForUrl, _stringConstant.StringIdForTest);
        //    _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);
        //    string actual = await _scrumSetUpRepository.ProcessSetUpMessagesAsync(_stringConstant.StringIdForTest, slackChannelDetails, _stringConstant.UnlinkCommand);
        //    string expected = string.Format(_stringConstant.UnlinkedSuccessfully, _stringConstant.OAuthProjectName, _stringConstant.GroupName);
        //    Assert.Equal(expected, actual);
        //}


        ///// <summary>
        ///// Method ProcessSetUpMessagesAsync Testing unlinking channel but linked to another channel
        ///// </summary>
        //[Fact, Trait("Category", "Required")]
        //public async Task UnLinkChannelLinked()
        //{
        //    await UserProjectSetup();
        //    slackChannelDetails.ChannelId = _stringConstant.GroupNameStartsWith + _stringConstant.SlackChannelIdForTest;
        //    slackChannelDetails.ProjectId = 1;
        //    await _slackChannelReposiroty.AddSlackChannelAsync(slackChannelDetails);
        //    await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
        //    var projectResponse = Task.FromResult(_stringConstant.ProjectTeamLeaderDetail);
        //    string projectRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.DetailsAndSlashForUrl, _stringConstant.StringIdForTest);
        //    _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);
        //    string actual = await _scrumSetUpRepository.ProcessSetUpMessagesAsync(_stringConstant.StringIdForTest, slackChannelDetails, _stringConstant.UnlinkCommand);
        //    string expected = string.Format(_stringConstant.NotLinkedToChannel, _stringConstant.OAuthProjectName);
        //    Assert.Equal(expected, actual);
        //}


        ///// <summary>
        ///// Method ProcessSetUpMessagesAsync Testing unlinking channel but not linked yet
        ///// </summary>
        //[Fact, Trait("Category", "Required")]
        //public async Task UnLinkChannelNotLinked()
        //{
        //    await UserProjectSetup();
        //    slackChannelDetails.ChannelId = _stringConstant.GroupNameStartsWith + _stringConstant.SlackChannelIdForTest;
        //    slackChannelDetails.ProjectId = null;
        //    await _slackChannelReposiroty.AddSlackChannelAsync(slackChannelDetails);
        //    await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
        //    var projectResponse = Task.FromResult(_stringConstant.ProjectTeamLeaderDetail);
        //    string projectRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.DetailsAndSlashForUrl, _stringConstant.StringIdForTest);
        //    _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);
        //    string actual = await _scrumSetUpRepository.ProcessSetUpMessagesAsync(_stringConstant.StringIdForTest, slackChannelDetails, _stringConstant.UnlinkCommand);
        //    string expected = string.Format(_stringConstant.NotLinkedYet, _stringConstant.OAuthProjectName);
        //    Assert.Equal(expected, actual);
        //}


        ///// <summary>
        ///// Method ProcessSetUpMessagesAsync Testing unlinking channel but no team leader
        ///// </summary>
        //[Fact, Trait("Category", "Required")]
        //public async Task UnLinkChannelNoTeamleader()
        //{
        //    await UserProjectSetup();
        //    slackChannelDetails.ChannelId = _stringConstant.GroupNameStartsWith + _stringConstant.SlackChannelIdForTest;
        //    slackChannelDetails.ProjectId = 2;
        //    await _slackChannelReposiroty.AddSlackChannelAsync(slackChannelDetails);
        //    await _slackUserRepository.AddSlackUserAsync(slackUserDetails);
        //    var projectResponse = Task.FromResult(_stringConstant.ProjectTeamLeaderInActiveDetail);
        //    string projectRequestUrl = string.Format(_stringConstant.FirstAndSecondIndexStringFormat, _stringConstant.DetailsAndSlashForUrl, _stringConstant.StringIdForTest);
        //    _mockHttpClient.Setup(x => x.GetAsync(_stringConstant.ProjectUrl, projectRequestUrl, _stringConstant.AccessTokenForTest)).Returns(projectResponse);
        //    string actual = await _scrumSetUpRepository.ProcessSetUpMessagesAsync(_stringConstant.StringIdForTest, slackChannelDetails, _stringConstant.UnlinkTest);
        //    string expected = string.Format(_stringConstant.NotTeamLeader, _stringConstant.StringIdForTest);
        //    Assert.Equal(expected, actual);
        //}


        //#endregion


        //#endregion

    }
}
