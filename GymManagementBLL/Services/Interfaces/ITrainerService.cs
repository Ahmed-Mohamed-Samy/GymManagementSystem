using GymManagementBLL.ViewModels.TrainerViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.Services.Interfaces
{
    public interface ITrainerService
    {
        Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync();

        Task<bool> CreateTrainerAsync(CreateTrainerViewModel createTrainer);

        Task<TrainerDetailsViewModel?> GetTrainerDetailsAsync(int id);

        Task<TrianerToUpdateViewModel?> GetTrainerToUpdateAsync(int id);

        Task<bool> UpdateTrainerAsync(int id , TrianerToUpdateViewModel updateTrianer);

        Task<bool> RemoveTrainerAsync(int id);

    }
}
