using Aspose.Zip;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using iTextSharp.text.pdf;
using iTextSharp.text.io;
using System.Text.RegularExpressions;

namespace AutoFill
{
    public class UnzipFile
    {
        public UnzipFile() {
        }

        public void extractFile(string fileName, string pwd) {
            // using Microsoft.Win32;

            var downloadPath = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", "{374DE290-123F-4565-9164-39C4925E467B}", String.Empty).ToString();

            var filePath = @downloadPath + "\\" + fileName + ".zip";
            var startTime = DateTime.Now;
            while (!File.Exists(filePath))
            {
                Thread.Sleep(1000);
                var currentDate = DateTime.Now;
                if (currentDate.Subtract(startTime).TotalMinutes > 10)
                    break;
            }

            //// Open ZIP file
            using (FileStream zipFile = File.Open(filePath, FileMode.Open))
            {
                // Decrypt using password
                using (var archive = new Archive(zipFile, new ArchiveLoadOptions() { DecryptionPassword = pwd }))
                {
                    // Extract files to folder
                    archive.ExtractToDirectory(@downloadPath + "\\" + fileName);
                  
                    // Open a file
                    var p = new Process();
                    p.StartInfo = new ProcessStartInfo(@downloadPath + "\\" + fileName + "\\" + fileName + ".pdf")
                    {
                        UseShellExecute = true
                    };
                    p.Start();
                    var sourceFile = @downloadPath + "\\" + fileName + "\\" + fileName + ".pdf";
                    var destFile = @downloadPath + "\\" +  fileName + ".pdf";
                    System.IO.File.Copy(sourceFile, destFile, true);
                }
            }

        }

        public Dictionary<string, string> getChallanDetails(string filePath, string pan) {
            Dictionary<string, string> challanDet = new Dictionary<string, string>();
            PDFParser pdfParser = new PDFParser();
            PdfReader reader = new PdfReader(@filePath);
            var text = new PDFParser().ExtractTextFromPDFBytes(reader.GetPageContent(1)).Trim().ToString();
            Console.WriteLine(text);
            var serialNo = GetWordAfterMatch(text, "Challan Serial No.");
            Console.WriteLine("Challan Serial NO :" + serialNo);
            var paninDoc = GetWordAfterMatch(text, "PAN:");
            if (pan != paninDoc.ToString())
                return challanDet;
            challanDet.Add("serialNo", serialNo.ToString());
            var itns = GetWordAfterMatch(text, "Challan No./ITNS");
            Console.WriteLine("ITNS :" + itns);
            var tenderDate = GetWordAfterMatch(text, "Tender Date");
            challanDet.Add("tenderDate", tenderDate.ToString());
            var challamAmount = GetWordAfterMatch(text, "Rs. :");
            challanDet.Add("challanAmount", challamAmount.ToString());
            // var PAN = "BUZPP5880P"; //todo pass the pan number
            // pan = "ADMPC7474M";
            var tds = GetTDSConfirmationNo(text, pan);
            Console.WriteLine("tds conf NO :" + tds);
            challanDet.Add("acknowledge", tds.ToString());
            Console.ReadLine();
            return challanDet;
        }

        private object GetWordAfterMatch(string text, string word)
        {

            var pattern = string.Format(@"\b\w*" + word + @"\w*\s+\w+\b");
            string match = Regex.Match(text, @pattern).Groups[0].Value;
            string[] words = match.Split(' ');
            string wordAfter = words[words.Length - 1];

            return wordAfter;
        }

        private object GetTDSConfirmationNo(string text, string word)
        {

            var pattern = string.Format(word + @"?.*");
            string match = Regex.Match(text, @pattern).Groups[0].Value.Substring(25, 100);
            string[] words = match.Split(',');
            string wordAfter = words[3];

            return wordAfter;
        }

        private object GetCertificateNoAfterMatch(string text, string word)
        {
            var pattern = string.Format(@"\b\w*" + word + @"\w*\s+\w+\s+\w+(-)\w+\s+\w+\b");
            string match = Regex.Match(text, @pattern).Groups[0].Value;
            string[] words = match.Split(' ');
            string wordAfter = words[words.Length - 1];
            return wordAfter;
        }

        public string GetCertificateNo(string filePath,string pan)
        {
            PDFParser pdfParser = new PDFParser();
            PdfReader reader = new PdfReader(@filePath);
            var text = new PDFParser().ExtractTextFromPDFBytes(reader.GetPageContent(1)).Trim().ToString();
            Console.WriteLine(text);
         
            var certNo = GetCertificateNoAfterMatch(text, pan);
           
            return certNo.ToString();
        }
    }
}
