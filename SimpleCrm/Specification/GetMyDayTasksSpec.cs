using SimpleCrm.Models;

namespace SimpleCrm.Specification
{
    public class GetMyDayTasksSpec:BaseSpecification<Tasks>
    {
        public GetMyDayTasksSpec(string UserId):base(z=>z.UserId==UserId)
        {
            Criteria = z=>z.StartAt.Date == DateTime.Now.Date;
        }
    }
}
