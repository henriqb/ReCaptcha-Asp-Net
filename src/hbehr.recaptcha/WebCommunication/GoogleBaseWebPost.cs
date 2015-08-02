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
using System.Net;
using System.Web;

namespace hbehr.recaptcha.WebCommunication
{
    internal class GoogleBaseWebPost
    {
        protected const string GoogleRecapthcaUrl = "https://www.google.com/recaptcha/api/siteverify";

        protected string GetPostData(string response, string secretKey)
        {
            if (HttpContext.Current != null)
            {
                return string.Format("secret={0}&response={1}&remoteip={2}", secretKey, response,
                    HttpContext.Current.Request.UserHostAddress);
            }
            // For testing purpouses
            return string.Format("secret={0}&response={1}", secretKey, response);
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