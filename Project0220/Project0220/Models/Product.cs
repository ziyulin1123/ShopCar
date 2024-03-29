﻿using System.ComponentModel.DataAnnotations;

namespace Project0220.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }

        [Display(Name = "產品名稱")]
        public string ProductName { get; set; }

        [Display(Name = "供應商ID")]
        public int SupplierID { get; set; }

        [Display(Name = "類別ID")]
        public int CategoryID { get; set; }

        [Display(Name = "單價")]
        public int UnitPrice { get; set; }

        [Display(Name = "庫存")]
        public int UnitInStock { get; set; }

        [Display(Name = "圖片1")]
        public string Image1 { get; set; }

        [Display(Name = "圖片2")]
        public string? Image2 { get; set; }

        [Display(Name = "圖片3")]
        public string? Image3 { get; set; }

        [Display(Name = "圖片4")]
        public string? Image4 { get; set; }

        [Display(Name = "顏色1")]
        public string Color1 { get; set; }

        [Display(Name = "顏色2")]
        public string? Color2 { get; set; }

        [Display(Name = "長度")]
        public string? Length { get; set; }

        [Display(Name = "寬度")]
        public string? Width { get; set; }

        [Display(Name = "高度")]
        public string? Height { get; set; }

        [Display(Description ="描述")]
        public string? Description { get; set; }

        [Display(Name = "特殊區域類型")]
        public string? SpecialZoneType { get; set; }

        [Display(Name = "創建時間")]
        public DateTime CreatedAt { get; set; }

    }

}
