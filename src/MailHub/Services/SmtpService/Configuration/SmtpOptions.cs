namespace MailHub.Services.MailService.Configuration
{
    public class SmtpOptions
    {
        public string ServerName { get; set; }
        public string[] AllowedDomains { get; set; }

        public override string ToString()
        {
            return $"ServeName: {ServerName}, AllowedDomains: {string.Join(",", AllowedDomains)}";
        }
    }
}