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
    class ActionHandlingService
    {
        private readonly IConfiguration _config;
        private readonly RestService _myRest;
        private readonly DiscordSocketClient _client;
        private readonly TimerFactory _timerFactory;

        private List<(Timer timer, Func<Task> action)> _repeatingActions;
        private ActionModule actions; // I will eventually implement picking up Actions via Reflection
                                      // similar to how Discord.NET picks up commands, at which point
                                      // this variable will be no longer needed


        public ActionHandlingService(IServiceProvider services)
        {
            Console.WriteLine("Action Handler initializing...");

            _config = services.GetRequiredService<IConfiguration>();
            _myRest = services.GetRequiredService<RestService>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            _timerFactory = services.GetRequiredService<TimerFactory>();

            // await AddActionsAsync();
            AddActionsTemp();
        }

        /*
         * Since timers will only start once ActionHandler is initialised, and DI container does not 
         * initialise - only instantiate, I have to call empty InitializeAsync() method from Main() to 
         * get it going
         * 
         * I will also need async initialisation for when actions from ActionModule are loaded dynamically
         */
        public async Task InitializeAsync() { }

        /*
         * Placeholder method that directly initializes ActionModule and creates hardcoded Timer for SetBirthdayAction
         * To be replaced later with AddActionsAsync()
         */
        public void AddActionsTemp()
        {
            actions = new(_config, _client, _myRest);
            _repeatingActions = new();

            _repeatingActions.Add((_timerFactory.CreateTimer(Interval.HOUR * 24), actions.SetBirthdaysAction));
            foreach (var action in _repeatingActions)
            {
                action.timer.Elapsed += async (object sender, ElapsedEventArgs e) => await action.action.Invoke();
                action.timer.Start();
            }
        }

        /*
         * * Work In Progress
         * * When it is complete, it will do the following:
         * 
         * Loads Actions into the list and initialises their designated timers
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
