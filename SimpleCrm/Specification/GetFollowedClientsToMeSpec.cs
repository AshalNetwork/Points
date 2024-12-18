using SimpleCrm.Models;

namespace SimpleCrm.Specification
{
    public class GetFollowedClientsToMeSpec:BaseSpecification<FollowedClient>
    {
        public GetFollowedClientsToMeSpec(string UserId):base(z=>z.ToId==UserId)
        {
            Includes.Add(z => z.Client.CommingReason);

        }
    }
}
