using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Video.Streaming.Models;
using Microsoft.AspNet.Identity;
using System.IO;

namespace Video.Streaming.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        ApplicationDbContext context;
        Entities ctx;

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public UserController()
        {
            ctx = new Entities();
            context = new ApplicationDbContext();
        }

        // GET: User
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult VideoList()
        {
            var assignedVideoList = ctx.GetAssignedVideosList(User.Identity.GetUserId());
            List<Videos> videos = new List<Videos>();
            foreach (var item in assignedVideoList)
            {


                videos.Add(new Videos
                {
                    Description = item.Description,
                    FilePath = "/api/videos/"+ Path.GetExtension(item.FilePath).Replace(".", "")+"/" + Path.GetFileNameWithoutExtension(item.FilePath),
                    Key = item.Key,
                    Name = item.Name,
                    Title = item.Title,
                    UserId = item.UserId,
                    VideoId = item.VideoId
                });
            }
            return View(videos);
        }
    }
}