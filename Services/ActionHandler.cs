using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;
using BirthdayBot.Modules;
using System.Timers;
using System.Collections.Generic;

namespace BirthdayBot.Services
{
    class ActionHandler
    {
        private readonly IConfiguration _config;
        private readonly RestService _myRest;
        private readonly DiscordSocketClient _client;
        private readonly TimerFactory _timerFactory;

        private List<(Timer, Func<Task>)> repeatingActions;
        private readonly ActionModule actions; // I will eventually implement picking up Actions via Reflection
                                               // similar to how Discord.NET picks up commands, therefore this 
                                               // doesn't have to be a part of DI container

        public ActionHandler(IServiceProvider services)
        {
            Console.WriteLine("Action Handler initializing...");

            _config = services.GetRequiredService<IConfiguration>();
            _myRest = services.GetRequiredService<RestService>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            _timerFactory = services.GetRequiredService<TimerFactory>();

            // Placeholder code that initializes ActionModule and creates hardcoded Timer for SetBirthdayAction
            actions = new(_config, _client, _myRest);
            repeatingActions = new();

            repeatingActions.Add((_timerFactory.CreateTimer(interval.SECOND * 20), actions.SetBirthdaysAction));
            foreach (var action in repeatingActions)
            {
                action.Item1.Elapsed += async (object sender, ElapsedEventArgs e) => await action.Item2.Invoke();
                action.Item1.Start();
            }
        }

        public void Initialize() { }
    }
}
