using Psy.DAL.Repositories.Abstract;
using Psy.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PschologyProject.Areas.Admin.Services
{
    public class AvailabilityService
    {
        private readonly IUnitOfWork _avaDb;

        public AvailabilityService(IUnitOfWork unitOfWork) 
        {
            _avaDb = unitOfWork;
        }
        
        public void CreateAvailabilityForPsychologist(int psychologistId, DateTime startDate, DateTime endDate, DateTime workingHoursStart, DateTime workingHoursEnd )
        {
            var daysOfWeek = new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };

            for (var date = startDate.Date; date <= endDate.Date;date = date.AddDays(1))
            {
                if (daysOfWeek.Contains(date.DayOfWeek))
                {
                    for (int hour = workingHoursStart.Hour; hour < workingHoursEnd.Hour; hour++)
                    {
                        var availability = new Availability
                        {
                            PsychologistId = psychologistId,
                            Date = date,
                            StartTime = new TimeSpan(hour, 0, 0),
                            IsAvailable = true,
                        };
                        _avaDb.Availabilities.Add(availability);
                    }
                }
               
            }

            _avaDb.Save();
        }
    }
}
