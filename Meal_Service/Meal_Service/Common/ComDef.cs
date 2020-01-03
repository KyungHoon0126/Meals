using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meal_Service.Common
{
    public class ComDef
    {
        /// <summary>
        /// <param before_day_date = "전날" ></param>
        /// </summary>
        public static string before_day_date = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
        
        
        /// <summary>
        /// <param todays_date = "오늘" ></param>
        /// </summary>
        public static string todays_date = DateTime.Now.ToString("yyyyMMdd");


        /// <summary>
        /// <param next_day_date = "전날" ></param>
        /// </summary>
        public static string next_day_date = DateTime.Now.AddDays(1).ToString("yyyyMMdd");


        // 원래 날짜로 다시 되돌아 갈 때
        public static string return_todays_date = DateTime.Now.ToString("yyyyMMdd");


        // 요일 한글 변환
        public static CultureInfo cultures = CultureInfo.CreateSpecificCulture("ko-KR");


        // Before's, Today's, Tomorrow's 구분
        public static string idx1 = "Before's "; // 전날
        public static string idx2 = "Today's "; // 오늘
        public static string idx3 = "Tomorrow's ";  // 내일


        // 급식 정보가 없을 때 
        public static string meal_None = "급식 정보가 없습니다.";
    }
}
