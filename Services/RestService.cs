﻿using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BirthdayBot.Services
{
    public class RestService
    {
        private readonly HttpClient _client = new();
        private readonly IConfiguration _config;
        private readonly string _uriBase = "https://discord.com/api";

        public RestService(IConfiguration config)
        {
            _config = config;

            GenerateHeaders();
        }

        private void GenerateHeaders()
        {
            _client.DefaultRequestHeaders.Clear();
            // Add Authentication token from configuration
            _client.DefaultRequestHeaders.Add("Authorization", "Bot" + " " + _config["Token"]);
            // _client.DefaultRequestHeaders.Add("Content-Type", "application/json");
        }

        public async Task PutAsync(string requestString, HttpContent content)
        {
            try
            {
                var response = await _client.PutAsync(_uriBase + requestString, content);
                if (!(response is null)) Console.WriteLine("[{0} {1}]", this.GetType().Assembly, response.Content.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("[{0} {1}]", this.GetType().Assembly, e.Message);
            }
        }
    }
}
