using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PurchasingSystemApps.Data;
using PurchasingSystemApps.Models;
using PurchasingSystemApps.Repositories;
using System.Diagnostics;

namespace PurchasingSystemApps.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;

        public HomeController(ILogger<HomeController> logger,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)
        {
            _logger = logger;
            _applicationDbContext = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            ViewBag.Active = "Dashboard";
            var countUser = _applicationDbContext.UserActives.GroupBy(u => u.UserActiveId).Select(y => new
            {
                UserActiveId = y.Key,
                CountOfUsers = y.Count()
            }).ToList();
            ViewBag.CountUser = countUser.Count;

            var countPrincipal = _applicationDbContext.Principals.GroupBy(u => u.PrincipalId).Select(y => new
            {
                PrincipalId = y.Key,
                CountOfPrincipals = y.Count()
            }).ToList();
            ViewBag.CountPrincipal = countPrincipal.Count;

            var countProduct = _applicationDbContext.Products.GroupBy(u => u.ProductId).Select(y => new
            {
                ProductId = y.Key,
                CountOfProducts = y.Count()
            }).ToList();
            ViewBag.CountProduct = countProduct.Count;

            var countPurchaseRequest = _applicationDbContext.PurchaseRequests.GroupBy(u => u.PurchaseRequestId).Select(y => new
            {
                PurchaseRequestId = y.Key,
                CountOfPurchaseRequests = y.Count()
            }).ToList();
            ViewBag.CountPurchaseRequest = countPurchaseRequest.Count;

            var countPurchaseOrder = _applicationDbContext.PurchaseOrders.GroupBy(u => u.PurchaseOrderId).Select(y => new
            {
                PurchaseOrderId = y.Key,
                CountOfPurchaseOrders = y.Count()
            }).ToList();
            ViewBag.CountPurchaseOrder = countPurchaseOrder.Count;

            var countReceiveOrder = _applicationDbContext.ReceiveOrders.GroupBy(u => u.ReceiveOrderId).Select(y => new
            {
                ReceiveOrderId = y.Key,
                CountOfReceiveOrders = y.Count()
            }).ToList();
            ViewBag.CountReceiveOrder = countReceiveOrder.Count;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
