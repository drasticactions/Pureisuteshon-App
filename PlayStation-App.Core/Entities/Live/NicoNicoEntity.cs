using System.Collections.Generic;

namespace PlayStation_App.Core.Entities.Live
{
    public class NicoNicoEntity
    {
        public class User
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        public class Community
        {
            public string id { get; set; }
            public string name { get; set; }
            public string thumbnail_url { get; set; }
        }

        public class Sce
        {
            public string user_online_id { get; set; }
            public string title_id { get; set; }
            public string title_name { get; set; }
            public string title_product_id { get; set; }
            public string title_preset_text_description { get; set; }
            public bool title_preset { get; set; }
            public string title_session_id { get; set; }
        }

        public class Program
        {
            public string id { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public User user { get; set; }
            public string open_time { get; set; }
            public string start_time { get; set; }
            public string end_time { get; set; }
            public string thumbnail_url { get; set; }
            public Community community { get; set; }
            public string status { get; set; }
            public int view_num { get; set; }
            public string request_token { get; set; }
            public int comment_num { get; set; }
            public Sce sce { get; set; }
        }
        public bool success { get; set; }
        public List<Program> programs { get; set; }
    }
}
