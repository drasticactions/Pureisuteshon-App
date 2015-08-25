using System;
using System.Net.Http;
using System.Threading.Tasks;
using PlayStation_App.Core.Entities;
using PlayStation_App.Core.Entities.User;
using PlayStation_App.Core.Managers;

namespace PlayStation_App.Core.Interfaces
{
    public interface IWebManager
    {
        bool IsNetworkAvailable { get; }

        Task<WebManager.Result> PutData(Uri uri, StringContent json, UserAccountEntity userAccountEntity);

        Task<WebManager.Result> DeleteData(Uri uri, StringContent json, UserAccountEntity userAccountEntity);

        Task<WebManager.Result> PostData(Uri uri, StringContent content, UserAccountEntity userAccountEntity);

        Task<WebManager.Result> PostData(Uri uri, FormUrlEncodedContent header, UserAccountEntity userAccountEntity);

        Task<WebManager.Result> GetData(Uri uri, UserAccountEntity userAccountEntity);
        Task<WebManager.Result> PostData(Uri uri, MultipartContent content, UserAccountEntity userAccountEntity);
    }
}
