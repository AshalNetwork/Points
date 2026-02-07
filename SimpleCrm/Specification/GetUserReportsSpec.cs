using DocumentFormat.OpenXml.Spreadsheet;
using SimpleCrm.Models;

namespace SimpleCrm.Specification
{
    public class GetUserReportsSpec:BaseSpecification<Report>
    {
        public GetUserReportsSpec(string UserId):base(z=>z.UserId==UserId)
        {
            Includes.Add(z => z.User);
            Criteria = e => e.UserId == UserId && e.Date >= DateTime.Now.AddDays(-38);

            OrderBy = z => z.Date;
        }
    }
}
