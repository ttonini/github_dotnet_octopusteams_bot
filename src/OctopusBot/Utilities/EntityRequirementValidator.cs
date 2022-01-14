using System;
using System.Collections.Generic;
using OctopusBot.ViewModels;

namespace OctopusBot.Utilities
{
    public class EntityRequirementValidator
    {
        public bool IsTeamRequired(OctopusViewModel octopusViewModel)
        {
            // Checks for services that do not require a team name in the slug string
            var teamNameRequiredCheck = octopusViewModel.SlugStringSpecialCases.NoTeamNameRequired;
            
            return !teamNameRequiredCheck.Contains(octopusViewModel.Service);
        }
        
        public bool IsLanguageRequired(OctopusViewModel octopusViewModel)
        {
            // Checks for services that do not require a language name in the slug string
            var languageNameRequiredCheck = octopusViewModel.SlugStringSpecialCases.NoLanguageNameRequired;
            
            return !languageNameRequiredCheck.Contains(octopusViewModel.Service);
        }
    }
}