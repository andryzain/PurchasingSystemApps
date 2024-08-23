using PurchasingSystemApps.Repositories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurchasingSystemApps.Areas.MasterData.Models
{
    [Table("MstUserActive", Schema = "dbo")]
    public class UserActive : UserActivity
    {
        [Key]
        public Guid UserActiveId { get; set; }
        public string UserActiveCode { get; set; }
        public string FullName { get; set; }
        public string IdentityNumber { get; set; }
        public string PlaceOfBirth { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string Handphone { get; set; }
        public string Email { get; set; }
        public string? Foto { get; set; }
    }
}
