using System;
using System.ComponentModel.DataAnnotations;

namespace RudeValidation.Web.Validators
{
    /// <summary>
    /// Support two types of date validation:
    /// 1) Ensure dates are in the past
    /// 2) Ensure dates are in the future
    /// </summary>
    /// <remarks>
    /// No date can ever be the present for more
    /// than an instant.
    /// </remarks>
    public enum DateValidatorType
    {
        Past,
        Future
    }

    /// <summary>
    /// Validate that a date value is either a Past or Future date
    /// as appropriate.
    /// </summary>
    public class DateValidatorAttribute : ValidationAttribute
    {
        /// <summary>
        /// The type of date expected.
        /// </summary>
        public DateValidatorType ValidatorType { get; private set; }

        /// <summary>
        /// Validate that a date value is either a Past or Future date
        /// as appropriate.
        /// </summary>
        /// <param name="validatorType"></param>
        public DateValidatorAttribute(DateValidatorType validatorType)
        {
            this.ValidatorType = validatorType;
        }

        /// <summary>
        /// Conditionally validate that the date is either in the past
        /// or in the future.
        /// </summary>
        /// <param name="value">The date to validate.</param>
        /// <param name="validationContext">The validation context.</param>
        /// <returns>
        /// <see cref="ValidationResult.Success"/> when the date matches the
        /// expected date type, otherwise a <see cref="ValidationResult"/>.
        /// </returns>
        protected override ValidationResult IsValid(object value,
            ValidationContext validationContext)
        {
            DateTime date = (DateTime)value;
            int comparison = date.CompareTo(DateTime.Now);

            if (comparison < 0)
            {
                if (this.ValidatorType != Validators.DateValidatorType.Past)
                {
                    return new ValidationResult(
                        string.Format("{0} cannot be in the past", validationContext.DisplayName),
                        new[] { validationContext.MemberName });
                }
            }
            else if (comparison > 0)
            {
                if (this.ValidatorType != Validators.DateValidatorType.Future)
                {
                    return new ValidationResult(
                        string.Format("{0} cannot be in the future", validationContext.DisplayName),
                        new[] { validationContext.MemberName });
                }
            }

            return ValidationResult.Success;
        }
    }
}