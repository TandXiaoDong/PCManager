using System.Windows;
using System.Windows.Controls;

namespace ShikuIM
{

    /// <summary>
    /// The MoniterPassword attached property for a <see cref="PasswordBox"/>
    /// </summary>
    public class MoniterPasswordProperty : BaseAttachedProperty<MoniterPasswordProperty, bool>
    {

        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            //Get the caller
            var passwordBox = sender as PasswordBox;
            //Makesure it is a Password box
            if (passwordBox == null)
            {
                return;
            }
            //
            if ((bool)e.NewValue)
            {
                //Set default value
                HasTextProperty.SetValue(passwordBox, passwordBox.SecurePassword.Length > 0);

                passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {

            //set the default value
            HasTextProperty.SetValue(sender as PasswordBox);
        }
    }

    /// <summary>
    /// The Hastext attached property for a <see cref="PasswordBox"/>
    /// </summary>
    public class HasTextProperty : BaseAttachedProperty<HasTextProperty, bool>
    {

        public static void SetValue(DependencyObject sender)
        {
            SetValue(sender, ((PasswordBox)sender).SecurePassword.Length > 0);
        }
            
    }
}
