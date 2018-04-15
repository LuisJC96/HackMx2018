using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Luis;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Globalization;
using System.Data.Entity.Validation;

namespace FootballOne
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                if (activity.Text.ToLower().Contains("como se ve"))
                {
                    HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
                    HtmlAgilityPack.HtmlDocument doc = web.Load("https://www.google.com/search?q=" + activity.Text.ToLower().Replace("como se ve una ","").Replace("como se ve un ","").Replace(" ", "+").Replace("?","") + "&tbm=isch");
                    var images = doc.DocumentNode.SelectNodes("//img[@src]").ToList();
                    Random rn = new Random();
                    var src = images[rn.Next(0, images.Count / 2)].GetAttributeValue("src", null);
                    Attachment att = new Attachment();
                    att.ContentType = "image/png";
                    att.ContentUrl = src;
                    var reply = activity.CreateReply();
                    reply.Attachments.Add(att);
                    ConnectorClient client = new ConnectorClient(new Uri(activity.ServiceUrl));
                    await client.Conversations.ReplyToActivityAsync(reply);
                }
                else
                {
                    await Conversation.SendAsync(activity, () => new Dialogs.LuisHandler());
                }
            }
            else
            if (activity.Type == ActivityTypes.ConversationUpdate)
            {
                if (activity.MembersAdded.Any(o => o.Id == activity.Recipient.Id))
                {
                    ConnectorClient client = new ConnectorClient(new Uri(activity.ServiceUrl));
                    var reply = activity.CreateReply();
                    reply.Text = "Hola bienvenido a New Starters estoy aquí para ayudarte, pregúntame lo que necesites **:)**";
                    await client.Conversations.ReplyToActivityAsync(reply);
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