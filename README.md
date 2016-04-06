# Malone

Malone is a tool to test Web APIs and is inspired by [Postman](https://www.getpostman.com/).

![Screenshot](http://i.imgur.com/sC0gogt.png)

## Comparison with Postman

Postman is awesome, seriously, you should probably [just go and use that](https://www.getpostman.com/) if you can. Due to various factors we couldn't use Postman in our organization so we built something custom enough for our internal needs, but general enough to possibly be useful to others.

The following comparison were factors at the time of creating Malone, and may no longer be relevant.

|Postman|Malone|
|-|
|Started life as a [Chrome browser extension](https://github.com/postmanlabs/postman-chrome-extension-legacy), then became a closed source packaged app.|Open source.|
|Cross-platform.|Windows only.|
|Had a few limitations because Chrome wouldn't let you override certain things like the `User-Agent` header. This may be resolved as it looks like they have since moved to an Electron app.|Should hopefully have less limitations with access to lower-level .NET APIs.|
|There are way too many ways to do OAuth 2, but the supported way of sharing the `Client Secret` and redirecting through the Postman site for tokens was way too intrusive for our liking.|We couldn't wait for a response to the [feature request](https://github.com/postmanlabs/postman-app-support/issues/957) so built Malone.|
|Can handle a whole range of authentication methods, not just OAuth 2.|Currently only handles our flavor of OAuth 2. This may be the same as your flavor, but probably not.|
|Did we mention how awesome Postman is? Just go use it already!|What are you still doing here‽|

## Credits

### Name

Malone is named after one of the all time great NBA power forwards: [Karl Malone](https://en.wikipedia.org/wiki/Karl_Malone). It is a homage to Postman, as he earned the nickname "The Mailman" because he "always delivered".

### Logo
[Basketball](https://thenounproject.com/term/ball/73762/) by [Arthur Shlain](https://thenounproject.com/ArtZ91/) via [the Noun Project](https://thenounproject.com/), licenced under Creative Commons. [Professionally colored](http://i.imgur.com/SQzUurI.png?1) by **stajs**.

### Libraries

- https://github.com/Caliburn-Micro/Caliburn.Micro
- https://github.com/MahApps/MahApps.Metro
- https://github.com/restsharp/RestSharp
- https://github.com/Squirrel/Squirrel.Windows
- https://github.com/DotNetOpenAuth/DotNetOpenAuth
- https://github.com/icsharpcode/SharpZipLib
- https://github.com/icsharpcode/AvalonEdit
- https://github.com/jbevain/cecil/
