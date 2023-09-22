using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.GPSVTSInstallationDetails.Models
{
    public class GPSVTS_DetailsModel
    {
        public GPSVTS_DetailsModel() {
            VehicleList = new List<SelectListItem>();
        }
        public string Year { get; set; }
        public string Package { get; set; }
        public string Batch { get; set; }
        public int RoadCode { get; set; }

        [Display(Name = "Road Name")]
        public string RoadName { get; set; }

        [Display(Name ="GPS VTS Installed")]
        public string Is_GPSVTS_Installed { get; set; }
        [Required(ErrorMessage = "Vehicle is required.")]
        public int Vehicle { get; set; }
        public List<SelectListItem> VehicleList { get; set; }

        [Display(Name = "No. of Vehicles")]
        [Required(ErrorMessage = "No. of Vehicles is required.")]
        public int VehiclesCount { get; set; }

        [Display(Name = "GPS Instrument ID")]
        public string[] VehiclesID { get; set; }

        [Display(Name = "Total No. of Vehicles")]
        public int TotalNoVehicles { get; set; }

        [Display(Name = "Date of Installation")]
        [Required(ErrorMessage = "Date of Installation is required.")]
  
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Please Enter valid Date of Installation.")]
      
        public string VTS_InstallationDate { get; set; }
        public bool IsEditDetails { get; set; }
        public string Vehicle_Gps_Ids { get; set; }
        //
        public bool IsDetailsAlreadyPresent { get; set; }
        //
        //
        public string isFinalized { get; set; }
        //

    }
    public class GPSVTSDetailsDataModel
    {
        public List<GPSVTSDataModel> VTS_INSTRUMENT_GPS_Details { get; set; }
        public string RoadCodeValue { get; set; }
        public string Is_GPSVTS_Installed { get; set; }
        public string Vehicle_Gps_Ids { get; set; }
    }
    public class GPSVTSDataModel
    //{
    //    public string Vehicle_Id { get; set; }
    //    public string VehiclesCount { get; set; }
    //    public List<string> VTS_INSTRUMENT_GPS_ID { get; set; }
    //    public string VTS_InstallationDate { get; set; }
    //}
    {
        public string Vehicle { get; set; }
        public string VehiclesCount { get; set; }
        public List<string> VehiclesID { get; set; }
        public string VTS_InstallationDate { get; set; }
    }
    public class SavedVehicleDetails
    {
        public int IMS_PR_ROAD_CODE { get; set; }
        public string GPS_INSTALLED { get; set; }
        public int? NO_OF_VEHICLES { get; set; }
        public DateTime? DATE_OF_INSTALLATION { get; set; }
        // public List<string>  VTS_INSTRUMENT_GPS_ID { get; set; }
        //public List<string> VTS_INSTRUMENT_GPS_ID { get; set; }
        public string VTS_INSTRUMENT_GPS_ID { get; set; }

        public int? Vehicle { get; set; }

        public long? VEHICLE_ID { get; set; }
        public string VTS_VEHICLE_GPS_IDS { get; set; }
    }

    public class FileUploadViewModel
    {
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Invalid Pdf Description,Can only contains AlphaNumeric values")]
        public string PdfDescription { get; set; }
        public string ErrorMessage { get; set; }
        public int? NumberofPdfs { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public string url { get; set; }
        public string thumbnail_url { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int size { get; set; }
        public string delete_url { get; set; }
        public string delete_type { get; set; }

        public long FILE_ID { get; set; }
        public string isFinalized { get; set; }
    }

    }