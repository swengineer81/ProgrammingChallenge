using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProgrammingChallange.Models
{
    /// <summary>
    /// Provides mapping for the chosen source vehicle csv or text file. The properties are the acceptable headers which the file must have
    /// </summary>
    class VehicleSourceData
    {
        public string year { get; set; }
        public string make { get; set; }
        public string model { get; set; }
        public string msrp { get; set; }
     
    }
}