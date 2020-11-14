using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plex.Api;
using Plex.Api.Api;
using Plex.Api.Helpers;
using Plex.Api.Models;
using Plex.Api.Models.Friends;
using Plex.Api.Models.Server;
using Plex.Api.Models.Status;

namespace PlexMediaArchiver
{
    public class PlexAPI
    {
        protected readonly ServiceProvider ServiceProvider;
        protected readonly IConfiguration Configuration;
        protected readonly ClientOptions ClientOptions;

        private IPlexClient plexApi;
        private User plexUser;

        private string AuthenticationToken
        {
            get
            {
                return plexUser?.AuthenticationToken;
            } 
        }

        public PlexAPI()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;

            ClientOptions = new ClientOptions
            {
                Platform = "Web",
                Product = "API_UnitTests",
                DeviceName = "API_UnitTests",
                ClientId = "PlexApi",
                Version = "v1"
            };

            var services = new ServiceCollection();
            services.AddLogging();
            services.AddSingleton(ClientOptions);
            services.AddTransient<IPlexClient, PlexClient>();
            services.AddTransient<IApiService, ApiService>();
            services.AddTransient<IPlexRequestsHttpClient, PlexRequestsHttpClient>();

            ServiceProvider = services.BuildServiceProvider();

            SignIn();
        }

        private void SignIn()
        {
            plexApi = ServiceProvider.GetService<IPlexClient>();

            var login = ConfigurationManager.AppSettings["PlexLogin"];
            var password = ConfigurationManager.AppSettings["PlexPassword"];

            plexUser = plexApi.SignIn(login, password).Result; 
        }

        private Server GetPreferredServer()
        {
            var servers = GetServers();
            var preferredServer = ConfigurationManager.AppSettings["PlexServer"];

            if (servers.Any())
            {
                if (!string.IsNullOrEmpty(preferredServer))
                {
                    var server = servers.FirstOrDefault(s => s.Name == preferredServer);

                    if (server != null)
                    {
                        return server;
                    }
                    else
                    {
                        return servers[0];
                    }
                }
                else
                {
                    return servers[0];
                }
            }

            return null;
        }

        private List<Server> GetServers()
        {
            if (plexApi != null && AuthenticationToken != null)
            {
                return plexApi.GetServers(AuthenticationToken).Result;
            }

            return null;
        }

        public List<string> ServerNames()
        {
            var servers = GetServers();

            if (servers != null)
            {
                return servers.Select(s => s.Name).ToList();
            }

            return null;
        }

        public Plex.Api.Models.PlexMediaContainer GetMovieLibrary()
        {
            var plexServer = GetPreferredServer();
            var fullURL = plexServer.FullUri.ToString();
            var plexMediaContainer = plexApi.GetLibraries(plexServer.AccessToken, fullURL).Result;
            //Getting connection forcibly closed????
            //Not sure if the rest of this works
            //It does :)

            var movieLibrary = plexMediaContainer.MediaContainer.Directory.FirstOrDefault(c => c.Title == "Movies");
            var tvLibrary = plexMediaContainer.MediaContainer.Directory.FirstOrDefault(c => c.Title == "TV Shows");
            //Small update to the above...it gets the libraries now.

            Console.WriteLine(movieLibrary.Key);
            return plexApi.GetLibrary(plexServer.AccessToken, fullURL, movieLibrary.Key).Result;
        }
    }
}
