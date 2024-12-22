using SimpleCrm.Models;

namespace SimpleCrm.Specification
{
    public class GetUserByEmail:BaseSpecification<ApplicationUser>
    {
        public GetUserByEmail(string email)
        {
            Criteria = e => e.Email == email;
        }
    }
}
