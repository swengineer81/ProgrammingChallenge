using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProgrammingChallange.Models
{
    /// <summary>
    /// Provides the model type for the views that will display the formatted report data
    /// </summary>
    public class VehicleDataReport
    {
        public int year { get; set; }
        public string make { get; set; }
        public string model { get; set; }
        public double msrp { get; set; }
        public double listPrice { get; set; }
    }
}