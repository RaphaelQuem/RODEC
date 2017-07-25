using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace RODEC.ViewModel
{
    public class MonitorVM : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        private bool canRun;
        public bool CanRun { get { return canRun; } set { canRun = value; NotifyPropertyChanged("CanRun"); } }
        private bool canStop;
        public bool CanStop { get { return canStop; } set { canStop = value; NotifyPropertyChanged("CanStop"); } }

        private List<string> logs;

        public List<string> Logs
        {
            get
            { return logs; }
            set
            {
                logs = value;
                NotifyPropertyChanged("Logs");
            }
        }

        private List<string> singleLogs;

        public List<string> SingleLogs
        {
            get
            { return singleLogs; }
            set
            {
                singleLogs = value;
                NotifyPropertyChanged("SingleLogs");
            }
        }

        private string singleLog;
        public string SingleLog { get { return singleLog; } set { singleLog = value; NotifyPropertyChanged("SingleLog"); } }
        private string log;
        public string Log { get { return log; } set { log = value; NotifyPropertyChanged("Log"); } }

        private string status;
        public bool FreezeLogs;
        public bool FreezeSingleLogs;

        public string Status { get { return status; } set { status = value; NotifyPropertyChanged("Status"); } }
        public MonitorVM()
        {
            Status = "Parado";
            CanRun = true;
            CanStop = false;
            Logs = new List<string>();
            SingleLogs = new List<string>();
        }
    }
}
