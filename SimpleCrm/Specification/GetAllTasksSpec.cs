
using SimpleCrm.Enums;

namespace SimpleCrm.Specification
{
    public class GetAllTasksSpec:BaseSpecification<Models.Tasks>
    {
        public GetAllTasksSpec():base()
        {
            Criteria = z => z.StartAt.Date == DateTime.Now.Date && z.Status != StatusEnums.Completed;
            Includes.Add(z=>z.User);
            OrderBy = z => z.EndAt;
        }
    }
}
