using MagniseTestTaskFintacharts.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Threading.Tasks;

namespace MagniseTestTaskFintacharts.Web
{
    public class GetHistoricalData
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly Authentication _authentication;
        public GetHistoricalData(IHttpClientFactory httpClientFactory, Authentication authentication)
        {
            _httpClientFactory = httpClientFactory;
            _authentication = authentication;
        }

        async public Task<List<InstrumentValuesTimely>> GetDateRangeAsync(Guid guid, DateTime start, DateTime? end)
        {
            var httpClient = _httpClientFactory.CreateClient("Fintacharts");

            string uri = $"/api/bars/v1/bars/date-range?instrumentId={guid.ToString()}&provider=oanda&interval=1&periodicity=minute&startDate={start.Date.ToString("yyyy-MM-dd")}";

            if(end != null )
            {
                uri += $"&endDate={end.Value.Date.ToString("yyyy-MM-dd")}";
            }

            httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", await _authentication.Access_Token());

            using HttpResponseMessage response = await httpClient.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                JObject o = JObject.Parse(content);
                if(o.Count == 1)
                {                    
                        var timeRoot = JsonConvert.DeserializeObject<Root>(content);
                        return timeRoot.data;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                throw new Exception(response.Content.ToString());
            }
        }
    }
}
