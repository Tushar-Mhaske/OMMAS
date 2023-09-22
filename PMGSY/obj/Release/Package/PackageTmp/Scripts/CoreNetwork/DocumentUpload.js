$(function () {
    'use strict';

    // Initialize the jQuery File Upload widget:
    $('#fileupload').fileupload({
        maxNumberOfFiles: 5 - $("#NumberOfFiles").val() ,
        acceptFileTypes: /(\.|\/)(kml|KML|kmlz|KMLZ)$/i,
        maxFileSize: 4194304,
    });

    //Enable iframe cross-domain access via redirect option:
    $('#fileupload').fileupload(
        'option',
        'redirect',
        window.location.href.replace(
            /\/[^\/]*$/,
            '/cors/result.html?%s'
        )
    );

    // Load existing files:
    $.ajax({
        url: $('#fileupload').fileupload('option', 'url'),
        dataType: 'json',
        context: $('#fileupload')[0]
    }).done(function (result) {
        $(this).fileupload('option', 'done')
            .call(this, null, { result: result });
    });


    // For validation
    $('#fileupload').bind('fileuploadsubmit', function (e, data) {

        var flagStartChainage = true;
        var flagEndChainage = true;
        var StartChainage ="";
        var EndChainage = "";
        var flagCompareChainage = true;
        var start = 0;
        var end = 0;
        var startChainage = [];//new Array();
        var endChainage = []//new Array();
        $("#tblPresentation input[name='StartChainage[]']").each(function () {

            var regExAmount = /^\d{1,18}(\.\d{1,2})?$/;
            StartChainage = $(this).val();
            

            if (StartChainage == "") {
                flagStartChainage = false;
                $("#divError").show('slow');
                $("#divError").html("Please Enter Start Chainage");
                $(this).focus();
                return false;
            }
            if (!regExAmount.test(StartChainage)) {
                $("#divError").show('slow');
                $("#divError").html("Please Enter valid Start Chainage,Only Numeric values,Total 16 Digits and 2 digits after decimal place are allowed.");
                flagStartChainage = false;
                return false;
            }

            startChainage.push($(this).val());

        });

        $("#tblPresentation input[name='EndChainage[]']").each(function () {

            var regExAmount = /^\d{1,18}(\.\d{1,2})?$/;;
            EndChainage = $(this).val();
            

            if (EndChainage == "") {
                flagEndChainage = false;
                $("#divError").show('slow');
                $("#divError").html("Please Enter End Chainage");
                $(this).focus();
                return false;
            }

            if (!regExAmount.test(EndChainage)) {
                $("#divError").show('slow');
                $("#divError").html("Please Enter valid End Chainage,Only Numeric values,Total 16 Digits and 2 digits after decimal place are allowed.");
                flagEndChainage = false;
                return false;
            }

            endChainage.push($(this).val());
        });

        for (var item = 0; item < startChainage.length; item ++) {
            
            if (parseFloat(endChainage[item]) <= parseFloat(startChainage[item])) {
                flagCompareChainage = false;
                $("#divError").show();
                $("#divError").html("Start Chainage must be less than End Chainage.");
                $(this).focus();
                return false;
            }
        }

        var inputs = data.context.find(':input');
        var text = data.context.find('text');
        data.formData = inputs.serializeArray();

        if (flagStartChainage && flagEndChainage && flagCompareChainage) {
            return true;
        }

        return false;

    });

    $("#fileupload").bind('fileuploadfail', function (e, data) {
        $("#divGlobalProgress").html("");
    });

    $("#fileupload").bind("fileuploaddone", function (e, data) {
        $("#divSuccess").show("slow");
        $("#divError").hide();
        $("#tbPDFFilesList").trigger('reloadGrid');
    });

    $('#fileupload').bind('fileuploadadd', function (e, data) {
        $("#divSuccess").hide("slow");
        $("#divError").hide();
    });

});

$(document).ready(function () {
    ListFiles($("#PLAN_CN_ROAD_CODE").val());
});


// List of Uploaded Files
function ListFiles(PLAN_CN_ROAD_CODE) {

    blockPage();
    jQuery("#tbPDFFilesList").jqGrid({
        url: '/CoreNetWork/ListFiles',
        datatype: "json",
        mtype: "POST",
        colNames: ["File", "Start Chainage(in Kms)","End Chainage(in KMs)","Uploaded Date", "Edit", "Delete", "Action"],
        colModel: [
                    { name: 'PDF', index: 'PDF', width: 125, sortable: false, align: "center", formatter: AnchorFormatter, search: false, editable: false },
                    { name: 'PLAN_START_CHAINAGE', index: 'PLAN_START_CHAINAGE Chainage', sortable: false, align: "center", search: false, editable: true, editoptions: { maxlength: 255 }, editrules: { custom: true, custom_func: ValidateStartChainage } },
                    { name: 'PLAN_END_CHAINAGE', index: 'PLAN_END_CHAINAGE', sortable: false, align: "center", search: false, editable: true, editoptions: { maxlength: 255 }, editrules: { custom: true, custom_func: ValidateEndChainage} },
                    { name: 'PLAN_UPLOAD_DATE', index: 'PLAN_UPLOAD_DATE Date', sortable: false, align: "center", search: false, editable: false },
                    { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center", editable: false },
                    { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center", editable: false },
                    { name: 'Action', index: 'Save', width: 80, sortable: false, align: "center", editable: false,hidden:true }
        ],
        postData: { "PLAN_CN_ROAD_CODE": PLAN_CN_ROAD_CODE },
        pager: jQuery('#dvPDFFilesListPager'),
        rowNum: 5,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Files",
        height: 'auto',
        sortname: 'PDF',
        rownumbers: true,
        editurl: "/CoreNetWork/UpdatePDFDetails",
        loadComplete: function () {
            unblockPage();
        },
        loadError: function (xhr, ststus, error) {
            if (xhr.responseText == "session expired") {
                unblockPage();
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                unblockPage();
                alert("Session Timeout !!!");
                window.location.href = "/Login/LogIn";
            }
            unblockPage();
        }
    }); //end of grid    
}

function EditFileDetails(paramFileID) {
    jQuery("#tbPDFFilesList").editRow(paramFileID);
    $("#tbPDFFilesList").jqGrid('showCol', 'Action');
}

function SaveDetails(paramFileID) {
    jQuery("#tbPDFFilesList").saveRow(paramFileID, checksave);
}

function CancelSaveDetails(paramFileID) {
    jQuery("#tbPDFFilesList").restoreRow(paramFileID);
}

function checksave(result) {
    $("#tbPDFFilesList").jqGrid('hideCol', 'Action');
    if (result.responseText == "true") {
        alert('Details updated successfully.');
        return true;
    }
    else if (result.responseText != "") {
        alert(result.responseText.replace('"', "").replace('"', ""));
        return false;
    }
}

function ValidateStartChainage(value, colname) {

    var regEx = /^(?=.+)(?:[0-9]\d*|0)?(?:\.\d+)?$/;
    if (!regEx.test(value)) {
        //alert("Invalid Start Chainage,Only Numbers are allowed.");
        return [ "Invalid Start Chainage,Only Numbers are allowed."]
    }
    else {
        return [true,""];
    }
}

function ValidateEndChainage(value, colname) {

    var regEx = /^(?=.+)(?:[0-9]\d*|0)?(?:\.\d+)?$/;
    if (!regEx.test(value)) {
        //alert("Invalid Start Chainage,Only Numbers are allowed.");
       return ["Invalid Start Chainage,Only Numbers are allowed."];
    }
    else {
        return [true];
    }
}

function DownloadFile(paramFileName) {
    $.ajax({
        url: '/CoreNetWork/DownloadFile',
        type: 'POST',
        beforeSend: function () {
            blockPage();
        },
        data: { PLAN_FILE_NAME: paramFileName, value: Math.random() },
        success: function (response) {
            unblockPage();
            if (response.Success) {

            }
        },
        error: function (xhr, AjaxOptions, thrownError) {
            alert("Error occured while processing the request.");
            unblockPage();
        }
    });
}

function downloadFileFromAction(paramurl) {
    //window.location = paramurl;
    window.open(paramurl, '_blank');
}

function AnchorFormatter(cellvalue, options, rowObject) {

    var url = "/CoreNetWork/DownloadFile/" + cellvalue;

    return "<a href='#' onclick=downloadFileFromAction('" + url + "'); return false;> <img style='height:16px;width:16px' height='20' width='20' border=0 src='../../Content/images/FileIcon.png' /> </a>";
}

function DeleteFileDetails(PLAN_CN_ROAD_CODE, PLAN_FILE_NAME) {

    if (confirm("Are you sure to delete the File and it's details ? ")) {
        $.ajax({
            url: "/CoreNetWork/DeleteFileDetails/",
            type: "POST",
            cache: false,
            data: {
                PLAN_CN_ROAD_CODE: PLAN_CN_ROAD_CODE, PLAN_FILE_NAME: PLAN_FILE_NAME
            },
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
                if (response.Success) {
                    $("#tbPDFFilesList").trigger('reloadGrid');
                    $("#NumberOfFiles").val(parseFloat($("#NumberOfFiles").val()) - 1);
                    alert("File Details Deleted Succesfully.");
                    $("#divAddForm").load('/CoreNetWork/CoreNetworkFileUpload/' + $("#PLAN_CN_ROAD_CODE").val(), function () {
                        $.validator.unobtrusive.parse($('#fileupload'));
                        unblockPage();
                    });
                }
                else {
                    alert(response.ErrorMessage);
                }
            }
        });
    }
    else {
        return;
    }
}