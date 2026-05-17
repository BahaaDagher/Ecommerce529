using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;

namespace Ecommerce529.Areas.Admin.Controllers
{
    [Area(CD.ADMIN_AREA)]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController()
        {
            _context = new ApplicationDbContext();
        }

        public IActionResult Index(string categoryName , int page = 1  )
        {
            var categories = _context.Categories.AsQueryable(); 
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
        public IActionResult Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            _context.Categories.Add(category);  
            _context.SaveChanges();
            //Response.Cookies.Append("Success_Notification" , "Category Careted Successfully");
            TempData["Success_Notification"] = "Category Careted Successfully";  
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Edit(int id )
        {
            var category = _context.Categories.FirstOrDefault(c=>c.Id == id); 
            if (category is null )
            {
                return NotFound(); 
            }
            return View(category);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);  
            }
            _context.Categories.Update(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));  
        }
        public IActionResult Delete(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category is null)
            {
                return NotFound();
            }
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));

        }
    }
}
