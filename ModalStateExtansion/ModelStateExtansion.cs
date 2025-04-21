using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Blog.ModalStateExtansion;

public static class ModelStateExtansion
{
    public static List<string> GetErrors(this ModelStateDictionary modelState)
    {
        var result = new List<string>();
        foreach (var items in modelState.Values) 
            result.AddRange(items.Errors.Select(error => error.ErrorMessage));
        
        return result;
    }
}