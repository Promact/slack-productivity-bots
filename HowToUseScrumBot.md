# How to use Scrum Bot 
#### Step by step guide in using Scrum Bot to automate your scrum meeting

You may skip Step 1, if it's already done.

1. Common Initial Setup
  * The user must be registered(details such as Slack Name must be exactly same as slack username) in Promact-OAuth-Server and must be externally login from Promact-OAuth-Server in our slack application.
  * Admin of their team must add our slack app 	Promact https://promact.slack.com/apps/A1WCQ9A3S-promact to their slack team.

2. Scrum Meeting Setup
  * Add the project details of the slack channel in which scrum meeting has to be conducted to Promact-Oauth-Server, where the slack channel name must be entered as it is given in slack.
  * Add the team members of the project(members of the slack channel) except the TL of the project.
  * Add the scrum bot to the slack channel in which scrum meeting has to be conducted.

3. Conduct the Scrum 
  * Start a scrum meeting by writing “scrum time”.
  * The bot will start asking questions to each member of the channel by calling out their names along with the question.The employee being asked must answer.
  * If an employee is on leave, when the bot asks a question to this employee then anyone can write “leave @username” to mark his absence.
  * If an employee is unavailable to answer scrum questions when he was asked questions by the bot but would be available later then, that employee can be marked to answer later by writing “later @username”. The employee's scrum can be conducted later by writing “scrum @username”.
  * Scrum will continue until all employees have answered all the questions. 
  * If scrum meeting was disrupted due to any reason it can be resumed by typing “scrum time”.
  * Scrum meeting can be halted by writing “scrum halt” and can be resumed by writing “scrum resume”.
  * The previous day scrum data (if any) of an employee will be displayed to the employee along with the first question.
  * If any individual user contacts the Bot (as in direct conversation with the bot), Bot will not respond, unless if it is for taking help i.e. by writing “scrum help”.


# Rules and Limitations
#### List of Rules and Limitations of Scrum meeting conducted by a scrum bot
  * Answers have to be given in one go.
  * Only the person who is asked and nobody else must answer the questions.
  * The answers given are not editable.
  * No other conversations would be made in between the scrum meeting(in the case when scrum is not halted). If slack trigger words (like “scrum meeting” or “scrum help”) are used and slackbot/scrumbot makes a conversation (like “Yeah..It's scrum time” or with Help message) then that might hinder the scrum meeting. 
  * Scrum meeting for a team can be conducted only once. Elaborating- If scrum meeting was disrupted the first time and an employee's scrum was not conducted, then the second time the scrum meeting is called, only his scrum meeting would be conducted.
  * The employees of a slack channel are identified from OAuth server data. So the OAuth server database must be in sync with slack channel details. (Bots must not be added as employees/users of a project at OAuth server.)
  * An employee cannot mark himself as on leave.


 
