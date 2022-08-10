using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CsvHelper;
using System.Web;
using System.Text.RegularExpressions;
//using System.Web.Http;

namespace BankTech_API_V1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DebitOrderController : ControllerBase
    {
        static FileHelper fileHelper = new FileHelper();
        private string logDirectory = "logs\\Messages\\Log.txt";
        //private readonly ILogger<DebitOrderController> _logger;
        private readonly ILogger<DebitOrderController> _logger;
        private IConfiguration configuration;
        private static string key = "";

        public DebitOrderController(ILogger<DebitOrderController> logger, IConfiguration iConfig)
        {
            this._logger = logger;
            configuration = iConfig;
        }

        [HttpPost]
        [Route("CreateEFTCollections")]
        public async Task<IActionResult> CreateEFTCollection(Collection[] collections)
        {
            /*  Stream req = Request.Body;
              req.Seek(0, System.IO.SeekOrigin.Begin);
              //_logger.LogInformation();
              //HttpContext.Request
              //var rawMessage = await Request.Content.ReadAsStringAsync();
              //logger.LogInformation(RequestLogger.Invoke(this.HttpContext).Result); 

              //string resp = await new StreamReader(Request.HttpContext.Request.Body).ReadToEnd()
              using (System.IO.StreamReader str = new StreamReader(HttpContext.Request.Body))
              {
                  Request.EnableBuffering();
                  Request.Body.Seek(0, SeekOrigin.Begin);
                  string resp = await str.ReadLineAsync();
                  logger.LogInformation("LOGGING BODY: " + resp);
              }
            */

          
            // new System.IO.StreamReader(Request.Body).ReadLineAsync()
            try
            {
                foreach(var item in collections)
                {
                    item.SetDefaults();
                }

               

                AuthHelper authHelper = new(configuration);
                VerificationHelper verificationHelper = new(_logger,configuration);
                List<ValidatedCollection> submittedList = new List<ValidatedCollection>();
                key = await authHelper.CheckToken();

                foreach (Collection col in collections)
                {
                    //var validate = await verificationHelper.ValidateAccNum(col.DebtorAccountNumber, col.DebtorAccountType,
                    //    col.DebtorBranchCode, key);
                    //if (validate.Success)
                    //{
                    //    var verify = await verificationHelper.VerifyAccountAVS(col.DebtorAccountNumber, col.PreferredBank,
                    //        col.DebtorAccountType, col.DebtorBranchCode,
                    //        col.DebtorName.Substring(0, 1), col.DebtorName.Split(" ")[1],
                    //        col.DebtorIdentificationType, col.DebtorIdentification, key);
                    //    if (verify.Success)
                    //    {
                            submittedList.Add(new ValidatedCollection(col.ObjectId ?? 1, col.CollectionReference,
                                col.ContractReference,
                                col.CollectionAmount, col.ForDate, col.RequestedActionDate, col.RequestedSubmissionDate,
                                col.CollectionGrouping,
                                col.AccountingReference, col.ExternalReference, col.PreferredServiceType,
                                col.PreferredBank,
                                col.CreditorAccountNumber,
                                col.CreditorShortName, col.DebtorName, col.DebtorIdentification,
                                col.DebtorIdentificationType, col.DebtorAccountType,
                                col.DebtorPhoneNumber, col.DebtorAccountNumber, col.DebtorBranchCode, col.DebtorEmail,
                                col.IsPreApproved ?? true,
                                true, true));
                    //    }
                    //    else
                    //    {
                    //        return Ok(verify);
                    //    }
                    //}
                    //else
                    //{
                    //    return Ok(validate);
                    //}
                }



                string json = JsonConvert.SerializeObject(submittedList, Formatting.None);
                Console.WriteLine(json);
                bool tempbool = true;
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    try
                    {
                        var response =
                        await client.PostAsync("https://api.sandbox.bank.tech/collections/bulk/eft", content);
                        //await client.PostAsync("https://api.bank.tech/collections/bulk/eft", content);
                        _logger.LogInformation("Create EFT Collections Response: " + response.Content.ReadAsStringAsync());
                        //logDirectory = "logs\\CreateEFTs\\Log.txt";
                        fileHelper.EnsureDirectoryExists(logDirectory);
                        string tempRes = await response.Content.ReadAsStringAsync();
                        using (StreamWriter sw = new StreamWriter(logDirectory, append: true))
                        {
                            sw.WriteLine("---------------------------------------------------------");
                            sw.WriteLine("Request at: " + DateTime.Now.ToString() + "\n\nMessage from: Create EFT Collections \n\nResponse: " + await response.Content.ReadAsStringAsync() + "\n");
                        }
                        dynamic temp = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                        string resMessage = String.Empty;
                        if(temp.success == false)
                        {
                            var validationErrors = JsonConvert.DeserializeObject(temp.validationErrors.ToString());
                            foreach(var item in validationErrors)
                            {
                                resMessage += item.First.First + "; ";
                            }
                            bool tempBool = true;
                            //
                        }
                        else
                        {
                            resMessage = temp.message;
                        }
                        VerificationResult resfinal = new VerificationResult() { Message = resMessage, Success = temp.success };
                        return Ok(resfinal);
                    }
                    catch (Exception ex)
                    {
                        //
                        return BadRequest(ex.Message);
                    }
                }
            }
            catch (Exception e)
            {
                return BadRequest("Error creating collection: " + e.Message);
            }


            /*Collection[] cols = new Collection[]
      {
          new Collection(1, "Ref1", "Ref2", 1,
              "2022-02-26", "2022-02-28", "2022-02-28", "Group 1",
              "AccRef1", "ExtRef1", "EFT_0DAY", "ABSA",
              "4099441627", "WFS CC DO", "Nathan Baker", "8206105177088",
              "ID", "Current", "27735791006", "62393943800", "250655",
              "debtor@email.com", true, true, true)
      };*/


            /*int objID, string collectionRef,string contractRef, int collectionAmount, string forDate,
            string reqActionDate,string reqSubmissionDate, string collectionGrouping,
            string accountingRef, string externalRef, string prefServiceType, string prefBank,
            string creditorAccNum, string creditorShortName, string debtorName, string debtorID,
            string debtorIDType,string debtorAccType, string debtorPhoneNum, string debtorAccNum,
            string debtorBranchCode, string debtorEmail,bool isPreApproved,bool isAccValidated,
            bool isAccVerified*/

            /*1, "Ref1", "Ref2", 1,
            "2022-02-26", "2022-02-28", "2022-02-28", "Group 1",
            "AccRef1", "ExtRef1", "EFT_0DAY", "ABSA",
            "4099441627", "WFS CC DO", "Nathan Baker", "8206105177088",
            "ID", "Current", "27735791006", "62393943800", "250655",
            "debtor@email.com", true, true, true*/
        }

        [HttpGet]
        [Route("GetEFTCollections")]
        public async Task<IActionResult> GetEFTCollections()
        {
            try
            {
                AuthHelper authHelper = new(configuration);
                key = await authHelper.CheckToken();
            }
            catch (Exception e)
            {
                return BadRequest("Authentication error: " + e.Message);
            }

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = await client.GetAsync("https://api.sandbox.bank.tech/collections");
                    //var response = await client.GetAsync("https://api.bank.tech/collections");
                    var responseString = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Get EFT Collections Response: " + responseString);
                    //logDirectory = "logs\\GetEFTs\\Log.txt";
                    fileHelper.EnsureDirectoryExists(logDirectory);
                    using (StreamWriter sw = new StreamWriter(logDirectory, append: true))
                    {
                        sw.WriteLine("---------------------------------------------------------");
                        sw.WriteLine("Request at: " + DateTime.Now.ToString() + "\n\nMessage from: Get EFT Collections \n\nResponse: " + responseString + "\n");
                    }
                    CollectionArr collections =
                        JsonConvert.DeserializeObject<CollectionArr>(await response.Content.ReadAsStringAsync());
                    var collectionsList = collections.Collections.ToList();

                    using (var writer = new StreamWriter("C:/0.InovoCIM/x.CollectionsFiles/Collections.csv"))
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        csv.WriteRecords(collectionsList);
                    }

                    return Ok(JsonConvert.SerializeObject(responseString));
                }
            }
            catch (Exception e)
            {
                return BadRequest("Error with fetching collections: " + e.Message);
            }
        }
    }
}


/*{ "objectId", "1" },
{ "collectionReference", "Ref123" },
{ "contractReference", "Ref456" },
{ "collectionAmount", "159.53" },
{ "forDate", "2022-02-12" },
{ "requestedActionDate", "2022-02-12" },
{ "requestedSubmissionDate", "2022-02-10" },
{ "collectionGrouping", "Group 23" },
{ "accountingReference", "AccRef12" },
{ "externalReference", "ExtRef56" },
{ "preferredServiceType", "EFT_2DAY" },
{ "preferredBank", "ABSA" },
{ "creditorAccountNumber", "8075486669" },
{ "creditorShortName", "BANKTECH" },
{ "debtorName", "John Doe" },
{ "debtorIdentification", "9907095671609" },
{ "debtorIdentificationType", "ID" },
{ "debtorAccountType", "CURRENT" },
{ "debtorPhoneNumber", "27990790567" },
{ "debtorAccountNumber", "6355954392" },
{ "debtorBranchCode", "632005" },
{ "debtorEmail", "debtor@email.com" },
{ "isPreApproved", "true" }*/