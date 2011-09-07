using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using RudeValidation.Web.Models;

namespace RudeValidation.Web.Validators
{
    /// <summary>
    /// Custom validators that apply to meetings.
    /// </summary>
    public static class MeetingValidators
    {
        /// <summary>
        /// Ensure that meetings aren't scheduled to start too early.
        /// </summary>
        /// <param name="meetingStartTime">
        /// The time the meeting it set to start.
        /// </param>
        /// <param name="validationContext">
        /// The context for the validation being performed.
        /// </param>
        /// <returns>
        /// A <see cref="ValidationResult"/> with an error or <see cref="ValidationResult.Success"/>.
        /// </returns>
        public static ValidationResult NoEarlyMeetings(DateTime meetingStartTime,
            ValidationContext validationContext)
        {
            if (meetingStartTime.TimeOfDay.Hours < 9)
            {
                return new ValidationResult(
                    "While you may be an early bird, it's not fair to schedule a meeting before 9:00 AM."
                    , new[] { validationContext.MemberName });
            }

            return ValidationResult.Success;
        }

        /// <summary>
        /// Validate that the end time for a meeting is not before the
        /// start time for the meeting.
        /// </summary>
        /// <param name="time">The start or end time being validated.</param>
        /// <param name="validationContext">
        /// The validation context, which includes the meeting instance.
        /// </param>
        /// <returns>
        /// A <see cref="ValidationResult"/> with an error or <see cref="ValidationResult.Success"/>.
        /// </returns>
        public static ValidationResult NoTimeTravel(DateTime time, ValidationContext validationContext)
        {
            Meeting meeting = (Meeting)validationContext.ObjectInstance;

            DateTime start = validationContext.MemberName == "Start" ? time : meeting.Start;
            DateTime end = validationContext.MemberName == "End" ? time : meeting.End;

            if (start > end)
            {
                return new ValidationResult(
                    "Meetings cannot result in time travel.",
                    new[] { "Start", "End" });
            }

            return ValidationResult.Success;
        }

        /// <summary>
        /// Ensure that long meetings don't include too many attendees.
        /// </summary>
        /// <param name="meeting">The meeting to validate.</param>
        /// <returns>
        /// A <see cref="ValidationResult"/> with an error or <see cref="ValidationResult.Success"/>.
        /// </returns>
        public static ValidationResult PreventExpensiveMeetings(Meeting meeting)
        {
            TimeSpan duration = meeting.End - meeting.Start;
            int attendees = (meeting.MaximumAttendees + meeting.MinimumAttendees) / 2;
            int cost = attendees * 50 * duration.Hours;

            if (cost > 10000)
            {
                return new ValidationResult("Meetings cannot cost the company more than $10,000.");
            }

            return ValidationResult.Success;
        }

        public static ValidationResult PreventDoubleBooking(Meeting meeting, ValidationContext validationContext)
        {
            if (validationContext.Items.ContainsKey("AllowOverBooking"))
            {
                bool allowOverBooking = (bool)validationContext.Items["AllowOverBooking"];

                if (!allowOverBooking)
                {
                    var meetingData = validationContext.GetService(typeof(IMeetingDataProvider))
                        as IMeetingDataProvider;

                    if (meetingData != null)
                    {
                        var conflicts = from other in meetingData.Meetings.Except(new[] { meeting })
                                        where other.Location == meeting.Location
                                        // Check for conflicts by seeing if the times overlap in any way
                                        && (
                                            (other.Start >= meeting.Start && other.Start <= meeting.End) ||
                                            (meeting.Start >= other.Start && meeting.Start <= other.End) ||
                                            (other.End >= meeting.Start && other.End <= meeting.End) ||
                                            (meeting.End >= other.Start && meeting.End <= other.End)
                                            )
                                        select other;

                        if (conflicts.Any())
                        {
                            return new ValidationResult(
                                "The location selected is already booked at this time.",
                                new[] { "Location", "Start", "End" });
                        }
                    }
                }
            }

            return ValidationResult.Success;
        }

        public static ValidationResult IsValidLocation(string location, ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(location))
            {
                var meetingData = validationContext.GetService(typeof(IMeetingDataProvider))
                    as IMeetingDataProvider;

                if (meetingData != null)
                {
                    if (!meetingData.Locations.Any(l => l.LocationName == location))
                    {
                        return new ValidationResult(
                            "That is not a valid location",
                            new[] { validationContext.MemberName });
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}