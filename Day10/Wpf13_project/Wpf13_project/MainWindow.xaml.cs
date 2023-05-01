using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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

using Wpf13_project.Logics;
using Wpf13_project.Models;
using Newtonsoft.Json.Linq;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Windows.Media.Converters;
using CefSharp.DevTools.WebAuthn;

namespace Wpf13_project
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //부산시 openAPI조회
        
        private async void BtnReference_Click(object sender, RoutedEventArgs e)
        {
            string openApiUri = "https://apis.data.go.kr/6260000/BusanTrafficLightInfoService/getWarningLightInfo?serviceKey=myBoxMMnVmVAd1WagamqC4MGu3klCbrVPo7RwKxHruybfCv%2FS02ivv8Ai2RziICOVkm221Q8FfQC4lkTJ9DznA%3D%3D&pageNo=1&numOfRows=5000&resultType=json";
            string result = string.Empty;

            WebRequest req = null;
            WebResponse resp = null;
            StreamReader reader = null;

            try
            {
                req = WebRequest.Create(openApiUri);
                resp = await req.GetResponseAsync();
                reader = new StreamReader(resp.GetResponseStream());
                result = reader.ReadToEnd();

                Debug.WriteLine(result);
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류",$"OpenAPI 조회오류 {ex.Message}");
            }
            
            var jsonResult = JObject.Parse(result);
            //var status = Convert.ToInt32(jsonResult["status"]);
            var resultCode = Convert.ToString(jsonResult["getWarningLightInfo"]["header"]["resultCode"]);

            try
            {
                if (resultCode == "00")
                {
                    var data = jsonResult["getWarningLightInfo"]["body"]["items"]["item"];
                    var json_array = data as JArray;

                    var trafficLocations = new List<TrafficLight>();
                    foreach(var location in json_array)   
                    {
                        trafficLocations.Add(new TrafficLight
                        {
                            Mgrnu = Convert.ToInt32(location["mgrnu"]),
                            Road = Convert.ToString(location["road"]),
                            Ins_place = Convert.ToString(location["ins_place"]),
                            Gubun = Convert.ToString(location["gubun"]),
                            Ins_date = Convert.ToDateTime(location["ins_date"]),
                            Road_type = Convert.ToString(location["road_type"]),
                            Sigungu = Convert.ToString(location["sigungu"]),
                            Address = Convert.ToString(location["address"]),
                            Lat = Convert.ToDouble(location["lat"]),
                            Lng = Convert.ToDouble(location["lng"]),
                            Confirm_date = Convert.ToDateTime(location["confirm_date"])
                        });
                    }
                    this.DataContext = trafficLocations;
                    StsResult.Content = $"OpenAPI {trafficLocations.Count} 건 조회완료";
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"JSON 처리오류 {ex.Message}");
            }
            
        }

        private async void BtnSaveData_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.Items.Count ==0)
            {
                await Commons.ShowMessageAsync("오류", "조회하고 저장하세요.");
                return;
            }
            try
            {
                using(MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    if (conn.State == System.Data.ConnectionState.Closed) conn.Open();
                    var query = @"INSERT INTO trafficlightlocation
                                        (Mgrnu,
                                        Road,
                                        Ins_place,
                                        Gubun,
                                        Ins_date,
                                        Road_type,
                                        Sigungu,
                                        Address,
                                        Lat,
                                        Lng,
                                        Confirm_date)
                                        VALUES
                                        (@Mgrnu,
                                        @Road,
                                        @Ins_place,
                                        @Gubun,
                                        @Ins_date,
                                        @Road_type,
                                        @Sigungu,
                                        @Address,
                                        @Lat,
                                        @Lng,
                                        @Confirm_date)";
                    var insRes = 0;
                    foreach(var temp in GrdResult.Items)
                    {
                        if (temp is TrafficLight)
                        {
                            var item = temp as TrafficLight;

                            MySqlCommand cmd = new MySqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@mgrnu", item.Mgrnu);
                            cmd.Parameters.AddWithValue("@road", item.Road);
                            cmd.Parameters.AddWithValue("@ins_place", item.Ins_place);
                            cmd.Parameters.AddWithValue("@gubun", item.Gubun);
                            cmd.Parameters.AddWithValue("@ins_date", item.Ins_date);
                            cmd.Parameters.AddWithValue("@road_type", item.Road_type);
                            cmd.Parameters.AddWithValue("@sigungu", item.Sigungu);
                            cmd.Parameters.AddWithValue("@address", item.Address);
                            cmd.Parameters.AddWithValue("@lat", item.Lat);
                            cmd.Parameters.AddWithValue("@lng", item.Lng);
                            cmd.Parameters.AddWithValue("@confirm_date", item.Confirm_date);

                            insRes += cmd.ExecuteNonQuery();
                        }
                    }
                    await Commons.ShowMessageAsync("저장", "DB저장 성공!!");
                    StsResult.Content = $"DB저장 {insRes}건 성공";
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB저장오류!{ex.Message}");
            }

        }

        private void Cbosigungu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Cbosigungu.SelectedValue != null)
            {
                //MessageBox.Show(Cbosigungu.SelectedValue.ToString());
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    conn.Open();
                    var query = @"SELECT    Id,
                                            Mgrnu,
                                           Road,
                                           Ins_place,
                                            Gubun,
                                            Ins_date,
                                            Road_type,
                                            Sigungu,
                                            Address,
                                            Lat,
                                            Lng,
                                            Confirm_date
                                        FROM trafficlightlocation
                                        WHERE sigungu = @sigungu";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@sigungu", Cbosigungu.SelectedValue.ToString());
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds, "trafficlightlocation");
                    List<TrafficLight> trafficLocations = new List<TrafficLight>();
                    foreach (DataRow row in ds.Tables["trafficlightlocation"].Rows)
                    {
                        trafficLocations.Add(new TrafficLight
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            Mgrnu = Convert.ToInt32(row["Mgrnu"]),   // mysql 컬럼이름에 대소문자 구분없이 쓰기때문에
                            Road = Convert.ToString(row["Road"]),
                            Ins_place = Convert.ToString(row["Ins_place"]),
                            Gubun = Convert.ToString(row["Gubun"]),
                            Ins_date = Convert.ToDateTime(row["Ins_date"]),
                            Road_type = Convert.ToString(row["Road_type"]),
                            Sigungu = Convert.ToString(row["Sigungu"]),
                            Address = Convert.ToString(row["Address"]),
                            Lat = Convert.ToDouble(row["Lat"]),
                            Lng = Convert.ToDouble(row["Lng"]),
                            Confirm_date = Convert.ToDateTime(row["Confirm_date"])
                        });
                    }

                    this.DataContext = trafficLocations;
                    StsResult.Content = $"신호등 {trafficLocations.Count}개 조회완료";

                }
            }
            else
            {
                this.DataContext = null;
                StsResult.Content = $"DB 조회 클리어";
            }
        }

        private void GrdResult_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selItem = GrdResult.SelectedItem as TrafficLight;
            var mapWindow = new MapWindow(selItem.Lat, selItem.Lng);
            mapWindow.Owner = this;
            mapWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            mapWindow.ShowDialog();

        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
            {
                conn.Open();
                var query = @"SELECT sigungu AS Save_Date
                                      FROM trafficlightlocation
                                       GROUP BY 1
                                        ORDER BY 1";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                List<string> saveDateList = new List<string>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    saveDateList.Add(Convert.ToString(row["Save_Date"]));
                }

                Cbosigungu.ItemsSource = saveDateList;

            }
        }

        private async void BtnFind_Click(object sender, RoutedEventArgs e)
        {
        //    var FindWindow = new FindWindow();
        //    FindWindow.Owner = this;
        //    FindWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        //    FindWindow.ShowDialog();
            if (GrdResult.SelectedItems.Count != 2)
            {
                await Commons.ShowMessageAsync("오류", "2개를 선택하세요");
            }
            else
            {
                // 경로표시
                var selItem1 = GrdResult.SelectedItems[0] as TrafficLight;
                var selItem2 = GrdResult.SelectedItems[1] as TrafficLight;
                double lat1 = selItem1.Lat;
                double lng1 = selItem1.Lng;
                double lat2 = selItem2.Lat;
                double lng2 = selItem2.Lng;
                var findWindow = new FindWindow(selItem1.Lat, selItem1.Lng, selItem2.Lat, selItem2.Lng);
                
                //var selItem1 = GrdResult.SelectedItems[0] as TrafficLight;
                //var selItem2 = GrdResult.SelectedItems[1] as TrafficLight;
                //string ins_place1 = selItem1.Ins_place;
                //string ins_place2 = selItem2.Ins_place;

                //var findWindow = new FindWindow(selItem1.Ins_place, selItem2.Ins_place);               


                // 창띄움
                findWindow.Owner = this;
                findWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                findWindow.ShowDialog();


            
            }
            
        }
    }
}
