using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankTech_API_V1
{
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public TokenObj Token { get; set; }

    }

    public class VerifyResponse
    {
        public bool Success { get; set; }
        public string Reference { get; set; }
        public string TraceID { get; set; }
        public bool Valid { get; set; }
        public string Message { get; set; }
        public bool AccountOpen { get; set; }
        public bool AccountFound { get; set; }
        public bool AccountOpenLongerThan3Months { get; set; }
        public bool IDMatched { get; set; }
        public bool NameMatched { get; set; }
        public bool AccountAllowsCredit { get; set; }
        public bool AccountAcceptsCredit { get; set; }
        public bool AccountAllowsDebit { get; set; }
        public bool AccountAcceptsDebit { get; set; }
        public bool EmailAddressMatch { get; set; }
        public bool CellNumberMatch { get; set; }
    }

    public class ValidateResponse
    {
        public bool Success { get; set; }
        public string Reference { get; set; }
        public string TraceID { get; set; }
        public bool Valid { get; set; }
        public string Message { get; set; }
    }
}
