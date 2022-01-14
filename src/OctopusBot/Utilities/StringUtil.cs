using System.Text;
using OctopusBot.ViewModels;

namespace OctopusBot.Utilities
{
    public class StringUtil
    {
        private static readonly EntityRequirementValidator Validator = new();
        private const int MAXCHARS = 80;
        
        public string ConvertToSlugString(OctopusViewModel result)
        {
            StringBuilder sb;
            
            if (!Validator.IsLanguageRequired(result))
            {
                sb = Validator.IsTeamRequired(result) ? new StringBuilder(result.Team + '-' + result.Service, MAXCHARS) : new StringBuilder(result.Service, MAXCHARS);
            }
            else
            {
                sb = new StringBuilder(result.Team + '-', MAXCHARS);
                sb.Append(result.Language + '-' + result.Service);
            }
            // A new string should be returned in the format team-language-service, team-service, or service 
            return sb.ToString();
        }
    }
}