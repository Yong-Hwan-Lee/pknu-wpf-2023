using MahApps.Metro.Controls;
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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Wpf13_project1
{
    /// <summary>
    /// FindWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FindWindow : MetroWindow
    {
        public FindWindow()
        {
            InitializeComponent();
        }
        public FindWindow(string place_nm) : this()
        {
            
            BrsLocLight.Address = $"https://www.google.com/maps/dir/내 위치/{place_nm}";                     

        }

    }
}
