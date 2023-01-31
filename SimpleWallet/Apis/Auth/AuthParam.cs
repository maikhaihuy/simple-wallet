using System.ComponentModel.DataAnnotations;

namespace SimpleWallet.Apis.Auth
{
    public class RegisterParam
    {
        [Required]
        public string LoginName { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class LoginNameParam
    {
        [Required]
        public string LoginName { get; set; }
    }
}