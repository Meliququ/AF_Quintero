using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace AF_Quintero.Functions.Entities
{
    public class AFEntity : TableEntity
    {
        //Field declaration of the  local table
        public string employedId { get; set; }
        public DateTime createdtime { get; set; }
        public byte Type { get; set; }
        public bool Isconsolidated { get; set; }
    }
}
