using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmKurdu.Models
{
    [Table("users")]
    public class Users
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        
        public bool IsAdmin { get; set; }

        [Required(ErrorMessage = "Please enter a username.")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9]{2,16}$", ErrorMessage = "Username must be between 3 and 16 characters.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter an email.")]
        [EmailAddress(ErrorMessage = "Invalid email format. Please use a valid email address.")]
        public string Mail { get; set; }

        public string Image { get; set; }

        [Required(ErrorMessage = "Please enter a password.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+}{"":;'?/>.<,])[A-Za-z\d@$!%*?&#]{7,32}$", ErrorMessage = "Password must be between 8 and 32 characters and contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

    }
}