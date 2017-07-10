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
        }

        private void btnExportClick(object sender, RoutedEventArgs e)
        {
            controller.Export();
        }

        private void btnExportSingleClick(object sender, RoutedEventArgs e)
        {

            controller.ExportSingle(textBox1.Text, textBox.Text);
        }

        private void btnStopClick(object sender, RoutedEventArgs e)
        {
            ViewModel.Status = "Parando...";
            ViewModel.CanStop = false;
            ViewModel.CanRun = false;
        }
          
    }
}
