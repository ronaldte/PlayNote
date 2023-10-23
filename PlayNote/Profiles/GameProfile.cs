using AutoMapper;

namespace PlayNote.Profiles
{
    /// <summary>
    /// Maping profiles for <see cref="Entities.Game"/>.
    /// </summary>
    public class GameProfile : Profile
    {
        /// <summary>
        /// Created new mapping for <see cref="Entities.Game"/> and its DTOs.
        /// </summary>
        public GameProfile()
        {
            CreateMap<Entities.Game, Models.GameDto>();
            CreateMap<Entities.Game, Models.GameWithoutRatingsDto>();
        }
    }
}
