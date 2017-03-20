using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelInCloud.Models.HomeViewModels
{
    public class AllCommentsViewModel
    {
        public int ProductId { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
    }
}
