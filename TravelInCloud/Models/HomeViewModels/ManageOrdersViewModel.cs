using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelInCloud.Models.HomeViewModels
{
    public class ManageOrdersViewModel
    {
        public IEnumerable<Order> Orders { get; set; }
    }
}
