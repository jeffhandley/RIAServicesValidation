using System;
using System.ComponentModel.DataAnnotations;
using RudeValidation.Web.Resources;
using RudeValidation.Web.Validators;

namespace RudeValidation.Web.Models
{
    [CustomValidation(typeof(MeetingValidators), "PreventExpensiveMeetings")]
    [CustomValidation(typeof(MeetingValidators), "PreventDoubleBooking")]
    [MetadataType(typeof(Meeting.MeetingMetadata))]
    public partial class Meeting
    {
        public class MeetingMetadata
        {
            [Key]
            [Display(AutoGenerateField = false)]
            public int MeetingId { get; set; }

            [Required]
            [CustomValidation(typeof(MeetingValidators), "NoEarlyMeetings")]
            [CompareValidator(CompareOperator.LessThan, "End",
                ErrorMessage = "Meetings cannot result in time travel.")]
            [DateValidator(DateValidatorType.Future)]
            [Display(Order = 1)]
            public DateTime Start { get; set; }

            [Required]
            [CompareValidator(CompareOperator.GreaterThan, "Start",
                ErrorMessage = "Meetings cannot result in time travel.")]
            [Display(Order = 2)]
            public DateTime End { get; set; }

            [Required]
            [StringLength(80, MinimumLength = 5,
                ErrorMessageResourceType = typeof(ValidationErrorResources),
                ErrorMessageResourceName = "TitleStringLengthErrorMessage")]
            // {0} must be at least {2} characters and no more than {1}.
            [Display(Order = 0)]
            public string Title { get; set; }

            [ConditionallyRequired("IsLongMeeting",
                ErrorMembers = "Start, End",
                ErrorMessage = "If you're asking for more than an hour of time, provide an agenda.")]
            [ConditionallyRequired("Location", "18/3367",
                ErrorMessage = "No one can ever find this room; please be sure to include directions.")]
            public string Details { get; set; }

            [Required]
            [RegularExpression(@"\d{1,3}/\d{4}",
                ErrorMessage = "{0} must be in the format of 'Building/Room'")]
            [CustomValidation(typeof(MeetingValidators), "IsValidLocation")]
            [Display(Order = 3)]
            public string Location { get; set; }

            [Range(2, 100)]
            [CompareValidator(CompareOperator.LessThanEqual, "MaximumAttendees")]
            [Display(Name = "Minimum Attendees", Order = 4)]
            public int MinimumAttendees { get; set; }

            [Range(2, 100)]
            [CompareValidator(CompareOperator.GreaterThanEqual, "MinimumAttendees")]
            [Display(Name = "Maximum Attendees", Order = 5)]
            public int MaximumAttendees { get; set; }
        }
    }
}