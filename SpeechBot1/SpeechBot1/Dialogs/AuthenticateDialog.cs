using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.CognitiveServices.Speech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SpeechBot1.Dialogs
{
    public class AuthenticateDialog : CancelAndHelpDialog
    {
        private const string BookingDialogId = "bookingdialogId";
        string cnum, pass;
        public AuthenticateDialog()
            :base(nameof(AuthenticateDialog))
        {
            AddDialog(new BookingDialog(BookingDialogId));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
           
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ClientAcAsync,
                PasswordAsync,
                DeciderAsync,
               
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }
        public Boolean checkCnum(String cnum)
        {
            if(cnum[0] >= 'a' && cnum[0]<= 'z' )
            {
                if(Char.IsLetter(cnum[1]))
                {
                    for(int i=2;i<cnum.Length;i++)
                    {
                        if(!Char.IsDigit(cnum[i]))
                        {
                            return false;
                            
                        }
                    }
                }
            }
            return true;
        }
        //private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
           /* int flag = 0;
            
            System.Diagnostics.Debug.WriteLine("Getting executed");
            var config = SpeechConfig.FromSubscription("f2304a4b309140f789c7f215349eb074", "westus");

            using (var recognizer = new SpeechRecognizer(config))
            {
                await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Client account number") }, cancellationToken);
                var result = await recognizer.RecognizeOnceAsync();
                System.Diagnostics.Debug.WriteLine("runnnnn" + result.Text.Trim().ToLower());
                cnum = result.Text.Replace(" ", String.Empty).ToLower();
                while(!checkCnum(cnum))
                {
                    await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Client account number please") }, cancellationToken);
                    var resultt = await recognizer.RecognizeOnceAsync();
                    System.Diagnostics.Debug.WriteLine("runnnnn" + resultt.Text.Trim().ToLower());
                    cnum = resultt.Text.Replace(" ", String.Empty).ToLower();
                }
                cnum = cnum.Substring(0, cnum.Length - 1);
                //cnum = result.Text.ToLower();

                await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Password") }, cancellationToken);
                var result1 = await recognizer.RecognizeOnceAsync();
                System.Diagnostics.Debug.WriteLine("runnnnn" + result1.Text);
                pass = result1.Text.Replace(" ", String.Empty).ToLower();
                pass = pass.Substring(0, cnum.Length);
                //pass = result1.Text.ToLower();
            }
            try
            {
                using(HttpClient client = new HttpClient())
                {
                    string uri = "http://localhost:8080/check?param1=" + cnum + "&param2=" + pass;
                    System.Diagnostics.Debug.WriteLine(uri);
                    HttpResponseMessage responseMessage = await client.GetAsync(uri);
                    String xxx = await responseMessage.Content.ReadAsStringAsync();
                    if(xxx.Equals("Yes"))
                    {
                        flag = 1;
                    }
                }
            }
            catch(Exception e)
            {

            }
            if (flag == 1)
                return await stepContext.BeginDialogAsync(BookingDialogId, cancellationToken: cancellationToken);
            else
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("You are not authenticated") }, cancellationToken);

               */


        //}
        private async Task<DialogTurnResult> ClientAcAsync(WaterfallStepContext stepContext,CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please enter ID") }, cancellationToken);
        }
        private async Task<DialogTurnResult> PasswordAsync(WaterfallStepContext stepContext,CancellationToken cancellationToken)
        {
            cnum = (String)stepContext.Result;
            //if (pass == null)
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please enter password") }, cancellationToken);
            //else
                return await stepContext.NextAsync(pass, cancellationToken);

        }
        private async Task<DialogTurnResult> Password1Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            cnum = (String)stepContext.Result;
            if(!checkCnum(cnum))
            {
                cnum = null;
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please enter client account number in correct format") }, cancellationToken);
                //cnum = (String)stepContext.Result;
            }
            else
            {
                return await stepContext.NextAsync(pass, cancellationToken);
            }
            if (cnum == null)
            {
                cnum = (String)stepContext.Result;
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please enter password") }, cancellationToken);

            }
            else
                return await stepContext.NextAsync(cnum, cancellationToken);
           /* if (pass == null)
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please enter password") }, cancellationToken);

            }
            else
                return await stepContext.NextAsync(pass, cancellationToken);*/
        }
        private async Task<DialogTurnResult> DeciderAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            int flag = 0;
            pass = (String)stepContext.Result;
            System.Diagnostics.Debug.WriteLine(cnum);
            System.Diagnostics.Debug.WriteLine(pass);
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string uri = "http://localhost:8080/check?param1=" + cnum + "&param2=" + pass;
                    System.Diagnostics.Debug.WriteLine(uri);
                    HttpResponseMessage responseMessage = await client.GetAsync(uri);
                    String xxx = await responseMessage.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine(xxx);
                    if(xxx.Equals("Yes"))
                    {
                        flag = 1;
                    }
                }
            }catch(Exception e) { }

            if (flag == 1)
                return await stepContext.BeginDialogAsync(BookingDialogId, cancellationToken: cancellationToken);
            else
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("You are not authenticated") }, cancellationToken);
        }
    }
}
