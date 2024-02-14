using Books.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Books.Controllers
{
    public class HomeController : Controller
    {
        private IBookRepository? _repository;
        private readonly ILogger<HomeController>? _logger;

        public HomeController(IBookRepository repo, ILogger<HomeController> logger)
        {
            _repository = repo;
            _logger = logger;
        }
        //public IActionResult Index()
        //{
        //    return View(_repository.Nodes
        //        .Where(p => p.TreeLevel == 1)
        //        .OrderBy(p => p.Heading));
        //}

        public IActionResult Index() => View(_repository.Nodes.Where(n => n.ParentNodeId == 0));


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