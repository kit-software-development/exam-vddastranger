﻿using System.Text.RegularExpressions;

namespace Client.Validations
{
    public class LoginValidator
    {
        const string LOGIN_REGEX = @"^(?=[a-z])[-\w.]{0,23}([a-zA-Z\d])$";

        public bool validate(string login)
        {
            return Regex.IsMatch(login, LOGIN_REGEX);
        }

        /*
         * if(validate("email@em.pl") == false) 
         * {
         *      thorrow new ArgumentException("invalid email adress");
         * }
         */
    }
}
