using NorthOps.Api.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NorthOps.Api.Controllers
{
    public class TownCityController : ApiController
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        public IHttpActionResult Get()
        {
            return Ok(unitOfWork.TownCityRepo.Get(includeProperties: "AddressStateProvince"));
        }

    }
}
