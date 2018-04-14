using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Builder.Luis;
using System.Linq;
namespace FootballOne.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // calculate something for us to return
            int length = (activity.Text ?? string.Empty).Length;

            WordAnalyzer wa = new WordAnalyzer();
            int vocals = wa.CountVocals(activity.Text);
            // return our reply to the user
            await context.PostAsync($"You sent {activity.Text} which was {length} characters and {vocals} vocals");

            context.Wait(MessageReceivedAsync);
        }
    }
}