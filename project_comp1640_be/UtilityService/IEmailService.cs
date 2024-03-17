using project_comp1640_be.Model;

namespace project_comp1640_be.UtilityService
{
    public interface IEmailService
    {
        void SendEmail(EmailModel emailModel);
    }
}
