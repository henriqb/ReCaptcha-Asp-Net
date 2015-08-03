/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2015 Henrique B. Behr
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
using System;
using System.Threading;
using System.Web;
using hbehr.recaptcha.Exceptions;
using NUnit.Framework;

namespace hbehr.recaptcha.unittest
{
    [TestFixture]
    public class UnitTests : TimedTests
    {
        private const string SiteKey = "6LcPoQoTAAAAAGCwybPpDBHx3-NZ73HafE-shOaw", SecretKey = "6LcPoQoTAAAAABDWAO5QneIrEigl9aqFGJ_AUiGV";
        
        [SetUp]
        public void ResetTest()
        {
            ReCaptcha.ResetConfiguration();
        }

        [Test]
        [ExpectedException(typeof(ReCaptchaException))]
        public void AssertTestWillConectAndFailInvalidUserAnswer()
        {
            ReCaptcha.Configure(SiteKey, SecretKey);
            bool answer = ReCaptcha.ValidateCaptcha("resposta-fajuta");
            Assert.IsFalse(answer);
        }

        [Test]
        [ExpectedException(typeof(ReCaptchaException))]
        public void AssertTestWillConectAndFailInvalidUserAnswerAsync()
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
        }

        [Test]
        [ExpectedException(typeof(ReCaptchaException))]
        public void ExceptionWhenNotConfigured()
        {
            ReCaptcha.GetCaptcha();
        }

        [Test]
        [ExpectedException(typeof(ReCaptchaException))]
        public void InvalidSecretKeyException()
        {
            ReCaptcha.Configure("something", "Invalid-Secret-Key");
            bool answer = ReCaptcha.ValidateCaptcha("resposta-fajuta");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WrongSiteKeyArgumentException()
        {
            ReCaptcha.Configure("", "something");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WrongSecretKeyArgumentException()
        {
            ReCaptcha.Configure("something", null);
        }

        [Test]
        public void AssertScriptDivIsCorrect()
        {
            ReCaptcha.Configure("my-public-key", "my-secret-key");
            IHtmlString captcha = ReCaptcha.GetCaptcha();
            string captchaString = captcha.ToHtmlString();
            Assert.AreEqual("<div class='g-recaptcha' data-sitekey='my-public-key'></div><script src='https://www.google.com/recaptcha/api.js'></script>", captchaString);
        }

    }
}
