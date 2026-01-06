using GuhaStore.Core.Entities;

namespace GuhaStore.Core.Interfaces;

public interface IEmailService
{
    Task SendOrderConfirmationAsync(Order order);
    Task SendOrderStatusUpdateAsync(Order order, int newStatus);
    Task SendPasswordResetAsync(string email, string token);
    Task<bool> SendEmailAsync(string to, string subject, string body);
}

