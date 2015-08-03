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
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using hbehr.recaptcha.WebInterface;
using Newtonsoft.Json;

namespace hbehr.recaptcha.WebCommunication
{
#if !NET40
    internal class GoogleWebPostAsync : GoogleBaseWebPost, IReChaptaWebInterfaceAsync
    {
        public async Task<ReCaptchaJsonResponse> PostUserAnswerAsync(string response, string secretKey)
        {
            string postData = GetPostData(response, secretKey);
            var webRequest = CreatePostWebRequestAsync(postData);
            return await GetAnswerAsync(await webRequest);
        }

        private async Task<WebRequest> CreatePostWebRequestAsync(string postData)
        {
            var webRequest = CreateEmptyPostWebRequest();
            webRequest.ContentLength = postData.Length;

            using (var requestWriter = new StreamWriter(webRequest.GetRequestStream()))
            {
                await requestWriter.WriteAsync(postData);
            }
            return webRequest;
        }

        private async Task<ReCaptchaJsonResponse> GetAnswerAsync(WebRequest webRequest)
        {
            var webResponse = webRequest.GetResponseAsync();
            return JsonConvert.DeserializeObject<ReCaptchaJsonResponse>(await ReadAnswerFromWebResponseAsync(webResponse));
        }

        private async Task<string> ReadAnswerFromWebResponseAsync(Task<WebResponse> webResponse)
        {
            Stream responseStream = (await webResponse).GetResponseStream();

            if (responseStream == null)
            {
                throw new HttpException(string.Format("No answer from {0}. Check the server web condition.", GoogleRecapthcaUrl));
            }

            using (var responseReader = new StreamReader(responseStream))
            {
                string answer = await responseReader.ReadToEndAsync();
                return answer;
            }
        }
    }
#endif
}