using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf13_project1.Models
{
    public class FavoriteConcert
    {
        public int Id { get; set; }
        public int res_no { get; set; }
        public string title { get; set; }
        public DateTime op_st_dt { get; set; }
        public DateTime op_ed_dt { get; set; }
        public string op_at { get; set; }
        public string place_id { get; set; }
        public string place_nm { get; set; }
        public string theme { get; set; }
        public string runtime { get; set; }
        public string showtime { get; set; }
        public string rating { get; set; }
        public string price { get; set; }
        public string original { get; set; }
        public string casting { get; set; }
        public string crew { get; set; }
        public int avg_star { get; set; }
        public string enterprise { get; set; }
        public string dabom_url { get; set; }
    }
}
