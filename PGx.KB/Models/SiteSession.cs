using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Globalization;
using PGx.Model.Enums;

namespace PGx.KB.Models
{
    public class SiteSession
    {
        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        public int UserID { get; set; }
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; }
        ///// <summary>
        ///// Gets or sets the user role.
        ///// </summary>
        //public UserRoles UserRole { get; set; }
        /// <summary>
        /// Gets or sets the current UI culture.
        /// </summary>
        /// <remarks>
        /// Values meaning: 0 = InvariantCulture (en-US), 1 = en-US, 2 = de-DE, 3 = zh-CN
        /// </remarks>
        public static int CurrentUICulture
        {
            get
            {
                if (Thread.CurrentThread.CurrentUICulture.Name == "zh-CN")
                    return 1;
                else if (Thread.CurrentThread.CurrentUICulture.Name == "en-US")
                    return 2;
                //else if (Thread.CurrentThread.CurrentUICulture.Name == "da-DK")
                //    return 3;
                //else if (Thread.CurrentThread.CurrentUICulture.Name == "pt-BR")
                    //return 4;
                else
                    return GlobalSetting.DemoMode ? 2 : 1;
            }
            set
            {
                //
                // Set the thread's CurrentUICulture.
                //
                if (value == 1)
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CN");
                else if (value == 2)
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                //else if (value == 3)
                //    Thread.CurrentThread.CurrentUICulture = new CultureInfo("da-DK");
                //else if (value == 4)
                //    Thread.CurrentThread.CurrentUICulture = new CultureInfo("pt-BR");
                else
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(GlobalSetting.DemoMode ? "en-US" : "zh-CN"); // Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

                //
                // Set the thread's CurrentCulture the same as CurrentUICulture.
                //
                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;
            }
        }
    }
}