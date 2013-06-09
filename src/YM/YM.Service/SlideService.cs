using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YM.Service
{
    public class SlideResponse
    {
        public string Result { get; set; }
    }
    
    public class SlideService
    {
        public SlideResponse Get()
        {
            return new SlideResponse()
            {
                Result = "Hello"
            };
        }
    }
}
