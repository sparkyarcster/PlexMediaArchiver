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

        private Plex.Api.Models.PlexMediaContainer GetLibrary(string libraryName)
        {
            var plexServer = GetPreferredServer();
            var fullURL = plexServer.FullUri.ToString();
            var plexMediaContainer = plexApi.GetLibraries(plexServer.AccessToken, fullURL).Result;
            //If this errors for "forcibly closed by the remote host", go to your Plex settings, choose "Network" and set it to "Preferred".
            //If it is set to "required", it will error for not using https. But the HTTPS cert for the IP is not valid.

            var library = plexMediaContainer.MediaContainer.Directory.FirstOrDefault(c => c.Title == libraryName);

            if (library == null)
            {
                return null;
            }

            return plexApi.GetLibrary(plexServer.AccessToken, fullURL, library.Key).Result;
        }

        public Plex.Api.Models.PlexMediaContainer GetMovieLibrary()
        {
            return GetLibrary("Movies");
        }

        public Plex.Api.Models.PlexMediaContainer GetTVLibrary()
        {
            return GetLibrary("TV Shows");
        }

        private DateTime? ConvertLastViewed(int lastViewedAt)
        {
            DateTime? lastViewed = null;

            if (lastViewedAt > 0)
            {
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(lastViewedAt);
                lastViewed = DateTime.SpecifyKind(dateTimeOffset.DateTime, DateTimeKind.Utc);
            }

            return lastViewed.HasValue ? lastViewed.Value.ToLocalTime() : (DateTime?)null;
        }

        public MovieViewModel GetMovieByTitle(string fullOrPartialTitle)
        {
            List<MovieViewModel> movie = new List<MovieViewModel>();
            var movieLibrary = GetMovieLibrary();
            var metaData = movieLibrary.MediaContainer.Metadata.FirstOrDefault(movieLibrary => movieLibrary.Title.Contains(fullOrPartialTitle));

            return new MovieViewModel() {
                LastViewed = ConvertLastViewed(metaData.LastViewedAt),
                MetaData = metaData
            };
        }

        public List<MovieViewModel> GetMoviesByTitle(string fullOrPartialTitle)
        {
            List<MovieViewModel> movies = new List<MovieViewModel>();
            var movieLibrary = GetMovieLibrary();

            foreach (var movie in movieLibrary.MediaContainer.Metadata.Where(movieLibrary => movieLibrary.Title.Contains(fullOrPartialTitle)))
            {
                movies.Add(new MovieViewModel()
                {
                    LastViewed = ConvertLastViewed(movie.LastViewedAt),
                    MetaData = movie
                });
            }

            return movies;
        }

        public List<MovieViewModel> GetMoviesNotViewed(int days = 60)
        {
            List<MovieViewModel> movies = new List<MovieViewModel>();
            var movieLibrary = GetMovieLibrary();

            foreach (var movie in movieLibrary.MediaContainer.Metadata)
            {
                DateTime? lastViewed = ConvertLastViewed(movie.LastViewedAt);

                if (!lastViewed.HasValue || lastViewed.Value < DateTime.UtcNow.AddDays(days * -1))
                {
                    movies.Add(new MovieViewModel()
                    {
                        LastViewed = lastViewed.HasValue ? lastViewed.Value.ToLocalTime() : (DateTime?)null,
                        MetaData = movie
                    });
                }
            }

            return movies;
        }
    }
}
