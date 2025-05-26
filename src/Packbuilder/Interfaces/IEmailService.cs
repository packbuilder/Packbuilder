namespace Packbuilder.Interfaces
{
    public interface IEmailService
    {
        public Task SendVerificationEmail(string email, string token, int userId);
        public Task SendPasswordResetEmail(string email, string token, int userId);
    }
}