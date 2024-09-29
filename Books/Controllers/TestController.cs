using Books.Models;
using Microsoft.AspNetCore.Mvc;
namespace Books.Controllers
{
    public class TestController : Controller
    {
        public ViewResult Index()
        {
            return View(Product.GetProducts());
        }
    }
}
