using AutoMapper;
using MondakiComics.Data;
using MondakiComics.DTO;

namespace MondakiComics.Configuration
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<User, UserReadOnlyDTO>().ReverseMap();
            CreateMap<UserRegisterDTO, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Password, opt => opt.Ignore())
            .ForMember(dest => dest.UserRole, opt => opt.Ignore())
            .ForMember(dest => dest.InsertedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Artworks, opt => opt.Ignore())
            .ForMember(dest => dest.ContactMessages, opt => opt.Ignore());

            // Artwork mappings
            CreateMap<Artwork, ArtworkReadOnlyDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images));

            CreateMap<ArtworkImage, ArtworkImageReadOnlyDTO>();
            CreateMap<ArtworkInsertDTO, Artwork>();
            CreateMap<ArtworkUpdateDTO, Artwork>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Category mappings
            CreateMap<ArtworkCategory, ArtworkCategoryReadOnlyDTO>();
            CreateMap<ArtworkCategoryInsertDTO, ArtworkCategory>();

            // Contact mappings
            CreateMap<ContactMessage, ContactMessageReadOnlyDTO>();

            // News mappings
            CreateMap<NewsPost, NewsPostReadOnlyDTO>();
            CreateMap<NewsPostInsertDTO, NewsPost>();
            CreateMap<NewsPostUpdateDTO, NewsPost>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}