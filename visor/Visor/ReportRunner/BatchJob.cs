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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class BatchJobs : ObservableCollection<BatchJob>
    {
    }
}