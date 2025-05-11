namespace Blog;

public class Configuration
{
    // JWT - TOKEN - Json Web Token
    public static string JWT_SECRET_KEY = "ZDc4ZTc1ZWI4NDg4YjU4ZDRhNzY5YzRjZGNmYjA1ZGI4ZGU5YjEyNDJjYmE4ZGRmZjI1NTdiZGRjMmY";
    public static string ApiKeyName = "api_key";
    public static string ApiKey = "curso_api_IlTevUM/z0ey3NwCV/unWg==";
    public static SmtpConfiguration Smtp = new();
    
    // Configuração do meu SMTP de envio de email
    public class SmtpConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; } = 25;
        public string Username { get; set; }
        public string Password { get; set; }
        public string From { get; set; }
    }
}