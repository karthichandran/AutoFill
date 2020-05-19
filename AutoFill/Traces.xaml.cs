using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows;

namespace AutoFill
{
    /// <summary>
    /// Interaction logic for Traces.xaml
    /// </summary>
    public partial class Traces : Window
    {
        private service svc;
        private int transID;
        private decimal challanAmt;
        private RemittanceDto remittance;
        public Traces(int trnasactionID, decimal challanAmount)
        {
            InitializeComponent();
            transID = trnasactionID;
            challanAmt = challanAmount;
            svc = new service();
            LoadRemitance();
        }
        private void LoadRemitance()
        {
            remittance = svc.GetRemitanceByTransID(transID);

            if (remittance.F16BDateOfReq == null)
                RequestDate.Text = DateTime.Now.Date.ToString();
            else
                RequestDate.Text = remittance.F16BDateOfReq.ToString();

            upload.Visibility = Visibility.Visible;

            customerPan.Content = remittance.CustomerPAN;
            dateOfBirth.Content = remittance.DateOfBirth.ToString("ddMMyyyy");

            RequestNo.Text = remittance.F16BRequestNo;
            CertificateNo.Text = remittance.F16BCertificateNo;
            CustomerPropertyFileDto customerPropertyFileDto = svc.GetFile(remittance.F16BFileID.ToString());
            if (customerPropertyFileDto != null)
            {
                FileNameLabel.Content = customerPropertyFileDto.FileName;
            }

        }

        private void Upload_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();

            Nullable<bool> result = openFileDlg.ShowDialog();
            if (result == true)
            {
                FileNameLabel.Content = openFileDlg.SafeFileName;

                var formData = new MultipartFormDataContent();
                var fileContent = new ByteArrayContent(File.ReadAllBytes(openFileDlg.FileName));
                var fileType = System.IO.Path.GetExtension(openFileDlg.FileName);
                var contentType = svc.GetContentType(fileType);
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
                var name = System.IO.Path.GetFileName(openFileDlg.FileName);
                formData.Add(fileContent, "file", name);
                svc.UploadFile(formData, remittance.F16BFileID.ToString(), 6);
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (Validate())
            {
                if (remittance.ClientPaymentTransactionID == 0)
                    remittance.ClientPaymentTransactionID = transID;
                remittance.F16BCertificateNo = CertificateNo.Text.Trim();
                remittance.F16BDateOfReq = Convert.ToDateTime(RequestDate.Text.Trim());
                remittance.F16BRequestNo = RequestNo.Text.Trim();
               if( remittance.F16BCertificateNo!="")
                remittance.RemittanceStatusID = 4;
               else
                    remittance.RemittanceStatusID = 3;

                bool result = svc.SaveRemittance(remittance);
                if (result)
                {
                    LoadRemitance();
                    MessageBox.Show("Request details are saved successfully");
                }
                else
                    MessageBox.Show("Request details are not saved ");
            }
        }


        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private bool Validate()
        {

            string requestDate = RequestDate.Text.Trim();          
            string errorMsg = "";
            if (requestDate == "")
                errorMsg = "Please enter the request date";       
          
            if (errorMsg != "")
            {
                MessageBox.Show(errorMsg);
                return false;
            }

            return true;
        }
    }

}
