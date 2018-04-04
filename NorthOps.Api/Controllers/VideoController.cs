using NorthOps.Api.Models;
using NorthOps.Api.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace NorthOps.Api.Controllers
{
    [Authorize]
    public class VideoController : ApiController
    {
        UnitOfWork unitOfWork = new UnitOfWork();
        public IHttpActionResult Get()
        {
            return Ok(unitOfWork.VideoRepo.Get(includeProperties: "Exam"));
        }
        public async Task<IHttpActionResult> Post([FromBody]Video item)
        {
            if (ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var files = HttpContext.Current.Request.Files[0];
                var ms = new MemoryStream();
                files.InputStream.CopyTo(ms);

                item.VideoId = Guid.NewGuid();
                item.Video1 = ms.ToArray();
                item.Extension = Path.GetExtension(files.FileName);
                unitOfWork.VideoRepo.Insert(item);
                await unitOfWork.SaveAsync();
            }
            catch (Exception e)
            {
                return BadRequest(ModelState);
            }
            return Ok();
        }
        [HttpGet, Route("api/video/delete/{VideoId}")]
        public async Task<IHttpActionResult> Delete(Guid VideoId)
        {
            unitOfWork.VideoRepo.Delete(await unitOfWork.VideoRepo.GetByIDAsync(VideoId));
            await unitOfWork.SaveAsync();
            return Ok();
        }
    }
}
