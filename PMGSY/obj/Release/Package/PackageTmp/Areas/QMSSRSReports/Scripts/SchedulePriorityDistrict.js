$.ajaxSetup({
    // Disable caching of AJAX responses
    cache: false
});
$(document).ready(function () {
    DistrictPriorityInitialization();

});


/* District Wise Priority Start Here*/
function DistrictPriorityInitialization() {
    $("#btnDistrictPriority1Submit").click(function () {
        var progMin = $("#txtDistrictPriority1Prog").val();
        var CompMin = $("#txtDistrictPriority1Comp").val();
        var FromMonth = $("#txtDistrictPriority1InspMonthFrom").val();
        
        var StateCode = $("#hdStateCode").val();

        if (!isInteger(progMin)) {
            alert("Enter Valid Progress Value.");
        }
        else if (!isInteger(CompMin)) {
            alert("Enter Valid Completed Value.");
        }
        else if (!isInteger(FromMonth)) {
            alert("Enter Valid Inspection Month.");
        }
        else {
            var data = { priority: 1, ProgressMin: progMin, ProgressMax: 0, CompletedMin: CompMin, CompletedMax: 0, StateCode: StateCode, FromMonth: FromMonth, ToMonth: 0 };
            LoadDistrictReport($('#div1DistrictPriorityList'), data);
            
        }
    });

    $("#btnDistrictPriority2Submit").click(function () {

        var progMin = $("#txtDistrictPriority2ProgMin").val();
        var CompMin = $("#txtDistrictPriority2CompMin").val();
        var progMax = $("#txtDistrictPriority2ProgMax").val();
        var CompMax = $("#txtDistrictPriority2CompMax").val();
        var FromMonth = $("#txtDistrictPriority2InspMonthFrom").val();
        var ToMonth = $("#txtDistrictPriority2InspMonthTo").val();
        var StateCode = $("#hdStateCode").val();
       
        if (!isInteger(progMin)) {
            alert("Enter Valid Progress Min Value.");
        }
        else if (!isInteger(CompMin)) {
            alert("Enter Valid Completed Min Value.");
        }
        else if (!isInteger(progMax)) {
            alert("Enter Valid Progress max Value.");
        }
        else if (!isInteger(CompMax)) {
            alert("Enter Valid Completed max Value.");
        }
        else if (!isInteger(FromMonth)) {
            alert("Enter Valid Inspection Min Month.");
        }
        else if (!isInteger(ToMonth)) {
            alert("Enter Valid Inspection Max Month.");
        }
        else {

            var data = { priority: 2, ProgressMin: progMin, ProgressMax: progMax, CompletedMin: CompMin, CompletedMax: CompMax, StateCode: StateCode, FromMonth: FromMonth, ToMonth: ToMonth };
            LoadDistrictReport($('#div2DistrictPriorityList'), data);

        }
    });

    $("#btnDistrictPriority3Submit").click(function () {
        var progMin = $("#txtDistrictPriority3Prog").val();
        var CompMin = $("#txtDistrictPriority3Comp").val();
        var FromMonth = $("#txtDistrictPriority3InspMonthFrom").val();
        var StateCode = $("#hdStateCode").val();

        if (!isInteger(progMin)) {
            alert("Enter Valid Progress Min Value.");
        }
        else if (!isInteger(CompMin)) {
            alert("Enter Valid Completed Min Value.");
        }
        else if (!isInteger(FromMonth)) {
            alert("Enter Valid Inspection Month.");
        }
        else {
            var data = { priority: 3, ProgressMin: progMin, ProgressMax: 0, CompletedMin: CompMin, CompletedMax: 0, StateCode: StateCode, FromMonth: FromMonth, ToMonth: 0 };
            LoadDistrictReport($('#div3DistrictPriorityList'), data);

        }
    });
    $("#txtDistrictPriority1Prog").focusout(function () {
        if (isInteger($("#txtDistrictPriority1Prog").val())) {
            $("#txtDistrictPriority2ProgMax").val($("#txtDistrictPriority1Prog").val());
            $("#txtDistrictPriority2ProgMin").val($("#txtDistrictPriority1Prog").val());
        }
        else {
            $("#txtDistrictPriority2ProgMax").val(0);
            $("#txtDistrictPriority2ProgMin").val(0);
        }
    });

    $("#txtDistrictPriority1Comp").focusout(function () {
        if (isInteger($("#txtDistrictPriority1Comp").val())) {
            $("#txtDistrictPriority2CompMax").val($("#txtDistrictPriority1Comp").val());
            $("#txtDistrictPriority2CompMin").val($("#txtDistrictPriority1Comp").val());
        }
        else {
            $("#txtDistrictPriority2CompMax").val(0);
            $("#txtDistrictPriority2CompMin").val(0);
        }
    });
    $("#txtDistrictPriority1InspMonthFrom").focusout(function () {
        if (isInteger($("#txtDistrictPriority1Prog").val())) {
            $("#txtDistrictPriority2InspMonthFrom").val($("#txtDistrictPriority1InspMonthFrom").val());
            $("#txtDistrictPriority2InspMonthTo").val($("#txtDistrictPriority1InspMonthFrom").val());
            
        }
        else {
            $("#txtDistrictPriority2InspMonthFrom").val(0);
            $("#txtDistrictPriority2InspMonthTo").val(0);
        }
    });

    $("#txtDistrictPriority2ProgMin").focusout(function () {
        if (isInteger($("#txtDistrictPriority2ProgMin").val()))
            $("#txtDistrictPriority3Prog").val($("#txtDistrictPriority2ProgMin").val());
        else
            $("#txtDistrictPriority3Prog").val(0);
    });

    

    $("#txtDistrictPriority2CompMin").focusout(function () {
        if (isInteger($("#txtDistrictPriority2CompMin").val()))
            $("#txtDistrictPriority3Comp").val($("#txtDistrictPriority2CompMin").val());
        else
            $("#txtDistrictPriority3Comp").val(0);
    });

    $("#txtDistrictPriority2InspMonthFrom").focusout(function () {
        if (isInteger($("#txtDistrictPriority1Prog").val())) {
            $("#txtDistrictPriority3InspMonthFrom").val($("#txtDistrictPriority2InspMonthFrom").val());
        }
        else {
            $("#txtDistrictPriority3InspMonthFrom").val(0);
        }
    });

}



function LoadDistrictReport(grid, data) {
    grid.load("/QMSSRSReports/QMSSRSReports/DistrictSchedulePriorityReport/", data);
}
