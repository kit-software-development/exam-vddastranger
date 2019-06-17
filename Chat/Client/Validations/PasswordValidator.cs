using System.Security;
using System.Text.RegularExpressions;

namespace Client.Validations
{
    public class PasswordValidator
    {
        const string PASSWORD_REGEX = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$";

        public bool validate(SecureString password)
        {
            return Regex.IsMatch(new System.Net.NetworkCredential(string.Empty, password).Password, PASSWORD_REGEX);
        }
    }
}
