using Microsoft.EntityFrameworkCore;
using PurchasingSystemApps.Areas.Order.Models;
using PurchasingSystemApps.Data;

namespace PurchasingSystemApps.Areas.Order.Repositories
{
    public class IApprovalRepository
    {
        private string _errors = "";
        private readonly ApplicationDbContext _context;

        public IApprovalRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public string GetErrors()
        {
            return _errors;
        }

        public Approval Tambah(Approval Approval)
        {
            _context.Approvals.Add(Approval);
            _context.SaveChanges();
            return Approval;
        }

        public async Task<Approval> GetApprovalById(Guid Id)
        {
            var Approval = _context.Approvals
                .Where(i => i.ApprovalId == Id)
                .Include(u => u.ApplicationUser)
                .Include(t => t.PurchaseRequest)
                .Include(r => r.UserApproval)
                .FirstOrDefault(p => p.ApprovalId == Id);

            if (Approval != null)
            {
                var ApprovalDetail = new Approval()
                {
                    ApprovalId = Approval.ApprovalId,
                    PurchaseRequestId = Approval.PurchaseRequestId,
                    PurchaseRequestNumber = Approval.PurchaseRequestNumber,
                    UserAccessId = Approval.UserAccessId,
                    ApplicationUser = Approval.ApplicationUser,
                    UserApprovalId = Approval.UserApprovalId,
                    ApproveDate = Approval.ApproveDate,
                    ApproveBy = Approval.ApproveBy,
                    Status = Approval.Status,
                    Note = Approval.Note
                };
                return ApprovalDetail;
            }
            return Approval;
        }

        public async Task<Approval> GetApprovalByIdNoTracking(Guid Id)
        {
            return await _context.Approvals.AsNoTracking().Where(i => i.ApprovalId == Id).FirstOrDefaultAsync(a => a.ApprovalId == Id);
        }

        public async Task<List<Approval>> GetApprovals()
        {
            return await _context.Approvals./*OrderBy(p => p.CreateDateTime).*/Select(Approval => new Approval()
            {
                ApprovalId = Approval.ApprovalId,
                PurchaseRequestId = Approval.PurchaseRequestId,
                PurchaseRequestNumber = Approval.PurchaseRequestNumber,
                UserAccessId = Approval.UserAccessId,
                ApplicationUser = Approval.ApplicationUser,
                UserApprovalId = Approval.UserApprovalId,
                ApproveDate = Approval.ApproveDate,
                ApproveBy = Approval.ApproveBy,
                Status = Approval.Status,
                Note = Approval.Note
            }).ToListAsync();
        }

        public IEnumerable<Approval> GetAllApproval()
        {
            return _context.Approvals
                .Include(u => u.ApplicationUser)
                .Include(t => t.PurchaseRequest)
                .Include(r => r.UserApproval)
                .ToList();
        }

        public async Task<Approval> Update(Approval update)
        {          
            var Approval = _context.Approvals.Attach(update);
            Approval.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return update;
        }

        public Approval Delete(Guid Id)
        {
            var Approval = _context.Approvals.Find(Id);
            if (Approval != null)
            {
                _context.Approvals.Remove(Approval);
                _context.SaveChanges();
            }
            return Approval;
        }
    }
}
