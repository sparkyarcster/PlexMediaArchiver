using AutoMapper;
using System;

namespace PlexMediaArchiver.Classes
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Tautulli.MediaInfoData, PMAData.Model.Movie>()
                .ForMember(d => d.ID, o => o.MapFrom(s => int.Parse(s.rating_key)))
                .ForMember(d => d.FileSize, o => o.MapFrom(s => s.file_size ?? 0))
                .ForMember(d => d.LastPlayed, o => o.MapFrom(s => ConvertLastViewed(s.last_played)))
                .ForMember(d => d.Added, o => o.MapFrom(s => ConvertLastViewed(s.added_at)))
                .ForMember(d => d.GenericData, o => o.Ignore())
                .ForMember(d => d.Title, o => o.MapFrom(s => s.title))
                .ForMember(d => d.Year, o => o.MapFrom(s => s.year))
                .ForMember(d => d.IsArchived, o => o.Ignore())
                .ForMember(d => d.IsCurrent, o => o.Ignore());

            CreateMap<Tautulli.MediaInfoData, PMAData.Model.TVShow>()
                .ForMember(d => d.ID, o => o.MapFrom(s => int.Parse(s.rating_key)))
                .ForMember(d => d.FileSize, o => o.MapFrom(s => s.file_size ?? 0))
                .ForMember(d => d.LastPlayed, o => o.MapFrom(s => ConvertLastViewed(s.last_played)))
                .ForMember(d => d.Added, o => o.MapFrom(s => ConvertLastViewed(s.added_at)))
                .ForMember(d => d.GenericData, o => o.Ignore())
                .ForMember(d => d.Title, o => o.MapFrom(s => s.title))
                .ForMember(d => d.Year, o => o.MapFrom(s => s.year))
                .ForMember(d => d.IsArchived, o => o.Ignore())
                .ForMember(d => d.IsCurrent, o => o.Ignore());

            CreateMap<Tautulli.User, PMAData.Model.User>()
                .ForMember(d => d.GenericData, o => o.Ignore())
                .ForMember(d => d.ID, o => o.MapFrom(s => s.UserID))
                .ForMember(d => d.LastActivity, o => o.MapFrom(s => ConvertLastViewed(s.LastSeen)))
                .ForMember(d => d.LastLogin, o => o.Ignore())
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.UserName));
        }

        private DateTime? ConvertLastViewed(int? lastViewedAt)
        {
            DateTime? lastViewed = null;

            if (!lastViewedAt.HasValue)
            {
                lastViewedAt = 0;
            }

            try
            {
                if (lastViewedAt.Value > 0)
                {
                    DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(lastViewedAt.Value);
                    lastViewed = DateTime.SpecifyKind(dateTimeOffset.DateTime, DateTimeKind.Utc);
                }

                return lastViewed.HasValue ? lastViewed.Value.ToLocalTime() : (DateTime?)null;
            }
            catch (Exception ex)
            {
                Classes.AppLogger.log.Error(ex.Message);
                return null;
            }
        }
    }
}
