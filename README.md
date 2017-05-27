# ReCaptcha-Asp-Net
Google ReCaptcha for Asp Net, simplified

## Installation
Nuget Url:
https://www.nuget.org/packages/ReCaptcha-AspNet

```
PM > Install-Package ReCaptcha-AspNet
```

## Configuration 

Get your secret and public keys on https://www.google.com/recaptcha/admin 

Add to you App/Web.config inside <appSettings>
```xml
<add key="recaptcha-secret-key" value="...[secret key]" />
<add key="recaptcha-public-key" value="...[public-key]" />
```
Optional if you want to alter the default language of the Captcha (Get the language code on ReCaptcha site: https://developers.google.com/recaptcha/docs/language)
Or you can use value "Auto" and it will get the Language from System.Thread.CurrentCulture
```xml
<add key="recaptcha-language-key" value="[language-code]" />
```

Or via C# code:
It's only needed to call it once, a good place to put this is at Application_Start() function 
```C#
string publicKey = "...[public-key]";
string secretKey = "...[secret-key]";
ReCaptcha.Configure(publicKey, secretKey);

// Optional, select a default language:
ReCaptchaLanguage defaultLanguage = ReCaptchaLanguage.German;
ReCaptcha.Configure(publicKey, secretKey, defaultLanguage);

//Auto-select language from System.Thread.CurrentCulture
ReCaptchaLanguage defaultLanguage = ReCaptchaLanguage.Auto;
ReCaptcha.Configure(publicKey, secretKey, defaultLanguage);
```

## How to use

### Client Side (v2):
Inside your form 
```html
<form action="myAction">
  <input type="text" name="myinput1" />
  @ReCaptcha.GetCaptcha() <!-- Will show your captcha as your configured Language, 
  if no language is defined it will show ReCaptcha default language (English) -->
</form>
```

Optional if you want to override your configured default language: 
```html
<form action="myAction">
  <input type="text" name="myinput1" />
  @ReCaptcha.GetCaptcha(ReCaptchaLanguage.PortugueseBrazil) <!-- Will show your ReCaptcha as Portuguese, 
  overriding any previous configuration -->
</form>
```

### Client Side (Invisible):
Inside your form 
```html
<script type="text/javascript">function submit() { $('form').submit(); }</script>
<form action="myAction">
  <input type="text" name="myinput1" />
  @ReCaptcha.GetInvisibleCaptcha("submit", "Save") <!-- Will show a button, with a Label Save and  call function "submit();" after user click ok and pass Captcha -->
</form>
```

Optional if you want to override your configured default language: 
```html
<script type="text/javascript">function submit() { $('form').submit(); }</script>
<form action="myAction">
  <input type="text" name="myinput1" />
  @ReCaptcha.GetInvisibleCaptcha("submit", "Save", ReCaptchaLanguage.PortugueseBrazil) <!-- Will show your Invisible ReCaptcha as Portuguese, overriding any previous configuration -->
</form>
```

### Server Side:
Inside your controller function or in a filter
```C#
string userResponse = HttpContext.Request.Params["g-recaptcha-response"];
bool validCaptcha = ReCaptcha.ValidateCaptcha(userResponse);
if (validCaptcha){
  // Real User, validated !
  DoStuff();
  ...
} else {
  // Bot Attack, non validated !
  return RedirectToAction("YouAreARobot", "Index");
}
```

#### Optional: Proxy
You may use a Proxy to send user Response to ReCaptcha Server
```C#
...
const string proxyIp = "xxx.xxx.xxx.xxx";
const int proxyPort = 1234;
WebProxy webProxy = new WebProxy(proxyIp, proxyPort); 
bool validCaptcha = ReCaptcha.ValidateCaptcha(userResponse, webProxy);
...
``` 

May throws the following exception, if the secret key is invalid, or you pass a invalid user response as the ValidateCaptcha parameter:
```C#
throw new ReCaptchaException();
```

It can also be called async:
```C#
public async ActionResult MyFunction() {
  string userResponse = HttpContext.Request.Params["g-recaptcha-response"];
  var validCaptcha = ReCaptcha.ValidateCaptchaAsync(userResponse);
  DoSomeParallelStuff();
  
  if (await validCaptcha){
    // Real User, validated !
    DoStuff();
    ...
  } else {
    // Bot Attack, non validated !
    return RedirectToAction("YouAreARobot", "Index");
  }
}
```
