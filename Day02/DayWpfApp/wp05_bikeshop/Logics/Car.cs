using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace wp05_bikeshop.Logics
{
    internal class Car
    {
        public string Names { get; set; }    // Auto Property
        public double Speed { get; set; }
        public Color Colors { get; set; }
        public Human driver { get; set; }
    }
}
