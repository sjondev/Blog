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
    [HttpPost("v1/account/")]
    public async Task<IActionResult> Post(
        [FromBody] RegisterViewModel model, 
        [FromServices] BlogDataContext dataContext)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));
        }

        var user = new User()
        {
            Name = model.Name,
            Email = model.Email,
            Slug = model.Email.Replace("@", "-").Replace(".", "-")
        };
        
        // aqui vamos gerar uma senha com o pacote que instalamos que se chama "SecureIdentity" 
        // ele foi criado pela equipe balta.io para gerar senhas seguras antes de ser armazenadas no
        // banco de dados.
        var password = PasswordGenerator.Generate(25, true, true);
        user.PasswordHash = PasswordHasher.Hash(password);

        try
        {
            await dataContext.Users.AddAsync(user);
            await dataContext.SaveChangesAsync();

            return Ok(new ResultViewModel<dynamic>(new
            {
                user = user.Email, password
            }));
        }
        catch (DbUpdateException)
        {
            return StatusCode(400, new ResultViewModel<string>("5x99 - Este Email já existe!"));
        } 
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("5x99 - Error server!"));
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
    
    
    
    
    
}