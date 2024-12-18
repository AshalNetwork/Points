using SimpleCrm.Models;

namespace SimpleCrm.Specification
{
    public class GetAdminCancelledClientsSpec:BaseSpecification<Clients>
    {
        public GetAdminCancelledClientsSpec() : base(z => z.Status == Enums.ClientStatusEnum.Deleted)
        {
            
            OrderByDesc = z => z.CreatedAt;
            Includes.Add(z => z.CommingReason);
        }
    }
}
