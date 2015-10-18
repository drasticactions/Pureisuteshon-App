namespace PlayStation.Entities.Web
{
    public class Result
    {
        public Result(bool isSuccess = false, string json = "", string token = "")
        {
            IsSuccess = isSuccess;
            ResultJson = json;
            Tokens = token;
        }

        public bool IsSuccess { get; set; }
        public string ResultJson { get; set; }

        public string Error { get; set; }

        public string Tokens { get; set; }
    }
}
