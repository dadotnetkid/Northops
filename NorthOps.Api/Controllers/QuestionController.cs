using NorthOps.Api.Models;
using NorthOps.Api.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace NorthOps.Api.Controllers
{
    public class QuestionController : ApiController
    {
        private UnitOfWork unitOfWork = new UnitOfWork();

        #region Questions
        [HttpGet, Authorize, Route("api/question/{QuestionId}")]
        public IHttpActionResult Get(Guid? QuestionId)
        {
            return Ok(unitOfWork.QuestionRepo.Fetch(includeProperties: "Exam", filter: m => m.QuestionId == QuestionId).Select(x => new
            {
                ApplicantAnswers = x.ApplicantAnswers,
                Exam = x.Exam,
                Choices = x.Choices,
                QuestionId = x.QuestionId,
                ExamId = x.ExamId,
                VideoId = x.VideoId,
                Title = x.Title,
                Number = x.Number,
                Question = x.Question1,
                DateCreated = x.DateCreated
            }));
        }
        [HttpGet, Authorize, Route("api/question/first-result/{QuestionId?}/{ExamId?}")]
        public IHttpActionResult FirstResult(System.Nullable<Guid> QuestionId, System.Nullable<Guid> ExamId)
        {
            return Ok(unitOfWork.QuestionRepo.Get(filter: m => m.QuestionId == QuestionId && m.ExamId == ExamId).FirstOrDefault() ?? new Question() { ExamId = ExamId.Value });
        }
        [HttpPost, Authorize, Route("api/question")]
        public IHttpActionResult Post(Question item)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            try
            {
                unitOfWork.QuestionRepo.Insert(item);
                unitOfWork.Save();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }
        [HttpGet, Authorize, Route("api/question/delete/{QuestionId?}")]
        public async Task<IHttpActionResult> Delete(System.Guid? QuestionId)
        {
            unitOfWork.QuestionRepo.Delete(await unitOfWork.QuestionRepo.GetByIDAsync(QuestionId));
            await unitOfWork.SaveAsync();
            return Ok();
        }
        //TODO: Insert end point
        [HttpPost, Authorize, Route("api/question/insert")]
        public async Task<IHttpActionResult> Insert(Question item)
        {
            item.QuestionId = Guid.NewGuid();
            unitOfWork.QuestionRepo.Insert(item);
            await unitOfWork.SaveAsync();
            return Ok();
        }
        //TODO: Update End Point
        [HttpPost, Authorize, Route("api/question/update")]
        public async Task<IHttpActionResult> Update(Question item)
        {
            unitOfWork.QuestionRepo.Update(item);
            await unitOfWork.SaveAsync();
            return Ok();
        }
        #endregion


    }
}
