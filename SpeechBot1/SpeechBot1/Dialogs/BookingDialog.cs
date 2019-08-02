// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.3.0

using System;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Intent;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

namespace SpeechBot1.Dialogs
{
    public class BookingDialog : CancelAndHelpDialog
    {
        private const string VisaDialogId = "visadialogId";
        private const string BookingDialogId = "bookingdialogId";
        private const string RMA_BSAId = "rma_bsaId";
        public BookingDialog(string dialogId)
            : base(dialogId)
        {
            

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new VisaDialog(VisaDialogId));
            AddDialog(new AttachmentPrompt(nameof(AttachmentPrompt)));
            AddDialog(new RMA_BSA(RMA_BSAId));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                
                SecondStepAsync,
               
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

       
        private async Task<DialogTurnResult> SecondStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
           
            await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please enter a choice :"+Environment.NewLine+"1.RMA"+Environment.NewLine+"2.Visa"+Environment.NewLine+"3.Help") }, cancellationToken);
            System.Diagnostics.Debug.WriteLine("Getting executed");
            //var config = SpeechConfig.FromSubscription("f2304a4b309140f789c7f215349eb074", "westus");
            var config = SpeechConfig.FromSubscription("6c86751ab4a94268b5fda16fc524e07a", "westus");
            using (var recognizer = new IntentRecognizer(config))
            {
                //var result = await recognizer.RecognizeOnceAsync();
                //System.Diagnostics.Debug.WriteLine("runnnnn"+result.Text);
                
                // await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(result.Text,null,System.Drawing.Color.Brown.ToString() )}, cancellationToken);

                var model = LanguageUnderstandingModel.FromAppId("58137a0a-9008-44f1-b852-816c1bd21ea6");
                recognizer.AddIntent(model, "HelpIntent", "help");
                recognizer.AddIntent(model, "VisaIntent", "visa");
                recognizer.AddIntent(model, "RMABSAIntent", "rma");
                //recognizer.AddIntent(model, "None", "null");

                var result = await recognizer.RecognizeOnceAsync();

                System.Diagnostics.Debug.WriteLine(result.Text);
                System.Diagnostics.Debug.WriteLine("IDD"+result.IntentId);

                AdaptiveCard card1 = new AdaptiveCard();

                 card1.Body.Add(new TextBlock()
                 {
                     Text = result.IntentId,
                     HorizontalAlignment = HorizontalAlignment.Right,
                     Color = TextColor.Accent,
                     //Size = TextSize.Large,
                     Weight = TextWeight.Bolder
                 });
                //Attachment card = new Attachment();
                //HeroCard plCard = new HeroCard()
                //{
                //    Title = $" "+result.Text,
                //    Text = "More Words <br> New Line </br> New Line <b><font color=\"#11b92f\">GREEN</font></b>"


                //Subtitle = $"{cardContent.Key} Wikipedia Page",
                //Images = cardImages,
                //Buttons = cardButtons
                // };
                //Attachment a = card1.ToAttachment();
                // Attachment plAttachment = plCard.ToAttachment();

                /*card.Body.Add(new AdaptiveTextBlock()
                {
                    Text = "This is a test",
                    Weight = TextWeight.Bolder
                        Size = TextSize.Medium,
                }*/

                /*card.Body.Add(new TextBlock()
                {
                    Text = "This is a test",
                    Weight = TextWeight.Bolder,
                        Size = TextSize.Medium,
                });*/
                //Activity a1 = new Activity();
                 Attachment attachment = new Attachment()
                 {
                     ContentType = AdaptiveCard.ContentType,
                     Content = card1
                 };
                // a1.Attachments.Add(attachment);
                // MessageFactory.Attachment()
                // response.Attachments.Add(card.ToAttachment());
                //await stepContext.PromptAsync(nameof(AttachmentPrompt), options: new PromptOptions { Prompt = (Activity)MessageFactory.Attachment(plAttachment, null, null, null)}, cancellationToken);
                 await stepContext.PromptAsync(nameof(AttachmentPrompt), options: new PromptOptions { Prompt = (Activity)MessageFactory.Attachment(attachment) }, cancellationToken);
                // await context.PostAsync(response);
                
                if (result.IntentId.Equals("rma"))
                {
                    //await stepContext.Context.SendActivityAsync(MessageFactory.Text($"In RMA"));
                    //await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please enter a choice :\n1.RMA\n2.Visa\n3.Help") }, cancellationToken);
                    //await stepContext.RepromptDialogAsync();
                    return await stepContext.BeginDialogAsync(RMA_BSAId, cancellationToken: cancellationToken);
                }
                if (result.IntentId.Equals("visa"))
                {
                    return await stepContext.BeginDialogAsync(VisaDialogId, cancellationToken :cancellationToken);
                }
                if (result.IntentId == "help")
                {
                    return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Help will be provided soon.") }, cancellationToken);
                   // await stepContext.RepromptDialogAsync();
                }
            }
            
            
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Thankyou.") }, cancellationToken);
            //return await stepContext.EndDialogAsync();
            //return await stepContext.NextAsync();

        }
       
    }
}
