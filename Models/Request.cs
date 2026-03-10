using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    [Serializable]
    public class Request
    {
        public Operation Operation { get; set; }
        public object Data { get; set; }
    }
}
