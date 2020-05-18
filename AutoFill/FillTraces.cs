﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace AutoFill
{
    public class FillTraces : Base
    {
        public static void AutoFillForm16B(TdsRemittanceDto tdsRemittanceDto) {
            try {
                var driver = GetChromeDriver();
                driver.Navigate().GoToUrl("https://www.tdscpc.gov.in/app/login.xhtml");
                WaitForReady(driver);
                FillLogin(driver, tdsRemittanceDto);
                RquestForm16B(driver, tdsRemittanceDto);
            }
            catch (Exception e) {
            }
        }

        public static void AutoFillDownload(TdsRemittanceDto tdsRemittanceDto,string requestNo )
        {
            try
            {
                var driver = GetChromeDriver();
                driver.Navigate().GoToUrl("https://www.tdscpc.gov.in/app/login.xhtml");
                WaitForReady(driver);
                FillLogin(driver, tdsRemittanceDto);
                DownloadForm(driver, requestNo);
            }
            catch (Exception e)
            {
            }
        }

        private static void FillLogin(IWebDriver webDriver, TdsRemittanceDto tdsRemittanceDto) {
            var logintype = webDriver.FindElement(By.Id("tpao"));
            logintype.Click();
            
            WaitForReady(webDriver);

            var userId = webDriver.FindElement(By.Id("userId"));
            //  userId.SendKeys("AJLPG4797J");
            userId.SendKeys(tdsRemittanceDto.CustomerPAN);
            userId.SendKeys(Keys.Tab);
            var pwd = webDriver.FindElement(By.Id("psw"));
            //  pwd.SendKeys("Girish&123");
            pwd.SendKeys(tdsRemittanceDto.TracesPassword);

            MessageBoxResult result = MessageBox.Show("Please fill the capcha and press ok button", "Confirmation", MessageBoxButton.YesNo);
            WaitForReady(webDriver);
            var confirmationChk= webDriver.FindElement(By.Id("Details"));
            confirmationChk.Click();
            WaitFor(webDriver, 2);
            var confirmationBtn = webDriver.FindElement(By.Id("btn"));
            confirmationBtn.Click();
            WaitForReady(webDriver);           
        }

        private static void RquestForm16B(IWebDriver webDriver, TdsRemittanceDto tdsRemittanceDto) {
            webDriver.Navigate().GoToUrl("https://www.tdscpc.gov.in/app/tap/download16b.xhtml");
            WaitForReady(webDriver);

            var formType = webDriver.FindElement(By.Id("formTyp"));
            var formTypeDDL = new SelectElement(formType);
            formTypeDDL.SelectByText("26QB");

            var assessmentYear = webDriver.FindElement(By.Id("assmntYear"));
            var assessmentYearDDL = new SelectElement(assessmentYear);
            // assessmentYearDDL.SelectByText("2020-21");
            assessmentYearDDL.SelectByText(tdsRemittanceDto.AssessmentYear);

            var actkNo = webDriver.FindElement(By.Id("ackNo"));
            actkNo.SendKeys(tdsRemittanceDto.ChallanAckNo);

            var panOfSeller = webDriver.FindElement(By.Id("panOfSeller"));
            //panOfSeller.SendKeys("AJLPG4797J");
            panOfSeller.SendKeys(tdsRemittanceDto.SellerPAN);

            var process = webDriver.FindElement(By.Id("clickGo"));
        }

        private static void DownloadForm(IWebDriver webDriver, string requestNo)
        {
            webDriver.Navigate().GoToUrl("https://www.tdscpc.gov.in/app/tap/tpfiledwnld.xhtml");
            WaitForReady(webDriver);

            var searchOpt = webDriver.FindElement(By.Id("search1"));
            searchOpt.Click();
           
            var requestTxt = webDriver.FindElement(By.Id("reqNo"));
            requestTxt.SendKeys(requestNo);

            var viewRequestBtn = webDriver.FindElement(By.Id("getListByReqId"));
            viewRequestBtn.Click();
           
            var rows = webDriver.FindElements(By.ClassName("jqgrow"));
            if (rows.Count == 0)
                return;
             
            var statusCell= rows[0].FindElements(By.TagName("td"))[6];
            if (statusCell.Text.Trim() != "Available")
                return;
            statusCell.Click();

            var httpDownload = webDriver.FindElement(By.Id("downloadhttp"));
            httpDownload.Click();
        }
    }
}
