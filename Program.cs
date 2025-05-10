using System.Text;
using Blog;
using Blog.Data;
using Blog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
LoadConfigurationAuthentication(builder);
LoadConfigureMvc(builder);
LoadConfigureServices(builder);
var app = builder.Build();
LoadConfiguration(app);
LoadProject(app);

void LoadConfigurationAuthentication(WebApplicationBuilder builder)
{
    var key = Encoding.ASCII.GetBytes(Configuration.JWT_SECRET_KEY);
    builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(x =>
    {
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
}
void LoadConfigureMvc(WebApplicationBuilder builder)
{
    builder.Services.AddControllers().ConfigureApiBehaviorOptions(options => {
        options.SuppressModelStateInvalidFilter = true;
    });
}
void LoadConfigureServices(WebApplicationBuilder builder)
{
    builder.Services.AddDbContext<BlogDataContext>();
    builder.Services.AddTransient<TokenServices>();
    builder.Services.AddTransient<EmailServices>();
}
void LoadConfiguration(WebApplication app)
{
    Configuration.JWT_SECRET_KEY = app.Configuration.GetValue<string>("JWT_SECRET_KEY");
    Configuration.ApiKeyName = app.Configuration.GetValue<string>("ApiKeyName");
    Configuration.ApiKeyName = app.Configuration.GetValue<string>("ApiKey");

    // serve para trabalhar com envio de email.
    var smtp = new Configuration.SmtpConfiguration();
    app.Configuration.GetSection("Smtp").Bind(smtp);
    
    Configuration.Smtp = smtp;
}
void LoadProject(WebApplication app)
{
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
}   