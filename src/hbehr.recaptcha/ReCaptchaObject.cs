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
using hbehr.recaptcha.Internazionalization;
using hbehr.recaptcha.WebInterface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace hbehr.recaptcha
{
    internal class ReCaptchaObject
    {
        private string _captchaDiv, _invisibleCaptchaDiv, _secretKey, _language;
        private ReCaptchaLanguage? _defaultLanguage;
        private ReCaptchaTheme _defaultTheme;
        private bool _configured;
        
        internal ReCaptchaObject()
        {
            // Auto .config configuration
            var reader = new AppSettingsReader();
            try
            {
                string secretKey = reader.GetValue("recaptcha-secret-key", typeof(string)).ToString();
                string publicKey = reader.GetValue("recaptcha-public-key", typeof(string)).ToString();

                Initialize(publicKey, secretKey);
            }
            catch
            {
                // No configuration on .config
            }
            try
            {
                _language = reader.GetValue("recaptcha-language-key", typeof(string)).ToString();
                if ("AUTO".Equals(_language, StringComparison.InvariantCultureIgnoreCase))
                {
                    _defaultLanguage = ReCaptchaLanguage.Auto;
                }
            }
            catch
            {
                // No language on .config
            }

            _defaultTheme = ReCaptchaTheme.light;
            try
            {
                var theme = reader.GetValue("recaptcha-language-theme", typeof(string)).ToString();
                if ("dark".Equals(theme))
                {
                    _defaultTheme = ReCaptchaTheme.dark;
                }
            }
            catch
            {
                // No language on .config
            }
        }

        internal ReCaptchaObject(string publicKey, string secretKey, ReCaptchaLanguage? defaultLanguage = null, ReCaptchaTheme theme = ReCaptchaTheme.light)
        {
            Initialize(publicKey, secretKey, defaultLanguage, theme);
        }

        private void Initialize(string publicKey, string secretKey, ReCaptchaLanguage? defaultLanguage = null, ReCaptchaTheme theme = ReCaptchaTheme.light)
        {
            if (string.IsNullOrWhiteSpace(publicKey))
            {
                throw new ArgumentNullException("publicKey");
            }
            if (string.IsNullOrWhiteSpace(secretKey))
            {
                throw new ArgumentNullException("secretKey");
            }
            _defaultLanguage = defaultLanguage;
            if (defaultLanguage.HasValue)
            {
                _language = defaultLanguage.Value.GetLanguage();
            }
            _defaultTheme = theme;
            _configured = true;
            _secretKey = secretKey;
            _captchaDiv = string.Format("<div class='g-recaptcha' data-sitekey='{0}'{{1}} data-theme='{{2}}'></div><script src='https://www.google.com/recaptcha/api.js{{0}}'></script>", publicKey);
            _invisibleCaptchaDiv = string.Format("<button class='{{1}}' data-sitekey='{0}' data-callback='{{2}}'>{{3}}</button><script src='https://www.google.com/recaptcha/api.js{{0}}'></script>", publicKey);
        }

        private string GetHlCode(ReCaptchaLanguage? language)
        {
            string strLang = language.HasValue ? language.Value.GetLanguage() : (_defaultLanguage == ReCaptchaLanguage.Auto ? 
                _defaultLanguage.Value.GetLanguage() : _language);
            return string.IsNullOrWhiteSpace(strLang) ? "" : "?hl=" + strLang;
        }

        private void CheckIfIamConfigured()
        {
            if (_configured) { return; }
            throw new ReCaptchaException("ReCaptcha is not configured. Get your site and secret keys from google. And call function ReCaptcha.Configure(publicKey, secretKey), or add the keys to the .config file <add key='recaptcha-public-key' value='...' /><add key='recaptcha-site-key' value='...'/>");
        }

        internal IHtmlString GetCaptcha(ReCaptchaLanguage? language, string callback, string expiredCallback, string errorCallback, ReCaptchaTheme? theme)
        {
            CheckIfIamConfigured();
            return new HtmlString(string.Format(_captchaDiv, GetHlCode(language), GetCaptchaDataCallbacks(callback, expiredCallback, errorCallback), theme ?? _defaultTheme));
        }

        internal IHtmlString GetInvisibleCaptcha(string callback, string buttonText, ReCaptchaLanguage? language, IEnumerable<string> additionalClasses)
        {
            CheckIfIamConfigured();
            var classes = new List<string> {"g-recaptcha"};
            if (additionalClasses != null)
            {
                classes.AddRange(additionalClasses);
            }
            return new HtmlString(string.Format(_invisibleCaptchaDiv, GetHlCode(language), string.Join(" ", classes), callback, buttonText));
        }

        internal bool ValidateResponse(IReChaptaWebInterface webInterface, string response)
        {
            CheckIfIamConfigured();
            var answer = webInterface.PostUserAnswer(response, _secretKey);
            TreatReCaptchaError(answer);
            return answer.Success;
        }

        internal bool ValidateResponse(IReChaptaWebInterface webInterface, string response, WebProxy proxy)
        {
            CheckIfIamConfigured();
            var answer = webInterface.PostUserAnswer(response, _secretKey, proxy);
            TreatReCaptchaError(answer);
            return answer.Success;
        }

        internal async Task<bool> ValidateResponseAsync(IReChaptaWebInterfaceAsync webInterface, string response)
        {
            CheckIfIamConfigured();
            var answer = await webInterface.PostUserAnswerAsync(response, _secretKey);
            TreatReCaptchaError(answer);
            return answer.Success;
        }

        internal async Task<bool> ValidateResponseAsync(IReChaptaWebInterfaceAsync webInterface, string response, WebProxy proxy)
        {
            CheckIfIamConfigured();
            var answer = await webInterface.PostUserAnswerAsync(response, _secretKey, proxy);
            TreatReCaptchaError(answer);
            return answer.Success;
        }

        private static string GetCaptchaDataCallbacks(string callback, string expiredCallback, string errorCallback)
        {
            var dataCallbacks = new List<string>();

            if (!string.IsNullOrWhiteSpace(callback))
                dataCallbacks.Add($"data-callback='{callback}'");

            if (!string.IsNullOrWhiteSpace(expiredCallback))
                dataCallbacks.Add($"data-expired-callback='{expiredCallback}'");

            if (!string.IsNullOrWhiteSpace(errorCallback))
                dataCallbacks.Add($"data-error-callback='{errorCallback}'");

            return dataCallbacks.Any() ? $" {string.Join(" ", dataCallbacks)}" : string.Empty;
        }

        private static void TreatReCaptchaError(ReCaptchaJsonResponse answer)
        {
            var error = new ReCaptchaError(answer.ErrorCodes);

            if (error.InvalidInputSecret)
            {
                throw new ReCaptchaException("Invalid ReCaptcha Secret Key !");
            }
            if (error.InvalidInputResponse)
            {
                throw new ReCaptchaException("Invalid Input Response, make sure you are passing correctly the user answer from the Captcha.");
            }
        }
    }
}