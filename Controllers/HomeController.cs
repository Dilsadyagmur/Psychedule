using Microsoft.AspNetCore.Mvc;
using PschologyProject.Models;
using Psy.DAL.Repositories.Abstract;
using System.Diagnostics;

namespace PschologyProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public HomeController(IUnitOfWork unitOfWork) 
        { 
            this.unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            HomeVm homeVm = new HomeVm();
            homeVm.psychologists = unitOfWork.Psychologists.GetAll().ToList();
            return View(homeVm);    
        }

        public IActionResult Privacy()
        {
            return View();
        }

    }
}
