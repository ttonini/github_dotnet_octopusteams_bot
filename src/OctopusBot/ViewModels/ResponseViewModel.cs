// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with CoreBot .NET Template version v4.13.2


using OctopusBot.Utilities;

namespace OctopusBot.ViewModels
{
    public class ResponseViewModel
    {
        public string EnvironmentId { get; set; }

        public string Version { get; set; }

        public string State { get; set; }
        
        public string ErrorMessage { get; set; }

        public bool IsValidProject { get; set; }
        
        public bool IsValidEnv { get; set; }

        public bool IsSuccessfulDeployment { get; set; }
    }
}
