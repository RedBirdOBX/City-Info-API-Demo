using System;
using System.ComponentModel.DataAnnotations;

namespace CityInfoAPI.Dtos.CustomValidations
{

#pragma warning disable CS1591

    // https://andrewlock.net/creating-an-empty-guid-validation-attribute/

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class NotEmptyGuidAttribute : ValidationAttribute
    {
        public const string _defaultErrorMessage = "The {0} identifier cannot be empty.";
        public NotEmptyGuidAttribute() : base(_defaultErrorMessage)
        { }

        public override bool IsValid(object value)
        {

            switch (value)
            {
                case Guid guid:
                    return guid != Guid.Empty;
                default:
                    return true;
            }
        }
    }

    #pragma warning restore CS1591
}
