using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Networking.Connectivity;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using NotificationsExtensions.Tiles;
using NotificationsExtensions.Toasts;
using PlayStation_App.Models.RecentActivity;

namespace Pureisuteshon.Notifications
{
    public class NotifyStatusTile
    {
        public static bool IsInternet()
        {

#if DEBUG
            return true;
#endif
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            bool internet = connections != null &&
                            connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            return internet;
        }

        public static void CreateRecentActvityLiveTile(Feed feed)
        {
            TileBindingContentAdaptive bindingContent = new TileBindingContentAdaptive()
            {
                
                Children =
                {
                    new TileText()
                    {
                        Text = feed.Caption,
                        Style = TileTextStyle.Body
                    }
                }
            };
            if (feed.SmallImageUrl != null || feed.LargeImageUrl != null)
            {
                bindingContent.PeekImage = new TilePeekImage()
                {
                    Source =
                        new TileImageSource(!string.IsNullOrEmpty(feed.LargeImageUrl)
                            ? feed.LargeImageUrl
                            : feed.SmallImageUrl)
                };
            }
            var binding = new TileBinding()
            {
                Branding = TileBranding.NameAndLogo,
                Content = bindingContent
            };
            var content = new TileContent()
            {
                Visual = new TileVisual()
                {
                    TileMedium = binding,
                    TileWide = binding,
                    TileLarge = binding
                }
            };
            var tileXml = content.GetXml();
            var tileNotification = new TileNotification(tileXml);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
        }

        //public static void CreateToastNotification(Thread forumThread)
        //{
        //    string replyText = forumThread.RepliesSinceLastOpened > 1 ? " has {0} replies." : " has {0} reply.";
        //    string test = "{" + string.Format("type:'toast', 'threadId':{0}", forumThread.ThreadId) + "}";
        //    ToastContent content = new ToastContent()
        //    {
        //        Launch = test,
        //        Visual = new ToastVisual()
        //        {
        //            TitleText = new ToastText()
        //            {
        //                Text = string.Format("\"{0}\"", forumThread.Name)
        //            },
        //            BodyTextLine1 = new ToastText()
        //            {
        //                Text = string.Format(replyText, forumThread.RepliesSinceLastOpened)
        //            },
        //            AppLogoOverride = new ToastAppLogo()
        //            {
        //                Source = new ToastImageSource(forumThread.ImageIconLocation)
        //            }
        //        },
        //        Actions = new ToastActionsCustom()
        //        {
        //            Buttons =
        //            {
        //                new ToastButton("Open Thread", test)
        //                {
        //                    ActivationType = ToastActivationType.Foreground
        //                },
        //                new ToastButton("Sleep", "sleep")
        //                {
        //                    ActivationType = ToastActivationType.Background
        //                }
        //            }
        //        },
        //        Audio = new ToastAudio()
        //        {
        //            Src = new Uri("ms-winsoundevent:Notification.Reminder")
        //        }
        //    };

        //    XmlDocument doc = content.GetXml();

        //    var toastNotification = new ToastNotification(doc);
        //    var nameProperty = toastNotification.GetType().GetRuntimeProperties().FirstOrDefault(x => x.Name == "Tag");
        //    nameProperty?.SetValue(toastNotification, forumThread.ThreadId.ToString());
        //    ToastNotificationManager.CreateToastNotifier().Show(toastNotification);
        //}

        public static void CreateToastNotification(string header, string text)
        {
            XmlDocument notificationXml =
    ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText02);
            XmlNodeList toastElements = notificationXml.GetElementsByTagName("text");
            toastElements[0].AppendChild(
                notificationXml.CreateTextNode(header));
            toastElements[1].AppendChild(
                 notificationXml.CreateTextNode(text));
            XmlNodeList imageElement = notificationXml.GetElementsByTagName("image");
            string imageName = string.Empty;
            if (string.IsNullOrEmpty(imageName))
            {
                imageName = @"Assets/Logo.scale-100.png";
            }
            imageElement[0].Attributes[1].NodeValue = imageName;
            IXmlNode toastNode = notificationXml.SelectSingleNode("/toast");
            string test = "{" + string.Format("type:'toast'") + "}";
            var xmlElement = (XmlElement)toastNode;
            xmlElement?.SetAttribute("launch", test);
            var toastNotification = new ToastNotification(notificationXml)
            {
                ExpirationTime = DateTimeOffset.UtcNow.AddSeconds(30)
            };
            ToastNotificationManager.CreateToastNotifier().Show(toastNotification);
        }
    }
}
