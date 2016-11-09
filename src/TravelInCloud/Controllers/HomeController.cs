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
        public async Task<IActionResult> Index(int LocationId = -1)
        {
            IEnumerable<Product> _products = _dbContext.Products
                .Include(t => t.Owner)
                .Include(t => t.ImageOfProducts)
                .Include(t => t.ProductTypes);

            if (LocationId != -1)
            {
                _products = _products.Where(t => t.Owner.LocationId == LocationId);
            }

            var Model = new IndexViewModel
            {
                Products = _products.OrderByDescending(t => t.ViewTimes).Take(5),
                Locations = _dbContext.Locations,
                CurrentLocation = await _dbContext.Locations.SingleOrDefaultAsync(t => t.LocationId == LocationId)
            };
            return View(Model);
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

        public async Task<IActionResult> Order(int OrderStatus = -1, int ProductType = -1)
        {
            var user = await GetCurrentUserAsync();
            IEnumerable<Order> Orders = _dbContext
                    .Orders
                    .Include(t => t.ProductType)
                    .Include(t => t.ProductType.BelongingProduct)
                    .Include(t => t.ProductType.BelongingProduct.ImageOfProducts)
                    .Include(t => t.Owner)
                    .Where(t => t.OwnerId == user.Id);

            if (OrderStatus != -1)
            {
                Orders = Orders.Where(t => t.OrderStatus == (OrderStatus)OrderStatus);
            }
            if (ProductType != -1)
            {
                Orders = Orders.Where(t => t.ProductType.BelongingProduct.Owner.StoreType == (StoreType)ProductType);
            }

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
                .Include(t => t.Comments)
                .Where(t => t.Owner.StoreType == StoreType)
                .Take(20)
                .ToList();

            var Model = new ProductListViewModel
            {
                StoreType = (int)StoreType
            };

            switch (queryMethod)
            {
                case 1:
                    Model.Products = _products
                        .Where(t => t.ProductTypes.Count > 0)
                        .OrderBy(t => t.ProductTypes.Min(p => p.Price));
                    Model.QueryMethod = "按价格排序";
                    break;
                case 2:
                    Model.Products = _products.OrderByDescending(t => t.BuyTimes);
                    Model.QueryMethod = "按人气排序";
                    break;
                case 3:
                    Model.Products = _products.OrderByDescending(t => t.ProductId);
                    Model.QueryMethod = "默认排序";
                    break;
                case 4:
                    Model.Products = _products.OrderByDescending(t => t.ViewTimes);
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
                .Include(t => t.Comments)
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
            return View(new ApplyStoreViewModel
            {
                AvaliableLocations = _dbContext.Locations
            });
        }

        [HttpPost]
        public async Task<IActionResult> ApplyStore(ApplyStoreViewModel Model)
        {
            if (ModelState.IsValid)
            {
                var User = await GetCurrentUserAsync();
                var TempOrders = User.Orders.ToList();
                _dbContext.Orders.RemoveRange(User.Orders);
                await _dbContext.SaveChangesAsync();

                var NewStoreUser = new Store(User);
                NewStoreUser.StoreDescription = Model.StoreDescription;
                NewStoreUser.StoreLocation = Model.StoreLocation;
                NewStoreUser.StoreName = Model.StoreName;
                NewStoreUser.StoreOwnerCode = Model.StoreOwnerCode;
                NewStoreUser.StoreOwnerName = Model.StoreOwnerName;
                NewStoreUser.PhoneNumber = Model.StoreOwnerPhone;
                NewStoreUser.StoreType = Model.StoreType;
                NewStoreUser.LocationId = Model.LocationId;

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
            var cuser = await GetCurrentUserAsync();
            var TargetOrder = await _dbContext
                .Orders
                .Include(t => t.ProductType)
                .Where(t => t.OwnerId == cuser.Id)//这个人的订单中
                .OrderByDescending(t => t.CreateTime)//按创建时间排序
                .FirstAsync();//的第一个，也就是最新的那个

            string wxJsApiParam;
            var jsApiPay = new JsApiPay(cuser.openid, TargetOrder.ProductType.Price, HttpContext.Connection.RemoteIpAddress.ToString());
            var unifiedOrderResult = await jsApiPay.GetUnifiedOrderResult();

            wxJsApiParam = jsApiPay.GetJsApiParameters();
            var Result = unifiedOrderResult.ToPrintStr();
            return View(new PayViewModel { unifiedOrderResult = Result, wxJsApiParam = wxJsApiParam });
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await _dbContext.Users
                .Include(t => t.Orders)
                .SingleOrDefaultAsync(t => t.UserName == User.Identity.Name);
        }

        public async Task<IActionResult> PreOrder(int id)
        {
            var TargetProductType = await _dbContext.ProductTypes.SingleOrDefaultAsync(t => t.ProductTypeId == id);

            var user = await GetCurrentUserAsync();
            if (!user.FullInfo())
            {
                return RedirectToAction(nameof(ChangeMyInfo), new { ReturnUrl = Request.Path });
            }
            var _Model = new PreOrderViewModel
            {
                TargetProductId = id,
                TargetProduct = TargetProductType
            };
            return View(_Model);
        }

        [HttpPost]
        public async Task<IActionResult> PreOrder(PreOrderViewModel Model)
        {
            if (ModelState.IsValid)
            {
                var cuser = await GetCurrentUserAsync();
                _dbContext.Orders.Add(new Order
                {
                    OwnerId = cuser.Id,
                    CreateTime = DateTime.Now,
                    OrderStatus = OrderStatus.unPaid,
                    ProductTypeId = Model.TargetProductId,
                    UseTime = Model.UseDate
                });
                await _dbContext.SaveChangesAsync();

                return RedirectToAction(nameof(PayTest));
            }
            return RedirectToAction(nameof(PreOrder), new { id = Model.TargetProductId });
        }

        public async Task<IActionResult> ChangeStoreInfo()
        {
            var user = await GetCurrentUserAsync() as Store;
            if (user != null)
            {
                var _Model = new ChangeStoreInfoViewModel
                {
                    StoreDescription = user.StoreDescription,
                    StoreLocation = user.StoreLocation,
                    StoreName = user.StoreName,
                    StoreOwnerCode = user.StoreOwnerCode,
                    StoreOwnerName = user.StoreOwnerName,
                    StoreOwnerPhone = user.PhoneNumber
                };
                return View(_Model);
            }
            return RedirectToAction(nameof(Settings));
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStoreInfo(ChangeStoreInfoViewModel Model)
        {
            var user = await GetCurrentUserAsync() as Store;
            if (ModelState.IsValid && user != null)
            {
                user.StoreDescription = Model.StoreDescription;
                user.StoreName = Model.StoreName;
                user.StoreLocation = Model.StoreLocation;
                user.StoreOwnerCode = Model.StoreOwnerCode;
                user.StoreOwnerName = Model.StoreOwnerName;
                user.PhoneNumber = Model.StoreOwnerPhone;
                await _userManager.UpdateAsync(user);
                return RedirectToAction(nameof(Settings));
            }
            return View(Model);
        }

        public async Task<IActionResult> PayFinished()
        {
            var cuser = await GetCurrentUserAsync();
            var TargetOrder = await _dbContext
                .Orders
                .Include(t => t.ProductType)
                .Include(t => t.ProductType.BelongingProduct)
                .Where(t => t.OwnerId == cuser.Id)//这个人的订单中
                .OrderByDescending(t => t.CreateTime)//按创建时间排序
                .FirstAsync();//的第一个，也就是最新的那个

            TargetOrder.OrderStatus = OrderStatus.Paid;
            TargetOrder.ProductType.BelongingProduct.BuyTimes++;
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Order));
        }

        public async Task<IActionResult> ProductWarning(int id)
        {
            var Product = await _dbContext.Products.SingleOrDefaultAsync(t => t.ProductId == id);
            var _model = new ProductWarningViewModel
            {
                Product = Product
            };
            return View(_model);
        }

        public async Task<IActionResult> ManageOrders(int OrderStatus = -1)
        {
            var User = await GetCurrentUserAsync() as Store;
            if (User != null)
            {
                var _model = new ManageOrdersViewModel
                {
                    Orders = _dbContext
                        .Orders
                        .Include(t => t.Owner)
                        .Include(t => t.ProductType)
                        .Include(t => t.ProductType.BelongingProduct)
                        .Include(t => t.ProductType.BelongingProduct.ImageOfProducts)
                        .Where(t => t.ProductType.BelongingProduct.OwnerId == User.Id)
                        .OrderBy(t => t.CreateTime)
                };

                if (OrderStatus != -1)
                {
                    _model.Orders = _model.Orders.Where(t => t.OrderStatus == (OrderStatus)OrderStatus);
                }

                return View(_model);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> SetUsed(int OrderId, OrderStatus Target)
        {
            var TargetOrder = await _dbContext
                .Orders
                .Include(t => t.ProductType)
                .Include(t => t.ProductType.BelongingProduct)
                .Include(t => t.ProductType.BelongingProduct.Owner)
                .SingleOrDefaultAsync(t => t.OrderId == OrderId);

            var User = await GetCurrentUserAsync() as Store;
            //Confirm this order is his order...
            if (TargetOrder.ProductType.BelongingProduct.OwnerId == User.Id)
            {
                TargetOrder.OrderStatus = Target;
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(ManageOrders));
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> PublishComment(int ProductId)
        {
            var TargetProduct = await _dbContext.Products.SingleOrDefaultAsync(t => t.ProductId == ProductId);
            if (TargetProduct != null)
            {
                return View(new PublishCommentViewModel { });
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> PublishComment(PublishCommentViewModel model)
        {
            var user = await GetCurrentUserAsync();

            var TargetProduct = await _dbContext
                .Products
                .Include(t => t.Comments)
                .SingleOrDefaultAsync(t => t.ProductId == model.ProductId);

            _dbContext.Comments.Add(new Comment
            {
                ProductId = model.ProductId,
                UserId = user.Id,
                Content = model.Content,
                PublishTime = DateTime.Now
            });
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(AllComments), new { ProductId = model.ProductId });
        }

        [AllowAnonymous]
        public async Task<IActionResult> AllComments(int ProductId)
        {
            var TargetProduct = await _dbContext
                .Products
                .Include(t => t.Comments)
                .SingleOrDefaultAsync(t => t.ProductId == ProductId);

            var _model = new AllCommentsViewModel
            {
                ProductId = ProductId,
                Comments = TargetProduct.Comments
            };

            return View(_model);
        }
    }
}