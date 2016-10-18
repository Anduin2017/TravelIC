using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TravelInCloud.Models.HomeViewModels
{
    public class IndexViewModel
    {
        public virtual List<Product> Products { get; set; }
    }
}
