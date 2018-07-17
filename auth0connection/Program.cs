using System;
using System.Configuration;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;

namespace auth0connection
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Initialize a request to Auth0
            var authUrl = ConfigurationManager.AppSettings["auth_url"];
            var client = new RestClient(authUrl);
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");

            // Filling up the request body using configs in app.config
            var content = new RequestBodyContent();
            request.AddParameter("application/json", content.ToJson(), ParameterType.RequestBody);

            // Getting Auth0 access token
            var response = client.Execute(request);
            var responseContent = JsonConvert.DeserializeObject<ResponseBodyContent>(response.Content);

            // Output if we got the token from Auth0
            Console.WriteLine($"Getting Auth0 token: {(response.StatusCode == HttpStatusCode.OK ? "Succeeded" : "Failed")}");

            // Initialize a request to our API
            var apiUrl = ConfigurationManager.AppSettings["api_url"];
            client = new RestClient(apiUrl);
            request = new RestRequest(Method.GET);

            // Set the token in the authorization header
            // TODO: Don't request a new token every time, save it in your cache.
            request.AddHeader("authorization", responseContent.ToString());

            // Getting an object from the API
            response = client.Execute(request);
            var apiRequestSucceeded = response.StatusCode == HttpStatusCode.OK;
            
            // Output the api response
            Console.WriteLine($"Getting Api Response: {(apiRequestSucceeded ? "Succeeded" : "Failed")}");
            if (apiRequestSucceeded)
            {
                Console.WriteLine($"Api Response: {response.Content}");
            }

            // Leave console open
            Console.WriteLine("Press any key to quit...");
            while (Console.ReadKey().KeyChar == 0) { }
        }
    }

    // Auth0 Request
    internal class RequestBodyContent
    {
        public string ClientId { get; }
        public string ClientSecret { get; }
        public string Audience { get; }
        public string GrantType { get; }

        public RequestBodyContent()
        {
            ClientId = ConfigurationManager.AppSettings["client_id"];
            ClientSecret = ConfigurationManager.AppSettings["client_secret"];
            Audience = ConfigurationManager.AppSettings["audience"];
            GrantType = "client_credentials";
        }
    }

    // Auth Response
    internal class ResponseBodyContent
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public string ExpiresAt { get; set; }

        public override string ToString()
        {
            // Token example --> Bearer vWduaTRSejY4WG5halJMQGNsaWVudHMiLCJhdWQiOiJodHRwczovL3FzbC5jb20v...
            return $"{TokenType} {AccessToken}";
        }
    }

    internal static class ObjectExtensions
    {
        public static string ToJson(this RequestBodyContent value)
        {
            var serializedSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };
            return JsonConvert.SerializeObject(value, serializedSettings);
        }
    }
}
