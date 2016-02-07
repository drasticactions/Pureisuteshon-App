using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;

namespace PlayStation_App.Models.Authentication
{
    public class AccountUser
    {
        [PrimaryKey]
        public string AccountId { get; set; }

        public string Username { get; set; }

        public string AvatarUrl { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public long RefreshDate { get; set; }
        public string Region { get; set; }
        public string Language { get; set; }
    }
}
