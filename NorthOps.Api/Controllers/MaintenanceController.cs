using DevExpress.Data.Linq.Helpers;
using DevExpress.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json.Linq;
using NorthOps.Api.Models;
using NorthOps.Api.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace NorthOps.Api.Controllers
{
    [Authorize, RoutePrefix("api/maintenance")]
    public class MaintenanceController : ApiController
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
        #region User
        [HttpGet, Route("users")]
        public IHttpActionResult Users()
        {
            return Ok(unitOfWork.UserRepository.Get(includeProperties: "UserRoles"));
        }
        [HttpGet, Route("users-count/{FilterExpression?}")]
        public IHttpActionResult UsersCount(string FilterExpression = "")
        {
            var count = unitOfWork.UserRepository.Fetch().Count();
            if (FilterExpression != "")
                count = unitOfWork.UserRepository.Fetch().Select(x => new { FullName = x.FirstName + " " + x.LastName, Email = x.Email }).ApplyFilter(FilterExpression).Count();
            return Ok(count);
        }
        [HttpPost, Route("users-data/{Skip}/{Take}/{filterExpression?}")]
        public IHttpActionResult UsersData([FromBody] IEnumerable<GridViewColumnState> columnState, int Skip, int Take, string filterExpression = "")
        {
            var res = unitOfWork.UserRepository.Fetch().Select(x => new { Id = x.Id, FirstName = x.FirstName, MiddleName = x.MiddleName, LastName = x.LastName, FullName = x.FirstName + " " + x.LastName, Email = x.Email });
            if (columnState.Count() > 0 && columnState != null)
                return Ok(res.ApplySorting(columnState)
                     .ApplyFilter(filterExpression)
                     .Skip(Skip).Take(Take));
            else
                return Ok(res.OrderBy(m => m.Id).ApplyFilter(filterExpression)
                   .Skip(Skip).Take(Take));


            return Ok(res);
        }

        [HttpGet, Route("first-user/{Id}")]
        public IHttpActionResult FirstUser(string Id)
        {
            var user = unitOfWork.UserRepository.Find(m => m.Id == Id);
            user.userRole = user.UserRoles.FirstOrDefault() == null ? "" : user.UserRoles.FirstOrDefault().Name;
            return Ok(user);
        }
        [HttpPost, Route("insert-user")]
        public async Task<IHttpActionResult> InsertUser(User item)
        {
            item.Id = Guid.NewGuid().ToString();
            var res = await UserManager.CreateAsync(item, item.Password);
            if (res.Succeeded)
            {
                await UserManager.AddToRoleAsync(item.Id, item.userRole);
                unitOfWork.JobApplicationRepo.Insert(new JobApplication()
                {
                    JobApplicationId = Guid.NewGuid(),
                    UserId = item.Id
                });
                await unitOfWork.SaveAsync();
            }
            return Ok();
        }
        [HttpPost, Route("update-user")]
        public async Task<IHttpActionResult> UpdateUser(User item)
        {
            var user = await UserManager.FindByIdAsync(item.Id);
            user.FirstName = item.FirstName;
            user.LastName = item.LastName;
            user.MiddleName = item.MiddleName;
            user.Gender = item.Gender;
            user.BirthDate = item.BirthDate;
            user.AddressLine1 = item.AddressLine1;
            user.AddressLine2 = item.AddressLine2;
            user.TownCity = item.TownCity;
            user.Cellular = item.Cellular;
            user.Religion = item.Religion;
            user.Citizenship = item.Citizenship;
            user.Languages = item.Languages;
            user.CivilStatus = item.CivilStatus;
            user.Skills = item.Skills;
            await UserManager.UpdateAsync(user);

            #region UpdateRole
            var roles = await UserManager.GetRolesAsync(user.Id);
            await UserManager.RemoveFromRolesAsync(user.Id, roles.ToArray());
            await UserManager.AddToRoleAsync(user.Id, item.userRole);
            #endregion

            if (item.Password != null)
            {
                var token = await UserManager.GeneratePasswordResetTokenAsync(item.Id);
                var res = await UserManager.ResetPasswordAsync(item.Id, token, item.Password);
            }
            return Ok();
        }
        [HttpGet, Route("delete-user/{Id}")]
        public async Task<IHttpActionResult> DeleteUser(string Id)
        {
            await UserManager.DeleteAsync(await UserManager.FindByIdAsync(Id));
            return Ok();
        }

        [HttpGet, Route("role-list")]
        public IHttpActionResult RoleList()
        {
            return Ok(new UnitOfWork().RoleRepository.Get());
        }
        #endregion
        #region Town
        [HttpGet, Route("state-province")]
        public IHttpActionResult StateProvince()
        {
            return Ok(unitOfWork.StateProvinceRepo.Get().Select(x => new { Id = x.StateProvinceId, Name = x.Name }).OrderBy(x => x.Name));
        }
        [HttpGet, Route("town-city")]
        public IHttpActionResult TownCity()
        {
            return Ok(unitOfWork.TownCityRepo.Get(includeProperties: "AddressStateProvince"));
        }
        [HttpGet, Route("delete-town-city/{TownCityId}")]
        public IHttpActionResult DeleteTownCity(System.Int32 TownCityId)
        {
            AddressTownCity addressTownCity = unitOfWork.TownCityRepo.GetByID(TownCityId);
            unitOfWork.TownCityRepo.Delete(TownCityId);
            unitOfWork.Save();
            return Ok();
        }
        [HttpPost, Route("update-town-city")]
        public IHttpActionResult UpdateTownCity(AddressTownCity item)
        {
            unitOfWork.TownCityRepo.Update(item);
            unitOfWork.Save();
            return Ok();
        }
        [HttpPost, Route("get-location-range")]
        public IHttpActionResult GetLocationRange([FromBody]FilterLocationModel model)
        {
            var ret = unitOfWork.TownCityRepo.Fetch(filter: m => m.Name.Contains(model.AddressName)).OrderBy(m => m.TownCityId).Skip(model.Skip).Take(model.Take).ToList();

            return Ok(ret);
        }
        [HttpGet, Route("get-location-by-id/{TownCityId}")]
        public IHttpActionResult GetLocationById(int TownCityId)
        {
            var ret = unitOfWork.TownCityRepo.Fetch(filter: m => m.TownCityId == TownCityId).OrderBy(m => m.TownCityId).Take(1).ToList();

            return Ok(ret);
        }
        #endregion
    }
    public class FilterLocationModel
    {
        public FilterLocationModel()
        {
        }

        public int Skip { get; set; }
        public int Take { get; set; }
        public string AddressName { get; set; }
    }
}
