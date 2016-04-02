using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using SnapFeud.WebApi.Models;
using Microsoft.Data.Entity;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace SnapFeud.WebApi.Controllers
{
    public class HomeController : Controller
    {
        private SnapFeudContext dbContext;

        public HomeController(SnapFeudContext context)
        {
            dbContext = context;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var highscores = dbContext.Games.Include(b => b.Player).OrderByDescending(x => x.Score).Take(10);
            return View(highscores.ToList());
        }
    }
}
