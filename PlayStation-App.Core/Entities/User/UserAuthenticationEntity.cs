using System;
using Newtonsoft.Json.Linq;

namespace PlayStation_App.Core.Entities.User
{
    public class UserAuthenticationEntity
    {
        public string AccessToken { get; private set; }

        public string RefreshToken { get; private set; }

        public long ExpiresIn { get; private set; }

        public string TokenType { get; private set; }

        public string Scope { get; private set; }

        public void Parse(string json)
        {
            var o = JObject.Parse(json);
            AccessToken = (String)o["access_token"];
            RefreshToken = (String)o["refresh_token"];
            ExpiresIn = (long)o["expires_in"];
            TokenType = (String)o["token_type"];
            Scope = (String)o["scope"];
        }
    }
}
