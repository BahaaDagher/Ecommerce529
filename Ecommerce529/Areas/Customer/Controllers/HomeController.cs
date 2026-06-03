using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Ecommerce529.Areas.Customer.Controllers
{
    [Area(CD.CUSTOMER_AREA)]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context ;
        public HomeController (ApplicationDbContext context)
        {
            _context = context; // = new ApplicationDbContext();
        }
        public IActionResult Index()
        {
            var products = _context.Products.Include(p=>p.Category).AsQueryable();
            // filter 

            // pagination 
            products = products.Skip(0).Take(8); 
            return View(products.AsEnumerable());
        }



        public IActionResult ProductDetails(int id )
        {
            var products = _context.Products.Include(p=>p.Category).AsQueryable();
            var product = products.FirstOrDefault(p=>p.Id == id );

            if (product is null )
            {
                return NotFound();
            }

            var relatedProducts = _context.Products.Where(p=>p.CategoryId == product.CategoryId && p.Id != id)
                                        .Skip(0)
                                        .Take(4); 

            return View(new ProductWithRelatedVM()
            {
                Product = product , 
                RelatedProducts = relatedProducts.AsEnumerable() 
            }); 
        }


        public ViewResult Welcome()
        {
            return View(); 
        }
        public ViewResult PersonlaInfo(int id ) 
        {
            var persons = new List<Person>()
            {
                new Person(){Id= 1  , Name = "Ali" , Salary = 1000  , Address= "Cairo" } ,
                new Person(){Id= 2  , Name = "Sayed", Salary = 2000 , Address= "Giza" } ,
                new Person(){Id= 3  , Name = "mohamed", Salary = 3000 , Address= "Alex" } ,
            };
            //persons = persons.Where(p=> p.Salary >1000).ToList(); 
            persons = persons.Where(p=> p.Id == id).ToList(); 
             var count = persons.Count; 
            return View(new PersonVM()
            {
                Persons = persons  , 
                Count   = count
            });
        }
        public IActionResult Privacy()
        {
            return View();
        }











        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
