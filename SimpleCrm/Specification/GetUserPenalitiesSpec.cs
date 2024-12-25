using SimpleCrm.Models;

namespace SimpleCrm.Specification
{
    public class GetUserPenalitiesSpec : BaseSpecification<Penality>
    {
        public GetUserPenalitiesSpec(string UserId) : base(z => z.UserId == UserId)
        {
            Includes.Add(z => z.User);

            OrderByDesc = z => z.Date;
        }
        public GetUserPenalitiesSpec() : base()
        {
            Includes.Add(z => z.User);
            OrderByDesc = z => z.Date;
        }
    }
}
