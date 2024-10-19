using System.ComponentModel.DataAnnotations;

namespace KoiCareSys.Data.DTO
{
    public class LoginDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        //[MinLength(6)]
        public string Password { get; set; }
    }
}
