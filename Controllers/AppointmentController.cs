using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Psy.DAL.Repositories.Abstract;
using Psy.Entities;

namespace PschologyProject.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly IUnitOfWork _unitofwork;
        private readonly UserManager<AppUser> _userManager;

      

        public AppointmentController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager) 
        { 
            _unitofwork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetAll()
        {
            var psychologists = _unitofwork.Psychologists.GetAll(p => !p.User.IsDeleted).Select(p => new
            {
                Id = p.User.Id,
                Username = p.User.UserName,
                Email = p.User.Email,
                PhoneNumber = p.User.PhoneNumber
            }).ToList();
        

            return Json(new
            {
                data = psychologists
            });
        }
        public IActionResult GetAvailableTimeSlots(int psychologistId, DateTime selectedDate)
        {
            var startTimes = _unitofwork.Availabilities.GetAll(a => a.PsychologistId == psychologistId
                                                           && a.Date == selectedDate
                                                           && a.IsAvailable).Select(a=>a.StartTime.ToString(@"hh\:mm")).ToList();
                           
                           
            return Json(startTimes);
        }
        [HttpPost]
        public IActionResult BookAppointment (int psychologistId, DateTime selectedDate, string selectedTime)
        {
            var starttimespan = TimeSpan.Parse(selectedTime);
          
           
            var availability = _unitofwork.Availabilities.GetAll()
     .FirstOrDefault(a => a.PsychologistId == psychologistId
                           && a.Date.Date == selectedDate.Date
                           && starttimespan == a.StartTime
                           
                           && a.IsAvailable);
            if (selectedDate < DateTime.Today )
            {
                return BadRequest("Seçtiğiniz tarihte randevu bulunmamaktadır.");
            }
            

            if (availability == null)
            {
                return BadRequest("Seçilen zaman dilimi uygun değil veya dolmuş.");
            }

            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userId = int.Parse(_userManager.GetUserId(User));
                    var appointment = new Appointments
                    {
                        UserId = userId,
                        PsychologistsId = psychologistId,
                        AppointmentDateTime = selectedDate,
                        AvailabilityId = availability.Id,
                        AppointmentStartTime = starttimespan,
                        Status = Appointments.AppointmentStatus.Pending,
                    };

                    _unitofwork.Appointments.Add(appointment);
                    availability.IsAvailable = false;
                    _unitofwork.Save();
                    return Ok("Randevu Oluşturuldu");
                }
            }
            catch (Exception)
            {

               return  BadRequest("Hata oluştu.");
            }

            return View();
        }
        
        public IActionResult MyAppointments()
        {
            return View();
        }
        [HttpGet]
        public IActionResult GetAllMyAppointments()
        {
            var userId = int.Parse(_userManager.GetUserId(User));


            var appointments = _unitofwork.Appointments.GetAll(a => a.UserId == userId && a.IsDeleted == false).Select(a => new
            {
                a.AppointmentDateTime,
                a.AppointmentStartTime,
                a.Psychologists.User.UserName,
                Status = a.Status.HasValue ? a.Status.Value.ToString() : "Bilinmiyor"
            }).ToList();

            return Json(new
            {
                data = appointments
            });
        }
    }
}
