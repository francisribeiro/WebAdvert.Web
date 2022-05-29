using AutoMapper;
using WebAdvert.Web.Models;
using WebAdvert.Web.Models.Home;
using WebAdvert.AdvertApi.Models;
using WebAdvert.Web.ServiceClients;
using WebAdvert.Web.Models.AdvertManagement;

namespace WebAdvert.Web.Classes
{
    public class WebsiteProfiles : Profile
    {
        public WebsiteProfiles()
        {
            CreateMap<CreateAdvertModel, CreateAdvertViewModel>().ReverseMap();

            CreateMap<AdvertModel, Advertisement>().ReverseMap();

            CreateMap<Advertisement, IndexViewModel>()
                .ForMember(dest => dest.Title, src => src.MapFrom(field => field.Title))
                .ForMember(dest => dest.Image, src => src.MapFrom(field => field.FilePath));

            CreateMap<AdvertType, SearchViewModel>()
                .ForMember(dest => dest.Id, src => src.MapFrom(field => field.Id))
                .ForMember(dest => dest.Title, src => src.MapFrom(field => field.Title));
        }
    }
}