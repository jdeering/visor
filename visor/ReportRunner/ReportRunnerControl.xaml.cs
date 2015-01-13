using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Visor.Options;

namespace Visor.ReportRunner
{
    /// <summary>
    ///     Interaction logic for ReportRunnerControl.xaml
    /// </summary>
    public partial class ReportRunnerControl : UserControl
    {
        //public static ObservableCollection<BatchJob> Jobs = new ObservableCollection<BatchJob>();

        public ReportRunnerControl()
        {
            InitializeComponent();
        }

        private BatchJobs _jobs
        {
            get { return ((BatchJobs) Resources["Jobs"]); }
        }

        public void AddBatchJob(string file, SymDirectory directory)
        {
            _jobs.Add(new BatchJob
                {
                    FileName = file,
                    Sequence = 0,
                    RunTime = DateTime.Now,
                    Status = "Initializing",
                    Directory = directory
                });
        }

        public void RemoveBatchJob()
        {
            _jobs.RemoveAt(_jobs.Count - 1);
        }

        public void SetSequence(int sequence)
        {
            _jobs.Last(x => x.Sequence == 0).Sequence = sequence;
        }

        public void UpdateStatus(int sequence, string status)
        {
            _jobs.Last(x => x.Sequence == sequence).Status = status;
        }

        public void UpdateStatus(string fileName, string status)
        {
            _jobs.Last(x => x.FileName == fileName).Status = status;
        }

        public BatchJob GetJob(int sequence)
        {
            return _jobs.Last(x => x.Sequence == sequence);
        }

        private void OpenReport(object sender, RoutedEventArgs e)
        {
            try
            {
                var report = (sender as MenuItem).CommandParameter as Report;
                report.Open();
            }
            catch
            {
            }
        }

        private void RunFileMaintenance(object sender, RoutedEventArgs e)
        {
            try
            {
                var report = (sender as MenuItem).CommandParameter as Report;
            }
            catch
            {
            }
        }
    }
}