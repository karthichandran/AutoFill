using Aspose.Zip;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;

namespace AutoFill
{
    public  class UnzipFile
    {
       public UnzipFile() { 
        }

        public void extractFile(string fileName,string pwd) {
            // using Microsoft.Win32;
          
            var downloadPath = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", "{374DE290-123F-4565-9164-39C4925E467B}", String.Empty).ToString();

            var filePath = @downloadPath + "\\" + fileName + ".zip";
            var startTime = DateTime.Now;
            while (!File.Exists(filePath))
            {
                Thread.Sleep(1000);
                var currentDate= DateTime.Now;
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
                    archive.ExtractToDirectory(@downloadPath+ "\\" + fileName);
                    var p = new Process();
                    p.StartInfo = new ProcessStartInfo(@downloadPath + "\\" + fileName + "\\" + fileName + ".pdf")
                    {
                        UseShellExecute = true
                    };
                    p.Start();
                }
            }
                       
           

            //// Open ZIP file
            //using (FileStream zipFile = File.Open("compressed_files.zip", FileMode.Open))
            //{
            //    // Decrypt using password
            //    using (var archive = new Archive(zipFile, new ArchiveLoadOptions() { DecryptionPassword = "p@s$" }))
            //    {
            //        // Extract files to folder
            //        archive.ExtractToDirectory("Unzipped Files");
            //    }
            //}
        }
    }
}
