using System.Collections.Generic;
using System.Windows;
using SPD1.Misc;

namespace SPD1
{
    public partial class RPQViewer : Window
    {
        private List<RPQJob> _rpqJobs;

        public List<RPQJob> RPQJobs
        {
            get { return _rpqJobs; }
            set { _rpqJobs = value; }
        }

        public RPQViewer(List<RPQJob> jobs)
        {
            RPQJobs = jobs;
            InitializeComponent();
            DataContext = this;
        }
    }
}
