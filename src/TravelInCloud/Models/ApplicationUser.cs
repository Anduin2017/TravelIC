using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelInCloud.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public virtual string openid { get; set; }
        public virtual string NickName { get; set; }
        public virtual string IconAddress { get; set; }
        public virtual string Discriminator { get; set; }

        /// <summary>
        /// 该用户下的所有订单
        /// </summary>
        [InverseProperty(nameof(Order.Owner))]
        public virtual List<Order> Orders { get; set; }

        [InverseProperty(nameof(Comment.User))]
        public virtual List<Comment> Comments { get; set; }

        public virtual DateTime RegisterTime { get; set; } = DateTime.Now;

        public virtual string Name { get; set; }
        public virtual string IDCode { get; set; }

        public bool FullInfo()
        {
            var HasName = !string.IsNullOrEmpty(Name);
            var HasCode = !string.IsNullOrEmpty(IDCode);
            var HasPhone = !string.IsNullOrEmpty(PhoneNumber);
            return HasName && HasCode && HasPhone;
        }
    }
}
