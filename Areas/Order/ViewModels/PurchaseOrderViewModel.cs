using PurchasingSystemApps.Areas.Order.Models;

namespace PurchasingSystemApps.Areas.Order.ViewModels
{
    public class PurchaseOrderViewModel
    {
        public Guid PurchaseOrderId { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public Guid? PurchaseRequestId { get; set; }
        public string PurchaseRequestNumber { get; set; }
        public string UserAccessId { get; set; }
        public Guid? UserApprovalId { get; set; }
        public Guid? TermOfPaymentId { get; set; }
        public string Status { get; set; }
        public int QtyTotal { get; set; }
        public decimal GrandTotal { get; set; }
        public string? Note { get; set; }
        public List<PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetail>();
    }
}
