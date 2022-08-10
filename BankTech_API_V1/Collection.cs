using System;
using System.Collections;

namespace BankTech_API_V1
{
    public class CollectionArr// : IEnumerable
    {
        public ReceivedCollection[] Collections { get; set; }

        /*public IEnumerator GetEnumerator()
        {
            return (IEnumerator)this;
        }*/

        /*public CollectionArr(ReceivedCollection[] collections)
        {
            Collections = collections;
        }*/
    }

    /*"id":3010866,"collectionIdentification":"0E4B4B3097086535A0DB8219FA6615CE","contractReference":"Ref2","collectionReference":"Ref1",
    "currency":"ZAR","collectionAmount":1.00,"accountingReference":"AccRef1","externalReference":"ExtRef1","forDate":"2022-02-26T00:00:00",
    "requestedActionDate":"2022-02-28T00:00:00","collectionGrouping":"Group 1","collectionStatus":"Approved","creditorUltimateShortName":"WFS CC DO",
    "creditorAccountNumber":"4099441627","debtorAccountValidated":true,"debtorAccountVerified":true,"recalled":false,"collectionInstrument":"EFT",
    "trackingPeriod":"","submissionDate":"2022-02-28T00:00:00","actionDate":"2022-02-28T00:00:00","disputed":false*/

    public class ReceivedCollection
    {
        public int ID { get; set; }
        //public int MandateID { get; set; }
        public string CollectionIdentification { get; set; }
        public string ContractReference { get; set; }
        public string CollectionReference { get; set; }
        public string Currency { get; set; }
        public decimal CollectionAmount { get; set; }
        //public string InstalmentOccurrence { get; set; }
        public string AccountingReference { get; set; }
        public string ExternalReference { get; set; }
        public string ForDate { get; set; }
        public string RequestedActionDate { get; set; }
        public string CollectionGrouping { get; set; }
        public string CollectionStatus { get; set; }
        public string CreditorUltimateShortName { get; set; }
        public string CreditorAccountNumber { get; set; }
        //public string ResultType { get; set; }
        public bool DebtorAccountValidated { get; set; }
        public bool DebtorAccountVerified { get; set; }
        public bool Recalled { get; set; }
        public string CollectionInstrument { get; set; }
        public string TrackingPeriod { get; set; }
        public string SubmissionDate { get; set; }
        public string ActionDate { get; set; }
        //public string ResultCode { get; set; }
        //public string ResultDate { get; set; }
        //public string ProcessingDate { get; set; }
        public bool Disputed { get; set; }
        //public string DisputedDate { get; set; }
    }

    public class Collection
    {
        public int? ObjectId { get; set; }
        public string CollectionReference { get; set; }
        public string ContractReference { get; set; }
        public decimal CollectionAmount { get; set; }
        public string ForDate { get; set; }
        public string RequestedActionDate { get; set; }
        public string RequestedSubmissionDate { get; set; }
        public string CollectionGrouping { get; set; }
        public string AccountingReference { get; set; }
        public string ExternalReference { get; set; }
        public string PreferredServiceType { get; set; }
        public string PreferredBank { get; set; }
        public string CreditorAccountNumber { get; set; }
        public string CreditorShortName { get; set; }
        public string DebtorName { get; set; }
        public string DebtorIdentification { get; set; }
        public string DebtorIdentificationType { get; set; }
        public string DebtorAccountType { get; set; }
        public string DebtorPhoneNumber { get; set; }
        public string DebtorAccountNumber { get; set; }
        public string DebtorBranchCode { get; set; }
        public string DebtorEmail { get; set; }
        public bool? IsPreApproved { get; set; }

       public void SetDefaults()
        {
            this.IsPreApproved = true;

            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            int secondsSinceEpoch = (int)t.TotalSeconds;
            this.ObjectId = secondsSinceEpoch;
            //this.CollectionReference = "Ref" + ObjectId.ToString();
            //this.ContractReference = "Ref" + (ObjectId + 1).ToString();
            this.CollectionGrouping = "Group 1";
            this.AccountingReference = "AccRef1";
            this.ExternalReference = "ExtRef1";
            this.PreferredServiceType = "EFT_0DAY";
            this.PreferredBank = "ABSA";
            this.CreditorAccountNumber = "4099441627";
            this.CreditorShortName = (this.CollectionReference.Substring(0,6) == "600785") ? "WFS SC/PL" : "WFS CC DO";
            this.CollectionReference = (this.CollectionReference.Length > 14) ? Right(this.CollectionReference, 14) : this.CollectionReference;
            this.ContractReference = (this.ContractReference.Length > 14) ? Right(this.ContractReference, 14) : this.ContractReference;
            DateTime temp = Convert.ToDateTime(this.ForDate);
            temp = (temp <= DateTime.Now && temp.Date == DateTime.Now.Date) ? DateTime.Now.AddHours(1) : temp;
            this.ForDate = temp.ToString();
            temp = Convert.ToDateTime(this.RequestedActionDate);
            temp = (temp <= DateTime.Now && temp.Date == DateTime.Now.Date) ? DateTime.Now.AddHours(1) : temp;
            this.RequestedActionDate = temp.ToString();
            this.GetTokenReference();
        }

        public static string Right(string str, int length)
        {
            return str.Substring(str.Length - length, length);
        }

        public void GetTokenReference()
        {
            if(this.CollectionReference.Substring(0,4) == "0375" || this.CollectionReference.Substring(0, 4) == "0154")
            {
                this.CollectionReference = "41" + this.CollectionReference;
            }
            if(this.CollectionReference.Substring(0,4) == "0785" || this.CollectionReference.Substring(0, 4) == "0154")
            {
                this.CollectionReference = "60" + this.CollectionReference;
            }

            if (this.CollectionReference.Substring(0, 6) == "400154" || this.CollectionReference.Substring(0, 6) == "410374" || this.CollectionReference.Substring(0, 6) == "410375" || this.CollectionReference.Substring(0, 6) == "600785")
            {
                this.CollectionReference = this.CollectionReference.Substring(2);
            }

        }



        public Collection()
        {
            
        }

    }

    public class ValidatedCollection : Collection
    {
        public bool DebtorAccountValidated { get; set; }
        public bool DebtorAccountVerified { get; set; }

        public ValidatedCollection(int objectId, string collectionReference, string contractReference, decimal collectionAmount, string forDate, string requestedActionDate, string requestedSubmissionDate, string collectionGrouping, string accountingReference, string externalReference, string preferredServiceType, string preferredBank, string creditorAccountNumber, string creditorShortName, string debtorName, string debtorIdentification, string debtorIdentificationType, string debtorAccountType, string debtorPhoneNumber, string debtorAccountNumber, string debtorBranchCode, string debtorEmail, bool isPreApproved, bool debtorAccountValidated, bool debtorAccountVerified)
        {
            ObjectId = objectId;
            CollectionReference = collectionReference;
            ContractReference = contractReference;
            CollectionAmount = collectionAmount;
            ForDate = forDate;
            RequestedActionDate = requestedActionDate;
            RequestedSubmissionDate = requestedSubmissionDate;
            CollectionGrouping = collectionGrouping;
            AccountingReference = accountingReference;
            ExternalReference = externalReference;
            PreferredServiceType = preferredServiceType;
            PreferredBank = preferredBank;
            CreditorAccountNumber = creditorAccountNumber;
            CreditorShortName = creditorShortName;
            DebtorName = debtorName;
            DebtorIdentification = debtorIdentification;
            DebtorIdentificationType = debtorIdentificationType;
            DebtorAccountType = debtorAccountType;
            DebtorPhoneNumber = debtorPhoneNumber;
            DebtorAccountNumber = debtorAccountNumber;
            DebtorBranchCode = debtorBranchCode;
            DebtorEmail = debtorEmail;
            IsPreApproved = isPreApproved;
            DebtorAccountValidated = debtorAccountValidated;
            DebtorAccountVerified = debtorAccountVerified;
        }
    }
}
