using System.ComponentModel.DataAnnotations;

namespace Ecommerce529.CustomeValidations
{
    public class MinMaxLengthAttribute : ValidationAttribute
    {
        private readonly int minLength;
        private readonly int maxLength;
        public MinMaxLengthAttribute(int minLength , int  maxLength )
        {
            this.minLength = minLength; 
            this.maxLength = maxLength;
        }
        public override bool IsValid(object? value)
        {
            if(value is string result)
            {
                if (result.Length >= this.minLength && result.Length <= this.maxLength)
                    return true; 
            }
            return false;  
        }
        public override string FormatErrorMessage(string name)
        {
            return $"the {name} field must be >= {minLength} and <= {maxLength}"; 
        }
    }
}
