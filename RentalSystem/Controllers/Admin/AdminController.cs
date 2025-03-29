using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RentalSystem.DTO;
using RentalSystem.Models;

namespace RentalSystem.Controllers.admin
{
    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly RentalSystemContext _context;
        private readonly IWebHostEnvironment _environment;
        public AdminController(RentalSystemContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public bool checkSession()
        {
            bool checkS = true;
            var httpContext = HttpContext;
            if (httpContext != null && httpContext.Session != null)
            {
                string isCustomerAuthenticated = httpContext.Session.GetString("admin");
                if (string.IsNullOrEmpty(isCustomerAuthenticated))
                {
                    checkS = false;
                }
            }

            return checkS;
        }
        [HttpGet("dashboard")]
        public IActionResult Dashboard(string service, int year)
        {
            if (checkSession() == false)
            {
                return RedirectToAction("Login");
            }
            int totalUsers = _context.Customers.Count(); // Đếm số lượng Users
            int totalBookings = _context.Bookings.Count(); // Đếm số lượng Bookings

            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalBookings = totalBookings;

            return View();
        }
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            var session = HttpContext.Session;

            if (session.Keys.Contains("admin"))
            {
                session.Remove("admin");
            }
            return RedirectToAction("Login");
        }

        [HttpGet("authenticate/login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("authenticate/login")]
        public IActionResult Login(string username, string password)
        {
            if (_context.Admins != null)
            {
                if (username == string.Empty || password == string.Empty)
                {
                    ViewBag.ErrorMess = "Tên đăng nhập hoặc mật khẩu không được bỏ trống.";
                    return View();
                }
                else
                {
                    Admin admin = _context.Admins.FirstOrDefault(x => x.FullName.Equals(username) && x.PasswordHash.Equals(password));
                    if (admin != null)
                    {
                      
                        string adminJson = JsonConvert.SerializeObject(admin);
                        HttpContext.Session.SetString("admin", adminJson);
                        return RedirectToAction("Dashboard");
                    }
                    else
                    {
                        ViewBag.ErrorMess = "Tên đăng nhập hoặc mật khẩu sai.";
                        return View();
                    }
                }
            }
            return View();
        }
        [HttpGet("orderlist")]
        public IActionResult OrderList()
        {
            if (!checkSession())
            {
                return RedirectToAction("Login");
            }

            try
            {
                // Lấy danh sách tất cả đơn hàng, sắp xếp theo ngày tạo mới nhất
                var orders = _context.Bookings
                                     .Include(o => o.Tenant)
                                     .OrderByDescending(o => o.CreatedAt)
                                     .ToList();

                ViewBag.OrderList = orders ?? new List<Booking>();

                if (!orders.Any())
                {
                    ViewBag.ErrorMessage = "There are no orders available.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.OrderList = new List<Booking>();
                ViewBag.ErrorMessage = "An error occurred while fetching orders. Please try again.";
                Console.WriteLine($"Error: {ex.Message}");
            }

            return View();
        }
        [HttpPost("change-order-status")]
        public IActionResult ChangeOrderStatus(int orderId)
        {
            if (!checkSession())
            {
                return RedirectToAction("Login");
            }

            try
            {
                var order = _context.Bookings.FirstOrDefault(o => o.BookingId == orderId);
                if (order == null)
                {
                    TempData["ErrorMessage"] = "Order not found.";
                    return RedirectToAction("OrderList");
                }

                if (order.Status == "Wait")
                {
                    order.Status = "Accept";
                    _context.Bookings.Update(order);
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "Order status updated successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Order status cannot be changed.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while updating order status.";
                Console.WriteLine($"Error: {ex.Message}");
            }

            return RedirectToAction("OrderList");
        }


        [HttpGet("orderdetail/{orderId}")]
        public IActionResult OrderDetail(int orderId)
        {
            if (!checkSession())
            {
                return RedirectToAction("Login");
            }

            try
            {
                // Lấy đơn hàng theo ID và bao gồm thông tin chi tiết đơn hàng
                var order = _context.Bookings
                                    .Include(o => o.Tenant)
                         
                     
                                    .FirstOrDefault(o => o.BookingId == orderId);

                if (order == null)
                {
                    TempData["ErrorMessage"] = "Order not found.";
                    return RedirectToAction("OrderList");
                }

                return View(order);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while fetching order details.";
                Console.WriteLine($"Error: {ex.Message}");
                return RedirectToAction("OrderList");
            }
        }
        [HttpGet("customerlist")]
        public IActionResult CustomerList()
        {
            if (!checkSession())
            {
                return RedirectToAction("Login");
            }

            try
            {
                // Lấy danh sách tất cả khách hàng từ cơ sở dữ liệu
                var customers = _context.Customers.ToList();

                ViewBag.CustomerList = customers ?? new List<Customer>();
                ViewBag.CustomerList = customers ?? new List<Customer>();

                if (!customers.Any())
                {
                    ViewBag.ErrorMessage = "No customers found.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.CustomerList = new List<Customer>();
                ViewBag.ErrorMessage = "An error occurred while fetching customers.";
                Console.WriteLine($"Error: {ex.Message}");
            }

            return View();
        }
        // Để deactivate tài khoản
        [HttpPost("deactivate-customer")]
        public IActionResult DeactivateCustomer(int customerId)
        {
            if (!checkSession())
            {
                return RedirectToAction("Login");
            }

            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);
            if (customer == null)
            {
                TempData["ErrorMessage"] = "Customer not found.";
                return RedirectToAction("CustomerList");
            }

            // Đổi trạng thái tài khoản thành "Deactive"
            customer.Status = "Deactive";
            _context.Customers.Update(customer);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Customer account deactivated successfully!";
            return RedirectToAction("CustomerList");
        }

        // Để activate tài khoản
        [HttpPost("activate-customer")]
        public IActionResult ActivateCustomer(int customerId)
        {
            if (!checkSession())
            {
                return RedirectToAction("Login");
            }

            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);
            if (customer == null)
            {
                TempData["ErrorMessage"] = "Customer not found.";
                return RedirectToAction("CustomerList");
            }

            // Đổi trạng thái tài khoản thành "Active"
            customer.Status = "Active";
            _context.Customers.Update(customer);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Customer account activated successfully!";
            return RedirectToAction("CustomerList");
        }


        [HttpGet("adminprofile")]
        public IActionResult AdminProfile()
        {
            if (!checkSession())
            {
                return RedirectToAction("Login");
            }

            try
            {
                // Lấy thông tin nhà hàng từ cơ sở dữ liệu
                string adminJson = HttpContext.Session.GetString("admin");
                var admin = JsonConvert.DeserializeObject<Admin>(adminJson);

                if (admin == null)
                {
                    TempData["ErrorMessage"] = "Admin not found.";
                    return RedirectToAction("Dashboard");
                }

                // Truyền thông tin nhà hàng vào ViewBag
                ViewBag.Admin = admin;
                return View(admin);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while fetching ad profile.";
                Console.WriteLine($"Error: {ex.Message}");
                return RedirectToAction("Dashboard");
            }
        }
        [HttpGet("edit-profile")]
        public IActionResult EditAdminProfile()
        {
            if (!checkSession())
            {
                return RedirectToAction("Login");
            }

            try
            {
                // Lấy thông tin nhà hàng từ cơ sở dữ liệu
                string adminJson = HttpContext.Session.GetString("admin");
                var admin = JsonConvert.DeserializeObject<Admin>(adminJson);

                if (admin == null)
                {
                    TempData["ErrorMessage"] = "Ad not found.";
                    return RedirectToAction("Dashboard");
                }

                return View(admin);  
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while fetching ad profile for editing.";
                Console.WriteLine($"Error: {ex.Message}");
                return RedirectToAction("Dashboard");
            }
        }



        [HttpPost("edit-profile")]
        public IActionResult EditAdminProfile(Admin admin)
        {
            if (!checkSession())
            {
                return RedirectToAction("Login");
            }

            try
            {
                var existingAdmin = _context.Admins.FirstOrDefault(r => r.AdminId == admin.AdminId);

                if (existingAdmin == null)
                {
                    TempData["ErrorMessage"] = "Ad not found.";
                    return RedirectToAction("Dashboard");
                }

                // Cập nhật thông tin nhà hàng
                existingAdmin.FullName = admin.FullName;
                existingAdmin.Email = admin.Email;
                existingAdmin.PhoneNumber = admin.PhoneNumber;
            

                // Cập nhật thông tin vào cơ sở dữ liệu
                _context.Admins.Update(existingAdmin);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Ad profile updated successfully!";
                return RedirectToAction("AdminProfile");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the ad profile.";
                Console.WriteLine($"Error: {ex.Message}");
                return RedirectToAction("Dashboard");
            }
        }
        [HttpGet("apartment/{listingId}/reviews")]
        public IActionResult ReviewList(int listingId)
        {
            if (!checkSession())
            {
                return RedirectToAction("Login");
            }

            var apartment = _context.Apartments
                .Where(a => a.ListingId == listingId)
                .Select(a => new { a.ListingId, a.Title }) // Lấy ID + Tên
                .FirstOrDefault();

            if (apartment == null)
            {
                return NotFound("Apartment not found!");
            }

            var reviews = _context.Reviews
                .Include(r => r.Tenant)
                .Include(r => r.Listing)
                .Where(r => r.ListingId == listingId)
                .ToList();

            ViewBag.ApartmentName = apartment.Title; // Gửi tên sang View
            return View(reviews);
        }

        [HttpGet("reply-review/{id}")]
        public IActionResult ReplyReview(int id)
        {
            if (!checkSession())
            {
                return RedirectToAction("Login");
            }

            var review = _context.Reviews.FirstOrDefault(r => r.ReviewId == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        [HttpPost("reply-review/{id}")]
        public async Task<IActionResult> ReplyReview(int id, [FromForm] string adminReply)
        {
            if (!checkSession())
            {
                return RedirectToAction("Login");
            }

            if (string.IsNullOrWhiteSpace(adminReply))
            {
                TempData["Error"] = "Reply content cannot be empty!";
                return RedirectToAction("ReviewList");
            }

            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            review.AdminReply = adminReply.Trim();
            review.AdminReplyAt = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Reply sent successfully!";

      
            return Redirect($"/admin/apartment/{review.ListingId}/reviews");
        }


        [HttpGet("apartments")]
        public IActionResult ApartmentList()
        {
            if (!checkSession())
            {
                return RedirectToAction("Login");
            }

            var apartments = _context.Apartments.Include(a => a.Category).Include(a => a.Landlord).ToList();
            return View(apartments);
        }



        [HttpGet("apartment/create")]
        public IActionResult CreateApartment()
        {
            if (!checkSession())
            {
                return RedirectToAction("Login");
            }

            ViewBag.Categories = _context.Categories.ToList(); // Load danh mục
            return View();
        }



        [HttpPost("apartment/create")]
        public async Task<IActionResult> CreateApartment(ApartmentDTO apartmentDto)
        {
            if (!checkSession())
            {
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }

                ViewBag.Categories = _context.Categories.ToList();
                return View(apartmentDto);
            }

            var newApartment = new Apartment
            {
                Title = apartmentDto.Title,
                Description = apartmentDto.Description,
                Price = apartmentDto.Price,
                Location = apartmentDto.Location,
                Area = apartmentDto.Area,
                Bedrooms = apartmentDto.Bedrooms,
                Bathrooms = apartmentDto.Bathrooms,
                AvailableFrom = apartmentDto.AvailableFrom,
                CategoryId = apartmentDto.CategoryId,
                Status = apartmentDto.Status ?? "Available", // Nếu không nhập thì mặc định là Available
                Images = apartmentDto.Images
            };

            try
            {
                _context.Apartments.Add(newApartment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Apartment created successfully!";
                return RedirectToAction("ApartmentList");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi lưu database: " + ex.Message);
                ModelState.AddModelError("", "Có lỗi khi lưu vào database.");
                ViewBag.Categories = _context.Categories.ToList();
                return View(apartmentDto);
            }
        }


        [HttpGet("apartment/edit/{listingId}")]
        public IActionResult EditApartment(int listingId)
        {
            if (!checkSession())
            {
                return RedirectToAction("Login");
            }

            var apartment = _context.Apartments.Find(listingId);
            if (apartment == null)
            {
                return NotFound();
            }

            ViewBag.Categories = _context.Categories.ToList(); // Load danh mục
            return View(apartment);
        }


        [HttpPost("apartment/edit/{listingId}")]
        public async Task<IActionResult> EditApartment(int listingId, Apartment updatedApartment)
        {
            if (!checkSession())
            {
                return RedirectToAction("Login");
            }

            var apartment = await _context.Apartments.FindAsync(listingId);
            if (apartment == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                apartment.Title = updatedApartment.Title;
                apartment.Description = updatedApartment.Description;
                apartment.Price = updatedApartment.Price;
                apartment.Location = updatedApartment.Location;
                apartment.Area = updatedApartment.Area;
                apartment.Bedrooms = updatedApartment.Bedrooms;
                apartment.Bathrooms = updatedApartment.Bathrooms;
                apartment.Status = updatedApartment.Status;

                await _context.SaveChangesAsync();
                TempData["Success"] = "Apartment updated successfully!";
                return RedirectToAction("ApartmentList"); // Redirect về danh sách
            }

            ViewBag.Categories = _context.Categories.ToList(); // Load lại danh mục để tránh lỗi null
            return View(updatedApartment);
        }



        [HttpPost("apartment/delete/{listingId}")]
        public async Task<IActionResult> DeleteApartment(int listingId)
        {
            if (!checkSession())
            {
                return RedirectToAction("Login");
            }

            var apartment = await _context.Apartments.FindAsync(listingId);
            if (apartment == null)
            {
                return NotFound();
            }

            _context.Apartments.Remove(apartment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Apartment deleted successfully!";
            return RedirectToAction("ApartmentList");
        }

    }
}
