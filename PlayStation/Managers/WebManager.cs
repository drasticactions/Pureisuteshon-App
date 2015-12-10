using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using PlayStation.Entities.User;
using PlayStation.Entities.Web;
using PlayStation.Interfaces;
using PlayStation.Tools;

namespace PlayStation.Managers
{
    public class WebManager : IWebManager
    {
        public async Task<Result> PutData(Uri uri, StringContent json, UserAuthenticationEntity userAuthenticationEntity, string language = "ja")
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var authenticationManager = new AuthenticationManager();
                    Result result = new Result(false, "");
                    if (RefreshTime(userAuthenticationEntity.ExpiresInDate))
                    {
                        var tokens = await authenticationManager.RefreshAccessToken(userAuthenticationEntity.RefreshToken);
                        result.Tokens = tokens.Tokens;
                    }
                    httpClient.DefaultRequestHeaders.Add("Accept-Language", language);
                    httpClient.DefaultRequestHeaders.Add("Origin", "http://psapp.dl.playstation.net");
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userAuthenticationEntity.AccessToken);
                    var response = await httpClient.PutAsync(uri, json);
                    var responseContent = await response.Content.ReadAsStringAsync();
                    result.IsSuccess = response.IsSuccessStatusCode;
                    result.ResultJson = responseContent;
                    return result;
                }
                catch (Exception)
                {

                    return new Result(false, string.Empty);
                }
            }
        }

        public async Task<Result> DeleteData(Uri uri, StringContent json, UserAuthenticationEntity userAuthenticationEntity, string language = "ja")
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var authenticationManager = new AuthenticationManager();
                    Result result = new Result(false, "");
                    if (RefreshTime(userAuthenticationEntity.ExpiresInDate))
                    {
                        var tokens = await authenticationManager.RefreshAccessToken(userAuthenticationEntity.RefreshToken);
                        result.Tokens = tokens.Tokens;
                    }
                    httpClient.DefaultRequestHeaders.Add("Accept-Language", language);
                    httpClient.DefaultRequestHeaders.Add("Origin", "http://psapp.dl.playstation.net");
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userAuthenticationEntity.AccessToken);
                    var response = await httpClient.DeleteAsync(uri);
                    var responseContent = await response.Content.ReadAsStringAsync();
                    result.IsSuccess = response.IsSuccessStatusCode;
                    result.ResultJson = responseContent;
                    return result;
                }
                catch (Exception)
                {
                    return new Result(false, string.Empty);
                }
            }
        }

        public async  Task<Result> PostData(Uri uri, StringContent content, UserAuthenticationEntity userAuthenticationEntity, string language = "ja")
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var authenticationManager = new AuthenticationManager();
                    Result result = new Result(false, "");
                    if (RefreshTime(userAuthenticationEntity.ExpiresInDate))
                    {
                        var tokens = await authenticationManager.RefreshAccessToken(userAuthenticationEntity.RefreshToken);
                        result.Tokens = tokens.Tokens;
                    }
                    httpClient.DefaultRequestHeaders.Add("Accept-Language", language);
                    httpClient.DefaultRequestHeaders.Add("Origin", "http://psapp.dl.playstation.net");
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userAuthenticationEntity.AccessToken);
                    var response = await httpClient.PostAsync(uri, content);
                    var responseContent = await response.Content.ReadAsStringAsync();
                    result.IsSuccess = response.IsSuccessStatusCode;
                    result.ResultJson = responseContent;
                    return result;
                }
                catch (Exception)
                {
                    return new Result(false, string.Empty);
                }
            }
        }

        public async Task<Result> PostData(Uri uri, FormUrlEncodedContent header, UserAuthenticationEntity userAuthenticationEntity, string language = "ja")
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var authenticationManager = new AuthenticationManager();
                    Result result = new Result(false, "");
                    if (RefreshTime(userAuthenticationEntity.ExpiresInDate))
                    {
                        var tokens = await authenticationManager.RefreshAccessToken(userAuthenticationEntity.RefreshToken);
                        result.Tokens = tokens.Tokens;
                    }
                    httpClient.DefaultRequestHeaders.Add("Accept-Language", language);
                    httpClient.DefaultRequestHeaders.Add("Origin", "http://psapp.dl.playstation.net");
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userAuthenticationEntity.AccessToken);
                    var response = await httpClient.PostAsync(uri, header);
                    var responseContent = await response.Content.ReadAsStringAsync();
                    result.IsSuccess = response.IsSuccessStatusCode;
                    result.ResultJson = responseContent;
                    return result;
                }
                catch
                {
                    // TODO: Add detail error result to json object.
                    return new Result(false, string.Empty);
                }
            }
        }

        public async Task<Result> GetData(Uri uri, UserAuthenticationEntity userAuthenticationEntity, string language = "ja")
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var authenticationManager = new AuthenticationManager();
                    Result result = new Result(false, "");
                    if (RefreshTime(userAuthenticationEntity.ExpiresInDate))
                    {
                        var tokens = await authenticationManager.RefreshAccessToken(userAuthenticationEntity.RefreshToken);
                        result.Tokens = tokens.Tokens;
                    }
                    httpClient.DefaultRequestHeaders.Add("Origin", "http://psapp.dl.playstation.net");
                    httpClient.DefaultRequestHeaders.Add("Accept-Language", language);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userAuthenticationEntity.AccessToken);
                    var response = await httpClient.GetAsync(uri);
                    var responseContent = await response.Content.ReadAsStringAsync();
                    result.IsSuccess = response.IsSuccessStatusCode;
                    result.ResultJson = responseContent;
                    return result;
                }
                catch(Exception ex)
                {
                    // TODO: Add detail error result to json object.
                    return new Result(false, string.Empty);
                }
            }
        }

        public async Task<Result> PostData(Uri uri, MultipartContent content, UserAuthenticationEntity userAuthenticationEntity, string language = "ja")
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var authenticationManager = new AuthenticationManager();
                    Result result = new Result(false, "");
                    if (RefreshTime(userAuthenticationEntity.ExpiresInDate))
                    {
                        var tokens = await authenticationManager.RefreshAccessToken(userAuthenticationEntity.RefreshToken);
                        result.Tokens = tokens.Tokens;
                    }
                    httpClient.DefaultRequestHeaders.Add("Accept-Language", language);
                    httpClient.DefaultRequestHeaders.Add("Origin", "http://psapp.dl.playstation.net");
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userAuthenticationEntity.AccessToken);
                    var response = await httpClient.PostAsync(uri, content);
                    var responseContent = await response.Content.ReadAsStringAsync();
                    result.IsSuccess = response.IsSuccessStatusCode;
                    result.ResultJson = responseContent;
                    return result;
                }
                catch
                {
                    // TODO: Add detail error result to json object.
                    return new Result(false, string.Empty);
                }
            }
        }

        private bool RefreshTime(long refreshTime)
        {
            return AuthHelpers.GetUnixTime(DateTime.Now) > refreshTime;
        }
    }
}
