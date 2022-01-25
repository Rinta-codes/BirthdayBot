# BirthdayBot  
  
**Summary**  
  
BirthdayBot is a Discord chat bot built using Discord.Net library.  
This is foremost a performative project, and therefore design decisions and implementation approaches are often deliberately chosen for the sake of experimenting with different libraries and design patterns.    
  
Functionally, it is intended to assign server-specific birthday role to the users on their birthdays.  
  
Implementation-wise, it showcases the following and more:  
- Comfortable use of a reasonably sized external library (Discord.NET) and its many functionalities  
- Dependency Injection via ServiceCollection container, including:   
  - Options pattern (IOptions)  
  - Factories (HttpClient)  
- Manual Factory implementation (TimerFactory)  
- Manual Rest API use (RestService)  
- Manual data storage implementation (IBirthdayRepository / BirthdayRepositoryCached)  
- Expanded functionality over what the external library provides (scheduled / repeated bot actions via ActionHandlingService)  
- Probably some other stuff; Look at the code  
  
**Bot Commands**  
*use "help" command to get most current list of commands*  
  
Potentially outdated:  
  
*beep*  
Simple interaction to test that the Bot is up.  
  
*good bot*  
Be polite to your Bot.  
  
*thank you bot*  
Say "thank you" to your Bot.  
  
*birthdayme*  
Assign "Birthday Cake" role to command caller.  
  
*birthday*  
Assign configured birthday role to @mentioned user.  
Only the first mentioned user will be processed.  
  
*birthdaycheck*  
If @mentioned user has a birthday - assign configured birthday role.  
Only the first mentioned user will be processed.  
  
*guilduser_discordlib*  
Get GuildUser via Discord.NET Rest client  
  
*birthday_discordlib*  
Assign birthday role to @mentioned user via Discord.NET Rest client  
  
*help*  
Lists available commands and their descriptions.  
Ignores disabled commands.  
  
**Configuration**  
  
Create config.json file in main folder with the following content:  
```json  
{  
    "Token": "",  
    "Prefix": "",  
    "Role Name": ""  
}  
```  
  
Token - Bot's Token  
Prefix - command prefix, can be empty or a string  
Role Name - name of the birthday role  
  
Create birthdays.json file in main folder with the following content:  
```json  
{  
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
```  
  
Id - User Id  
Date - Birthday date in DD MMM format (for example, "05 Apr")  