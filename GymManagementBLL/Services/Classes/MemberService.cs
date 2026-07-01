using AutoMapper;
using GymManagementBLL.AttachmentService;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.MemberViewModel;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Classes;
using GymManagementDAL.Repositories.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAttachmentService _attachmentService;

        public MemberService(IUnitOfWork unitOfWork, IMapper mapper ,IAttachmentService attachmentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _attachmentService = attachmentService;
        }


        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync()
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync();
            if (members == null || !members.Any()) return [];

            var MemberViewModels = _mapper.Map<IEnumerable<MemberViewModel>>(members);

            return MemberViewModels;

        }
        public async Task<bool> CreateMemberAsync(CreateMemberViewModel createMember)
        {
            try
            {
                if (createMember == null) return false;


                if (await IsEmailExistsAsync(createMember.Email) || await IsPhoneExistsAsync(createMember.Phone)) return false;

                var PhotoName = await _attachmentService.UploadAsync("Members", createMember.PhotoFile);


                if(string.IsNullOrEmpty(PhotoName)) return false;
                    

                var member = _mapper.Map<Member>(createMember);
                member.Photo = PhotoName;


                await _unitOfWork.GetRepository<Member>().AddAsync(member);
                var IsCreated = await _unitOfWork.SaveChangesAsync() > 0;

                if(!IsCreated)
                    _attachmentService.Delete(PhotoName, "Members");
                   

                return IsCreated;
            }
            catch
            {
                return false;
            }



        }

        public async Task<MemberDetailsViewModel?> GetMemberDetailsAsync(int memberId)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId);

            if (member == null) return null;



            var viewModel = _mapper.Map<MemberDetailsViewModel>(member);


            var ActivememberShip = await _unitOfWork.GetRepository<MemberShip>().GetFirstOrDefaultAsync(MS => MS.MemberId == member.Id && MS.EndDate > DateTime.UtcNow);

            if (ActivememberShip is not null)
            {
                viewModel.MemberShipStartDate = ActivememberShip.CreatedAt.ToShortDateString();
                viewModel.MemberShipEndDate = ActivememberShip.EndDate.ToShortDateString();
                var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(ActivememberShip.PlanId);
                viewModel.PlanName = plan?.Name ?? "";
            }


            return viewModel;




        }

        public async Task<HealthRecordViewModel?> GetMemberHealthRecordDetailsAsync(int memberId)
        {
            var memberHealthRecord = await _unitOfWork.GetRepository<HealthRecord>().GetByIdAsync(memberId);

            if (memberHealthRecord == null) return null;

            var HealthRecordMapped = _mapper.Map<HealthRecordViewModel>(memberHealthRecord);

            return HealthRecordMapped;


        }

        public async Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int id)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id);

            if (member == null) return null;

            var memberMappedViewModel = _mapper.Map<MemberToUpdateViewModel>(member);

            return memberMappedViewModel;


        }

        public async Task<bool> UpdateMemberAsync(int id, MemberToUpdateViewModel memberToUpdate)
        {

            try
            {

                var EmailExists = await _unitOfWork.GetRepository<Member>()
                    .GetFirstOrDefaultAsync(member => member.Email == memberToUpdate.Email && member.Id != id);

                var PhoneExists = await _unitOfWork.GetRepository<Member>()
                    .GetFirstOrDefaultAsync(member => member.Phone == memberToUpdate.Phone && member.Id != id);

                if (EmailExists is not null || PhoneExists is not null) return false;


                var _memberRepository = _unitOfWork.GetRepository<Member>();

                var member = await _memberRepository.GetByIdAsync(id);

                if (member == null) return false;



                _mapper.Map(memberToUpdate, member);





                _memberRepository.Update(member);


                return await _unitOfWork.SaveChangesAsync() > 0;

            }
            catch
            {
                return false;
            }

        }

        public async Task<bool> RemoveMemberAsync(int memberId)
        {
            try
            {
                var _memberRepository = _unitOfWork.GetRepository<Member>();
                var member = await _memberRepository.GetByIdAsync(memberId);

                if (member == null) return false;

                var Sessions = await _unitOfWork.BookingRepository.GetAllAsync(X => X.MemberId == memberId);
                var SessionIds = Sessions.Select(X => X.SessionId);

                var HasActiveMemberSession = await _unitOfWork.GetRepository<Session>().GetAllAsync(X => SessionIds.Contains(X.Id) && X.StartDate > DateTime.UtcNow);



                if (HasActiveMemberSession.Any()) return false;


                var _memberShipRepository = _unitOfWork.GetRepository<MemberShip>();
                var MemberShips = await _memberShipRepository.GetAllAsync(MS => MS.MemberId == member.Id);



                if (MemberShips.Any())
                    foreach (var memberShip in MemberShips)
                        _memberShipRepository.Delete(memberShip);


                _memberRepository.Delete(member);

                var IsRemoved = await _unitOfWork.SaveChangesAsync() > 0;

                if (IsRemoved)
                    _attachmentService.Delete(member.Photo, "Members");

                return IsRemoved;

            }
            catch
            {
                return false;
            }
        }


        #region Helper

        private async Task<bool> IsEmailExistsAsync(string email)
        {
            var Exist = await _unitOfWork.GetRepository<Member>().GetFirstOrDefaultAsync(member => member.Email == email);
            return Exist is null ? false : true;
        }
        private async Task<bool> IsPhoneExistsAsync(string phone)
        {
           
            var Exist = await _unitOfWork.GetRepository<Member>().GetFirstOrDefaultAsync(member => member.Phone == phone);
            return Exist is null ? false : true;
        }


        #endregion
    }
}
