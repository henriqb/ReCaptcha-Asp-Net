/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2015 - 2017 Henrique B. Behr
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
using hbehr.recaptcha.Exceptions;
using NUnit.Framework;
using System;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Web;

namespace hbehr.recaptcha.unittest
{
    [TestFixture]
    public class UnitTests : TimedTests
    {
        private const string SiteKey = "6LcPoQoTAAAAAGCwybPpDBHx3-NZ73HafE-shOaw", SecretKey = "6LcPoQoTAAAAABDWAO5QneIrEigl9aqFGJ_AUiGV";
        private const string TestProxyIp = "177.55.254.113"; private const int PortProxy = 8080;/// Working on 27/05/2017 http://www.freeproxylists.net/121.8.98.201.html

        [SetUp]
        public void ResetTest()
        {
            ReCaptcha.ResetConfiguration();
        }

        [Test]
        public void AssertTestWillConectAndFailInvalidUserAnswer()
        {
            ReCaptcha.Configure(SiteKey, SecretKey);
            Assert.Throws<ReCaptchaException>(() => ReCaptcha.ValidateCaptcha("resposta-fajuta"));
        }

        [Test]
        public void AssertTestWillConectAndFailInvalidUserAnswerWithProxy()
        {
            ReCaptcha.Configure(SiteKey, SecretKey);
            Assert.Throws<ReCaptchaException>(() => ReCaptcha.ValidateCaptcha("resposta-fajuta", new WebProxy(TestProxyIp, PortProxy)));
        }

        [Test]
        public void AssertTestWillConectAndFailInvalidUserAnswerAsync()
        {
            Assert.Throws<ReCaptchaException>(() =>
            {
                try
                {
                    ReCaptcha.Configure(SiteKey, SecretKey);
                    var task = ReCaptcha.ValidateCaptchaAsync("resposta-fajuta");

                    while (task.IsCompleted == false)
                    {
                        Thread.Sleep(1);
                    }

                    var answer = task.Result;
                    Assert.IsFalse(answer);
                }
                catch (AggregateException e)
                {
                    throw e.InnerException;
                }
            });
        }

        [Test]
        public void AssertTestWillConectAndFailInvalidUserAnswerAsyncWithProxy()
        {
            Assert.Throws<ReCaptchaException>(() =>
            {
                try
                {
                    ReCaptcha.Configure(SiteKey, SecretKey);
                    var task = ReCaptcha.ValidateCaptchaAsync("resposta-fajuta", new WebProxy(TestProxyIp, PortProxy));

                    while (task.IsCompleted == false)
                    {
                        Thread.Sleep(1);
                    }

                    var answer = task.Result;
                    Assert.IsFalse(answer);
                }
                catch (AggregateException e)
                {
                    throw e.InnerException;
                }
            });
        }

        [Test]
        public void ExceptionWhenNotConfigured()
        {
            Assert.Throws<ReCaptchaException>(() => ReCaptcha.GetCaptcha());
        }

        [Test]
        public void InvalidSecretKeyException()
        {
            Assert.Throws<ReCaptchaException>(() =>
            {
                ReCaptcha.Configure("something", "Invalid-Secret-Key");
                bool answer = ReCaptcha.ValidateCaptcha("resposta-fajuta");
            });
        }

        [Test]
        public void WrongSiteKeyArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() => ReCaptcha.Configure("", "something"));
        }

        [Test]
        public void WrongSecretKeyArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() => ReCaptcha.Configure("something", null));
        }

        [Test]
        public void AssertScriptDivIsCorrect()
        {
            ReCaptcha.Configure("my-public-key", "my-secret-key");
            IHtmlString captcha = ReCaptcha.GetCaptcha();
            string captchaString = captcha.ToHtmlString();
            Assert.AreEqual("<div class='g-recaptcha' data-sitekey='my-public-key'></div><script src='https://www.google.com/recaptcha/api.js'></script>", captchaString);
        }

        [Test]
        public void AssertScriptDivIsCorrectWithLanguage()
        {
            ReCaptcha.Configure("my-public-key", "my-secret-key", ReCaptchaLanguage.German);
            IHtmlString captcha = ReCaptcha.GetCaptcha();
            string captchaString = captcha.ToHtmlString();
            Assert.AreEqual("<div class='g-recaptcha' data-sitekey='my-public-key'></div><script src='https://www.google.com/recaptcha/api.js?hl=de'></script>", captchaString);
        }

        [Test]
        public void AssertScriptDivIsCorrectWithAutoLanguage()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("no");
            ReCaptcha.Configure("my-public-key", "my-secret-key", ReCaptchaLanguage.Auto);

            IHtmlString captcha = ReCaptcha.GetCaptcha();
            string captchaString = captcha.ToHtmlString();
            Assert.AreEqual("<div class='g-recaptcha' data-sitekey='my-public-key'></div><script src='https://www.google.com/recaptcha/api.js?hl=no'></script>", captchaString);

            captcha = ReCaptcha.GetCaptcha(ReCaptchaLanguage.PortugueseBrazil);
            captchaString = captcha.ToHtmlString();
            Assert.AreEqual("<div class='g-recaptcha' data-sitekey='my-public-key'></div><script src='https://www.google.com/recaptcha/api.js?hl=pt-BR'></script>", captchaString);

            Thread.CurrentThread.CurrentUICulture = new CultureInfo("nl");
            captcha = ReCaptcha.GetCaptcha();
            captchaString = captcha.ToHtmlString();
            Assert.AreEqual("<div class='g-recaptcha' data-sitekey='my-public-key'></div><script src='https://www.google.com/recaptcha/api.js?hl=nl'></script>", captchaString);
        }

        [Test]
        public void AssertScriptDivIsCorrectWithLanguageOverrideConfiguration()
        {
            ReCaptcha.Configure("my-public-key", "my-secret-key", ReCaptchaLanguage.EnglishUs);
            IHtmlString captcha = ReCaptcha.GetCaptcha(ReCaptchaLanguage.PortugueseBrazil);
            string captchaString = captcha.ToHtmlString();
            Assert.AreEqual("<div class='g-recaptcha' data-sitekey='my-public-key'></div><script src='https://www.google.com/recaptcha/api.js?hl=pt-BR'></script>", captchaString);
        }

        [Test]
        public void AssertScriptDivIsCorrectWithCallbacks()
        {
            ReCaptcha.Configure("my-public-key", "my-secret-key");
            IHtmlString captcha1 = ReCaptcha.GetCaptcha(callback: "callback", expiredCallback: "expiredCallback", errorCallback: "errorCallback");
            Assert.AreEqual("<div class='g-recaptcha' data-sitekey='my-public-key' data-callback='callback' data-expired-callback='expiredCallback' data-error-callback='errorCallback'></div><script src='https://www.google.com/recaptcha/api.js'></script>", captcha1.ToHtmlString());

            IHtmlString captcha2 = ReCaptcha.GetCaptcha(callback: "callback", errorCallback: "errorCallback");
            Assert.AreEqual("<div class='g-recaptcha' data-sitekey='my-public-key' data-callback='callback' data-error-callback='errorCallback'></div><script src='https://www.google.com/recaptcha/api.js'></script>", captcha2.ToHtmlString());

            IHtmlString captcha3 = ReCaptcha.GetCaptcha(callback: "callback", expiredCallback: "expiredCallback");
            Assert.AreEqual("<div class='g-recaptcha' data-sitekey='my-public-key' data-callback='callback' data-expired-callback='expiredCallback'></div><script src='https://www.google.com/recaptcha/api.js'></script>", captcha3.ToHtmlString());

            IHtmlString captcha4 = ReCaptcha.GetCaptcha(callback: "callback");
            Assert.AreEqual("<div class='g-recaptcha' data-sitekey='my-public-key' data-callback='callback'></div><script src='https://www.google.com/recaptcha/api.js'></script>", captcha4.ToHtmlString());

            IHtmlString captcha5 = ReCaptcha.GetCaptcha(expiredCallback: "expiredCallback", errorCallback: "errorCallback");
            Assert.AreEqual("<div class='g-recaptcha' data-sitekey='my-public-key' data-expired-callback='expiredCallback' data-error-callback='errorCallback'></div><script src='https://www.google.com/recaptcha/api.js'></script>", captcha5.ToHtmlString());

            IHtmlString captcha6 = ReCaptcha.GetCaptcha(expiredCallback: "expiredCallback");
            Assert.AreEqual("<div class='g-recaptcha' data-sitekey='my-public-key' data-expired-callback='expiredCallback'></div><script src='https://www.google.com/recaptcha/api.js'></script>", captcha6.ToHtmlString());

            IHtmlString captcha7 = ReCaptcha.GetCaptcha(errorCallback: "errorCallback");
            Assert.AreEqual("<div class='g-recaptcha' data-sitekey='my-public-key' data-error-callback='errorCallback'></div><script src='https://www.google.com/recaptcha/api.js'></script>", captcha7.ToHtmlString());
        }

        [Test]
        public void AssertScriptDivIsCorrectWithCallbacksAndLanguage()
        {
            ReCaptcha.Configure("my-public-key", "my-secret-key");
            IHtmlString captcha = ReCaptcha.GetCaptcha(ReCaptchaLanguage.Spanish, "callback", "expiredCallback", "errorCallback");
            Assert.AreEqual("<div class='g-recaptcha' data-sitekey='my-public-key' data-callback='callback' data-expired-callback='expiredCallback' data-error-callback='errorCallback'></div><script src='https://www.google.com/recaptcha/api.js?hl=es'></script>", captcha.ToHtmlString());
        }

        [Test]
        public void AssertScriptInvisibleDivIsCorrectWithLanguageOverrideConfiguration()
        {
            ReCaptcha.Configure("my-public-key", "my-secret-key", ReCaptchaLanguage.EnglishUs);
            IHtmlString captcha = ReCaptcha.GetInvisibleCaptcha("callback", "SUBMIT", ReCaptchaLanguage.PortugueseBrazil);
            string captchaString = captcha.ToHtmlString();
            Assert.AreEqual("<button class='g-recaptcha' data-sitekey='my-public-key' data-callback='callback'>SUBMIT</button><script src='https://www.google.com/recaptcha/api.js?hl=pt-BR'></script>", captchaString);
        }

        [Test]
        public void AssertScriptInvisibleDivIsCorrectWithAdditionalClassesOverrideConfiguration()
        {
            ReCaptcha.Configure("my-public-key", "my-secret-key", ReCaptchaLanguage.EnglishUs);
            IHtmlString captcha = ReCaptcha.GetInvisibleCaptcha("callback", "SUBMIT", additionalClasses: new[] { "btn", "btn-autosize" });
            string captchaString = captcha.ToHtmlString();
            Assert.AreEqual("<button class='g-recaptcha btn btn-autosize' data-sitekey='my-public-key' data-callback='callback'>SUBMIT</button><script src='https://www.google.com/recaptcha/api.js?hl=en'></script>", captchaString);
        }
    }
}