using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Mvc;
using SimpleCrm.Models;
using System.Data;

namespace SimpleCrm.Services
{
    public static class GenerateExcel
    {
       
        public static DataTable GenerateAttendanceSheet(List<Attendance> AttendanceList)
        {

            // Create a new DataTable to hold the data
            DataTable dt = new DataTable("Attendance");
            dt.Columns.AddRange(new DataColumn[4]
            {
                new DataColumn("Date"),
                new DataColumn("Check In"),
                new DataColumn("Check Out"),
                new DataColumn("Late"),
            });
             
            var egyptTime = new TimeSpan(9, 0, 0);
                
            
            
            // Add client data to the DataTable
            foreach (var attendance in AttendanceList)
            {
                var lat = attendance.CheckIn - @egyptTime;
                var date = attendance.Date;
                var checkIn = attendance.CheckIn;
                var checkout = attendance.CheckOut;
                var Late = new TimeSpan(@lat.Hours , @lat.Minutes , @lat.Seconds);

                dt.Rows.Add(date, checkIn, checkout, Late);
            }
            return dt;
        }
    }
}
