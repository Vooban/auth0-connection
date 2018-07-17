using System;
using System.Configuration;
using System.IO;
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
            var authorizationToken = Authorization();

            ApiGet(authorizationToken);

            ApiPost(authorizationToken);

            // Leave console open
            Console.WriteLine("// Press any key to quit...");
            while (Console.ReadKey().KeyChar == 0) { }
        }

        private static ResponseBodyContent Authorization()
        {
            // Initialize a request to Auth0
            var authUrl = ConfigurationManager.AppSettings["auth_token_url"];
            var client = new RestClient(authUrl);
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");

            // Filling up the request body using configs in app.config
            var content = new RequestBodyContent();
            request.AddParameter("application/json", content.ToJson(), ParameterType.RequestBody);

            // Getting Auth0 access token
            var response = client.Execute(request);
            var responseContent = response.Content.ParseToResponseBodyContent();

            // Output if we got the token from Auth0
            Console.WriteLine($"// Auth0 GET Token Status: {(response.StatusCode == HttpStatusCode.OK ? "Succeeded" : $"Failed ({response.StatusCode})")}");
            Console.WriteLine($"// Auth0 GET Token Result:\n{response.Content}");

            return responseContent;
        }

        private static void ApiGet(ResponseBodyContent authorizationToken)
        {
            // Initialize a GET request to our API
            var apiUrl = ConfigurationManager.AppSettings["api_get_url"];
            var client = new RestClient(apiUrl);
            var request = new RestRequest(Method.GET);

            // Set the token in the authorization header
            // TODO: Don't request a new token every time, save it in your cache.
            request.AddHeader("Authorization", authorizationToken.ToString());

            // GET an object from the API
            var response = client.Execute(request);
            var apiResponseSucceeded = response.StatusCode == HttpStatusCode.OK;

            // Output the api response
            Console.WriteLine($"// API GET Status: {(apiResponseSucceeded ? "Succeeded" : $"Failed ({response.StatusCode})")}");
            if (apiResponseSucceeded)
            {
                Console.WriteLine($"// API GET Result:\n{response.Content}");
            }

            // TODO: Then deserialize the response content JSON to a class.
            // e.g. var myClass = JsonConvert.DeserializeObject<ResponseBodyContent>(response.content);
        }

        private static void ApiPost(ResponseBodyContent authorizationToken)
        {
            // Initialize a POST request to our API
            var apiUrl = ConfigurationManager.AppSettings["api_post_url"];
            var client = new RestClient(apiUrl);
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");

            // Set the token in the authorization header
            // TODO: Don't request a new token every time, save it in your cache.
            request.AddHeader("Authorization", authorizationToken.ToString());

            // Set the request JSON body from a path to test the api
            // TODO: Instead, use a class to serialize it to a JSON object and POST this to the API.
            // e.g. var json = JsonConvert.SerializeObject(myClass);
            var apiPostJsonPath = ConfigurationManager.AppSettings["api_post_json_path"];
            var apiPostJsonBody = !File.Exists(apiPostJsonPath) ? "" : new StreamReader(apiPostJsonPath).ReadToEnd();
            request.AddJsonBody(apiPostJsonBody);

            // POST an object to the API
            var response = client.Execute(request);
            var apiResponseSucceeded = response.StatusCode == HttpStatusCode.OK;

            // Output the api response
            Console.WriteLine($"// API POST Status: {(apiResponseSucceeded ? "Succeeded" : $"Failed ({response.StatusCode})")}");
            if (apiResponseSucceeded)
            {
                Console.WriteLine($"// API POST Result:\n{response.Content}");
            }
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
        public string ExpiresIn { get; set; }

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

        public static ResponseBodyContent ParseToResponseBodyContent(this string value)
        {
            var serializedSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };
            return JsonConvert.DeserializeObject<ResponseBodyContent>(value, serializedSettings);
        }
    }
}
