using DevExpress.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using NorthOps.Ops.Models;
using NorthOps.Ops.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NorthOps.Ops.Controllers {
    //[RoutePrefix("recruit")]
    public class RecruitController : Controller {
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager {
            get {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set {
                _userManager = value;
            }
        }
        private UnitOfWork unitOfWork = new UnitOfWork();

        public RecruitController() { }
        public RecruitController(ApplicationUserManager userManager) {
            UserManager = userManager;
        }


        #region Applicants
        public ActionResult Index() {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult ApplicantGridViewPartial() {
            //var model = unitOfWork.UserRepository.Get(includeProperties: "UserRoles");
            return PartialView("_ApplicantGridViewPartial", unitOfWork.UserRepository.Get(includeProperties: "UserRoles").Where(x => x.UserRoles.FirstOrDefault().Name == "Applicant"));
            //return PartialView("_ApplicantGridViewPartial", unitOfWork.UserRepository.Get());
        }



        #endregion
        #region Applications
        [Route("recruit/job-applications"), HttpGet]
        public ActionResult JobApplications() {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult JobApplicationGridPartial() {
            var model = unitOfWork.JobApplicationRepo.Get(includeProperties: "User").Where(x => x.User.UserRoles.Any(u => u.Name == "Applicant"));

            return PartialView("_JobApplicationGridPartial", model);
        }


        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> JobApplicationGridPartialUpdate(NorthOps.Ops.Models.JobApplication item) {
            var model = new object[0];
            if (ModelState.IsValid) {
                try {
                    var job = await unitOfWork.JobApplicationRepo.GetByIDAsync(item.JobApplicationId);

                    if ((item.ContractDate != null && job.ContractDate == null) && job.ContractDate != item.ContractDate)
                        await UserManager.SendEmailAsync(job.UserId, "NorthOps Contract Signing Date", $"NorthOps Contract Signing Date { item.ContractDate}");
                    else if ((item.OnBoardingDate != null && job.OnBoardingDate == null) && item.OnBoardingDate != job.OnBoardingDate)
                        await UserManager.SendEmailAsync(job.UserId, "NorthOps On BoardingDate Date", $"NorthOps On Boarding Date { item.OnBoardingDate}");
                    else if ((item.TrainingDate != null && job.TrainingDate == null) && item.TrainingDate != job.TrainingDate)
                        await UserManager.SendEmailAsync(job.UserId, "NorthOps Training Date", $"NorthOps Training Date { item.TrainingDate}");
                    else if ((item.PersonalInterviewDate != null && job.PersonalInterviewDate == null) && job.PersonalInterviewDate != item.PersonalInterviewDate)
                        await UserManager.SendEmailAsync(job.UserId, "NorthOps Personal Interview Date", $"NorthOps Personal Interview Date { item.PersonalInterviewDate}");
                    else if ((item.PhoneInterviewDate != null && job.PhoneInterviewDate == null) && job.PhoneInterviewDate != item.PhoneInterviewDate)
                        await UserManager.SendEmailAsync(job.UserId, "NorthOps Phone Interview Date", $"NorthOps Phone Interview Date { item.PhoneInterviewDate}");

                    job.PhoneInterviewDate = item.PhoneInterviewDate;
                    job.PhoneInterview = item.PhoneInterview;
                    job.PersonalInterviewDate = item.PersonalInterviewDate;
                    job.PersonalInterview = item.PersonalInterview;
                    job.TrainingDate = item.TrainingDate;
                    job.Training = item.Training;
                    job.OnBoardingDate = item.OnBoardingDate;
                    job.OnBoarding = item.OnBoarding;
                    job.Contract = item.Contract;
                    job.ContractDate = item.ContractDate;

                    unitOfWork.JobApplicationRepo.Update(job);
                    await unitOfWork.SaveAsync();
                }
                catch (Exception e) {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_JobApplicationGridPartial", unitOfWork.JobApplicationRepo.Get(includeProperties: "User").Where(x => x.User.UserRoles.FirstOrDefault().Name == "Applicant"));
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult JobApplicationGridPartialDelete(System.Guid JobApplicationId) {
            var model = new object[0];
            if (JobApplicationId != null) {
                try {
                    // Insert here a code to delete the item from your model
                }
                catch (Exception e) {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("_JobApplicationGridPartial", model);
        }

        #endregion
        #region OpenExamPartial
        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> OpenExamPartial(string UserId, bool? Open) {
            if (Open == true) {
                var exams = unitOfWork.ExamRepo.Get();
                foreach (var i in exams) {
                    unitOfWork.Applicant.Insert(new Applicant() {
                        ApplicantId = Guid.NewGuid(),
                        ExamId = i.ExamId,
                        UserId = UserId

                    });
                    unitOfWork.Save();
                }
                await UserManager.SendEmailAsync(UserId, "Your exam is ready", "Your exam is ready");
            }
            return PartialView("_btnOpenExamPartial", unitOfWork.Applicant.Get(filter: m => m.UserId == UserId).FirstOrDefault());
        }
        #endregion

    }
}