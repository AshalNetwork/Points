using SimpleCrm.Models;

namespace SimpleCrm.Specification
{
    public class GetUserReportsSpec:BaseSpecification<Report>
    {
        public GetUserReportsSpec(string UserId):base(z=>z.UserId==UserId)
        {
            OrderByDesc = z => z.Date;
        }
    }
}
