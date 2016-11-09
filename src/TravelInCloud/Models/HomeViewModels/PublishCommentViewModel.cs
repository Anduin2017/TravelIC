using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelInCloud.Models.HomeViewModels
{
    public class PublishCommentViewModel
    {
        public virtual int ProductId { get; set; }
        public virtual string Content { get; set; }
    }
}
