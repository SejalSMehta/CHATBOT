using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using System.Threading;
using Microsoft.Bot.Builder;
using AdaptiveCards;
using Microsoft.Bot.Schema;
using Microsoft.CognitiveServices.Speech.Intent;

namespace SpeechBot1.Dialogs
{
    public class VisaDialog : CancelAndHelpDialog
    {
        private const string VisaFlowDialogId = "visaflowdialogId";
        public VisaDialog(string dialogId)
            : base(dialogId)
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new VisaFlowDialog(VisaFlowDialogId));
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
            await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please enter a choice :"+Environment.NewLine+"1.Visa Annual Card Fee Reversal"+Environment.NewLine+"2.Visa Credit Card Finance/Fee Refund") }, cancellationToken);
            System.Diagnostics.Debug.WriteLine("Getting executed");
            //var config = SpeechConfig.FromSubscription("f2304a4b309140f789c7f215349eb074", "westus");
            var config = SpeechConfig.FromSubscription("6c86751ab4a94268b5fda16fc524e07a", "westus");
            using (var recognizer = new IntentRecognizer(config))
            {
                var model = LanguageUnderstandingModel.FromAppId("58137a0a-9008-44f1-b852-816c1bd21ea6");
                recognizer.AddIntent(model, "VChoice1", "Visa Choice 1");
                recognizer.AddIntent(model, "VChoice2", "Visa Choice 2");
                var result = await recognizer.RecognizeOnceAsync();

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
                if (result.IntentId.Equals("Visa Choice 1"))
                {
                    //await stepContext.Context.SendActivityAsync(MessageFactory.Text($"In RMA"));
                    return await stepContext.BeginDialogAsync(VisaFlowDialogId, cancellationToken: cancellationToken);
                }
                if (result.IntentId.Equals("Visa Choice 2"))
                {
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Still Working!!"));
                    //await stepContext.RepromptDialogAsync();
                }
               
            }

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Thank You") }, cancellationToken);

            //return await stepContext.NextAsync();


        }
        private async Task<DialogTurnResult> SecondStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            System.Diagnostics.Debug.WriteLine("End dialog executed");
            return await stepContext.EndDialogAsync();
        }
       
    }
}
