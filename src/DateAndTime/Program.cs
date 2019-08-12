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
            // DateAndTimeOffset();
            // FormatDates();
            // DateTimeArithmetic();
            // WeekNumber();
            ExtendingDates();

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
            // always parse using UTC
            var parsedDate2 = DateTime.Parse(date2, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            System.Console.WriteLine("DateTime.Parse: " + parsedDate2 + " - Kind: " + parsedDate2.Kind);
        }

        static void FormatDates()
        {
            System.Console.WriteLine("------------- Formatting dates -------------");
            var date = "9/10/2019 10:00:00 PM";
            var parsedDate = DateTimeOffset.ParseExact(date, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
            parsedDate = parsedDate.ToOffset(TimeSpan.FromHours(10));
            // "s" string format represents ISO8601
            // var formattedDate = parsedDate.ToString("s");

            // "o" preserves time zone information with ISO 8601. Good for serializing
            // https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings#Roundtrip
            var formattedDate = parsedDate.ToString("o");
            System.Console.WriteLine(formattedDate);

            System.Console.WriteLine("-------------------- UTC to Local-----------------");
            // DateTimeOffset UtcNow is always used for the current time and serialized. Convert it to local time only on client side to avoid DateTime ambiguity
            // changing to DateTime.UtcNow will ignore the timezone which will cause problems
            var now = DateTimeOffset.UtcNow;
            System.Console.WriteLine(now.ToLocalTime());

            System.Console.WriteLine("-------------------- Local Time to UTC -----------------");
            var nowLocal = DateTimeOffset.Now;
            System.Console.WriteLine(nowLocal.ToLocalTime());

        }

        static void DateTimeArithmetic()
        {
            System.Console.WriteLine("---------------- Calculating Timespan Representations ----------------");

            // 60 hours, 100 minutes, 200 seconds = 2 days, 13 hours, 43 minutes, and 20 seconds
            var timeSpan = new TimeSpan(60, 100, 200);
            // timespan allows to convert the time given into a required representation of the time.
            System.Console.WriteLine("Timespan Hours: " + timeSpan.Hours);
            System.Console.WriteLine("Timespan Total Days: " + timeSpan.TotalDays);

            System.Console.WriteLine("---------------- Calculating Time Difference ----------------");
            var start = DateTimeOffset.UtcNow;
            var end = start.AddSeconds(50);

            TimeSpan difference = end - start;
            System.Console.WriteLine("Timespan Difference: " + difference);
            System.Console.WriteLine("Timespan Difference Seconds: " + difference.Seconds);
            System.Console.WriteLine("Timespan Difference Milliseconds: " + difference.Milliseconds);
            System.Console.WriteLine("Timespan Difference Total Seconds: " + difference.TotalSeconds);
            System.Console.WriteLine("Timespan Difference Total Minutes: " + difference.TotalMinutes);

            TimeSpan twiceTime = difference.Multiply(2);
            System.Console.WriteLine("Timespan Twice Difference: " + twiceTime);
            System.Console.WriteLine("Timespan Twice Difference Seconds: " + twiceTime.Seconds);
            System.Console.WriteLine("Timespan Twice Difference Milliseconds: " + twiceTime.Milliseconds);
            System.Console.WriteLine("Timespan Twice Difference Total Seconds: " + twiceTime.TotalSeconds);
            System.Console.WriteLine("Timespan Twice Difference Total Minutes: " + twiceTime.TotalMinutes);

        }
        static void WeekNumber()
        {
            Calendar calendar = CultureInfo.InvariantCulture.Calendar;
            var start = new DateTimeOffset(2010, 3, 2, 0, 0, 0, TimeSpan.Zero);
            var week = calendar.GetWeekOfYear(start.DateTime, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            System.Console.WriteLine(week);

            // var isoWeek = ISOWeek.GetWeekOfYear(start.DateTime)  // .net core 3
        }
        static void ExtendingDates()
        {
            var contractDate = new DateTimeOffset(2019, 7, 15, 0, 0, 0, TimeSpan.Zero);
            System.Console.WriteLine($"Contract Date: {contractDate}");

            // the AddTick will get contract to end the day before. ex.. Decemeber 31st 11:59pm rather than January 1
            var contractDate1 = contractDate.AddMonths(6).AddTicks(-1);
            System.Console.WriteLine($"Extended by 6 months: {contractDate1}");

            // use the new ExtendContract method to get contract to end exactly at the end of the month
            var contractDate2 = ExtendContract(contractDate, 6);
            System.Console.WriteLine($"Extending 6 months till end of month: {contractDate2}");
        }
        
        /// <summary>
        /// Extend a contract by the whole month and not by just adding a months time.
        /// ex. 02-29 to 03-28 should actually be 02-29 to 03-31
        /// </summary>
        /// <param name="current"></param>
        /// <param name="months"></param>
        /// <returns></returns>
        static DateTimeOffset ExtendContract(DateTimeOffset current, int months)
        {
            // add months to get the current month this contract should end
            var newContractDate = current.AddMonths(months).AddTicks(-1);
            return new DateTimeOffset(newContractDate.Year, newContractDate.Month, 
                DateTime.DaysInMonth(newContractDate.Year, newContractDate.Month), 23, 59, 59, current.Offset);
        }
    }
}
