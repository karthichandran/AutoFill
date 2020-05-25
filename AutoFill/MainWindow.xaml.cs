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
        private IList<RemittanceStatus> remittanceStatusList;
        BackgroundWorker worker;
        public MainWindow()
        {
            InitializeComponent();
            svc = new service();
           
            LoadRemitance();
            //  AutoFillForm296Q();
            progressbar1.Visibility = Visibility.Hidden;
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

        private bool AutoFillForm296Q(int clientPaymentTransactionID) {
            AutoFillDto autoFillDto = svc.GetAutoFillData(clientPaymentTransactionID);
            if (autoFillDto == null)
            {
                MessageBox.Show("Data is not available to proceed Form26QB", "alert", MessageBoxButton.OK);
                return false; ;
            }
           
            FillForm26Q.AutoFillForm26QB(autoFillDto);
            return true;
        }

        private void proceedForm(object sender, RoutedEventArgs e)
        {
            var model = (sender as Button).DataContext as TdsRemittanceDto;
            MethodThatWillCallComObject(AutoFillForm296Q,model.ClientPaymentTransactionID);         
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
         
            IList<TdsRemittanceDto> remitanceList = svc.GetTdsRemitance(custName, premise, unit, lot);
            remitanceGrid.ItemsSource = remitanceList;                   
        }

        private void TracesSearchFilter()
        {
            var remiitanceStatusID =( tracesRemitanceStatusddl.SelectedValue == null || Convert.ToInt32( tracesRemitanceStatusddl.SelectedValue) == -1) ? null : tracesRemitanceStatusddl.SelectedValue.ToString();
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
                Traces traces = new Traces(model,reqNo);
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
            
            Traces traces = new Traces(model);
            traces.Owner = this;
            traces.ShowDialog();
        }

       
    }
}
