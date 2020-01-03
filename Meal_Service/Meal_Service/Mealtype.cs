using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meal_Service
{
    public enum Mealtype
    {
        /// <summary>
        /// <param NONEALL = "급식정보가 모두 없을 때." ></param>
        /// </summary>
        NONEALL,


        /// <summary>
        /// <param NONEBREAKFAST = "아침 급식정보가 없을 때" ></param>
        /// </summary>
        NONEBREAKFAST,


        /// <summary>
        /// <param NONELUNCH = "점심 급식정보가 없을 때." ></param>
        /// </summary>
        NONELUNCH,


        /// <summary>
        /// <param NONEDINNER = "저녁 급식정보가 없을 때." ></param>
        /// </summary>
        NONEDINNER,
    }
}
