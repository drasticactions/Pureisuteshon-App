using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PlayStation_App.Core.Entities;
using PlayStation_App.Core.Entities.User;
using PlayStation_App.Core.Exceptions;
using PlayStation_App.Core.Interfaces;
using PlayStation_App.Core.Tools;
using Refractored.Xam.Settings;

namespace PlayStation_App.Core.Managers
{
    public class AuthenticationManager
    {
        private readonly IWebManager _webManager;

        public AuthenticationManager(IWebManager webManager)
        {
            _webManager = webManager;
        }

        public AuthenticationManager()
            : this(new WebManager())
        {
        }

        public bool HasLoginTokens()
        {
            return !string.IsNullOrEmpty(CrossSettings.Current.GetValueOrDefault<string>("accessToken"));
        }

        public async Task<UserAccountEntity.User> GetUserEntity(UserAccountEntity userAccountEntity)
        {
            var result = await _webManager.GetData(new Uri(EndPoints.VerifyUser), userAccountEntity);
            try
            {
                var user = UserAccountEntity.ParseUser(result.ResultJson);
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to parse user", ex);
            }
        }

        public async Task<UserAuthenticationEntity> RequestAccessToken(string code)
        {
            try
            {
                var dic = new Dictionary<String, String>();
                dic["grant_type"] = "authorization_code";
                dic["client_id"] = EndPoints.ConsumerKey;
                dic["client_secret"] = EndPoints.ConsumerSecret;
                dic["redirect_uri"] = "com.playstation.PlayStationApp://redirect";
                dic["state"] = "x";
                dic["scope"] = "psn:sceapp";
                dic["code"] = code;
                var theAuthClient = new HttpClient();
                var header = new FormUrlEncodedContent(dic);
                var response = await theAuthClient.PostAsync(EndPoints.OauthToken, header);
                string responseContent = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseContent))
                {
                    throw new Exception("Failed to get access token");
                }
                var authEntity = new UserAuthenticationEntity();
                authEntity.Parse(responseContent);
                CrossSettings.Current.AddOrUpdateValue("accessToken", authEntity.AccessToken);
                CrossSettings.Current.AddOrUpdateValue("expiresIn", authEntity.ExpiresIn);
                CrossSettings.Current.AddOrUpdateValue("refreshToken", authEntity.RefreshToken);
                return authEntity;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get access token", ex); 
            }

        }

        public async Task RefreshAccessToken(UserAccountEntity account)
        {
            try
            {
                var dic = new Dictionary<String, String>();
                dic["grant_type"] = "refresh_token";
                dic["client_id"] = EndPoints.ConsumerKey;
                dic["client_secret"] = EndPoints.ConsumerSecret;
                dic["refresh_token"] = account.GetRefreshToken();
                dic["scope"] = "psn:sceapp";

                account.SetAccessToken("updating", null);
                account.SetRefreshTime(1000);
                var theAuthClient = new HttpClient();
                HttpContent header = new FormUrlEncodedContent(dic);
                var response = await theAuthClient.PostAsync(EndPoints.OauthToken, header);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var o = JObject.Parse(responseContent);
                    if (string.IsNullOrEmpty(responseContent))
                    {
                        throw new RefreshTokenException("Could not refresh the user token, no response data");
                    }
                    account.SetAccessToken((String)o["access_token"], (String)o["refresh_token"]);
                    account.SetRefreshTime(long.Parse((String)o["expires_in"]));

                    var authEntity = new UserAuthenticationEntity();
                    authEntity.Parse(responseContent);
                    CrossSettings.Current.AddOrUpdateValue("accessToken", authEntity.AccessToken);
                    CrossSettings.Current.AddOrUpdateValue("expiresIn", authEntity.ExpiresIn);
                    CrossSettings.Current.AddOrUpdateValue("refreshToken", authEntity.RefreshToken);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not refresh the user token", ex);
            }
        }

        public string Status { get; set; }

        public async Task<UserAccountEntity> Authenticate(string userName, string password, int timeout = EndPoints.DefaultTimeoutInMilliseconds)
        {
            if (!_webManager.IsNetworkAvailable)
            {
                throw new LoginFailedException(
                    "The network is unavailable. Check your network settings and please try again.");
            }

            try
            {
                return await SendLoginData(userName, password);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to login: There is a problem with the network", e);
            }
        }

        private async Task<UserAccountEntity> SendLoginData(string userName, string password)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 8_1_2 like Mac OS X) AppleWebKit/600.1.4 (KHTML, like Gecko) Version/8.0 Mobile/12B440 Safari/600.1.4");
            httpClient.DefaultRequestHeaders.Add("Accept-Language", "ja-jp");
            httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            var ohNoTest = await httpClient.GetAsync(new Uri(EndPoints.Login));

            httpClient.DefaultRequestHeaders.Referrer = new Uri("https://auth.api.sonyentertainmentnetwork.com/login.jsp?service_entity=psn&request_theme=liquid");
            httpClient.DefaultRequestHeaders.Add("Origin", "https://auth.api.sonyentertainmentnetwork.com");
            var nameValueCollection = new Dictionary<string, string>
                {
                { "params", "c2VydmljZV9lbnRpdHk9cHNuJnJlcXVlc3RfdGhlbWU9bGlxdWlkJmF1dGhlbnRpY2F0aW9uX2Vycm9yPXRydWU=" },
                { "rememberSignIn", "On" },
                { "j_username", userName },
                { "j_password", password },
            };

            var form = new FormUrlEncodedContent(nameValueCollection);
            var response = await httpClient.PostAsync(EndPoints.LoginPost, form);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to log in: " + response.StatusCode);
            }

            ohNoTest = await httpClient.GetAsync(new Uri(EndPoints.Login));
            var codeUrl = ohNoTest.RequestMessage.RequestUri;
            var queryString = UriExtensions.ParseQueryString(codeUrl.ToString());
            if (queryString.ContainsKey("authentication_error"))
            {
                throw new LoginFailedException("Wrong Username/Password");
            }
            if (!queryString.ContainsKey("targetUrl")) return null;
            queryString = UriExtensions.ParseQueryString(WebUtility.UrlDecode(queryString["targetUrl"]));
            if (!queryString.ContainsKey("code")) return null;

            var authManager = new AuthenticationManager();
            var authEntity = await authManager.RequestAccessToken(queryString["code"]);
            if (authEntity == null) return null;
            var userAccountEntity = new UserAccountEntity();
            userAccountEntity.SetAccessToken(authEntity.AccessToken, authEntity.RefreshToken);
            userAccountEntity = await LoginTest(userAccountEntity);

            return userAccountEntity;
        }

        public async Task<UserAccountEntity> LoginTest(UserAccountEntity userAccountEntity)
        {
            try
            {
                var authManager = new AuthenticationManager();
                UserAccountEntity.User user = await authManager.GetUserEntity(userAccountEntity);
                if (user == null) return null;
                userAccountEntity.SetUserEntity(user);
                return userAccountEntity;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
