using SimpleCrm.Models;

namespace SimpleCrm.Specification
{
    public class GetAllSalesSpec:BaseSpecification<Sale>
    {
        public GetAllSalesSpec():base()
        {
            Includes.Add(z=>z.User);
            OrderByDesc = z => z.Date;
        }
        public GetAllSalesSpec(string UserId):base(z=>z.UserId==UserId)
        {
            Includes.Add(z=>z.User);
            OrderByDesc = z => z.Date;
        }
    }
}
