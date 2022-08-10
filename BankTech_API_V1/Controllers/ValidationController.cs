using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BankTech_API_V1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValidationController : ControllerBase
    {
        //private readonly ILogger<ValidationController> _logger;
        private readonly ILogger logger;
        private IConfiguration configuration;
        private static string key = "";

        public ValidationController(ILogger<ValidationController> logger, IConfiguration iConfig)
        {
            this.logger = logger;
            configuration = iConfig;
        }


        [HttpPost]
        [Route("ValidateAccount")]
        public async Task<IActionResult> ValidateAndVerify(Verification Details)
        {
            try
            {
                AuthHelper authHelper = new(configuration);
                VerificationHelper verificationHelper = new(logger,configuration);
                key = await authHelper.CheckToken();
                Console.WriteLine(key);

                // if ()
                var validate = await verificationHelper.ValidateAccNum(Details.DebtorAccountNumber,
                    Details.DebtorAccountType,
                    Details.DebtorBranchCode, key);

                if (!validate.Success)
                {
                    //Newtonsoft.Json.Linq.JObject obj2 = Newtonsoft.Json.Linq.JObject.Parse(await validate.Content.ReadAsStringAsync());

                    return Ok(new VerificationResult() { Success = validate.Success, Message = validate.Message});
                }
                // {
                var verification = await verificationHelper.VerifyAccountAVS(Details.DebtorAccountNumber,
                    Details.PreferredBank,
                    Details.DebtorAccountType, Details.DebtorBranchCode,
                    Details.DebtorName.Substring(0, 1), Details.DebtorName.Split(" ")[1],
                    Details.DebtorIdentificationType, Details.DebtorIdentification, key);

                //Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(await verification.Content.ReadAsStringAsync());

                if(!verification.Success)
                {
                    return Ok(new VerificationResult() { Success = false, Message = verification.Message});
                }
                else
                {
                    return Ok(new VerificationResult() { Success = true, Message = verification.Message});
                }
                //}
                //return await new HttpResponseMessage(HttpStatusCode.BadRequest).;
            }


            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}