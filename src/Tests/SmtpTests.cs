using MailKit.Net.Smtp;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using MimeKit;
using NUnit.Framework;
using System.IO;
using MimeKit.Utils;
using MailHub.Services.MessageService;
using System.Threading.Tasks;

namespace Tests
{
    public class SmtpTests
    {
        private IWebHost _webHost;
        [SetUp]
        public void Setup()
        {
            _webHost = WebHost.CreateDefaultBuilder()
                .UseStartup<TestStartup>()
                .Build();

            _webHost.RunAsync();
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
        public async Task SendMail_WithInlineAttachments_Should_BeOk()
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Mr.Test", "mr@test.com"));
            message.To.Add(new MailboxAddress("Mrs.Test", "mrs@test.com"));
            message.Subject = "Test message with inline attachments";

            var builder = new BodyBuilder
            {
                TextBody = "Test message"
            };

            var image = builder.LinkedResources.Add("attachment.png");
            image.ContentId = MimeUtils.GenerateMessageId();
            builder.HtmlBody = string.Format(@"<p>Hey!</p><img src=""cid:{0}"">", image.ContentId);

            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            client.Connect("localhost", 25, false);
            client.Send(message);
            client.Disconnect(true);

            var messageService = _webHost.Services.GetService(typeof(MessageService)) as MessageService;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            await messageService.DeleteMessagesAsync("mrs@test.com");
#pragma warning restore CS8602 // Dereference of a possibly null reference.
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

            var file = "attachment.png";
            var attachment = new MimePart("image", "png")
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
}