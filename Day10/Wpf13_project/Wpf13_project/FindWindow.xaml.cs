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

namespace Wpf13_project
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
        public FindWindow(double lat1, double lng1, double lat2, double lng2) : this()
        {
            BrsLocLight.Address = $"https://map.google.com/maps/dir/{lat1},{lng1}/{lat2},{lng2}";
            //BrsLocLight.Address = $"https://map.naver.com/v5/directions/{lat1},{lng1}/{lat2},{lng2}/car?c=15,0,0,0,dh"; 
            

        }

    }
}
