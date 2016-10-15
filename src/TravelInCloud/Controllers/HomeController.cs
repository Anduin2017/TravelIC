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
using TravelInCloud.Data;
using Microsoft.EntityFrameworkCore;

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
        private readonly ApplicationDbContext _dbContext;

        public HomeController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailSender emailSender,
        ISmsSender smsSender,
        ILoggerFactory loggerFactory,
        ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _logger = loggerFactory.CreateLogger<ManageController>();
            _dbContext = dbContext;
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
        public async Task<IActionResult> Parent()
        {
            var user = await GetCurrentUserAsync();
            var Model = new ParentViewModel
            {
                NickName = user.NickName,
                IconAddress = user.IconAddress,
                OurAccount = !user.Email.Contains(Secrets.TempUserName)
            };
            return View(Model);
        }

        [AllowAnonymous]
        public ActionResult ProductList(StoreType StoreType)
        {
            var _products = _dbContext.Products.Include(t => t.Owner).ToList();
            var Model = new ProductListViewModel
            {
                Products = _products.Where(t=>t.Owner.StoreType==StoreType).ToList()
            };
            ViewData["Title"] = StoreType;
            return View(Model);
        }

        [AllowAnonymous]
        public async Task<ActionResult> Product(int id)
        {
            var _product = await _dbContext.Products.Include(t => t.Owner).SingleOrDefaultAsync(t => t.ProductId == id);
            return View(new ProductViewModel
            {
                Product = _product
            });
        }

        public async Task<IActionResult> Mine()
        {
            var user = await GetCurrentUserAsync();
            var Model = new MineViewModel
            {
                NickName = user.NickName,
                IconAddress = user.IconAddress,
                OurAccount = !user.Email.Contains(Secrets.TempUserName)
            };
            return View(Model);
        }

        public IActionResult CreateAccount()
        {
            return View(new CreateAccountViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount(CreateAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await GetCurrentUserAsync();
            user.Email = model.Email;
            user.UserName = model.Email;
            await _userManager.UpdateAsync(user);
            await _userManager.ChangePasswordAsync(user, Secrets.TempPassword, model.Password);
            return RedirectToAction(nameof(Mine));
        }

        public IActionResult Error()
        {
            return View();
        }

        public async Task<IActionResult> Settings()
        {
            var User = await GetCurrentUserAsync();
            var Model = new SettingsViewModel
            {
                IsStore = User.Discriminator == nameof(Store),
                IsOurAccount = !User.Email.Contains(Secrets.TempUserName)
            };
            return View(Model);
        }

        public IActionResult ApplyStore()
        {
            return View(new ApplyStoreViewModel { });
        }

        [HttpPost]
        public async Task<IActionResult> ApplyStore(ApplyStoreViewModel Model)
        {
            if (ModelState.IsValid)
            {
                var User = await GetCurrentUserAsync();
                var NewStoreUser = new Store(User);
                NewStoreUser.StoreDescription = Model.StoreDescription;
                NewStoreUser.StoreLocation = Model.StoreLocation;
                NewStoreUser.StoreName = Model.StoreName;
                NewStoreUser.StoreOwnerCode = Model.StoreOwnerCode;
                NewStoreUser.StoreOwnerName = Model.StoreOwnerName;
                NewStoreUser.PhoneNumber = Model.StoreOwnerPhone;
                NewStoreUser.StoreType = Model.StoreType;
                await _userManager.DeleteAsync(User);
                await _userManager.CreateAsync(NewStoreUser);
                await _signInManager.SignInAsync(NewStoreUser, false);
                return RedirectToAction(nameof(Settings));
            }
            return View(Model);
        }
        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
    }
}
