using System;

namespace hbehr.recaptcha.Exceptions
{
    public class ReCaptchaException : Exception
    {
        public ReCaptchaException() {}

        public ReCaptchaException(string message) : base(message) {}

        public ReCaptchaException(string message, Exception innerException) : base(message, innerException) { }
    }
}
