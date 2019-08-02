// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.3.0

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SpeechBot1
{
    public static class LuisHelper
    {
        public static async Task<BookingDetails> ExecuteLuisQuery(IConfiguration configuration, ILogger logger, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var bookingDetails = new BookingDetails();

            try
            {
                // Create the LUIS settings from configuration.
                var luisApplication = new LuisApplication(
                    configuration["3ad25541-71af-4a2e-b968-1a9e8db3e31c"],
                    configuration["503c0c9b7ef34c35bd4ec89a2d02b57c"],
                    "https://" + configuration["westus.api.cognitive.microsoft.com/luis/v2.0/apps/3ad25541-71af-4a2e-b968-1a9e8db3e31c?verbose=true&timezoneOffset=-360&subscription-key=503c0c9b7ef34c35bd4ec89a2d02b57c&q="]
                );

                var recognizer = new LuisRecognizer(luisApplication);

                // The actual call to LUIS
                var recognizerResult = await recognizer.RecognizeAsync(turnContext, cancellationToken);

                var (intent, score) = recognizerResult.GetTopScoringIntent();
                if (intent == "Book_flight")
                {
                    // We need to get the result from the LUIS JSON which at every level returns an array.
                }
            }
            catch (Exception e)
            {
                logger.LogWarning($"LUIS Exception: {e.Message} Check your LUIS configuration.");
            }

            return bookingDetails;
        }
    }
}
