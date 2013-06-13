using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YM.Service
{
    
    [Route("/slide", Summary="Get the slide name")]
    public class SlideRequest
    {
        [ApiMember(Name="Name", IsRequired=true)]
        public string Name { get; set; }
    }
    
    public class SlideResponse
    {
        public string Result { get; set; }
    }
    
    public class SlideService : ServiceStack.ServiceInterface.Service
    {
        public SlideResponse Get(SlideRequest req)
        {
            return new SlideResponse()
            {
                Result = "Hello"
            };
        }
    }
}
