using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Visor.Annotations;

namespace Visor.ReportRunner
{
    public class BatchJob : INotifyPropertyChanged
    {
        private int _sequence;
        private string _status;
        private string _fileName;
        private ReportList _reports;

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

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

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

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Report
    {
        public string Title { get; set; }
        public int Sequence { get; set; }
    }

    public class BatchJobs : ObservableCollection<BatchJob>
    {
    }

    public class ReportList : ObservableCollection<Report>
    {
    }
}