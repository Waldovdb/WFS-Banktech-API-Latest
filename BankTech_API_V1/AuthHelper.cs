using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace BankTech_API_V1
{
    public class AuthHelper
    {
        private IConfiguration configuration;
        private static TokenObj token;
        private static string clientKey = "";
        private static string clientSecret = "";

        public AuthHelper(IConfiguration iConfig)
        {
            configuration = iConfig;
        }


        public async Task<string> CheckToken()
        {
            try
            {
                token =
                    JsonConvert.DeserializeObject<TokenObj>(
                        File.ReadAllText(Directory.GetCurrentDirectory() + @"\auth.json"));

                if (token != null)
                {
                    if (token.TokenExpiry < DateTime.Now.Subtract(new TimeSpan(2, 0, 0)))
                    {
                        return await GetToken();
                    }

                    return await RefreshToken();
                }

                return await GetToken();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public async Task<string> GetToken()
        {
            try
            {
                clientKey = configuration.GetSection("MySettings").GetSection("ClientKey").Value;
                clientSecret = configuration.GetSection("MySettings").GetSection("ClientSecret").Value;
            }
            catch (Exception e)
            {
                return "Config File Error: " + e.Message;
            }

            try
            {
                var values = new Dictionary<string, string>
                {
                    { "clientKey", clientKey },
                    { "clientSecret", clientSecret }
                };

                string json = JsonConvert.SerializeObject(values, Newtonsoft.Json.Formatting.None);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = await client.PostAsync("https://api.sandbox.bank.tech/auth/get-token",
                        content);
                    //var response = await client.PostAsync("https://api.bank.tech/auth/get-token",
                    //content);
                    if (response.IsSuccessStatusCode)
                    {
                        AuthResponse authResponse =
                            JsonConvert.DeserializeObject<AuthResponse>(await response.Content.ReadAsStringAsync());
                        token = authResponse.Token;

                        File.WriteAllText(Directory.GetCurrentDirectory() + @"\auth.json",
                            JsonConvert.SerializeObject(token));
                        return token.Token;
                    }

                    return "";
                }
            }
            catch (Exception e)
            {
                return "Internal Token Fetching Error, Exception: " + e.Message;
            }
        }

        public async Task<string> RefreshToken()
        {
            try
            {
                var content = new StringContent("", Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response =
                        await client.PostAsync("https://api.sandbox.bank.tech/auth/refresh-token", content);
                    //var response =
                    //    await client.PostAsync("https://api.bank.tech/auth/refresh-token", content);
                    if (response.IsSuccessStatusCode)
                    {
                        AuthResponse authResponse =
                            JsonConvert.DeserializeObject<AuthResponse>(await response.Content.ReadAsStringAsync());
                        token = authResponse.Token;

                        File.WriteAllText(Directory.GetCurrentDirectory() + @"\auth.json",
                            JsonConvert.SerializeObject(token));
                        return token.Token;
                    }

                    return "";
                }
            }
            catch (Exception e)
            {
                return "Token Refresh Error: " + e.Message;
            }
        }
    }
}