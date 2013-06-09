using ServiceStack.Mvc;
using ServiceStack.Mvc.MiniProfiler;
using ServiceStack.ServiceInterface.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YM.Web.UI.Controllers
{
    [ProfilingActionFilter]
    public class YMController : ServiceStackController
    {

    }
}