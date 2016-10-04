using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TravelInCloud.Models.HomeViewModels;
using Microsoft.AspNetCore.Authorization;
using TravelInCloud.Models;
using TravelInCloud.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

namespace TravelInCloud.Controllers
{
    [RequireHttps]
    [Authorize]
    public class HomeController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;

        public HomeController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailSender emailSender,
        ISmsSender smsSender,
        ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _logger = loggerFactory.CreateLogger<ManageController>();
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Cart()
        {
            return View();
        }
        public IActionResult Order()
        {
            return View();
        }
        public async Task<IActionResult> Mine()
        {
            var user = await GetCurrentUserAsync();
            var Model = new MineViewModel
            {
                NickName = user.NickName,
                IconAddress = user.IconAddress
            };
            return View(Model);
        }

        public IActionResult Error()
        {
            return View();
        }


        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
    }
}
