using GuhaStore.Core.Entities;
using GuhaStore.Core.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace GuhaStore.Application.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendOrderConfirmationAsync(Order order)
    {
        var subject = $"Xác nhận đơn hàng #{order.OrderCode}";
        var body = $@"
            <h2>Cảm ơn bạn đã đặt hàng!</h2>
            <p>Đơn hàng của bạn đã được xác nhận với mã đơn hàng: <strong>#{order.OrderCode}</strong></p>
            <p>Tổng tiền: {order.TotalAmount:N0} VNĐ</p>
            <p>Phương thức thanh toán: Tiền mặt khi nhận hàng (COD)</p>
            <p>Chúng tôi sẽ liên hệ với bạn sớm nhất để xác nhận và giao hàng.</p>
        ";

        // In a real implementation, you would get the customer email from the order
        // For now, this is a placeholder
        await SendEmailAsync("customer@example.com", subject, body);
    }

    public async Task SendOrderStatusUpdateAsync(Order order, int newStatus)
    {
        var statusText = newStatus switch
        {
            0 => "Đang chờ xử lý",
            1 => "Đang xử lý",
            2 => "Đã giao hàng",
            3 => "Đã hoàn thành",
            4 => "Đã hủy",
            _ => "Không xác định"
        };

        var subject = $"Cập nhật trạng thái đơn hàng #{order.OrderCode}";
        var body = $@"
            <h2>Cập nhật đơn hàng</h2>
            <p>Đơn hàng #{order.OrderCode} của bạn đã được cập nhật trạng thái:</p>
            <p><strong>{statusText}</strong></p>
        ";

        await SendEmailAsync("customer@example.com", subject, body);
    }

    public async Task SendPasswordResetAsync(string email, string token)
    {
        var subject = "Đặt lại mật khẩu";
        var resetLink = $"{_configuration["AppSettings:BaseUrl"]}/Account/ResetPassword?token={token}";
        var body = $@"
            <h2>Đặt lại mật khẩu</h2>
            <p>Bạn đã yêu cầu đặt lại mật khẩu. Vui lòng click vào link sau:</p>
            <p><a href=""{resetLink}"">{resetLink}</a></p>
            <p>Link này sẽ hết hạn sau 24 giờ.</p>
        ";

        await SendEmailAsync(email, subject, body);
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration["EmailSettings:FromEmail"] ?? "noreply@guhastore.com"));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = body
            };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                _configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com",
                int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587"),
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                _configuration["EmailSettings:Username"] ?? "",
                _configuration["EmailSettings:Password"] ?? "");

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

            return true;
        }
        catch
        {
            // Log error in production
            return false;
        }
    }
}

