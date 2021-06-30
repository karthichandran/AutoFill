using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;
using WebDriverManager.DriverConfigs.Impl;

namespace AutoFill
{
   public class Base
    {
        protected static void WaitForReady(IWebDriver webDriver)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(120);
            WebDriverWait wait = new WebDriverWait(webDriver, timeSpan);
            wait.Until(driver => {
                bool isAjaxFinished = (bool)((IJavaScriptExecutor)driver).
                    ExecuteScript("return jQuery.active == 0");
                try
                {
                    var loader = driver.FindElement(By.ClassName("loader-mask")).GetAttribute("style");
                    Console.WriteLine(loader);
                    return loader.Split(':')[1] == " none;";
                }
                catch
                {
                    return isAjaxFinished;
                }
            });
        }
        
        protected static void WaitFor(IWebDriver webDriver, int inSeconds = 0)
        {
            // webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(inSeconds);
            Thread.Sleep(inSeconds * 1000);
        }

        protected static IWebDriver GetChromeDriver()
        {
            try
            {
                //Runtime.getRuntime().exec("taskkill /F /IM chromedriver.exe /T");

                //Process[] chromeDriverProcesses = Process.GetProcessesByName("chromedriver");
                //foreach (var chromeDriverProcess in chromeDriverProcesses)
                //{
                //    chromeDriverProcess.Kill();
                //}

                ChromeOptions options = new ChromeOptions();
                options.AddArgument("--no-sandbox");
                options.AddArgument("--disable-infobars");
                options.AddArgument("--disable-dev-shm-usage");
                options.AddArgument("--start-maximized");

                // options.BinaryLocation = AppDomain.CurrentDomain.BaseDirectory+"chromedriver.exe";
                // options.AddArgument("--remote-debugging-port=9222");

                //var driver = new ChromeDriver(AppDomain.CurrentDomain.BaseDirectory, options);

               // new WebDriverManager.DriverManager().SetUpDriver(new ChromeConfig());
                ChromeDriver driver = new ChromeDriver( options);

               // ChromeDriver driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), options);

                return driver;
                //var ieDriver = GetIEDriver();
                //return ieDriver;
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        protected static IWebDriver GetIEDriver() {

           // InternetExplorerDriver ie = new InternetExplorerDriver();
            EdgeDriver edge = new EdgeDriver();
            return edge;
        }

    }
}
