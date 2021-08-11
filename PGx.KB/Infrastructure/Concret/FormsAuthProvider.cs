using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PGx.KB.Infrastructure.Abstract;
using System.Web.Security;
namespace PGx.KB.Infrastructure.Concret
{
    public class FormsAuthProvider : IAuthProvider
    {
        public bool Authenticate(string username, string password)
        {
            bool result = FormsAuthentication.Authenticate(username, password);
            if (result)
            {
                FormsAuthentication.SetAuthCookie(username, false);
            }
            return result;
        }

        public void SignOff()
        {
            FormsAuthentication.SignOut();
        }
    }
}