using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Psy.DAL.Repositories.Abstract;
using Psy.Entities;
using System.Numerics;

namespace PschologyProject.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _userdb;

        public UserController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager) 
        {
            _userManager = userManager;
            _userdb = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> MyProfile()
        {
            var user = _userManager.GetUserAsync(User);


            return View(user);
        }


    }
}
