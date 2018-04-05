using DevExpress.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using NorthOps.Ops.ApiRepository;
using NorthOps.Ops.Models;
using NorthOps.Ops.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NorthOps.Ops.Controllers
{
    //[RoutePrefix("recruit")]
    public class RecruitController : Controller
    {
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        private UnitOfWork unitOfWork = new UnitOfWork();

        public RecruitController() { }
        public RecruitController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }


        #region Applicants
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult ApplicantGridViewPartial()
        {

            return PartialView("_ApplicantGridViewPartial", unitOfWork.UserRepository.Get(includeProperties: "UserRoles").Where(x => x.UserRoles.FirstOrDefault().Name == "Applicant"));
        }



        #endregion
        #region Applications
        [Route("recruit/job-applications"), HttpGet]
        public ActionResult JobApplications()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult JobApplicationGridPartial()
        {
            //var model = unitOfWork.JobApplicationRepo.Get(includeProperties: "User").Where(x => x.User.UserRoles.Any(u => u.Name == "Applicant"));
            IEnumerable<JobApplication> model = new List<JobApplication>();
            model = new ApiGenericRepository().GetFetch<IEnumerable<JobApplication>>("api/recruitment/job-application");
            return PartialView("_JobApplicationGridPartial", model);
        }


        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> JobApplicationGridPartialUpdate(NorthOps.Ops.Models.JobApplication item)
        {
            var model = new object[0];
            if (ModelState.IsValid)
            {
                try
                {
                    await new ApiGenericRepository().UpdateAsync("api/recruitment/job-application", item);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_JobApplicationGridPartial", new ApiGenericRepository().GetFetch<IEnumerable<JobApplication>>("api/recruitment/job-application"));
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult JobApplicationGridPartialDelete(System.Guid JobApplicationId)
        {
            var model = new object[0];
            if (JobApplicationId != null)
            {
                try
                {
                    // Insert here a code to delete the item from your model
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("_JobApplicationGridPartial", model);
        }

        #endregion
        #region OpenExamPartial
        [HttpPost, ValidateInput(false)]
        public ActionResult OpenExamPartial(string UserId, bool? Open)
        {
            var model = new OpenExamModel()
            {
                UserId = UserId,
                Open = Open
            };
            return PartialView("_btnOpenExamPartial", new ApiGenericRepository().PostFetch<Applicant>("api/recruitment/open-exam", model));
        }
        #endregion

    }

    internal class OpenExamModel
    {
        public OpenExamModel()
        {
        }

        public string UserId { get; set; }
        public bool? Open { get; set; }
    }
}