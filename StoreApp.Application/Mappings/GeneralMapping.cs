using AutoMapper;
using StoreApp.Application.CQRS.Auth.Command.Request; // Yeni Auth Request modelin
using StoreApp.Application.Email.Commands;
using StoreApp.Domain.Entities;

namespace StoreApp.Application.Mappings
{
    public class GeneralMapping : Profile
    {
        public GeneralMapping()
        {
            // 1. RegisterUserCommandRequest -> User (Auth qovluğundakı peşəkar model)
            CreateMap<RegisterUserCommandRequest, User>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.Surname))
                .ForMember(dest => dest.Birthday, opt => opt.MapFrom(src => src.Birthday))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Fin, opt => opt.MapFrom(src => src.Fin))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.IsConfirmed, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false));

            // 2. Əgər Login üçün də mapping lazımdırsa bura əlavə edə bilərsən
            // CreateMap<LoginUserCommandRequest, User>();
        }
    }
}