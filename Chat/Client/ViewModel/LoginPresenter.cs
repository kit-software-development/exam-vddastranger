using System.Security;
using System.Windows;
using System.Windows.Input;
using Client.View;
using Client.ViewModel.Others;

namespace Client.ViewModel
{
    public class LoginPresenter : ObservableObject
    {
        private string login;

        public SecureString SecurePassword { private get; set; }

        public string LoginTB
        {
            get { return login; }
            set
            {
                login = value;
                RaisePropertyChangedEvent(nameof(LoginTB));
            }
        }

        public ICommand LoginCommand => new DelegateCommand(() =>
        {
            if (!string.IsNullOrWhiteSpace(login) && !string.IsNullOrWhiteSpace(new System.Net.NetworkCredential(string.Empty, SecurePassword).Password))
            {
                //LoginValidator loginValidator = new LoginValidator();
                //PasswordValidator passwordValidator = new PasswordValidator();
                //loginValidator.validate(login);
                //passwordValidator.validate(SecurePassword);

                ClientLogin clientLogin = new ClientLogin();
                clientLogin.SendLoginAndEncryptPass(login, SecurePassword);
            }
            else
                MessageBox.Show("Login or password cant be null", "Login Information", MessageBoxButton.OK, MessageBoxImage.Error);
        });

        public ICommand RegistrationWindowCommand => new DelegateCommand(() =>
        {
            RegistrationWindow registration = new RegistrationWindow(this);
            registration.Show();
        });

        
    }
}
