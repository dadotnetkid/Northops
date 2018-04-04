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
    [Authorize]
    public class ChoiceController : ApiController
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        // GET: api/Choice
        [Route("api/choice/{QuestionId?}/{ChoiceId?}")]
        public IHttpActionResult Get(Guid? QuestionId, System.Guid? ChoiceId)
        {
            var model = unitOfWork.ChoiceRepo.Get(filter: m => m.QuestionId == QuestionId, includeProperties: "Question");
            if (ChoiceId != null)
            {
                model = unitOfWork.ChoiceRepo.Get(filter: m => m.ChoiceId == ChoiceId, includeProperties: "Question");
            }
            return Ok(model);
        }
        [Route("api/choice/Choices/{QuestionId?}")]
        public IHttpActionResult Choices(Guid? QuestionId)
        {
            var model = unitOfWork.ChoiceRepo.Get(filter: m => m.QuestionId == QuestionId, includeProperties: "Question");
            return Ok(model);
        }
        // POST: api/Choice
        [HttpPost,Route("insert")]
        public IHttpActionResult Insert([FromBody]Choice item)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                item.ChoiceId = Guid.NewGuid();
                item.DateCreated = DateTime.Now;
                unitOfWork.ChoiceRepo.Insert(item);
                unitOfWork.Save();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        // DELETE: api/Choice/5
        [HttpGet, Route("api/Choice/delete/{ChoiceId}")]
        public async Task< IHttpActionResult> Delete(System.Guid ChoiceId)
        {
            unitOfWork.ChoiceRepo.Delete(await unitOfWork.ChoiceRepo.GetByIDAsync(ChoiceId));
            await unitOfWork.SaveAsync();
            return Ok();
        }

        [HttpPost, Route("api/Choice/update")]
        public async Task< IHttpActionResult> Update([FromBody]Choice item)
        {
            unitOfWork.ChoiceRepo.Update(item);
            await unitOfWork.SaveAsync();
            return Ok();
        }
    }
}
