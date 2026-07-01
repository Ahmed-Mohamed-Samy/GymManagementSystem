using GymManagementDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementDAL.Repositories.Interfaces
{
    public interface ISessionRepository : IGenericRepostitory<Session>
    {
        Task<IEnumerable<Session>> GetAllSessionsWithTrainersAndCategoriesAsync();

        Task<int> GetCountOfBookedSlotsAsync(int sessionId);

        Task<Session?> GetSessionWithTrainerAndCategoryAsync(int sessionId);
    }
}
