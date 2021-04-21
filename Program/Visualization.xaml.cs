using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System;

namespace SPD1
{
    /// <summary>
    /// Visualisation of Gannt Chart
    /// </summary>
    public partial class Visualization : Window
    {
        private Color textColor = Colors.Black;
        private Color fillColor1 = Colors.LightBlue;
        private Color fillColor2 = Colors.Orange;
        private const int GridHeight = 100;
        private const double unit = 40;

        public Visualization(List<List<JobObject>> jobsList, double elapsedTime, string algorithmName)
        {
            InitializeComponent();
            if (!IsListCorrect(jobsList))
            {
                MessageBox.Show("Niepoprawny wynik działania algorytmu!");
            }
            int Cmax = GetCMax(jobsList);
            TopText.Text = algorithmName + "    Total Makespan(Cmax): " + Cmax.ToString() + "    Algorithm time: " + elapsedTime.ToString() + "ms";
            //Lista rzędów dla maszyn
            List<RowDefinition> Machines = new List<RowDefinition>();
            //Pierwszy rząd na definicje jednostek czasu
            RowDefinition timeRow = new RowDefinition();
            timeRow.Height = new GridLength(40);
            GridControl.RowDefinitions.Add(timeRow);
            //Nowy grid na sam wykres
            Grid grid = new Grid();
            GridControl.Children.Add(grid);
            Grid.SetRow(grid, 1);
            //Kolumn tyle ile jest potrzebnych, o stałej szerokości unit
            for (int i = 0; i < Cmax; i++)
            {
                ColumnDefinition column = new ColumnDefinition();
                column.Width = new GridLength(unit);
                grid.ColumnDefinitions.Add(column);
                Rectangle rec = new Rectangle();
                rec.Stroke = new SolidColorBrush(Colors.Black);
                grid.Children.Add(rec);
                Grid.SetColumn(rec, i);
                TextBlock text = new TextBlock();
                text.Text = (i + 1).ToString();
                text.FontWeight = FontWeights.Bold;
                text.Foreground = new SolidColorBrush(textColor);
                text.TextAlignment = TextAlignment.Center;
                text.VerticalAlignment = VerticalAlignment.Center;
                grid.Children.Add(text);
                Grid.SetColumn(text, i);
            }
            for (int i = 0; i < jobsList.Count; i++)
            {
                //Dla każdej maszyny
                Machines.Add(new RowDefinition());
                GridControl.RowDefinitions.Add(Machines[i]);
                Machines[i].Height = new GridLength(GridHeight);
                grid = new Grid();
                GridControl.Children.Add(grid);
                // Pierwsze 2 rzędy są już zajęte
                Grid.SetRow(grid, i + 2);
                List<ColumnDefinition> Jobs = new List<ColumnDefinition>();
                int time = 0;
                int j = 0;
                int k = 0;
                foreach (JobObject job in jobsList[i])
                {
                    //Dla każdego zadania
                    Jobs.Add(new ColumnDefinition());
                    grid.ColumnDefinitions.Add(Jobs.Last());
                    //Jeśli zadanie zaczyna się odrazu po poprzednim
                    if (job.StartTime == time)
                    {
                        //Szerokośc jako długość trwania * unit
                        Jobs.Last().Width = new GridLength((job.StopTime - job.StartTime) * unit);
                        //Rectangle jako wypełnienie
                        Rectangle rec = new Rectangle();
                        if (k % 2 == 0)
                        {
                            rec.Fill = new SolidColorBrush(fillColor1);
                        }
                        else
                        {
                            rec.Fill = new SolidColorBrush(fillColor2);
                        }
                        rec.Stroke = new SolidColorBrush(textColor);
                        grid.Children.Add(rec);
                        Grid.SetColumn(rec, j);
                        //Text jako numer zadania
                        TextBlock text = new TextBlock();
                        text.Text = job.JobIndex.ToString();
                        text.FontWeight = FontWeights.Bold;
                        text.Foreground = new SolidColorBrush(textColor);
                        text.TextAlignment = TextAlignment.Center;
                        text.VerticalAlignment = VerticalAlignment.Center;
                        grid.Children.Add(text);
                        Grid.SetColumn(text, j);
                        j++;
                    }
                    else
                    //Jeśli jest przerwa między zadaniami
                    {
                        //Przerwa między zadaniami jako pusta komórka
                        Jobs.Last().Width = new GridLength((job.StartTime - time) * unit);
                        //Szerokośc jako długość trwania * unit
                        Jobs.Add(new ColumnDefinition());
                        grid.ColumnDefinitions.Add(Jobs.Last());
                        Jobs.Last().Width = new GridLength((job.StopTime - job.StartTime) * unit);
                        //Rectangle jako wypełnienie
                        Rectangle rec = new Rectangle();
                        if (k % 2 == 0)
                        {
                            rec.Fill = new SolidColorBrush(fillColor1);
                        }
                        else
                        {
                            rec.Fill = new SolidColorBrush(fillColor2);
                        }
                        rec.Stroke = new SolidColorBrush(textColor);
                        grid.Children.Add(rec);
                        Grid.SetColumn(rec, j + 1);
                        //Text jako numer zadania
                        TextBlock text = new TextBlock();
                        text.Text = job.JobIndex.ToString();
                        text.FontWeight = FontWeights.Bold;
                        text.Foreground = new SolidColorBrush(textColor);
                        text.TextAlignment = TextAlignment.Center;
                        text.VerticalAlignment = VerticalAlignment.Center;
                        grid.Children.Add(text);
                        Grid.SetColumn(text, j + 1);
                        j += 2;
                    }
                    time = job.StopTime;
                    k++;
                }
            }
            GridControl.ShowGridLines = true;
        }

        private int GetCMax(List<List<JobObject>> jobsList)
        {
            return jobsList.Last().Last().StopTime;
        }
        private bool IsListCorrect(List<List<JobObject>> jobsList)
        {
            //Rozpoczecie w chwili 0
            if (jobsList[0][0].StartTime != 0)
            {
                return false;
            }

            for (int i = 0; i < jobsList[0].Count; i++)
            {
                JobObject job = jobsList[0][i];
                for (int j = 1; j < jobsList.Count; j++)
                {
                    //Jesli zadanie i na maszynie j rozpoczyna się wcześniej niż na poprzedniej
                    // maszynie to jest źle
                    if (jobsList[j][i].StartTime < job.StopTime)
                    {
                        return false;
                    }
                    //Jeśli numer zadania się nie zgadza
                    if (jobsList[j][i].JobIndex != job.JobIndex)
                    {
                        return false;
                    }
                    job = jobsList[j][i];
                }
            }
            for (int i = 0; i < jobsList.Count; i++)
            {
                JobObject job = jobsList[i][0];
                for (int j = 1; j < jobsList[i].Count; j++)
                {
                    //Jesli kolejne zadanie j rozpoczyna się wcześniej niż poprzednie się
                    // zakończyło na tej samej maszynie to jest źle.
                    if (jobsList[i][j].StartTime < job.StopTime)
                    {
                        return false;
                    }
                    job = jobsList[i][j];
                }
            }
            return true;
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            GC.Collect(2, GCCollectionMode.Forced);
        }
    }
}
