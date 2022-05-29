using System;
using AutoMapper;
using System.Net;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using WebAdvert.AdvertApi.Models;
using Microsoft.Extensions.Configuration;

namespace WebAdvert.Web.ServiceClients
{
    public class AdvertApiClient : IAdvertApiClient
    {
        private readonly string _baseAddress;
        private readonly HttpClient _client;
        private readonly IMapper _mapper;

        public AdvertApiClient(IConfiguration configuration, HttpClient client, IMapper mapper)
        {
            _client = client;
            _mapper = mapper;

            _baseAddress = configuration.GetSection("AdvertApi").GetValue<string>("BaseUrl");
        }

        public async Task<AdvertResponse> CreateAsync(CreateAdvertModel model)
        {
            var advertApiModel = _mapper.Map<AdvertModel>(model);
            var jsonModel = JsonConvert.SerializeObject(advertApiModel);

            var response = await _client.PostAsync(new Uri($"{_baseAddress}/create"),
                new StringContent(
                    jsonModel,
                    Encoding.UTF8,
                    "application/json")).ConfigureAwait(false);

            var createAdvertResponse = JsonConvert.DeserializeObject<CreateAdvertResponse>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
            var advertResponse = _mapper.Map<AdvertResponse>(createAdvertResponse);

            return advertResponse;
        }

        public async Task<bool> ConfirmAsync(ConfirmAdvertRequest model)
        {
            var advertModel = _mapper.Map<ConfirmAdvertModel>(model);
            var jsonModel = JsonConvert.SerializeObject(advertModel);

            var response = await _client
                .PutAsync(new Uri($"{_baseAddress}/confirm"),
                    new StringContent(
                        jsonModel,
                        Encoding.UTF8,
                        "application/json")).ConfigureAwait(false);

            return response.StatusCode == HttpStatusCode.OK;
        }

        public async Task<List<Advertisement>> GetAllAsync()
        {
            var apiCallResponse = await _client.GetAsync(new Uri($"{_baseAddress}/all")).ConfigureAwait(false);
            var allAdvertModels = JsonConvert.DeserializeObject<List<AdvertModel>>(await apiCallResponse.Content.ReadAsStringAsync().ConfigureAwait(false));

            return allAdvertModels.Select(x => _mapper.Map<Advertisement>(x)).ToList();
        }

        public async Task<Advertisement> GetAsync(string advertId)
        {
            var apiCallResponse = await _client.GetAsync(new Uri($"{_baseAddress}/{advertId}")).ConfigureAwait(false);
            var fullAdvert = JsonConvert.DeserializeObject<AdvertModel>(await apiCallResponse.Content.ReadAsStringAsync().ConfigureAwait(false));

            return _mapper.Map<Advertisement>(fullAdvert);
        }
    }
}