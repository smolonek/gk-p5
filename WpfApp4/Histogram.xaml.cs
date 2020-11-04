using System;
using System.Collections.Generic;
using System.Drawing;
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

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for Histogram.xaml
    /// </summary>
    public partial class Histogram : Window
    {
        public List<KeyValuePair<int, int>> greenList { get; set; }
        public List<KeyValuePair<int, int>> redList { get; set; }
        public Histogram(List<KeyValuePair<int, int>> r, List<KeyValuePair<int, int>> g, List<KeyValuePair<int, int>> blueList)
        {
            this.redList = r;
            this.greenList = g;
            InitializeComponent();
            Red.DataContext = redList;
            Green.DataContext = greenList;
            Blue.DataContext = blueList;
        }
    }
}
