﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Project0220.Models;
using Project0220.myModels;
using Project0220.ViewModel;
using System.Text.Json;
using System;
using System.Drawing;
using System.Linq;
using static NuGet.Packaging.PackagingConstants;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics.Metrics;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Xml.Linq;
using Newtonsoft.Json;
namespace Project0220.Controllers
{
    public class CheckOutController : Controller
    {

        private readonly ScaffoldEcommerceDbContext _context;

        public CheckOutController(ScaffoldEcommerceDbContext context)
        {
            _context = context;
        }

       

        public IActionResult Index()
        {
            return View();
        }

        public void OnGet()
        {
            if (Request.Cookies.ContainsKey("membercookie"))
            {
                // 從 cookie 讀取 customerId
                var customerId = Request.Cookies["membercookie"];

                // 將 customerId 賦值給模型或 ViewData
                ViewData["CustomerId"] = customerId;
            }
            else
            {
                // 設置一個默認值或保持未設置
                ViewData["CustomerId"] = ""; // 或根據需求進行調整
            }
        }

        //public IActionResult NextConsignee()
        //{

        //    // 檢查是否存在名為 "membercookie" 的 cookie
        //    if (HttpContext.Request.Cookies["membercookie"] != null)
        //    {
        //        // 如果存在相應的 cookie，繼續執行其他操作
        //        return RedirectToAction("ChechOut", "Consignee", new { id = HttpContext.Request.Cookies["membercookie"] });

        //    }

        //    // 如果用戶未通過身份驗證，導向登入頁面
        //    return RedirectToAction("Login", "Customers");

        //}

        //private bool IsAuthenticated()
        //{
        //    // 檢查是否存在名為 "membercookie" 的 cookie
        //    var memberCookie = HttpContext.Request.Cookies["membercookie"];
        //    return !string.IsNullOrEmpty(memberCookie);
        //}

        public IActionResult Consignee()
        {
            // 從 cookie 讀取 customerId
            var customerId = Request.Cookies["membercookie"];

            // 將 customerId 賦值給 ViewData
            ViewData["CustomerId"] = customerId;

            // 初始化 OrderDate 為當前日期
            var Order = new Order            {
                OrderDate = DateTime.Today
            };

            return View(Order);
            //var memberCookie = Convert.ToInt32(HttpContext.Request.Cookies["membercookie"]);
            //ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId");
        }


        

        //[HttpPost]
        //public IActionResult Create(int customerId, int orderId, string consignee, 
        //    string contactPhone, string Carrier, string ShippingDate, string PostalCode, 
        //    string ShippingAddress, string PaymentMethod
        //    )
        //{  
        //    // 從Cookies獲取已登入用戶的CustomerID
        //    var memberCookie = HttpContext.Request.Cookies["membercookie"];
        //    var customerIDFromCookie = _context.Customers
        //        .FirstOrDefault(c => c.CustomerId.ToString() == memberCookie)?.CustomerId;

        //if (customerIDFromCookie == null || customerIDFromCookie != customerId)
        //{
        //    // 如果cookie中的customerID與傳入的customerID不匹配，返回錯誤
        //    return Unauthorized();
        //}
        //// 檢查用戶是否已通過身份驗證，以確保只有登入用戶才能添加收件人資料
        //if (!IsAuthenticated())
        //{
        //    // 如果用戶未登入，可以選擇重定向到登入頁面或返回錯誤
        //    return RedirectToAction("Login", "Customer"); // 假設有一個名為Login的Action
        //}



        //根據customerId和orderId找到對應的Order
        //var order = _context.Orders.FirstOrDefault(o => o.CustomerId == customerId && o.OrderId == orderId);
        // if (order == null)
        // {
        //     // 如果沒有找到訂單，返回錯誤
        //     return NotFound();
        // }

        // // 更新訂單的收件人資料
        // order.Consignee = consignee;
        // order.ContactPhone = contactPhone;
        // order.Carrier = Carrier;
        // order.ShippingAddress = ShippingAddress;
        // order.PostalCode = PostalCode;
        // order.PaymentMethod = PaymentMethod;

        //if (DateOnly.TryParse(ShippingDate, out DateOnly parsedDate))
        //{
        //    order.ShippingDate = parsedDate;
        //}
        //else
        //{
        //    // 日期轉換失敗，向ModelState添加一個錯誤
        //    ModelState.AddModelError("ShippingDate", "日期轉換失敗。請確保日期格式正確，例如：2023-01-31。");

        //    // 可以選擇將用戶重新導向到表單頁面，並顯示錯誤消息
        //    // 假設您有一個名為"EditOrder"的視圖，用於編輯訂單
        //    return View("CheckOut", order); // 確保將模型傳回視圖，以便用戶可以看到他們原先的輸入和錯誤消息
        //}


        // 保存變更到資料庫
        // _context.SaveChanges();

        // 返回適當的響應，比如重定向到某個頁面或返回成功訊息
        //    return RedirectToAction("CheckOut", new { orderId = orderId }); // 有一個顯示訂單詳情的Action
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,CustomerId,OrderDate,TotalAmount,PaymentMethod,Carrier,ShippingDate,PostalCode,ShippingAddress,Consignee,ContactPhone,Status")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CheckOut));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", order.CustomerId);
            return View(order);
        }




        public async Task<IActionResult> CheckOut()
        {
            var customerId = Convert.ToInt32(Request.Cookies["membercookie"]);
            ViewData["CustomerId"] = customerId;//找customerId 

            var cartItems = await _context.CartItems.Where(a => a.CustomerID == customerId).ToListAsync();
            var customer = await _context.Customers.FirstOrDefaultAsync(a => a.CustomerId == customerId);
            // 如果没有找到客户或購物車為，則返回一個空的模型列表
            if (customer == null || !cartItems.Any())
            {
                return View(new List<CartOrderModel>()); // 確保這裡有返回
            }

            var productIds = cartItems.Select(item => item.ProductID).ToList();//找加入購物車的東西
            var products = new List<myModels.Product>();
            if (productIds.Any())
            {
                products = await _context.Products.Where(a => productIds.Contains(a.ProductId)).ToListAsync();
            } //如果有產品ID存在（productIds.Any()），則這段程式碼會從資料庫中查詢與這些ID相匹配的產品。

            var lastOrder = await _context.Orders.Where(o => o.CustomerId == customerId).OrderByDescending(o => o.OrderId).FirstOrDefaultAsync();

            List<CartOrderModel> modelList = new List<CartOrderModel>();
            var model = new CartOrderModel
            {
                CartItem = cartItems,
                Customer = customer,
                Order = lastOrder,
                Products = products
            };

            modelList.Add(model);

            return View(modelList);
        }


            
            [HttpPost]
            public async Task<IActionResult> SaveCheckOut(int orderId)
            {
            var customerId = Convert.ToInt32(Request.Cookies["membercookie"]);
            var cartItems = await _context.CartItems.Where(a => a.CustomerID == customerId).ToListAsync();

            if (!cartItems.Any())
            {
                return View("Error");
            }

            // 根據 customerId 查找最新的訂單
            var order = await _context.Orders
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate) // 假設有 OrderDate 表示訂單日期
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound($"沒有客戶 ID{customerId} 的訂單。");
            }

            int totalAmount = 0; // 初始化總金額為 int 類型

            // 使用找到的訂單 OrderId
            foreach (var item in cartItems)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == item.ProductID);
                if (product != null)
                {
                   
                    var orderDetails = new OrderDetail
                    {
                        OrderId = orderId,
                        ProductId = item.ProductID,
                        Quantity = item.Quantity,
                        UnitPrice = product.UnitPrice,
                        SelectedColor = item.SelectedColor,
                        Discount = 1,
                        Amount =item.Quantity * product.UnitPrice

                    };
                        // 將 OrderDetail 添加到資料庫
                    _context.OrderDetails.Add(orderDetails);
                    
                    int amount = item.Quantity * product.UnitPrice.GetValueOrDefault();
                    totalAmount += amount; // 累加到總金額

                    // 減少產品庫存
                    product.UnitInStock -= item.Quantity;
                }
            }
            

            // 更新 Order 的 TotalAmount 計算出的總金額
            order.TotalAmount = totalAmount;
            order.Status = "配送中";
            await _context.SaveChangesAsync(); // 更新 Order 的總金額

            // 清空購物車
            _context.CartItems.RemoveRange(cartItems);
                await _context.SaveChangesAsync();

            
            return RedirectToAction("CheckOutDone", new { orderId = orderId }); 
        }



       
        //public async Task<IActionResult> CheckOut()
        //{
        //    var customerId = Convert.ToInt32(Request.Cookies["membercookie"]);
        //    ViewData["CustomerId"] = customerId;

        //    List<CartOrderModel> modelList = new List<CartOrderModel>();


        //    // var lastOrder = await _context.Orders.OrderByDescending(o => o.OrderId).FirstOrDefaultAsync();

        //    var cartItems = await _context.CartItems.Where(a => a.CustomerID == customerId).ToListAsync();
        //    var customer = await _context.Customers.FirstOrDefaultAsync(a => a.CustomerId == customerId);
        //    var lastOrder = await _context.Orders.OrderByDescending(o => o.OrderId).FirstOrDefaultAsync();
        //    var productIds = cartItems.Select(item => item.ProductID).ToList();

        //    // 如果productIds为空，则没有必要查询数据库
        //    var products = _context.Products.Where(a => productIds.Contains(a.ProductId));

        //    // 填充modelList
        //    if (cartItems.Any() && customer != null && products.Any())
        //    {
        //        var model = new CartOrderModel
        //        {
        //            CartItem = cartItems,
        //            Customer = customer,

        //        };

        //        modelList.Add(model);
        //    }

        //    return View(modelList);


        //var customerId = Convert.ToInt32(Request.Cookies["membercookie"]);
        // ViewData["CustomerId"] = customerId;

        //List<CartOrderModel> modelList = new List<CartOrderModel>();

        //var lastOrder = await _context.Orders.OrderByDescending(o => o.OrderId).FirstOrDefaultAsync();

        //var carItem = _context.CartItems.Where(a => a.CustomerID ==customerId ) ;

        //var customer =  _context.Customers.FirstOrDefault(a => a.CustomerId == customerId);

        //List<int> productIds = new List<int>();

        //foreach(CartItem item in carItem)
        //{
        //    productIds.Add(item.ProductID);
        //}
        //var products = _context.Products.Where(a => productIds.Contains(a.ProductId));


        //return View(modelList);
        // 檢查是否有訂單存在
        //if (lastOrder == null)
        //{
        //    // 處理沒有訂單的情況，例如顯示錯誤信息或重定向
        //    return View("ErrorView"); // 假設有一個顯示錯誤的view
        //}

        //// 如果視圖需要處理多個訂單，保留下列代碼
        //return View(new List<Order> { lastOrder });

        //如果視圖只需要一個訂單，使用這行代碼
        //return View(lastOrder);


        public IActionResult CreateOrder()
        {
            var model = new Order
            {
                OrderDate = DateTime.Today // 設定為今天的日期
            };

            return View(model);
        }



        public async Task<IActionResult> CheckOutDone()
        {
            var lastOrder = await _context.Orders.OrderByDescending(o => o.OrderId).FirstOrDefaultAsync(); //遞減排序的第一個

            return View(new List<Order> { lastOrder });

        }

  
    }
}
