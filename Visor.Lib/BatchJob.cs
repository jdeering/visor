using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Visor.Options;

namespace Visor.Lib
{
    public class BatchJob : INotifyPropertyChanged
    {
        private ReportList _reports;
        private int _sequence;
        private string _status;

        public SymDirectory Directory { get; set; }

        public int Sequence
        {
            get { return _sequence; }
            set
            {
                if (value == _sequence) return;
                _sequence = value;
                OnPropertyChanged();
            }
        }

        public string FileName { get; set; }

        public DateTime RunTime { get; set; }

        public string Status
        {
            get { return _status; }
            set
            {
                if (value == _status) return;
                _status = value;
                OnPropertyChanged();
            }
        }

        public ReportList Reports
        {
            get { return _reports; }
            set
            {
                if (value == _reports) return;
                _reports = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void AddReport(Report report)
        {
            report.Parent = this;
            Reports.Add(report);
        }
    }

    public class BatchJobs : ObservableCollection<BatchJob>
    {
    }

    public class ReportList : ObservableCollection<Report>
    {
    }
}