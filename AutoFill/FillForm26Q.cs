using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace AutoFill
{
    public class FillForm26Q :Base
    {
       
       static BankAccountDetailsDto _bankLogin;       
        public static bool AutoFillForm26QB(AutoFillDto autoFillDto,string tds,string interest,string lateFee, BankAccountDetailsDto bankLogin,string transID)
        {
            try
            {
                _bankLogin = bankLogin;// rgan31
               // _bankLogin =new BankAccountDetailsDto{ UserName="reprosri",UserPassword="Repro&123"}; // Note : sri ram account
               // _bankLogin = new BankAccountDetailsDto { UserName = "579091011.RGANESH", UserPassword = "Rajalara@123" }; 
               // _bankLogin = new BankAccountDetailsDto { UserName = "579091011.VIJAYALA", UserPassword = "Sriram@123" }; 

                var driver = GetChromeDriver();
                // var driver = new ChromeDriver(AppDomain.CurrentDomain.BaseDirectory, options);
                //var driver = new ChromeDriver(options);
                //driver.Manage().Window.Maximize();
                driver.Navigate().GoToUrl("https://onlineservices.tin.egov-nsdl.com/etaxnew/tdsnontds.jsp");
                WaitForReady(driver);
                // var proceedBtn = driver.FindElement(By.XPath("//a[@href='javascript:sendRequest(\'PropertyTaxForm\');']"));
                driver.FindElement(By.XPath("//*[@id='selectform']/div[3]/div[1]/section/div/div/a")).Click(); //todo improve xpath


                WaitForReady(driver);
                FillTaxPayerInfo(driver, autoFillDto.tab1);

                WaitForReady(driver);
                FillAddress(driver, autoFillDto.tab2);

                WaitForReady(driver);
                FillPropertyinfo(driver, autoFillDto.tab3);

                WaitForReady(driver);
                FillPaymentinfo(driver, autoFillDto.tab4);

                WaitForReady(driver);
                ProcessToBank(driver, tds, interest, lateFee, transID);

                driver.Quit();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                MessageBox.Show("Processing Form26QB Failed");
                return false;
               // throw;
            }
        }

        //Note : both autofillform26q should be same functionality
        public static bool AutoFillForm26QB_NoMsg(AutoFillDto autoFillDto, string tds, string interest, string lateFee, BankAccountDetailsDto bankLogin, string transID)
        {
            try
            {
                _bankLogin = bankLogin;
                                      

                var driver = GetChromeDriver();
                driver.Navigate().GoToUrl("https://onlineservices.tin.egov-nsdl.com/etaxnew/tdsnontds.jsp");
                WaitForReady(driver);
                driver.FindElement(By.XPath("//*[@id='selectform']/div[3]/div[1]/section/div/div/a")).Click(); //todo improve xpath

                WaitForReady(driver);
                FillTaxPayerInfo(driver, autoFillDto.tab1);

                WaitForReady(driver);
                FillAddress(driver, autoFillDto.tab2);

                WaitForReady(driver);
                FillPropertyinfo(driver, autoFillDto.tab3);

                WaitForReady(driver);
                FillPaymentinfo(driver, autoFillDto.tab4);

                WaitForReady(driver);
                ProcessToBank(driver, tds, interest, lateFee, transID);
                driver.Quit();
                return true;
            }
            catch (Exception e)
            {
                return false;
                // throw;
            }
        }

        private static void FillTaxPayerInfo(IWebDriver webDriver, Tab1 tab1)
        {
            var taxApplicable = webDriver.FindElement(By.Id(tab1.TaxApplicable));
            taxApplicable.Click();

            var resident = "";
            if (tab1.StatusOfPayee)
                resident = "Indian";
            else
                resident = "NRI";
            var statusOfPayer = webDriver.FindElement(By.Id(resident));
            statusOfPayer.Click();

            var pan = webDriver.FindElement(By.Id("PAN_purchaser"));
            pan.SendKeys(tab1.PanOfPayer);
            var confirmPan = webDriver.FindElement(By.Id("ConfirmTransferee"));
            confirmPan.SendKeys(tab1.PanOfPayer);
            var sellerPan = webDriver.FindElement(By.Id("PAN_seller"));
            sellerPan.SendKeys(tab1.PanOfTransferor);
            var confirmSellerPan = webDriver.FindElement(By.Id("ConfirmTransferers"));
            confirmSellerPan.SendKeys(tab1.PanOfTransferor);
            WaitForReady(webDriver);

            webDriver.FindElement(By.XPath("//a[@href='#next']")).Click();
        }

        private static void FillAddress(IWebDriver webDriver, Tab2 tab2)
        {
            if (tab2.AddressPremisesOfTransferee != "" && tab2.AddressPremisesOfTransferee != null)
            {
                var address1 = webDriver.FindElement(By.Id("Add_Line2"));
                address1.SendKeys(tab2.AddressPremisesOfTransferee);
            }
            if (tab2.AdressLine1OfTransferee != "" && tab2.AdressLine1OfTransferee != null)
            {
                var flot = webDriver.FindElement(By.Name("Add_Line1"));
                flot.SendKeys(tab2.AdressLine1OfTransferee);
            }
            if (tab2.AddressLine2OfTransferee != "" && tab2.AddressLine2OfTransferee != null)
            {
                var road = webDriver.FindElement(By.Name("Add_Line3"));
                road.SendKeys(tab2.AddressLine2OfTransferee);
            }
            var city = webDriver.FindElement(By.Name("Add_Line5"));
            city.SendKeys(tab2.CityOfTransferee);
            var state = webDriver.FindElement(By.Name("Add_State"));
            var stateDDl = new SelectElement(state);
            stateDDl.SelectByText(tab2.StateOfTransferee);
            var pin = webDriver.FindElement(By.Name("Add_PIN"));
            pin.SendKeys(tab2.PinCodeOfTransferee);
            var email = webDriver.FindElement(By.Name("Add_EMAIL"));
            email.SendKeys(tab2.EmailOfOfTransferee);
            if (tab2.MobileOfOfTransferee != "" && tab2.MobileOfOfTransferee != null)
            {
                var mobile = webDriver.FindElement(By.Name("Add_MOBILE"));
                mobile.SendKeys(tab2.MobileOfOfTransferee);
            }
            var moreThanOeBuyer = "";
            if (tab2.IsCoTransferee)
                moreThanOeBuyer = "Yes";
            else moreThanOeBuyer = "No";
            var coBuyer = webDriver.FindElement(By.Name("Buyer"));
            var coBuyerDDl = new SelectElement(coBuyer);
            coBuyerDDl.SelectByText(moreThanOeBuyer);


            if (tab2.AddressPremisesOfTransferor != "" && tab2.AddressPremisesOfTransferor != null)
            {
                var address1Trans = webDriver.FindElement(By.Name("transferer_Add_Line2"));
                address1Trans.SendKeys(tab2.AddressPremisesOfTransferor);
            }
            if (tab2.AddressLine1OfTransferor != "" && tab2.AddressLine1OfTransferor != null)
            {
                var flotTrans = webDriver.FindElement(By.Name("transferer_Add_Line1"));
                flotTrans.SendKeys(tab2.AddressLine1OfTransferor);
            }
            if (tab2.AddressLine2OfTransferor != "" && tab2.AddressLine2OfTransferor != null)
            {
                var roadTrans = webDriver.FindElement(By.Name("transferer_Add_Line3"));
                roadTrans.SendKeys(tab2.AddressLine2OfTransferor);
            }
            var cityTrans = webDriver.FindElement(By.Name("transferer_Add_Line5"));
            cityTrans.SendKeys(tab2.CityOfTransferor);
            var stateTrans = webDriver.FindElement(By.Name("transferer_Add_State"));
            var stateDDlTrans = new SelectElement(stateTrans);
            stateDDlTrans.SelectByText(tab2.StateOfTransferor);
            var pinTrans = webDriver.FindElement(By.Name("transferer_Add_PIN"));
            pinTrans.SendKeys(tab2.PinCodeOfTransferor);
            if (tab2.EmailOfOfTransferor != "" && tab2.EmailOfOfTransferor != null)
            {
                var emailTrans = webDriver.FindElement(By.Name("transferer_Add_EMAIL"));
                emailTrans.SendKeys(tab2.EmailOfOfTransferor);
            }
            if (tab2.MobileOfOfTransferor != "" && tab2.MobileOfOfTransferor != null)
            {
                var mobiletrans = webDriver.FindElement(By.Name("transferer_Add_MOBILE"));
                mobiletrans.SendKeys(tab2.MobileOfOfTransferor);
            }

            var sellerOpt = "";
            if (tab2.IsCoTransferor)
                sellerOpt = "Yes";
            else sellerOpt = "No";

            var Seller = webDriver.FindElement(By.Name("Seller"));
            var SellerDDl = new SelectElement(Seller);
            SellerDDl.SelectByText(sellerOpt);

            webDriver.FindElement(By.XPath("//a[@href='#next']")).Click();
        }

        private static void FillPropertyinfo(IWebDriver webDriver, Tab3 tab3)
        {
            var propType = "";
            if (tab3.TypeOfProperty == "1")
                propType = "Land";
            else
                propType = "Building";
            var property = webDriver.FindElement(By.Name("propertyType"));
            var propertyDDl = new SelectElement(property);
            propertyDDl.SelectByValue(propType);

            if (tab3.AddressPremisesOfProperty != "" && tab3.AddressPremisesOfProperty != null)
            {
                var address1 = webDriver.FindElement(By.Name("p_Add_Line2"));
                address1.SendKeys(tab3.AddressPremisesOfProperty);
            }
            if (tab3.AddressLine1OfProperty != "" && tab3.AddressLine1OfProperty != null)
            {
                var flot = webDriver.FindElement(By.Name("p_Add_Line1"));// Note : this should be hide  pased on property type
                flot.SendKeys(tab3.AddressLine1OfProperty);
            }
            if (tab3.AddressLine2OfProperty != "" && tab3.AddressLine2OfProperty != null)
            {
                var road = webDriver.FindElement(By.Name("p_Add_Line3"));
                road.SendKeys(tab3.AddressLine2OfProperty);
            }
            var city = webDriver.FindElement(By.Name("p_Add_Line5"));
            city.SendKeys(tab3.CityOfProperty);
            var state = webDriver.FindElement(By.Name("p_Add_State"));
            var stateDDl = new SelectElement(state);
            stateDDl.SelectByText(tab3.StateOfProperty);
            var pin = webDriver.FindElement(By.Name("p_Add_PIN"));
            pin.SendKeys(tab3.PinCodeOfProperty);

            var day = webDriver.FindElement(By.Name("agmt_day"));
            day.Click();
            var dayDDl = new SelectElement(day);
            var opts = dayDDl.Options;
            var daysOpt = dayDDl.Options.Where(x => x.Text.Trim() == tab3.DateOfAgreement.Day.ToString()).FirstOrDefault();
            dayDDl.SelectByText(daysOpt.Text);

            var month = webDriver.FindElement(By.Name("agmt_month"));
            var monthDDl = new SelectElement(month);
            var monthOpt = monthDDl.Options.Where(x => x.Text.Trim().ToLower() == tab3.DateOfAgreement.Month.ToLower()).FirstOrDefault();
            monthDDl.SelectByText(monthOpt.Text);

            var year = webDriver.FindElement(By.Name("agmt_year"));
            var yearDDl = new SelectElement(year);
            var yearOpt = yearDDl.Options.Where(x => x.Text.Trim() == tab3.DateOfAgreement.Year.ToString()).FirstOrDefault();
            yearDDl.SelectByText(yearOpt.Text);

            var totalValue = webDriver.FindElement(By.Name("totalPropertyValue"));
            totalValue.SendKeys(tab3.TotalAmount.ToString());

            var stampDutyValue = webDriver.FindElement(By.Id("stampdutyvalue"));
            stampDutyValue.SendKeys(tab3.StampDuty.ToString());

            var paymentType = webDriver.FindElement(By.Name("paymentType"));
            var paymentTypeDDl = new SelectElement(paymentType);
            paymentTypeDDl.SelectByIndex(tab3.PaymentType);


            var paymentDay = webDriver.FindElement(By.Name("pymntDay"));
            paymentDay.Click();
            var paymentDayDDl = new SelectElement(paymentDay);         
            var paymentdaysOpt = paymentDayDDl.Options.Where(x => x.Text.Trim() == tab3.RevisedDateOfPayment.Day.ToString()).FirstOrDefault();
            paymentDayDDl.SelectByText(paymentdaysOpt.Text);

            var paymentMonth = webDriver.FindElement(By.Name("pymntMonth"));
            var paymentMonthDDl = new SelectElement(paymentMonth);
            var payMonthOpt = paymentMonthDDl.Options.Where(x => x.Text.Trim().ToLower() == tab3.RevisedDateOfPayment.Month.ToLower()).FirstOrDefault();
            paymentMonthDDl.SelectByText(payMonthOpt.Text);

            var paymentyear = webDriver.FindElement(By.Name("pymntYear"));
            var paymentyearDDl = new SelectElement(paymentyear);
            var paymentyearOpt = paymentyearDDl.Options.Where(x => x.Text.Trim() == tab3.RevisedDateOfPayment.Year.ToString()).FirstOrDefault();
            paymentyearDDl.SelectByText(paymentyearOpt.Text);

            var deductionDay = webDriver.FindElement(By.Name("deductionDay"));
            deductionDay.Click();
            var deductionDDl = new SelectElement(deductionDay);
            var deductiondaysOpt = deductionDDl.Options.Where(x => x.Text.Trim() == tab3.DateOfDeduction.Day.ToString()).FirstOrDefault();
            deductionDDl.SelectByText(deductiondaysOpt.Text);

            var deductionMonth = webDriver.FindElement(By.Name("deductionMonth"));
            var deductionMonthDDl = new SelectElement(deductionMonth);
            var deductionMonthOpt = deductionMonthDDl.Options.Where(x => x.Text.Trim().ToLower() == tab3.DateOfDeduction.Month.ToLower()).FirstOrDefault();
            deductionMonthDDl.SelectByText(deductionMonthOpt.Text);

            var deductionyear = webDriver.FindElement(By.Name("deductionYear"));
            var deductionyearDDl = new SelectElement(deductionyear);
            var deductionyearOpt = deductionyearDDl.Options.Where(x => x.Text.Trim() == tab3.DateOfDeduction.Year.ToString()).FirstOrDefault();
            deductionyearDDl.SelectByText(deductionyearOpt.Text);

            var higherRate = webDriver.FindElement(By.Id("tds_higher_rate"));
            if (higherRate.Displayed)
            {
                var higherRateDDl = new SelectElement(higherRate);
                higherRateDDl.SelectByText("No");

            }
            // AssignAmount(webDriver, "111111111");
            var ones = webDriver.FindElement(By.Name("Ones"));
            var onesDDl = new SelectElement(ones);
            onesDDl.SelectByText(tab3.AmountPaidParts.Ones.ToString());

            var ten = webDriver.FindElement(By.Name("Tens"));
            var tenDDl = new SelectElement(ten);
            tenDDl.SelectByText(tab3.AmountPaidParts.Tens.ToString());

            var hundreds = webDriver.FindElement(By.Name("Hundreds"));
            var hundredsDDl = new SelectElement(hundreds);
            hundredsDDl.SelectByText(tab3.AmountPaidParts.Hundreds.ToString());

            var thousands = webDriver.FindElement(By.Name("Thousands"));
            var thousandsDDl = new SelectElement(thousands);
            thousandsDDl.SelectByText(tab3.AmountPaidParts.Thousands.ToString());

            var lakh = webDriver.FindElement(By.Name("Lakh"));
            var lakhDDl = new SelectElement(lakh);
            lakhDDl.SelectByText(tab3.AmountPaidParts.Lakhs.ToString());

            var crores = webDriver.FindElement(By.Name("Crores"));
            var croresDDl = new SelectElement(crores);
            croresDDl.SelectByText(tab3.AmountPaidParts.Crores.ToString());

            var totalpPaid = webDriver.FindElement(By.Name("value_entered_user"));
            totalpPaid.SendKeys(tab3.AmountPaid.ToString());

            var tdsAmount = webDriver.FindElement(By.Name("TDS_amt"));
            tdsAmount.SendKeys(tab3.BasicTax.ToString());
            if (tab3.Interest != 0)
            {
                var interest = webDriver.FindElement(By.Name("interest"));
                interest.SendKeys(tab3.Interest.ToString());
            }
            if (tab3.LateFee != 0)
            {
                var fee = webDriver.FindElement(By.Name("fee"));
                fee.SendKeys(tab3.LateFee.ToString());
            }
            webDriver.FindElement(By.XPath("//a[@href='#next']")).Click();
        }

        private static void FillPaymentinfo(IWebDriver webDriver, Tab4 tab4)
        {
            var modePofPay = "";
            if (tab4.ModeOfPayment == "modeBankSelection")
                modePofPay = "onlineRadio";
            else
                modePofPay = "offlineRadio";
            var address1 = webDriver.FindElement(By.Id(modePofPay));
            address1.Click();
            if (modePofPay == "onlineRadio")
            {
                var bank = webDriver.FindElement(By.Id("NetBank_Name_c"));
                var bankDDl = new SelectElement(bank);
                bankDDl.SelectByText("ICICI Bank");
            }
            var captcha = ReadCaptcha(webDriver, "Captcha");
            if (captcha == "")
            {
                MessageBoxResult result = MessageBox.Show("Please fill the captcha and press OK button.", "Confirmation",
                                                     MessageBoxButton.OK, MessageBoxImage.Asterisk,
                                                     MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
            }
            else
            {
                var captchaInput = webDriver.FindElement(By.Name("captchaText"));
                captchaInput.SendKeys(captcha);
            }


           
            var proceedBtn = webDriver.FindElement(By.XPath("//a[@href='#finish']"));
            proceedBtn.Click();
            WaitFor(webDriver, 2);
            var DatePopupBtn = webDriver.FindElements(By.XPath("//button[@data-dismiss='modal' and contains(.,'OK')]"));
            if (DatePopupBtn!=null && DatePopupBtn.Count > 0)
            {
                DatePopupBtn[0].Click();
            }
            WaitForReady(webDriver);
            WaitFor(webDriver, 2);
            var confirmCheck = webDriver.FindElement(By.Id("consentCheck"));
            confirmCheck.Click();
            var confirmBtn = webDriver.FindElement(By.Id("Submit"));
            confirmBtn.Click();
            WaitForReady(webDriver);
            WaitFor(webDriver, 3);
            //  new WebDriverWait(webDriver, TimeSpan.FromSeconds(60)).Until(ExpectedConditions.ElementExists(By.XPath("//button[@data-dismiss='modal']")));
            var closeBtn = webDriver.FindElement(By.XPath("//button[@data-dismiss='modal']"));
            closeBtn.Click();
            WaitForReady(webDriver);
            WaitFor(webDriver, 3);
            var submitToBankBtn = webDriver.FindElement(By.Id("Submit"));
            submitToBankBtn.Click();
            WaitForReady(webDriver);
            //var day = webDriver.FindElement(By.Name("pymntDay"));
            //day.Click();
            //var dayDDl = new SelectElement(day);
            //dayDDl.SelectByText(tab4.DateOfPayment.Day.ToString());
            //var month = webDriver.FindElement(By.Name("pymntMonth"));
            //var monthDDl = new SelectElement(month);
            //monthDDl.SelectByText(tab4.DateOfPayment.Month.ToString());
            //var year = webDriver.FindElement(By.Name("pymntYear"));
            //var yearDDl = new SelectElement(year);
            //yearDDl.SelectByText(tab4.DateOfPayment.Year.ToString());

            //var dayDeduction = webDriver.FindElement(By.Name("deductionDay"));
            //dayDeduction.Click();
            //var dayDeductionDDl = new SelectElement(dayDeduction);
            //dayDeductionDDl.SelectByText(tab4.DateOfTaxDeduction.Day.ToString());
            //var monthDeduction = webDriver.FindElement(By.Name("deductionMonth"));
            //var monthDeductionDDl = new SelectElement(monthDeduction);
            //monthDeductionDDl.SelectByText(tab4.DateOfTaxDeduction.Month.ToString());
            //var yearDeduction = webDriver.FindElement(By.Name("deductionYear"));
            //var yearDeductionDDl = new SelectElement(yearDeduction);
            //yearDeductionDDl.SelectByText(tab4.DateOfTaxDeduction.Year.ToString());

        }

        private static void ProcessToBank(IWebDriver webDriver,string tds,string interest,string lateFee,string transId) {
            if (_bankLogin == null)
            {
               var result = MessageBox.Show("Bank login details not available", "Confirmation",
                                                        MessageBoxButton.OK, MessageBoxImage.Asterisk,
                                                        MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            WaitFor(webDriver,3);

            var payBtn = webDriver.FindElement(By.Id("CIB_11X_PROCEED"));
            payBtn.Click();
            WaitForReady(webDriver);
            WaitFor(webDriver,3);
            var userIdTxt= webDriver.FindElement(By.Id("login-step1-userid"));
            userIdTxt.SendKeys(_bankLogin.UserName);
            var pwdTxt = webDriver.FindElement(By.Id("AuthenticationFG.ACCESS_CODE"));
            pwdTxt.SendKeys(_bankLogin.UserPassword);
            var proceedBtn = webDriver.FindElement(By.Id("VALIDATE_CREDENTIALS1"));
            proceedBtn.Click();
            WaitForReady(webDriver);
            var incomeTaxTxt = webDriver.FindElement(By.Id("TranRequestManagerFG.TAX_AMOUNT_STR"));
            incomeTaxTxt.SendKeys(tds);
            WaitFor(webDriver, 1);
            if (!string.IsNullOrEmpty(interest)) {
                var interestTxt = webDriver.FindElement(By.Id("TranRequestManagerFG.INTEREST_AMOUNT_STR"));
                interestTxt.SendKeys(interest);
            }
            WaitFor(webDriver, 1);
            if (!string.IsNullOrEmpty(lateFee)) {
                var feeTxt = webDriver.FindElement(By.Id("TranRequestManagerFG.OTHER_FEE_AMT_STR"));
                feeTxt.SendKeys(lateFee);
            }

            WaitFor(webDriver, 1);
            if (!string.IsNullOrEmpty(transId))
            {
                var feeTxt = webDriver.FindElement(By.Id("TranRequestManagerFG.PMT_RMKS"));
                feeTxt.SendKeys(transId);
            }

            var gridAuth = webDriver.FindElements(By.Id("TranRequestManagerFG.AUTH_MODES"));
            gridAuth[1].Click();

            var continueBtn = webDriver.FindElements(By.Id("CONTINUE_PREVIEW"));
            if (continueBtn.Count > 0)
            {
                continueBtn[0].Click();
                WaitForReady(webDriver);
            }
            ProcessGridData(webDriver);

            var submitBtn = webDriver.FindElements(By.Id("CONTINUE_SUMMARY"));
            if (submitBtn.Count > 0)
            {
                submitBtn[0].Click();
                WaitForReady(webDriver);
            }

            var downloadBtn = webDriver.FindElement(By.Id("SINGLEPDF"));
            downloadBtn.Click();
            WaitFor(webDriver, 3);
        }
        private static void AssignAmount(IWebDriver webDriver, string amount)
        {
            if (amount.Length == 1)
            {
                var ones = webDriver.FindElement(By.Name("Ones"));
                var onesDDl = new SelectElement(ones);
                onesDDl.SelectByText(amount);
            }
            else if (amount.Length == 2)
            {
                var ones = webDriver.FindElement(By.Name("Ones"));
                var onesDDl = new SelectElement(ones);
                onesDDl.SelectByText(amount.Substring(1, 1));

                var ten = webDriver.FindElement(By.Name("Tens"));
                var tenDDl = new SelectElement(ten);
                tenDDl.SelectByText(amount.Substring(0, 1));
            }
            else if (amount.Length == 3)
            {
                var ones = webDriver.FindElement(By.Name("Ones"));
                var onesDDl = new SelectElement(ones);
                onesDDl.SelectByText(amount.Substring(2, 1));

                var ten = webDriver.FindElement(By.Name("Tens"));
                var tenDDl = new SelectElement(ten);
                tenDDl.SelectByText(amount.Substring(1, 1));

                var hundreds = webDriver.FindElement(By.Name("Hundreds"));
                var hundredsDDl = new SelectElement(hundreds);
                hundredsDDl.SelectByText(amount.Substring(0, 1));
            }
            else if (amount.Length > 3 && amount.Length < 6)
            {
                var ones = webDriver.FindElement(By.Name("Ones"));
                var onesDDl = new SelectElement(ones);
                onesDDl.SelectByText(amount.Substring(3, 1));

                var ten = webDriver.FindElement(By.Name("Tens"));
                var tenDDl = new SelectElement(ten);
                tenDDl.SelectByText(amount.Substring(2, 1));

                var hundreds = webDriver.FindElement(By.Name("Hundreds"));
                var hundredsDDl = new SelectElement(hundreds);
                hundredsDDl.SelectByText(amount.Substring(1, 1));

                var lngth = amount.Length == 4 ? 1 : 2;
                var thousands = webDriver.FindElement(By.Name("Thousands"));
                var thousandsDDl = new SelectElement(thousands);
                thousandsDDl.SelectByText(amount.Substring(0, lngth));

            }
            else if (amount.Length > 5 && amount.Length < 8)
            {
                var ones = webDriver.FindElement(By.Name("Ones"));
                var onesDDl = new SelectElement(ones);
                onesDDl.SelectByText(amount.Substring(4, 1));

                var ten = webDriver.FindElement(By.Name("Tens"));
                var tenDDl = new SelectElement(ten);
                tenDDl.SelectByText(amount.Substring(3, 1));

                var hundreds = webDriver.FindElement(By.Name("Hundreds"));
                var hundredsDDl = new SelectElement(hundreds);
                hundredsDDl.SelectByText(amount.Substring(2, 1));


                var thousands = webDriver.FindElement(By.Name("Thousands"));
                var thousandsDDl = new SelectElement(thousands);
                thousandsDDl.SelectByText(amount.Substring(1, 2));

                var lngth = amount.Length == 6 ? 1 : 2;
                var lakh = webDriver.FindElement(By.Name("Lakh"));
                var lakhDDl = new SelectElement(lakh);
                lakhDDl.SelectByText(amount.Substring(0, lngth));
            }

            else if (amount.Length > 7)
            {
                var ones = webDriver.FindElement(By.Name("Ones"));
                var onesDDl = new SelectElement(ones);
                onesDDl.SelectByText(amount.Substring(amount.Length - (amount.Length - 1), 1));

                var ten = webDriver.FindElement(By.Name("Tens"));
                var tenDDl = new SelectElement(ten);
                tenDDl.SelectByText(amount.Substring(amount.Length - (amount.Length - 2), 1));

                var hundreds = webDriver.FindElement(By.Name("Hundreds"));
                var hundredsDDl = new SelectElement(hundreds);
                hundredsDDl.SelectByText(amount.Substring(amount.Length - (amount.Length - 3), 1));


                var thousands = webDriver.FindElement(By.Name("Thousands"));
                var thousandsDDl = new SelectElement(thousands);
                thousandsDDl.SelectByText(amount.Substring(amount.Length - (amount.Length - 5), 2));


                var lakh = webDriver.FindElement(By.Name("Lakh"));
                var lakhDDl = new SelectElement(lakh);
                lakhDDl.SelectByText(amount.Substring(amount.Length - 7, 2));

                var lngth = amount.Length - 7;
                var crores = webDriver.FindElement(By.Name("Crores"));
                var croresDDl = new SelectElement(crores);
                croresDDl.SelectByText(amount.Substring(0, lngth));
            }

        }

        private static void ProcessGridData(IWebDriver webDriver) {
            Dictionary<string, string>  grid = new Dictionary<string, string>();
            grid.Add("A", _bankLogin.LetterA.ToString());
            grid.Add("B", _bankLogin.LetterB.ToString());
            grid.Add("C", _bankLogin.LetterC.ToString());
            grid.Add("D", _bankLogin.LetterD.ToString());
            grid.Add("E", _bankLogin.LetterE.ToString());
            grid.Add("F", _bankLogin.LetterF.ToString());
            grid.Add("G", _bankLogin.LetterG.ToString());
            grid.Add("H", _bankLogin.LetterH.ToString());
            grid.Add("I", _bankLogin.LetterI.ToString());
            grid.Add("J", _bankLogin.LetterJ.ToString());
            grid.Add("K", _bankLogin.LetterK.ToString());
            grid.Add("L", _bankLogin.LetterL.ToString());
            grid.Add("M", _bankLogin.LetterM.ToString());
            grid.Add("N", _bankLogin.LetterN.ToString());
            grid.Add("O", _bankLogin.LetterO.ToString());
            grid.Add("P", _bankLogin.LetterP.ToString());

            ////579091011.RGANESH
            //grid.Add("A", "20");
            //grid.Add("B", "43");
            //grid.Add("C", "12");
            //grid.Add("D", "32");
            //grid.Add("E", "64");
            //grid.Add("F", "76");
            //grid.Add("G", "04");
            //grid.Add("H", "42");
            //grid.Add("I", "47");
            //grid.Add("J", "67");
            //grid.Add("K", "71");
            //grid.Add("L", "03");
            //grid.Add("M", "41");
            //grid.Add("N", "71");
            //grid.Add("O", "93");
            //grid.Add("P", "11");

            ////579091011.VIJAYALAKSHMI
            //grid.Add("A", "38");
            //grid.Add("B", "94");
            //grid.Add("C", "84");
            //grid.Add("D", "08");
            //grid.Add("E", "51");
            //grid.Add("F", "47");
            //grid.Add("G", "23");
            //grid.Add("H", "81");
            //grid.Add("I", "21");
            //grid.Add("J", "81");
            //grid.Add("K", "16");
            //grid.Add("L", "91");
            //grid.Add("M", "63");
            //grid.Add("N", "13");
            //grid.Add("O", "11");
            //grid.Add("P", "55");

            ////Repro Sri
            //grid.Add("A", "90");
            //grid.Add("B", "82");
            //grid.Add("C", "45");
            //grid.Add("D", "71");
            //grid.Add("E", "42");
            //grid.Add("F", "57");
            //grid.Add("G", "54");
            //grid.Add("H", "01");
            //grid.Add("I", "83");
            //grid.Add("J", "10");
            //grid.Add("K", "60");
            //grid.Add("L", "82");
            //grid.Add("M", "21");
            //grid.Add("N", "92");
            //grid.Add("O", "53");
            //grid.Add("P", "34");

            //RGAN31
            //grid.Add("A", "17");
            //grid.Add("B", "45");
            //grid.Add("C", "32");
            //grid.Add("D", "41");
            //grid.Add("E", "64");
            //grid.Add("F", "28");
            //grid.Add("G", "50");
            //grid.Add("H", "86");
            //grid.Add("I", "32");
            //grid.Add("J", "93");
            //grid.Add("K", "06");
            //grid.Add("L", "93");
            //grid.Add("M", "40");
            //grid.Add("N", "51");
            //grid.Add("O", "47");
            //grid.Add("P", "29");

            var gridElms = webDriver.FindElements(By.ClassName("gridauth_input_cell_style"));
            var firstLetter = gridElms[0].Text;
            var secondLetter = gridElms[1].Text;
            var thirdLetter = gridElms[2].Text;

            var firstInput = webDriver.FindElement(By.Id("TranRequestManagerFG.GRID_CARD_AUTH_VALUE_1__"));
            firstInput.SendKeys(grid[firstLetter]);
            var secondInput = webDriver.FindElement(By.Id("TranRequestManagerFG.GRID_CARD_AUTH_VALUE_2__"));
            secondInput.SendKeys(grid[secondLetter]);
            var thirstInput = webDriver.FindElement(By.Id("TranRequestManagerFG.GRID_CARD_AUTH_VALUE_3__"));
            thirstInput.SendKeys(grid[thirdLetter]);

        }
    }
}
