using Newtonsoft.Json.Linq;

namespace Identity.API.Providers.Interfaces
{
    public interface IExternalAuthProvider
    {
        JObject GetUserInfo(string accessToken);
    }
}
