using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlayStation.Entities.User;
using PlayStation.Entities.Web;
using PlayStation.Interfaces;
using PlayStation.Tools;

namespace PlayStation.Managers
{
    public class MessageManager
    {
        private readonly IWebManager _webManager;

        public MessageManager(IWebManager webManager)
        {
            _webManager = webManager;
        }

        public MessageManager()
            : this(new WebManager())
        {
        }

        public async Task<Result> GetMessageGroup(string username, UserAuthenticationEntity userAuthenticationEntity, string region = "jp", string language = "ja")
        {
            var url = string.Format(EndPoints.MessageGroup, region, username, language);
            url += "&r=" + Guid.NewGuid();
            return await _webManager.GetData(new Uri(url), userAuthenticationEntity);
        }

        public async Task<Result> ClearMessages(string messageGroupId, List<int> messageUids, UserAuthenticationEntity userAuthenticationEntity, string region = "jp")
        {
            var url = string.Format(EndPoints.ClearMessages, region, messageGroupId, string.Join(",", messageUids));
            var json = new StringContent("{\"seenFlag\":true}", Encoding.UTF8, "application/json");
            return await _webManager.PutData(new Uri(url), json, userAuthenticationEntity);
        }

        public async Task<Result> GetGroupConversation(string messageGroupId,
            UserAuthenticationEntity userAuthenticationEntity, string region = "jp", string language = "ja")
        {
            var url = string.Format(EndPoints.MessageGroup2, region, messageGroupId, language);
            url += "&r=" + Guid.NewGuid();
            return await _webManager.GetData(new Uri(url), userAuthenticationEntity);
        }

        public async Task<Result> DeleteMessageThread(string messageGroupId, string onlineId,
           UserAuthenticationEntity userAuthenticationEntity, string region = "jp")
        {
            var url = string.Format(EndPoints.DeleteThread, region, messageGroupId, onlineId);
            url += "&r=" + Guid.NewGuid();
            return await _webManager.DeleteData(new Uri(url), null, userAuthenticationEntity);
        }

        public async Task<Result> CreatePost(string messageUserId, string post,
            UserAuthenticationEntity userAuthenticationEntity, string region = "jp")
        {
            var url = string.Format(EndPoints.CreatePost, region, messageUserId);
            const string boundary = "abcdefghijklmnopqrstuvwxyz";
            var messageJson = new SendMessage
            {
                message = new Message()
                {
                    body = post,
                    fakeMessageUid = 1384958573288,
                    messageKind = 1
                }
            };

            var json = JsonConvert.SerializeObject(messageJson);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            stringContent.Headers.Add("Content-Description", "message");
            var form = new MultipartContent("mixed", boundary) { stringContent };
            return await _webManager.PostData(new Uri(url), form, userAuthenticationEntity);
        }

        public async Task<Result> CreatePostWithMedia(string messageUserId, string post, string path, byte[] fileStream,
            UserAuthenticationEntity userAuthenticationEntity, string region = "jp")
        {
            var url = string.Format(EndPoints.CreatePost, region, messageUserId);
            const string boundary = "abcdefghijklmnopqrstuvwxyz";
            var messageJson = new SendMessage
            {
                message = new Message()
                {
                    body = post,
                    fakeMessageUid = 1384958573288,
                    messageKind = 1
                }
            };

            var json = JsonConvert.SerializeObject(messageJson);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            stringContent.Headers.Add("Content-Description", "message");
            var form = new MultipartContent("mixed", boundary) { stringContent };

            Stream stream = new MemoryStream(fileStream);
            var t = new StreamContent(stream);
            var s = Path.GetExtension(path);
            if (s != null && s.Equals(".png"))
            {
                t.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            }
            else
            {
                var extension = Path.GetExtension(path);
                if (extension != null && (extension.Equals(".jpg") || extension.Equals(".jpeg")))
                {
                    t.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                }
                else
                {
                    t.Headers.ContentType = new MediaTypeHeaderValue("image/gif");
                }
            }
            t.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            t.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            t.Headers.Add("Content-Description", "image-data-0");
            t.Headers.Add("Content-Transfer-Encoding", "binary");
            t.Headers.ContentLength = stream.Length;
            form.Add(t);

            return await _webManager.PostData(new Uri(url), form, userAuthenticationEntity);
        }

        public class SendMessage
        {
            public Message message { get; set; }
        }

        public class Message
        {
            public string body { get; set; }

            public long fakeMessageUid { get; set; }

            public int messageKind { get; set; }

            public int messageUid { get; set; }
        }
    }
}
