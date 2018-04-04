using DevExpress.Web.Mvc;
using NorthOps.Ops.ApiRepository;
using NorthOps.Ops.Models;
using NorthOps.Ops.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NorthOps.Ops.Controllers
{
    public class QuestionController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        private ApiGenericRepository apiRepo = new ApiGenericRepository();
        #region Question Grid
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult QuestionGridPartial()
        {
            var model = new ApiGenericRepository().GetFetch<IEnumerable<Question>>("api/question"); //unitOfWork.QuestionRepo.Get(includeProperties: "Exam");
            return PartialView("_QuestionGridPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult QuestionGridPartialAddNew(NorthOps.Ops.Models.Question item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    new ApiGenericRepository().Insert("api/question", item);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_QuestionGridPartial", new ApiGenericRepository().GetFetch<IEnumerable<Question>>("api/question"));
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult QuestionGridPartialDelete(System.Guid? QuestionId)
        {
            var model = new object[0];
            if (QuestionId != null)
            {
                try
                {
                    new ApiGenericRepository().Delete($"api/question/delete/{QuestionId}");
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("_QuestionGridPartial", new ApiGenericRepository().GetFetch<IEnumerable<Question>>("api/question"));
        }
        #endregion


        [ValidateInput(false)]
        public ActionResult ChoicesGridPartial(Guid QuestionId, System.Guid? ChoiceId)
        {
            var model = apiRepo.GetFetch<IEnumerable<Choice>>($"api/choice/{QuestionId}/{ChoiceId}");
            ViewBag.QuestionId = QuestionId;
            return PartialView("_ChoicesGridPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ChoicesGridPartialAddNew(NorthOps.Ops.Models.Choice item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var res = apiRepo.Insert("api/Choice", item);
                    if(res!=System.Net.HttpStatusCode.OK)
                        ViewData["EditError"] = res.ToString();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            ViewBag.QuestionId = item.QuestionId;
            return PartialView("_ChoicesGridPartial", apiRepo.GetFetch<IEnumerable<Choice>>($"api/choice/{item.QuestionId}"));
        }
       
        [HttpPost, ValidateInput(false)]
        public ActionResult ChoicesGridPartialDelete(System.Guid ChoiceId, System.Guid QuestionId)
        {
            var model = new object[0];
            ViewBag.QuestionId = QuestionId;
            if (ChoiceId != null)
            {
                try
                {

                    apiRepo.Delete($"api/Choice/delete/{ChoiceId}");
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("_ChoicesGridPartial", apiRepo.GetFetch<IEnumerable<Choice>>($"api/choice/{QuestionId}"));
        }
    }
}