$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmTraceMaps'));
    $("#btnSearch").click(function () {
        //$.ajax({
        //    url: '/ExistingRoads/GetDistrictList/',
        //    data: $("#frmTraceMaps").serialize(),
        //    type: 'POST',
        //    catche: false,
        //    error: function (xhr, status, error) {
        //        alert('An Error occured while processig your request.')
        //        return false;
        //    },
        //    success: function (data) {

        //    }
        //});
        if ($("#frmTraceMaps").valid())
            LoadGrid();
        else
            return false
    });

    $("#spCollapseIconCN").click(function () {
        if ($("#tbldiv").is(':visible'))
            $("#tbldiv").hide("slow");
        else
            $("#tbldiv").show("slow");
    });

    if ($("#RoleCode").val() == 25) {
        $("#ddlStateSearch").change(function () {
            $.ajax({
                url: "/ExistingRoads/PopulateDistrictList/",
                type: "GET",
                cache: false,
                data: { StateCode: $("#ddlStateSearch option:selected").val(), statename: $("#ddlStateSearch option:selected").text() },
                beforeSend: function () {
                    blockPage();
                },
                error: function (xhr, status, error) {
                    unblockPage();
                    Alert("Request can not be processed at this time,please try after some time!!!");
                    return false;
                },
                success: function (response) {
                    unblockPage();
                    console.log(response);
                    $("#ddlDistrictSearch").empty();
                    for (var i = 0; i < response.length; i++) {
                        $("#ddlDistrictSearch").append("<option value=" + response[i].Value + ">" + response[i].Text + "</option>");

                    }
                    $("#ddlDistrictSearch option[selected='selected']").remove();

                }
            });
        });
    }


});

function GetColNamePDF() {
    if ($("#RoleCode").val() == "22")
        return "View Uploaded PDF";
    else
        return "Upload / View PDF file";
}

function GetColNameCSV() {
    if ($("#RoleCode").val() == "22")
        return "View Uploaded CSV";
    else
        return "Upload / View CSV file";
}

function GetColNameHabCSV() {
    if ($("#RoleCode").val() == "22")
        return "View Uploaded Habitation CSV";
    else
        return "Upload / View Habitation CSV file";
}

function LoadGrid() {



    $('#tblTraceMapList').jqGrid('GridUnload');
    jQuery("#tblTraceMapList").jqGrid({
        url: '/ExistingRoads/GetDistrictList',
        datatype: "json",
        mtype: "POST",
        colNames: ["Distict Name", "Block Name", 'Download Facility List', GetColNamePDF(), GetColNameCSV(), GetColNameHabCSV(), GetFinalizeStatus(), "Definalize"],
        colModel: [
                             { name: 'Districtname', index: 'Districtname', height: 'auto', width: 140, align: "center", hidden: isMordLogin() },
                             { name: 'BlockName', index: 'BlockName', height: 'auto', width: 140, align: "center", hidden: false },
                             { name: 'DownloadExcel', index: 'DownloadExcel', height: 'auto', width: 120, align: "center", hidden: false },
                             { name: 'UploadPDF', index: 'UploadPDF', height: 'auto', width: 120, align: "center", hidden: false },
                             { name: 'UploadCSV', index: 'UploadCSV', height: 'auto', width: 120, align: "center", hidden: false },

                             { name: 'UploadHabCSV', index: 'UploadHabCSV', height: 'auto', width: 120, align: "center", hidden: false }, // UploadHabCSV()
                             { name: 'Finalize', index: 'Finalize', height: 'auto', width: 120, align: "center", hidden: false },
                             { name: 'Definalize', index: 'Finalize', height: 'auto', width: 120, align: "center", hidden: isMordLogin() },
        ],
        postData: { formdata: $("#frmTraceMaps").serialize(), DistrictCodeDD: $("#ddlDistrictSearch option:selected").val(), statecode: $("#ddlStateSearch option:selected").val() },
        pager: jQuery('#dvTraceMapsListPager'),
        rowNum: 15,
        rowList: [15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortorder: "asc",
        sortname: 'BlockName',
        caption: "Trace Maps List. ",
        height: 'auto',
        autowidth: true,
        rownumbers: true,

        loadComplete: function () {

        },

        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")

            }
        }
    });
}


function UploadPDF(blockcode) {
    var random = Math.random();

    $("#accordionMonitorsInspection div").html("");
    $("#accordionMonitorsInspection h3").html(
            "<a href='#' style= 'font-size:15px;' >&nbsp;&nbsp;  Upload File</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="closeMonitorsInspectionDetails();" /></a>' +
            '<span style="float: right;"></span>'
            );

    var number = Math.floor((Math.random() * 99999999) + 1);

    $('#accordionMonitorsInspection').show('slow', function () {
        blockPage();
        $("#divFileTraceMapsUpload").load('/ExistingRoads/PdfFileUploadView/' + blockcode, function () {
            unblockPage();
        });
    });

    $("#divFileTraceMapsUpload").css('height', 'auto');
    $('#divFileTraceMapsUpload').show('slow');

}

function closeMonitorsInspectionDetails() {
    $('#accordionMonitorsInspection').hide('slow');
    $('#divFileTraceMapsUpload').hide('slow');
    //$("#tbMonitorsInspectionList").jqGrid('setGridState', 'visible');
}

function UploadCSV(blockcode) {
    var random = Math.random();

    $("#accordionMonitorsInspection div").html("");
    $("#accordionMonitorsInspection h3").html(
            "<a href='#' style= 'font-size:15px;' >&nbsp;&nbsp;  Upload File</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="closeMonitorsInspectionDetails();" /></a>' +
            '<span style="float: right;"></span>'
            );

    var number = Math.floor((Math.random() * 99999999) + 1);

    $('#accordionMonitorsInspection').show('slow', function () {
        blockPage();
        $("#divFileTraceMapsUpload").load('/ExistingRoads/CsvFileUploadView/' + blockcode, function () {
            unblockPage();
        });
    });

    $("#divFileTraceMapsUpload").css('height', 'auto');
    $('#divFileTraceMapsUpload').show('slow');

}

function UploadHabCSV(blockcode) {
    var random = Math.random();

    $("#accordionMonitorsInspection div").html("");
    $("#accordionMonitorsInspection h3").html(
            "<a href='#' style= 'font-size:15px;' >&nbsp;&nbsp;  Upload File</a>" +

            '<a href="#" style="float: right;">' +
            '<img  style="border:none;border-width: 0; " class="ui-icon ui-icon-closethick" style="border-width: 0;" onclick="closeMonitorsInspectionDetails();" /></a>' +
            '<span style="float: right;"></span>'
            );

    var number = Math.floor((Math.random() * 99999999) + 1);

    $('#accordionMonitorsInspection').show('slow', function () {
        blockPage();
        $("#divFileTraceMapsUpload").load('/ExistingRoads/HabCsvFileUploadView/' + blockcode, function () {
            unblockPage();
        });
    });

    $("#divFileTraceMapsUpload").css('height', 'auto');
    $('#divFileTraceMapsUpload').show('slow');

}


function closeMonitorsInspectionDetails() {
    $('#accordionMonitorsInspection').hide('slow');
    $('#divFileTraceMapsUpload').hide('slow');
    //$("#tbMonitorsInspectionList").jqGrid('setGridState', 'visible');
}

function FinalisePDFDetails(id) {

    if (confirm("Are you sure to Finalize the details ? ")) {

        $.ajax({
            url: "/ExistingRoads/FinaliseFileDetails/",
            type: "POST",
            cache: false,
            data: { BlockCode: id },
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            },
            success: function (response) {
                unblockPage();
                if (response.success) {
                    //    $("#NumberofFiles").val(parseFloat($("#NumberofFiles").val()) - 1);
                    alert("Details has been finalized Succesfully.");

                    $("#tblTraceMapList").trigger('reloadGrid');
                    $("#tbPDFFilesList").trigger('reloadGrid');
                    $("#tbCSVFilesList").trigger('reloadGrid');
                    closeMonitorsInspectionDetails();
                    $(".ui-icon-closethick").trigger("click");

                }
                else {
                    alert("Something went wrong.");
                }
            }
        });

    }
    else {
        return;
    }
}

//function DownloadFacilityList(id) {
//    $.ajax({
//        url: "/ExistingRoads/DownloadFacilityList/",
//            type: "POST",
//            cache: false,
//            data: { BlockCode: id },
//            beforeSend: function () {
//                blockPage();
//            },
//            error: function (xhr, status, error) {
//                unblockPage();
//                Alert("Request can not be processed at this time,please try after some time!!!");
//                return false;
//            },
//            success: function (response) {
//                unblockPage();
//                if (response.success) {
//                    //    $("#NumberofFiles").val(parseFloat($("#NumberofFiles").val()) - 1);
//                    alert("Details has been finalized Succesfully.");

//                    $("#tblTraceMapList").trigger('reloadGrid');
//                    $("#tbPDFFilesList").trigger('reloadGrid');
//                    $("#tbCSVFilesList").trigger('reloadGrid');
//                    closeMonitorsInspectionDetails();
//                    $(".ui-icon-closethick").trigger("click");

//                }
//                else {
//                    alert("Something went wrong.");
//                }
//            }
//        });
//}
function DownloadFacilityList(id) {
    window.location.href = "/ExistingRoads/DownloadFacilityList/" + id;
}


function isMordLogin()
{
    if ($("#RoleCode").val() == 25) {
        return false;
    }
    else
    {
        return true;
    }
}

function DefinalizeTraceMapDetails(id) {

    if (confirm("Are you sure to DeFinalize the details ? ")) {

        $.ajax({
            url: "/ExistingRoads/DefinalizeTraceMaps/",
            type: "POST",
            cache: false,
            data: { BlockCode: id },
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            },
            success: function (response) {
                unblockPage();
                if (response.success) {
                    //    $("#NumberofFiles").val(parseFloat($("#NumberofFiles").val()) - 1);
                    alert("Trace Maps definalized Succesfully.");

                    $("#tblTraceMapList").trigger('reloadGrid');
                    $("#tbPDFFilesList").trigger('reloadGrid');
                    $("#tbCSVFilesList").trigger('reloadGrid');
                    closeMonitorsInspectionDetails();
                    $(".ui-icon-closethick").trigger("click");

                }
                else {
                    alert("Something went wrong.");
                }
            }
        });

    }
    else {
        return;
    }
}

function GetFinalizeStatus()
{
    if ($("#RoleCode").val() == 25) {
        return "Status";
    }
    else
    {
        return "Finalize";
    }
}