# How to use Scrum Bot 
#### Step by step guide in using Scrum Bot to automate scrum meeting

You may skip Step 1, if it's already done.

#### 1. Common Initial Setup
  * A user must add Promact's slack app to their slack team.
  * A user must be registered(details such as Slack Name must be exactly same as slack username) in Promact-OAuth-Server and must be externally logged in from Promact-OAuth-Server in our slack application.

#### 2. Scrum Meeting Setup
  * Add the project details of the slack channel in which scrum meeting has to be conducted to Promact-Oauth-Server, where the slack channel name must be entered as it is given in slack.
  * Add the team members of the project(members of the slack channel) except the TL of the project.
  * Add the scrum bot to the slack channel in which scrum meeting has to be conducted.

#### 3. Conduct the Scrum 
  * Start a scrum meeting by writing **“start @botname”**.
  * The bot will start asking questions to each member(except the TL) of the channel by calling out their names along with the question.The employee being asked must answer.
  * If a user is on leave, when the bot asks a question to this employee then anyone can write **“leave _@username_”** to mark his absence.
  * Scrum will continue until all employees have answered all the questions. 
  * If scrum meeting was disrupted due to any reason it can be started from where it was left by writing **“start @botname”**.
  * Scrum meeting can be halted by writing **“scrum halt”** and can be resumed by writing **“scrum resume”**.
  * The previous day scrum data (if any) of a user will be displayed to the user along with the first question.
  * If any user contacts the Bot (as in direct conversation with the bot), bot will not respond as expected, unless if it is for taking help i.e. by writing **“scrum help”**.


# Rules and Limitations
#### List of Rules and Limitations of Scrum meeting conducted by a scrum bot
  * Answers have to be given in one go.
  * Only the person who is asked and nobody else must answer the questions.
  * The answers given are not editable.
  * Scrum meeting for a team can be conducted only once. Elaborating- If scrum meeting was disrupted the first time and a user's scrum was not conducted, then the second time the scrum meeting is called, only his scrum meeting would be conducted.
  * The users of a slack channel are identified from OAuth server data. So the OAuth server database must be in sync with slack channel details. Only active users of the OAuth server will be asked questions. Bots must not be added as employees/users of a project at OAuth server.
  * A user cannot mark himself as on leave.


 
