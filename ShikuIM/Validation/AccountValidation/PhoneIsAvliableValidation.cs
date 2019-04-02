using Newtonsoft.Json;
using ShikuIM.Model;
using System.Globalization;
using System.Windows.Controls;

namespace ShikuIM.Validation
{

    /// <summary>
    /// 标识手机号是否可用
    /// </summary>
    public class PhoneIsAvliableValidation : ValidationRule
    {

        /// <summary>
        /// 验证手机号是否可用   
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string phoneNumber = value as string;//Get phone number
            if (phoneNumber.Length > 0)
            {
                string res = APIHelper.TelephoneVerify(phoneNumber);//Use Http to verify phonenumber
                var result = JsonConvert.DeserializeObject<JsonBase>(res);
                if (result.resultCode != 1)
                {
                    return new ValidationResult(false, "手机号已注册");
                }
                else
                {
                    return ValidationResult.ValidResult;
                }
            }
            return ValidationResult.ValidResult;
        }
    }
}
