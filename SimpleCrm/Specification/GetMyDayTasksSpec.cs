using SimpleCrm.Enums;
using SimpleCrm.Models;

namespace SimpleCrm.Specification
{
    public class GetMyDayTasksSpec:BaseSpecification<Tasks>
    {
        public GetMyDayTasksSpec(string UserId):base()
        {
            Criteria = z => z.UserId == UserId && z.StartAt.Date == DateTime.Now.Date&&( z.Status == StatusEnums.Pending || z.Status == StatusEnums.InProgress);
            OrderBy = z => z.EndAt.Date;
        }
    }
}
