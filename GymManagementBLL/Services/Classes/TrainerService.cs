using AutoMapper;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.TrainerViewModels;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.Services.Classes
{
    public class TrainerService : ITrainerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TrainerService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> CreateTrainerAsync(CreateTrainerViewModel createTrainer)
        {
            try
            {
                if (createTrainer is null) return false;


                if (await IsEmailExistsAsync(createTrainer.Email) || await IsPhoneExistsAsync(createTrainer.Phone)) return false;

                var trainer = _mapper.Map<Trainer>(createTrainer);

                await _unitOfWork.GetRepository<Trainer>().AddAsync(trainer);

                return await _unitOfWork.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }

        }

        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync()
        {
            var Trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync();

            if (Trainers is null || !Trainers.Any()) return [];

            var trainersMappedViewModel = _mapper.Map<IEnumerable<TrainerViewModel>>(Trainers);

            return trainersMappedViewModel;

        }

        public async Task<TrainerDetailsViewModel?> GetTrainerDetailsAsync(int id)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id);


            if(trainer is null) return null;
            var TrainerMapped = _mapper.Map<TrainerDetailsViewModel>(trainer);

            return TrainerMapped;

        }

        public async Task<TrianerToUpdateViewModel?> GetTrainerToUpdateAsync(int id)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id);

            if(trainer is null) return null;

            var TrainerToUpdate = _mapper.Map<TrianerToUpdateViewModel>(trainer);


            return TrainerToUpdate;
        }

        public async Task<bool> RemoveTrainerAsync(int id)
        {
            try
            {
                
                var trainerRepo = _unitOfWork.GetRepository<Trainer>();
                var trainer = await trainerRepo.GetByIdAsync(id);

                if(trainer == null) return false;

                var hasFutureSession = await _unitOfWork.GetRepository<Session>().GetAllAsync(S => S.TrainerId == trainer.Id && S.StartDate > DateTime.UtcNow);

                if(hasFutureSession.Any()) return false;


                trainerRepo.Delete(trainer);
                return await _unitOfWork.SaveChangesAsync() > 0;

            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateTrainerAsync(int id, TrianerToUpdateViewModel updateTrianer)
        {
            try
            {
                if (updateTrianer == null) return false;
                var TrainerRepo = _unitOfWork.GetRepository<Trainer>();

                var trainer = await TrainerRepo.GetByIdAsync(id);

                var IsEmailExists = await TrainerRepo.GetFirstOrDefaultAsync(T => T.Email == updateTrianer.Email && T.Id != id );
                var IsPhoneExists = await TrainerRepo.GetFirstOrDefaultAsync(T => T.Phone == updateTrianer.Phone && T.Id != id );

                if (trainer is null || IsEmailExists is not null || IsPhoneExists is not null) return false;

                _mapper.Map(updateTrianer, trainer);



                TrainerRepo.Update(trainer);

                return await _unitOfWork.SaveChangesAsync() > 0;
                
            }
            catch
            {
                return false;
            }
        }


        #region Helper Methods

        async Task<bool> IsEmailExistsAsync(string email)  => await _unitOfWork.GetRepository<Trainer>().GetFirstOrDefaultAsync(T => T.Email == email) is not null;
        async Task<bool> IsPhoneExistsAsync(string phone)  => await _unitOfWork.GetRepository<Trainer>().GetFirstOrDefaultAsync(T => T.Phone == phone) is not null;





        #endregion
    }
}
