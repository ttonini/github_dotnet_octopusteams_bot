using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Octopus.Client;
using Octopus.Client.Exceptions;
using OctopusBot.Dialogs;
using OctopusBot.ViewModels;

namespace OctopusBot.OctopusApi
{
    public class OctopusApi
    {
        private readonly AppSettings _appSettings;
        protected readonly ILogger<OctopusApi> _logger;

        public OctopusApi(IOptions<AppSettings> options, ILogger<OctopusApi> logger)
        {
            _appSettings = options.Value;
            _logger = logger;
        }

        public async Task<ResponseViewModel> OctopusApiStateAsync(OctopusViewModel octopusViewModel, ResponseViewModel responseViewModel, WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            OctopusApiService apiService = new OctopusApiService();

            var OctopusUrl = _appSettings.OctopusConnection.OctoUrl;
            var Key = _appSettings.OctopusConnection.ApiKey;

            var endpoint = new OctopusServerEndpoint(OctopusUrl, Key);
            var client = await OctopusAsyncClient.Create(endpoint);
            var repository = new OctopusAsyncRepository(client);

            //Get necessary Environment and Project variables
            responseViewModel.EnvironmentId = !string.IsNullOrEmpty(octopusViewModel.Environment) ?
                apiService.OctopusApiKeyServiceDict[octopusViewModel.Environment] : null;

            try
            {
                var project = await client.Repository.Projects.Get(octopusViewModel.SlugString);

                var projProgHistory = await repository.Projects.GetProgression(project);

                responseViewModel.IsValidProject = true;

                for (int i = 0; i < projProgHistory.Releases.Count; i++)
                {
                    if (projProgHistory.Releases[i].Deployments.ContainsKey(responseViewModel.EnvironmentId))
                    {
                        responseViewModel.State = projProgHistory.Releases[i].Deployments[responseViewModel.EnvironmentId][0].State.ToString();
                        responseViewModel.Version = projProgHistory.Releases[i].Deployments[responseViewModel.EnvironmentId][0].ReleaseVersion;
                        responseViewModel.IsValidEnv = true;

                        if (responseViewModel.State == "Success")
                        {
                            responseViewModel.IsSuccessfulDeployment = true;
                        }
                        else
                        {
                            responseViewModel.IsSuccessfulDeployment = false;
                            responseViewModel.ErrorMessage =
                                projProgHistory.Releases[i].Deployments[responseViewModel.EnvironmentId][0]
                                    .ErrorMessage;
                        }
                        return responseViewModel;
                    }
                    responseViewModel.IsValidEnv = false;
                }
            }

            //TODO: Catch more specific exceptions as Octopus API features get added 
            catch (OctopusResourceNotFoundException ex)
            {
                Console.WriteLine(ex);
                _logger.LogWarning("Octopus API error for invalid project name {SlugString} on the {Environment}: {Ex}",
                    octopusViewModel.SlugString, octopusViewModel.Environment, ex.Message);
                responseViewModel.IsValidProject = false;
                return responseViewModel;
            }

            // Used to catch other exceptions
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _logger.LogWarning("Error making Octopus API request for {SlugString} on the {Environment}: {Ex}",
                    octopusViewModel.SlugString, octopusViewModel.Environment, ex.Message);
                return responseViewModel;
            }

            return responseViewModel;
        }
    }
}