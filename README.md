# BirthdayBot  
  
**Summary**  
  
BirthdayBot is a Discord chat bot built using [Discord.Net library](https://github.com/discord-net/Discord.Net).  
This is foremost a performative project, and therefore design decisions and implementation approaches are often deliberately chosen for the sake of experimenting with different libraries and design patterns.    
  
Functionally, it is intended to assign server-specific birthday role to the users on their birthdays.  
  
Implementation-wise, it showcases the following and more:  
- Comfortable use of a reasonably sized external library ([Discord.NET](https://github.com/discord-net/Discord.Net)) and its many functionalities  
- Dependency Injection via ServiceCollection container, including:   
  - Options pattern (IOptions)  
  - Factories (HttpClient)  
- Manual Factory implementation ([TimerFactory](BirthdayBotSource/Services/TimerFactory.cs))  
- Manual Rest API use ([RestService](BirthdayBotSource/Services/RestService.cs))  
- Manual data storage implementation ([IBirthdaysRepository / IBirthdaysCache](../../tree/master/BirthdayBotSource/Data))  
- Reflection ([ActionHandlingService](../../tree/master/BirthdayBotSource/Services/ActionHandlingService.cs))
- Expanded functionality over what the external library provides (scheduled / repeated bot actions via ActionHandlingService)  
- Unit testing via MSTest ([BirthdayBotTest](../../tree/master/BirthdayBotTest))
- Etc.  
  
**Bot Commands**  
*use "help" command to get most current list of commands*  
  
Text commands (potentially outdated):  
*beep* - Simple interaction to test that the Bot is up  
*good bot* - Be polite to your Bot  
*thank you bot* - Say "thank you" to your Bot  
*birthdayme* - Assign "Birthday Cake" role to command caller  
*birthday* - Assign configured birthday role to @mentioned user  
*birthdaycheck* - If @mentioned user has a birthday - assign configured birthday role  
*guilduser_discordlib* - Get GuildUser via Discord.NET Rest client  
*birthday_discordlib* - Assign birthday role to @mentioned user via Discord.NET Rest client  
*help* - Lists available commands and their descriptions.  

Slash commands:
*/beep*
   
**Configuration**  
  
Create config.json file in main folder with the following content:  
```json  
{  
    "Token": "",  
    "Prefix": "",  
    "RoleName": ""  
}  
```  
  
Token - Bot's Token  
Prefix - command prefix, can be empty or a string  
RoleName - name of the birthday role  
  
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