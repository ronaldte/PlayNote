using AutoMapper;

namespace PlayNote.Profiles
{
    /// <summary>
    /// Maping profiles for <see cref="Entities.Rating"/>.
    /// </summary>
    public class RatingProfile : Profile
    {
        /// <summary>
        /// Created new mapping for <see cref="Entities.Rating"/> and its DTOs.
        /// </summary>
        public RatingProfile()
        {
            CreateMap<Entities.Rating, Models.RatingDto>();
            CreateMap<Models.RatingForCreationDto, Entities.Rating>();
            CreateMap<Models.RatingForUpdateDto, Entities.Rating>();
            CreateMap<Entities.Rating, Models.RatingForUpdateDto>();
        }
    }
}
