using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf13_project.Models
{
    public class TrafficLight
    {
        /*"mgrnu":"9077",
         * "road":"",
         * "ins_place":"벡스코역앞",
         * "gubun":"전자신호 제어",
         * "ins_date":"2020-01-01",
         * "road_type":"교차로",
         * "sigungu":"해운대구",
         * "address":"부산광역시 해운대구 우동 1119-57",
         * "lat":"35.17137542",
         * "lng":"129.1352778",
         * "confirm_date":"2020-12-31"
    */
        public int Id { get; set; }
        public int Mgrnu { get; set; }
        public string Road { get; set; }
        public string Ins_place { get; set; }
        public string Gubun { get; set; }
        public DateTime Ins_date { get; set; }
        public string Road_type { get; set; }
        public string Sigungu { get; set; }
        public string Address { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public DateTime Confirm_date { get; set; }
    }
}
