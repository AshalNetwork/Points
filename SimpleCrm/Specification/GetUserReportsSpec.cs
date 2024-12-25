using SimpleCrm.Models;

namespace SimpleCrm.Specification
{
    public class GetUserReportsSpec:BaseSpecification<Report>
    {
        public GetUserReportsSpec(string UserId):base(z=>z.UserId==UserId)
        {
            Includes.Add(z => z.User);
            OrderByDesc = z => z.Date;
        }
    }
}
