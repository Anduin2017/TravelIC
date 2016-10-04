using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TravelInCloud.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public virtual string openid { get; set; }
        public virtual string NickName { get; set; }
        public virtual string IconAddress { get; set; }
        /// <summary>
        /// 该用户下的所有订单
        /// </summary>
        [InverseProperty(nameof(Order.Owner))]
        public virtual List<Order> Orders { get; set; }
    }
}
