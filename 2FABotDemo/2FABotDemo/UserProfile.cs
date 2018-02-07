using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System;

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
            return new FormBuilder<UserProfile>().Message("Welcome! Before I'm of any use to you, I will need to verify your identity. Please answer the following questions.")
                .OnCompletion(async (context, UserProfile) => {
                    context.PrivateConversationData.SetValue<bool>("ProfileComplete", true);
                    await context.PostAsync("Your profile is complete.");
                })
                .Build();
        }
    }
   
}