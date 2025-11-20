using Microsoft.AspNetCore.Mvc;
using DynamicFormBuilder.Data;

namespace DynamicFormBuilder.Controllers
{
    public class HomeController : Controller
    {
        private readonly FormRepository _repo;

        public HomeController(FormRepository repo)
        {
            _repo = repo;
        }

        public IActionResult Index()
        {
            return View();
        }

        // DataTables server-side pagination
        [HttpPost]
        public IActionResult List(int draw, int start = 0, int length = 10, string? search = null)
        {
            // DataTables posts search[value]
            if (string.IsNullOrEmpty(search))
            {
                var dtSearch = Request.Form["search[value]"];
                if (!string.IsNullOrWhiteSpace(dtSearch))
                    search = dtSearch!;
            }

            var (data, total, filtered) = _repo.GetFormsPaged(start, length, search);

            return Json(new
            {
                draw = draw,
                recordsTotal = total,
                recordsFiltered = filtered,
                data = data
            });
        }
    }
}
