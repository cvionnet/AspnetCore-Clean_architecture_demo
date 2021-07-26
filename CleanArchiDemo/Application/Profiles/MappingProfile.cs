using Application.Features._ViewModels;
using Domain.Entities;
using AutoMapper;

namespace Application.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Company
            CreateMap<Company, CompanyVM>();

            // Company - Set the dest field (CompanyVM.CountryName) to take the value in Company.Country.CountryName)
            CreateMap<Company, CompanyVM>().ForMember(dest => dest.CountryName,
                opts => opts.MapFrom(src => src.Country.CountryName));
        }
    }
}
