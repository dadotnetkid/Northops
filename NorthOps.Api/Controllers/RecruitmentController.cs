using Microsoft.AspNet.Identity.Owin;
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
    [Authorize]
    public class RecruitmentController : ApiController
    {
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
        private UnitOfWork unitOfWork = new UnitOfWork();
        [HttpGet, Route("api/recruitment/applicants")]
        public IHttpActionResult Applicants()
        {
            return Ok(unitOfWork.UserRepository.Get(includeProperties: "UserRoles").Where(x => x.UserRoles.FirstOrDefault().Name == "Applicant"));
        }
        [HttpGet, Route("api/recruitment/job-application")]
        public IHttpActionResult JobApplication()
        {
            return Ok(unitOfWork.JobApplicationRepo.Get(includeProperties: "User,User.UserRoles").Where(x => x.User.UserRoles.Any(u => u.Name == "Applicant")));
        }

        [HttpPost, Route("api/recruitment/job-application")]
        public async Task<IHttpActionResult> JobApplication([FromBody]JobApplication item)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            try
            {
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
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }
        [HttpPost, Route("api/recruitment/open-exam")]
        public async Task<IHttpActionResult> OpenExam([FromBody]OpenExamModel model)
        {
            if (model.Open == false)
                return Ok(unitOfWork.Applicant.Get(filter: m => m.UserId == model.UserId).FirstOrDefault());

            var exams = unitOfWork.ExamRepo.Get();
            foreach (var i in exams)
            {
                unitOfWork.Applicant.Insert(new Applicant()
                {
                    ApplicantId = Guid.NewGuid(),
                    ExamId = i.ExamId,
                    UserId = model.UserId

                });
                unitOfWork.Save();
            }
            await UserManager.SendEmailAsync(model.UserId, "Your exam is ready", "Your exam is ready");

            return Ok(unitOfWork.Applicant.Get(filter: m => m.UserId == model.UserId,includeProperties: "Exam,User").FirstOrDefault());
        }

    }

    public class OpenExamModel
    {
        public string UserId { get; set; }
        public bool Open { get; set; }
    }
}
