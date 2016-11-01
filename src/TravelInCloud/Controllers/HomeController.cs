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
            var _products = _dbContext.Products
                .Include(t => t.Owner)
                .Include(t => t.ImageOfProducts)
                .Include(t => t.ProductTypes)
                .ToList();

            var Model = new IndexViewModel
            {
                Products = _products.OrderByDescending(t => t.BuyTimes).Take(10).ToList()
            };
            return View(Model);
        }

        public async Task<IActionResult> Buy()
        {
            var _user = await GetCurrentUserAsync();
            if (!_user.FullInfo())
            {
                return RedirectToAction(nameof(ChangeMyInfo), new { ReturnUrl = Request.Path });
            }
            else
            {
                return View();
            }
        }

        public IActionResult Cart()
        {
            return View();
        }

        public async Task<IActionResult> ChangeMyInfo(string ReturnUrl = "")
        {
            var _user = await GetCurrentUserAsync();
            var _model = new ChangeMyInfoViewModel
            {
                Name = _user.Name,
                Phone = _user.PhoneNumber,
                IDCode = _user.IDCode,
                ReturnUrl = ReturnUrl
            };
            return View(_model);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeMyInfo(ChangeMyInfoViewModel model, string ReturnUrl = "")
        {
            var _user = await GetCurrentUserAsync();
            _user.Name = model.Name;
            _user.PhoneNumber = model.Phone;
            _user.IDCode = model.IDCode;
            await _userManager.UpdateAsync(_user);

            if (string.IsNullOrEmpty(ReturnUrl) || !_user.FullInfo())
            {
                return RedirectToAction(nameof(Mine));
            }
            else
            {
                return LocalRedirect(ReturnUrl);
            }
        }


        public async Task<IActionResult> Order()
        {
            var user = await GetCurrentUserAsync();
            var Orders = _dbContext
                    .Orders
                    .Include(t => t.ProductType)
                    .Include(t => t.Owner)
                    .Where(t => t.OwnerId == user.Id)
                    .ToList();
            Orders.ForEach(async t =>
            {
                t.ProductType.BelongingProduct = await _dbContext
                    .Products
                    .SingleOrDefaultAsync(p => p.ProductId == t.ProductType.BelongingProductId);
            });

            var Model = new OrderViewModel
            {
                NickName = user.NickName,
                IconAddress = user.IconAddress,
                OurAccount = !user.Email.Contains(Secrets.TempUserName),
                Orders = Orders
            };
            return View(Model);
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
        public ActionResult ProductList(StoreType StoreType, int queryMethod = 3)
        {
            var _products = _dbContext.Products
                .Include(t => t.Owner)
                .Include(t => t.ImageOfProducts)
                .Include(t => t.ProductTypes)
                .ToList()
                .Where(t => t.Owner.StoreType == StoreType)
                .ToList();

            var Model = new ProductListViewModel
            {
                StoreType = (int)StoreType
            };

            switch (queryMethod)
            {
                case 1:
                    Model.Products = _products
                        .Where(t => t.ProductTypes.Count > 0)//所有具有价值的商品
                        .OrderBy(t => t.ProductTypes.Min(p => p.Price))//价格最低值
                        .ToList();
                    Model.QueryMethod = "按价格排序";
                    break;
                case 2:
                    Model.Products = _products.OrderByDescending(t => t.BuyTimes).ToList();
                    Model.QueryMethod = "按人气排序";
                    break;
                case 3:
                    Model.Products = _products;
                    Model.QueryMethod = "默认排序";
                    break;
                case 4:
                    Model.Products = _products.OrderByDescending(t => t.ViewTimes).ToList();
                    Model.QueryMethod = "按浏览量排序";
                    break;
                default:
                    throw new Exception();
            }
            ViewData["Title"] = StoreType;
            return View(Model);
        }

        [AllowAnonymous]
        public IActionResult SetMethod(int id)
        {
            var _model = new SetMethodViewModel { id = id };
            return View(_model);
        }

        [AllowAnonymous]
        public async Task<ActionResult> Product(int id)
        {
            var _product = await _dbContext
                .Products
                .Include(t => t.ImageOfProducts)
                .Include(t => t.ProductTypes)
                .Include(t => t.Owner)
                .SingleOrDefaultAsync(t => t.ProductId == id);

            _product.ViewTimes++;
            await _dbContext.SaveChangesAsync();

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

        public async Task<IActionResult> ApplyStore()
        {
            var _user = await GetCurrentUserAsync() as Store;
            if (_user != null)
            {
                return View(new ApplyStoreViewModel
                {
                    StoreDescription = _user.StoreDescription,
                    StoreName = _user.StoreName,
                    StoreLocation = _user.StoreLocation,
                    StoreOwnerCode = _user.StoreOwnerCode,
                    StoreOwnerName = _user.StoreOwnerName,
                    StoreOwnerPhone = _user.PhoneNumber,
                    StoreType = _user.StoreType
                });
            }
            else
            {
                return View(new ApplyStoreViewModel { });
            }
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

        public IActionResult Pay()
        {
            return View();
        }
        public async Task<IActionResult> PayTest()
        {
            string wxJsApiParam;
            var cuser = await GetCurrentUserAsync();
            var jsApiPay = new JsApiPay(cuser.openid, 1, HttpContext.Connection.RemoteIpAddress.ToString());
            var unifiedOrderResult = await jsApiPay.GetUnifiedOrderResult();

            wxJsApiParam = jsApiPay.GetJsApiParameters();
            var Result = unifiedOrderResult.ToPrintStr();
            return View(new PayViewModel { unifiedOrderResult = Result, wxJsApiParam = wxJsApiParam });
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
    }
}
