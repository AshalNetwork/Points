using SimpleCrm.Models;

namespace SimpleCrm.Specification
{
    public class GetUserFollowingClientsSpec:BaseSpecification<Clients>
    {
        public GetUserFollowingClientsSpec(string UserId) : base(z =>z.UserId==UserId&& z.Status == Enums.ClientStatusEnum.Following)
        {
            
            OrderByDesc = z => z.CreatedAt;
            Includes.Add(z => z.CommingReason);

        }
    }
}
