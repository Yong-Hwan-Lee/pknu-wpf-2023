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

using Wpf13_project1.Logics;
using Wpf13_project1.Models;
using Newtonsoft.Json.Linq;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Windows.Media.Converters;
using CefSharp.DevTools.WebAuthn;
using System.Data.SqlClient;

namespace Wpf13_project1
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
            string openApiUri = "https://apis.data.go.kr/6260000/BusanCultureConcertDetailService/getBusanCultureConcertDetail?serviceKey=myBoxMMnVmVAd1WagamqC4MGu3klCbrVPo7RwKxHruybfCv%2FS02ivv8Ai2RziICOVkm221Q8FfQC4lkTJ9DznA%3D%3D&pageNo=1&numOfRows=1000&resultType=json";
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
            var resultCode = Convert.ToString(jsonResult["getBusanCultureConcertDetail"]["header"]["code"]);

            try
            {
                if (resultCode == "00")
                {
                    var data = jsonResult["getBusanCultureConcertDetail"]["item"];
                    var json_array = data as JArray;

                    var concertReservations = new List<ConcertReservation>();
                    foreach(var concert in json_array)   
                    {
                        concertReservations.Add(new ConcertReservation
                        {
                            res_no = Convert.ToInt32(concert["res_no"]),
                            title = Convert.ToString(concert["title"]),
                            op_st_dt = Convert.ToDateTime(concert["op_st_dt"]),
                            op_ed_dt = Convert.ToDateTime(concert["op_ed_dt"]),
                            op_at = Convert.ToString(concert["op_at"]),
                            place_id = Convert.ToString(concert["place_id"]),
                            place_nm = Convert.ToString(concert["place_nm"]),
                            theme = Convert.ToString(concert["theme"]),
                            runtime = Convert.ToString(concert["runtime"]),
                            showtime = Convert.ToString(concert["showtime"]),
                            rating = Convert.ToString(concert["rating"]),
                            price = Convert.ToString(concert["price"]),
                            original = Convert.ToString(concert["original"]),
                            casting = Convert.ToString(concert["casting"]),
                            crew = Convert.ToString(concert["crew"]),
                            avg_star = Convert.ToInt32(concert["avg_star"]),
                            enterprise = Convert.ToString(concert["enterprise"]),
                            dabom_url = Convert.ToString(concert["dabom_url"])
                        });
                    }
                    this.DataContext = concertReservations;
                    StsResult.Content = $"OpenAPI {concertReservations.Count} 건 조회완료";
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
                    var query = @"INSERT INTO concert
                                        (res_no,
                                        title,
                                        op_st_dt,
                                        op_ed_dt,
                                        op_at,
                                        place_id,
                                        place_nm,
                                        theme,
                                        showtime,
                                        rating,
                                        price,
                                        original,
                                        casting, 
                                        crew,
                                        avg_star,
                                        enterprise,
                                        dabom_url
                                        )
                                        VALUES
                                        (@res_no,
                                        @title,
                                        @op_st_dt,
                                        @op_ed_dt,
                                        @op_at,
                                        @place_id,
                                        @place_nm,
                                        @theme,
                                        @showtime,
                                        @rating,
                                        @price,
                                        @original,
                                        @casting, 
                                        @crew,
                                        @avg_star,
                                        @enterprise,
                                        @dabom_url)";
                    var insRes = 0;
                    foreach(var temp in GrdResult.Items)
                    {
                        if (temp is ConcertReservation)
                        {
                            var item = temp as ConcertReservation;

                            MySqlCommand cmd = new MySqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@res_no", item.res_no);                            
                            cmd.Parameters.AddWithValue("@title", item.title);
                            cmd.Parameters.AddWithValue("@op_st_dt", item.op_st_dt);
                            cmd.Parameters.AddWithValue("@op_ed_dt", item.op_ed_dt);
                            cmd.Parameters.AddWithValue("@op_at", item.op_at);
                            cmd.Parameters.AddWithValue("@place_id", item.place_id);
                            cmd.Parameters.AddWithValue("@place_nm", item.place_nm);
                            cmd.Parameters.AddWithValue("@theme", item.theme);
                            cmd.Parameters.AddWithValue("@runtime", item.runtime);
                            cmd.Parameters.AddWithValue("@showtime", item.showtime);
                            cmd.Parameters.AddWithValue("@rating", item.rating);
                            cmd.Parameters.AddWithValue("@price", item.price);
                            cmd.Parameters.AddWithValue("@original", item.original);
                            cmd.Parameters.AddWithValue("@casting", item.casting);
                            cmd.Parameters.AddWithValue("@crew", item.crew);
                            cmd.Parameters.AddWithValue("@avg_star", item.avg_star);
                            cmd.Parameters.AddWithValue("@enterprise", item.enterprise);
                            cmd.Parameters.AddWithValue("@dabom_url", item.dabom_url);

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

        private void Cboplace_nm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Cboplace_nm.SelectedValue != null)
            {
                //MessageBox.Show(Cbosigungu.SelectedValue.ToString());
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    conn.Open();
                    var query = @"SELECT    Id,
                                            res_no,
                                           title,
                                           op_st_dt,
                                            op_ed_dt,
                                            op_at,
                                            place_id,
                                            place_nm,
                                            theme,
                                            runtime,
                                            showtime,
                                            rating,
                                            price,
                                            original,
                                            casting,
                                            crew,
                                            avg_star,
                                            enterprise,
                                            dabom_url
                                        FROM concert
                                        WHERE place_nm = @place_nm";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@place_nm", Cboplace_nm.SelectedValue.ToString());
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds, "concert");
                    List<ConcertReservation> concertReservations = new List<ConcertReservation>();
                    foreach (DataRow row in ds.Tables["concert"].Rows)
                    {
                        concertReservations.Add(new ConcertReservation
                        {
                            Id = Convert.ToInt32(row["id"]),
                            res_no = Convert.ToInt32(row["res_no"]),
                            title = Convert.ToString(row["title"]),
                            op_st_dt = Convert.ToDateTime(row["op_st_dt"]),
                            op_ed_dt = Convert.ToDateTime(row["op_ed_dt"]),
                            op_at = Convert.ToString(row["op_at"]),
                            place_id = Convert.ToString(row["place_id"]),
                            place_nm = Convert.ToString(row["place_nm"]),
                            theme = Convert.ToString(row["theme"]),
                            runtime = Convert.ToString(row["runtime"]),
                            showtime = Convert.ToString(row["showtime"]),
                            rating = Convert.ToString(row["rating"]),
                            price = Convert.ToString(row["price"]),
                            original = Convert.ToString(row["original"]),
                            casting = Convert.ToString(row["casting"]),
                            crew = Convert.ToString(row["crew"]),
                            avg_star = Convert.ToInt32(row["avg_star"]),
                            enterprise = Convert.ToString(row["enterprise"]),
                            dabom_url = Convert.ToString(row["dabom_url"])
                        });
                    }

                    this.DataContext = concertReservations;
                    isFavorite = false;
                    StsResult.Content = $"콘서트 {concertReservations.Count}개 조회완료";

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
            var selItem = GrdResult.SelectedItem as ConcertReservation;
            var mapWindow = new MapWindow(selItem.dabom_url);
            mapWindow.Owner = this;
            mapWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            mapWindow.ShowDialog();

        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
            {
                conn.Open();
                var query = @"SELECT place_nm AS Save_Date
                                      FROM concert
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

                Cboplace_nm.ItemsSource = saveDateList;

            }
        }

        private async void BtnFind_Click(object sender, RoutedEventArgs e)
        {
        //    var FindWindow = new FindWindow();
        //    FindWindow.Owner = this;
        //    FindWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        //    FindWindow.ShowDialog();
            if (GrdResult.SelectedItems.Count != 1)
            {
                await Commons.ShowMessageAsync("오류", "목적지를 선택하세요");
            }
            else
            {
                // 경로표시
                var selItem = GrdResult.SelectedItem as ConcertReservation;                               
                string place_nm = selItem.place_nm;                
                var findWindow = new FindWindow(selItem.place_nm);
                
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
        bool isFavorite = false;
        private async void BtnFavorite_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.SelectedItems.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "즐겨찾기에 추가할 콘서트를 선택하세요(복수선택 가능");
                return;
            }

            if (isFavorite)
            {
                await Commons.ShowMessageAsync("오류", "이미 즐겨찾기한 콘서트입니다.");
                return;
            }

            //List<FavoriteConcert> list = new List<FavoriteConcert>();
            //foreach (ConcertReservation item in GrdResult.SelectedItems)
            //{
            //    var favoriteConcert = new FavoriteConcert
            //    {
            //        Id = item.Id,
            //        res_no = item.res_no,
            //        title = item.title,
            //        op_st_dt = item.op_st_dt,
            //        op_ed_dt = item.op_ed_dt,
            //        op_at = item.op_at,
            //        place_id = item.place_id,
            //        place_nm = item.place_nm,
            //        runtime = item.runtime,
            //        showtime = item.showtime,
            //        rating = item.rating,
            //        price = item.price,
            //        original = item.original,
            //        casting = item.casting,
            //        crew = item.crew,
            //        avg_star = item.avg_star,
            //        enterprise = item.enterprise,
            //        dabom_url = item.dabom_url,

            //    };
            //    list.Add(favoriteConcert);
            //}


            try
            {
                // MySQL DB 데이터 입력
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    var query = @"INSERT INTO favoriteconcert
                                    (Id,
                                    res_no,
                                    title,
                                    op_st_dt,
                                    op_ed_dt,
                                    op_at,
                                    place_id,
                                    place_nm,
                                    theme,
                                    runtime,
                                    showtime,
                                    rating,
                                    price,
                                    original,
                                    casting,
                                    crew,
                                    avg_star,
                                    enterprise,
                                    dabom_url)
                                    VALUES
                                    (@Id,
                                    @res_no,
                                    @title,
                                    @op_st_dt,
                                    @op_ed_dt,
                                    @op_at,
                                    @place_id,
                                    @place_nm,
                                    @theme,
                                    @runtime,
                                    @showtime,
                                    @rating,
                                    @price,
                                    @original,
                                    @casting,
                                    @crew,
                                    @avg_star,
                                    @enterprise,
                                    @dabom_url);
                                    ";
                    var insRes = 0;
                    foreach (FavoriteConcert item in GrdResult.SelectedItems)
                    {
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Id", item.Id);
                        cmd.Parameters.AddWithValue("@res_no", item.res_no);
                        cmd.Parameters.AddWithValue("@title", item.title);
                        cmd.Parameters.AddWithValue("@op_st_dt", item.op_st_dt);
                        cmd.Parameters.AddWithValue("@op_ed_dt", item.op_ed_dt);
                        cmd.Parameters.AddWithValue("@op_at", item.op_at);
                        cmd.Parameters.AddWithValue("@place_id", item.place_id);
                        cmd.Parameters.AddWithValue("@place_nm", item.place_nm);
                        cmd.Parameters.AddWithValue("@theme", item.theme);
                        cmd.Parameters.AddWithValue("@runtime", item.runtime);
                        cmd.Parameters.AddWithValue("@showtime", item.showtime);
                        cmd.Parameters.AddWithValue("@rating", item.rating);
                        cmd.Parameters.AddWithValue("@price", item.price);
                        cmd.Parameters.AddWithValue("@original", item.original);
                        cmd.Parameters.AddWithValue("@casting", item.casting);
                        cmd.Parameters.AddWithValue("@crew", item.crew);
                        cmd.Parameters.AddWithValue("@avg_star", item.avg_star);
                        cmd.Parameters.AddWithValue("@enterprise", item.enterprise);
                        cmd.Parameters.AddWithValue("@dabom_url", item.dabom_url);
                        insRes += cmd.ExecuteNonQuery();
                    }
                    if (GrdResult.SelectedItems.Count == insRes)
                    {
                        await Commons.ShowMessageAsync("저장", "DB저장성공");
                        StsResult.Content = $"즐겨찾기 {insRes} 건 저장완료";
                    }

                    else
                    {
                        await Commons.ShowMessageAsync("저장", "DB저장오류 관리자에게 문의하세요");

                    }

                    //var result = cmd.ExecuteScalar();
                    // await Commons.ShowMessageAsync("데이터 갯수", result.ToString());
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB 저장오류 {ex.Message}");
            };

        }

        private async void BtnViewFavorite_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = null;

            List<FavoriteConcert> list = new List<FavoriteConcert>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();

                    var query = @"SELECT Id,
                                    res_no,
                                    title,
                                    op_st_dt,
                                    op_ed_dt,
                                    op_at,
                                    place_id,
                                    place_nm,
                                    theme,
                                    runtime,
                                    showtime,
                                    rating,
                                    price,
                                    original,
                                    casting,
                                    crew,
                                    avg_star,
                                    enterprise,
                                    dabom_url
                                 FROM FavoriteConcert
                                 ORDER BY Id ASC";
                    var cmd = new MySqlCommand(query, conn);
                    var adapter = new MySqlDataAdapter(cmd);
                    var dSet = new DataSet();
                    adapter.Fill(dSet, "FavoriteConcert");

                    foreach (DataRow dr in dSet.Tables["FavoriteConcert"].Rows)
                    {
                        list.Add(new FavoriteConcert
                        {
                            Id = Convert.ToInt32(dr["id"]),
                            res_no = Convert.ToInt32(dr["res_no"]),
                            title = Convert.ToString(dr["title"]),
                            op_st_dt = Convert.ToDateTime(dr["op_st_dt"]),
                            op_ed_dt = Convert.ToDateTime(dr["op_ed_dt"]),
                            op_at = Convert.ToString(dr["op_at"]),
                            place_id = Convert.ToString(dr["place_id"]),
                            place_nm = Convert.ToString(dr["place_nm"]),
                            theme = Convert.ToString(dr["theme"]),
                            runtime = Convert.ToString(dr["runtime"]),
                            showtime = Convert.ToString(dr["showtime"]),
                            rating = Convert.ToString(dr["rating"]),
                            price = Convert.ToString(dr["price"]),
                            original = Convert.ToString(dr["original"]),
                            casting = Convert.ToString(dr["casting"]),
                            crew = Convert.ToString(dr["crew"]),
                            avg_star = Convert.ToInt32(dr["avg_star"]),
                            enterprise = Convert.ToString(dr["enterprise"]),
                            dabom_url = Convert.ToString(dr["dabom_url"])
                        });
                    }

                    this.DataContext = list;
                    isFavorite = true;
                    StsResult.Content = $"즐겨찾기 {list.Count} 건 조회완료";

                }

            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB조회 오류 {ex.Message}");
            }

        }

        private async void BtnDel_Click(object sender, RoutedEventArgs e)
        {

            if (isFavorite == false)
            {
                await Commons.ShowMessageAsync("오류", "즐겨찾기만 삭제할 수 있습니다.");
                return;
            }

            if (GrdResult.SelectedItems.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "삭제할 영화를 선택하세요.");
                return;
            }

            try //삭제
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();

                    var query = "DELETE FROM FavoriteConcert Where Id = @Id";
                    var delRes = 0;

                    foreach (FavoriteConcert item in GrdResult.SelectedItems)
                    {
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Id", item.Id);

                        delRes += cmd.ExecuteNonQuery();
                    }

                    if (delRes == GrdResult.SelectedItems.Count)
                    {
                        await Commons.ShowMessageAsync("삭제", "DB삭제 성공!!");
                        StsResult.Content = $"즐겨찾기 {delRes} 건 삭제완료";    // 화면에 안나옴  

                    }
                    else
                    {
                        await Commons.ShowMessageAsync("삭제", "DB삭제 일부성공!!");    // 발생할일이 거의 전무
                    }
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB삭제 오류 {ex.Message}");
            }

            BtnViewFavorite_Click(sender, e);       // 즐겨찾기 보기 이벤트핸들러를 한번 실행
        }

    }
    
}
