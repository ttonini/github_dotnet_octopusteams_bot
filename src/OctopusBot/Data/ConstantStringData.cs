using OctopusBot.Utilities;
using OctopusBot.ViewModels;

namespace OctopusBot.Data
{
    public static class ConstantStringData
    {
        //DeploymentDialog text
        public const string ServiceStepMsgText = "What service would you like to check?";
        public const string RepromptServiceStepMsgText = "I'm sorry, I could not find a service with that name. " +
                                               "Please try again by entering another service name.";
        public const string EnvironmentStepMsgText = "What environment would you like to check?";
        public const string RepromptEnvironmentStepMsgText = "I'm sorry, I could not find an environment with that name. " +
                                                            "Please try again by entering another environment name. ";
        public const string TeamStepMsgText = "What team would you like to check? Please enter arcus, cloud, or tconnect.";
        public const string RepromptTeamStepMsgText = "I'm sorry, I could not find a team with that name. " +
                                                       "Please try again by entering arcus, cloud, or tconnect.";
        public const string LanguageStepMsgText = "What language would you like to check? Please enter node or dotnet.";
        public const string RepromptLanguageStepMsgText = "I'm sorry, I could not find a language with that name. " +
                                                       "Please try again by entering node or dotnet.";
        private static readonly EnvironmentNameSynonymData EnvironmentSynonyms = new();

        //MainDialog text
        public const string LuisNotConfiguredText = "NOTE: LUIS is not configured. To enable all capabilities, add 'LuisAppId', 'LuisAPIKey' and 'LuisAPIHostName' to the appsettings.json file.";
        public const string IntroMsgText = "What can I help you with today?\nSay something like \"Show me what the arcus node decode service is doing on test blue\"";
        public const string DidntGetThatText = "Sorry, I didn't get that. Please try asking in a different way. Provide the bot with a team name, service, language, and environment in order to checkout your deployment!";
        public const string PromptMessage = "What else can I do for you?";


        public static string ConfirmStepText(OctopusViewModel octopusViewModelDetails)
        {
            // Create confirmation statement string based on which entities are null
            var emojiChar = char.ConvertFromUtf32(0x1F642); // Smiley Emoji :)
            
            var messageBeginningText = $"Please confirm, you are looking for the following:\n\n\n";

            var teamNameText = !string.IsNullOrEmpty(octopusViewModelDetails.Team) ?
                $"Team Name: {octopusViewModelDetails.Team}\n\n" : "";

            var languageNameText = !string.IsNullOrEmpty(octopusViewModelDetails.Language) ?
                $"Language Name: {octopusViewModelDetails.Language}\n\n" : "";
            
            var additionalText = teamNameText + languageNameText;
            
            var messageEndingText =
                $"Service Name: {octopusViewModelDetails.Service}\n\n" +
                $"Environment name: {octopusViewModelDetails.Environment}\n\n\n" +
                "Is this correct? " + emojiChar;
            
            string messageText = messageBeginningText + additionalText + messageEndingText;

            return messageText;
        }
        public static string FinalResult(ResponseViewModel response, OctopusViewModel result)
        {

            string finalResultText;
            
            if (!response.IsValidEnv && response.IsValidProject)
            {
                finalResultText = $"The project {result.SlugString} does not exist on the environment {result.Environment}. " +
                                        $"Please check the deployment again on a different environment.";

            }
            else if (!response.IsValidProject)
            {
                finalResultText = $"The project {result.SlugString} does not exist. Make sure you are entering a valid project name.";

            }
            else
            {
                finalResultText = $"Project Name: {result.SlugString}\n\n" + 
                                  $"Deployment Status: {response.State} " + 
                                  EmojiConverter.ConvertStatusToEmoji(response.State) + "\n\n" +
                                  $"Environment: {result.Environment}\n\n" +
                                  $"Version: {response.Version}";
                var errorMessageText = !string.IsNullOrEmpty(response.ErrorMessage) ?
                    $"\n\nError Message: {response.ErrorMessage}" : "";
                finalResultText += errorMessageText;
            }
            
            return finalResultText;
        }
    }
}