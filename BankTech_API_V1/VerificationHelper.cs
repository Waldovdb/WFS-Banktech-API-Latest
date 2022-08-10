using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BankTech_API_V1.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BankTech_API_V1
{
    public class VerificationHelper
    {
        static FileHelper fileHelper = new FileHelper();
        private string logDirectory = "logs\\Messages\\Log.txt";
        //private readonly ILogger<VerificationHelper> _logger;
        private readonly ILogger logger;
        private static IConfiguration configuration;
        private static AuthHelper authHelper = new(configuration);
        //private static string key = "";

        public VerificationHelper(ILogger logger, IConfiguration iConfig)
        {
            configuration = iConfig;
            this.logger = logger;
        }

        public async Task<VerificationResult> ValidateAccNum(string accNumber, string accType, string branchCode,string authKey)
        {

            //key = await authHelper.CheckToken();

            // Account number validation using Check Digit Validation (CDV) of branch code and account number.
                

            var values = new Dictionary<string, string>
            {
                { "accountNumber", accNumber },
                { "accountType", accType },
                { "branchCode", branchCode },
                { "reference",accNumber }
            };

            string json = JsonConvert.SerializeObject(values, Newtonsoft.Json.Formatting.None);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.PostAsync("https://api.sandbox.bank.tech/verifications/account/validate", content);
                //var response = await client.PostAsync("https://api.bank.tech/verifications/account/validate", content);
                var responseString = await response.Content.ReadAsStringAsync();
                logger.LogInformation("Validate Account Number Response: " + responseString);
                //logDirectory = "logs\\ValidateAcc\\Log.txt";
                fileHelper.EnsureDirectoryExists(logDirectory);
                using (StreamWriter sw = new StreamWriter(logDirectory, append: true))
                {
                    sw.WriteLine("---------------------------------------------------------");
                    sw.WriteLine("Request at: " + DateTime.Now.ToString() + "\n\nMessage From: Validate Account Number \n\nResponse: " + responseString + "\n");
                }

                /*if (response.IsSuccessStatusCode)
                {
                    //ValidateResponse valid = JsonConvert.DeserializeObject<ValidateResponse>(responseString);
                    //return valid.Valid;
                    return response;
                }*/
                dynamic temp = JsonConvert.DeserializeObject(responseString);
                //.success .message
                VerificationResult result = new();
                result.Message = temp.message;
                result.Success = (temp.valid == null) ? temp.success : Convert.ToBoolean(temp.success) & Convert.ToBoolean(temp.valid);
                return result;
                //return false;

            }

        }

        public async Task<VerificationResult> VerifyAccountAVS(string accNumber, string bankName, string accType, string branchCode, string initials, string surname, string idType, string iD,string authKey)
        {
            //key = await authHelper.CheckToken();
            // Account Verification Service Real-time (AVS-R)

            var values = new Dictionary<string, string>
            {
                { "reference", accNumber },
                { "bank", bankName },
                { "branchCode", branchCode },
                { "accountNumber", accNumber },
                { "accountType", accType },
                { "initials", initials },
                { "surname", surname },
                { "identificationType", idType },
                { "identification", iD }
            };

            /*{ "reference", "62393943800" },
            { "bank", "STANDARD" },
            { "branchCode", "250655" },
            { "accountNumber","62393943800" },
            { "accountType","Current" },
            { "initials","N" },
            { "surname","Baker" },
            { "identificationType","ID" },
            { "identification","8206105177088" }
            */

            string json = JsonConvert.SerializeObject(values, Newtonsoft.Json.Formatting.None);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.PostAsync("https://api.sandbox.bank.tech/verifications/account/verify", content);
                //var response = await client.PostAsync("https://api.bank.tech/verifications/account/verify", content);
                var responseString = await response.Content.ReadAsStringAsync();
                logger.LogInformation("Verify Account AVS Response: " + responseString);
                //logDirectory = "logs\\VerifyAVS\\Log.txt";
                fileHelper.EnsureDirectoryExists(logDirectory);
                using (StreamWriter sw = new StreamWriter(logDirectory, append: true))
                {
                    sw.WriteLine("---------------------------------------------------------");
                    sw.WriteLine("Request at: " + DateTime.Now.ToString() + "\n\nMessage from: Verify Account AVS \n\nResponse: " + responseString + "\n");
                }
                /*if (response.IsSuccessStatusCode)
                {
                    //VerifyResponse verified = JsonConvert.DeserializeObject<VerifyResponse>(responseString);
                   // return verified.Valid;
                    //return await response.Content.ReadAsStringAsync();
                    return response;
                }*/
                dynamic temp = JsonConvert.DeserializeObject(responseString);
                //.success .message
                VerificationResult result = new();
                result.Success = Convert.ToBoolean(temp.success) && Convert.ToBoolean(temp.valid);
                if(Convert.ToBoolean(temp.success) && !Convert.ToBoolean(temp.valid))
                {
                    result.Message = "Account Details Incorrect";
                }
                else
                {
                    result.Message = temp.message;
                }
                return result;

                //return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

        }

    }
}
