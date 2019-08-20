using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using SessionBuilder.Core;

namespace SessionBuilder.Web.Pages
{
    public class DownloadController : Controller
    {
        private readonly ISpeakerRepository speakerRepository;

        public DownloadController(ISpeakerRepository speakerRepository)
        {
            this.speakerRepository = speakerRepository;
        }

        public FileContentResult Index(Guid id)
        {
            var speaker = speakerRepository.Get(id);

            var csv = "Title,Speaker,Length,ScheduledAt" + Environment.NewLine;

            // the offset is an example of a user passing in a date
            var offset = TimeSpan.FromHours(-11);

            DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            foreach (var session in speaker.Sessions)
            {
                // toString("o") converts date to ISO 8601 format
                // add .ToOffset to set the date and time to the given timezone offset
                csv += $"{session.Title},{speaker.Name},{session.Length},{session.ScheduledAt.ToOffset(offset).ToString("o")}{Environment.NewLine}";
            }
            // Create and return the CSV file
            return File(Encoding.UTF8.GetBytes(csv), "text/csv", $"sessions for {speaker.Name}.csv");
        }
    }
}