using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SessionBuilder.Core;

namespace SessionBuilder.Web.Pages
{
    public enum DateComparison
    {
        Earlier = -1,
        Later = 1,
        TheSame = 0
    };

    public class IndexModel : PageModel
    {
        private readonly ISpeakerRepository repository;

        public Speaker Speaker { get; set; }

        public IndexModel(ISpeakerRepository repository)
        {
            this.repository = repository;
        }

        public void OnGet()
        {
            Speaker = repository.Get("Filip Ekberg");
        }

        public bool IsSessionOverlapping(Session currentSession)
        {
            foreach (var session in Speaker.Sessions)
            {
                if (session.Id == currentSession.Id) continue;

                // check if sessions start time is between current sessions start and end time
                // does session x start between session Ys start & end?
                if (session.ScheduledAt.IsBetween(currentSession.ScheduledAt,
                    currentSession.ScheduledAt.Add(currentSession.Length)))
                {
                    return true;
                }
                // does session Y start between session Xs start & end?
                else if (currentSession.ScheduledAt.IsBetween(session.ScheduledAt,
                    session.ScheduledAt.Add(session.Length)))
                {
                    return true;
                }
            }

            return false;
        }

        public bool DoesSpeakerHaveEarlierSessions(Session currentSession)
        {
            return Speaker.Sessions.Any(
                session => session.Id != currentSession.Id &&
                // compares each session with current session using CompareTo 
                // method from DateTimeOffset which returns an integer indicating if a date was earlier(-1), later(1), or the same (0)
                // Created a DateComparison enum to represent these CompareTo integers
                session.ScheduledAt.CompareTo(currentSession.ScheduledAt) == (int)DateComparison.Earlier
            );
        }

        public bool DoesSpeakerHaveLaterSessions(Session currentSession)
        {
            // same logic as DoesSpeakerHaveEarlierSessions
            return Speaker.Sessions.Any(
                session => session.Id != currentSession.Id &&
                session.ScheduledAt.CompareTo(currentSession.ScheduledAt) == (int)DateComparison.Later
            );
        }

        public TimeSpan GetTimeUntilNextSession(Session currentSession)
        {
            // order the sessions based on ScheduledAt time
            var nextSession = Speaker.Sessions.OrderBy(session => session.ScheduledAt)
            // select first session that comes after the current session
                .FirstOrDefault(session => session.Id != currentSession.Id &&
                session.ScheduledAt >= currentSession.ScheduledAt);

            // return minimum timespan to indicate that a session could not be found
            if (nextSession == null) return TimeSpan.MinValue;

            // subtract the next session schedule time with current session end date by adding the length to the current sessions start time
            return nextSession.ScheduledAt - currentSession.ScheduledAt.Add(currentSession.Length);
        }

        public TimeSpan TimeSinceSubmission(Session session)
        {
            var timeSinceSubmission =
            // using ToOffset(session.SubmittedAt.Offset) to take the current Utc time and
            // offsetting it to match the session offset so that their timezones align to the same zone. 
            // This will give a current time difference from the session SubmittedAt to now without worry about which timezone the time was
            // submitted in.
                session.SubmittedAt - DateTimeOffset.UtcNow.ToOffset(session.SubmittedAt.Offset);

            return timeSinceSubmission;
        }

        public int GetSpeakerAge()
        {
            var today = DateTime.UtcNow.Date;
            // this will tell the age as soon as January 1st is hit for a new year. If the birthday is on june 21 then it will be displayed early
            var age = today.Year - Speaker.Birthday.Year;

            // check if the current date as a whole including months and days is greater than todays date minus the age in years
            if (Speaker.Birthday.Date > today.Date.AddYears(-age))
            {
                // if it is greater than it means the birthday is in a later month within this year so we subract 1 year
                age -= 1;
            }

            return age;
        }

        public int GetDaysUntilNextBirthday()
        {
            var today = DateTime.UtcNow.Date;
            // date of birthday in the current year.
            // Defaulting day to 1 here to avoid issues like a leap year where adding the wrong number 
            // of days can cause an ArgumentOutOfRangeException but will work fine in other years.
            var birthday = new DateTime(today.Year, Speaker.Birthday.Month, 1);
            // add the days here to avoid leap year issues. Subract "1" so the birthday is not shown 
            // as the day after since the default was on the first day and now we're adding the total 
            // number of days with that additinal one day.
            birthday = birthday.AddDays(Speaker.Birthday.Day - 1);

            // birthday has already passed
            if (birthday < today)
            {
                // since birthday has passed, the year is incremented by 1 to adjust
                birthday = new DateTime(today.Year + 1, Speaker.Birthday.Month, 1);
                birthday = birthday.AddDays(Speaker.Birthday.Day - 1);
            }

            return (int)(birthday - today).TotalDays;
        }
    }

    public static class DateTimeOffsetExtensions
    {
        public static bool IsBetween(this DateTimeOffset source, DateTimeOffset start, DateTimeOffset end)
        {
            return source > start && source < end;
        }
    }
}
