using SimpleCrm.Models;

namespace SimpleCrm.Specification
{
    public class GetUserCancelledClientsSpec:BaseSpecification<Clients>
    {
        public GetUserCancelledClientsSpec( string UserId) : base(z => z.UserId == UserId && z.Status == Enums.ClientStatusEnum.Deleted)
        {
            
            OrderByDesc = z => z.CreatedAt;
            Includes.Add(z => z.CommingReason);

        }
    }
}
