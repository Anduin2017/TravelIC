using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TravelInCloud.Models.HomeViewModels
{
    public class PayViewModel
    {
        public string wxJsApiParam { get; set; }
        public string unifiedOrderResult { get; set; }
    }
}
