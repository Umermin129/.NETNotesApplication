using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class Response
    {
       public int StatusCode { get; set; }
        public string Message { get; set; }
        public object ModelObject { get; set; }
    }
}
