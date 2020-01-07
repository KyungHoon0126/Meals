using System;
using System.Collections.Generic;
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
using agi = HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using Meal_Service.Common;

namespace Meal_Service
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            nowdate.Text = DateTime.Now.ToString(string.Format("yyyy년 MM월 dd일 ddd요일 급식 정보", ComDef.cultures));
        }
        public void LoadMealDataAsync(int meal_option, string select_date, string dateInfo)
        {
            WebClient wc = new WebClient();
            wc.Headers["Content-Type"] = "application/json"; 
            wc.Encoding = Encoding.UTF8;

            string html = wc.DownloadString("데이터를 가져오는 파싱 주소");

            agi.HtmlDocument doc = new agi.HtmlDocument();
            doc.LoadHtml(html);

            string json = doc.Text;

            JObject jobj = JObject.Parse(json);

            if(jobj["mealServiceDietInfo"] == null)
            {
                return;
            }

            IsMealNone(jobj, Mealtype.NONEALL);

            string before_process_todays_meal = string.Empty;
            string temp_meal = string.Empty; 
            string complete_process_todays_meal = string.Empty;

            for (var i = 0; i < Convert.ToInt32(jobj["mealServiceDietInfo"][0]["head"][0]["list_total_count"]); i += 1)
            {
                before_process_todays_meal = jobj["mealServiceDietInfo"][1]["row"][i]["DDISH_NM"].ToString();

                temp_meal = Regex.Replace(before_process_todays_meal, @"\d", ""); 
                temp_meal = temp_meal.Replace("<br/>", " "); 
                complete_process_todays_meal = complete_process_todays_meal + temp_meal + Environment.NewLine + Environment.NewLine;
            }

            for (int i = 0; i < complete_process_todays_meal.Length; i++)
            {
                if (complete_process_todays_meal[i].Equals('('))
                {
                    for (int j = i; j < complete_process_todays_meal.Length; j++)
                    {
                        if (complete_process_todays_meal[j].Equals(')'))
                        {
                            complete_process_todays_meal = complete_process_todays_meal.Remove(i, j - i + 1);
                            break;
                        }
                    }
                }
            }

            complete_process_todays_meal = complete_process_todays_meal.Replace(".", "");

            string[] split_meal_info = complete_process_todays_meal.Split(new string[] { "\n" }, StringSplitOptions.None);

            IsMealNone(jobj, Mealtype.NONEBREAKFAST);
            IsMealNone(jobj, Mealtype.NONELUNCH);
            IsMealNone(jobj, Mealtype.NONEDINNER);

            string[] re_split_breakfast__info = split_meal_info[meal_option].Split(new string[] { " " }, StringSplitOptions.None);
            string[] re_split_lunch__info = split_meal_info[meal_option].Split(new string[] { " " }, StringSplitOptions.None); 
            string[] re_split_dinner__info = split_meal_info[meal_option].Split(new string[] { " " }, StringSplitOptions.None); 


            #region 급식 정보 나타내기
            /// <summary>
            /// meal_option이 0인 경우 : 아침(BreakFast)
            /// </summary>
            /// <param meal_option = "0" ></param>
            /// dateInfo를 통해 해당 급식 날짜를 전달 받음.
            if (meal_option == 0)
            {
                IsMealNone(jobj, Mealtype.NONEBREAKFAST);

                mealTitle.Text = dateInfo + "BreakFast Menu";

                for (var i = 0; i < re_split_breakfast__info.Length; i++)
                {
                    tbBreakFast.Text += re_split_breakfast__info[i] + Environment.NewLine + Environment.NewLine;
                }

                // Remove
                tbLunch.Text = string.Empty;
                tbDinner.Text = string.Empty;
            }


            /// <summary>
            /// meal_option이 0인 경우 : 점심(Lunch)
            /// </summary>
            /// <param meal_option = "2" ></param>
            /// dateInfo를 통해 해당 급식 날짜를 전달 받음.
            if (meal_option == 2) // 점심
            {
                IsMealNone(jobj, Mealtype.NONELUNCH);

                mealTitle.Text = dateInfo + "Lunch Menu";

                for (var i = 0; i < re_split_lunch__info.Length; i++)
                {
                    tbLunch.Text += re_split_lunch__info[i] + Environment.NewLine + Environment.NewLine;
                }

                // Remove
                tbBreakFast.Text = string.Empty;
                tbDinner.Text = string.Empty;
            }


            /// <summary>
            /// meal_option이 0인 경우 : 저녁(Dinner)
            /// </summary>
            /// <param meal_option = "4" ></param>
            /// dateInfo를 통해 해당 급식 날짜를 전달 받음.
            if (meal_option == 4)
            {
                IsMealNone(jobj, Mealtype.NONEDINNER);

                mealTitle.Text = dateInfo + "Dinner Menu";

                for (var i = 0; i < re_split_dinner__info.Length; i++)
                {
                    tbDinner.Text += re_split_dinner__info[i] + Environment.NewLine + Environment.NewLine;
                }

                // Remove
                tbBreakFast.Text = string.Empty;
                tbLunch.Text = string.Empty;
            }
        }
        #endregion

        private void IsMealNone(JObject jobj, Mealtype mealtype)
        {
            if (jobj["mealServiceDietInfo"] == null)
            {
                switch (mealtype)
                {
                    case Mealtype.NONEALL:
                        IsMealNoneAll();
                        break;
                    case Mealtype.NONEBREAKFAST:
                        IsMealNoneBreakFast();
                        break;
                    case Mealtype.NONELUNCH:
                        IsMealNoneLunch();
                        break;
                    case Mealtype.NONEDINNER:
                        IsMealNoneDinner();
                        break;
                }
            }
            else if(jobj["mealServiceDietInfo"][0]["head"][1]["RESULT"]["CODE"].ToString() != "INFO-000" && jobj["RESULT"]["CODE"].ToString() == "INFO-200")
            {
                switch (mealtype)
                {
                    case Mealtype.NONEALL:
                        IsMealNoneAll();
                        break;
                    case Mealtype.NONEBREAKFAST:
                        IsMealNoneBreakFast();
                        break;
                    case Mealtype.NONELUNCH:
                        IsMealNoneLunch();
                        break;
                    case Mealtype.NONEDINNER:
                        IsMealNoneDinner();
                        break;
                }
            }
        }

        private void IsMealNoneAll()
        {
            tbBreakFast.Text = ComDef.meal_None;
            tbLunch.Text = ComDef.meal_None;
            tbDinner.Text = ComDef.meal_None;
            mealTitle.Text = string.Empty;
        }
        private void IsMealNoneBreakFast()
        {
            tbBreakFast.Text = ComDef.meal_None;
            mealTitle.Text = string.Empty;
        }
        private void IsMealNoneLunch()
        {
            tbLunch.Text = ComDef.meal_None;
            mealTitle.Text = string.Empty;
        }
        private void IsMealNoneDinner()
        {
            tbDinner.Text = ComDef.meal_None;
            mealTitle.Text = string.Empty;
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
            ButtonCloseMenu.Visibility = Visibility.Visible;
        }
        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Visible;
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
        }

        private void breakfast_Selected(object sender, RoutedEventArgs e)
        {
            LoadMealDataAsync(0, ComDef.todays_date, ComDef.idx2);
        }
        private void lunch_Selected(object sender, RoutedEventArgs e)
        {
            LoadMealDataAsync(2, ComDef.todays_date, ComDef.idx2);
        }
        private void dinner_Selected(object sender, RoutedEventArgs e)
        {
            LoadMealDataAsync(4, ComDef.todays_date, ComDef.idx2);
        }

        private void Btn_BeforeDay_Meal_Info_Click(object sender, RoutedEventArgs e)
        {
            ComDef.todays_date = ComDef.before_day_date;
            nowdate.Text = DateTime.Now.AddDays(-1).ToString(string.Format("yyyy년 MM월 dd일 ddd요일 급식 정보", ComDef.cultures));

            ComDef.idx2 = ComDef.idx1;

            LoadMealDataAsync(0, ComDef.todays_date, ComDef.idx2);
            LoadMealDataAsync(2, ComDef.todays_date, ComDef.idx2);
            LoadMealDataAsync(4, ComDef.todays_date, ComDef.idx2);
        }

        private void Btn_Today_Meal_Info_Click(object sender, RoutedEventArgs e)
        {
            ComDef.todays_date = ComDef.return_todays_date;
            nowdate.Text = DateTime.Now.ToString(string.Format("yyyy년 MM월 dd일 ddd요일 급식 정보", ComDef.cultures));

            ComDef.idx2 = "Today's ";

            LoadMealDataAsync(0, ComDef.todays_date, ComDef.idx2);
            LoadMealDataAsync(2, ComDef.todays_date, ComDef.idx2);
            LoadMealDataAsync(4, ComDef.todays_date, ComDef.idx2);
        }

        private void Btn_NextDay_Meal_Info_Click(object sender, RoutedEventArgs e)
        {
            ComDef.todays_date = ComDef.next_day_date;
            nowdate.Text = DateTime.Now.AddDays(1).ToString(string.Format("yyyy년 MM월 dd일 ddd요일 급식 정보", ComDef.cultures));

            ComDef.idx2 = ComDef.idx3;

            LoadMealDataAsync(0, ComDef.todays_date, ComDef.idx2);
            LoadMealDataAsync(2, ComDef.todays_date, ComDef.idx2);
            LoadMealDataAsync(4, ComDef.todays_date, ComDef.idx2);
        }
    }
}