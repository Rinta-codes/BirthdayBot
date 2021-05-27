using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BirthdayBot.Services
{
    /**
     * Custom implementation of direct call to Discord REST API
     * 
     * Currently only used to circumvent Discord.NET implementation of Add Role call,
     * which inadvertently requires GuildUser object, retrieval of which in turn requires 
     * Presence Intent to be enabled for the Discord Bot. 
     * 
     * This is actually unnecessary for Add Role API call.
     * 
     * Implementation is currently abstract enough to support generic PUT calls, so can be
     * reused if similar issue is encountered for other PUT calls in Discord.NET.
     * 
     * However it can be easily expanded to other types of calls as well.
     */
    public class RestService
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _config;
        private readonly string _uriBase = "https://discord.com/api";

        public RestService(IConfiguration config, IHttpClientFactory factory)
        {
            _config = config;
            _client = factory.CreateClient();

            GenerateHeaders();
        }

        private void GenerateHeaders()
        {
            _client.DefaultRequestHeaders.Clear();
            // Add Authorization header as per Discord documentation
            _client.DefaultRequestHeaders.Add("Authorization", "Bot" + " " + _config["Token"]);
            // _client.DefaultRequestHeaders.Add("Content-Type", "application/json"); // Not a request header, and not needed for Add Role call
            // Will sort out later to be more universal
        }

        /** 
         * Send PUT call to Discord REST API, print any response / exception to console
         */
        public async Task PutAsync(string requestString, HttpContent content)
        {
            try
            {
                var response = await _client.PutAsync(_uriBase + requestString, content);
                var responseString = await response.Content.ReadAsStringAsync();
                if (!String.IsNullOrEmpty(responseString))
                    Console.WriteLine("[{0}] {1}", this.GetType().Name, responseString);
                else
                    Console.WriteLine("[{0}] Command executed.", this.GetType().Name);
            }
            catch (Exception e)
            {
                Console.WriteLine("[{0}] {1}", this.GetType().Name, e.Message);
            }
        }
    }
}
