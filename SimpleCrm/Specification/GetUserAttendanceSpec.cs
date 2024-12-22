using SimpleCrm.Models;

namespace SimpleCrm.Specification
{
    public class GetUserAttendanceSpec:BaseSpecification<Attendance>
    {
        public GetUserAttendanceSpec(string email, DateTime date)
        {
            Includes.Add(e => e.ApplicationUser);
            Criteria = e=>e.ApplicationUser.Email == email && e.Date.Date==date.Date;
        }
    }
}
