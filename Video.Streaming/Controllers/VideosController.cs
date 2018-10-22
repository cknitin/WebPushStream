using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Web;

namespace Video.Streaming.Controllers
{
    [Authorize]
    public class VideosController : ApiController
    {

        //public HttpResponseMessage Get(string filename, string ext)
        //{

        //        var _filename = System.Web.HttpContext.Current.Server.MapPath("~/Uploads/" + filename + "." + ext);
        //        using (FileStream fileStream = File.OpenRead(_filename))
        //        {
        //            //create new MemoryStream object
        //            MemoryStream stream = new MemoryStream();
        //            stream.SetLength(fileStream.Length);
        //            //read file to MemoryStream
        //            fileStream.Read(stream.GetBuffer(), 0, (int)fileStream.Length);
        //            HttpResponseMessage partialResponse = Request.CreateResponse(HttpStatusCode.PartialContent);
        //            partialResponse.Content = new ByteRangeStreamContent(stream, Request.Headers.Range, "video/mp4");
        //            return partialResponse;
        //        }

        //}

        #region new Code

        public HttpResponseMessage Get(string filename, string ext)
        {
            var _filename = System.Web.HttpContext.Current.Server.MapPath("~/Uploads/" + filename + "." + ext);
            var response = Request.CreateResponse();
            response.Content = new PushStreamContent(async (Stream outputStream, HttpContent content, TransportContext context) =>
            {
                try
                {
                    var buffer = new byte[65536];
                    using (var video = File.Open(_filename, FileMode.Open, FileAccess.Read))
                    {
                        var length = (int)video.Length;
                        var bytesRead = 1;
                        while (length > 0 && bytesRead > 0)
                        {
                            bytesRead = video.Read(buffer, 0, Math.Min(length, buffer.Length));
                            await outputStream.WriteAsync(buffer, 0, bytesRead);
                            length -= bytesRead;
                        }
                    }
                }
                catch (HttpException ex)
                {
                    return;
                }
                finally
                {
                    outputStream.Close();
                }
            }, new MediaTypeHeaderValue("video/" + ext));
            return response;
        }

        #endregion
    }
}
