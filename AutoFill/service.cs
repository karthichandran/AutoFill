using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AutoFill
{
   public class service
    {
        private HttpClient client;
        public service()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44301/api/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public IList<TdsRemittanceDto> GetTdsRemitance(string custName,string premises,string unit,string lot)
        {
            IList<TdsRemittanceDto> remitance = null;
            HttpResponseMessage response = new HttpResponseMessage();

            var query = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(custName))
                query["customerName"] = custName;
            if (!string.IsNullOrEmpty(premises))
                query["PropertyPremises"] = premises;
            if (!string.IsNullOrEmpty(unit))
                query["unitNo"] = unit;
            if (!string.IsNullOrEmpty(lot))
                query["lotNo"] = lot;


            response = client.GetAsync(QueryHelpers.AddQueryString("TdsRemittance", query)).Result;
           
            if (response.IsSuccessStatusCode)
            {
                remitance = response.Content.ReadAsAsync<IList<TdsRemittanceDto>>().Result;
            }
            return remitance;
        }

        public AutoFillDto GetAutoFillData(int clientPaymentTransactionID)
        {

            AutoFillDto autoFillDto = null;
            HttpResponseMessage response = new HttpResponseMessage();
            response = client.GetAsync("AutoFill/"+ clientPaymentTransactionID).Result;
          
            if (response.IsSuccessStatusCode)
            {
                autoFillDto = response.Content.ReadAsAsync<AutoFillDto>().Result;
            }
            return autoFillDto;
        }
    }

    public class TdsRemittanceDto
    {
        public int ClientPaymentTransactionID { get; set; }
        public int ClientPaymentID { get; set; }
        public Guid OwnershipID { get; set; }
        public string PropertyPremises { get; set; }
        public int UnitNo { get; set; }
        public bool TdsCollectedBySeller { get; set; }
        public Guid InstallmentID { get; set; }
        public DateTime? RevisedDateOfPayment { get; set; }
        public DateTime DateOfDeduction { get; set; }
        public string ReceiptNo { get; set; }
        public int LotNo { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal SellerShare { get; set; }
        public string SellerName { get; set; }
        public string CustomerName { get; set; }
        public decimal CustomerShare { get; set; }
        public decimal GstAmount { get; set; }
        public decimal TdsAmount { get; set; }
        public decimal TdsInterest { get; set; }
        public decimal LateFee { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal OwnershipAmount { get; set; }
        public int StatusTypeID { get; set; }
        public decimal GrossShareAmount { get; set; }
        public int RemittanceStatus { get; set; }
    }

    public class AutoFillDto
    {
        public AutoFillDto()
        {
            tab1 = new Tab1();
            tab2 = new Tab2();
            tab3 = new Tab3();
            tab4 = new Tab4();
        }

        public Tab1 tab1 { get; set; }
        public Tab2 tab2 { get; set; }
        public Tab3 tab3 { get; set; }
        public Tab4 tab4 { get; set; }

    }

    public class Tab1
    {
        public string TaxApplicable { get; set; }

        public bool StatusOfPayee { get; set; }


        public string PanOfPayer { get; set; }
        public string PanOfTransferor { get; set; }

    }
    public class Tab2
    {
        //Transferee / buyer
        public string AddressPremisesOfTransferee { get; set; }
        public string AdressLine1OfTransferee { get; set; }
        public string AddressLine2OfTransferee { get; set; }
        public string CityOfTransferee { get; set; }
        public string StateOfTransferee { get; set; }
        public string PinCodeOfTransferee { get; set; }
        public string EmailOfOfTransferee { get; set; }
        public string MobileOfOfTransferee { get; set; }
        public bool IsCoTransferee { get; set; }



        //transferor/seller
        public string AddressPremisesOfTransferor { get; set; }
        public string AddressLine1OfTransferor { get; set; }
        public string AddressLine2OfTransferor { get; set; }
        public string CityOfTransferor { get; set; }
        public string StateOfTransferor { get; set; }
        public string PinCodeOfTransferor { get; set; }
        public string EmailOfOfTransferor { get; set; }
        public string MobileOfOfTransferor { get; set; }
        public bool IsCoTransferor { get; set; }

    }
    /// <summary>
    /// Property Details
    /// </summary>
    public class Tab3
    {
        //Property Details
        public string TypeOfProperty { get; set; }
        public string AddressPremisesOfProperty { get; set; }
        public string AddressLine1OfProperty { get; set; }
        public string AddressLine2OfProperty { get; set; }
        public string CityOfProperty { get; set; }
        public string StateOfProperty { get; set; }
        public string PinCodeOfProperty { get; set; }
        public DatePart DateOfAgreement { get; set; }

        public int TotalAmount { get; set; }
        public int PaymentType { get; set; }
        public PlaceValues AmountPaidParts { get; set; }
        public int AmountPaid { get; set; }
        public Decimal BasicTax { get; set; }
        public Decimal Interest { get; set; }
        public Decimal LateFee { get; set; }


        public Guid OwnershipId { get; set; }
        public Guid InstallmentId { get; set; }
        public int PropertyID { get; set; }
    }

    public class Tab4
    {

        //Payment Info
        public string ModeOfPayment { get; set; }
        public DatePart DateOfPayment { get; set; }
        public DatePart DateOfTaxDeduction { get; set; }
    }
    public class DatePart
    {
        public int Day { get; set; }
        public string Month { get; set; }
        public int Year { get; set; }
    }

    public class PlaceValues
    {
        public int Crores { get; set; }
        public int Lakhs { get; set; }
        public int Thousands { get; set; }
        public int Hundreds { get; set; }
        public int Tens { get; set; }
        public int Ones { get; set; }
    }
}
