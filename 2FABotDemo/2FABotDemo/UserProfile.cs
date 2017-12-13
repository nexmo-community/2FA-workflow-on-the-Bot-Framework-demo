using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _2FABotDemo
{
    [Serializable]
    public class UserProfile
    {
        [Prompt("What's your first name?")]
        public string FirstName;
        [Prompt("How about your last name?")]
        public string LastName;
        [Prompt("I now need your phone number in its international format but without '+' or '00'. I will shortly send a verification code.")]
        public string PhoneNumber;
        public static IForm<UserProfile> BuildForm()
        { 
            return new FormBuilder<UserProfile>().Message("Welcome to the 2FA demo bot!").Build();
        }

    }
}