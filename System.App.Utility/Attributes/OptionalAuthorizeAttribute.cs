using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;

namespace System.App.Utility.Attributes
{
    /// <summary>
    /// This attribute only decorates controller, not action. 
    /// </summary>
    public class OptionalAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly bool _authorize;

        public OptionalAuthorizeAttribute()
        {
            _authorize = true;
        }

        public OptionalAuthorizeAttribute(bool authorize)
        {
            _authorize = authorize;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!_authorize)
                return true;

            return base.AuthorizeCore(httpContext);
        }

#if false

        /// <remarks>
        /// ajax partial request returns login page problem
        /// http://stackoverflow.com/questions/8304689/ajax-actionlink-returns-login-page-within-div-when-user-need-to-login-again
        /// </remarks>
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                // It was an AJAX request => no need to redirect
                // to the login url, just return a JSON object
                // pointing to this url so that the redirect is done 
                // on the client

                var referrer = filterContext.HttpContext.Request.UrlReferrer;

                filterContext.Result = new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new
                    {
                        redirectTo = FormsAuthentication.LoginUrl +
                            "?ReturnUrl=" +
                        referrer.LocalPath.Replace("/", "%2f")
                    }
                };
            }
            else
                base.HandleUnauthorizedRequest(filterContext);
        }
#endif

    }
}
