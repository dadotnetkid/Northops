using DevExpress.Web.Mvc;
using Microsoft.AspNet.Identity;
using NorthOps.Ops.ApiRepository;
using NorthOps.Ops.Models;
using NorthOps.Ops.Models.ViewModels;
using NorthOps.Ops.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NorthOps.Ops.Controllers
{
    public class MaintenanceController : IdentityController
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        private ApiGenericRepository apiRepo = new ApiGenericRepository();
        private UserViewModel userViewModel = new UserViewModel();
        public ActionResult Index()
        {
            return View();
        }

        #region TownCity
        [Route("maintenance/town-cities")]
        public ActionResult TownCity()
        {
            return View();
        }
        [ValidateInput(false)]
        public ActionResult TownCityGridPartial()
        {
            ViewBag.States = apiRepo.GetFetch<IEnumerable<AddressStateProvince>>("api/maintenance/state-province");
            var model = apiRepo.GetFetch<IEnumerable<AddressStateProvince>>("api/maintenance/town-city");
            //unitOfWork.TownCityRepo.Get(includeProperties: "AddressStateProvince")
            return PartialView("_TownCityGridPartial", model);
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult TownCityGridPartialUpdate(NorthOps.Ops.Models.AddressTownCity item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    unitOfWork.TownCityRepo.Update(item);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = apiRepo.GetFetch<IEnumerable<AddressStateProvince>>("api/maintenance/town-city");
            //unitOfWork.TownCityRepo.Get(includeProperties: "AddressStateProvince")
            return PartialView("_TownCityGridPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult TownCityGridPartialDelete(System.Int32 TownCityId)
        {
            var model = new object[0];
            if (TownCityId >= 0)
            {
                try
                {
                    apiRepo.Delete($"api/maintenance/delete-town-city/{TownCityId}");
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("_TownCityGridPartial", model);
        }
        #endregion
        #region User
        public ActionResult Users()
        {
            return View();
        }

        #region Test
        [ValidateInput(false)]
        public ActionResult UserGridViewPartial()
        {

            return PartialView("_UserGridViewPartial", userViewModel.UserGridViewModel());
        }
        public ActionResult UserFilteringGridViewPartial(GridViewPagerState pager = null, GridViewFilteringState filteringState = null, GridViewColumnState column = null, bool reset = true)
        {
            return PartialView("_UserGridViewPartial", userViewModel.UserGridViewModel(column, reset, pager, filteringState));
        }



        #endregion


        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> UserGridViewPartialAddNew(NorthOps.Ops.Models.User item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await apiRepo.InsertAsync("api/maintenance/insert-user", item);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var m in ModelState)
                {
                    if (m.Value.Errors.Count() > 0)
                    {
                        ModelState.AddModelError(m.Key, string.Join(Environment.NewLine, m.Value.Errors.Select(x => x.ErrorMessage)));
                        stringBuilder.AppendLine(m.Value.Errors[0].ErrorMessage);
                    }
                }
                ViewData["EditError"] = "Please, correct all errors." + Environment.NewLine + stringBuilder.ToString();
                ViewData["Model"] = item;
            }
            var model = apiRepo.GetFetch<IEnumerable<User>>("api/maintenance/users");
            return PartialView("_UserGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> UserGridViewPartialUpdate(NorthOps.Ops.Models.User item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await apiRepo.UpdateAsync("api/maintenance/update-user", item);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var m in ModelState)
                {
                    if (m.Value.Errors.Count() > 0)
                    {
                        ModelState.AddModelError(m.Key, string.Join(Environment.NewLine, m.Value.Errors.Select(x => x.ErrorMessage)));
                        stringBuilder.AppendLine(m.Value.Errors[0].ErrorMessage);
                    }
                }
                ViewData["EditError"] = "Please, correct all errors." + Environment.NewLine + stringBuilder.ToString();
                ViewData["Model"] = item;
            }

            var model = apiRepo.GetFetch<IEnumerable<User>>("api/maintenance/users");

            return PartialView("_UserGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> UserGridViewPartialDelete(NorthOps.Ops.Models.User item)
        {
            if (item.Id != null)
            {
                try
                {
                    await apiRepo.DeleteAsync($"delete-user/{item.Id}");
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            var model = apiRepo.GetFetch<IEnumerable<User>>("api/maintenance/users");
            return PartialView("_UserGridViewPartial", model);
        }
        [ChildActionOnly]
        public ActionResult UserAddEditPartial(NorthOps.Ops.Models.User item)
        {
            var user = apiRepo.GetFetch<User>($"api/maintenance/first-user/{item.Id}");
            ViewBag.TownCity = apiRepo.GetFetch<IEnumerable<AddressTownCity>>("api/maintenance/town-city"); //new UnitOfWork().TownCityRepo.Get();
            return PartialView("_useraddeditpartial", user);
        }
        #endregion
        #region User Role
        [Route("maintenance/user-roles")]
        public ActionResult UserRoles()
        {
            return View();
        }
        [ValidateInput(false)]
        public ActionResult UserRolesGridViewPartial()
        {

            return PartialView("_UserRolesGridViewPartial", unitOfWork.RoleRepository.Get());
        }

        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> UserRolesGridViewPartialAddNew(NorthOps.Ops.Models.UserRole item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    item.Id = Guid.NewGuid().ToString();
                    unitOfWork.RoleRepository.Insert(item);
                    await unitOfWork.SaveAsync();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                foreach (var i in ModelState)
                {
                    if (i.Value.Errors.Count() > 0)
                    {
                        ModelState.AddModelError(i.Key, string.Join(Environment.NewLine, i.Value.Errors));
                    }
                }
                ViewData["EditError"] = "Please, correct all errors.";
                ViewData["model"] = item;
            }

            return PartialView("_UserRolesGridViewPartial", await unitOfWork.RoleRepository.GetAsync());
        }
        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> UserRolesGridViewPartialUpdate(NorthOps.Ops.Models.UserRole item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    unitOfWork.RoleRepository.Update(item);
                    await unitOfWork.SaveAsync();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                foreach (var i in ModelState)
                {
                    if (i.Value.Errors.Count() > 0)
                    {
                        ModelState.AddModelError(i.Key, string.Join(Environment.NewLine, i.Value.Errors));
                    }
                }
                ViewData["EditError"] = "Please, correct all errors.";
                ViewData["model"] = item;
            }
            return PartialView("_UserRolesGridViewPartial", await unitOfWork.RoleRepository.GetAsync());
        }

        [ChildActionOnly]
        public ActionResult UserRoleAddEditPartial(NorthOps.Ops.Models.UserRole item)
        {
            return PartialView("_UserRoleAddEditPartial", unitOfWork.RoleRepository.Get(filter: m => m.Id == item.Id).FirstOrDefault());
        }
        #endregion


        public ActionResult SelectTownCityPartial(int? TownCityId)
        {
            if (TownCityId == null)
                return PartialView("_SelectTowncityPartial", new AddressTownCity());
            else
                return PartialView("_SelectTowncityPartial", new AddressTownCity() { TownCityId = TownCityId.Value });
        }
    }
}