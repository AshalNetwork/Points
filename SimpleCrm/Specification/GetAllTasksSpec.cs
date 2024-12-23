
using SimpleCrm.Enums;

namespace SimpleCrm.Specification
{
    public class GetAllTasksSpec:BaseSpecification<Models.Tasks>
    {
        public GetAllTasksSpec():base(z=>z.Status==StatusEnums.UnderReview)
        {
            Includes.Add(z=>z.User);
            OrderBy = z => z.EndAt;
        }
    }
}
