using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TravelInCloud.Models.HomeViewModels
{
    public class IndexViewModel
    {
        public virtual IEnumerable<Product> Products { get; set; }
        public virtual IEnumerable<Location> Locations { get; set; }
        public Location CurrentLocation { get; set; }
    }
}
