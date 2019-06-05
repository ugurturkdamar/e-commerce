using e_Ticaret.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace e_Ticaret.Filters
{
    public class Auth : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (CurrentSession.User == null)
            {
                filterContext.Result = new RedirectResult("/Home/Login");
            }
        }
    }
}