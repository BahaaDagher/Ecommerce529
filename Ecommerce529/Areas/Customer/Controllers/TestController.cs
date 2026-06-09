using Ecommerce529.LifeTime.InterfaceLifeTime;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce529.Areas.Customer.Controllers
{
    [Area(CD.CUSTOMER_AREA)]
    public class TestController : Controller
    {
        private readonly ITransientInterface _transientInterface1; 
        private readonly ITransientInterface _transientInterface2; 
        private readonly IScopedInterface _scopedInterface1; 
        private readonly IScopedInterface _scopedInterface2;
        private readonly ISingletonInterface _singletonInterface1; 
        private readonly ISingletonInterface _singletonInterface2;

        public TestController(ITransientInterface transientInterface1, ITransientInterface transientInterface2, IScopedInterface scopedInterface1, IScopedInterface scopedInterface2, ISingletonInterface singletonInterface1, ISingletonInterface singletonInterface2)
        {
            _transientInterface1 = transientInterface1;
            _transientInterface2 = transientInterface2;
            _scopedInterface1 = scopedInterface1;
            _scopedInterface2 = scopedInterface2;
            _singletonInterface1 = singletonInterface1;
            _singletonInterface2 = singletonInterface2;
        }

        public IActionResult Index()
        {
            return Json(new
            {
                transientInterface1 = _transientInterface1.Id,
                transientInterface2 = _transientInterface2.Id,

                scopedInterface1 = _scopedInterface1.Id,
                scopedInterface2 = _scopedInterface2.Id,

                singletonInterface1 = _singletonInterface1.Id,
                singletonInterface2 = _singletonInterface2.Id,
            }); 
        }

    }
}
