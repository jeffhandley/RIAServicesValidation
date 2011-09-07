using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace RudeValidation.Web.Validators
{
    /// <summary>
    /// Define the comparison operators for
    /// the <see cref="CompareValidatorAttribute"/>.
    /// </summary>
    public enum CompareOperator
    {
        [Display(Name = "must be less than")]
        LessThan,

        [Display(Name = "cannot be more than")]
        LessThanEqual,

        [Display(Name = "must be the same as")]
        Equal,

        [Display(Name = "must be different from")]
        NotEqual,

        [Display(Name = "cannot be less than")]
        GreaterThanEqual,

        [Display(Name = "must be more than")]
        GreaterThan
    }

    /// <summary>
    /// A comparison validator that will compare a value to another property
    /// and validate that the comparison is valid.
    /// </summary>
    public class CompareValidatorAttribute : ValidationAttribute
    {
        public CompareValidatorAttribute(CompareOperator compareOperator, string compareToProperty)
            : base("{0} {1} {2}")
        {
            this.CompareOperator = compareOperator;
            this.CompareToProperty = compareToProperty;
        }

        public CompareOperator CompareOperator { get; private set; }
        public string CompareToProperty { get; private set; }

        /// <summary>
        /// Cache the property info for the compare to property.
        /// </summary>
        private PropertyInfo _compareToPropertyInfo;

        /// <summary>
        /// Validate the value against the <see cref="CompareProperty"/>
        /// using the <see cref="CompareOperator"/>.
        /// </summary>
        /// <param name="value">The value being validated.</param>
        /// <param name="validationContext">The validation context.</param>
        /// <returns>
        /// A <see cref="ValidationResult"/> if invalid or <see cref="ValidationResult.Success"/>.
        /// </returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Get the property that we need to compare to.
            this._compareToPropertyInfo = validationContext.ObjectType
                .GetProperty(this.CompareToProperty);

            object compareToValue = this._compareToPropertyInfo
                .GetValue(validationContext.ObjectInstance, null);

            int comparison = ((IComparable)value).CompareTo(compareToValue);

            bool isValid;
            if (comparison < 0)
            {
                isValid = this.CompareOperator == CompareOperator.LessThan
                       || this.CompareOperator == CompareOperator.LessThanEqual
                       || this.CompareOperator == CompareOperator.NotEqual;
            }
            else if (comparison > 0)
            {
                isValid = this.CompareOperator == CompareOperator.GreaterThan
                       || this.CompareOperator == CompareOperator.GreaterThanEqual
                       || this.CompareOperator == CompareOperator.NotEqual;
            }
            else
            {
                isValid = this.CompareOperator == CompareOperator.LessThanEqual
                       || this.CompareOperator == CompareOperator.Equal
                       || this.CompareOperator == CompareOperator.GreaterThanEqual;
            }

            if (!isValid)
            {
                return new ValidationResult(
                    this.FormatErrorMessage(validationContext.DisplayName),
                    new[] { validationContext.MemberName, this.CompareToProperty });
            }

            return ValidationResult.Success;
        }
        
        /// <summary>
        /// Format the error message string using the property's
        /// name, the compare operator, and the comparison property's
        /// display name.
        /// </summary>
        /// <param name="name">The display name of the property validated.</param>
        /// <returns>The formatted error message.</returns>
        public override string FormatErrorMessage(string name)
        {
            return string.Format(this.ErrorMessageString,
                                    name,
                                    GetOperatorDisplay(this.CompareOperator),
                                    GetPropertyDisplay(this._compareToPropertyInfo));
        }
        
        /// <summary>
        /// Get the display name for the specified compare operator.
        /// </summary>
        /// <param name="compareOperator">The operator.</param>
        /// <returns>The display name for the operator.</returns>
        private static string GetOperatorDisplay(CompareOperator compareOperator)
        {
            return typeof(CompareOperator)
                .GetField(compareOperator.ToString())
                .GetCustomAttributes(typeof(DisplayAttribute), false)
                .Cast<DisplayAttribute>()
                .Single()
                .GetName();
        }
        
        /// <summary>
        /// Get the display name for the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The display name of the property.</returns>
        private static string GetPropertyDisplay(PropertyInfo property)
        {
            DisplayAttribute attribute = property
                .GetCustomAttributes(typeof(DisplayAttribute), false)
                .Cast<DisplayAttribute>()
                .SingleOrDefault();

            return attribute != null ? attribute.GetName() : property.Name;
        }
    }
}