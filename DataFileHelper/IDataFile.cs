using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ProgrammingChallange.Models;

namespace ProgrammingChallange.DataFileHelper
{
    /// <summary>
    /// Method to utilize the csvhelper solution and return a list of some type
    /// </summary>
    /// <typeparam name="T"> Generic Type</typeparam>
    interface IDataFile<T>
    {
       List<T> ConvertDataFile(string dataFilePath);




    }
}
