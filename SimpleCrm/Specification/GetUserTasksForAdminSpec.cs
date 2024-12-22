
using SimpleCrm.Enums;
using SimpleCrm.Models;

namespace SimpleCrm.Specification
{
    public class GetUserTasksForAdminSpec:BaseSpecification<Tasks>
    {
        public GetUserTasksForAdminSpec(string UserId,StatusEnums status) :base(z=>z.Status==status)
        {
            OrderBy = z => z.EndAt.Date;

        }
    }
}
