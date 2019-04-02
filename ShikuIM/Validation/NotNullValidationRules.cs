using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace ShikuIM.Validation
{

    /// <summary>
    /// 用于创建群的字段验证
    /// </summary>
    public class NotNullValidationRules : ValidationRule
    {
        /// <summary>
        /// 对应字段的名称
        /// </summary>
        public string FieldName { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string)
            {
                var tmpTxt = (string)value;
                if (string.IsNullOrEmpty(tmpTxt))
                {
                    return new ValidationResult(false, FieldName + "不能为空");
                }
                else
                {
                    return ValidationResult.ValidResult;
                }
            }
            else
            {
                return new ValidationResult(false, "请输入正确的数据");
            }
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo, BindingGroup owner)
        {
            return base.Validate(value, cultureInfo, owner);
        }
    }
}
