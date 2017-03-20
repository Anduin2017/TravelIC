using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TravelInCloud.Models.AccountViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage ="必须输入邮箱账号")]
        [EmailAddress(ErrorMessage ="输入的邮箱账号不合法")]
        public string Email { get; set; }

        [Required(ErrorMessage ="必须输入密码")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
