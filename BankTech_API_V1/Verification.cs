using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankTech_API_V1
{
    public class Verification
    {
        public string DebtorAccountType { get; set; }
        public string DebtorPhoneNumber { get; set; }
        public string DebtorAccountNumber { get; set; }
        public string PreferredBank { get; set; }
        public string DebtorBranchCode { get; set; }
        public string DebtorName { get; set; }
        public string DebtorIdentificationType { get; set; }
        public string DebtorIdentification { get; set; }

        public Verification()
        {
        }
    }

    public class VerificationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}