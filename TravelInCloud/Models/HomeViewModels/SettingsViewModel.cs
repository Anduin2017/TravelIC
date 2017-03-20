using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TravelInCloud.Models.HomeViewModels
{
    public class SettingsViewModel
    {
        public virtual bool IsStore { get; set; }
        public virtual bool IsOurAccount { get; set; }
    }
}
