using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TravelInCloud.Models.HomeViewModels
{
    public class OrderViewModel
    {
        public virtual string NickName { get; set; }
        public virtual string IconAddress { get; set; }
        public virtual bool OurAccount { get; set; }
        public virtual List<Order> Orders { get; set; }
    }
}
