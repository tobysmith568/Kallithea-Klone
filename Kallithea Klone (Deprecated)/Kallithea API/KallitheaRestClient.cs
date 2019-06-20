using Kallithea_Klone.Account_Settings;
using Kallithea_Klone.Kallithea;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Kallithea_Klone.Kallithea_API
{
    public class KallitheaRestClient<T>
    {
        //  Variables
        //  =========

        private readonly RestClient client;
        private readonly RestRequest request;

        //  Constructors
        //  ============

        public KallitheaRestClient(string action, string host = null, string apiKey = null)
        {
            if (action == null)
                action = "";
            if (host == null)
                host = AccountSettings.Host;
            if (apiKey == null)
                apiKey = AccountSettings.APIKey;

            client = new RestClient($"{host}/_admin/api");

            request = new RestRequest(Method.POST);
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("undefined", "{\"id\":\"1\",\"api_key\":\"" + apiKey + "\",\"method\":\"" + action + "\",\"args\":{}}", ParameterType.RequestBody);
        }

        //  Methods
        //  =======

        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="WebException"></exception>
        /// <exception cref="Kallithea_Klone.Kallithea_API.InvalidKallitheaResponseTypeException"></exception>
        public async Task<KallitheaResponse<T>> Run()
        {
            IRestResponse response = await Task.Run(() =>
            {
                return client.Execute(request);
            });

            switch (response.ResponseStatus)
            {
                case ResponseStatus.Completed:
                    KallitheaResponse<T> result = JsonConvert.DeserializeObject<KallitheaResponse<T>>(response.Content);

                    if (result.Result == null)
                        throw new InvalidKallitheaResponseTypeException(typeof(T));

                    return result;
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
