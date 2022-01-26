﻿using BirthdayBot.ActionModules;
using BirthdayBot.Data;
using BirthdayBot.Configuration;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;

namespace BirthdayBot.Services
{
    public class ActionHandlingService
    {
        private readonly int _interval = Interval.SECOND * 10000; // For now this variable will store period (in
                                                                // milliseconds) for how often Actions will be executed

        private readonly IOptions<BirthdayConfiguration> _birthdayConfig;
        private readonly RestService _myRest;
        private readonly DiscordSocketClient _client;
        private readonly TimerFactory _timerFactory;
        private readonly IBirthdaysRepository _birthdays;

        private List<(Timer timer, Func<Task> action)> _repeatingActions;
        private ActionModule _actions; // I will eventually implement picking up Actions via Reflection
                                       // similar to how Discord.NET picks up commands, at which point
                                       // this variable will be no longer needed


        public ActionHandlingService(IOptions<BirthdayConfiguration> birthdayConfig, RestService myRest, DiscordSocketClient client, TimerFactory timerFactory, IBirthdaysRepository birthdays)
        {
            Console.WriteLine("Action Handler initializing...");

            _birthdayConfig = birthdayConfig;
            _myRest = myRest;
            _client = client;
            _timerFactory = timerFactory;
            _birthdays = birthdays;

            AddActionsTemp();
        }

        /*
         * Since timers will only start once ActionHandler is initialised, and DI container does not 
         * initialise - only instantiate, I have to call empty InitializeAsync() method from Main() to 
         * get it going
         * 
         * I will also need async initialisation for when actions from ActionModule are loaded dynamically
         */
        public async Task InitializeAsync()
        {
            // await AddActionsAsync();
        }

        /*
         * Placeholder method that directly initializes ActionModule and creates hardcoded Timer for SetBirthdayAction
         * To be replaced later with AddActionsAsync()
         */
        private void AddActionsTemp()
        {
            _actions = new(_birthdayConfig, _client, _myRest, _birthdays);
            _repeatingActions = new();

            _repeatingActions.Add((_timerFactory.CreateTimer(_interval), _actions.SetBirthdaysActionAsync));
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