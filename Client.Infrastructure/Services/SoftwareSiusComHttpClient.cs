using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.Extensions.Options;
using Newtonsoft.Json;
//using Serilog;
using SharedModels.Models;

namespace Client.Infrastructure.Services
{
    public interface ISoftwareSiusComHttpClient
    {
        Task<IList<SoftwareSiusComFileModel>> GetSoftwareSiusComFileModels();
    }

    public class SoftwareSiusComHttpClient : ISoftwareSiusComHttpClient
    {
        //private readonly SiusUpdateBackendSettings _siusUpdateBackendSettings;
        private readonly HttpClient _client;

        private readonly string _getSoftwareSiusChannels = "/api/channels";
        private readonly string _getSoftwareSiusChannelsRelease = "/api/channels/release";

        //public SoftwareSiusComHttpClient(HttpClient client, IOptions<SiusUpdateBackendSettings> siusUpdateBackendSettings)
        public SoftwareSiusComHttpClient(HttpClient client)
        {
            //_siusUpdateBackendSettings = siusUpdateBackendSettings.Value;
            //client.BaseAddress = new Uri(_siusUpdateBackendSettings.UrlForScaDownload);
            client.BaseAddress = new Uri("https://software.sius.com");

            _client = client;
        }

        public async Task<IList<SoftwareSiusComFileModel>> GetSoftwareSiusComFileModels()
        {
            //var requestUrl = $"{_getSoftwareSiusFileModels}/{_siusUpdateBackendSettings.SoftwareSiusComChannel}";
            var requestUrl = _getSoftwareSiusChannelsRelease;

            HttpResponseMessage response = null;
            HttpContent content = null;
            string contentString = null;
            IList<SoftwareSiusComFileModel> result = null;
            try
            {
                response = await _client.GetAsync(requestUrl);
                content = response.Content;
                contentString = await content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<IList<SoftwareSiusComFileModel>>(contentString);
            }
            catch (Exception ex) 
            {
                result = new List<SoftwareSiusComFileModel>();
            }

            return result;
        }
    }
}
