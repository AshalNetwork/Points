using SimpleCrm.Models;

namespace SimpleCrm.Specification
{
    public class GetAttendanceSpec:BaseSpecification<Attendance>
    {
        public GetAttendanceSpec()
        {
            Includes.Add(e=>e.ApplicationUser);
            Criteria= e=>e.Date==DateTime.Now.Date;
        }
    }
}
