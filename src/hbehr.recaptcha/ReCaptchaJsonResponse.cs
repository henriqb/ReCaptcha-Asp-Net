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

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace hbehr.recaptcha
{
    [DataContract]
    internal class ReCaptchaJsonResponse
    {
        [DataMember(Name = "success")]
        internal bool Success { get; set; }
        [DataMember(Name = "error-codes")]
        internal string[] ErrorCodes { get; set; }

        internal static ReCaptchaJsonResponse DeserializeResponse(string response)
        {
            var serializer = new DataContractJsonSerializer(typeof(ReCaptchaJsonResponse));
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(response);
            writer.Flush();
            stream.Position = 0;

            return (ReCaptchaJsonResponse)serializer.ReadObject(stream);
        }
    }
}