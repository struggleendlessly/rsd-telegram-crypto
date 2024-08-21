using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class errorResponse
    {
        public Error[] error { get; set; } = [];
        public class Error
        {
            public string Code { get; set; } = "";
        }
    }
}
