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
using System.Windows.Shapes;

namespace SPD1
{
    /// <summary>
    /// Interaction logic for Visualization.xaml
    /// </summary>
    public partial class Visualization : Window
    {
        const int GridHeight = 660;
        const int GridWidth = 1200;
        public Visualization(List<List<JobObject>> jobsList, int Cmax)
        {
            List<RowDefinition> Machines = new List<RowDefinition>();
            InitializeComponent();
            double unit = GridWidth / Cmax;
            RowDefinition timeRow = new RowDefinition();
            timeRow.Height = new GridLength(40);
            GridControl.RowDefinitions.Add(timeRow);
            Grid grid = new Grid();
            GridControl.Children.Add(grid);
            Grid.SetRow(grid, 0);
            for (int i = 0; i < Cmax; i++)
            {
                ColumnDefinition column = new ColumnDefinition();
                column.Width = new GridLength(GridWidth / Cmax);
                grid.ColumnDefinitions.Add(column);
                Rectangle rec = new Rectangle();
                rec.Stroke = new SolidColorBrush(Colors.Black);
                grid.Children.Add(rec);
                Grid.SetColumn(rec, i);
                TextBlock text = new TextBlock();
                text.Text = (i + 1).ToString();
                text.FontWeight = FontWeights.Bold;
                text.Foreground = new SolidColorBrush(Colors.Green);
                text.TextAlignment = TextAlignment.Center;
                text.VerticalAlignment = VerticalAlignment.Center;
                grid.Children.Add(text);
                Grid.SetColumn(text, i);
            }
            for(int i=0; i<jobsList.Count;i++)
            {
                Machines.Add(new RowDefinition());
                GridControl.RowDefinitions.Add(Machines[i]);
                Machines[i].Height = new GridLength(GridHeight / jobsList.Count);
                grid = new Grid();
                GridControl.Children.Add(grid);
                Grid.SetRow(grid, i+1);
                List<ColumnDefinition> Jobs = new List<ColumnDefinition>();
                int time = 0;
                int j = 0;
                foreach(JobObject job in jobsList[i])
                {
                    Jobs.Add(new ColumnDefinition());
                    grid.ColumnDefinitions.Add(Jobs.Last());
                    if (job.StartTime == time)
                    {
                        Jobs.Last().Width=new GridLength((job.StopTime- job.StartTime)*unit);
                        Rectangle rec = new Rectangle();
                        rec.Fill = new SolidColorBrush(Colors.Bisque);
                        grid.Children.Add(rec);
                        Grid.SetColumn(rec, j);
                        TextBlock text = new TextBlock();
                        text.Text = job.JobIndex.ToString();
                        text.FontWeight = FontWeights.Bold;
                        text.Foreground = new SolidColorBrush(Colors.Green);
                        text.TextAlignment = TextAlignment.Center;
                        text.VerticalAlignment = VerticalAlignment.Center;
                        grid.Children.Add(text);
                        Grid.SetColumn(text, j);
                        j++;
                    }
                    else
                    {
                        Jobs.Last().Width = new GridLength((job.StopTime - job.StartTime)*unit);
                        Jobs.Add(new ColumnDefinition());
                        grid.ColumnDefinitions.Add(Jobs.Last());
                        Jobs.Last().Width = new GridLength((job.StopTime - job.StartTime)*unit);
                        Rectangle rec = new Rectangle();
                        rec.Fill = new SolidColorBrush(Colors.Bisque);
                        grid.Children.Add(rec);
                        Grid.SetColumn(rec, j + 1);
                        TextBlock text = new TextBlock();
                        text.Text = job.JobIndex.ToString();
                        text.FontWeight = FontWeights.Bold;
                        text.Foreground = new SolidColorBrush(Colors.Green);
                        text.TextAlignment = TextAlignment.Center;
                        text.VerticalAlignment = VerticalAlignment.Center;
                        grid.Children.Add(text);
                        Grid.SetColumn(text, j+1);
                        j +=2;
                    }
                    time = job.StopTime;
                }
                grid.ShowGridLines = true;
            }
            GridControl.ShowGridLines = true;
        }
    }
}
