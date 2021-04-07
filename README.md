# BirthdayBot  

**Summary**  

BirthdayBot is a Discord chat bot built using Discord.Net library.  
The main planned functionality is to assign server-specific birthday role to the users on their birthdays.  

This is a practice project, and therefore design decisions and implementation approaches are often deliberately chosen for the sake of experimenting with different libraries and design patterns.  

**Bot Commands (may be outdated)**  
test  
beep  
boop  
good bot  
birthdayme  
birthday @mention  
birthdaycheck @mention  

**Configuration**  

Create config.json file in main folder with the following content:  

	{  
	  "Token": "",  
	  "Prefix": "",  
	  "Role Name": ""  
	  "Birthdays": [  
        {  
          "Id": "",  
          "Date": ""  
        },  
        {  
          "Id": "",  
          "Date": ""  
        }  
      ]  
	}  

| Token - Bot's Token  
| Prefix - command prefix, can be empty or a string  

| Role Name - name of the birthday role  
| Birthdays - an array of user birthdays the bot checks against  
|   Id - User Id  
|   Date - Birthday date in DD MMM format (for example, "05 Apr")  

**Log**

Functional:  
\[x\] Create a Bot in Discord Developer Portal  
\[x\] Make Bot go online  
\[x\] Make Bot do simple action - react "boop" if I say "beep"  
\[x\] Make Bot grant role by hardcoded ID when prompted  
\[x\] Make Bot grant role by hardcoded Role Name when prompted  
\[x\] (Refactoring) Implement Bot commands via CommandService feature of Discord.Net  
\[x\] Make Bot change role with keyword prompt of @mention and Role Name from config  
\[x\] Make Bot change role with keyword prompt based on date/userID in config file  
\[ \] Make Bot change role automatically based on date/userID in birthday.config file \[in progress - partially implemented\]  

Tasks:  
\[x\] Write Readme  
\[x\] Implement direct REST call for adding user role  
  

