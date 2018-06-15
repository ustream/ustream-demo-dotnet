using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace ustream_demo_dotnet
{
    [DataContract]
    class AccessToken
    {
        [DataMember]
        public string access_token;
        [DataMember]
        public string token_type;
        [DataMember]
        public int expires_in;
    }

    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            var token = GetToken(args[0], args[1]).Result;
            Console.WriteLine(
                string.Format("{0} access token is {1}, this token will exire after {2} seconds", token.token_type, token.access_token, token.expires_in)
            );
        }

        private static async Task<AccessToken> GetToken(string clientId, string clientSecret)
        {
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + clientSecret);
            var request = new HttpRequestMessage(HttpMethod.Post, "https://www.ustream.tv/oauth2/token");

            var formParams = new List<KeyValuePair<string, string>>();
            formParams.Add(new KeyValuePair<string, string>("client_id", clientId));
            formParams.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
            formParams.Add(new KeyValuePair<string, string>("token_type", "bearer"));
            formParams.Add(new KeyValuePair<string, string>("scope", "broadcaster"));
            request.Content = new FormUrlEncodedContent(formParams);

            var response = await (await client.SendAsync(request)).Content.ReadAsStreamAsync();
            var serializer = new DataContractJsonSerializer(typeof(AccessToken));
            var token = serializer.ReadObject(response) as AccessToken;
            return token;
        }
    }
}
