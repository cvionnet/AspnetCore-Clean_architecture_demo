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
        }
    }
}
