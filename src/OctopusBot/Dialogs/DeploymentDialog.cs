// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with CoreBot .NET Template version v4.13.2

using System;
using System.Text;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;
using OctopusBot.Data;
using OctopusBot.Utilities;
using OctopusBot.ViewModels;
using OctopusBot.Services;
using System.Collections.Generic;

namespace OctopusBot.Dialogs
{
    public class DeploymentDialog : CancelAndHelpDialog
    {
        private static readonly EntityRequirementValidator EntityValidator = new();
        private static readonly StringUtil StringUtil = new();
        private static List<Dictionary<string, string>> _synonymsDictionaryList;
        private readonly ICosmosService _cosmosService;
        private readonly IGetSynonymsService _getSynonymsService;

        public DeploymentDialog(ICosmosService cosmosService, IGetSynonymsService getSynonymsService)
            : base(nameof(DeploymentDialog))
        {
            AddDialog(new TextPrompt("serviceName", ServiceNamePromptValidatorAsync));
            AddDialog(new TextPrompt("teamName", TeamNamePromptValidatorAsync));
            AddDialog(new TextPrompt("languageName", LanguageNamePromptValidatorAsync));
            AddDialog(new TextPrompt("environmentName", EnvironmentNamePromptValidatorAsync));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ServiceStepAsync,
                TeamStepAsync,
                LanguageStepAsync,
                EnvironmentStepAsync,
                ConfirmStepAsync,
                FinalStepAsync
            }));

            _cosmosService = cosmosService;
            _getSynonymsService = getSynonymsService;

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }


        private async Task<DialogTurnResult> ServiceStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Grab the current OctopusViewModel
            var octopusViewModelDetails = (OctopusViewModel)stepContext.Options;

            var data = await _cosmosService.GetSynonymsAsync();
            _synonymsDictionaryList = _getSynonymsService.GetSynonymsDictionaryList(data);

            // Note: we duplicate the MsgText because the second param is used by speech enabled output
            var promptMessage = MessageFactory.Text(ConstantStringData.ServiceStepMsgText, ConstantStringData.ServiceStepMsgText, InputHints.ExpectingInput);
            var repromptMessage = MessageFactory.Text(ConstantStringData.RepromptServiceStepMsgText, ConstantStringData.RepromptServiceStepMsgText, InputHints.ExpectingInput);

            if (String.IsNullOrEmpty(octopusViewModelDetails.Service))
            {
                return await stepContext.PromptAsync("serviceName",
                    new PromptOptions
                    {
                        Prompt = promptMessage,
                        RetryPrompt = repromptMessage
                    }, cancellationToken);
            }

            return await stepContext.NextAsync(octopusViewModelDetails.Service, cancellationToken);
        }

        private async Task<DialogTurnResult> TeamStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var octopusViewModelDetails = (OctopusViewModel)stepContext.Options;
            var tempService = (string)stepContext.Result;
            

            // Assign the value returned from ServiceStepAsync to the view model depending on the value of the key
            octopusViewModelDetails.Service = _synonymsDictionaryList[0][tempService.ToLower()];

            var promptMessage = MessageFactory.Text(ConstantStringData.TeamStepMsgText, ConstantStringData.TeamStepMsgText, InputHints.ExpectingInput);
            var repromptMessage = MessageFactory.Text(ConstantStringData.RepromptTeamStepMsgText, ConstantStringData.RepromptTeamStepMsgText, InputHints.ExpectingInput);

            // Check for a null team name for deployments that have a team name
            if (String.IsNullOrEmpty(octopusViewModelDetails.Team) && EntityValidator.IsTeamRequired(octopusViewModelDetails))
            {
                return await stepContext.PromptAsync("teamName",
                    new PromptOptions
                    {
                        Prompt = promptMessage,
                        RetryPrompt = repromptMessage
                    }, cancellationToken);
            }

            return await stepContext.NextAsync(octopusViewModelDetails.Team, cancellationToken);
        }

        private async Task<DialogTurnResult> LanguageStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var octopusViewModelDetails = (OctopusViewModel)stepContext.Options;

            var tempTeam = (string)stepContext.Result;

            if (!String.IsNullOrEmpty(tempTeam))
            {
                octopusViewModelDetails.Team = _synonymsDictionaryList[1][tempTeam.ToLower()];
            }

            if (String.IsNullOrEmpty(octopusViewModelDetails.Language) && EntityValidator.IsLanguageRequired(octopusViewModelDetails))
            {
                var promptMessage = MessageFactory.Text(ConstantStringData.LanguageStepMsgText, ConstantStringData.LanguageStepMsgText, InputHints.ExpectingInput);
                var repromptMessage = MessageFactory.Text(ConstantStringData.RepromptLanguageStepMsgText, ConstantStringData.RepromptLanguageStepMsgText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync("languageName",
                    new PromptOptions
                    {
                        Prompt = promptMessage,
                        RetryPrompt = repromptMessage
                    }, cancellationToken);
            }

            return await stepContext.NextAsync(octopusViewModelDetails.Language, cancellationToken);
        }

        private async Task<DialogTurnResult> EnvironmentStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var octopusViewModelDetails = (OctopusViewModel)stepContext.Options;

            var tempLanguage = (string)stepContext.Result;

            if (!String.IsNullOrEmpty(tempLanguage))
            {
                octopusViewModelDetails.Language = _synonymsDictionaryList[2][tempLanguage.ToLower()];
            }


            var promptMessage = MessageFactory.Text(ConstantStringData.EnvironmentStepMsgText, ConstantStringData.EnvironmentStepMsgText, InputHints.ExpectingInput);
            var repromptMessage = MessageFactory.Text(ConstantStringData.RepromptEnvironmentStepMsgText, ConstantStringData.RepromptEnvironmentStepMsgText, InputHints.ExpectingInput);

            if (String.IsNullOrEmpty(octopusViewModelDetails.Environment))
            {
                return await stepContext.PromptAsync("environmentName",
                new PromptOptions
                {
                    Prompt = promptMessage,
                    RetryPrompt = repromptMessage
                }, cancellationToken);
            }

            return await stepContext.NextAsync(octopusViewModelDetails.Environment, cancellationToken);
        }


        private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var octopusViewModelDetails = (OctopusViewModel)stepContext.Options;

            var tempEnvironment = (string)stepContext.Result;

            // Assign value returned from EnvironmentStepAsync to the view model depending on the value of the key
            octopusViewModelDetails.Environment = _synonymsDictionaryList[3][tempEnvironment.ToLower()];

            string confirmText = ConstantStringData.ConfirmStepText(octopusViewModelDetails);

            var promptMessage = MessageFactory.Text(confirmText, confirmText, InputHints.ExpectingInput);

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }


        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result)
            {
                var octopusViewModelDetails = (OctopusViewModel)stepContext.Options;

                // Create a slug string for the Octopus API once we have received all of the details
                var resultString = StringUtil.ConvertToSlugString(octopusViewModelDetails);
                octopusViewModelDetails.SlugString = resultString;

                return await stepContext.EndDialogAsync(octopusViewModelDetails, cancellationToken);
            }

            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

        private static Task<bool> ServiceNamePromptValidatorAsync(PromptValidatorContext<String> promptContext, CancellationToken cancellationToken)
        {
            // Checks if the service is in the service name dictionary
            return Task.FromResult(promptContext.Recognized.Succeeded && _synonymsDictionaryList[0].ContainsKey(promptContext.Recognized.Value.ToLower()));
        }

        private static Task<bool> TeamNamePromptValidatorAsync(PromptValidatorContext<String> promptContext, CancellationToken cancellationToken)
        {
            // Checks if the team name is in the team name dictionary
            return Task.FromResult(promptContext.Recognized.Succeeded && _synonymsDictionaryList[1].ContainsKey(promptContext.Recognized.Value.ToLower()));
        }

        private static Task<bool> LanguageNamePromptValidatorAsync(PromptValidatorContext<String> promptContext, CancellationToken cancellationToken)
        {
            // Checks if the language is in the dictionary
            return Task.FromResult(promptContext.Recognized.Succeeded && _synonymsDictionaryList[2].ContainsKey(promptContext.Recognized.Value.ToLower()));
        }

        private static Task<bool> EnvironmentNamePromptValidatorAsync(PromptValidatorContext<String> promptContext, CancellationToken cancellationToken)
        {
            // Checks if the Environment is in the dictionary
            return Task.FromResult(promptContext.Recognized.Succeeded && _synonymsDictionaryList[3].ContainsKey(promptContext.Recognized.Value.ToLower()));
        }
    }
}