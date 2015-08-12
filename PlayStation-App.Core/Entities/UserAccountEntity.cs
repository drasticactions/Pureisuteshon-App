using System;
using Newtonsoft.Json.Linq;
using Refractored.Xam.Settings;

namespace PlayStation_App.Core.Entities
{
    public class UserAccountEntity
    {
        private User _entity;
        private readonly AccountData _data;
        private Boolean _isCalled;

        public String GetAccessToken()
        {
            if (GetUnixTime(DateTime.Now) - this._data.StartTime < this._data.RefreshTime)
                return _data.AccessToken;
            if (this._isCalled) return _data.AccessToken;
            _isCalled = true;
            return "refresh";
        }

        public UserAccountEntity()
        {
            var accessToken = CrossSettings.Current.GetValueOrDefault<string>("accessToken");
            var refreshToken = CrossSettings.Current.GetValueOrDefault<string>("refreshToken");
            _data = new AccountData((string)accessToken, (string)refreshToken, 3600);
            _entity = null;
            _isCalled = false;
        }

        public void SetUserEntity(User entity)
        {
            _entity = entity;
        }

        public User GetUserEntity()
        {
            return _entity;
        }

        public void SetAccessToken(String token, String refresh)
        {
            _data.AccessToken = token;
            _data.RefreshToken = refresh;
            if (_data.RefreshToken != null)
            {
                _isCalled = false;
            }
        }

        public void SetRefreshTime(long time)
        {
            this._data.RefreshTime = GetUnixTime(DateTime.Now) + time;
            this._data.StartTime = GetUnixTime(DateTime.Now);
        }

        private class AccountData
        {
            public AccountData(String token, String refresh, int time)
            {
                this.AccessToken = token;
                this.RefreshToken = refresh;
                this.RefreshTime = UserAccountEntity.GetUnixTime(DateTime.Now) + time;
                this.StartTime = UserAccountEntity.GetUnixTime(DateTime.Now);
            }

            public string AccessToken;
            public string RefreshToken;
            public long RefreshTime;
            public long StartTime;
        }

        public string GetRefreshToken()
        {
            return _data.RefreshToken;
        }

        public static long GetUnixTime(DateTime time)
        {
            time = time.ToUniversalTime();
            TimeSpan timeSpam = time - (new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local));
            return (long)timeSpam.TotalSeconds;
        }

        public override string ToString()
        {
            return this.GetAccessToken() + ":" + this.GetRefreshToken();
        }

        public class User
        {
            public string AccountId { get; set; }

            public string MAccountId { get; set; }

            public string Region { get; set; }

            public string Language { get; set; }

            public string OnlineId { get; set; }

            public string Age { get; set; }

            public string DateOfBirth { get; set; }

            public string CommunityDomain { get; set; }

            public bool SubAccount { get; set; }

            public bool Ps4Available { get; set; }
        }

        public static User ParseUser(string json)
        {
            var o = JObject.Parse(json);
            return new User()
            {
                AccountId = (string)o["accountId"] ?? string.Empty,
                MAccountId = (string)o["mAccountId"] ?? string.Empty,
                Region = (string)o["region"] ?? string.Empty,
                Language = (string)o["language"] ?? string.Empty,
                OnlineId = (string)o["onlineId"] ?? string.Empty,
                Age = (string)o["age"] ?? string.Empty,
                DateOfBirth = (string)o["dateOfBirth"] ?? string.Empty,
                CommunityDomain = (string)o["communityDomain"] ?? string.Empty,
                Ps4Available = o["ps4Available"] != null && (bool)o["ps4Available"],
                SubAccount = o["subaccount"] != null && (bool)o["subaccount"]
            };
        }
    }
}
