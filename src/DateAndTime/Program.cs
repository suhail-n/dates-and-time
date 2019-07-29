using System.Globalization;
using System;

namespace DateAndTime
{
    class Program
    {
        static void Main(string[] args)
        {
            // ISO 8601 to remove Date ambiguity
            // year-month-day-hour-Time Delimiter-minute-second-time zone offset or zulu time(z)
            // 2019-06-10-T18:00:00+00:00
            // good practice to always work with UTC date time

            // TimeZoneInfoDateTime();
            DateAndTimeOffset();

        }
        static void TimeZoneInfoDateTime()
        {
            // represents the current time on the system. Avoid using since it varies system to system
            var now = DateTime.Now;
            Console.WriteLine("Local System Time Now: " + now);

            // get a list of all available timezone ids
            // foreach (TimeZoneInfo timeZone in TimeZoneInfo.GetSystemTimeZones()){  
            //     Console.WriteLine(timeZone.Id);  
            // }  

            // get timezone for sydney
            TimeZoneInfo sydneyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Australia/Sydney");
            // var timeZones = TimeZoneInfo.GetSystemTimeZones();
            // convert from current system time to sydney timezone
            var sydneyTime = TimeZoneInfo.ConvertTime(now, sydneyTimeZone);
            Console.WriteLine("Sydney Time: " + sydneyTime);

            // get utc time
            var sydneyUtc = TimeZoneInfo.ConvertTimeToUtc(sydneyTime);
            System.Console.WriteLine("---------------- Sydney UTC ---------------- ");
            System.Console.WriteLine("SYDNEY UTC: " + sydneyUtc);
        }

        static void DateAndTimeOffset()
        {
            // prefered over using DateTime. DateTimeOffset will provide date and time information as well as time zone information

            // doesn't include the timezone
            System.Console.WriteLine("Local System Time: " + DateTime.Now);

            // this also includes the time zone
            System.Console.WriteLine("Local System Time DateTimeOffset: " + DateTimeOffset.Now);

            System.Console.WriteLine("---------- Using Time offset ----------");

            // ToOffset lets us set an offset to UTC based on the TimeSpan given
            // this will be a -10 hour offset
            var time = DateTimeOffset.Now.ToOffset(TimeSpan.FromHours(-10));
            foreach (var timeZone in TimeZoneInfo.GetSystemTimeZones())
            {
                // find timezones with the same offset as this machines
                // this machine offset is defaulted above to -10 hours
                // finding all timezones that have a -10 hour difference to UTC
                if (timeZone.GetUtcOffset(time) == time.Offset)
                {
                    System.Console.WriteLine(timeZone);
                }
            }
            // in most cases we store everything in UTC time

            // Parsing date and time from string
            System.Console.WriteLine("------------- Parsing date time string to DateTime -------------");
            var date = "9/10/2019 10:00:00 PM";
            var parsedDate = DateTime.ParseExact(date, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
            System.Console.WriteLine("Parsed Data: " + parsedDate);

            // DateTime.Parse is leveraging the local systems timzone when parsing dates
            var date2 = "9/10/2019 10:00:00 PM +02:00";
            var parsedDate2 = DateTime.Parse(date2);
            System.Console.WriteLine("DateTime.Parse: " + parsedDate2);


        }
    }
}
