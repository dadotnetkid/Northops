﻿using DevExpress.Web.Mvc;
using NorthOps.Ops.Models;
using NorthOps.Ops.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NorthOps.Ops.Controllers {
    public class CategoryController : Controller {
        private UnitOfWork unitOfWork = new UnitOfWork();
        public ActionResult Index() {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult CategoryViewPartial() {
            var model = new object[0];
            return PartialView("_CategoryViewPartial", unitOfWork.CategoryRepo.Get());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CategoryViewPartialAddNew(NorthOps.Ops.Models.Category item) {
            var model = new object[0];
            if (ModelState.IsValid) {
                try {
                    item.CategoryId = Guid.NewGuid();
                    unitOfWork.CategoryRepo.Insert(item);
                    unitOfWork.Save();
                }
                catch (Exception e) {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_CategoryViewPartial", unitOfWork.CategoryRepo.Get());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult CategoryViewPartialUpdate(NorthOps.Ops.Models.Category item) {
            var model = new object[0];
            if (ModelState.IsValid) {
                try {
                    unitOfWork.CategoryRepo.Update(item);
                    unitOfWork.Save();
                }
                catch (Exception e) {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_CategoryViewPartial", unitOfWork.CategoryRepo.Get());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult CategoryViewPartialDelete(System.Guid CategoryId) {
            var model = new object[0];
            if (CategoryId != null) {
                try {
                    Category addressTownCity = unitOfWork.CategoryRepo.GetByID(CategoryId);
                    unitOfWork.CategoryRepo.Delete(CategoryId);
                    unitOfWork.Save();
                }
                catch (Exception e) {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("_CategoryViewPartial", unitOfWork.CategoryRepo.Get());
        }
    }
}