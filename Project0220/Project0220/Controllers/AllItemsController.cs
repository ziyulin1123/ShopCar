﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Project0220.Models;
using Project0220.myModels;
using Project0220.ViewModel;

namespace Project0220.Controllers
{
    public class AllItemsController : Controller
    {
        private readonly ScaffoldEcommerceDbContext _contextNew;


        public AllItemsController(ScaffoldEcommerceDbContext contextNew)
        {
            _contextNew = contextNew;
        }

        public async Task<IActionResult> Index(string bee)
        {

            var data = from o2 in _contextNew.Products
                       select o2;

            if (!string.IsNullOrEmpty(bee))
            {
                data = data.Where(o => o.ProductName!.Contains(bee));
            }

            return View(await data.ToListAsync());
        }
        [HttpPost]
        public IActionResult Search(string bee)
        {
            if (bee == null)
            {
                return BadRequest("Search string cannot be null.");
            }

            // Redirect to Index page with search query
            return RedirectToAction("Index", new { bee });
        }

        [HttpPost]
        public IActionResult Subscribe(string email)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId.HasValue)
            {
                //找到這個人是誰  從客戶資料表裡面找
                var user = _contextNew.Customers.Find(userId.Value);
                if (user != null)
                {   //確定有此使用者
                    //var product = _contextNew.Products.Find(ProductId);

                    return Json(new { success = false, message = "成功" });
                }
                else
                {
                    // 使用者未驗證
                    return Json(new { success = false, message = "User not authenticated" });
                }
            }
            else
            {
                // 使用者未登錄
                return Json(new { success = false, message = "尚未登錄 請登入" });
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> Index(string bee)
        //{

        //    var data = from o2 in _context.Products
        //               where o2.ProductName!.IndexOf(bee) > -1
        //               select o2;
        //    return View(await data.ToListAsync());
        //}

        public async Task<IActionResult> ItemDetails(int bee)
        {
            var z = bee;
            var data = await _contextNew.Products
                            .Where(o2 => o2.ProductId == bee)
                            .ToListAsync();


            var category = await _contextNew.Categories
                                            .Where(c => c.CategoryId == bee)
                                            .ToListAsync();
            var viewModel = new ItemDetailsViewModel
            {
                Products = data,
                Category = category
            };
            return View(viewModel);

        }

        public async Task<IActionResult> Clearance(string bee)
        {

            var data = from o2 in _contextNew.Products
                       where o2.SpecialZoneType == "出清"
                       select o2;

            if (!string.IsNullOrEmpty(bee))
            {
                data = data.Where(o => o.ProductName.Contains(bee));
            }

            return View(await data.ToListAsync());
        }




        public async Task<IActionResult> New(string bee)
        {

            var data = from o2 in _contextNew.Products
                       where o2.SpecialZoneType == "新品"
                       select o2;

            if (!string.IsNullOrEmpty(bee))
            {
                data = data.Where(o => o.ProductName.Contains(bee));
            }

            return View(await data.ToListAsync());
        }


        //會員追蹤 
        [HttpPost]
        public IActionResult FollowColor(int ProductId, string Color)
        {
         
            //先判斷這個人是誰
            var userId = Convert.ToInt32(HttpContext.Request.Cookies["membercookie"]);
            if (userId != null)
            {
                //找到這個人是誰  從客戶資料表裡面找
                var user = _contextNew.Customers.Find(userId);
                if (user != null)
                {   //確定有此使用者
                    var product = _contextNew.Products.Find(ProductId);

                    if (product != null)
                    {
                        var existingTrack = _contextNew.TrackLists
                    .FirstOrDefault(t => t.CustomerID == userId && t.ProductID == ProductId && t.Color == Color);

                        if (existingTrack == null)
                        {

                            var trackListModel = new TrackList
                            {
                                CustomerID = userId,
                                ProductID = ProductId,
                                Color = Color

                            };
                           
                            _contextNew.TrackLists.Add(trackListModel);
                            _contextNew.SaveChanges();

                            return Json(new { success = true, message = "商品追蹤成功" });
                        }
                        else
                        {
                            return Json(new { success = false, message = "產品已追蹤過了" });
                        }
                    }
                    else
                    {
                        // 使用者未驗證
                        return Json(new { success = false, message = "此商品不存在" });
                    }
                }
                else
                {
                    // 使用者未登錄
                    return Json(new { success = false, message = "尚未登錄 請登入" });
                }

            }

            return Json(new { success = false, message = "尚未登錄 請登入" });
        
    }

        [HttpPost]
        public IActionResult Follow(string data)
        {
            myModels.Product products = JsonConvert.DeserializeObject<myModels.Product>(data);
            int _productId = products.ProductId;
            //先判斷這個人是誰
            var userId = Convert.ToInt32(HttpContext.Request.Cookies["membercookie"]);
            if (userId != null)
            {
                //找到這個人是誰  從客戶資料表裡面找
                var user = _contextNew.Customers.Find(userId);
                if (user != null)
                {   //確定有此使用者
                    var product = _contextNew.Products.Find(_productId);

                    if (product != null)
                    {
                        var existingTrack = _contextNew.TrackLists
                    .FirstOrDefault(t => t.CustomerID == userId && t.ProductID == _productId );

                        if (existingTrack == null)
                        {

                            var trackListModel = new TrackList
                            {
                                CustomerID = userId,
                                ProductID = _productId,
                          

                            };

                            _contextNew.TrackLists.Add(trackListModel);
                            _contextNew.SaveChanges();

                            return Json(new { success = true, message = "Product tracked successfully" });
                        }
                        else
                        {
                            return Json(new { success = false, message = "產品已追蹤過了" });
                        }
                    }
                    else
                    {
                        // 使用者未驗證
                        return Json(new { success = false, message = "此商品不存在" });
                    }
                }
                else
                {
                    // 使用者未登錄
                    return Json(new { success = false, message = "尚未登錄 請登入" });
                }

            }

            return Json(new { success = false, message = "尚未登錄 請登入" });

        }



     






    }
}
