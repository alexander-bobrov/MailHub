using Database.Configuration;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MimeKit;
using NUnit.Framework;
using MailHub.Services.MailService.Configuration;
using System.IO;
using MimeKit.Utils;
using MailKit;
using System;

namespace Tests
{
    public class SmtpTests
    {
        [SetUp]
        public void Setup()
        {
            WebHost.CreateDefaultBuilder()
                .UseStartup<StartupTest>()
                .Build()
                .RunAsync();
        }

        [Test]
        public void SendMail_Should_BeOk()
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Mr.Test", "mr@test.com"));
            message.To.Add(new MailboxAddress("Mrs.Test", "mrs@test.com"));
            message.Subject = "Test message";

            message.Body = new TextPart("plain")
            {
                Text = @"It's a test message"
            };

            using var client = new SmtpClient();
            client.Connect("127.0.0.1", 25, false);
            client.Send(message);
            client.Disconnect(true);
        }

        [Test]
        public void SendMail_WithInlineAttachments_Should_BeOk()
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Mr.Test", "mr@test.com"));
            message.To.Add(new MailboxAddress("Mrs.Test", "mrs@test.com"));
            message.Subject = "Test message with inline attachments";

            var builder = new BodyBuilder
            {
                TextBody = "Test message"
            };

            var image = builder.LinkedResources.Add("attachment.jpg");
            image.ContentId = MimeUtils.GenerateMessageId();
            builder.HtmlBody = string.Format(@"<p>Hey!</p><img src=""cid:{0}"">", image.ContentId);

            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient(new ProtocolLogger("smtp.log"));
            client.Connect("127.0.0.1", 25, false);
            client.Send(message);
            client.Disconnect(true);
        }

        [Test]
        public void SendMail_WithAttachments_Should_BeOk()
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Mr.Test", "mr@test.com"));
            message.To.Add(new MailboxAddress("Mrs.Test", "mrs@test.com"));
            message.Subject = "Test message with attachments";

            var body = new TextPart("plain")
            {
                Text = @"It's a test message with attachment"
            };

            var file = "attachment.jpg";
            var attachment = new MimePart("image", "jpeg")
            {
                Content = new MimeContent(File.OpenRead(file), ContentEncoding.Default),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = Path.GetFileName(file)
            };

            var multipart = new Multipart("mixed")
            {
                body,
                attachment
            };

            message.Body = multipart;

            using var client = new SmtpClient();
            client.Connect("127.0.0.1", 25, false);
            client.Send(message);
            client.Disconnect(true);
        }
    
    }

    public class StartupTest 
    {
        private readonly IConfiguration _configuration;

        public StartupTest(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDatabase(_configuration);
            services.UseSmtpService(_configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) { }

    }
}