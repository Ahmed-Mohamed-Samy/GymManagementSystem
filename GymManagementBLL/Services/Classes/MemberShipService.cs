using AutoMapper;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.MemberShipViewModels;
using GymManagementDAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.Services.Classes
{
    public class MemberShipService : IMemberShipService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MemberShipService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IEnumerable<MemberShipViewModel> GetAllMemberShips()
        {
            var MemberShips = _unitOfWork.MemberShipRepository.GetMemberShipsWithMemberAndPlan();

            if (MemberShips is null || !MemberShips.Any()) return [];

            var MappedMemberShips = _mapper.Map<IEnumerable<MemberShipViewModel>>(MemberShips);

            return MappedMemberShips;
        }
    }
}
