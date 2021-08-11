using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PGx.KB.Models;
using PGx.KB.Infrastructure.Abstract;
using System.App.Utility.Attributes;
namespace PGx.KB.Controllers
{
    [HandleError]
    [OptionalAuthorize(false)]
    public class AccountController : BaseController
    {
        // GET: Account
        IAuthProvider authProvider;
        public AccountController(IAuthProvider auth)
        {
            authProvider = auth;
        }
        public ViewResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (authProvider.Authenticate(model.UserName, model.Password))
                {
                    return Redirect(returnUrl ?? Url.Action("Index", "Home"));
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect username or password");
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        public ActionResult Logoff()
        {
            authProvider.SignOff();
            return RedirectToAction("Index","Home");
        }
    }
}
