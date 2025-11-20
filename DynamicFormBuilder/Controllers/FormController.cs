using Microsoft.AspNetCore.Mvc;
using DynamicFormBuilder.Data;
using DynamicFormBuilder.Models;

namespace DynamicFormBuilder.Controllers
{
    public class FormController : Controller
    {
        private readonly FormRepository _formRepo;
        private readonly OptionRepository _optionRepo;

        public FormController(FormRepository formRepo, OptionRepository optionRepo)
        {
            _formRepo = formRepo;
            _optionRepo = optionRepo;
        }

        public IActionResult Create()
        {
            // Default Option Group (OptionId = 1)
            ViewData["DefaultOptionId"] = 1;
            return View();
        }

        // AJAX API: Save Form with Fields
        [HttpPost]
        [Route("api/form/save")]
        public IActionResult Save([FromBody] FormDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Title))
                return BadRequest("Title is required.");

            int formId = _formRepo.SaveForm(model.Title, model.Fields);
            return Ok(new { formId = formId });
        }

        // Preview page
        public IActionResult Preview(int id)
        {
            var form = _formRepo.GetFormWithFields(id);
            if (form == null)
                return NotFound();

            return View(form);
        }

        // API to get Option values dynamically
        [HttpGet]
        [Route("api/options/{optionId}")]
        public IActionResult GetOptionValues(int optionId)
        {
            var values = _optionRepo.GetOptionValues(optionId);
            return Ok(values);
        }
    }
}
