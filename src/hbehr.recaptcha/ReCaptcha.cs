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

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using hbehr.recaptcha.WebCommunication;

namespace hbehr.recaptcha
{
    public static class ReCaptcha
    {      
        private static ReCaptchaObject _reCaptcha = new ReCaptchaObject();

        public static void Configure(string publicKey, string secretKey)
        {
            _reCaptcha = new ReCaptchaObject(publicKey, secretKey);
        }

        public static void ResetConfiguration()
        {
            _reCaptcha = new ReCaptchaObject();
        }

        public static IHtmlString GetCaptcha()
        {
            return _reCaptcha.GetCaptcha();
        }

        public static bool ValidateCaptcha(string response)
        {
            return _reCaptcha.ValidateResponse(new GoogleWebPost(), response);
        }

#if !NET40
        public static async Task<bool> ValidateCaptchaAsync(string response)
        {
            return await _reCaptcha.ValidateResponseAsync(new GoogleWebPostAsync(), response);
        }
#endif
    }
}
