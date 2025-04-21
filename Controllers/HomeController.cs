using Blog.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers;

[ApiController]
[Route("")]
public class HomeController : ControllerBase
{
    // Health Check -> estou verificando se minha API está online.
    /*[HttpGet("health-check")]
    public IActionResult Get()
    {
        return Ok();
    }*/
    
    public IActionResult Get()
    {
        return Ok("ola mundo");
    }
}