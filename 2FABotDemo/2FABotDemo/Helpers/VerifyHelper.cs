using Nexmo.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _2FABotDemo.Helpers
{
    public  class VerifyHelper
    {
        public  string RequestId { get; set; }
        public  void SendVerificationCode(string phoneNumber)
        {
            var result = NumberVerify.Verify(new NumberVerify.VerifyRequest
            {
                number = phoneNumber,
                brand = "NexmoQS"
            });

            RequestId = result.request_id;
        }

        public  string CheckVerificationCode(string code)
        {
            var result = NumberVerify.Check(new NumberVerify.CheckRequest
            {
                request_id = RequestId,
                code = code
            });

            if (result.status == "0")
            {
                return "Verification Sucessful";
            }
            else
            {
                return result.error_text;
            }
        }
    }
}