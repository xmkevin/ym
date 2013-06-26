using ServiceStack.ServiceInterface.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YM.Web.UI.ViewModels;

namespace YM.Web.UI.Controllers.Account
{
    public class AccountController : YMController
    {
        public IUserAuthRepository UserRep { get; set; }
        
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        
        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            UserRep.CreateUserAuth(new UserAuth()
            {
                 Id = 1,
                  DisplayName = model.DisplayName,
                   Email = model.Email
                    
            }, model.Password);

            return null;

        }
    }
}