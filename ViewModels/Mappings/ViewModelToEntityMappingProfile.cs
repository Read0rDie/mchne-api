using mchne_api.Models;
using AutoMapper;
 

namespace mchne_api.ViewModels.Mappings
{
    public class ViewModelToEntityMappingProfile : Profile
    {
        public ViewModelToEntityMappingProfile()
        {
            CreateMap<RegistrationViewModel, AppUser>()
                .ForMember(au => au.alias, map => map.MapFrom(vm => vm.Username))
                .ForMember(au => au.UserName, map => map.MapFrom(vm => vm.Email));
        }
    }
}