using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.FormFlow;
using System;
using Nexmo.Api;
using _2FABotDemo.Helpers;

namespace _2FABotDemo
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private static VerifyHelper verify;

        public static VerifyHelper GetVerify()
        {
            return verify;
        }

        public static void SetVerify(VerifyHelper value)
        {
            verify = value;
        }

        internal static IDialog<UserProfile> MakeRootDialog() => Chain.From(() => FormDialog.FromForm(UserProfile.BuildForm))
                .Do(async (context, userprofile) =>
                {
                    SetVerify(new VerifyHelper());
                    try
                    {
                        var completed = await userprofile;
                        GetVerify().SendVerificationCode(completed.PhoneNumber);
                        await context.PostAsync("All Done! I sent a verification code to the phone number you provided. Could you please tell me the code once you receive it?");
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
                });
       
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                StateClient sc = activity.GetStateClient();
                BotData userData = sc.BotState.GetPrivateConversationData(
                    activity.ChannelId, activity.Conversation.Id, activity.From.Id);
                var boolProfileComplete = userData.GetProperty<bool>("ProfileComplete");
                if (!boolProfileComplete)
                {
                    await Conversation.SendAsync(activity, () => MakeRootDialog());
                }
                else
                {
                    ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                    Activity replyMessage = activity.CreateReply(GetVerify().CheckVerificationCode(activity.Text));
                    await connector.Conversations.ReplyToActivityAsync(replyMessage);
                }
       
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