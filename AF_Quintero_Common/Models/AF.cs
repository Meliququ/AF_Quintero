using System;

namespace AF_Quintero_Common.Models
{
    public class AF
    {
        //Field declaration of the table
        public string employedId { get; set; }
        public DateTime createdtime { get; set; }
        public byte Type { get; set; }
        public bool Isconsolidated { get; set; }
    }
}
