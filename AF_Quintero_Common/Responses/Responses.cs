using System;
using System.Collections.Generic;
using System.Text;

namespace AF_Quintero_Common.Responses
{
   public  class Responses
    {
        //statement of query result fields
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public object Result { get; set; }
    }
}
