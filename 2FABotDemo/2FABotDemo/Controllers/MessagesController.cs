using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.FormFlow;
using System;
using Nexmo.Api;

namespace _2FABotDemo
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        internal static IDialog<UserProfile> MakeRootDialog()
        {
            return Chain.From(() => FormDialog.FromForm(UserProfile.BuildForm))
                .Do(async(context, userprofile) => {
                    try
                    {
                        var completed = await userprofile;
                        SendVerificationCode(completed.PhoneNumber);
                        await context.PostAsync("All Done! I sent a verification code to the phone number you provided.");
                    }
                    catch (FormCanceledException<UserProfile> e)
                    {
                        string reply;
                        if (e.InnerException == null)
                        {
                            reply = $"You quit on {e.Last} -- maybe you can finish next time!";
                        }
                        else
                        {
                            reply = "Sorry, I've had a short circuit. Please try again.";
                        }
                        await context.PostAsync(reply);
                    }
                }) ;
        }

        private static void SendVerificationCode(string phoneNumber)
        {
            var sverificationtart = NumberVerify.Verify(new NumberVerify.VerifyRequest
            {
                number = phoneNumber,
                brand = "NexmoQS"
            });
        }

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, ()=> MakeRootDialog());
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}