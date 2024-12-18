using SimpleCrm.Models;

namespace SimpleCrm.Specification
{
    public class GetFollowedClientsSpec:BaseSpecification<FollowedClient>
    {
        public GetFollowedClientsSpec():base(z=>z.Client.Status == Enums.ClientStatusEnum.Following)
        {
            Includes.Add(z => z.Client);
        }
    }
}
