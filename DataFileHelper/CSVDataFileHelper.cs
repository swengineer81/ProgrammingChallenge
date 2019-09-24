using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CsvHelper;
using System.IO;
using ProgrammingChallange.Models;

namespace ProgrammingChallange.DataFileHelper
{
    class CSVDataFileHelper :IDataFile<VehicleSourceData>
    {
        public CSVDataFileHelper() { }
      /// <summary>
      /// Utilizes the CSVHelper nuget packacge to read and convert the uploaded vehicle data file to a list of type VehicleSourceData.
      /// </summary>
      /// <param name="dataFilePath">Full path to source vehicle data file on server</param>
      /// <returns>List of type VehicleSourceData</returns>
      public List<VehicleSourceData> ConvertDataFile(string dataFilePath)
        {
                       
            using (var reader = new StreamReader(dataFilePath))
            using (var csv = new CsvReader(reader))
            {
                try
                {
                    var vehicleData = csv.GetRecords<VehicleSourceData>();
                    var listiems = vehicleData.ToList();

                    return listiems;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
               

              

            }

            
        }


       
    }
}