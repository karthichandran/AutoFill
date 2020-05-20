using System;
using System.Collections.Generic;
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
        private IList<RemittanceStatus> remittanceStatusList;
        public MainWindow()
        {
            InitializeComponent();
            svc = new service();
           
            LoadRemitance();
            //  AutoFillForm296Q();
            progressbar1.Visibility = Visibility.Hidden;
        }

        private void LoadRemitance() {
           
            IList<TdsRemittanceDto> remitanceList = svc.GetTdsRemitance("","","","","");
            remitanceGrid.ItemsSource = remitanceList;
            remittanceStatusList = svc.GetTdsRemitanceStatus();
            remitanceStatusddl.ItemsSource = remittanceStatusList;
            remitanceStatusddl.DisplayMemberPath = "RemittanceStatusText";
            remitanceStatusddl.SelectedValuePath = "RemittanceStatusID";

            tracesRemitanceStatusddl.ItemsSource = remittanceStatusList;
            tracesRemitanceStatusddl.DisplayMemberPath = "RemittanceStatusText";
            tracesRemitanceStatusddl.SelectedValuePath = "RemittanceStatusID";
        }

        private void AutoFillForm296Q(int clientPaymentTransactionID) {
            AutoFillDto autoFillDto = svc.GetAutoFillData(clientPaymentTransactionID);
            if (autoFillDto == null)
            {
                MessageBox.Show("Data is not available to proceed Form26QB", "alert", MessageBoxButton.OK);
                return;
            }
           
            FillForm26Q.AutoFillForm26QB(autoFillDto);           
        }

        private void proceedForm(object sender, RoutedEventArgs e)
        {
            progressbar1.Visibility = Visibility.Visible;         

            var model = (sender as Button).DataContext as TdsRemittanceDto;
            MethodThatWillCallComObject(model);         
        }

        private void TdsPaid(object sender, RoutedEventArgs e)
        {
            var model = (sender as Button).DataContext as TdsRemittanceDto;
            var challanAmount = model.TdsAmount + model.TdsInterest + model.LateFee;
            Challan challan = new Challan(model.ClientPaymentTransactionID, challanAmount);
            challan.Owner = this;
            challan.ShowDialog();

            //MessageBoxResult result = MessageBox.Show("Are you sure want to change the status?", "Confirmation", MessageBoxButton.YesNo);
            //if (result == MessageBoxResult.No)
            //    return;

            //var model = (sender as Button).DataContext as TdsRemittanceDto;
            //bool isSucess = false;
            //progressbar1.Visibility = Visibility.Visible;

            //System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{            
            //    isSucess = svc.SetToTdsPaid(model.ClientPaymentTransactionID);               

            //}).ContinueWith(t =>
            //{
            //    if (isSucess)
            //    {
            //        RemittanceSearchFilter();
            //        MessageBox.Show("Status is updated");
            //    }
            //    else
            //        MessageBox.Show("Unable to Update Status");
            //    progressbar1.Visibility = Visibility.Hidden;
            //}, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());           
           
        }

        private void MethodThatWillCallComObject(TdsRemittanceDto model)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                //this will call in background thread               
                AutoFillForm296Q(model.ClientPaymentTransactionID);

            }).ContinueWith(t =>
            {
                progressbar1.Visibility = Visibility.Hidden;
            }, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {            
              RemittanceSearchFilter();
           
            //Challan challan = new Challan(12, 12);
            //challan.Owner = this;
            //challan.ShowDialog();
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
            var remiitanceStatusID = remitanceStatusddl.SelectedValue == null ? null : remitanceStatusddl.SelectedValue.ToString();
            var custName = customerNameTxt.Text;
            var premise = PremisesTxt.Text;
            var unit = unitNoTxt.Text;
            var lot = lotNoTxt.Text;
            IList<TdsRemittanceDto> remitanceList = svc.GetTdsRemitance(custName, premise, unit, lot, remiitanceStatusID);
            remitanceGrid.ItemsSource = remitanceList;
        }

        private void TracesSearchFilter()
        {
            var remiitanceStatusID = tracesRemitanceStatusddl.SelectedValue == null ? null : tracesRemitanceStatusddl.SelectedValue.ToString();
            var custName = tracesCustomerNameTxt.Text;
            var premise = tracesPremisesTxt.Text;
            var unit = tracesUnitNoTxt.Text;
            var lot = tracesLotNoTxt.Text;
            IList<TdsRemittanceDto> remitanceList = svc.GetTdsPaidList(custName, premise, unit, lot, remiitanceStatusID);
            //foreach (var model in remitanceList) {
            //    model.ChallanAmount = model.TdsAmount + model.TdsInterest + model.LateFee;
            //}
            TracesGrid.ItemsSource = remitanceList;
        }

        private void RequestForm16B(object sender, RoutedEventArgs e)
        {
            var model = (sender as Button).DataContext as TdsRemittanceDto;
            var tdsremittanceModel = svc.GetTdsRemitanceById(model.ClientPaymentTransactionID);
            var reqNo = "";
            if (tdsremittanceModel!=null)
             reqNo = FillTraces.AutoFillForm16B(tdsremittanceModel);

            if (reqNo != "")
            {
                var challanAmount = model.TdsAmount + model.TdsInterest + model.LateFee;
                Traces traces = new Traces(model.ClientPaymentTransactionID, challanAmount,reqNo);
                traces.Owner = this;
                traces.ShowDialog();
            }
        }
        private void DownLoadForm(object sender, RoutedEventArgs e)
        {
            var model = (sender as Button).DataContext as TdsRemittanceDto;
            var tdsremittanceModel = svc.GetTdsRemitanceById(model.ClientPaymentTransactionID);
            var remittanceModel = svc.GetRemitanceByTransID(model.ClientPaymentTransactionID);
            if (tdsremittanceModel != null)
                FillTraces.AutoFillDownload(tdsremittanceModel, remittanceModel.F16BRequestNo,remittanceModel.DateOfBirth);
        }

        private void UpdateRemittance(object sender, RoutedEventArgs e)
        {
            var model = (sender as Button).DataContext as TdsRemittanceDto;
            var challanAmount = model.TdsAmount + model.TdsInterest + model.LateFee;
            Traces traces = new Traces(model.ClientPaymentTransactionID, challanAmount);
            traces.Owner = this;
            traces.ShowDialog();
        }
    }
}
