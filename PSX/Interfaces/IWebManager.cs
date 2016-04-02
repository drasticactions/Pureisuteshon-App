using System;
using System.Net.Http;
using System.Threading.Tasks;
using PlayStation.Entities.User;
using PlayStation.Entities.Web;

namespace PlayStation.Interfaces
{
    public interface IWebManager
    {
        Task<Result> PutData(Uri uri, StringContent json, UserAuthenticationEntity userAuthenticationEntity, string language = "ja");

        Task<Result> DeleteData(Uri uri, StringContent json, UserAuthenticationEntity userAuthenticationEntity, string language = "ja");

        Task<Result> PostData(Uri uri, StringContent content, UserAuthenticationEntity userAuthenticationEntity, string language = "ja", bool isMessage = false);

        Task<Result> PostData(Uri uri, FormUrlEncodedContent header, UserAuthenticationEntity userAuthenticationEntity, string language = "ja", bool isMessage = false);

        Task<Result> GetData(Uri uri, UserAuthenticationEntity userAuthenticationEntity, string language = "ja");
        Task<Result> PostData(Uri uri, MultipartContent content, UserAuthenticationEntity userAuthenticationEntity, string language = "ja", bool isMessage = false);
    }
}
