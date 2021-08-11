using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.App.Utility.Attributes;
using PGx.KB.Models;
//using MaxMind.GeoIP;
using PGx.KB.services;

namespace PGx.KB.Controllers
{
   
        [OptionalAuthorize(true)]
        public class BaseController : Controller
        {

            public SiteSession CurrentSiteSession
            {
                get
                {
                    SiteSession session = (SiteSession)this.Session["SiteSession"];
                    return session;
                }
            }

            /// <summary>
            /// Get current user IP
            /// </summary>
            public string CurrentUserIPAddress
            {
                get
                {
                    return HttpContext.Request.UserHostAddress;
                }
            }

            /// <summary>
            /// Dispose the used resource.
            /// </summary>
            /// <param name="disposing">The disposing flag.</param>
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                }

                base.Dispose(disposing);
            }

            ~BaseController()
            {
                Dispose(false);
            }

            protected override void ExecuteCore()
            {
                int culture = 0;
                if (this.Session == null || this.Session["CurrentUICulture"] == null)
                {
                    int.TryParse(System.Configuration.ConfigurationManager.AppSettings["Culture"], out culture);
                    this.Session["CurrentUICulture"] = culture;
                }
                else
                {
                    culture = (int)this.Session["CurrentUICulture"];
                }
                //
                SiteSession.CurrentUICulture = culture;

                base.ExecuteCore();
            }

            /// <summary>
            /// Called when an unhandled exception occurs in the action.
            /// </summary>
            /// <param name="filterContext">Information about the current request and action.</param>
            protected override void OnException(ExceptionContext filterContext)
            {
                if (filterContext.Exception is UnauthorizedAccessException)
                {
                    filterContext.ExceptionHandled = true;
                    filterContext.Result = RedirectToAction("Home", "Index");
                }
                base.OnException(filterContext);
            }
        }
    }


