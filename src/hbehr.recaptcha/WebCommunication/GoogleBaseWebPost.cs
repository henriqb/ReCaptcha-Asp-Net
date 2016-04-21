/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2015 - 2016 Henrique B. Behr
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
using System.Net;
using System.Web;

namespace hbehr.recaptcha.WebCommunication
{
    internal class GoogleBaseWebPost
    {
        protected const string GoogleRecapthcaUrl = "https://www.google.com/recaptcha/api/siteverify";

        protected string GetPostData(string response, string secretKey)
        {
            // For testing purpouses, this shouldn't happened
            if (HttpContext.Current == null) return string.Format("secret={0}&response={1}", secretKey, response);

            string clientIp = GetClientIp();
            return string.Format("secret={0}&response={1}&remoteip={2}", secretKey, response, clientIp);
        }

        private string GetClientIp()
        {
            // Look for a proxy address first
            string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            // If there is no proxy, get the standard remote address
            if (!string.IsNullOrWhiteSpace(ip) && ip.ToLower() != "unknown")
            {
                return ip;
            }
            return HttpContext.Current.Request.UserHostAddress;
        }

        protected WebRequest CreateEmptyPostWebRequest()
        {
            var webRequest = WebRequest.Create(GoogleRecapthcaUrl);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            return webRequest;
        }
    }
}