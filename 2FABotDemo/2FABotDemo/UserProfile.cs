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
        public string FirstName;
        public string LastName;
        public string PhoneNumber;
        public static IForm<UserProfile> BuildForm()
        { 
            return new FormBuilder<UserProfile>().Message("Welcome to the 2FA demo bot!").Build();
        }

    }
}