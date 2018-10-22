using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Video.Streaming.Models;

namespace Video.Streaming.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        ApplicationDbContext context;
        Entities ctx;
        public AdminController()
        {
            ctx = new Entities();
            context = new ApplicationDbContext();
        }

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SelectUser(string userId)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            var assignedVideoList = ctx.GetAssignedVideosList(userId);
            var videoList = (from x in ctx.VideoFiles select x).ToList();
            data.Add("AssignedVideoList", assignedVideoList);
            data.Add("VideoList", videoList);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET: Admin
        public ActionResult AssignVideos()
        {
            AssignVideoModel model = new AssignVideoModel();
            model.UserList = new SelectList(context.Users.ToList(), "Id", "Email");
            model.VideoList = new SelectList(ctx.VideoFiles.ToList(), "Id", "Name");
            model.AssignedVideoList = new SelectList(new List<VideoFiles>(), "Id", "Name");

            return View(model);
        }

        [HttpPost]
        public ActionResult AssignVideos(AssignVideoModel model)
        {

            model.UserList = new SelectList(context.Users.ToList(), "Id", "Email");
            model.VideoList = new SelectList(ctx.VideoFiles.ToList(), "Id", "Name");

            if (string.IsNullOrEmpty(model.VideoIds))
            {
                model.AssignedVideoList = new SelectList(new List<VideoFiles>(), "Id", "Name");
            }

            if (!string.IsNullOrEmpty(model.VideoIds))
            {
                string[] assignedVideoList = model.VideoIds.Split(',');

                foreach (var vid in assignedVideoList)
                {
                    long? id = Convert.ToInt64(vid);

                    var alreadySave = (from x in ctx.AssignedVideos where x.VideoId == id select x).ToList();

                    if (alreadySave.Count > 0)
                    {

                    }
                    else
                    {
                        AssignedVideo video = new AssignedVideo();
                        video.UserId = model.UserId;
                        video.VideoId = Convert.ToInt64(vid);
                        ctx.AssignedVideos.Add(video);
                        ctx.SaveChanges();
                    }
                }
            }
            else
                model.VideoList = new SelectList(new List<VideoFiles>(), "Id", "Name");


            model.UserId = string.Empty;

            ViewBag.Message = "Videos assigned successfully;";
            return AssignVideos();
        }


        [HttpGet]
        public ActionResult VideoList()
        {
            List<VideoFiles> videolist = new List<VideoFiles>();
            string CS = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand("spGetAllVideoFile", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    VideoFiles video = new VideoFiles();
                    video.ID = Convert.ToInt32(rdr["ID"]);
                    video.Name = rdr["Name"].ToString();
                    video.Title = rdr["Title"].ToString();
                    video.Key = rdr["Key"].ToString();
                    video.Description = rdr["Description"].ToString();
                    video.FileSize = Convert.ToInt32(rdr["FileSize"]);
                    video.FilePath = rdr["FilePath"].ToString();

                    videolist.Add(video);
                }
            }
            return View(videolist);
        }

        [HttpPost]
        public ActionResult UploadVideo(HttpPostedFileBase fileupload)
        {
            if (fileupload != null)
            {
                string fileName = Path.GetFileName(fileupload.FileName);
                int fileSize = fileupload.ContentLength;
                int Size = fileSize / 1000;
                fileupload.SaveAs(Server.MapPath("~/Uploads/" + fileName));

                string CS = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand("spAddNewVideoFile", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    cmd.Parameters.AddWithValue("@Name", fileName);
                    cmd.Parameters.AddWithValue("@Title", fileName);
                    cmd.Parameters.AddWithValue("@Description", fileName);
                    cmd.Parameters.AddWithValue("@Key", Guid.NewGuid().ToString());
                    cmd.Parameters.AddWithValue("@FileSize", Size);
                    cmd.Parameters.AddWithValue("FilePath", "~/Uploads/" + fileName);
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("VideoList");
        }
    }
}