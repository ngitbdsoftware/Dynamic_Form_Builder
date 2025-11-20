using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace DynamicFormBuilder.ViewComponents
{
    // ViewModel for the dropdown
    public class DropdownViewModel
    {
        public int SelectedId { get; set; }
        public string Name { get; set; }
        public bool IsRequired { get; set; }
        public IEnumerable<SelectListItem> Options { get; set; }
    }

    // ViewComponent class
    public class DropdownViewComponent : ViewComponent
    {
        // This method is invoked by @Component.InvokeAsync
        public IViewComponentResult Invoke(int optionId, int? selectedId, string name, bool isRequired)
        {
            // Sample logic: get options from database or predefined list
            var options = GetOptions(optionId);

            var model = new DropdownViewModel
            {
                Name = name,
                SelectedId = selectedId ?? 0,
                IsRequired = isRequired,
                Options = options
            };

            return View(model); // This will look for Default.cshtml in the convention path
        }

        // Example method to get dropdown options
        private IEnumerable<SelectListItem> GetOptions(int optionId)
        {
            // You can replace this with actual database call
            var sampleOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Option 1" },
                new SelectListItem { Value = "2", Text = "Option 2" },
                new SelectListItem { Value = "3", Text = "Option 3" },
            };

            // Filter options based on optionId if needed
            return sampleOptions;
        }
    }
}