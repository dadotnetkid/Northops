using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;

namespace NorthOps.Ops.Controllers
{
    [Authorize]
    public class UserController : ApiController
    {

        public IHttpActionResult GetUser()
        {
            return Ok(new JObject()
            {
                ["userId"]=User.Identity.GetUserId(),
                ["Username"]=User.Identity.GetUserName()
            });
        }
    }
}
