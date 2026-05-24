using Ecommerce529.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce529.Areas.Admin.Controllers
{
    [Area(CD.ADMIN_AREA)]
    public class BrandController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly Repository<Brand> _brandRepository;
        private readonly BrandService _brandService;

        public BrandController()
        {
            //_context = new ApplicationDbContext();
            _brandRepository = new Repository<Brand>(); 
            _brandService = new BrandService() ;
        }

        public async Task<IActionResult> Index(string brandName , int page = 1  )
        {
            //var brands = _context.Brands.AsQueryable(); 
            var brands =await  _brandRepository.GetAllAsync() ; 
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
            return View(new CreateBrandVM()); 
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateBrandVM createBrandVM)
        {
            if (!ModelState.IsValid)
            {
                return View(createBrandVM);
            }
            var brand = new Brand()
            {
                Name = createBrandVM.Name,
                Description = createBrandVM.Description,
                Status = createBrandVM.Status,
                
            };  
            if(createBrandVM.ImageFile != null && createBrandVM.ImageFile.Length > 0 )
            {
                var fileName = _brandService.SaveFile(createBrandVM.ImageFile);
                brand.Logo = fileName; 
            }
            //_context.Brands.Add(brand);
            await _brandRepository.CreateAsync(brand) ;
            //_context.SaveChanges(); 
            await _brandRepository.CommitAsync() ; 
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id )
        {
            //var brand = _context.Brands.FirstOrDefault(c=>c.Id == id); 
            var brand = await _brandRepository.GetOneAsync(c => c.Id == id);  
            if (brand is null )
            {
                return NotFound(); 
            }
            return View(brand);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Brand brand , IFormFile ImageFile)
        {
            //var brandInDb = _context.Brands.AsNoTracking().FirstOrDefault(b => b.Id == brand.Id);
            var brandInDb = await _brandRepository.GetOneAsync(b => b.Id == brand.Id , IsTracking: false);


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
            //_context.Brands.Update(brand);
            _brandRepository.Update(brand); 
            //_context.SaveChanges();
            await _brandRepository.CommitAsync();
            return RedirectToAction(nameof(Index));  
        }
        public async Task<IActionResult> Delete(int id)
        {
            //var brand = _context.Brands.FirstOrDefault(c => c.Id == id);
            var brand = await _brandRepository.GetOneAsync(c => c.Id == id);
            if (brand is null)
            {
                return NotFound();
            }
            _brandService.RemoveFile(brand.Logo);
            //_context.Brands.Remove(brand);
            _brandRepository.Delete(brand);
            //_context.SaveChanges();
            await _brandRepository.CommitAsync();
            return RedirectToAction(nameof(Index));

        }


    }
}
