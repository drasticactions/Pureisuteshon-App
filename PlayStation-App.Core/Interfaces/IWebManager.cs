using System;
using System.Net.Http;
using System.Threading.Tasks;
using PlayStation_App.Core.Entities;
using PlayStation_App.Core.Managers;

namespace PlayStation_App.Core.Interfaces
{
    public interface IWebManager
    {
        bool IsNetworkAvailable { get; }
        Task<WebManager.Result> PostData(Uri uri, FormUrlEncodedContent header, UserAccountEntity userAccountEntity);

        Task<WebManager.Result> GetData(Uri uri, UserAccountEntity userAccountEntity);
    }
}
