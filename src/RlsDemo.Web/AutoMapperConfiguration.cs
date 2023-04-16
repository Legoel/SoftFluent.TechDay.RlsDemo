using AutoMapper;
using RlsDemo.Context.Model;
using RslDemo.Context;

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
