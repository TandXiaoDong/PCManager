using System.Globalization;
using System.Windows.Controls;

namespace ShikuIM.Validation
{


    public class LoginValidation : ValidationRule
    {

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return ValidationResult.ValidResult;
        }
    }
}
