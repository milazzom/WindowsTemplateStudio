﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Ai.LUIS;
using Microsoft.Bot.Builder.Ai.QnA;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Services.BotTemplates.LuisBot.Dialogs;
using Microsoft.Services.BotTemplates.LuisBot.Dialogs.AdaptiveCards;
using Microsoft.Services.BotTemplates.LuisBot.Services;

namespace Microsoft.Services.BotTemplates.LuisBot
{
    /// <summary>
    /// This is a very simple demo bot based on LUIS
    /// </summary>
    public class DemoBot : IBot
    {
        private readonly DialogSet _dialogs;
        private readonly LuisRecognizer _luisRecognizer;

        public DemoBot(LuisRecognizer luisRecognizer, QnAMaker qnaMaker, BingMaps bingMaps)
        {
            _luisRecognizer = luisRecognizer;
            _dialogs = new DialogSet();
            _dialogs.Add("KBQuestion", new KBQuestion(qnaMaker));
            _dialogs.Add("BookRestaurant", new BookRestaurant(luisRecognizer, bingMaps));
            _dialogs.Add("TrunkStatusChanged", new TrunkStatusChanged());
        }

        public async Task OnTurn(ITurnContext context)
        {
            try
            {
                if (context.Activity.Type == ActivityTypes.ConversationUpdate && context.Activity.MembersAdded.FirstOrDefault()?.Id == context.Activity.Recipient.Id)
                {
                    await RenderWelcomeMessage(context);
                }
                else if (context.Activity.Type == ActivityTypes.Event && context.Activity.Name == "TrunkStatusChanged")
                {
                    var state = context.GetConversationState<Dictionary<string, object>>();
                    var dialogContext = _dialogs.CreateContext(context, state);
                    await dialogContext.Begin("TrunkStatusChanged", state);
                }
                else if (context.Activity.Type == ActivityTypes.Message)
                {
                    var state = context.GetConversationState<Dictionary<string, object>>();
                    var dialogContext = _dialogs.CreateContext(context, state);

                    var utterance = context.Activity.Text.ToLowerInvariant();
                    if (utterance == "cancel")
                    {
                        if (dialogContext.ActiveDialog != null)
                        {
                            await context.SendActivity("Ok... Cancelled");
                            dialogContext.EndAll();
                        }
                        else
                        {
                            await context.SendActivity("Nothing to cancel.");
                        }
                    }

                    if (!context.Responded)
                    {
                        await dialogContext.Continue();

                        if (!context.Responded)
                        {
                            var luisResult = await _luisRecognizer.Recognize(context.Activity.Text, CancellationToken.None);
                            var (intent, score) = luisResult.GetTopScoringIntent();
                            if (_dialogs.Find(intent) != null)
                            {
                                await dialogContext.Begin(intent);
                            }
                            else
                            {
                                var reply = context.Activity.CreateReply("Sorry, didn't get that");
                                //if (TextAnalyticsMiddleware.IsLowSentiment(context, .4) || ContentModeratorMiddleware.CurseWordsDetected(context))
                                //{
                                //    reply.Text = reply.Text + " \r\n  Would you like to talk to an agent?";
                                //    reply.AddSuggestedActions(new List<string> {"Talk to an agent"});
                                //}

                                await context.SendActivity(reply);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await context.SendActivity($"Yikes!!!\r\n{ex}", "Yikes!!");
                throw;
            }
        }

        private static async Task RenderWelcomeMessage(ITurnContext context)
        {
            var reply = context.Activity.CreateReply();
            reply.Attachments.Add(
                AdaptiveCardHelper.GetCardAttachmentFromJson("Resources\\Cards\\WelcomeCard.json",
                    new StringDictionary {{"UserName", "Gabo"}})
            );
            await context.SendActivity(reply);
        }
    }
}