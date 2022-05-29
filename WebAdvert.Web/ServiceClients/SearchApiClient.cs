using System;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using WebAdvert.Web.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace WebAdvert.Web.ServiceClients
{
    public class SearchApiClient : ISearchApiClient
    {
        private readonly HttpClient _client;
        private readonly string BaseAddress = string.Empty;
        public SearchApiClient(HttpClient client, IConfiguration configuration)
        {
            _client = client;
            BaseAddress = configuration.GetSection("SearchApi").GetValue<string>("url");
        }

        public async Task<List<AdvertType>> Search(string keyword)
        {
            var result = new List<AdvertType>();
            var callUrl = $"{BaseAddress}/search/v1/{keyword}";
            var httpResponse = await _client.GetAsync(new Uri(callUrl)).ConfigureAwait(false);

            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                var allAdverts = JsonConvert.DeserializeObject<List<AdvertType>>(await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false));

                result.AddRange(allAdverts);
            }

            return result;
        }
    }
}