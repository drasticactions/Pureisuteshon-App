using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlayStation_App.Core.Entities;
using PlayStation_App.Core.Entities.Message;
using PlayStation_App.Core.Entities.User;
using PlayStation_App.Core.Interfaces;
using PlayStation_App.Core.Tools;

namespace PlayStation_App.Core.Managers
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


        public async Task<MessageGroupEntity> GetMessageGroup(string username, UserAccountEntity userAccountEntity)
        {
            try
            {
                var user = userAccountEntity.GetUserEntity();
                var url = string.Format(EndPoints.MessageGroup, user.Region, username, user.Language);
                url += "&r=" + Guid.NewGuid();
                var result = await _webManager.GetData(new Uri(url), userAccountEntity);
                var messageGroup = JsonConvert.DeserializeObject<MessageGroupEntity>(result.ResultJson);
                return messageGroup;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<Stream> GetImageMessageContent(string id, MessageEntity.Message message,
            UserAccountEntity userAccountEntity)
        {
            try
            {
                var user = userAccountEntity.GetUserEntity();
                const string content = "image-data-0";
                var url = string.Format(EndPoints.MessageContent, user.Region, id, message.messageUid, content, user.Language);
                url += "&r=" + Guid.NewGuid();
                var theAuthClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
                var response = await theAuthClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStreamAsync();
                return responseContent;
            }
            catch (Exception ex)
            {

                throw new Exception("Failed to get image message content", ex);
            }
        }

        public async Task<bool> ClearMessages(MessageEntity messageEntity, UserAccountEntity userAccountEntity)
        {
            try
            {
                if (messageEntity.messages == null)
                {
                    return false;
                }
                var user = userAccountEntity.GetUserEntity();
                var messageUids = new List<int>();
                messageUids.AddRange(messageEntity.messages.Where(o => o.seenFlag == false).Select(message => message.messageUid));
                if (messageUids.Count == 0) return false;
                var url = string.Format(EndPoints.ClearMessages, user.Region, messageEntity.messageGroup.messageGroupId, string.Join(",", messageUids));
                var json = new StringContent("{\"seenFlag\":true}", Encoding.UTF8, "application/json");
                var result = await _webManager.PutData(new Uri(url), json, userAccountEntity);
                return result.IsSuccess;
            }
            catch (Exception ex)
            {

                throw new Exception("Failed to clear messages", ex);
            }
        }

        public async Task<MessageEntity> GetGroupConversation(string messageGroupId, UserAccountEntity userAccountEntity)
        {
            try
            {
                var user = userAccountEntity.GetUserEntity();
                var url = string.Format(EndPoints.MessageGroup2, user.Region, messageGroupId, user.Language);
                url += "&r=" + Guid.NewGuid();
                var result = await _webManager.GetData(new Uri(url), userAccountEntity);
                var messageGroup = JsonConvert.DeserializeObject<MessageEntity>(result.ResultJson);
                return messageGroup;
            }
            catch (Exception ex)
            {

                throw new Exception("Failed to get the group conversation", ex);
            }
        }

        public async Task<bool> DeleteMessageThread(string messageUserId, UserAccountEntity userAccountEntity)
        {
            try
            {
                var user = userAccountEntity.GetUserEntity();
                var url = string.Format(EndPoints.DeleteThread, user.Region, messageUserId, user.OnlineId);
                var result = await _webManager.DeleteData(new Uri(url), null, userAccountEntity);
                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to remove message thread", ex);
            }
        }

        public async Task<bool> CreatePost(string messageUserId, string post,
            UserAccountEntity userAccountEntity)
        {
            try
            {
                var user = userAccountEntity.GetUserEntity();
                var url = string.Format(EndPoints.CreatePost, user.Region, messageUserId);
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
                var result = await _webManager.PostData(new Uri(url), form, userAccountEntity);
                return result.IsSuccess;
            }
            catch (Exception ex)
            {

                throw new Exception("Failed to send post", ex);
            }
        }

        public async Task<bool> CreatePostWithMedia(string messageUserId, string post, String path, byte[] fileStream,
            UserAccountEntity userAccountEntity)
        {
            try
            {
                var user = userAccountEntity.GetUserEntity();
                var url = string.Format(EndPoints.CreatePost, user.Region, messageUserId);
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

                var result = await _webManager.PostData(new Uri(url), form, userAccountEntity);
                return result.IsSuccess;
            }
            catch (Exception ex)
            {

                throw new Exception("Failed to send post (with media)", ex);
            }
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
