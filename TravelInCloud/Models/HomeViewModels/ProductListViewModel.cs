using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TravelInCloud.Models.HomeViewModels
{
    public class ProductListViewModel
    {
        public virtual string ProductName { get; set; }
        public virtual string QueryMethod { get; set; }
        public virtual int StoreType { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}
