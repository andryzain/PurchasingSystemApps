using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PurchasingSystemApps.Areas.MasterData.Models;
using PurchasingSystemApps.Areas.MasterData.Repositories;
using PurchasingSystemApps.Areas.MasterData.ViewModels;
using PurchasingSystemApps.Areas.Order.Models;
using PurchasingSystemApps.Areas.Order.Repositories;
using PurchasingSystemApps.Areas.Order.ViewModels;
using PurchasingSystemApps.Data;
using PurchasingSystemApps.Models;
using PurchasingSystemApps.Repositories;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace PurchasingSystemApps.Areas.Order.Controllers
{
    [Area("Order")]
    [Route("Order/[Controller]/[Action]")]
    public class ApprovalController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUserActiveRepository _userActiveRepository;
        private readonly IApprovalRepository _approvalRepository;
        private readonly IProductRepository _productRepository;
        private readonly IPurchaseRequestRepository _purchaseRequestRepository;
        private readonly ITermOfPaymentRepository _termOfPaymentRepository;

        private readonly IHostingEnvironment _hostingEnvironment;

        public ApprovalController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            IApprovalRepository ApprovalRepository,
            IUserActiveRepository userActiveRepository,
            IProductRepository productRepository,
            IPurchaseRequestRepository purchaseRequestRepository,
            ITermOfPaymentRepository termOfPaymentRepository,

            IHostingEnvironment hostingEnvironment
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _approvalRepository = ApprovalRepository;
            _userActiveRepository = userActiveRepository;
            _productRepository = productRepository;
            _purchaseRequestRepository = purchaseRequestRepository;
            _termOfPaymentRepository = termOfPaymentRepository;

            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            ViewBag.Active = "Order";

            var countApproval = _applicationDbContext.Approvals.Where(p => p.Status == "Not Approved").GroupBy(u => u.PurchaseRequestId).Select(y => new
            {
                ApprovalId = y.Key,
                CountOfApprovals = y.Count()
            }).ToList();
            ViewBag.CountApproval= countApproval.Count;

            var getUserLogin = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var getUserActive = _userActiveRepository.GetAllUser().Where(c => c.UserActiveCode == getUserLogin.KodeUser).FirstOrDefault();

            if (getUserLogin.Id == "5f734880-f3d9-4736-8421-65a66d48020e")
            {
                var data = _approvalRepository.GetAllApproval();
                return View(data);
            }
            else 
            {
                var data = _approvalRepository.GetAllApproval().Where(u => u.UserApprovalId == getUserActive.UserActiveId).ToList();
                return View(data);
            }                        
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Index(DateTime tglAwalPencarian, DateTime tglAkhirPencarian)
        {
            ViewBag.Active = "Order";
            ViewBag.tglAwalPencarian = tglAwalPencarian.ToString("dd MMMM yyyy");
            ViewBag.tglAkhirPencarian = tglAkhirPencarian.ToString("dd MMMM yyyy");

            var data = _approvalRepository.GetAllApproval().Where(r => r.CreateDateTime.Date >= tglAwalPencarian && r.CreateDateTime.Date <= tglAkhirPencarian).ToList();
            return View(data);
        }
       
        [HttpGet]
        [AllowAnonymous]
        public async Task<ViewResult> DetailApproval(Guid Id)
        {
            ViewBag.Active = "Order";

            ViewBag.User = new SelectList(_userManager.Users, nameof(ApplicationUser.Id), nameof(ApplicationUser.NamaUser), SortOrder.Ascending);
            ViewBag.Product = new SelectList(await _productRepository.GetProducts(), "ProductId", "ProductName", SortOrder.Ascending);
            ViewBag.Approval = new SelectList(await _userActiveRepository.GetUserActives(), "UserActiveId", "FullName", SortOrder.Ascending);
            ViewBag.TermOfPayment = new SelectList(await _termOfPaymentRepository.GetTermOfPayments(), "TermOfPaymentId", "TermOfPaymentName", SortOrder.Ascending);

            var Approval = await _approvalRepository.GetApprovalById(Id);
            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            if (Approval == null)
            {
                Response.StatusCode = 404;
                return View("ApprovalNotFound", Id);
            }

            ApprovalViewModel viewModel = new ApprovalViewModel()
            {
                ApprovalId = Approval.ApprovalId,
                PurchaseRequestId = Approval.PurchaseRequestId,
                PurchaseRequestNumber = Approval.PurchaseRequestNumber,
                UserAccessId = Approval.UserAccessId,
                UserApprovalId = Approval.UserApprovalId,
                ApproveDate = Approval.ApproveDate,
                ApproveBy = getUser.NamaUser,
                Status = Approval.Status,
                Note = Approval.Note
            };

            var getPrNumber = _purchaseRequestRepository.GetAllPurchaseRequest().Where(pr => pr.PurchaseRequestNumber == viewModel.PurchaseRequestNumber).FirstOrDefault();

            viewModel.QtyTotal = getPrNumber.QtyTotal;
            viewModel.GrandTotal = Math.Truncate(getPrNumber.GrandTotal);

            var ItemsList = new List<PurchaseRequestDetail>();

            foreach (var item in getPrNumber.PurchaseRequestDetails)
            {
                ItemsList.Add(new PurchaseRequestDetail
                {
                    ProductNumber = item.ProductNumber,
                    ProductName = item.ProductName,
                    Principal = item.Principal,
                    Measurement = item.Measurement,
                    Qty = item.Qty,
                    Price = Math.Truncate(item.Price),
                    Discount = item.Discount,
                    SubTotal = Math.Truncate(item.SubTotal)
                });
            }

            viewModel.PurchaseRequestDetails = ItemsList;

            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> DetailApproval(ApprovalViewModel viewModel)
        {
            ViewBag.Active = "Order";

            if (ModelState.IsValid)
            {
                Approval approval = await _approvalRepository.GetApprovalByIdNoTracking(viewModel.ApprovalId);

                approval.Status = viewModel.Status;
                approval.Note = viewModel.Note;

                if (approval.Status == "Approved" || approval.Status == "Rejected")
                {
                    approval.ApproveDate = DateTime.Now;
                    approval.ApproveBy = viewModel.ApproveBy;
                }

                var result = _purchaseRequestRepository.GetAllPurchaseRequest().Where(c => c.PurchaseRequestNumber == viewModel.PurchaseRequestNumber).FirstOrDefault();
                if (result != null)
                {
                    result.Status = viewModel.Status;
                    result.Note = viewModel.Note;

                    _applicationDbContext.Entry(result).State = EntityState.Modified;
                }
                _approvalRepository.Update(approval);

                if (approval.Status == "Not Approved")
                {
                    TempData["SuccessMessage"] = "Number " + viewModel.PurchaseRequestNumber + " Not Approved";
                }
                else if (approval.Status == "Approved")
                {
                    TempData["SuccessMessage"] = "Number " + viewModel.PurchaseRequestNumber + " Approved";
                }
                else if (approval.Status == "Rejected")
                {
                    TempData["SuccessMessage"] = "Number " + viewModel.PurchaseRequestNumber + " Rejected";
                }
                return RedirectToAction("Index", "PurchaseOrder");
            }

            return View();
        }        
    }
}
