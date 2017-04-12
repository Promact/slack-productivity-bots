# How to use Leave Slash Command

#### Step by step guide in using Leave slash command to automate
You may skip Step 1 if it's already done.

#### 1. Common Initial Setup
  * User need to get externally logged in from Promact OAuth server and then need to add slack app in their private channel where they will receive message from leave bot regarding leave.

#### 2. Leave Slash Command Setup
  * Now the user are ready to use leave slash command.
  * Commands for leave slash command :- Suppose you have created a slack app and created slash command like this - **/leaves** for leave slash command. Then Command are like
    * **To apply casual leave:** /leaves apply cl [Reason(for long reason write in \" \")] [FromDate] [EndDate] [RejoinDate]
    * **To apply sick leave**: /leaves apply sl [Reason] [FromDate]
    * **To apply sick leave by admin for other**: /leaves apply sl [Reason] [FromDate] [username]
    * **For leaves list of yours**: /leaves list
    * **For leaves list of others**: /leaves list [username]
    * **For leave cancel**: /leaves cancel [leave Id number]
    * **For sick leave update(only for admin)**: / leaves update [leave id] [EndDate] [RejoinDate]
    * **For leave status of yours**: /leaves status
    * **For leave status of others**: /leaves status [username]
    * **For leaves balance**: /leaves balance
    * **For leave help**: /leaves help

#### Rules and Limitation
  * User cannot add leave date beyond today.
  * User cannot add leave for other. Only admin can add sick for others.
  * User cannot add sick leave more than one(1) day.
  * User cannot update any leave. Only admin can update sick leave of others.
  * Leave cannot be applied on the date, on which leave already exist. Example - I have applied a casual leave from 15-04-2017 to 19-04-2017 then I am not able to add new leave on and between 15-04-2017 to 19-04-2017.