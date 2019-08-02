using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.CognitiveServices.Speech;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SpeechBot1.Dialogs
{
    public class VisaFlowDialog : CancelAndHelpDialog
    {
        BookingDetails bookingDetails = new BookingDetails();
        
        public VisaFlowDialog(string dialogId)
            : base(dialogId)
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
        public Boolean IsDigitt(char c)
        {
            if (Char.IsDigit(c) || c.Equals("one") || c.Equals("two"))
                return true;
            else
                return false;
        }
        public Boolean checkCnum(String cnum)
        {
            System.Diagnostics.Debug.WriteLine("in checknum" + cnum);
            Boolean x;
            if (cnum[0] >= 'a' && cnum[0] <= 'z')
            {
                if (Char.IsLetter(cnum[1]))
                {
                    for (int i = 2; i < cnum.Length-1; i++)
                    {
                        if (!cnum.Contains("one") || !cnum.Contains("1"))
                        {
                            x = false;
                            System.Diagnostics.Debug.WriteLine(x);
                            return false;

                        }
                    }
                }
            }
            x = true;
            System.Diagnostics.Debug.WriteLine(x);
            return true;
        }
        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            System.Diagnostics.Debug.WriteLine("Getting executed");
            var config = SpeechConfig.FromSubscription("32dbaf27b0c6426fab70e0cc62398a9f", "westus");
            Random random = new Random();
            SpeechSynthesizer speechSynth = new SpeechSynthesizer(config);
            
            using (var recognizer = new SpeechRecognizer(config))
            {
                String ans;
                
                do
                {
                    await speechSynth.SpeakTextAsync("Please enter client account number\n");
                    await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please enter client account number") }, cancellationToken);
                    
                    var result = await recognizer.RecognizeOnceAsync();
                    
                    AdaptiveCard card1 = new AdaptiveCard();
                    card1.Body.Add(new TextBlock()
                    {
                        Text = result.Text,
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

                    //while(!checkCnum(result.Text.Replace(" ",String.Empty).ToLower()))
                    //{
                    //    await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please enter client account number") }, cancellationToken);
                    //    result = await recognizer.RecognizeOnceAsync();
                    //}
                    System.Diagnostics.Debug.WriteLine("runnnnn" + result.Text);
                    bookingDetails.clientAcNo = result.Text.Replace(" ",String.Empty).ToLower();

                    await speechSynth.SpeakTextAsync("Please enter date of fee\n");
                    await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please enter date of fee") }, cancellationToken);
                    
                    result = await recognizer.RecognizeOnceAsync();
                    AdaptiveCard card2 = new AdaptiveCard();
                    card2.Body.Add(new TextBlock()
                    {
                        Text = result.Text,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Color = TextColor.Accent,
                        //Size = TextSize.Large,
                        Weight = TextWeight.Bolder
                    });
                    Attachment attachment2 = new Attachment()
                    {
                        ContentType = AdaptiveCard.ContentType,
                        Content = card2
                    };
                    await stepContext.PromptAsync(nameof(AttachmentPrompt), options: new PromptOptions { Prompt = (Activity)MessageFactory.Attachment(attachment2) }, cancellationToken);
                    System.Diagnostics.Debug.WriteLine("runnnnn" + result.Text);
                    bookingDetails.DOF = result.Text;

                    await speechSynth.SpeakTextAsync("Please enter amount of fee\n");
                    
                    await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please enter amount of fee") }, cancellationToken);
                    
                    result = await recognizer.RecognizeOnceAsync();
                    String x = result.Text.ToString();
                    AdaptiveCard card3 = new AdaptiveCard();
                    card3.Body.Add(new TextBlock()
                    {
                        Text = "."+result.Text,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Color = TextColor.Accent,
                        //Size = TextSize.Large,
                        Weight = TextWeight.Bolder
                    });
                    Attachment attachment3 = new Attachment()
                    {
                        ContentType = AdaptiveCard.ContentType,
                        Content = card3
                    };
                    await stepContext.PromptAsync(nameof(AttachmentPrompt), options: new PromptOptions { Prompt = (Activity)MessageFactory.Attachment(attachment3) }, cancellationToken);
                    System.Diagnostics.Debug.WriteLine("runnnnn" + result.Text);
                    bookingDetails.AmtOfFee = result.Text;

                    /* await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Reverse Annual Card Fee?"+Environment.NewLine+"1.Against financial advisor"+Environment.NewLine+"2.Against branch") }, cancellationToken);
                     var result4 = await recognizer.RecognizeOnceAsync();
                     AdaptiveCard card4 = new AdaptiveCard();
                     card4.Body.Add(new TextBlock()
                     {
                         Text = result4.Text,
                         HorizontalAlignment = HorizontalAlignment.Right,
                         Color = TextColor.Accent,
                         //Size = TextSize.Large,
                         Weight = TextWeight.Bolder
                     });
                     Attachment attachment4 = new Attachment()
                     {
                         ContentType = AdaptiveCard.ContentType,
                         Content = card4
                     };
                     await stepContext.PromptAsync(nameof(AttachmentPrompt), options: new PromptOptions { Prompt = (Activity)MessageFactory.Attachment(attachment4) }, cancellationToken);
                     System.Diagnostics.Debug.WriteLine("runnnnn" + result4.Text);
                     bookingDetails.FeerevChoice = result4.Text;

                     await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Comments?") }, cancellationToken);
                     var result3 = await recognizer.RecognizeOnceAsync();
                     AdaptiveCard card5 = new AdaptiveCard();
                     card5.Body.Add(new TextBlock()
                     {
                         Text = result3.Text,
                         HorizontalAlignment = HorizontalAlignment.Right,
                         Color = TextColor.Accent,
                         //Size = TextSize.Large,
                         Weight = TextWeight.Bolder
                     });
                     Attachment attachment5 = new Attachment()
                     {
                         ContentType = AdaptiveCard.ContentType,
                         Content = card5
                     };
                     await stepContext.PromptAsync(nameof(AttachmentPrompt), options: new PromptOptions { Prompt = (Activity)MessageFactory.Attachment(attachment5) }, cancellationToken);
                     System.Diagnostics.Debug.WriteLine("runnnnn" + result3.Text);
                     bookingDetails.comments = result3.Text;*/

                    //await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Details are:"+Environment.NewLine+ "Account number: " + bookingDetails.clientAcNo +""+ Environment.NewLine+"2.Date of fee :" + bookingDetails.DOF +Environment.NewLine+ "3.Amount of fee : " + bookingDetails.AmtOfFee + Environment.NewLine+"4.Reversal Annual Card Fee : " + bookingDetails.FeerevChoice +Environment.NewLine+ "4.Comments : " + bookingDetails.comments) }, cancellationToken);
                    await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Details are:"+Environment.NewLine+ "Account number: " + bookingDetails.clientAcNo +""+ Environment.NewLine+"2.Date of fee :" + bookingDetails.DOF +Environment.NewLine+ "3.Amount of fee : " + bookingDetails.AmtOfFee) }, cancellationToken);
                    await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Choose : "+Environment.NewLine+"okay"+Environment.NewLine+"2.change") }, cancellationToken);
                    var result5 = await recognizer.RecognizeOnceAsync();
                    System.Diagnostics.Debug.WriteLine("runnnnn" + result5.Text);
                    ans = result5.Text;
                } while (ans.ToLower().Equals("change."));

                createJSON(bookingDetails);
                //retrieveAttachment(stepContext);


                await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Thank You") }, cancellationToken);
                await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Request "+random.Next(1000,2000)+" is raised") }, cancellationToken);

                var pdfPath = "D:\\json\\employee.json";
                Attachment attachment1 = new Attachment()
                {
                    Name = "Download json",
                    ContentType = "application/json",
                    ContentUrl = pdfPath
                };
                return await stepContext.PromptAsync(nameof(AttachmentPrompt), options: new PromptOptions { Prompt = (Activity)MessageFactory.Attachment(attachment1) }, cancellationToken);


            }



            //return await stepContext.NextAsync();


        }
        void createJSON(BookingDetails bookingDetails)
        {
            string JSONresult = JsonConvert.SerializeObject(bookingDetails);
            string path = @"D:\json\employee.json";
            if(File.Exists(path))
            {
                File.Delete(path);
                using (var tw = new StreamWriter(path, true))
                {
                    tw.WriteLine(JSONresult.ToString());
                    tw.Close();
                }
            }
            else if(!File.Exists(path))
            {
                using (var tw = new StreamWriter(path, true))
                {
                    tw.WriteLine(JSONresult.ToString());
                    tw.Close();
                }
            }
            
        }
        void retrieveAttachment(WaterfallStepContext stepContext)
        {
            

        }
    
        private async Task<DialogTurnResult> SecondStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            System.Diagnostics.Debug.WriteLine("End dialog executed");
            return await stepContext.EndDialogAsync();
        }
    }
}
