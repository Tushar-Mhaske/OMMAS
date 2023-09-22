jQuery.validator.addMethod("datecomparefieldvalidator", function (value, element, param) {

    var fromDate = $('#StartDate').val();
    var toDate = $('#EndDate').val();

    var frommonthfield = fromDate.split("/")[1];
    var fromdayfield = fromDate.split("/")[0];
    var fromyearfield = fromDate.split("/")[2];

    var tomonthfield = toDate.split("/")[1];
    var todayfield = toDate.split("/")[0];
    var toyearfield = toDate.split("/")[2];

    var sDate = new Date(fromyearfield, frommonthfield, fromdayfield);
    var eDate = new Date(toyearfield, tomonthfield, todayfield);

    //var endDate = $('#ToDate').datepicker("getDate");
    //var currentDate = new Date();

    if (sDate > eDate) {
        return false;
    }
    else {
        return true;
    }

});

jQuery.validator.unobtrusive.adapters.addBool("datecomparefieldvalidator");


jQuery.validator.addMethod("currentdatefieldvalidator", function (value, element, param) {

    var fromDate = $('#StartDate').val();
    var toDate = $('#EndDate').val();

    var frommonthfield = fromDate.split("/")[1];
    var fromdayfield = fromDate.split("/")[0];
    var fromyearfield = fromDate.split("/")[2];

    var tomonthfield = toDate.split("/")[1];
    var todayfield = toDate.split("/")[0];
    var toyearfield = toDate.split("/")[2];

    var sDate = new Date(fromyearfield, frommonthfield, fromdayfield);
    var eDate = new Date(toyearfield, tomonthfield, todayfield);

    var endDate = $('#EndDate').datepicker("getDate");
    var currentDate = new Date();

    if (endDate > currentDate) {
        return false;
    }
    else {
        return true;
    }

});

jQuery.validator.unobtrusive.adapters.addBool("currentdatefieldvalidator");

jQuery.validator.unobtrusive.adapters.addBool("requiredDateFields", function (value, element, param)
{
    alert("sd");
    var fromDate = $('#StartDate').val();
    var toDate = $('#EndDate').val();
    if (fromDate == "" || toDate == "")
        return false;
    else
        return true;
});
jQuery.validator.unobtrusive.adapters.addBool("requiredDateFields");


$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmECBriefLayout'));

    $('#StartDate').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a from date',
        buttonImageOnly: true,
        title: "Start Date",
        changeMonth: true,
        changeYear: true,
        buttonText: "Valid Date",
        maxDate: "-1d",
    });

    $('#EndDate').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a to date',
        buttonImageOnly: true,
        title: "End Date",
        //maxDate: new Date(),
        changeMonth: true,
        changeYear: true,
        buttonText: "End Date",
        maxDate: "-1d",
    });

      $("#ddlECState").change(function () {
        //loadDistrict($("#ddlECState").val());
        $("#ddlECDistrict").empty();
        $.ajax({
            url: '/ECBriefReport/ECBriefReport/PopulateDistricts',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { stateCode: $("#ddlECState").val(), value: Math.random() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    $("#ddlECDistrict").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                }
                $.unblockUI();
            },
            error: function (err) {
                $.unblockUI();
            }
        });
    });

    $("#btnViewEC").click(function () {
        if ($('#frmECBriefLayout').valid()) {
            $("#divLoadECReport").html("");

            if ($("#ddlECState").is(":visible")) {
                $("#StateName").val($("#ddlECState option:selected").text());
            }

            if ($("#ddlECDistrict").is(":visible")) {
                $("#DistName").val($("#ddlECDistrict option:selected").text());
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/OtherReports/OtherReports/GetWorksWithoutAGRPost/',
                type: 'POST',
                catche: false,
                data: $("#frmECBriefLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#divLoadECReport").html(response);

                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });

        }
        else {

        }
    });

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvECBriefParameter").toggle("slow");

    });

    closableNoteDiv("divCommonReport", "spnCommonReport");
});

//function loadDistrict(statCode) {
//    $("#ddlECDistrict").val(0);
//    $("#ddlECDistrict").empty();
//    $("#ddlECCollaboration").val(0);
//    $("#ddlECCollaboration").empty();
//    $("#ddlECCollaboration").append("<option value='0'>All Blocks</option>");

//    if (statCode > 0) {
//        if ($("#ddlECDistrict").length > 0) {
//            $.ajax({
//                url: '/ECBriefReport/ECBriefReport/PopulateCollaborationsStateWise',
//                type: 'POST',
//                data: { "StateCode": statCode },
//                success: function (jsonData) {
//                    for (var i = 0; i < jsonData.length; i++) {
//                        $("#DistrictList_AnaAvgLengthDetail").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
//                    }
//                    //$('#DistrictList_AnaAvgLengthDetail').find("option[value='0']").remove();
//                    //$("#DistrictList_AnaAvgLengthDetail").append("<option value='0'>Select District</option>");
//                    //$('#DistrictList_AnaAvgLengthDetail').val(0);

//                    //For Disable if District Login
//                    if ($("#Mast_District_Code").val() > 0) {
//                        $("#DistrictList_AnaAvgLengthDetail").val($("#Mast_District_Code").val());
//                        // $("#DistrictList_AnaAvgLengthDetail").attr("disabled", "disabled");
//                        $("#DistrictList_AnaAvgLengthDetail").trigger('change');
//                    }


//                },
//                error: function (xhr, ajaxOptions, thrownError) {
//                    alert(xhr.status);
//                    alert(thrownError);
//                }
//            });
//        }
//    }
//    else {

//        $("#DistrictList_AnaAvgLengthDetail").append("<option value='0'>All Districts</option>");
//        $("#BlockList_AnaAvgLengthDetail").empty();
//        $("#BlockList_AnaAvgLengthDetail").append("<option value='0'>All Blocks</option>");

//    }
//}

//District Change Fill Block DropDown List
//function loadBlock(stateCode, districtCode) {
//    $("#BlockList_AnaAvgLengthDetail").val(0);
//    $("#BlockList_AnaAvgLengthDetail").empty();

//    if (districtCode > 0) {
//        if ($("#BlockList_AnaAvgLengthDetail").length > 0) {
//            $.ajax({
//                url: '/AnalysisSSRSReport/AnalysisSSRSReport/BlockDetails',
//                type: 'POST',
//                data: { "StateCode": stateCode, "DistrictCode": districtCode },
//                success: function (jsonData) {
//                    for (var i = 0; i < jsonData.length; i++) {
//                        $("#BlockList_AnaAvgLengthDetail").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
//                    }

//                    if ($("#Mast_Block_Code").val() > 0) {
//                        $("#BlockList_AnaAvgLengthDetail").val($("#Mast_Block_Code").val());
//                        // $("#BlockList_AnaAvgLengthDetail").attr("disabled", "disabled");
//                        //$("#BlockList_AnaAvgLengthDetail").trigger('change');
//                    }
//                    //$('#BlockList_AnaAvgLengthDetail').find("option[value='0']").remove();
//                    //$("#BlockList_AnaAvgLengthDetail").append("<option value='0'>Select Block</option>");
//                    //$('#BlockList_AnaAvgLengthDetail').val(0);


//                },
//                error: function (xhr, ajaxOptions, thrownError) {
//                    alert(xhr.status);
//                    alert(thrownError);
//                }
//            });
//        }
//    } else {
//        $("#BlockList_AnaAvgLengthDetail").append("<option value='0'>All Blocks</option>");
//    }
//}