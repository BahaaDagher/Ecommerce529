using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce529.Areas.Admin.Controllers
{
    [Area(CD.ADMIN_AREA)]
    public class BrandController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BrandService _brandService;

        public BrandController()
        {
            _context = new ApplicationDbContext();
            _brandService = new BrandService() ;
        }

        public IActionResult Index(string brandName , int page = 1  )
        {
            var brands = _context.Brands.AsQueryable(); 
            //filter 
            if (brandName != null)
            {
                brands = brands.Where(c=>c.Name.Contains(brandName)); 
                ViewBag.BrandName = brandName;
            }

            // pagination 
            int totalPages = (int)Math.Ceiling(brands.Count() / 5.0); 
            brands = brands.Skip((page-1) * 5).Take(5); 
            return View(new BrandVM()
            {
                Brands = brands.AsEnumerable()  , 
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
        public IActionResult Create(Brand brand , IFormFile ImageFile)
        {
            if(ImageFile != null && ImageFile.Length > 0 )
            {
                var fileName = _brandService.SaveFile(ImageFile);
                brand.Logo = fileName; 
            }
            _context.Brands.Add(brand);
            _context.SaveChanges(); 
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Edit(int id )
        {
            var brand = _context.Brands.FirstOrDefault(c=>c.Id == id); 
            if (brand is null )
            {
                return NotFound(); 
            }
            return View(brand);
        }
        [HttpPost]
        public IActionResult Edit(Brand brand , IFormFile ImageFile)
        {
            var brandInDb = _context.Brands.AsNoTracking().FirstOrDefault(b => b.Id == brand.Id);


            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = _brandService.SaveFile(ImageFile); 
                brand.Logo = fileName;
                _brandService.RemoveFile(brandInDb.Logo);
            }
            else
            {
                brand.Logo = brandInDb.Logo;
            }
            _context.Brands.Update(brand);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));  
        }
        public IActionResult Delete(int id)
        {
            var brand = _context.Brands.FirstOrDefault(c => c.Id == id);
            if (brand is null)
            {
                return NotFound();
            }
            _brandService.RemoveFile(brand.Logo);
            _context.Brands.Remove(brand);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));

        }


    }
}
