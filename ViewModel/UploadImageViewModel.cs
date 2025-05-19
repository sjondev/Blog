using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModel;

public class UploadImageViewModel
{
    [Required(ErrorMessage = "Please enter an image file")]
    public string Base64Image { get; set; } 
}