using Microsoft.AspNet.Identity;
using NorthOps.Api.Models;
using NorthOps.Api.Repository;
using System.Collections.Generic;
using System.Web.Http;
using Microsoft.AspNet.Identity.Owin;
using System.Web;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace NorthOps.Api.Controllers
{
    public class MemberController : ApiController
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        #region Profile
        [HttpGet, Authorize, Route("api/member/profile/{Id}")]
        public IHttpActionResult Profile(string Id)
        {
            return Ok(unitOfWork.UserRepository.GetByID(Id ?? User.Identity.GetUserId()));

        }
        [HttpPost, Authorize, Route("api/member/update")]
        public async Task<IHttpActionResult> Update(User model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = await UserManager.FindByIdAsync(model.Id);
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.MiddleName = model.MiddleName;
            user.Gender = model.Gender;
            user.BirthDate = model.BirthDate;
            user.AddressLine1 = model.AddressLine1;
            user.AddressLine2 = model.AddressLine2;
            user.TownCity = model.TownCity;
            user.Cellular = model.Cellular;
            user.Religion = model.Religion;
            user.Citizenship = model.Citizenship;
            user.Languages = model.Languages;
            user.CivilStatus = model.CivilStatus;
            user.Skills = model.Skills;
            var result = await UserManager.UpdateAsync(user);
            #region UpdateRole
            var roles = await UserManager.GetRolesAsync(user.Id);
            await UserManager.RemoveFromRolesAsync(user.Id, roles.ToArray());
            await UserManager.AddToRoleAsync(user.Id, model.userRole);
            #endregion
            return Ok(result);

        }
        [HttpPost, Authorize, Route("api/member/create")]
        public async Task<IHttpActionResult> Create(User model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                model.Id = Guid.NewGuid().ToString();
                var res = await UserManager.CreateAsync( model, model.Password);
                if (!res.Succeeded)
                    return BadRequest(string.Join(Environment.NewLine, res.Errors));
                await UserManager.AddToRoleAsync(model.Id, model.userRole);
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }
        [HttpGet, Authorize, Route("api/member/Delete/{Id}")]
        public async Task<IHttpActionResult> Delete(string Id)
        {
            var user = UserManager.FindById(Id);
            if (user == null)
                return BadRequest();
            var result = await UserManager.DeleteAsync(user);
            if(!result.Succeeded)
                return BadRequest();
            return Ok();
        }
        [HttpGet, Authorize, Route("api/member/user-login-status")]
        public IHttpActionResult UserLoginStatus()
        {
           return  Ok((from u in UserManager.Users.ToList()
             where u.Id == User.Identity.GetUserId()
             select new LoginStatusModel
             {
                 Name = u.FullName ?? User.Identity.GetUserName(),
                 Position = UserManager.GetRoles(User.Identity.GetUserId()).FirstOrDefault(),
                 HireDate = u.HireDate,
                 Photo = u.Photo
             }).FirstOrDefault());
        }
        #endregion
    }
}
