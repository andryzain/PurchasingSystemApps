using PurchasingSystemApps.Areas.MasterData.Models;
using PurchasingSystemApps.Models;
using PurchasingSystemApps.Repositories;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurchasingSystemApps.Areas.Order.Models
{
    [Table("OrdApproval", Schema = "dbo")]
    public class Approval : UserActivity
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

        //Relationship
        [ForeignKey("PurchaseRequestId")]
        public PurchaseRequest? PurchaseRequest { get; set; }
        [ForeignKey("UserApprovalId")]
        public UserActive? UserApproval { get; set; }
        [ForeignKey("UserAccessId")]
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
