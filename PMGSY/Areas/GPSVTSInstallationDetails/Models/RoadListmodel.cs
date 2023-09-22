using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.GPSVTSInstallationDetails.Models
{
    public class RoadListmodel
    {
        public string StateName { get; set; }
        public string DistrictName { get; set; }
        public string BlockName { get; set; }
        public string PackageId { get; set; }
        public string Year { get; set; }
        public string Batch { get; set; }
        public decimal Length { get; set; }
        public decimal SanctionedAmt { get; set; }

        public string GPSVTS_Established { get; set; }

        public int RoadCode { get; set; }
        public string RoadName { get; set; }

        public bool Is_VTSEntryLastDate_Exceed { get; set; }
        //
        public string isFinalized { get; set; }

        public string IsGPSInstalled { get; set; }
        public bool IsVTSWorkUnfreezed { get; set; }

        public string GPSVTS_Finalized { get; set; } 

        //
    }
    //Added By Tushar on 19 July 2023
    public class GPSVTSSavedDetailsModel
    {
        public string GPSInstalled { get; set; }
        public string DateOfInstallation { get; set; }
        public int? NumberOfVehicles { get; set; }
        public string VTSInstrumentGPSID { get; set; }
        public int? VehicleTypeID { get; set; }
        public string VehicleName { get; set; }
        public long? VTSGPSID { get; set; }
        public long? VehicleID { get; set; }
        public string VTSVehicleGPSID { get; set; }
        public string Edit { get; set; }

        public string Detete { get; set; }
        public string Finalize { get; set; }

        public string DateOfSubmission { get; set; }
    }

    //End By Tushar on 19 July 2023
}