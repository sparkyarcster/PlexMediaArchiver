using AutoMapper;
using System;

namespace PlexMediaArchiver.Classes
{
    public static class Mapper
    {
        public static IMapper mapper;

        public static void Configure()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            configuration.AssertConfigurationIsValid();

            mapper = configuration.CreateMapper();
        }
    }
}
