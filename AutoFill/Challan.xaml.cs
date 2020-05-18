using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AutoFill
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Challan : Window
    {
        private service svc;
        private int transID;
        private decimal challanAmt;
        private RemittanceDto remittance;
        public Challan(int trnasactionID,decimal challanAmount)
        {
            InitializeComponent();
            transID = trnasactionID;
            challanAmt = challanAmount;
            svc = new service();
            LoadRemitance();
        }

       private void LoadRemitance() {
            remittance = svc.GetRemitanceByTransID(transID);
            if (remittance.RemittanceID == 0)
            {
                ChallanAmount.Text = challanAmt.ToString();
                ChallanDate.Text = DateTime.Now.Date.ToString();
                upload.Visibility = Visibility.Hidden;
            }
            else {
                upload.Visibility = Visibility.Visible;
                ChallanDate.Text = remittance.ChallanDate.ToString();
                ChallanNo.Text = remittance.ChallanID;
                AknowledgementNo.Text = remittance.ChallanAckNo;
                ChallanAmount.Text = remittance.ChallanAmount.ToString();
                CustomerPropertyFileDto customerPropertyFileDto= svc.GetFile(remittance.ChallanFileID.ToString());
                if (customerPropertyFileDto != null)
                {
                    FileNameLabel.Content = customerPropertyFileDto.FileName;
                }
            }
        }

        private void Upload_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();

            Nullable<bool> result = openFileDlg.ShowDialog();
            if (result == true)
            {
                FileNameLabel.Content = openFileDlg.SafeFileName;
                
                var formData= new MultipartFormDataContent();
                var fileContent = new ByteArrayContent(File.ReadAllBytes(openFileDlg.FileName));
                var fileType = System.IO.Path.GetExtension(openFileDlg.FileName);
                var contentType = svc.GetContentType(fileType);
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
                var name = System.IO.Path.GetFileName(openFileDlg.FileName);
                formData.Add(fileContent, "file", name);
                svc.UploadFile(formData, remittance.ChallanFileID.ToString(), 7);
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (Validate()) {
                if (remittance.ClientPaymentTransactionID == 0)
                    remittance.ClientPaymentTransactionID = transID;
                remittance.ChallanAmount = Convert.ToDecimal(ChallanAmount.Text.Trim());
                remittance.ChallanAckNo= AknowledgementNo.Text.Trim();
                remittance.ChallanDate = Convert.ToDateTime(ChallanDate.Text.Trim());
                remittance.ChallanID= ChallanNo.Text.Trim();
                remittance.RemittanceStatusID = 2;
               
                bool result = svc.SaveRemittance(remittance);
                if (result)
                {
                    LoadRemitance();
                    MessageBox.Show("Challan details are saved successfully");
                }
                else
                    MessageBox.Show("Challan details are not saved ");
            }
        }

        
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private bool Validate() {

            string challanDate = ChallanDate.Text.Trim();
            string challanNo = ChallanNo.Text.Trim();
            string acknowledgement = AknowledgementNo.Text.Trim();
            string challanAmount = ChallanAmount.Text.Trim();
            string errorMsg = "";
            if (challanDate == "")
                errorMsg = "Please enter the challan date";
            else if(challanNo=="")
                errorMsg = "Please enter the challan Number";
            else if (acknowledgement == "")
                errorMsg = "Please enter the challan Acknowledgement Number";
            else if (challanAmount == "")
                errorMsg = "Please enter the challan Amount";

            if (errorMsg != "")
            {
                MessageBox.Show(errorMsg);
                return false;
            }

            return true;
        }
    }
}
