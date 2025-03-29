using RentalSystem.Models;
using System.Text.RegularExpressions;

namespace RentalSystem.Utils
{
    public class Validation
    {
        public static bool IsUsernameUnique(string username, RentalSystemContext context)
        {
            return !context.Customers.Any(u => u.FullName == username);
        }

        public static bool IsUsernameAdmin(string username, RentalSystemContext context)
        {
            return context.Admins.Any(u => u.FullName == username);
        }

        public static bool IsEmailValid(string email)
        {
            string emailPattern = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";
            Regex regex = new Regex(emailPattern);
            return regex.IsMatch(email);
        }

        public static bool IsPasswordValid(string password)
        {
            return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$");
        }
    }
}
