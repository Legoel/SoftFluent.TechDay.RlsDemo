using AutoMapper;
using RlsDemo.Context.Model;

namespace RlsDemo.Web
{
    public class AutoMapperConfiguration : Profile
	{
		public AutoMapperConfiguration()
		{
			CreateMap<SensitiveDatum, SensitiveDatumDto>().ReverseMap();
		}
	}
}
