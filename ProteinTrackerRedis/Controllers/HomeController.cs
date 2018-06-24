using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProteinTrackerRedis.Models;
using ServiceStack.Redis;

namespace ProteinTrackerRedis.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (IRedisClient client = new RedisClient())
            {
                var userClient = client.As<User>();
                var users = userClient.GetAll();
                var userSelection = new SelectList(users,"Id","Name");
                ViewBag.UserId = userSelection;
            }
                return View();
        }

        //public ActionResult About()
        //{
        //    ViewBag.Message = "Your application description page.";

        //    return View();
        //}

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}
    }
}