using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModel;

public class EditorCategoryViewModel
{
    [Required(ErrorMessage = "Category is required.")]
    [StringLength(40, MinimumLength = 3, ErrorMessage = "Category must be between 3 and 40 characters.")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "This field is required")]
    public string Slug { get; set; }
}