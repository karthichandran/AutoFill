using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoFill
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       private service svc;
        private string tds;
        private string tdsInterest;
        private string lateFee;
        private IList<RemittanceStatus> remittanceStatusList;
        BackgroundWorker worker;
        public MainWindow()
        {
            InitializeComponent();
            svc = new service();
           
            LoadRemitance();
            
             progressbar1.Visibility = Visibility.Hidden;
            TracesProgressbar.Visibility = Visibility.Hidden;
        }

        private void LoadRemitance() {
            IList<TdsRemittanceDto> remitanceList = svc.GetTdsRemitance("","","","");
            remitanceGrid.ItemsSource = remitanceList;
            remittanceStatusList = svc.GetTdsRemitanceStatus();
            var emptyObj = new RemittanceStatus() { RemittanceStatusText = "", RemittanceStatusID = -1 };
            remittanceStatusList.Insert(0, emptyObj);            

            tracesRemitanceStatusddl.ItemsSource = remittanceStatusList;
            tracesRemitanceStatusddl.DisplayMemberPath = "RemittanceStatusText";
            tracesRemitanceStatusddl.SelectedValuePath = "RemittanceStatusID";
        }

        private bool AutoFillForm26Q(int clientPaymentTransactionID) {
            AutoFillDto autoFillDto = svc.GetAutoFillData(clientPaymentTransactionID);
            if (autoFillDto == null)
            {
                MessageBox.Show("Data is not available to proceed Form26QB", "alert", MessageBoxButton.OK);
                return false; ;
            }
            var bankLogin = svc.GetBankLoginDetails();
            FillForm26Q.AutoFillForm26QB(autoFillDto,tds, tdsInterest, lateFee, bankLogin);
            return true;
        }

        private void proceedForm(object sender, RoutedEventArgs e)
        {
            var model = (sender as Button).DataContext as TdsRemittanceDto;
            tds = model.TdsAmount.ToString();
            tdsInterest = model.TdsInterest.ToString();
            lateFee = model.LateFee.ToString();
            MethodThatWillCallComObject(AutoFillForm26Q,model.ClientPaymentTransactionID);         
        }

        private void TdsPaid(object sender, RoutedEventArgs e)
        {
            var model = (sender as Button).DataContext as TdsRemittanceDto;
            var challanAmount = model.TdsAmount + model.TdsInterest + model.LateFee;
            Challan challan = new Challan(model.ClientPaymentTransactionID, challanAmount);
            challan.Owner = this;
            challan.ShowDialog();
        }

        //private void MethodThatWillCallComObject(TdsRemittanceDto model)
        //{
        //    progressbar1.Visibility = Visibility.Visible;
        //    System.Threading.Tasks.Task.Factory.StartNew(() =>
        //    {
        //        //this will call in background thread               
        //        AutoFillForm296Q(model.ClientPaymentTransactionID);

        //    }).ContinueWith(t =>
        //    {
        //        progressbar1.Visibility = Visibility.Hidden;
        //    }, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
        //}

        private void MethodThatWillCallComObject(Func<int,bool> function,int id)
        {
            progressbar1.Visibility = Visibility.Visible;
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                function(id);

            }).ContinueWith(t =>
            {
                progressbar1.Visibility = Visibility.Hidden;
            }, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {            
            RemittanceSearchFilter();           
        }

        private void TracesSearch_Click(object sender, RoutedEventArgs e)
        {
            TracesSearchFilter();
        }

        private void TracesReset_Click(object sender, RoutedEventArgs e)
        {
            tracesCustomerNameTxt.Text = "";
            tracesPremisesTxt.Text = "";
            tracesUnitNoTxt.Text = "";
            tracesLotNoTxt.Text = "";
            tracesRemitanceStatusddl.SelectedValue = -1;
        }

        private void textboxKeydown(object sender, KeyEventArgs e)
        {
             if ( e.Key==Key.Enter)
                RemittanceSearchFilter();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            customerNameTxt.Text = "";
            PremisesTxt.Text = "";
            unitNoTxt.Text = "";
            lotNoTxt.Text = "";            
        }
        private void RemittanceSearchFilter() {
            var custName = customerNameTxt.Text;
            var premise = PremisesTxt.Text;
            var unit = unitNoTxt.Text;
            var lot = lotNoTxt.Text;
              RemittanceSearchTask(custName, premise, unit, lot);           
                   
        }

        private async void RemittanceSearchTask(string custName, string premise, string unit, string lot)
        {
            progressbar1.Visibility = Visibility.Visible;
            var remittanceList= await Task.Run(() => {
                return svc.GetTdsRemitance(custName, premise, unit, lot);
            });
            remitanceGrid.ItemsSource = remittanceList;
            progressbar1.Visibility = Visibility.Hidden;
          
        }
        
        private void TracesSearchFilter()
        {
            var remiitanceStatusID =( tracesRemitanceStatusddl.SelectedValue == null || Convert.ToInt32( tracesRemitanceStatusddl.SelectedValue) == -1) ? null : tracesRemitanceStatusddl.SelectedValue.ToString();
            var custName = tracesCustomerNameTxt.Text;
            var premise = tracesPremisesTxt.Text;
            var unit = tracesUnitNoTxt.Text;
            var lot = tracesLotNoTxt.Text;
            TracesSearchTask(custName, premise, unit, lot, remiitanceStatusID);
        }

        private async void TracesSearchTask(string custName, string premise, string unit, string lot,string remiittanceStatusID)
        {
            TracesProgressbar.Visibility = Visibility.Visible;
            var remittanceList = await Task.Run(() => {
                return svc.GetTdsPaidList(custName, premise, unit, lot, remiittanceStatusID);
            });
            TracesGrid.ItemsSource = remittanceList;
            TracesProgressbar.Visibility = Visibility.Hidden;
        }

        private async void RequestForm16B(object sender, RoutedEventArgs e)
        {
            var model = (sender as Button).DataContext as TdsRemittanceDto;
            var tdsremittanceModel = svc.GetTdsRemitanceById(model.ClientPaymentTransactionID);
            var reqNo = "";
            TracesProgressbar.Visibility = Visibility.Visible;
            if (tdsremittanceModel != null) {
                await Task.Run(() => {
                    reqNo = FillTraces.AutoFillForm16B(tdsremittanceModel);
                });
            }
            //reqNo = FillTraces.AutoFillForm16B(tdsremittanceModel);
            TracesProgressbar.Visibility = Visibility.Hidden;
            if (reqNo != "")
            {
                var challanAmount = model.TdsAmount + model.TdsInterest + model.LateFee;
                Traces traces = new Traces(model,reqNo);
                traces.Owner = this;
                traces.ShowDialog();
            }
        }
        private async void DownLoadForm(object sender, RoutedEventArgs e)
        {
            var model = (sender as Button).DataContext as TdsRemittanceDto;
            var tdsremittanceModel = svc.GetTdsRemitanceById(model.ClientPaymentTransactionID);
            var remittanceModel = svc.GetRemitanceByTransID(model.ClientPaymentTransactionID);
            if (tdsremittanceModel != null)
            {
                TracesProgressbar.Visibility = Visibility.Visible;
                await Task.Run(() => {
                    FillTraces.AutoFillDownload(tdsremittanceModel, remittanceModel.F16BRequestNo, remittanceModel.DateOfBirth);
                });

                // FillTraces.AutoFillDownload(tdsremittanceModel, remittanceModel.F16BRequestNo, remittanceModel.DateOfBirth);
                TracesProgressbar.Visibility = Visibility.Hidden;
            }
        }

        private void UpdateRemittance(object sender, RoutedEventArgs e)
        {
            var model = (sender as Button).DataContext as TdsRemittanceDto;
            var challanAmount = model.TdsAmount + model.TdsInterest + model.LateFee;
            
            Traces traces = new Traces(model);
            traces.Owner = this;
            traces.ShowDialog();
        }

        private async void DeleteFromRemittance(object sender, RoutedEventArgs e)
        {
            var model = (sender as Button).DataContext as TdsRemittanceDto;
            var remittanceModel = svc.GetRemitanceByTransID(model.ClientPaymentTransactionID);
            if (remittanceModel.RemittanceID == 0) {
                MessageBox.Show("Remittance record is not yet created");
                return;
            }
            var resultMsg= MessageBox.Show("Are you sure to delete this?", "alert", MessageBoxButton.OKCancel);
            if (resultMsg == MessageBoxResult.OK) {
                progressbar1.Visibility = Visibility.Visible;
                bool status = false;
                await Task.Run(() => {
                    status = svc.DeleteRemittance(remittanceModel.RemittanceID);
                });
                progressbar1.Visibility = Visibility.Hidden;
                if (status)
                {
                    MessageBox.Show("Remittance is deleted successfully");
                    RemittanceSearchFilter();
                }
                else
                {
                    MessageBox.Show("Remittance is not deleted.");
                }
                
            }
        }

        private async void DeleteFromTrace(object sender, RoutedEventArgs e)
        {
            var model = (sender as Button).DataContext as TdsRemittanceDto;
            var remittanceModel = svc.GetRemitanceByTransID(model.ClientPaymentTransactionID);
            if (remittanceModel.RemittanceID == 0)
            {
                MessageBox.Show("Remittance record is not yet created");
                return;
            }
            var resultMsg = MessageBox.Show("Are you sure to delete this?", "alert", MessageBoxButton.OKCancel);
            if (resultMsg == MessageBoxResult.OK)
            {
                TracesProgressbar.Visibility = Visibility.Visible;
                bool status = false ;
                await Task.Run(() => {
                    status= svc.DeleteRemittance(remittanceModel.RemittanceID);
                });
                TracesProgressbar.Visibility = Visibility.Hidden;
                if (status)
                {
                    MessageBox.Show("Remittance is deleted successfully");
                    TracesSearchFilter();
                }
                else
                {
                    MessageBox.Show("Remittance is not deleted.");
                }
            }
        }

        private async void SendMail(object sender, RoutedEventArgs e)
        {
            var model = (sender as Button).DataContext as TdsRemittanceDto;
            var challanAmount = model.TdsAmount + model.TdsInterest + model.LateFee;
            bool status = false;
            TracesProgressbar.Visibility = Visibility.Visible;
            await Task.Run(() => {
                status = svc.SendMail(model.ClientPaymentTransactionID);
            });
            TracesProgressbar.Visibility = Visibility.Hidden;
            if (status)
            {
                MessageBox.Show("Mail is delivered");
                TracesSearchFilter();
            }
            else
            {
                MessageBox.Show("Failed to send mail");
            }
        }

    }
}
