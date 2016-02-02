# PSX.Appx
### A PSX Client for Windows 10

[Download from the Windows Store](https://www.microsoft.com/en-us/store/apps/psxappx/9nblggh6cvbj)

PSX.Appx is a UWP app that lets you connect to the psx on any of your devices running Windows 10. Right now it's pretty basic, supporting the following.

![Trophy list](http://i.imgur.com/Nfftuj6.png "Trophy List")

* Viewing Recent Activity Feeds
* Messaging friends via text
* Viewing and comparing trophy lists
* Viewing and searching what's new on on UStream, NicoVideo, and Twitch
* Making new friend request links, to be sent via SMS or Email, or shared elsewhere.
* Able to log in with multiple accounts.
* Translated into English and Japanese

There are many more things it _could_ do in the future, such as 

* Support adding and viewing events
* Sending messages with text and voice
* Notifications (Not push, no way to do that via their API)
* Game Invites
* Communities (once available on the mobile app, it's not public yet)
* Translate into more languages

![Trophy list](http://i.imgur.com/JW1x3HB.png "Trophy List")

### The Basics

This app is seperated into two parts, the core library and the UWP app. The core library is a PCL targeting Windows 10 and (once I remove some SQLite dependencies) ASP.NET Core. This library manages the API calls made to the psx.

### Authentication

Authentication is handled in the [Authentication Manager](https://github.com/drasticactions/PSX-App/blob/master/PSX/Managers/AuthenticationManager.cs). Normally, in the official psx apps on iOS and Android, there is an OAuth 2 based dance between a webview and the client, passing an oauth token back and forth between the webview and the client. Now, this works great for their app, but it's a pain for any other app trying to gain proper credentials, because they except a specific return URI to be on the query string to launch their app on the client. We don't want that.

So I basically brute forced my way around their system by matching the calls their view expects, so I can fake being their page. Then I can parse out the Oauth code from their return URI and then finish the handoff myself. The user just has to enter their username and password, and the library takes care of the rest.

### Tokens

Once authenticated, their access and refresh tokens are stored in a SQlite database on the client. Any time a psx function is accessed, a user authentication object is passed along with the request. Should the token need refreshing, it can be refreshed along side the users request, and then passed back to the client.

Passwords are not stored on the client, only Oauth tokens.

### Contribute

I am open to any and all pull requests! If you happen to know more, or better, ways to handle these private APIs I'm open to suggestions. If you would like to translate the app, by all means do so. Any issues can be files here on Github as well.

