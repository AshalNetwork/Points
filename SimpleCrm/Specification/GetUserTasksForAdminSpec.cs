
using SimpleCrm.Enums;
using SimpleCrm.Models;

namespace SimpleCrm.Specification
{
    public class GetUserTasksForAdminSpec:BaseSpecification<Tasks>
    {
        public GetUserTasksForAdminSpec(string UserId,StatusEnums status) :base()
        {
            Criteria = z => z.Status == status&&z.UserId== UserId;
            OrderBy = z => z.EndAt.Date;

        }
    }
}
