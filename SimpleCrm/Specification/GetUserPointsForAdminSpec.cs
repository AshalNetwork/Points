using DocumentFormat.OpenXml.Spreadsheet;
using SimpleCrm.Models;

namespace SimpleCrm.Specification
{
    public class GetUserPointsForAdminSpec:BaseSpecification<UserPoint>
    {
        public GetUserPointsForAdminSpec(string UserId,DateTime startOfWeek,DateTime endOfWeek) :base(p => p.UserId == UserId && p.DateTime.Date >= startOfWeek.Date && p.DateTime.Date <= endOfWeek.Date)
        {
            
        }
    }
}
