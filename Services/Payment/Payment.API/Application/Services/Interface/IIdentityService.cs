namespace Payment.API.Infrastructure.Services.Interface
{
    public interface IIdentityService
    {
        string GetUserIdentity();

        string GetUserName();
    }
}