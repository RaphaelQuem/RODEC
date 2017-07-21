using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private string stringLogs;
        public string StringLogs
        {
            get { return stringLogs; }
            set
            {
                if (value.Split('\n').Count() >= 19)
                    stringLogs = value.Replace(value.Substring(0, value.IndexOf('\n')), "").Trim();
                else
                    stringLogs = value;
                NotifyPropertyChanged("StringLogs");
            }
        }
        private string status;

        public string Status { get { return status; } set { status = value; NotifyPropertyChanged("Status"); } }
        public MonitorVM()
        {
            Status = "Parado";
            CanRun = true;
            CanStop = false;
        }
    }
}
