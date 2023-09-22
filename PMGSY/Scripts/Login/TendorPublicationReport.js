$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmECBriefLayout'));
    $("#ddlECState").change(function () {
        //loadDistrict($("#ddlECState").val());
        $("#ddlECAgency").empty();
        $("#ddlECCollaboration").empty();
        $("#ddlECDistrict").empty();
        $("#ddlECBlock").empty();
        $("#ddlECBlock").append("<option value='0'>All Blocks</option>");

        $.ajax({
            url: '/ECBriefReport/ECBriefReport/PopulateCollaborations',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { stateCode: $("#ddlECState").val(), value: Math.random() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    if (jsonData[i].Value == 2) {
                        $("#ddlECCollaboration").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlECCollaboration").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                }

                $.unblockUI();
            },
            error: function (err) {
                //alert("error " + err);
                $.unblockUI();
            }
        });




        $.ajax({
            url: '/ECBriefReport/ECBriefReport/PopulateAgencies',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { stateCode: $("#ddlECState").val(), value: Math.random() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    if (jsonData[i].Selected == true) {
                        $("#ddlECAgency").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlECAgency").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                }

                $.unblockUI();
            },
            error: function (err) {
                //alert("error " + err);
                $.unblockUI();
            }
        });

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
                //alert("error " + err);
                $.unblockUI();
            }
        });


    });

    //$("#ddlECDistrict").change(function () {
    //    $("#ddlECBlock").empty();

    //    $.ajax({
    //        url: '/ECBriefReport/ECBriefReport/PopulateBlocks',
    //        type: 'POST',
    //        beforeSend: function () {
    //            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
    //        },
    //        data: { DistrictCode: $("#ddlECDistrict").val(), value: Math.random() },
    //        success: function (jsonData) {
    //            for (var i = 0; i < jsonData.length; i++) {
    //                if (jsonData[i].Value == 2) {
    //                    $("#ddlECBlock").append("<option value='" + jsonData[i].Value + "'selected>" + jsonData[i].Text + "</option>");
    //                }
    //                else {
    //                    $("#ddlECBlock").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
    //                }
    //            }

    //            $.unblockUI();
    //        },
    //        error: function (err) {
    //            //alert("error " + err);
    //            $.unblockUI();
    //        }
    //    });
    //});

    //$("#ddlECDistrict").change(function () {
    //    loadBlock($("#ddlECState").val(), $("#ddlECDistrict").val());

    //});
    $("#btnViewEC").click(function () {
        //if ($('#RoadWiseCheck_AnaAvgLengthDetail').prop("checked") == true) {
        //if ($("#ddlECState").is(":visible")) {
        //alert($('#frmECBriefLayout').valid());
        //if ($("#ddlECState").val() <= 0) {
        //    //alert("Please select State");
        //    return false;
        //}
        //}
        //}

        if ($('#frmECBriefLayout').valid()) {
            $("#divLoadECReport").html("");

            //$("#StateName").val($("#ddlECState option:selected").text());
            //$("#State_Name").val($("#ddlECState option:selected").text());
            //$("#StatusName").val($("#StatusList_AnaAvgLengthDetail option:selected").text());
            //$("#BatchName").val($("#BatchList_AnaAvgLengthDetail option:selected").text());
            //$("#YearName").val($("#PhaseYearList_AnaAvgLengthDetail option:selected").text());

            if ($("#ddlECState").is(":visible")) {
                //alert("1");
                $("#StateName").val($("#ddlECState option:selected").text());
            }

            if ($("#ddlECDistrict").is(":visible")) {

                //$('#DistrictList_AnaAvgLengthDetail').attr("disabled", false);
                $("#DistName").val($("#ddlECDistrict option:selected").text());
            }
            if ($("#ddlECBlock").is(":visible")) {

                $("#BlockName").val($("#ddlECBlock option:selected").text());
            }
            if ($("#ddlECCollaboration").is(":visible")) {

                $("#CollabName").val($("#ddlECCollaboration option:selected").text());
            }
            if ($("#ddlECAgency").is(":visible")) {

                $("#AgencyName").val($("#ddlECAgency option:selected").text());
            }
            if ($("#ddlECBatch").is(":visible")) {

                $("#BatchName").val($("#ddlECBatch option:selected").text());
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/Login/TendorPublicationReportLayoutPost/',
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

    //$("#btnViewEC").trigger('click');
    //if ($('#Mast_State_Code').val() > 0) {
    //    $("#btnViewAnaAvgLengthDetail").trigger('click');
    //}
    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvECBriefParameter").toggle("slow");

    });

    //this function call  on layout.js
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