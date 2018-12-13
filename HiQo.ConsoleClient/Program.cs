using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;

namespace HiQo.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //UseClientCredentials().GetAwaiter().GetResult();
            UsePasswordGrant().GetAwaiter().GetResult();
            Console.ReadKey();
        }

        private static async Task UseClientCredentials()
        {
            var disco =await DiscoveryClient.GetAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            var tokenClient=new TokenClient(disco.TokenEndpoint,"clien","secret");
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            // call api
            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("http://localhost:5001/identity");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }
        }

        private static async Task UsePasswordGrant()
        {
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            var tokenClient=new TokenClient(disco.TokenEndpoint,"ro.client","secret");
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("bob", "password1","api1");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");
        }
    }
}
