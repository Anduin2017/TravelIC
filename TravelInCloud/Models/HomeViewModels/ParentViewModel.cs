using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TravelInCloud.Models.HomeViewModels
{
    public class ParentViewModel
    {
        public virtual string NickName { get; set; }
        public virtual string IconAddress { get; set; }
        public virtual bool OurAccount { get; set; }
    }
}
