using Blog.Data;
using Blog.ModalStateExtansion;
using Blog.Models;
using Blog.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers;

[ApiController]
public class CategoryController : ControllerBase
{
     [HttpGet("v1/categories")]
     public async Task<IActionResult> GetAsync([FromServices] BlogDataContext context)
     {
          try
          {
               var categories = await context.Categories.ToListAsync();
               return Ok(new ResultViewModel<List<Category>>(categories));
          }
          catch
          {
               return StatusCode(500, new ResultViewModel<Category>("05X01 - Failed to get categories"));
          }
     }

     [HttpGet("v1/categories/{id:int}")]
     public async Task<IActionResult> GetByIdAsync([FromServices] BlogDataContext context, [FromRoute] int id)
     {
          try
          {
               var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
               if (category == null) return NotFound(new ResultViewModel<Category>("Content not found!"));
               return Ok(new ResultViewModel<Category>(category));
          }
          catch
          {
               return StatusCode(500, new ResultViewModel<Category>("05X01 - Failed to get category"));
          }
     }

     [HttpPost("v1/categories")]
     public async Task<IActionResult> PostAsync([FromServices] BlogDataContext context, [FromBody] EditorCategoryViewModel editorCategory)
     {
          if (!ModelState.IsValid)
               return BadRequest(new ResultViewModel<Category>(ModelState.GetErrors()));
          try
          {
               var category = new Category
               {
                    Id = 0,
                    Name = editorCategory.Name,
                    Slug = editorCategory.Slug.ToLower(),
               };
               await context.Categories.AddAsync(category);
               await context.SaveChangesAsync();
               return Created($"v1/categories/{category.Id}", new ResultViewModel<Category>(category));
          }
          catch (DbUpdateException ex)
          {
               return StatusCode(500, new ResultViewModel<Category>("05XE1 - Not possible to add category"));
          }
          catch
          {
               return StatusCode(500, new ResultViewModel<Category>("05XE1 - Failed to add category"));
          }
     }

     [HttpPut("v1/categories/{id:int}")]
     public async Task<IActionResult> PutAsync([FromServices] BlogDataContext context, [FromBody] EditorCategoryViewModel categoryEditor, [FromRoute] int id)
     {
          // aqui estamos padronizando os erros que podem vir genericos pelo model state
          if(!ModelState.IsValid) 
               return BadRequest(new ResultViewModel<Category>(ModelState.GetErrors()));
          
          try
          {
               var categoryModel = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
               if (categoryModel == null) return NotFound();

               categoryModel.Name = categoryEditor.Name;
               categoryModel.Slug = categoryEditor.Slug;

               context.Categories.Update(categoryModel);
               await context.SaveChangesAsync();

               return Ok(categoryEditor);
          }
          catch (DbUpdateException ex)
          {
               return StatusCode(500, new ResultViewModel<Category>("05XE1 - Not possible to update category"));
          }
          catch
          {
               return StatusCode(500, new ResultViewModel<Category>("05XE1 - Failed to update category"));
          }
     }

     
     [HttpDelete("v1/categories/{id:int}")]
     public async Task<IActionResult> DeleteAsync([FromServices] BlogDataContext context, [FromBody] Category category, [FromRoute] int id)
     {
          try
          {
               var delete = await context.Categories.FirstOrDefaultAsync(x => x.Id == category.Id);
               if (delete == null) return BadRequest();
               context.Categories.Remove(delete);
               await context.SaveChangesAsync();
               return Ok(category);
          }
          catch (DbUpdateException)
          {
               return StatusCode(500, new ResultViewModel<Category>("05XE1 - Failed to delete category"));
          }
          catch
          {
               return StatusCode(500, new ResultViewModel<Category>("05XE1 - Failed to delete category"));
          }
     }
}