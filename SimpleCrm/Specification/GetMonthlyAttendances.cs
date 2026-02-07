using SimpleCrm.Models;

namespace SimpleCrm.Specification
{
    public class GetMonthlyAttendances:BaseSpecification<Attendance>
    {
        public GetMonthlyAttendances(string userId)
        {
            Includes.Add(e=>e.ApplicationUser);

            Criteria = e => e.ApplicationUserId == userId && e.Date >= DateTime.Now.AddDays(-35);
            OrderBy= e=>e.Date;
        }
    }
}
