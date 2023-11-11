using ShopShoesAPI.model;

namespace ShopShoesAPI.email
{
    public interface IEmail
    {
        Task SendEmail(MailMessage mailMessage);
    }
}
