using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.ViewModels.MemberShipViewModels
{
    public class MemberShipViewModel
    {
        public int Id { get; set; }
        public string MemberName { get; set; } = null!;
        public string PlanName { get; set; } = null!;
        public string StartDate { get; set; } = null!;
        public string EndDate { get; set; } = null!;
        public int MemberId { get; set; }
    }
}
