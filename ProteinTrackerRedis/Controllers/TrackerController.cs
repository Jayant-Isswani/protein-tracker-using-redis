using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using Newtonsoft.Json;
using ProteinTrackerRedis.Models;
using ServiceStack.Redis;

namespace ProteinTrackerRedis.Controllers
{
    public class TrackerController : Controller
    {
        public ActionResult Index(long userId,int amount=0)
        {
            using(IRedisClient client=new RedisClient())
            {
                var userClient = client.As<User>();
                var user = userClient.GetById(userId);
                
                //var historyClient = client.As<int>();
                //var historyList = historyClient.Lists["urn:history:" + userId];
                if(amount>0)
                {
                    user.Total += amount;
                    if (user.History.Count() >= 5)
                    {
                        user.History.RemoveAt(0);
                    }
                    user.History.Add(amount);
                    userClient.Store(user);
                    client.AddItemToSortedSet("urn:leaderboard", user.Name, user.Total);
                }
                user.History.Reverse();
                ViewBag.HistoryItems = user.History;
                user.History.Reverse();
                ViewBag.UserName = user.Name;
                ViewBag.Total = user.Total;
                ViewBag.Goal = user.Goal;
                ViewBag.UserId = user.Id;
            }
            return View();
        }
    }
}
