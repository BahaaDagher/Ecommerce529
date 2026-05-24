using Ecommerce529.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;

namespace Ecommerce529.Areas.Admin.Controllers
{
    [Area(CD.ADMIN_AREA)]
    public class ProductController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Brand> _brandRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IProductColorRepository _productColorRepository;
        private readonly IProductSubImageRepository _productSubImageRepository;
        private readonly ProductService _productService = new ProductService();

        public ProductController(IRepository<Product> productRepository, IRepository<Brand> brandRepository, IRepository<Category> categoryRepository, IProductColorRepository productColorRepository, IProductSubImageRepository productSubImageRepository)
        {
            _productRepository = productRepository;
            _brandRepository = brandRepository;
            _categoryRepository = categoryRepository;
            _productColorRepository = productColorRepository;
            _productSubImageRepository = productSubImageRepository;
        }

        public async Task<IActionResult> Index(ProductFilterVM productFilterVM,  int page = 1  )
        
        {
            //var products = _context.Products.Include(p=>p.Category).Include(p => p.Brand).AsQueryable(); 
            var products = await _productRepository.GetAllAsync(includes: [p => p.Category, p => p.Brand]); 
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
            //ViewData["Categories"]  = _context.Categories.AsEnumerable();
            ViewData["Categories"]  = await _categoryRepository.GetAllAsync();
            //ViewData["Brands"]  = _context.Brands.AsEnumerable();
            ViewData["Brands"]  = await _brandRepository.GetAllAsync();
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
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var brands = await _brandRepository.GetAllAsync();
            return View(new CreateUpdateProductVM()
            {
                Categories = categories,
                Brands = brands
            });  
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product product , IFormFile ImageFile , List<IFormFile> SubImageFiles , List<string> Colors )
        {
            if(ImageFile != null && ImageFile.Length > 0 )
            {
                var fileName = _productService.SaveFile(ImageFile);
                product.MainImg = fileName; 
            }
            //var savedProduct = _context.Products.Add(product);
            var savedProduct = await _productRepository.CreateAsync(product);
            //_context.SaveChanges(); 
            await _productRepository.CommitAsync(); 

            if(SubImageFiles != null  && SubImageFiles.Count > 0 )
            {
                foreach( var image in SubImageFiles )
                {
                    if (image != null && image.Length > 0)
                    {
                        var fileName = _productService.SaveFile(image , ProductImageType.SubImage);
                        //_context.ProductSubImages.Add( new ProductSubImage()
                        await _productSubImageRepository.CreateAsync( new ProductSubImage()
                        {
                            Img = fileName , 
                            ProductId = savedProduct.Entity.Id 
                        });
                    }
                }
                //_context.SaveChanges();
                await _productSubImageRepository.CommitAsync();
            }
            if (Colors != null && Colors.Count > 0)
            {
                foreach (var color in Colors)
                {
                    //_context.ProductColors.Add(new ProductColor()
                    await _productColorRepository.CreateAsync(new ProductColor()
                    {
                        Color = color,
                        ProductId = savedProduct.Entity.Id
                    });
                }
                //_context.SaveChanges();
                await _productColorRepository.CommitAsync(); 
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id )
        {
            //var product = _context.Products.FirstOrDefault(c=>c.Id == id); 
            var product = await _productRepository.GetOneAsync(c => c.Id == id);  
            if (product is null )
            {
                return NotFound(); 
            }
            var categories = await _categoryRepository.GetAllAsync();
            var brands = await _brandRepository.GetAllAsync();
            return View(new CreateUpdateProductVM()
            {
                Product = product,
                Categories = categories,
                Brands = brands,
                //ProductSubImages = _context.ProductSubImages.Where(ps=>ps.ProductId == id),
                ProductSubImages = await _productSubImageRepository.GetAllAsync(ps => ps.ProductId == id),
                //ProductColors = _context.ProductColors.Where(pc=>pc.ProductId == id)
                ProductColors = await _productColorRepository.GetAllAsync(pc => pc.ProductId == id)
            });
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Product product , IFormFile ImageFile ,  List<IFormFile> SubImageFiles, List<string> Colors)
        {
            //var productInDb = _context.Products.AsNoTracking().FirstOrDefault(b => b.Id == product.Id);
            var productInDb = await _productRepository.GetOneAsync(b => b.Id == product.Id , IsTracking:false);

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
            //_context.Products.Update(product);
            _productRepository.Update(product);
            //_context.SaveChanges();
            await _productRepository.CommitAsync(); 

            if (SubImageFiles != null && SubImageFiles.Count > 0)
            {
                //var oldProductSubImages = _context.ProductSubImages.Where(ps=>ps.ProductId == product.Id);
                var oldProductSubImages = await _productSubImageRepository.GetAllAsync(ps => ps.ProductId == product.Id); 
                // delete from Database 
                //_context.ProductSubImages.RemoveRange(oldProductSubImages);
                _productSubImageRepository.DeleteRange(oldProductSubImages);
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
                        //_context.ProductSubImages.Add(new ProductSubImage()
                        await _productSubImageRepository.CreateAsync(new ProductSubImage()
                        {
                            Img = fileName,
                            ProductId = product.Id
                        });
                    }
                }
                //_context.SaveChanges();
                await _productSubImageRepository.CommitAsync();
            }
            if (Colors != null && Colors.Count > 0)
            {
                //var oldProductColors = _context.ProductColors.Where(pc=>pc.ProductId == product.Id);
                var oldProductColors = await _productColorRepository.GetAllAsync(pc => pc.ProductId == product.Id);
                //_context.ProductColors.RemoveRange(oldProductColors); 
                _productColorRepository.DeleteRange(oldProductColors); 

                foreach (var color in Colors)
                {
                    //_context.ProductColors.Add(new ProductColor()
                    await _productColorRepository.CreateAsync(new ProductColor()
                    {
                        Color = color,
                        ProductId = product.Id
                    });
                }
                //_context.SaveChanges();
                await _productColorRepository.CommitAsync();
            }
            return RedirectToAction(nameof(Index));  
        }
        public async Task<IActionResult> Delete(int id)
        {
            //var product = _context.Products.FirstOrDefault(c => c.Id == id);
            var product = await _productRepository.GetOneAsync(c => c.Id == id);
            if (product is null)
            {
                return NotFound();
            }
            _productService.RemoveFile(product.MainImg);
            //_context.Products.Remove(product);
            _productRepository.Delete(product);
            //var productSubImages = _context.ProductSubImages.Where(ps => ps.ProductId == product.Id);  
            var productSubImages = await _productSubImageRepository.GetAllAsync(ps => ps.ProductId == product.Id);  
            foreach(var item in productSubImages)
            {
                _productService.RemoveFile(item.Img , ProductImageType.SubImage);  
            }

            //_context.SaveChanges();
            await _productRepository.CommitAsync();
            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> DeleteSubImage(int id)
        {
            //var productSubImage = _context.ProductSubImages.FirstOrDefault(e=>e.Id == id);
            var productSubImage = await _productSubImageRepository.GetOneAsync(e => e.Id == id); 
            if (productSubImage is null) return NotFound();
            //_context.ProductSubImages.Remove(productSubImage);
            _productSubImageRepository.Delete(productSubImage);
            _productService.RemoveFile(productSubImage.Img , ProductImageType.SubImage);
            //_context.SaveChanges();
            await _productSubImageRepository.CommitAsync();
            return RedirectToAction(nameof(Index));

        }


    }
}
