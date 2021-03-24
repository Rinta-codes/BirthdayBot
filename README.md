# BirthdayBot  

**Summary**  

BirthdayBot is a Discord chat bot built using Discord.Net library.  
The main planned functionality is to assign server-specific birthday role to the users on their birthdays.  

This is a practice project, and therefore design decisions and implementation approaches are often deliberately chosen for the sake of experimenting with different libraries and design patterns.  

**Configuration**  

Create config.json file in main folder with the following content:  

	{  
	  "Token": "",  
	  "Prefix": "",  
	  "Role Name": ""  
	}  

Token - bot's Token  
Prefix - command prefix, can be empty or a string  
Role Name - name of the birthday role  

**Backlog**

Functional:  
\[x\] Create a Bot in Discord Developer Portal  
\[x\] Make Bot go online  
\[x\] Make Bot do simple action - react "boop" if I say "beep"  
\[x\] Make Bot grant role by hardcoded ID when prompted  
\[x\] Make Bot grant role by hardcoded Role Name when prompted  
\[x\] (Refactoring) Implement Bot commands via CommandService feature of Discord.Net  
\[x\] Make Bot change role with keyword prompt of @mention and Role Name from config  

Tasks:  
\[x\] Add string tag to log event hook and pass \[REST\] \[WebSocket\] etc depending on event we are hooking onto  
\[x\] See if either of TypeReaders are getting used by current code - THEY ARE NOT, BLOCKED BY BUG IN DISCORD.NET  
\[x\] Write Readme  
\[x\] Implement direct REST call for adding user role  
  

