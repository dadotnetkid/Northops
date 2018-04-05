using DevExpress.Web.Mvc;
using NorthOps.Ops.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NorthOps.Ops.Models;
using System.Threading.Tasks;
using NorthOps.Ops.ApiRepository;

namespace NorthOps.Ops.Controllers
{
    public class ExamController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        private ApiGenericRepository apiRepo = new ApiGenericRepository();
        public ActionResult Index()
        {
            return View();
        }



        public ActionResult ExportUploadUpload(Guid ExamId, string Name)
        {
            ExportUploadSettings.ExamId = ExamId;
            var res = UploadControlExtension.GetUploadedFiles(Name, ExportUploadSettings.UploadValidationSettings, ExportUploadSettings.FileUploadComplete);
            return null;
        }



        public ActionResult ExamDescriptionEditorrPartial()
        {
            return PartialView("_ExamDescriptionEditorrPartial");
        }
        public ActionResult ExamDescriptionEditorrPartialImageSelectorUpload()
        {
            HtmlEditorExtension.SaveUploadedImage("Description", ExamControllerExamDescriptionEditorSettings.ImageSelectorSettings);
            return null;
        }
        public ActionResult ExamDescriptionEditorrPartialImageUpload()
        {
            HtmlEditorExtension.SaveUploadedFile("Description", ExamControllerExamDescriptionEditorSettings.ImageUploadValidationSettings, ExamControllerExamDescriptionEditorSettings.ImageUploadDirectory);
            return null;
        }











        #region Categories
        public ActionResult Categories()
        {
            return View();
        }
        public ActionResult CategoryViewPartial()
        {
            var model = apiRepo.GetFetch<Choice>("api/category");
            //unitOfWork.CategoryRepo.Get()
            return PartialView("_CategoryViewPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CategoryViewPartialAddNew(NorthOps.Ops.Models.Category item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    apiRepo.Insert("api/category/insert", item);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = apiRepo.GetFetch<Choice>("api/category");
            // unitOfWork.CategoryRepo.Get()
            return PartialView("_CategoryViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult CategoryViewPartialUpdate(NorthOps.Ops.Models.Category item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    apiRepo.Update("api/category", item);

                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            var model = apiRepo.GetFetch<Choice>("api/category");
            // unitOfWork.CategoryRepo.Get()
            return PartialView("_CategoryViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult CategoryViewPartialDelete(System.Guid CategoryId)
        {
            if (CategoryId != null)
            {
                try
                {
                    apiRepo.Delete($"api/category/delete/{CategoryId}");
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            var model = apiRepo.GetFetch<Choice>("api/category");
            // unitOfWork.CategoryRepo.Get()
            return PartialView("_CategoryViewPartial", model);
        }

        #endregion
        #region Examinations

        public ActionResult ExamAddeditPartial(System.Nullable<Guid> ExamId)
        {
            return PartialView("_examaddeditpartial", unitOfWork.ExamRepo.GetByID(ExamId) ?? new Exam());
        }

        public ActionResult Examinations()
        {
            return View();
        }
        [ValidateInput(false)]
        public ActionResult ExamGridPartial()
        {
            //unitOfWork.ExamRepo.Get(filter:m=>m.ExamType!=(int)ExamTypes.TypingSkills,includeProperties: "Category")
            return PartialView("_ExamGridPartial", apiRepo.GetFetch<IEnumerable<Exam>>("api/exam"));
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ExamGridPartialAddNew(NorthOps.Ops.Models.Exam item)
        {
            item.DateCreated = DateTime.Now;
            item.ExamId = Guid.NewGuid();
            if (ModelState.IsValid)
            {
                try
                {
                    apiRepo.Insert("api/exam/insert", item);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_ExamGridPartial", apiRepo.GetFetch<IEnumerable<Exam>>("api/exam"));
        }
        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> ExamGridPartialUpdate(NorthOps.Ops.Models.Exam item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await apiRepo.UpdateAsync("api/exam/update", item);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_ExamGridPartial", apiRepo.GetFetch<IEnumerable<Exam>>("api/exam"));
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ExamGridPartialDelete(System.Guid ExamId)
        {
            var model = new object[0];
            if (ExamId != null)
            {
                try
                {
                    apiRepo.Delete($"api/exam/delete{ExamId}");
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("_ExamGridPartial", unitOfWork.ExamRepo.Get());
        }
        #endregion

        #region Choices

        [ValidateInput(false)]
        public ActionResult ChoiceGridViewPartial(System.Nullable<Guid> QuestionId, System.Nullable<Guid> ExamId)
        {
            ViewBag.QuestionId = QuestionId;
            ViewBag.ExamId = ExamId;
            var model = apiRepo.GetFetch<Choice>($"api/choice/Choices/{QuestionId}");
            return PartialView("_ChoiceGridViewPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> ChoiceGridViewPartialAddNew(NorthOps.Ops.Models.Choice item, System.Nullable<Guid> ExamId)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    /*  item.ChoiceId = Guid.NewGuid();
                      unitOfWork.ChoiceRepo.Insert(item);
                      await unitOfWork.SaveAsync();*/
                    await apiRepo.InsertAsync("api/choice/insert", item);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            ViewBag.QuestionId = item.QuestionId;
            ViewBag.ExamId = ExamId;
            var model = apiRepo.GetFetch<Choice>($"api/choice/Choices/{item.QuestionId}");//unitOfWork.ChoiceRepo.Get(filter: m => m.QuestionId == item.QuestionId);
            return PartialView("_ChoiceGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> ChoiceGridViewPartialUpdate(NorthOps.Ops.Models.Choice item, System.Nullable<Guid> ExamId)
        {
            var model = new object[0];
            if (ModelState.IsValid)
            {
                try
                {
                    await apiRepo.UpdateAsync("api/Choice/update", item);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            ViewBag.QuestionId = item.QuestionId;
            ViewBag.ExamId = ExamId;
            return PartialView("_ChoiceGridViewPartial", unitOfWork.ChoiceRepo.Get(filter: m => m.QuestionId == item.QuestionId));
        }
        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> ChoiceGridViewPartialDelete(System.Nullable<Guid> ChoiceId, System.Nullable<Guid> QuestionId, System.Nullable<Guid> ExamId)
        {
            var model = new object[0];
            if (ChoiceId != null)
            {
                try
                {
                    await apiRepo.DeleteAsync($"api/Choice/delete/{ChoiceId}");
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            ViewBag.QuestionId = QuestionId;
            ViewBag.ExamId = ExamId;
            return PartialView("_ChoiceGridViewPartial", unitOfWork.ChoiceRepo.Get(filter: m => m.QuestionId == QuestionId));
        }

        #endregion

        #region Question
        public ActionResult QuestionAddEditPartial(System.Nullable<Guid> QuestionId, System.Nullable<Guid> ExamId)
        {
            ViewBag.ExamId = ExamId.ToString();
            var model = apiRepo.GetFetch<Question>($"api/question/first-result/{QuestionId}/{ExamId}");
            //unitOfWork.QuestionRepo.Get(filter: m => m.QuestionId == QuestionId && m.ExamId == ExamId).FirstOrDefault() ?? new Question() { ExamId = ExamId.Value }
            return PartialView("_questionAddEditPartial", model);
        }
        [ValidateInput(false)]
        public ActionResult QuestionGridPartial(Guid examId)
        {
            ViewBag.ExamId = examId.ToString();
            var model = apiRepo.GetFetch<IEnumerable<Question>>("$api/question/{ExamId}");
            // unitOfWork.QuestionRepo.Get().Where(x => x.ExamId == examId)
            return PartialView("_QuestionGridPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> QuestionGridPartialAddNew(NorthOps.Ops.Models.Question item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await apiRepo.InsertAsync("api/question/insert", item);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            ViewBag.ExamId = item.ExamId;
            var model = apiRepo.GetFetch<IEnumerable<Question>>("$api/question/{ExamId}");
            //unitOfWork.QuestionRepo.Get(filter: m => m.ExamId == item.ExamId)
            return PartialView("_QuestionGridPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> QuestionGridPartialUpdate(NorthOps.Ops.Models.Question item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    await apiRepo.UpdateAsync("api/question/update", item);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            ViewBag.ExamId = item.ExamId;
            var model = apiRepo.GetFetch<IEnumerable<Question>>("$api/question/{ExamId}");
            //unitOfWork.QuestionRepo.Get(filter: m => m.ExamId == item.ExamId))
            return PartialView("_QuestionGridPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> QuestionGridPartialDelete(System.Guid QuestionId, System.Guid ExamId)
        {
            if (QuestionId != null)
            {
                try
                {
                    await apiRepo.DeleteAsync($"api/question/delete/{QuestionId}");
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            ViewBag.ExamId = ExamId;
            var model = apiRepo.GetFetch<IEnumerable<Question>>("$api/question/{ExamId}");
            // unitOfWork.QuestionRepo.Get(filter: m => m.ExamId == ExamId))
            return PartialView("_QuestionGridPartial", model);
        }
        #endregion







        public ActionResult ExamDescriptionEditorPartial()
        {
            return PartialView("_ExamDescriptionEditorPartial");
        }
    }


    public class ExamControllerExamDescriptionEditorSettings
    {
        public const string ImageUploadDirectory = "~/Content/UploadImages/";
        public const string ImageSelectorThumbnailDirectory = "~/Content/Thumb/";

        public static DevExpress.Web.UploadControlValidationSettings ImageUploadValidationSettings = new DevExpress.Web.UploadControlValidationSettings()
        {
            AllowedFileExtensions = new string[] { ".jpg", ".jpeg", ".jpe", ".gif", ".png" },
            MaxFileSize = 4000000
        };

        static DevExpress.Web.Mvc.MVCxHtmlEditorImageSelectorSettings imageSelectorSettings;
        public static DevExpress.Web.Mvc.MVCxHtmlEditorImageSelectorSettings ImageSelectorSettings
        {
            get
            {
                if (imageSelectorSettings == null)
                {
                    imageSelectorSettings = new DevExpress.Web.Mvc.MVCxHtmlEditorImageSelectorSettings(null);
                    imageSelectorSettings.Enabled = true;
                    imageSelectorSettings.UploadCallbackRouteValues = new { Controller = "Exam", Action = "ExamDescriptionEditorrPartialImageSelectorUpload" };
                    imageSelectorSettings.CommonSettings.RootFolder = ImageUploadDirectory;
                    imageSelectorSettings.CommonSettings.ThumbnailFolder = ImageSelectorThumbnailDirectory;
                    imageSelectorSettings.CommonSettings.AllowedFileExtensions = new string[] { ".jpg", ".jpeg", ".jpe", ".gif" };
                    imageSelectorSettings.UploadSettings.Enabled = true;
                }
                return imageSelectorSettings;
            }
        }
    }





}