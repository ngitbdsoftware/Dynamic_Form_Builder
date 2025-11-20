namespace DynamicFormBuilder.Models
{
    public class FieldDto
    {
        public int FieldId { get; set; }
        public string Label { get; set; } = string.Empty;
        public bool IsRequired { get; set; }      
        public int? SelectedOptionValueId { get; set; }   // store OptionValueId from database
    }
}
