using System.ComponentModel.DataAnnotations;

namespace Moldovan_Raul_Lab2.Models
{
    public class Author
    {
        public int ID { get; set; }

        [Display(Name = "FirstName")]
        public string? FirstName { get; set; }

        [Display(Name = "LastName")]
        public string? LastName { get; set; }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        public ICollection<Book>? Books { get; set; }
    }
}
