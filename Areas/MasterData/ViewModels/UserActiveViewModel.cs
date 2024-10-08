﻿namespace PurchasingSystemApps.Areas.MasterData.ViewModels
{
    public class UserActiveViewModel
    {
        public Guid UserActiveId { get; set; }
        public string UserActiveCode { get; set; }
        public string FullName { get; set; }
        public string IdentityNumber { get; set; }
        public string PlaceOfBirth { get; set; }
        public DateTime DateOfBirth { get; set; } = DateTime.Now;
        public string Gender { get; set; }
        public string Address { get; set; }
        public string Handphone { get; set; }
        public string Email { get; set; }
        public IFormFile? Foto { get; set; }
        public string? UserPhotoPath { get; set; }
    }
}
