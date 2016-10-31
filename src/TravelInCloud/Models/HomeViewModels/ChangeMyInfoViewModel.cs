using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TravelInCloud.Models.HomeViewModels
{
    public class ChangeMyInfoViewModel
    {
        public virtual string Name { get; set; }
        public virtual string Phone { get; set; }
        public virtual string IDCode { get; set; }
        public virtual string ReturnUrl { get; set; }
    }
}
