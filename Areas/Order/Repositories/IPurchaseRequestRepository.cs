using Microsoft.EntityFrameworkCore;
using PurchasingSystemApps.Areas.Order.Models;
using PurchasingSystemApps.Data;
using PurchasingSystemApps.Repositories;

namespace PurchasingSystemApps.Areas.Order.Repositories
{
    public class IPurchaseRequestRepository
    {
        private string _errors = "";
        private readonly ApplicationDbContext _context;

        public IPurchaseRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public string GetErrors()
        {
            return _errors;
        }

        public PurchaseRequest Tambah(PurchaseRequest purchaseRequest)
        {
            _context.PurchaseRequests.Add(purchaseRequest);
            _context.SaveChanges();
            return purchaseRequest;
        }

        public async Task<PurchaseRequest> GetPurchaseRequestById(Guid Id)
        {
            var purchaseRequest = _context.PurchaseRequests
                .Where(i => i.PurchaseRequestId == Id)
                .Include(d => d.PurchaseRequestDetails)
                .Include(u => u.ApplicationUser)
                .Include(t => t.TermOfPayment)
                .Include(a => a.UserApproval)
                .FirstOrDefault(p => p.PurchaseRequestId == Id);

            if (purchaseRequest != null)
            {
                var purchaseRequestDetail = new PurchaseRequest()
                {
                    PurchaseRequestId = purchaseRequest.PurchaseRequestId,
                    PurchaseRequestNumber = purchaseRequest.PurchaseRequestNumber,
                    UserAccessId = purchaseRequest.UserAccessId,
                    ApplicationUser = purchaseRequest.ApplicationUser,
                    UserApprovalId = purchaseRequest.UserApprovalId,
                    UserApproval = purchaseRequest.UserApproval,
                    TermOfPaymentId = purchaseRequest.TermOfPaymentId,
                    TermOfPayment = purchaseRequest.TermOfPayment,                    
                    Status = purchaseRequest.Status,
                    QtyTotal = purchaseRequest.QtyTotal,
                    GrandTotal = purchaseRequest.GrandTotal,
                    Note = purchaseRequest.Note,
                    PurchaseRequestDetails = purchaseRequest.PurchaseRequestDetails,
                };
                return purchaseRequestDetail;
            }
            return purchaseRequest;
        }

        public async Task<PurchaseRequest> GetPurchaseRequestByIdNoTracking(Guid Id)
        {
            return await _context.PurchaseRequests.AsNoTracking()
                .Where(i => i.PurchaseRequestId == Id)
                .Include(d => d.PurchaseRequestDetails)
                .Include(u => u.ApplicationUser)
                .Include(t => t.TermOfPayment)
                .Include(y => y.UserApproval)
                .FirstOrDefaultAsync(a => a.PurchaseRequestId == Id);
        }

        public async Task<List<PurchaseRequest>> GetPurchaseRequests()
        {
            return await _context.PurchaseRequests./*OrderBy(p => p.CreateDateTime).*/Select(purchaseRequest => new PurchaseRequest()
            {
                PurchaseRequestId = purchaseRequest.PurchaseRequestId,
                PurchaseRequestNumber = purchaseRequest.PurchaseRequestNumber,
                UserAccessId = purchaseRequest.UserAccessId,
                ApplicationUser = purchaseRequest.ApplicationUser,
                UserApprovalId = purchaseRequest.UserApprovalId,
                UserApproval = purchaseRequest.UserApproval,
                TermOfPaymentId = purchaseRequest.TermOfPaymentId,
                TermOfPayment = purchaseRequest.TermOfPayment,
                Status = purchaseRequest.Status,
                QtyTotal = purchaseRequest.QtyTotal,
                GrandTotal = purchaseRequest.GrandTotal,
                Note = purchaseRequest.Note,
                PurchaseRequestDetails = purchaseRequest.PurchaseRequestDetails
            }).ToListAsync();
        }

        public IEnumerable<PurchaseRequest> GetAllPurchaseRequest()
        {
            return _context.PurchaseRequests
                .Include(d => d.PurchaseRequestDetails)
                .Include(u => u.ApplicationUser)
                .Include(t => t.TermOfPayment)
                .Include(y => y.UserApproval)
                .ToList();
        }

        public async Task<PurchaseRequest> Update(PurchaseRequest update)
        {
            List<PurchaseRequestDetail> purchaseRequestDetails = _context.PurchaseRequestDetails.Where(d => d.PurchaseRequestId == update.PurchaseRequestId).ToList();
            _context.PurchaseRequestDetails.RemoveRange(purchaseRequestDetails);
            _context.SaveChanges();

            var purchaseRequest = _context.PurchaseRequests.Attach(update);
            purchaseRequest.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.PurchaseRequestDetails.AddRangeAsync(update.PurchaseRequestDetails);
            _context.SaveChanges();
            return update;
        }

        public PurchaseRequest Delete(Guid Id)
        {
            var PurchaseRequest = _context.PurchaseRequests.Find(Id);
            if (PurchaseRequest != null)
            {
                _context.PurchaseRequests.Remove(PurchaseRequest);
                _context.SaveChanges();
            }
            return PurchaseRequest;
        }
    }
}
