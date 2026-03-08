using SimpleCrm.Models;

namespace SimpleCrm.Specification
{
    /// <summary>
    /// Gets today's attendance record for a user that has check-in but no check-out yet (same record to update on checkout).
    /// </summary>
    public class GetTodayOpenAttendanceSpec : BaseSpecification<Attendance>
    {
        public GetTodayOpenAttendanceSpec(string applicationUserId, DateTime date)
        {
            Criteria = e => e.ApplicationUserId == applicationUserId
                && e.Date.Date == date.Date
                && e.CheckOut == TimeSpan.Zero;
        }
    }
}
