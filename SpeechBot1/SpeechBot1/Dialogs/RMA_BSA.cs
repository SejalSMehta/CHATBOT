using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Intent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SpeechBot1.Dialogs
{
    public class RMA_BSA : CancelAndHelpDialog
    {
        public RMA_BSA(string dialogId)
            :base(dialogId)
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                InitialStepAsync,
                SecondStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }
        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please enter : "+Environment.NewLine+"1.Choice 1"+Environment.NewLine+"2.Choice 2") }, cancellationToken);
            System.Diagnostics.Debug.WriteLine("Getting executed");
            //var config = SpeechConfig.FromSubscription("f2304a4b309140f789c7f215349eb074", "westus");
            var config = SpeechConfig.FromSubscription("6c86751ab4a94268b5fda16fc524e07a", "westus");

            using (var recognizer = new IntentRecognizer(config))
            {
                var model = LanguageUnderstandingModel.FromAppId("58137a0a-9008-44f1-b852-816c1bd21ea6");
                recognizer.AddIntent(model, "RChoice1", "RMA Choice 1");
                recognizer.AddIntent(model, "RChoice2", "RMA Choice 2");
                var result = await recognizer.RecognizeOnceAsync();
                //System.Diagnostics.Debug.WriteLine("runnnnn" + stepContext.Result);
                System.Diagnostics.Debug.WriteLine("runnnnn" + result.IntentId);
                AdaptiveCard card1 = new AdaptiveCard();
                card1.Body.Add(new TextBlock()
                {
                    Text = result.IntentId,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Color = TextColor.Accent,
                    //Size = TextSize.Large,
                    Weight = TextWeight.Bolder
                });
                Attachment attachment = new Attachment()
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = card1
                };
                await stepContext.PromptAsync(nameof(AttachmentPrompt), options: new PromptOptions { Prompt = (Activity)MessageFactory.Attachment(attachment) }, cancellationToken);
                if (result.IntentId.Equals("RMA Choice 1"))
                {
                    await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Working on choice one") }, cancellationToken);

                }
                if (result.IntentId.Equals("RMA Choice 2"))
                {
                    await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Working on choice two") }, cancellationToken);
                    // await stepContext.RepromptDialogAsync();
                }


            }
            //if(stepContext.Result)
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Thank You") }, cancellationToken);
            

            //await stepContext.Context.SendActivityAsync(MessageFactory.Text("Thank you."), cancellationToken);
            //return await stepContext.EndDialogAsync(cancellationToken:cancellationToken);
            //return await stepContext.NextAsync();


        }
        private async Task<DialogTurnResult> SecondStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            System.Diagnostics.Debug.WriteLine("End dialog executed");
            return await stepContext.EndDialogAsync();
        }
    }
}
