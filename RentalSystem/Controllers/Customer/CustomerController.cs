using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RentalSystem.Models;
using RentalSystem.DTO;
using System.Security.Claims;

namespace RentalSystem.Controllers.customer
{
    [Route("customer")]
    public class CustomerController : Controller
    {
        private readonly RentalSystemContext _context;
        private readonly IWebHostEnvironment _environment;
        public CustomerController(RentalSystemContext context, IWebHostEnvironment environment)
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
                string isCustomerAuthenticated = httpContext.Session.GetString("customer");
                if (string.IsNullOrEmpty(isCustomerAuthenticated))
                {
                    checkS = false;
                }
            }

            return checkS;
        }
        [HttpGet("home")]
        public IActionResult Home()
        {
            ViewBag.IsLogin = checkSession();

            // ✅ Lấy thông tin Customer từ Session nếu đã đăng nhập
            string customerJson = HttpContext.Session.GetString("customer");
            if (!string.IsNullOrEmpty(customerJson))
            {
                ViewBag.Customer = JsonConvert.DeserializeObject<Customer>(customerJson);
            }
            else
            {
                ViewBag.Customer = null;
            }
            // Lấy danh sách các căn hộ và các hình ảnh của nó từ cơ sở dữ liệu
            var aparts = _context.Apartments
                                 .Include(a => a.ImagesNavigation)  // Đảm bảo 'Images' là navigation property trong model
                                 .Include(a => a.Category) // Bao gồm cả thể loại căn hộ
                                 .ToList();

            return View(aparts);
        }


        [HttpGet("logout")]
        public IActionResult Logout()
        {
            ViewBag.IsLogin = checkSession();
            var session = HttpContext.Session;

            if (session.Keys.Contains("customer"))
            {
                session.Remove("customer");
            }
            return RedirectToAction("Login");
        }

        [HttpGet("authenticate/login")]
        public IActionResult Login()
        {
            ViewBag.IsLogin = checkSession();
            return View();
        }


        [HttpPost("authenticate/login")]
        public IActionResult Login(string username, string password)
        {
            ViewBag.IsLogin = checkSession();
            if (_context != null)
            {
                if (username == string.Empty || password == string.Empty)
                {
                    ViewBag.Message = "Tên đăng nhập hoặc mật khẩu không được bỏ trống!";
                    return View();
                }
                else
                {
                    Customer customer = _context.Customers.FirstOrDefault(x => x.FullName.Equals(username) && x.PasswordHash.Equals(password));

                    if (customer != null)
                    {
                        if (customer.Status.Equals("Deactive"))
                        {
                            ViewBag.Message = "Tài khoản đang bị vô hiệu hóa.";
                            return View();
                        }
                        string customerSession = JsonConvert.SerializeObject(customer);
                        HttpContext.Session.SetString("customer", customerSession);
                        return RedirectToAction("Home");
                    }
                    else
                    {
                        ViewBag.Message = "Tên đăng nhập hoặc mật khẩu sai! vui lòng thử lại";
                        return View();
                    }
                }
            }
            return View();
        }

        [HttpGet("authenticate/register")]
        public IActionResult Register()
        {
            ViewBag.IsLogin = checkSession();
            return View();
        }


        [HttpPost("authenticate/register")]
        public IActionResult Register(string name, string phone, string email, DateTime dob, string password, string repassword)
        {
            ViewBag.IsLogin = checkSession();
            bool checkInput = true;

            if (!Utils.Validation.IsUsernameUnique(name, _context))
            {
                ViewBag.UsernameError = "Tên đăng nhập đã tồn tại. Vui lòng chọn tên khác.";
                checkInput = false;
            }

            if (!Utils.Validation.IsEmailValid(email))
            {
                ViewBag.EmailError = "Định dạng email không hợp lệ.";
                checkInput = false;
            }

            if (!Utils.Validation.IsPasswordValid(password))
            {
                ViewBag.PasswordError = "Mật khẩu phải có ít nhất 6 ký tự, bao gồm chữ thường, chữ hoa và một số.";
                checkInput = false;
            }

            if (password != repassword)
            {
                ViewBag.RepasswordError = "Mật khẩu và nhập lại mật khẩu không khớp.";
                checkInput = false;
            }
            if (checkInput == true)
            {

                Customer customer = new Customer()
                {
                    FullName = name,
                    PhoneNumber = phone,
                    Email = email,       
                    PasswordHash = password,
                    Status = "Active"
                };

                _context.Customers.Add(customer);
                _context.SaveChanges();
              
                return RedirectToAction("Login");
            }
            return View();
        }

        // PROFILE VIEW
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            ViewBag.IsLogin = checkSession();
            if (checkSession() == false)
            {
                return RedirectToAction("Login");
            }
            int id = 0;
            string customerJson = HttpContext.Session.GetString("customer");
            if (!string.IsNullOrEmpty(customerJson))
            {
                Customer customerSession = JsonConvert.DeserializeObject<Customer>(customerJson);
                id = customerSession.CustomerId;
            }
            Customer customer = _context.Customers.FirstOrDefault(x => x.CustomerId == id);
            return View(customer);
        }

        [HttpGet("profile/edit")]
        public IActionResult EditProfile()
        {
            ViewBag.IsLogin = checkSession();
            if (checkSession() == false)
            {
                return RedirectToAction("Login");
            }
            int id = 0;
            string customerJson = HttpContext.Session.GetString("customer");
            if (!string.IsNullOrEmpty(customerJson))
            {
                Customer customerSession = JsonConvert.DeserializeObject<Customer>(customerJson);
                id = customerSession.CustomerId;
            }
            Customer customer = _context.Customers.FirstOrDefault(x => x.CustomerId == id);
            return View(customer);
        }

        [HttpPost("profile/edit")]
        public async Task<IActionResult> EditProfile(int id, string name, DateTime dob, string gender, string phone, string email, IFormFile fileUpload)
        {
            ViewBag.IsLogin = checkSession();
            if (checkSession() == false)
            {
                return RedirectToAction("Login");
            }
            Customer customer = _context.Customers.FirstOrDefault(x => x.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }

            customer.FullName = name;
    
            customer.PhoneNumber = phone;
            customer.Email = email;
            _context.Customers.Update(customer);
            _context.SaveChanges();
            return RedirectToAction("Profile");
        }
        [HttpGet("apartment")]
        public IActionResult Apartment()
        {
            ViewBag.IsLogin = checkSession();

            // ✅ Lấy thông tin Customer từ Session nếu đã đăng nhập
            string customerJson = HttpContext.Session.GetString("customer");
            if (!string.IsNullOrEmpty(customerJson))
            {
                ViewBag.Customer = JsonConvert.DeserializeObject<Customer>(customerJson);
            }
            else
            {
                ViewBag.Customer = null;
            }
            var apartments = _context.Apartments
                                     .Include(a => a.ImagesNavigation)  
                                     .Include(a => a.Category) 
                                     .ToList();

            var categories = _context.Categories.ToList();

            var viewModel = new ApartmentCategoryDTO
            {
                Apartments = apartments,
                Categories = categories
            };

            return View(viewModel);
        }

        [HttpGet("apartmentdetail/{id}")]
        public IActionResult ApartmentDetail(int id)
        {
            ViewBag.IsLogin = checkSession();

            string customerJson = HttpContext.Session.GetString("customer");
            if (!string.IsNullOrEmpty(customerJson))
            {
                ViewBag.Customer = JsonConvert.DeserializeObject<Customer>(customerJson);
            }
            else
            {
                ViewBag.Customer = null;
            }

            var apartment = _context.Apartments
                                    .Include(a => a.ImagesNavigation)
                                    .Include(a => a.Category)
                                    .FirstOrDefault(a => a.ListingId == id);

            if (apartment == null)
            {
                return NotFound();
            }

            var relatedApartments = _context.Apartments
                                            .Include(a => a.ImagesNavigation)
                                            .Where(a => a.CategoryId == apartment.CategoryId && a.ListingId != id)
                                            .ToList();

            var reviews = _context.Reviews
                                  .Where(r => r.ListingId == id)
                                  .Include(r => r.Tenant) 
                                  .ToList();

            var viewModel = new ApartmentDetailDTO
            {
                Apartment = apartment,
                RelatedApartments = relatedApartments,
                Reviews = reviews 
            };

            return View(viewModel);
        }

        [HttpGet("search")]
        public IActionResult Search(string? title, decimal? exactPrice, decimal? minPrice, decimal? maxPrice, int? categoryId, string? location)
        {
            ViewBag.IsLogin = checkSession();

            var query = _context.Apartments
                                .Include(a => a.ImagesNavigation)
                                .Include(a => a.Category)
                                .AsQueryable();

            // Tìm theo tiêu đề
            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(a => a.Title.Contains(title));
            }

            // Tìm theo địa điểm
            if (!string.IsNullOrEmpty(location))
            {
                query = query.Where(a => a.Location.Contains(location));
            }

            // Tìm theo giá chính xác
            if (exactPrice.HasValue)
            {
                query = query.Where(a => a.Price == exactPrice.Value);
            }

            // Tìm theo khoảng giá
            if (minPrice.HasValue && maxPrice.HasValue)
            {
                query = query.Where(a => a.Price >= minPrice.Value && a.Price <= maxPrice.Value);
            }

            // Tìm theo danh mục
            if (categoryId.HasValue)
            {
                query = query.Where(a => a.CategoryId == categoryId.Value);
            }

            var apartments = query.ToList();
            var categories = _context.Categories.ToList();

            var viewModel = new ApartmentCategoryDTO
            {
                Apartments = apartments,
                Categories = categories
            };

            return View("Apartment", viewModel);
        }
        [HttpGet("search")]
        public IActionResult Search(string? keyword)
        {
            ViewBag.IsLogin = checkSession();

            var query = _context.Apartments
                                .Include(a => a.ImagesNavigation)
                                .Include(a => a.Category)
                                .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(a =>
                    a.Title.Contains(keyword) ||
                    a.Location.Contains(keyword) ||
                    a.Category.CategoryName.Contains(keyword)
                );
            }

            var apartments = query.ToList();
            var categories = _context.Categories.ToList();

            var viewModel = new ApartmentCategoryDTO
            {
                Apartments = apartments,
                Categories = categories
            };

            return View("Apartment", viewModel); // Trả kết quả về trang Apartment.cshtml
        }

        [HttpPost]
        public IActionResult Create(int listingId)
        {
            // Kiểm tra session
            string customerJson = HttpContext.Session.GetString("customer");
            if (string.IsNullOrEmpty(customerJson))
            {
                TempData["ErrorMessage"] = "Your session has expired. Please log in again.";
                return RedirectToAction("Login");
            }

            var customer = JsonConvert.DeserializeObject<Customer>(customerJson);

            // Kiểm tra xem apartment có tồn tại không
            var apartment = _context.Apartments.Find(listingId);
            if (apartment == null)
            {
                TempData["Error"] = "Apartment not found.";
                return RedirectToAction("ApartmentDetail", new { id = listingId });
            }
            // Tạo booking
            var booking = new Booking
            {
                TenantId = customer.CustomerId,
                ListingId = listingId,
                BookingDate = DateOnly.FromDateTime(DateTime.UtcNow),
                Status = "Wait",
                CreatedAt = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);
            int affectedRows = _context.SaveChanges();

            if (affectedRows > 0)
            {
                TempData["Success"] = "Booking successfully created!";
            }
            else
            {
                TempData["Error"] = "Failed to create booking.";
            }

            return RedirectToAction("ApartmentDetail", new { id = listingId });
        }
        [HttpPost("create-review")]
        public async Task<IActionResult> CreateReview(int listingId, int rating, string comment)
        {
            if (!checkSession())
            {
                TempData["Error"] = "You need to login to post a review.";
                return RedirectToAction("ApartmentDetail", new { id = listingId });
            }

            var customerJson = HttpContext.Session.GetString("customer");
            var customer = JsonConvert.DeserializeObject<Customer>(customerJson);

            var review = new Review
            {
                TenantId = customer.CustomerId,
                ListingId = listingId,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.Now
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Review added successfully!";
            return RedirectToAction("ApartmentDetail", new { id = listingId });
        }

       

    }
}
