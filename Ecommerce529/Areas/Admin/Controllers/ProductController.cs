using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce529.Areas.Admin.Controllers
{
    [Area(CD.ADMIN_AREA)]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ProductService _productService;

        public ProductController()
        {
            _context = new ApplicationDbContext();
            _productService = new ProductService() ;
        }

        public IActionResult Index(ProductFilterVM productFilterVM,  int page = 1  )
        
        {
            var products = _context.Products.Include(p=>p.Category).Include(p => p.Brand).AsQueryable(); 
            //filter 
            if (productFilterVM.ProductName != null)
            {
                products = products.Where(c=>c.Name.Contains(productFilterVM.ProductName.Trim())); 
                ViewBag.ProductName = productFilterVM.ProductName;
            }
            if (productFilterVM.MinPrice >0)
            {
                products = products.Where(c => c.Price >= productFilterVM.MinPrice);
                ViewBag.MinPrice = productFilterVM.MinPrice;
            }
            if (productFilterVM.MaxPrice> 0)
            {
                products = products.Where(c => c.Price <=productFilterVM.MaxPrice);
                ViewBag.MaxPrice = productFilterVM.MaxPrice;
            }
            if (productFilterVM.CategoryId > 0)
            {
                products = products.Where(c => c.CategoryId  == productFilterVM.CategoryId);
                ViewBag.CategoryId = productFilterVM.CategoryId;
            }
            if (productFilterVM.BrandId > 0)
            {
                products = products.Where(c => c.BrandId == productFilterVM.BrandId);
                ViewBag.BrandId = productFilterVM.BrandId;
            }
            if (productFilterVM.IsLowQuantity)
            {
                products = products.OrderBy(p=>p.Quantity); 
                ViewBag.IsLowQuantity = productFilterVM.IsLowQuantity;
            }

            // pagination 
            int totalPages = (int)Math.Ceiling(products.Count() / 5.0); 
            products = products.Skip((page-1) * 5).Take(5); 
            return View(new ProductVM()
            {
                Products = products.AsEnumerable()  , 
                TotalPages = totalPages ,
                CurrentPage = page 
            });
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(); 
        }
        [HttpPost]
        public IActionResult Create(Product product , IFormFile ImageFile)
        {
            if(ImageFile != null && ImageFile.Length > 0 )
            {
                var fileName = _productService.SaveFile(ImageFile);
                product.MainImg = fileName; 
            }
            _context.Products.Add(product);
            _context.SaveChanges(); 
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Edit(int id )
        {
            var product = _context.Products.FirstOrDefault(c=>c.Id == id); 
            if (product is null )
            {
                return NotFound(); 
            }
            return View(product);
        }
        [HttpPost]
        public IActionResult Edit(Product product , IFormFile ImageFile)
        {
            var productInDb = _context.Products.AsNoTracking().FirstOrDefault(b => b.Id == product.Id);


            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = _productService.SaveFile(ImageFile); 
                product.MainImg = fileName;
                _productService.RemoveFile(productInDb.MainImg);
            }
            else
            {
                product.MainImg = productInDb.MainImg;
            }
            _context.Products.Update(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));  
        }
        public IActionResult Delete(int id)
        {
            var product = _context.Products.FirstOrDefault(c => c.Id == id);
            if (product is null)
            {
                return NotFound();
            }
            _productService.RemoveFile(product.MainImg);
            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));

        }


    }
}
