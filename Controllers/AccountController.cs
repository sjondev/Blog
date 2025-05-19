using System.Text.RegularExpressions;
using Blog.Data;
using Blog.ModalStateExtansion;
using Blog.Models;
using Blog.Services;
using Blog.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

namespace Blog.Controllers;


[ApiController]
public class AccountController : ControllerBase
{
    // Criar o cadastro do utilizador 
     [HttpPost("v1/accounts/")]
    public async Task<IActionResult> Post(
        [FromBody] RegisterViewModel model,
        [FromServices] BlogDataContext context,
        [FromServices] LocalhostEmailService emailService)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

        var user = new User
        {
            Name = model.Name,
            Email = model.Email,
            Slug = model.Email.Replace("@", "-").Replace(".", "-")
        };

        var password = PasswordGenerator.Generate(25);
        user.PasswordHash = PasswordHasher.Hash(password);

        try
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            await emailService.SendAsync(
                "Enviado para o meu resend",
                "<h1>subindo o Teste da minha maquina</h1>",
                user.Email, // "delivered@resend.dev"
                user.Name // "resend"
                );
            return Ok(new ResultViewModel<dynamic>(new
            {
                user = user.Email, password
            }));
        }
        catch (DbUpdateException)
        {
            return StatusCode(400, new ResultViewModel<string>("05X99 - Este E-mail já está cadastrado"));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("05X04 - Falha interna no servidor"));
        }
    }
    
    
    
    [HttpPost("v1/acconts/login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginViewModel model,
        [FromServices] BlogDataContext dataContext,
        [FromServices] TokenServices tokenServices)
    {
        if(!ModelState.IsValid) return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

        var user = await dataContext
            .Users
            .AsNoTracking()
            .Include(r => r.Roles)
            .FirstOrDefaultAsync(x => x.Email == model.Email);
        
        if (user == null)
            return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos"));

        if (!PasswordHasher.Verify(user.PasswordHash, model.Password))
            return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos"));

        try
        {
            var token = tokenServices.GenerateToken(user);
            return Ok(new ResultViewModel<string>(token, null));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("5x04 - Error internal server!"));
        }
    }

    [Authorize]
    [HttpPost("v1/accounts/upload-image")]
    public async Task<IActionResult> UploadImage(
        [FromBody] UploadImageViewModel model,
        [FromServices] BlogDataContext dataContext
        )
    {
        var fileName = $"{Guid.NewGuid().ToString()}.jpg";
        var data = new Regex(@"^data:image\/[a-z]+;base64,").Replace(model.Base64Image, "");
        var bytes = Convert.FromBase64String(model.Base64Image);
        // using var stream = new MemoryStream(bytes);

        try
        {
            await System.IO.File.WriteAllBytesAsync($"wwwroot/images/{fileName}", bytes);
        }
        catch (Exception e)
        {
            return StatusCode(500, new ResultViewModel<string>("5x04 - Error internal server!"));
        }

        var user = await dataContext
                .Users
                .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);
        
        if (user == null) return NotFound(new ResultViewModel<Category>("User not found"));

        user.Image = $"https://localhost:0000/images/{fileName}";

        try
        {
            dataContext.Users.Update(user);
            await dataContext.SaveChangesAsync();
        }

        catch (Exception e)
        {
            return StatusCode(500, new ResultViewModel<string>("5x04 - Error internal server!"));
        }

        return Ok(new ResultViewModel<string>("Image updated successfully"));
    }
}