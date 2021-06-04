using BirthdayBot.Modules;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using System.Reflection;

namespace BirthdayBot.Services
{
    class ActionHandler
    {
        private readonly IConfiguration _config;
        private readonly RestService _myRest;
        private readonly DiscordSocketClient _client;
        private readonly TimerFactory _timerFactory;

        private List<(Timer timer, Func<Task> action)> repeatingActions;
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

            AddActionsAsync();

            // Placeholder code that initializes ActionModule and creates hardcoded Timer for SetBirthdayAction
            actions = new(_config, _client, _myRest);
            repeatingActions = new();

            repeatingActions.Add((_timerFactory.CreateTimer(Interval.HOUR * 24), actions.SetBirthdaysAction));
            foreach (var action in repeatingActions)
            {
                action.timer.Elapsed += async (object sender, ElapsedEventArgs e) => await action.action.Invoke();
                action.timer.Start();
            }
        }

        /*
         * Since timers will only start once ActionHandler is initialised, and DI container does not 
         * initialise - only instantiate, I have to call empty Initialize() method from Main() to 
         * get it going
         * 
         * I will also need async initialisation for when actions from ActionModule are loaded dynamically
         */
        public async Task InitializeAsync() { }

        /*
         * Loads Actions and initialises their designated timers
         */
        private async Task AddActionsAsync()
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            var result = new List<TypeInfo>();

            foreach (var typeInfo in assembly.DefinedTypes)
            {
                if (typeInfo.IsPublic)
                {
                    Console.WriteLine(typeInfo.ToString());
                    result.Add(typeInfo);
                }
            }
        }
    }
}
