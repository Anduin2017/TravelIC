using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TravelInCloud.Models.HomeViewModels
{
    public class PreOrderViewModel
    {
        public virtual int TargetProductId { get; set; }
        public virtual ProductType TargetProduct { get; set; }
        [Required]
        public virtual DateTime UseDate { get; set; }
    }
}
