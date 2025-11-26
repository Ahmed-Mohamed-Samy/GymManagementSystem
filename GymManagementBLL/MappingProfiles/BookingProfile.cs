using AutoMapper;
using GymManagementBLL.ViewModels.BookingViewModels;
using GymManagementDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.MappingProfiles
{
    public class BookingProfile : Profile
    {
        public BookingProfile()
        {
            CreateMap<MemberSession, MemberForSessionViewModel>()
                .ForMember(dest => dest.MemberName, opt => opt.MapFrom(src => src.Member.Name))
                .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(src => src.CreatedAt.ToShortDateString()));

            CreateMap<CreateBookingViewModel, MemberSession>()
                .ForMember(dest => dest.IsAttended, opt => opt.Ignore());

        }
    }
}
