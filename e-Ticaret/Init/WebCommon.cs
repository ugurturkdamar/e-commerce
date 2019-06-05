using e_Ticaret.Entities;
using eTicaret.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace e_Ticaret.Init
{
    public class WebCommon : ICommon
    {
        public string GetCurrentUsername()
        {
            if(HttpContext.Current.Session["login"] != null)
            {
                TicaretUser user = HttpContext.Current.Session["login"] as TicaretUser;
                return user.Username;
            }
            return "system";
        }
    }
}