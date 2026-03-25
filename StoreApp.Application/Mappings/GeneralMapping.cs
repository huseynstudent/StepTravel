using AutoMapper;
using StoreApp.Application.Email.Commands;
using StoreApp.Domain.Entities;
using System;
namespace StoreApp.Application.Mappings
{
    public class GeneralMapping : Profile
    {
        public GeneralMapping()
        {
            CreateMap<RegisterUserCommand, User>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Birthday, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.Birthday)))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.IsConfirmed, opt => opt.MapFrom(src => false));
        }
    }
}