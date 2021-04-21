using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace SPD1
{
    /// <summary>
    /// Interaction logic for MakespansWindow.xaml
    /// </summary>
    public partial class MakespansWindow : Window//,INotifyPropertyChanged
    {
        public List<PermutationMakeSpan> permutationMakeSpanList;

        public MakespansWindow()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
