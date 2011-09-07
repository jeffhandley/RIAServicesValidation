using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.ServiceModel.DomainServices.Client;

namespace RudeValidation.Helpers
{
    public class ModelPropertyValidatorAttribute : ModelValidatorAttribute
    {
        public ModelPropertyValidatorAttribute(Type modelType)
            : base(modelType, ModelValidationMode.InferProperty) { }

        public ModelPropertyValidatorAttribute(Type modelType, string propertyName)
            : base(modelType, propertyName) { }
    }

    public class ModelObjectValidatorAttribute : ModelValidatorAttribute
    {
        public ModelObjectValidatorAttribute(Type modelType)
            : base(modelType, ModelValidationMode.Object) { }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Parameter, AllowMultiple = true)]
    public abstract class ModelValidatorAttribute : ValidationAttribute
    {
        public Type ModelType { get; private set; }
        public ModelValidationMode ValidationMode { get; private set; }
        public string ModelProperty { get; private set; }

        private object model;

        public ModelValidatorAttribute(Type modelType, ModelValidationMode validationMode)
        {
            this.ModelType = modelType;
            this.ValidationMode = validationMode;
        }

        public ModelValidatorAttribute(Type modelType, string modelPropertyName)
        {
            this.ModelType = modelType;
            this.ValidationMode = Helpers.ModelValidationMode.SpecifiedProperty;
            this.ModelProperty = modelPropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (model == null)
            {
                model = Activator.CreateInstance(this.ModelType);
            }

            ValidationContext redirectedContext = new ValidationContext(model, validationContext, validationContext.Items);

            switch (this.ValidationMode)
            {
                case ModelValidationMode.InferProperty:
                    redirectedContext.MemberName = validationContext.MemberName;
                    break;
                case ModelValidationMode.SpecifiedProperty:
                    redirectedContext.MemberName = this.ModelProperty;
                    break;
                case ModelValidationMode.Object:
                    redirectedContext.MemberName = null;
                    break;
            }

            ComplexObject targetEntity = validationContext.ObjectInstance as ComplexObject;
            var breakOnFirstError = (targetEntity == null);
            IEnumerable<ValidationResult> validationResults = TryValidateProperty(value, validationContext, redirectedContext, breakOnFirstError);

            if (validationResults.Any())
            {
                if (validationResults.Count() == 1)
                {
                    return validationResults.Single();
                }

                if (targetEntity != null)
                {
                    foreach (ValidationResult result in validationResults.Skip(1))
                    {
                        targetEntity.ValidationErrors.Add(result);
                    }
                }

                return validationResults.First();
            }

            return ValidationResult.Success;
        }

        private static IEnumerable<ValidationResult> TryValidateProperty(object value, ValidationContext validationContext, ValidationContext modelValidationContext, bool breakOnFirstError)
        {
            ICustomAttributeProvider validatorProvider;

            if (!string.IsNullOrEmpty(modelValidationContext.MemberName))
            {
                validatorProvider = modelValidationContext.ObjectType.GetProperty(modelValidationContext.MemberName);
            }
            else
            {
                validatorProvider = modelValidationContext.ObjectType;
            }

            IEnumerable<ValidationAttribute> validators = validatorProvider
                .GetCustomAttributes(typeof(ValidationAttribute), true)
                .Cast<ValidationAttribute>();

            IEnumerable<ValidationResult> results = GetValidationErrors(value, validationContext, validators, breakOnFirstError);
            return results;
        }

        private static IEnumerable<ValidationResult> GetValidationErrors(object value, ValidationContext validationContext, IEnumerable<ValidationAttribute> attributes, bool breakOnFirstError)
        {
            List<ValidationResult> errors = new List<ValidationResult>();
            bool hasErrors = false;
            ValidationResult result;

            foreach (RequiredAttribute required in attributes.OfType<RequiredAttribute>())
            {
                result = required.GetValidationResult(value, validationContext);

                if (result != ValidationResult.Success)
                {
                    errors.Add(result);
                    hasErrors = true;

                    if (breakOnFirstError)
                    {
                        return errors;
                    }
                }
            }

            if (hasErrors)
            {
                return errors;
            }

            foreach (ValidationAttribute attribute in attributes)
            {
                if (attribute is RequiredAttribute)
                {
                    continue;
                }

                result = attribute.GetValidationResult(value, validationContext);

                if (result != ValidationResult.Success)
                {
                    errors.Add(result);
                    hasErrors = true;

                    if (breakOnFirstError)
                    {
                        return errors;
                    }
                }
            }

            return errors;
        }
    }

    public enum ModelValidationMode
    {
        InferProperty,
        SpecifiedProperty,
        Object
    }
}
