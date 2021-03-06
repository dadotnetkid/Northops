﻿using DevExpress.Web.Mvc;
using Microsoft.AspNet.Identity;
using NorthOps.Ops.Models;
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
            ViewBag.States = unitOfWork.StateProvinceRepo.Get().Select(x => new { Id = x.StateProvinceId, Name = x.Name }).OrderBy(x => x.Name);
            var model = new object[0];
            return PartialView("_TownCityGridPartial", unitOfWork.TownCityRepo.Get(includeProperties: "AddressStateProvince"));
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TownCityGridPartialAddNew(NorthOps.Ops.Models.AddressTownCity item)
        {
            var model = new object[0];
            if (ModelState.IsValid)
            {
                try
                {
                    // Insert here a code to insert the new item in your model
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_TownCityGridPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult TownCityGridPartialUpdate(NorthOps.Ops.Models.AddressTownCity item)
        {
            var model = new object[0];
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
            return PartialView("_TownCityGridPartial", unitOfWork.TownCityRepo.Get(includeProperties: "AddressStateProvince"));
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult TownCityGridPartialDelete(System.Int32 TownCityId)
        {
            var model = new object[0];
            if (TownCityId >= 0)
            {
                try
                {
                    AddressTownCity addressTownCity = unitOfWork.TownCityRepo.GetByID(TownCityId);
                    unitOfWork.TownCityRepo.Delete(TownCityId);
                    unitOfWork.Save();
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

        [ValidateInput(false)]
        public ActionResult UserGridViewPartial()
        {

            return PartialView("_UserGridViewPartial", unitOfWork.UserRepository.Get(includeProperties: "UserRoles"));
        }

        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> UserGridViewPartialAddNew(NorthOps.Ops.Models.User item)
        {
            var model = new object[0];
            if (ModelState.IsValid)
            {
                try
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
                    else
                    {
                        ViewData["model"] = item;
                        ViewData["EditError"] = string.Join(Environment.NewLine, res.Errors);
                    }
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

            return PartialView("_UserGridViewPartial", unitOfWork.UserRepository.Get(includeProperties: "UserRoles"));
        }
        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> UserGridViewPartialUpdate(NorthOps.Ops.Models.User item)
        {
            if (ModelState.IsValid)
            {
                try
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
#if DEBUG
                        if (!res.Succeeded)
                        {
                            Debug.WriteLine(string.Join(",", res.Errors));
                        }
#endif
                    }


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

            return PartialView("_UserGridViewPartial", unitOfWork.UserRepository.Get(includeProperties: "UserRoles"));
        }
        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> UserGridViewPartialDelete(NorthOps.Ops.Models.User item)
        {
            if (item.Id != null)
            {
                try
                {

                    await UserManager.DeleteAsync(await UserManager.FindByIdAsync(item.Id));
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("_UserGridViewPartial", unitOfWork.UserRepository.Get(includeProperties: "UserRoles"));
        }
        [ChildActionOnly]
        public ActionResult UserAddEditPartial(NorthOps.Ops.Models.User item)
        {
            var user = UserManager.FindById(item.Id);
            if (user != null)
            {
                user.userRole = user.UserRoles.FirstOrDefault() == null ? "" : user.UserRoles.FirstOrDefault().Name;

            }
            ViewBag.TownCity = new UnitOfWork().TownCityRepo.Get();
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