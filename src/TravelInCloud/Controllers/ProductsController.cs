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
using Microsoft.AspNetCore.Identity;
using TravelInCloud.Services;
using Microsoft.Extensions.Logging;
using System.IO;

namespace TravelInCloud.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _context;

        public ProductsController(
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

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var _user = await GetCurrentUserAsync() as Store;
            var applicationDbContext = _context.Products.Where(t => t.OwnerId == _user.Id).Include(p => p.Owner);
            return View(await applicationDbContext.ToListAsync());
        }


        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductDetails,ProductInfo,ProductName,ProductWarnning")] Product product)
        {
            var _user = await GetCurrentUserAsync() as Store;
            if (ModelState.IsValid)
            {
                product.OwnerId = _user.Id;
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CreateImage), new { id = product.ProductId });
            }
            return View(product);
        }


        // GET: Products/CreateImage
        public async Task<IActionResult> CreateImage(int id)
        {
            var _product = await _context.Products.Include(t=>t.ImageOfProducts).SingleOrDefaultAsync(t=>t.ProductId==id);
            return View(_product);
        }

        // POST: Products/CreateImage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateImage(int ProductId, int param = 0)
        {
            var file = Request.Form.Files["file"];
            string fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (file != null && file.Length < 4096000 && fileExtension.IsImage())
            {

                if (Directory.Exists(Directory.GetCurrentDirectory() + (@"\wwwroot\images\TempUpload")) == false)
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + (@"\wwwroot\images\TempUpload"));

                string filepath = Directory.GetCurrentDirectory() + (@"\wwwroot\images\TempUpload");
                string fileName = StringOperation.RandomString(10) + fileExtension;
                string virpath = filepath + @"\" + fileName;
                var fileStream = new FileStream(path: virpath, mode: FileMode.Create);
                await file.CopyToAsync(fileStream);
                fileStream.Close();

                var WebPath = Request.Scheme + "://" + Request.Host.Value + "/images/TempUpload/" + fileName;
                var NewImage = new ImageOfProduct
                {
                    ImageSrc = WebPath,
                    ProductId = ProductId,
                };
                _context.ImageOfProduct.Add(NewImage);
                await _context.SaveChangesAsync();
            }
                return RedirectToAction(nameof(CreateImage),new { id=ProductId});
        }


        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.SingleOrDefaultAsync(m => m.ProductId == id);
            var _user = await GetCurrentUserAsync();
            if (product.OwnerId != _user.Id)
            {
                throw new Exception();
            }
            if (product == null)
            {
                return NotFound();
            }
            return View(nameof(Edit),product);
        }

        public async Task<IActionResult> PreEdit(int id)
        {
            var _product = await _context.Products.Include(t=>t.ImageOfProducts).SingleOrDefaultAsync(t=>t.ProductId==id);
            return View(_product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,BuyTimes,OwnerId,ProductDetails,ProductInfo,ProductName,ProductWarnning")] Product product)
        {
            var _user = await GetCurrentUserAsync();
            product.OwnerId = _user.Id;
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(PreEdit), new { id = product.ProductId });
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.SingleOrDefaultAsync(m => m.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }
            var _user = await GetCurrentUserAsync();
            if (product.OwnerId != _user.Id)
            {
                throw new Exception();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.SingleOrDefaultAsync(m => m.ProductId == id);
            var _user = await GetCurrentUserAsync();
            if (product.OwnerId != _user.Id)
            {
                throw new Exception();
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
    }
}
