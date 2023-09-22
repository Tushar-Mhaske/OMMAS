$.ajaxSetup({
    // Disable caching of AJAX responses
    cache: false
});
$(document).ready(function () {
    StateReport();
    
});

function StateReport() {
    $("#accordionStatePriority").load("/QMSSRSReports/QMSSRSReports/StateSchedulePriority/", function (Response) {
       // $("#accordionStatePriority").show();
       // $("#accordionDistrictPriority").hide();

    });
}


function DistrictReport(state) {
    
    $("#accordionDistrictPriority").load("/QMSSRSReports/QMSSRSReports/DistrictSchedulePriority/", { state: state }, function (Response) {
       // alert("Distrit Load Success");
        $("#accordionStatePriority").hide();
        $("#accordionDistrictPriority").show();
        $("#accordionRoadPriority").hide();
    });
    
}



function CloseDistrictPriority() {
    //StateReport();
    $("#accordionStatePriority").show();
    $("#accordionDistrictPriority").hide();
    $("#accordionRoadPriority").hide();

}

function RoadReport(DistrictCode) {

    var data = { DistrictCode: DistrictCode };
    $("#accordionRoadPriority").load("/QMSSRSReports/QMSSRSReports/RoadSchedulePriority/", data, function (Response) {
  //      alert("Road Loading Success");
        $("#accordionStatePriority").hide();
        $("#accordionDistrictPriority").hide();
        $("#accordionRoadPriority").show();
    });


}



function CloseRoadPriority() {
    $("#accordionStatePriority").hide();
    $("#accordionDistrictPriority").show();
    $("#accordionRoadPriority").hide();
}


function isInteger(n) {
    return $.isNumeric(n) && parseInt(n, 10) > 0;
}
