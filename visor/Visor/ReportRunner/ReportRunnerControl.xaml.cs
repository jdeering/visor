using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Visor.ReportRunner
{
    /// <summary>
    /// Interaction logic for ReportRunnerControl.xaml
    /// </summary>
    public partial class ReportRunnerControl : UserControl
    {
        //public static ObservableCollection<BatchJob> Jobs = new ObservableCollection<BatchJob>();

        private BatchJobs _jobs
        {
            get { return ((BatchJobs) Resources["Jobs"]); }
            set { Resources["Jobs"] = value; }
        }

        public ReportRunnerControl()
        {
            InitializeComponent();
        }

        public void AddReport(string file)
        {
            _jobs.Add(new BatchJob
                {
                    FileName = file,
                    Sequence = 0,
                    RunTime = DateTime.Now,
                    Status = "Initializing"
                });
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
    }
}
