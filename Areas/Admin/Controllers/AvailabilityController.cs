using Microsoft.AspNetCore.Mvc;
using PschologyProject.Areas.Admin.Services;
using Psy.DAL.Repositories.Abstract;

namespace PschologyProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AvailabilityController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AvailabilityService _availabilityService;

        public AvailabilityController(IUnitOfWork unitOfWork) 
        { 
            _unitOfWork = unitOfWork;
            _availabilityService = new AvailabilityService(unitOfWork);
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new
            {
                data = _unitOfWork.Psychologists.GetAll(p => p.User.IsDeleted == false).Select(p => new
                {
                    p.User.Id,
                    p.User.UserName,
                    p.User.Email,
                    p.User.PhoneNumber,
                    //p.User.Password, SİLİNDİ
                }

            ).ToList()
            });
        }

        [HttpPost]
        public IActionResult GenerateAvailabilitiesForPsychologist(int psychologistId, DateTime startDate, DateTime endDate, DateTime workingHoursStart, DateTime workingHoursEnd)
        {
            
            _availabilityService.CreateAvailabilityForPsychologist(psychologistId,startDate,endDate, workingHoursStart, workingHoursEnd);
            return Ok("Availabilities created successfully.");
        }


    }
}
