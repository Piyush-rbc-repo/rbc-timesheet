using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timesheet.Modal
{
    public class ProjectDashboardModal
    {
        public List<ResourceCostModal> resourceCostModal = new List<ResourceCostModal>();
        public CostModal costModal = new CostModal();

    }

    public class ResourceCostModal
    {
        public string FullName { get; set; }
        public string Category { get; set; }
        
        public decimal TotalHrs { get;set; }
        
        public decimal TotalHrsThisMonth { get; set; }
        public decimal TotalBilledThisMonth { get; set; }
        public decimal TotalUnBilledHrsThisMonth { get; set; }

        public decimal TotalHrsOtherMonths { get; set; }
        public decimal TotalBilledOtherMonths { get; set; }
        public decimal TotalUnBilledHrsOtherMonths { get; set; }

        public decimal? CostPerHr { get; set; }
        public decimal CostThisMonth { get; set; }
        public decimal EarlierCost { get; set; }
        public decimal TotalCost { get; set; }


        
    }
    public class CostModal
    {
        public string Name { get; set; }
        public decimal? Dev { get; set; }
        public decimal? QA { get; set; }
        public decimal? SDLC { get; set; }


    }

    public class ProjectCrBasedSearchModal
    {
        public int? CrId { get; set; }
        public int? ProjectId { get; set; }
        public int month { get; set; }
        
    }
        
}
