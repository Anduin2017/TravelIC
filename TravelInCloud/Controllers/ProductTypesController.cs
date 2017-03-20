using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TravelInCloud.Data;
using TravelInCloud.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using TravelInCloud.Services;
using TravelInCloud.Models.ProductTypesViewModels;

namespace TravelInCloud.Controllers
{
    [Authorize]
    public class ProductTypesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _context;

        public ProductTypesController(
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
            _context = dbContext;
        }

        // GET: ProductTypes
        public async Task<IActionResult> Index(int ProductId)
        {
            var user = await GetCurrentUserAsync();

            var _productTypes = _context.ProductTypes
                .Where(t => t.BelongingProductId == ProductId)
                .Include(p => p.BelongingProduct)
                .ToList()
                .Where(t => t.BelongingProduct.OwnerId == user.Id);
            var _model = new IndexViewModel
            {
                ProductTypes = _productTypes.ToList(),
                ProductId = ProductId
            };

            return View(_model);
        }

        // GET: ProductTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productType = await _context.ProductTypes.SingleOrDefaultAsync(m => m.ProductTypeId == id);
            if (productType == null)
            {
                return NotFound();
            }

            return View(productType);
        }

        // GET: ProductTypes/Create
        public IActionResult Create(int ProductId)
        {
            var _model = new ProductType
            {
                BelongingProductId = ProductId
            };
            return View(_model);
        }

        // POST: ProductTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductType _model)
        {
            var _user = await GetCurrentUserAsync();
            var Product = await _context.Products.SingleOrDefaultAsync(t => t.ProductId == _model.BelongingProductId);

            if (ModelState.IsValid && Product.OwnerId == _user.Id)
            {
                var newType = _model as ProductType;
                _context.Add(newType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { ProductId = _model.BelongingProductId });
            }
            return View(_model);
        }

        // GET: ProductTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productType = await _context.ProductTypes.SingleOrDefaultAsync(m => m.ProductTypeId == id);
            if (productType == null)
            {
                return NotFound();
            }

            return View(productType);
        }

        // POST: ProductTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var _user = await GetCurrentUserAsync();
            var productType = await _context
                .ProductTypes
                .Include(t=>t.BelongingProduct)
                .SingleOrDefaultAsync(m => m.ProductTypeId == id);

            if (productType.BelongingProduct.OwnerId == _user.Id)
            {
                int ProductId = productType.BelongingProductId;
                _context.ProductTypes.Remove(productType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { ProductId = ProductId });
            }
            return View(nameof(Delete));
        }

        private bool ProductTypeExists(int id)
        {
            return _context.ProductTypes.Any(e => e.ProductTypeId == id);
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
    }
}
