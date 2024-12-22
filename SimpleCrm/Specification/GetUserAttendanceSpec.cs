using SimpleCrm.Models;

namespace SimpleCrm.Specification
{
    public class GetUserAttendanceSpec:BaseSpecification<Attendance>
    {
        public GetUserAttendanceSpec(string userId, DateTime date)
        {
            Criteria = e=>e.ApplicationUserId==userId && e.Date.Date==date.Date;
        }
    }
}
