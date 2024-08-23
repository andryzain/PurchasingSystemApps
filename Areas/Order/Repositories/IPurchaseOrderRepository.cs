using Microsoft.EntityFrameworkCore;
using PurchasingSystemApps.Areas.Order.Models;
using PurchasingSystemApps.Data;

namespace PurchasingSystemApps.Areas.Order.Repositories
{
    public class IPurchaseOrderRepository
    {
        private string _errors = "";
        private readonly ApplicationDbContext _context;

        public IPurchaseOrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public string GetErrors()
        {
            return _errors;
        }

        public PurchaseOrder Tambah(PurchaseOrder PurchaseOrder)
        {
            _context.PurchaseOrders.Add(PurchaseOrder);
            _context.SaveChanges();
            return PurchaseOrder;
        }

        public async Task<PurchaseOrder> GetPurchaseOrderById(Guid Id)
        {
            var PurchaseOrder = _context.PurchaseOrders
                .Where(i => i.PurchaseOrderId == Id)
                .Include(d => d.PurchaseOrderDetails)
                .Include(u => u.ApplicationUser)
                .Include(t => t.TermOfPayment)
                .Include(a => a.UserApproval)
                .FirstOrDefault(p => p.PurchaseOrderId == Id);

            if (PurchaseOrder != null)
            {
                var PurchaseOrderDetail = new PurchaseOrder()
                {
                    PurchaseOrderId = PurchaseOrder.PurchaseOrderId,
                    PurchaseOrderNumber = PurchaseOrder.PurchaseOrderNumber,
                    PurchaseRequestId = PurchaseOrder.PurchaseRequestId,
                    PurchaseRequestNumber = PurchaseOrder.PurchaseRequestNumber,
                    UserAccessId = PurchaseOrder.UserAccessId,
                    ApplicationUser = PurchaseOrder.ApplicationUser,
                    UserApprovalId = PurchaseOrder.UserApprovalId,
                    UserApproval = PurchaseOrder.UserApproval,
                    TermOfPaymentId = PurchaseOrder.TermOfPaymentId,
                    TermOfPayment = PurchaseOrder.TermOfPayment,
                    Status = PurchaseOrder.Status,
                    QtyTotal = PurchaseOrder.QtyTotal,
                    GrandTotal = PurchaseOrder.GrandTotal,
                    Note = PurchaseOrder.Note,
                    PurchaseOrderDetails = PurchaseOrder.PurchaseOrderDetails,
                };
                return PurchaseOrderDetail;
            }
            return PurchaseOrder;
        }

        public async Task<PurchaseOrder> GetPurchaseOrderByIdNoTracking(Guid Id)
        {
            return await _context.PurchaseOrders.AsNoTracking()
                .Where(i => i.PurchaseOrderId == Id)
                .Include(d => d.PurchaseOrderDetails)
                .Include(u => u.ApplicationUser)
                .Include(t => t.TermOfPayment)
                .Include(y => y.UserApproval)
                .FirstOrDefaultAsync(a => a.PurchaseOrderId == Id);
        }

        public async Task<List<PurchaseOrder>> GetPurchaseOrders()
        {
            return await _context.PurchaseOrders.OrderBy(p => p.CreateDateTime).Select(PurchaseOrder => new PurchaseOrder()
            {
                PurchaseOrderId = PurchaseOrder.PurchaseOrderId,
                PurchaseOrderNumber = PurchaseOrder.PurchaseOrderNumber,
                PurchaseRequestId = PurchaseOrder.PurchaseRequestId,
                PurchaseRequestNumber = PurchaseOrder.PurchaseRequestNumber,
                UserAccessId = PurchaseOrder.UserAccessId,
                ApplicationUser = PurchaseOrder.ApplicationUser,
                UserApprovalId = PurchaseOrder.UserApprovalId,
                UserApproval = PurchaseOrder.UserApproval,
                TermOfPaymentId = PurchaseOrder.TermOfPaymentId,
                TermOfPayment = PurchaseOrder.TermOfPayment,
                Status = PurchaseOrder.Status,
                QtyTotal = PurchaseOrder.QtyTotal,
                GrandTotal = PurchaseOrder.GrandTotal,
                Note = PurchaseOrder.Note,
                PurchaseOrderDetails = PurchaseOrder.PurchaseOrderDetails
            }).ToListAsync();
        }

        public async Task<List<PurchaseOrder>> GetPurchaseOrdersFilters()
        {
            return await _context.PurchaseOrders.Where(p => p.Status == "InProcess").OrderBy(p => p.CreateDateTime).Select(PurchaseOrder => new PurchaseOrder()
            {
                PurchaseOrderId = PurchaseOrder.PurchaseOrderId,
                PurchaseOrderNumber = PurchaseOrder.PurchaseOrderNumber,
                PurchaseRequestId = PurchaseOrder.PurchaseRequestId,
                PurchaseRequestNumber = PurchaseOrder.PurchaseRequestNumber,
                UserAccessId = PurchaseOrder.UserAccessId,
                ApplicationUser = PurchaseOrder.ApplicationUser,
                UserApprovalId = PurchaseOrder.UserApprovalId,
                UserApproval = PurchaseOrder.UserApproval,
                TermOfPaymentId = PurchaseOrder.TermOfPaymentId,
                TermOfPayment = PurchaseOrder.TermOfPayment,
                Status = PurchaseOrder.Status,
                QtyTotal = PurchaseOrder.QtyTotal,
                GrandTotal = PurchaseOrder.GrandTotal,
                Note = PurchaseOrder.Note,
                PurchaseOrderDetails = PurchaseOrder.PurchaseOrderDetails
            }).ToListAsync();
        }

        public IEnumerable<PurchaseOrder> GetAllPurchaseOrder()
        {
            return _context.PurchaseOrders
                .Include(d => d.PurchaseOrderDetails)
                .Include(u => u.ApplicationUser)
                .Include(t => t.TermOfPayment)
                .Include(y => y.UserApproval)
                .ToList();
        }

        public async Task<PurchaseOrder> Update(PurchaseOrder update)
        {
            List<PurchaseOrderDetail> PurchaseOrderDetails = _context.PurchaseOrderDetails.Where(d => d.PurchaseOrderId == update.PurchaseOrderId).ToList();
            _context.PurchaseOrderDetails.RemoveRange(PurchaseOrderDetails);
            _context.SaveChanges();

            var PurchaseOrder = _context.PurchaseOrders.Attach(update);
            PurchaseOrder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.PurchaseOrderDetails.AddRangeAsync(update.PurchaseOrderDetails);
            _context.SaveChanges();
            return update;
        }

        public PurchaseOrder Delete(Guid Id)
        {
            var PurchaseOrder = _context.PurchaseOrders.Find(Id);
            if (PurchaseOrder != null)
            {
                _context.PurchaseOrders.Remove(PurchaseOrder);
                _context.SaveChanges();
            }
            return PurchaseOrder;
        }
    }
}
