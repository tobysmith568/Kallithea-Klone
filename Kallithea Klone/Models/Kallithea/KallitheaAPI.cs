using KallitheaKlone.Models.Repositories;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace KallitheaKlone.Models.Kallithea
{
    public class KallitheaAPI : IKallitheaAPI
    {
        //  Methods
        //  =======

        public async Task<ICollection<Repository>> GetRepositories(string host, string apiKey)
        {
            host = host ?? throw new ArgumentNullException(nameof(host));
            apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));

            ICollection<Repository> repositoryResponses = await GetResponse<ICollection<Repository>>(host, apiKey, "get_repos");

            return repositoryResponses;
        }

        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="WebException"></exception>
        public async Task<bool> ValidateUserAccount(string host, string apiKey)
        {
            host = host ?? throw new ArgumentNullException(nameof(host));
            apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));

            User userResponse = await GetResponse<User>(host, apiKey, "get_user");

            return userResponse != null && userResponse.Username != null;
        }

        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="WebException"></exception>
        private async Task<T> GetResponse<T>(string host, string apiKey, string action)
        {
            RestClient restClient = new RestClient($"{host}/_admin/api");

            IRestResponse response = await Task.Run(() =>
            {
                return restClient.Execute(GenerateRestRequest(apiKey, action));
            });

            return ProcessRestResponse<T>(response);
        }

        private RestRequest GenerateRestRequest(string apiKey, string action)
        {
            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("undefined", "{\"id\":\"1\",\"api_key\":\"" + apiKey + "\",\"method\":\"" + action + "\",\"args\":{}}", ParameterType.RequestBody);

            return request;
        }

        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="WebException"></exception>
        private T ProcessRestResponse<T>(IRestResponse response)
        {
            switch (response.ResponseStatus)
            {
                case ResponseStatus.Completed:
                    KallitheaBaseResponse<T> result = JsonConvert.DeserializeObject<KallitheaBaseResponse<T>>(response.Content);

                    if (result.Result == default)
                    {
                        return default;
                    }

                    return result.Result;
                case ResponseStatus.TimedOut:
                    throw new TimeoutException($"Webrequest to {response.ResponseUri} timed out");
                case ResponseStatus.Error:
                case ResponseStatus.Aborted:
                default:
                    throw new WebException("Error: " + response.ErrorMessage);
            }
        }
    }
}
