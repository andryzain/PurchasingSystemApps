using PurchasingSystemApps.Areas.Order.Models;

namespace PurchasingSystemApps.Areas.Order.ViewModels
{
    public class ApprovalViewModel
    {
        public Guid ApprovalId { get; set; }
        public Guid? PurchaseRequestId { get; set; }
        public string PurchaseRequestNumber { get; set; }
        public string UserAccessId { get; set; } //Dibuat Oleh
        public Guid? UserApprovalId { get; set; } //Mengetahui
        public DateTime ApproveDate { get; set; }
        public string ApproveBy { get; set; } //Disetujui Oleh
        public string Status { get; set; }
        public string? Note { get; set; }
        public int QtyTotal { get; set; }
        public decimal GrandTotal { get; set; }
        public List<PurchaseRequestDetail> PurchaseRequestDetails { get; set; }
    }
}
