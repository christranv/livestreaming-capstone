namespace Event.API.Infrastructure.Services.Interface
{
    public interface IIdentityService
    {
        string GetUserIdentity();

        string GetUserName();
    }
}