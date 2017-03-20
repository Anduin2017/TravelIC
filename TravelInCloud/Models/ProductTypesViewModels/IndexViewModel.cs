using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelInCloud.Models.ProductTypesViewModels
{
    public class IndexViewModel
    {
        public virtual int ProductId { get; set; }
        public virtual List<ProductType> ProductTypes { get; set; }
    }
}
