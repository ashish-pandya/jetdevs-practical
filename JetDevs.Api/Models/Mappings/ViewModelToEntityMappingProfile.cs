using AutoMapper;
using JetDevs.Api.Models.DbEntities;
using JetDevs.Api.Models.ViewModels;

namespace JetDevs.Api.Models.Mappings
{
	/// <summary>
	/// AutoMapper Mappings
	/// </summary>
	public class ViewModelToEntityMappingProfile : Profile
	{
		/// <summary>
		/// Mapping used by AutoMapper
		/// </summary>
		public ViewModelToEntityMappingProfile()
		{
			CreateMap<RegistrationUserViewModel, User>().ForMember(au => au.UserName, map => map.MapFrom(vm => vm.Email));
			CreateMap<BaseUserViewModel, User>().ForMember(au => au.UserName, map => map.MapFrom(vm => vm.Email));
		}
	}
}
