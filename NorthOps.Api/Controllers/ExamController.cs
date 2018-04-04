using NorthOps.Api.Models;
using NorthOps.Api.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NorthOps.Api.Controllers
{
    [Authorize]
    [RoutePrefix("api/exam")]
    public class ExamController : ApiController
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        [HttpGet]
        public IHttpActionResult Get()
        {
            return Ok(unitOfWork.ExamRepo.Get(filter: m => m.ExamType != (int)ExamTypes.TypingSkills, includeProperties: "Category"));
        }
        [HttpPost, Route("update")]
        public IHttpActionResult Update([FromBody]Exam item)
        {
            unitOfWork.ExamRepo.Update(item);
            unitOfWork.Save();
            return Ok();
        }
        [HttpGet, Route("delete/{ExamId}")]
        public IHttpActionResult Delete(System.Guid ExamId)
        {
            var exam = unitOfWork.ExamRepo.GetByID(ExamId);
            unitOfWork.ExamRepo.Delete(exam);
            unitOfWork.Save();
            return Ok();
        }
        [HttpPost, Route("insert")]
        public IHttpActionResult Insert([FromBody]Exam item)
        {
            item.DateCreated = DateTime.Now;
            item.ExamId = Guid.NewGuid();
            unitOfWork.ExamRepo.Insert(item);
            unitOfWork.Save();
            return Ok();
        }
   
    }
}
