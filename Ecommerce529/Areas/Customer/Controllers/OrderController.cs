using Ecommerce529.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce529.Areas.Customer.Controllers
{
    [Authorize]
    [Area(CD.CUSTOMER_AREA)]
    public class OrderController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;

        public OrderController(UserManager<ApplicationUser> userManager, IRepository<Order> orderRepository, IRepository<OrderItem> orderItemRepository)
        {
            _userManager = userManager;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
        }
        public async Task<IActionResult> Index(int page = 1  , string? carrierName = null  )
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound(); 
            var orders  = await _orderRepository.GetAllAsync(o=>o.ApplicationUserId == user.Id);

            if (carrierName is not null )
            {
                orders = orders.Where(o=>o.CarrierName.Contains(carrierName)); 
            }
            int totalPages = (int)Math.Ceiling(orders.Count() / 5.0);
            orders = orders.Skip((page - 1) * 5).Take(5);
            return View(new OrderVM()
            {
                Orders = orders.AsEnumerable(),
                TotalPages = totalPages,
                CurrentPage = page
            });
        }
    }
}
