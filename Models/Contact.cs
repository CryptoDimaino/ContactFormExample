using System.ComponentModel.DataAnnotations;

namespace ContactFormExample.Models
{
    public class Contact
    {
        [Required]
        [Display(Name = "First Name: ")]
        public string FirstName {get;set;}
        [Required]
        [Display(Name = "Last Name: ")]
        public string LastName {get;set;}
        [Required]
        [Display(Name = "Email: ")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email {get;set;}
        [Required]
        [Display(Name = "Message: ")]
        public string Message {get;set;}

        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }
    }
}