using SimpleCrm.Models;

namespace SimpleCrm.Specification
{
    public class GetAdminFollowingClientsSpec:BaseSpecification<Clients>
    {
        public GetAdminFollowingClientsSpec():base(z => z.Status == Enums.ClientStatusEnum.Following)
        {
            
            OrderByDesc = z => z.CreatedAt;
            Includes.Add(z => z.CommingReason);
        }
    }
}
