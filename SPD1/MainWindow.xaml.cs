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

namespace SPD1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //LoadData dataToLoad = new LoadData();
            //dataToLoad.ReadFromFile();
            List<List<JobObject>> list = new List<List<JobObject>>();
            for (int i = 0; i < 50; i++)
            {
                list.Add(new List<JobObject>());
                for (int j = 0; j < 5; j++)
                {
                    list[i].Add(new JobObject
                    {
                        JobIndex = j,
                        StartTime = j*3,
                        StopTime = j*3+2
                    });
                }
            }
            Visualization vis = new Visualization(list,40);
            vis.Show();
        }
    }
}
