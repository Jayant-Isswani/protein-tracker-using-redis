using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using ProteinTrackerRedis.Models;
using ServiceStack.Redis;

namespace ProteinTrackerRedis.Controllers
{
    public class UserController : Controller
    {
       public ActionResult NewUser()
        {
            return View();
        }
        public ActionResult Save(string username,int goal,long? userId)
        {
            using (IRedisClient client = new RedisClient())
            {
                var userClient = client.As<User>();
                var clientHash=client.Hashes["urn:users:"+userId];
                User user;
                if (userId != null)
                {
                    user = userClient.GetById(userId);
                    //user = client.GetFromHash<User>("urn:users:" + userId);
                    client.RemoveItemFromSortedSet("urn:leaderboard",user.Name);
                }
                else
                {
                    user = new User
                    {
                        Id = userClient.GetNextSequence(),
                        History = new List<int>()
                    };
                }
                user.Name = username;
                user.Goal = goal;
                userClient.Store(user);
                //client.SetEntryInHash("urn:users:"+user.Id,"name",username);
                userId = user.Id;
                client.AddItemToSortedSet("urn:leaderboard", user.Name, user.Total);
            }
            return RedirectToAction("Index","Tracker",new { userId});
        }

        public ActionResult Edit(long userId)
        {
            using(IRedisClient client=new RedisClient())
            {
                var userClient = client.As<User>();
                var user = userClient.GetById(userId);
                ViewBag.UserName = user.Name;
                ViewBag.Goal = user.Goal;
                ViewBag.UserId = user.Id;
            }
            return View("NewUser");
        }

        public ActionResult Delete(long userId)
        {
            using(IRedisClient client=new RedisClient())
            {
                var userClient = client.As<User>();
                var user = userClient.GetById(userId);
                //Remove from leaderboard
                client.RemoveItemFromSortedSet("urn:leaderboard", user.Name);
                //Remove history
                client.Remove("urn:history:" + user.Id);

                userClient.DeleteById(userId);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
