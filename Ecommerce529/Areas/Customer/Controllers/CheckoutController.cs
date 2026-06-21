using Ecommerce529.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.Checkout;

namespace Ecommerce529.Areas.Customer.Controllers
{
    [Area(CD.CUSTOMER_AREA)]
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Models.Product> _productRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;

        public CheckoutController(IEmailSender emailSender, UserManager<ApplicationUser> userManager, IRepository<Cart> cartRepository, IRepository<Product> productRepository, IRepository<Order> orderRepository, IRepository<OrderItem> orderItemRepository)
        {
            _emailSender = emailSender;
            _userManager = userManager;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
        }

        public async Task<IActionResult> Success(int orderId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();
            var order = await _orderRepository.GetOneAsync(o=>o.Id == orderId);
            if (order is null) return NotFound();

            // send email (payment succeded)
            await _emailSender.SendEmailAsync(
                user.Email,
                "Payment",
                $"<h1>Payment Succeeded</h1>"
            );
            // change orderStatus => Inprogress 
            order.OrderStatus = OrderStatus.InProgress;
            // change PaymentStatus => Completed 
            order.PaymentStatus = PaymentStatus.Completed;
            await _orderRepository.CommitAsync();
            // create orderItems 
            // decrese the quentity of the product 
            // delete the data in the cart 
            var userCarts = await _cartRepository.GetAllAsync(c=>c.ApplicationUserId == user.Id , includes:[c=>c.Product]); 
            foreach(var item in userCarts)
            {
                var orderItem = new OrderItem()
                {
                    OrderId = orderId ,
                    ProductId = item.ProductId , 
                    Price = item.Price ,
                    Count = item.Count ,
                };
                await _orderItemRepository.CreateAsync(orderItem);
                item.Product.Quantity -= item.Count;
                _cartRepository.Delete(item); 
            }
            await _orderItemRepository.CommitAsync();
            // transactionId 
            var service = new SessionService();
            var session = await service.GetAsync(order.SessionId);
            order.TransactionId = session.PaymentIntentId;
            await _orderRepository.CommitAsync(); 

            return View();
        }
        public async Task<IActionResult> Cancel(int orderId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();
            var order = await _orderRepository.GetOneAsync(o => o.Id == orderId);
            if (order is null) return NotFound();
            order.OrderStatus = OrderStatus.Canceled;
            await _orderRepository.CommitAsync(); 

            return View();
        }
    }
}
