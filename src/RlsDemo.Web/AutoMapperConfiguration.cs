using AutoMapper;
using RlsDemo.Context.Model;

namespace RlsDemo.Web
{
    public class AutoMapperConfiguration : Profile
	{
		public AutoMapperConfiguration()
		{
			CreateMap<SensitiveDatumType, SensitiveDatumTypeDto>().ReverseMap();
			CreateMap<SensitiveDatum, SensitiveDatumDto>().ReverseMap();
		}
	}
}
