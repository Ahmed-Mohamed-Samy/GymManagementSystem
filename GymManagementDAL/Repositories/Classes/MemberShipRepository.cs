using GymManagementDAL.Data.Contexts;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementDAL.Repositories.Classes
{
    public class MemberShipRepository : GenericRepostitory<MemberShip>, IMemberShipRepository
    {
        private readonly GymDbContext _dbContext;

        public MemberShipRepository(GymDbContext dbContext) : base(dbContext) 
        {
            _dbContext = dbContext;
        }

        public MemberShip? GetFirstMemberShip(Func<MemberShip, bool>? filter = null) => _dbContext.MemberShips.FirstOrDefault(filter ?? (_ => true));


        public IEnumerable<MemberShip> GetMemberShipsWithMemberAndPlan(Func<MemberShip, bool>? filter = null)
        => _dbContext.MemberShips.Include(MS => MS.Member).Include(MS => MS.Plan).Where(filter ?? (_ => true)).ToList();
        
    }
}
