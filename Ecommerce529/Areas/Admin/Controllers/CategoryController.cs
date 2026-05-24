using Ecommerce529.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;

namespace Ecommerce529.Areas.Admin.Controllers
{
    [Area(CD.ADMIN_AREA)]
    public class CategoryController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly Repository<Category> _categoryRepository;

        public CategoryController()
        {
            //_context = new ApplicationDbContext();
            _categoryRepository = new Repository<Category>();
        }

        public async Task<IActionResult> Index(string categoryName , int page = 1  )
        {
            //var categories = _context.Categories.AsQueryable(); 
            var categories = await _categoryRepository.GetAllAsync(); 
            //filter 
            if (categoryName != null)
            {
                categories = categories.Where(c=>c.Name.Contains(categoryName)); 
                ViewBag.CategoryName = categoryName;
            }

            // pagination 
            int totalPages = (int)Math.Ceiling(categories.Count() / 5.0); 
            categories = categories.Skip((page-1) * 5).Take(5); 
            return View(new CategoryVM()
            {
                Categories = categories.AsEnumerable()  , 
                TotalPages = totalPages ,
                CurrentPage = page 
            });
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Category()); 
        }
        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            //_context.Categories.Add(category);
            await _categoryRepository.CreateAsync(category);
            //_context.SaveChanges();
            await _categoryRepository.CommitAsync();
            //Response.Cookies.Append("Success_Notification" , "Category Careted Successfully");
            TempData["Success_Notification"] = "Category Careted Successfully";  
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id )
        {
            //var category = _context.Categories.FirstOrDefault(c=>c.Id == id); 
            var category = await _categoryRepository.GetOneAsync(c => c.Id == id); 
            if (category is null )
            {
                return NotFound(); 
            }
            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);  
            }
            //_context.Categories.Update(category);
            _categoryRepository.Update(category);
            //_context.SaveChanges();
            await _categoryRepository.CommitAsync();
            return RedirectToAction(nameof(Index));  
        }
        public async Task<IActionResult> Delete(int id)
        {
            //var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            var category = await _categoryRepository.GetOneAsync(c => c.Id == id);

            if (category is null)
            {
                return NotFound();
            }
            _categoryRepository.Delete(category); 
            //_context.SaveChanges();
            await _categoryRepository.CommitAsync();
            return RedirectToAction(nameof(Index));

        }
    }
}
