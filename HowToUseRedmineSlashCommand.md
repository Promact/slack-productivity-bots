# How to use Redmine Management

#### Step by step guide in using Redmine slash command to automate
You may skip Step 1 if it's already done.

#### 1. Common Initial Setup
  * User want to use redmine slash command must add Promact's slack app to their slack team.
  * The user must be registered in Promact-OAuth-Server and must be externally logged in from Promact-OAuth-Server in our slack application.

#### 2. Redmine Management Setup
  * Now the user are ready to use redmine slash command.
  *  Commands for redmine slash command :- Suppose you have created a slack app and created slash command like this - **/redmine** for redmine slash command. Then Command are like
    * **To list all projects** - /redmine projects list
    * **To get all issues assigned to me in particular project** - /redmine issues list [projectId]
    * **To create issue** - /redmine issues create [projectId] [Subject] [Description] [Priority] [Status] [Tracker] [AssignTo]
    * **To change assign person**/redmine issues changeassignee [issueId] [AssignedTo]
    * **to close the issue** - /redmine issues close [issueId]
    * **To add time entity for task** - /redmine issues timeentry [issueId] [hours] [date] [activity]
    * **To add or update** - /redmine apikey [API-Key]

#### Rules and Limitation
  * To use redmine slash command user need to add their redmine api key. 
  * To get api key - **Login with redmine** > **My account** > (on right side panel) **API access key** > click on **show** to get user redmine api key
  * User are able to get, create, update and change the issue on which user are assigned.