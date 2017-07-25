using RODEC.Controller;
using RODEC.ViewModel;
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
using System.Text.RegularExpressions;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RODEC.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IntegrationController controller;
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        public MonitorVM ViewModel
        {
            get { return DataContext as MonitorVM; }
            set { DataContext = value; }
        }
        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MonitorVM();
            controller = new IntegrationController(ViewModel);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void btnExportClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CanRun)
            {
                controller.Export();
            }
            Clean();
        }
        private void Clean()
        {
            textBox1.Focusable = true;
            Keyboard.Focus(textBox1);
        }
        private void btnExportSingleClick(object sender, RoutedEventArgs e)
        {

            controller.ExportSingle(textBox1.Text, textBox.Text);
            Clean();
        }

        private void btnStopClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CanStop)
            {
                ViewModel.Status = "Parando...";
                ViewModel.CanStop = false;
                ViewModel.CanRun = false;
            }
            Clean();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (!ViewModel.FreezeLogs)
            {
                gvLog.Items.Clear();
                foreach (string s in ViewModel.Logs)
                    gvLog.Items.Add(s);
            }

            if (!ViewModel.FreezeSingleLogs)
            {
                gvSingleLog.Items.Clear();
                foreach (string s in ViewModel.SingleLogs)
                    gvSingleLog.Items.Add(s);
            }
        }

        private void textBox1_GotFocus(object sender, RoutedEventArgs e)
        {
            textBox1.Text = "";
            textBox.Text = "";
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnFreezeLog_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.FreezeLogs = !ViewModel.FreezeLogs;
            Clean();
        }

        private void btnFreezeSingleLog_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.FreezeSingleLogs = !ViewModel.FreezeSingleLogs;
            Clean();
        }
    }
}
