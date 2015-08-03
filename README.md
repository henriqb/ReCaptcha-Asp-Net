# ReCaptcha-Asp-Net
Google ReCaptcha for Asp Net, simplified

## Instalation
Nuget Url:
https://www.nuget.org/packages/ReCaptcha-AspNet/1.0.0

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

Or via C# code:
It's only needed to call it once, a good place to put this is on inside Application_Start() function 
```C#
string publicKey = "...[public-key]"
string secretKey = "...[secret-key]"
ReCaptcha.Configure(publicKey, secretKey)
```

## How to use

### Client Side:
Inside your form
```html
<form action="myAction">
  <input type="text" name="myinput1" />
  @ReCaptcha.GetCaptcha()
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
