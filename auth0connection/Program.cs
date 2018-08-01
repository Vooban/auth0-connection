using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;

namespace auth0connection
{
    internal class Program
    {
        private static readonly HttpStatusCode[] OkStatus = { HttpStatusCode.OK, HttpStatusCode.Accepted, HttpStatusCode.Created, HttpStatusCode.NoContent };

        private static void Main(string[] args)
        {
            var apiGetUrl = ConfigurationManager.AppSettings["api_get_url"];
            var apiPostUrl = ConfigurationManager.AppSettings["api_post_url"];
            var apiPutUrl = ConfigurationManager.AppSettings["api_put_url"];
            var apiDeleteUrl = ConfigurationManager.AppSettings["api_delete_url"];

            var authorizationToken = Authorization();

            if (authorizationToken != null)
            {
                if (!string.IsNullOrEmpty(apiGetUrl))
                {
                    ApiGet(authorizationToken, apiGetUrl);
                }
                if (!string.IsNullOrEmpty(apiPostUrl))
                {
                    ApiPost(authorizationToken, apiPostUrl);
                }
                if (!string.IsNullOrEmpty(apiPutUrl))
                {
                    ApiPut(authorizationToken, apiPutUrl);
                }
                if (!string.IsNullOrEmpty(apiDeleteUrl))
                {
                    ApiDelete(authorizationToken, apiDeleteUrl);
                }
            }

            // Leave console open
            Console.WriteLine("// Press any key to quit...");
            while (Console.ReadKey().KeyChar == 0) { }
        }

        private static ResponseBodyContent Authorization()
        {
            // Initialize a request to Auth0
            var authUrl = ConfigurationManager.AppSettings["auth_token_url"];
            if (string.IsNullOrEmpty(authUrl))
            {
                Console.WriteLine("// No auth0 url set");
                return null;
            }

            var client = new RestClient(authUrl);
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");

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

        private static void ApiGet(ResponseBodyContent authorizationToken, string apiUrl)
        {
            // Initialize a GET request to our API
            var client = new RestClient(apiUrl);
            var request = new RestRequest(Method.GET);

            // Set the token in the authorization header
            // TODO: Don't request a new token every time, save it in your cache.
            request.AddHeader("Authorization", authorizationToken.ToString());

            // GET an object from the API
            var response = client.Execute(request);
            var apiResponseSucceeded = OkStatus.Contains(response.StatusCode);

            // Output the api response
            Console.WriteLine($"// API GET Status: {(apiResponseSucceeded ? "Succeeded" : $"Failed ({response.StatusCode})")}");
            if (!string.IsNullOrEmpty(response.Content))
            {
                Console.WriteLine($"// API GET Result:\n{response.Content}");
            }

            // TODO: Then deserialize the response content JSON to a class.
            // e.g. var myClass = JsonConvert.DeserializeObject<ResponseBodyContent>(response.content);
        }

        private static void ApiPost(ResponseBodyContent authorizationToken, string apiUrl)
        {
            // Initialize a POST request to our API
            var client = new RestClient(apiUrl);
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");

            // Set the token in the authorization header
            // TODO: Don't request a new token every time, save it in your cache.
            request.AddHeader("Authorization", authorizationToken.ToString());

            // Set the request JSON body from a path to test the api
            // TODO: Instead, use a class to serialize it to a JSON object and POST this to the API.
            // e.g. var json = JsonConvert.SerializeObject(myClass);
            var apiPostJsonPath = ConfigurationManager.AppSettings["api_post_json_path"];
            var apiPostJsonBody = !File.Exists(apiPostJsonPath) ? "" : new StreamReader(apiPostJsonPath).ReadToEnd();
            request.AddParameter("Application/Json", apiPostJsonBody, ParameterType.RequestBody);

            // POST an object to the API
            var response = client.Execute(request);
            var apiResponseSucceeded = OkStatus.Contains(response.StatusCode);

            // Output the api response
            Console.WriteLine($"// API POST Status: {(apiResponseSucceeded ? "Succeeded" : $"Failed ({response.StatusCode})")}");
            if (!string.IsNullOrEmpty(response.Content))
            {
                Console.WriteLine($"// API POST Result:\n{response.Content}");
            }
        }

        private static void ApiPut(ResponseBodyContent authorizationToken, string apiUrl)
        {
            // Initialize a PUT request to our API
            var client = new RestClient(apiUrl);
            var request = new RestRequest(Method.PUT);
            request.AddHeader("Content-Type", "application/json");

            // Set the token in the authorization header
            // TODO: Don't request a new token every time, save it in your cache.
            request.AddHeader("Authorization", authorizationToken.ToString());

            // Set the request JSON body from a path to test the api
            // TODO: Instead, use a class to serialize it to a JSON object and PUT this to the API.
            // e.g. var json = JsonConvert.SerializeObject(myClass);
            var apiPutJsonPath = ConfigurationManager.AppSettings["api_put_json_path"];
            var apiPutJsonBody = !File.Exists(apiPutJsonPath) ? "" : new StreamReader(apiPutJsonPath).ReadToEnd();
            request.AddParameter("Application/Json", apiPutJsonBody, ParameterType.RequestBody);

            // PUT an object to the API
            var response = client.Execute(request);
            var apiResponseSucceeded = OkStatus.Contains(response.StatusCode);

            // Output the api response
            Console.WriteLine($"// API PUT Status: {(apiResponseSucceeded ? "Succeeded" : $"Failed ({response.StatusCode})")}");
            if (!string.IsNullOrEmpty(response.Content))
            {
                Console.WriteLine($"// API PUT Result:\n{response.Content}");
            }
        }

        private static void ApiDelete(ResponseBodyContent authorizationToken, string apiUrl)
        {
            // Initialize a DELETE request to our API
            var client = new RestClient(apiUrl);
            var request = new RestRequest(Method.DELETE);

            // Set the token in the authorization header
            // TODO: Don't request a new token every time, save it in your cache.
            request.AddHeader("Authorization", authorizationToken.ToString());

            // DELETE an object from the API
            var response = client.Execute(request);
            var apiResponseSucceeded = OkStatus.Contains(response.StatusCode);

            // Output the api response
            Console.WriteLine($"// API DELETE Status: {(apiResponseSucceeded ? "Succeeded" : $"Failed ({response.StatusCode})")}");
            if (!string.IsNullOrEmpty(response.Content))
            {
                Console.WriteLine($"// API DELETE Result:\n{response.Content}");
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

    // Auth0 Response
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
