using System.Collections.Generic;

namespace hbehr.recaptcha
{
    public class ReCaptchaError
    {
        public ReCaptchaError(IEnumerable<string> errorCodes)
        {
            if (errorCodes == null) { return; }
            foreach (string errorCode in errorCodes)
            {
                if ("invalid-input-secret".Equals(errorCode))
                {
                    InvalidInputSecret = true;
                }
                else if ("invalid-input-response".Equals(errorCode))
                {
                    InvalidInputResponse = true;
                }
            }
        }

        //missing-input-secret	The secret parameter is missing. <- Shouldn't happen
        //missing-input-response	The response parameter is missing. <- Shouldn't happen

        //invalid-input-response	The response parameter is invalid or malformed.
        public bool InvalidInputResponse { get; set; }
        //invalid-input-secret	The secret parameter is invalid or malformed.
        public bool InvalidInputSecret { get; set; }
    }
}