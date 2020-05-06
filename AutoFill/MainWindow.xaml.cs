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
        public MainWindow()
        {
            InitializeComponent();
            svc = new service();
            LoadRemitance();
          //  AutoFillForm296Q();
        }

        private void LoadRemitance() {
           
            IList<TdsRemittanceDto> remitanceList = svc.GetTdsRemitance("","","","");
            remitanceGrid.ItemsSource = remitanceList;

        }

        private void AutoFillForm296Q(int clientPaymentTransactionID) {
            AutoFillDto autoFillDto = svc.GetAutoFillData(clientPaymentTransactionID);
            FillForm26Q.AutoFillForm26QB(autoFillDto);
        }

        private void proceedForm(object sender, RoutedEventArgs e)
        {
            var model = (sender as Button).DataContext as TdsRemittanceDto;
            AutoFillForm296Q(model.ClientPaymentTransactionID);
            //DemoModel model = (sender as Button).DataContext as DemoModel;
            //model.DynamicText = (new Random().Next(0, 100).ToString());
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            var custName = customerNameTxt.Text;
            var premise = PremisesTxt.Text;
            var unit = unitNoTxt.Text;
            var lot = lotNoTxt.Text;
            IList<TdsRemittanceDto> remitanceList = svc.GetTdsRemitance(custName, premise, unit, lot);
            remitanceGrid.ItemsSource = remitanceList;
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            customerNameTxt.Text = "";
            PremisesTxt.Text = "";
            unitNoTxt.Text = "";
            lotNoTxt.Text = "";
        }
    }
}
