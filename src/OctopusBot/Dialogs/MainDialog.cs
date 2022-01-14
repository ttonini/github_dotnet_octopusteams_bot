// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with CoreBot .NET Template version v4.13.2

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Luis;
using OctopusBot.OctopusApi;
using Microsoft.Extensions.Options;
using OctopusBot;
using OctopusBot.Utilities;
using OctopusBot.ViewModels;
using OctopusBot.Data;
using OctopusBot.Services;
using Octopus.GraphClient;

namespace OctopusBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly OctopusBotRecognizer _luisRecognizer;
        protected readonly ILogger<MainDialog> _logger;
        private readonly AppSettings _settings;
        private readonly OctopusApi.OctopusApi _octopus;
        private readonly ICosmosService _cosmosService;
        private readonly IGetSynonymsService _getSynonymService;


        // Dependency injection uses this constructor to instantiate MainDialog
        public MainDialog(OctopusBotRecognizer luisRecognizer, DeploymentDialog deploymentDialog, AuthenticationDialog authenticationDialog, ILogger<MainDialog> logger,
            IOptions<AppSettings> options, OctopusApi.OctopusApi octopus, ICosmosService cosmosService, IGetSynonymsService getSynonymService)
            : base(nameof(MainDialog))
        {
            _luisRecognizer = luisRecognizer;
            _logger = logger;
            _settings = options.Value;
            _octopus = octopus;
            _cosmosService = cosmosService;
            _getSynonymService = getSynonymService;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(authenticationDialog);
            AddDialog(deploymentDialog);
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                AuthenticationStepAsync,
                IntroStepAsync,
                ActStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> AuthenticationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.BeginDialogAsync(nameof(AuthenticationDialog), null, cancellationToken);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //_logger.LogInformation("Successfully logged to seq!");


            if (!_luisRecognizer.IsConfigured)
            {
                _logger.LogWarning("Luis not configured");
                await stepContext.Context.SendActivityAsync(
                    MessageFactory.Text(
                        ConstantStringData.LuisNotConfiguredText,
                        inputHint: InputHints.IgnoringInput), cancellationToken);

                return await stepContext.NextAsync(null, cancellationToken);
            }

            // Use the text provided in FinalStepAsync or the default if it is the first time.
            // Display default first message OR continue dialog
            var messageText = stepContext.Options?.ToString() ??
                              ConstantStringData.IntroMsgText;
            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            if (!_luisRecognizer.IsConfigured)
            {
                // When LUIS is not configured, run the DeploymentDialog with an empty instance of the OctopusViewModel
                // for user to fill out all details instead
                // This is a fail safe for LUIS
                return await stepContext.BeginDialogAsync(nameof(DeploymentDialog), new OctopusViewModel(), cancellationToken);
            }

            var luisResultViewModel = new LuisResultViewModel();

            var luisResult = await _luisRecognizer.RecognizeAsync<DeploymentChatbot>(stepContext.Context, cancellationToken);

            switch (luisResult.TopIntent().intent)
            {

                //TODO: Check with other intents we will need in the future
                case DeploymentChatbot.Intent.Check_Deployment:

                    var luisResultArray = luisResult.Entities.Octopus_Deployment_Entity;

                    // If an Octopus_Deployment_Entity is found, we can look for the details in the array
                    if (luisResultArray != null)
                    {
                        for (int i = 0; i < luisResultArray.Length; i++)
                        {
                            // TODO: Handle the case where multiple options(service, build, etc) are identified
                            // "ie - Give me valid values decode service on test blue and test green"
                            // Prompt user to pick one
                            // TODO (4): Propagate or not the second '?' in multidimensional array access
                            var tempServiceNull = luisResultArray[i].Service?[0]?[0];
                            if (tempServiceNull != null)
                            {
                                luisResultViewModel.Service = tempServiceNull;
                                tempServiceNull = null;
                            }

                            var tempBuildNull = luisResultArray[i].Build?[0][0];
                            if (tempBuildNull != null)
                            {
                                luisResultViewModel.Build = tempBuildNull;
                                tempBuildNull = null;
                            }

                            var tempEnvironmentNull = luisResultArray[i].Environment?[0][0];
                            if (tempEnvironmentNull != null)
                            {
                                luisResultViewModel.Environment = tempEnvironmentNull;
                                tempEnvironmentNull = null;
                            }

                            var tempTeamNull = luisResultArray[i].TeamName?[0][0];
                            if (tempTeamNull != null)
                            {
                                luisResultViewModel.Team = tempTeamNull;
                                tempTeamNull = null;
                            }

                            var tempLanguageNull = luisResultArray[i].Language?[0][0];
                            if (tempLanguageNull != null)
                            {
                                luisResultViewModel.Language = tempLanguageNull;
                                tempLanguageNull = null;
                            }
                        }
                    }

                    // Build a ViewModel based on users input
                    var octopusViewModelDetails = new OctopusViewModel()
                    {
                        Build = luisResultViewModel.Build,
                        Environment = luisResultViewModel.Environment,
                        Service = luisResultViewModel.Service,
                        Team = luisResultViewModel.Team,
                        Language = luisResultViewModel.Language,
                        SlugStringSpecialCases = _settings.SlugStringSpecialCases
                    };

                    return await stepContext.BeginDialogAsync(nameof(DeploymentDialog), octopusViewModelDetails, cancellationToken);

                default:
                    // Catch all for unhandled intents
                    var didntUnderstandMessageText =
                        ConstantStringData.DidntGetThatText;
                    var didntUnderstandMessage = MessageFactory.Text(didntUnderstandMessageText, didntUnderstandMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(didntUnderstandMessage, cancellationToken);
                    break;

            }

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // If the child dialog ("DeploymentDialog") was cancelled, the user failed to confirm or if the intent wasn't CheckDeployment
            // the stepContext.Result here will be null
            if (stepContext.Result is OctopusViewModel result)
            {
                var response = new ResponseViewModel();

                _logger.LogInformation("Octopus deployment API request made for {SlugString} on {Environment}",
                    result.SlugString, result.Environment);
                response = await _octopus.OctopusApiStateAsync(result, response, stepContext, cancellationToken);

                string finalResponseText = ConstantStringData.FinalResult(response, result);
                var message = MessageFactory.Text(finalResponseText, finalResponseText, InputHints.IgnoringInput);
                await stepContext.Context.SendActivityAsync(message, cancellationToken);
            }

            // Restart the main dialog with a different message the second time around
            return await stepContext.ReplaceDialogAsync(InitialDialogId, ConstantStringData.PromptMessage, cancellationToken);
        }
    }
}