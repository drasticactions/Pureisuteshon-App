using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PlayStation.Entities.User
{
    public class UserAuthenticationEntity
    {
        public UserAuthenticationEntity()
        {
        }

        public UserAuthenticationEntity(string accessToken, string refreshToken, long expiresInDate)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            ExpiresInDate = expiresInDate;
        }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }

        public long ExpiresInDate { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }
    }
}
