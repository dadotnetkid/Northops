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
    //api/category
    [RoutePrefix("api/category")]
    public class CategoryController : ApiController
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        [HttpGet]
        public IHttpActionResult Get()
        {
            return Ok(unitOfWork.CategoryRepo.Get());
        }
        [HttpPost,Route("insert")]
        public IHttpActionResult Insert([FromBody]Category item)
        {
            item.CategoryId = Guid.NewGuid();
            unitOfWork.CategoryRepo.Insert(item);
            unitOfWork.Save();
            return Ok();
        }
        [HttpPost, Route("update")]
        public IHttpActionResult Update([FromBody]Category item)
        {
            unitOfWork.CategoryRepo.Update(item);
            unitOfWork.Save();
            return Ok();
        }
        [HttpGet, Route("delete/{CategoryId}")]
        public IHttpActionResult Delete(System.Guid CategoryId)
        {
            Category addressTownCity = unitOfWork.CategoryRepo.GetByID(CategoryId);
            unitOfWork.CategoryRepo.Delete(CategoryId);
            unitOfWork.Save();
            return Ok();
        }
    }
}
