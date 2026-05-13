using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;

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
            ViewData["Categories"]  = _context.Categories.AsEnumerable();
            ViewData["Brands"]  = _context.Brands.AsEnumerable();
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
            var categories = _context.Categories.AsEnumerable();
            var brands = _context.Brands.AsEnumerable();
            return View(new CreateUpdateProductVM()
            {
                Categories = categories,
                Brands = brands
            });  
        }
        [HttpPost]
        public IActionResult Create(Product product , IFormFile ImageFile , List<IFormFile> SubImageFiles , List<string> Colors )
        {
            if(ImageFile != null && ImageFile.Length > 0 )
            {
                var fileName = _productService.SaveFile(ImageFile);
                product.MainImg = fileName; 
            }
            var savedProduct = _context.Products.Add(product);
            _context.SaveChanges(); 

            if(SubImageFiles != null  && SubImageFiles.Count > 0 )
            {
                foreach( var image in SubImageFiles )
                {
                    if (image != null && image.Length > 0)
                    {
                        var fileName = _productService.SaveFile(image , ProductImageType.SubImage);
                        _context.ProductSubImages.Add( new ProductSubImage()
                        {
                            Img = fileName , 
                            ProductId = savedProduct.Entity.Id 
                        });
                    }
                }
                _context.SaveChanges();
            }
            if (Colors != null && Colors.Count > 0)
            {
                foreach (var color in Colors)
                {
                    _context.ProductColors.Add(new ProductColor()
                    {
                        Color = color,
                        ProductId = savedProduct.Entity.Id
                    });
                }
                _context.SaveChanges();
            }
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
            var categories = _context.Categories.AsEnumerable();
            var brands = _context.Brands.AsEnumerable();
            return View(new CreateUpdateProductVM()
            {
                Product = product,
                Categories = categories,
                Brands = brands,
                ProductSubImages = _context.ProductSubImages.Where(ps=>ps.ProductId == id),
                ProductColors = _context.ProductColors.Where(pc=>pc.ProductId == id)
            });
        }
        [HttpPost]
        public IActionResult Edit(Product product , IFormFile ImageFile ,  List<IFormFile> SubImageFiles, List<string> Colors)
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

            if (SubImageFiles != null && SubImageFiles.Count > 0)
            {
                var oldProductSubImages = _context.ProductSubImages.Where(ps=>ps.ProductId == product.Id);
                // delete from Database 
                _context.ProductSubImages.RemoveRange(oldProductSubImages);
                // delete from  wwwroot
                foreach (var item in oldProductSubImages)
                {
                    _productService.RemoveFile(item.Img , ProductImageType.SubImage); 
                }
                foreach (var image in SubImageFiles)
                {
                    if (image != null && image.Length > 0)
                    {
                        // insert iton wwwroot 
                        var fileName = _productService.SaveFile(image, ProductImageType.SubImage);
                        // insert ito database 
                        _context.ProductSubImages.Add(new ProductSubImage()
                        {
                            Img = fileName,
                            ProductId = product.Id
                        });
                    }
                }
                _context.SaveChanges();
            }
            if (Colors != null && Colors.Count > 0)
            {
                var oldProductColors = _context.ProductColors.Where(pc=>pc.ProductId == product.Id);
                _context.ProductColors.RemoveRange(oldProductColors); 

                foreach (var color in Colors)
                {
                    _context.ProductColors.Add(new ProductColor()
                    {
                        Color = color,
                        ProductId = product.Id
                    });
                }
                _context.SaveChanges();
            }
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
            var productSubImages = _context.ProductSubImages.Where(ps => ps.ProductId == product.Id);  
            foreach(var item in productSubImages)
            {
                _productService.RemoveFile(item.Img , ProductImageType.SubImage);  
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));

        }
        public IActionResult DeleteSubImage(int id)
        {
            var productSubImage = _context.ProductSubImages.FirstOrDefault(e=>e.Id == id);
            if (productSubImage is null) return NotFound();
            _context.ProductSubImages.Remove(productSubImage);
            _productService.RemoveFile(productSubImage.Img , ProductImageType.SubImage);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));

        }


    }
}
