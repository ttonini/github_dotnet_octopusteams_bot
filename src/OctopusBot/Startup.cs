// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with CoreBot .NET Template version v4.13.2

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using OctopusBot.Bots;
using OctopusBot.Dialogs;
using OctopusBot.Services;
using Microsoft.Azure.Cosmos.Fluent;


namespace OctopusBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers().AddNewtonsoftJson();


            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Create the storage we'll be using for User and Conversation state. (Memory is great for testing purposes.)
            services.AddSingleton<IStorage, MemoryStorage>();

            // Create the User state. (Used in this bots Dialog implementation.)
            services.AddSingleton<UserState>();

            // Create the Conversation state. (Used by the Dialog system itself.)
            services.AddSingleton<ConversationState>();

            // Register LUIS recognizer
            services.AddSingleton<OctopusBotRecognizer>();

            // Register the DeploymentDialog.
            services.AddSingleton<DeploymentDialog>();

            services.AddSingleton<AuthenticationDialog>();

            // Register connection to grab data from db
            services.AddSingleton<IGetSynonymsService, GetSynonymsService>();

            // Register the octopus api connections
            services.AddSingleton<OctopusApi.OctopusApi>();

            services.Configure<AppSettings>(Configuration);

            // The MainDialog that will be run by the bot.
            services.AddSingleton<MainDialog>();

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, AuthAndWelcomeBot<MainDialog>>();

            //services.AddSingleton<IConfiguration>(Configuration);

            // Setup logging to utilize Seq
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddSeq(Configuration.GetSection("Seq"));
            });
            RegisterCosmosServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                // TODO: add more detailed logging
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseWebSockets()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
        }

        public static void RegisterCosmosServices(IServiceCollection services)
        {
            services.AddSingleton((services) =>
            {
                var config = services.GetService<IConfiguration>();
                var cosmosDbConnection = new CosmosDbConnection();
                config.GetSection(nameof(CosmosDbConnection)).Bind(cosmosDbConnection);
                return cosmosDbConnection;
            });
            services.AddSingleton(s =>
            {
                var dbSettings = s.GetService<CosmosDbConnection>();
                return new CosmosClientBuilder(dbSettings.EndpointUri, dbSettings.PrimaryKey).Build();
            });
            services.AddTransient<ICosmosService, CosmosService>();
        }
    }
}