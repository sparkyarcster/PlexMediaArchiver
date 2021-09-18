using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace PlexMediaArchiver.Classes
{
    public static class Constants
    {
        public static string DatabaseName
        {
            get
            {
                return GetSettingValue("DatabaseName", "pma");
            }
        }

        public static string PlexLogin
        {
            get
            {
                return GetSettingValue("PlexLogin", "");
            }
        }

        public static string PlexPassword
        {
            get
            {
                return GetSettingValue("PlexPassword", "");
            }
        }

        public static string PlexServer
        {
            get
            {
                return GetSettingValue("PlexServer", "");
            }
        }

        public static string TautulliAPIKey
        {
            get
            {
                return GetSettingValue("TautulliAPIKey", "");
            }
        }

        public static string TautulliPort
        {
            get
            {
                return GetSettingValue("TautulliPort", "");
            }
        }

        public static int MovieAge
        {
            get
            {
                int movieAge;

                if (int.TryParse(GetSettingValue("MovieAge", "730"), out movieAge))
                {
                    return movieAge;
                }

                return 730;
            }
        }

        public static int IgnoreMoviesLessThanDaysOld
        {
            get
            {
                int ignoreMovies;

                if (int.TryParse(GetSettingValue("IgnoreMoviesLessThanDaysOld", "365"), out ignoreMovies))
                {
                    return ignoreMovies;
                }

                return 365;
            }
        }

        public static int TVAge
        {
            get
            {
                int tvAge;

                if (int.TryParse(GetSettingValue("TVAge", "730"), out tvAge))
                {
                    return tvAge;
                }

                return 730;
            }
        }

        public static int IgnoreTVShowsLessThanDaysOld
        {
            get
            {
                int ignoreShows;

                if (int.TryParse(GetSettingValue("IgnoreTVShowsLessThanDaysOld", "365"), out ignoreShows))
                {
                    return ignoreShows;
                }

                return 365;
            }
        }

        public static bool RefreshDatabase
        {
            get
            {
                bool refreshDatabase;

                if (bool.TryParse(GetSettingValue("RefreshDatabase", "true"), out refreshDatabase))
                {
                    return refreshDatabase;
                }

                return true;
            }
        }

        private static string GetSettingValue(string configName, string defaultValue)
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings[configName]))
            {
                return ConfigurationManager.AppSettings[configName];
            }

            return defaultValue;
        }

        public static string ReportLocation
        {
            get
            {
                return GetSettingValue("ReportLocation", "");
            }
        }
    }
}
