using Ecommerce529.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ecommerce529.Areas.Customer.Controllers
{
    [Area(CD.CUSTOMER_AREA)]
    [Authorize]
    public class CartController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Promotion> _promotionRepository;

        public CartController(UserManager<ApplicationUser> userManager, IRepository<Cart> cartRepository, IRepository<Product> productRepository, IRepository<Promotion> promotionRepository)
        {
            _userManager = userManager;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _promotionRepository = promotionRepository;
        }

        public async Task<IActionResult> Index(string? code)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();
            if(code != null)
            {
                var promotion = await  _promotionRepository.GetOneAsync(p=>
                    p.Code == code && 
                    p.IsValid && 
                    p.ValidTo > DateTime.UtcNow && 
                    p.MaxUsage>0
                ); 
                if (promotion != null)
                {
                    var cartInDb = await _cartRepository.GetOneAsync(c=>c.ApplicationUserId == user.Id && c.ProductId == promotion.ProductId);  
                    if (cartInDb != null)
                    {
                        cartInDb.Price -= cartInDb.Price * promotion.Discount / 100;
                        promotion.MaxUsage--; 
                        await _cartRepository.CommitAsync();
                        TempData["Success_Notification"] = "promotion Applied Successfully";
                    }
                    else
                    {
                        TempData["Error_Notification"] = "there is no product Associeted with this promotion";
                    }
                }
                else
                {
                    TempData["Error_Notification"] = "invalid / expired promotion";
                }
            }
            var carts = await _cartRepository.GetAllAsync(c=>c.ApplicationUserId == user.Id , includes: [c=>c.Product]);
            return View(carts);
        }
        public async Task<IActionResult> AddToCart(int productId , int count)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();
            var product = await _productRepository.GetOneAsync(p=>p.Id == productId);
            if (product is null) return NotFound();
            var cartInDb = await _cartRepository.GetOneAsync(c=>c.ProductId == productId && c.ApplicationUserId == user.Id ); 
            if (cartInDb != null )
            {
                cartInDb.Count += count;
                await _cartRepository.CommitAsync(); 
            }
            else
            {
                var cart = new Cart()
                {
                    ProductId = productId , 
                    ApplicationUserId = user.Id  , 
                    Count = count  , 
                    Price = product.Price  - (product.Price * product.Discount / 100 )  , 
                };
                await _cartRepository.CreateAsync(cart);
                await _cartRepository.CommitAsync(); 
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> DecrementCount(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();
            var cartInDb = await  _cartRepository.GetOneAsync(c => c.ProductId == productId && c.ApplicationUserId == user.Id);
            if(cartInDb is null ) return NotFound();
            if (cartInDb.Count > 1) { 
                cartInDb.Count--;
            }
            await _cartRepository.CommitAsync();
            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> IncrementCount(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();
            var cartInDb = await _cartRepository.GetOneAsync(c => c.ProductId == productId && c.ApplicationUserId == user.Id);
            if (cartInDb is null) return NotFound();
            var product = await _productRepository.GetOneAsync(p=>p.Id == productId); 
            if(cartInDb.Count < product.Quantity )
                cartInDb.Count++;
            await _cartRepository.CommitAsync();
            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> DeleteProductCart(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();
            var cartInDb = await _cartRepository.GetOneAsync(c => c.ProductId == productId && c.ApplicationUserId == user.Id);
            if (cartInDb is null) return NotFound();
            _cartRepository.Delete(cartInDb);
            await _cartRepository.CommitAsync();
            return RedirectToAction(nameof(Index));

        }

    }
}
