using System;
using System.Collections.Generic;

namespace Identity.API.Helpers
{
    public static class QueryBuilder
    {
        public static string FacebookUserInfoQuery(List<string> fields, string token)
        {
            return "?fields=" + String.Join(",", fields) + "&access_token=" + token;
        }

        public static string GetQuery(Dictionary<string, string> values, ProviderType provider)
        {
            switch (provider)
            {
                case ProviderType.Facebook:

                    var fields = values["fields"];
                    var access_token = values["access_token"];
                    return $"?fields={fields}&access_token={access_token}";

                case ProviderType.Google:

                    var google_access_token = values["token"];
                    return $"?access_token={google_access_token}";

                default:
                    return null;
            }
        }
    }
}
