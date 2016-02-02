using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlayStation.Entities.User;
using PlayStation.Entities.Web;
using PlayStation.Interfaces;
using PlayStation.Tools;

namespace PlayStation.Managers
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

        public async Task<Result> GetUserEntity(UserAuthenticationEntity entity, string lang)
        {
            return await _webManager.GetData(new Uri(EndPoints.VerifyUser), entity, lang);
        }

        public async Task<string> RequestAccessToken(string code)
        {
            try
            {
                var dic = new Dictionary<string, string>
                {
                    ["grant_type"] = "authorization_code",
                    ["client_id"] = EndPoints.ConsumerKey,
                    ["client_secret"] = EndPoints.ConsumerSecret,
                    ["redirect_uri"] = "com.playstation.PlayStationApp://redirect",
                    ["duid"] = EndPoints.Duid,
                    ["scope"] = EndPoints.Scope,
                    ["code"] = code,
                    ["service_entity"] = "urn:service-entity:psn"
                };
                var theAuthClient = new HttpClient();
                var header = new FormUrlEncodedContent(dic);
                var response = await theAuthClient.PostAsync(EndPoints.OauthToken, header);
                string responseContent = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseContent))
                {
                    throw new Exception("Failed to get access token");
                }
                return responseContent;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get access token", ex);
            }
        }

        public async Task<Result> RefreshAccessToken(string refreshToken)
        {
            try
            {
                var dic = new Dictionary<string, string>
                {
                    ["grant_type"] = "refresh_token",
                    ["client_id"] = EndPoints.ConsumerKey,
                    ["client_secret"] = EndPoints.ConsumerSecret,
                    ["refresh_token"] = refreshToken,
                    ["scope"] = EndPoints.Scope
                };
                var theAuthClient = new HttpClient();
                HttpContent header = new FormUrlEncodedContent(dic);
                var response = await theAuthClient.PostAsync(EndPoints.OauthToken, header);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(responseContent))
                    {
                        throw new Exception("Could not refresh the user token, no response data");
                    }
                    return !string.IsNullOrEmpty(responseContent) ? new Result(true, null, responseContent) : new Result(false, null, null);
                }

                return new Result(false, null, null);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not refresh the user token", ex);
            }
        }

        public async Task<Result> SendLoginData(string username, string password)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    // Set the default headers
                    // These are needed so the PSN Auth server will accept our username and password
                    // and hand us a token
                    // If we don't, we will get an HTML page back with no codes

                    httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                    httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 9_2 like Mac OS X) AppleWebKit/601.1.46 (KHTML, like Gecko) Mobile/13C75 PlayStation Messages App/3.10.67");
                    httpClient.DefaultRequestHeaders.Add("Accept-Language", "ja-jp");
                    httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
                    httpClient.DefaultRequestHeaders.Referrer = new Uri(EndPoints.AuthReferrer);
                    httpClient.DefaultRequestHeaders.Add("Origin", "https://id.sonyentertainmentnetwork.com");

                    var nameValueCollection = new Dictionary<string, string>
                    {
                        {"authentication_type", "password"},
                        {"client_id", EndPoints.LoginKey},
                        {"username", username},
                        {"password", password},
                    };
                    var form = new FormUrlEncodedContent(nameValueCollection);

                    // Send out initial request to get an "SSO Cookie", which is actually in JSON too. For some reason.
                    var response = await httpClient.PostAsync(EndPoints.SsoCookie, form);

                    if (!response.IsSuccessStatusCode)
                    {
                        return ErrorHandler.CreateErrorObject(new Result(), "Username/Password Failed", "Auth");
                    }

                    var responseJson = await response.Content.ReadAsStringAsync();

                    // Get the npsso key. Add the client id, scope, and service entity and send it back.
                    var authorizeCheck = JsonConvert.DeserializeObject<AuthorizeCheck>(responseJson);
                    authorizeCheck.client_id = EndPoints.LoginKey;
                    authorizeCheck.scope = EndPoints.Scope;
                    authorizeCheck.service_entity = "urn:service-entity:psn";

                    var json = JsonConvert.SerializeObject(authorizeCheck);
                    var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                    // Call auth check so we can continue the flow...
                    await httpClient.PostAsync(EndPoints.AuthorizeCheck, stringContent);

                    // Get our code, so we can get our access tokens.
                    var testResult = await httpClient.GetAsync(new Uri(EndPoints.Login));

                    var codeUrl = testResult.Headers.Location;
                    var queryString = UriExtensions.ParseQueryString(codeUrl.ToString());
                    if (queryString.ContainsKey("authentication_error"))
                    {
                        return ErrorHandler.CreateErrorObject(new Result(), "Failed to get OAuth Code (Authentication_error)", "Auth");
                    }

                    if (!queryString.ContainsKey("code"))
                    {
                        return ErrorHandler.CreateErrorObject(new Result(), "Failed to get OAuth Code (No code)", "Auth");
                    }

                    var authManager = new AuthenticationManager();
                    var authEntity = await authManager.RequestAccessToken(queryString["code"]);
                    return !string.IsNullOrEmpty(authEntity) ? new Result(true, null, authEntity) : new Result(false, null, null);
                }
            }
            catch (Exception ex)
            {
                // All else fails, send back the stack trace.
                return ErrorHandler.CreateErrorObject(new Result(), ex.Message, ex.StackTrace);
            }
        }

        public async Task<Result> OldSendLoginData(string userName, string password)
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
                return new Result(false, null, null);
            }

            ohNoTest = await httpClient.GetAsync(new Uri(EndPoints.Login));
            var codeUrl = ohNoTest.RequestMessage.RequestUri;
            var queryString = UriExtensions.ParseQueryString(codeUrl.ToString());
            if (queryString.ContainsKey("authentication_error"))
            {
                return new Result(false, null, null);
            }
            if (!queryString.ContainsKey("targetUrl")) return new Result(false, null, null);
            queryString = UriExtensions.ParseQueryString(WebUtility.UrlDecode(queryString["targetUrl"]));
            if (!queryString.ContainsKey("code")) return null;

            var authManager = new AuthenticationManager();
            var authEntity = await authManager.RequestAccessToken(queryString["code"]);
            return !string.IsNullOrEmpty(authEntity) ? new Result(true, null, authEntity) : new Result(false, null, null);
        }

        private class AuthorizeCheck
        {
            public string client_id { get; set; }

            public string npsso { get; set; }

            public string scope { get; set; }

            public string service_entity { get; set; }
        }
    }
}
