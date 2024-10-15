using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Expressions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;

namespace MagniseTestTaskFintacharts.Web
{
    public class Authentication
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        private string access_token;
        private string refresh_token;

        private System.Timers.Timer aTimer;

        bool access_token_expired = true;

        async public Task<string> Access_Token()
        {
            if (access_token_expired == true)
            {
                await GetNewTokensAsync();
            }
            return access_token;
        }

        public Authentication(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            aTimer = new System.Timers.Timer(1800000);
        }

        async public Task GetNewTokensAsync()
        {
            var httpClient = _httpClientFactory.CreateClient("Fintacharts");
            var parameters = new Dictionary<string, string>();

            parameters.Add("grant_type", "password");
            parameters.Add("client_id", "app-cli");
            parameters.Add("username", _configuration["Web:USERNAME"]);
            parameters.Add("password", _configuration["Web:PASSWORD"]);

            using HttpResponseMessage response = await httpClient.PostAsync("identity/realms/fintatech/protocol/openid-connect/token", new FormUrlEncodedContent(parameters));
            if(response.IsSuccessStatusCode)
            {
                
                var content = await response.Content.ReadAsStringAsync();
                dynamic document = JsonConvert.DeserializeObject(content);
                access_token = document.access_token;
                refresh_token = document.refresh_token;

                access_token_expired = false;
                SetTimer();
            }
            else
            {
                throw new Exception(response.Content.ToString());
            }
        }

        private void SetTimer()
        {
            aTimer.Elapsed += OnTimedEvent;
            aTimer.Enabled = true;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}",
                              e.SignalTime);
            access_token_expired = true;
        }
    }
}
