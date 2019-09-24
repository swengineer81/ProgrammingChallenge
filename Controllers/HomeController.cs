using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProgrammingChallange.Models;
using System.IO;
using ProgrammingChallange.DataFileHelper;
using Rotativa;


namespace ProgrammingChallange.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Default routing to main index page on HTTPGET
        /// </summary>
        /// <returns>Index View without model data</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// The Index Action Method handles entered form data of dataFile(vehicle data file) and entered tax rate. The file is read and returned as a model of type VehicleDataReport
        /// </summary>
        /// <param name="dataFile">Entered source file</param>
        /// <param name="taxRate">Entered tax rate</param>
        /// <returns>Model of type VehicleDataReport </returns>
        /// 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(HttpPostedFileBase dataFile, string taxRate)
        {
            if (dataFile.ContentLength > 0)
            {
                try
                {
                    var dataFileName = Path.GetFileName(dataFile.FileName);
                    var serverFilePath = SaveDataFile(dataFile);
                    var model = GetVehicleModel(dataFileName, taxRate);

                    if (model == null)
                    {
                        return RedirectToAction("Error", "HandleError", new { errorMsg = "Please check your vehicle data file. We were unable to read it" });
                    }

                    ViewData["vehicleDataFileName"] = dataFileName;
                    ViewData["taxRate"] = taxRate;
                    ViewData["createdTextReportFile"] = "no";
                    return View(model);
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "HandleError", new { errorMsg = ex.Message });
                }

            }
            return View();

        }

        /// <summary>
        /// Obtains the model of type VehicleSourceData and converts it to type VehicleDataReport
        /// </summary>
        /// <param name="sourceDataFileName">The filename of the chosen vehicle file</param>
        /// <param name="taxRate">The entered tax rate</param>
        /// <returns>Model of type VehicleDataReport</returns>
        private IEnumerable<VehicleDataReport> GetVehicleModel(string sourceDataFileName, string taxRate)
        {
            try
            {
                string directoryPath = ReturnServerPath();
                string sourceDataFilePath = Path.Combine(directoryPath, sourceDataFileName);
                CSVDataFileHelper csvHelper = new CSVDataFileHelper();
                var vehicleData = csvHelper.ConvertDataFile(sourceDataFilePath);

                var model = vehicleData.Select(x => new VehicleDataReport { listPrice = Convert.ToDouble(x.msrp) * Convert.ToDouble(taxRate), make = x.make, model = x.model, msrp = Convert.ToDouble(x.msrp), year = Convert.ToInt32(x.year) });
                return model;

            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Utilizies the installed Rotativa nuget package to create a pdf formatted view with data of type VehicleDataReport.  The user is able to download the pdf to their client
        /// </summary>
        /// <param name="vehicleDataFileName">Filename of the chosen file</param>
        /// <param name="taxRate">Entered tax rate</param>
        /// <returns>Model of type VehicleDataReport to the _VehicleReportPD view formatted as a PDF </returns>
        public ActionResult SavePDFReport(string vehicleDataFileName, string taxRate)
        {
            try
            {
                var model = GetVehicleModel(vehicleDataFileName, taxRate);
                return new ViewAsPdf("_VehicleReportPDF", model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "HandleError", new { errorMsg = ex.Message });
            }

        }

        /// <summary>
        /// Returns the VehicleDataReport model back to index if a text report version was successfully created 
        /// </summary>
        /// <param name="vehicleDataFileName">filename of chosen vehicle data file</param>
        /// <param name="taxRate">entered tax rate</param>
        /// <returns>Model of type VehicleDataReport to Index view</returns>
        public ActionResult SaveTextReport(string vehicleDataFileName, string taxRate)
        {
            try
            {
                var model = GetVehicleModel(vehicleDataFileName, taxRate);

                bool isTextReportCreated = CreateVehicleTextReport(model);

                if(isTextReportCreated)
                {
                    ViewData["vehicleDataFileName"] = vehicleDataFileName;
                    ViewData["taxRate"] = taxRate;
                    ViewData["createdTextReportFile"] = "yes";
                    return View("Index", model);
                }
                else
                {
                    return RedirectToAction("Error", "HandleError", new { errorMsg = "Error creating text vehicle report: ErrorCode1911" });
                }            
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "HandleError", new { errorMsg = ex.Message });
            }


        }

        /// <summary>
        ///  Creates, formats and saves the VehicleDataReport model as a text file.  The file is saved in the directory of ~/VehicleDataFiles/. 
        /// </summary>
        /// <param name="model">IEnumerable of Class type of VehicleDataReport: model of vehicle data</param>
        /// <returns>True if text report has been created and save otherwise false</returns>
        private bool CreateVehicleTextReport(IEnumerable<VehicleDataReport> model)
        {
            string formattedVehicleReportOutput = null;
            string tab = "\t";
            string newline = Environment.NewLine;
            string reportTitle = "----------Vehicle Report----------";
            string reportDate = "Date: " + DateTime.Now.ToString("MM/dd/yyyy");

            try
            {
                var groupedDatesDescModel = model.OrderByDescending(d => d.year).Select(x => x.year).Distinct();
                var grandMRSPTotal = model.Select(y => y.msrp).Sum();
                var totalListPrice = model.Select(z => z.listPrice).Sum();

                formattedVehicleReportOutput += reportTitle.PadRight(50) + tab + reportDate + newline;
                foreach (var rowYear in groupedDatesDescModel)
                {
                    formattedVehicleReportOutput += formattedVehicleReportOutput = rowYear.ToString() + newline + newline;
                    foreach (var vehicle in model.Where(x => x.year == rowYear).OrderBy(k => k.make))
                    {
                        formattedVehicleReportOutput += tab + vehicle.make.PadRight(25) + tab + vehicle.model.PadRight(30) + tab + "MSRP: " + string.Format("{0:C}", vehicle.msrp).PadRight(10) + tab + "ListPrice: " + string.Format("{0:C}", vehicle.listPrice) + newline;
                    }

                    formattedVehicleReportOutput += newline;
                }

                formattedVehicleReportOutput += "----------GrandTotal----------" + newline;
                formattedVehicleReportOutput += tab + "MSRP: " + string.Format("{0:C}", grandMRSPTotal) + newline;
                formattedVehicleReportOutput += tab + "ListPrice: " + string.Format("{0:C}", totalListPrice) + newline;
                var outputTextVehicleFilePath = Path.Combine(ReturnServerPath(), "vehicles" + DateTime.Now.ToString("MMddyy") + ".txt");
                System.IO.File.WriteAllText(outputTextVehicleFilePath, formattedVehicleReportOutput);
                return true;
            }
            catch (Exception)
            {
                
                return false;
            }


        }


        /// <summary>
        /// Tries to save the chosen vehicle data file to the server path of ~/VehicleDataFiles/
        /// </summary>
        /// <param name="dataFile">Chosen source vehicle file to upload</param>
        /// <returns>Server file path to uploaded source vehicle file</returns>
        private string SaveDataFile(HttpPostedFileBase dataFile)
        {
            string fileName = Path.GetFileName(dataFile.FileName);
            string directoryPath = ReturnServerPath();
            string fullDataFilePath = Path.Combine(directoryPath, fileName);
            try
            {
                dataFile.SaveAs(fullDataFilePath);
                return fullDataFilePath;
            }
            catch (Exception ex)
            {
                return ex.Message;

            }

        }

        /// <summary>
        /// Creates and returns the directory path that files will be stored in.  Will create the path if it does not already exist
        /// </summary>
        /// <returns>Server directory path to VehicleDataFiles as a string </returns>
        public string ReturnServerPath()
        {
            string directoryPath = null;

            directoryPath = Server.MapPath("~/VehicleDataFiles/");

            if (Directory.Exists(directoryPath) == false)
            {
                Directory.CreateDirectory(directoryPath);
            }

            return directoryPath;
        }

        /// <summary>
        /// Displays a message explaining the goal of this application
        /// </summary>
        /// <returns>About view with message</returns>
        public ActionResult About()
        {
            ViewBag.Message = "Programming Challenge to perform the reading and writing of csv data.";
            return View();
        }
    }
}