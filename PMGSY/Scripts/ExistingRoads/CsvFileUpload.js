$(document).ready(function () {
    $(function () {
        'use strict';
        // Initialize the jQuery File Upload widget:
        $('#fileupload').fileupload({
            maxNumberOfFiles: 1,
            acceptFileTypes: /(\.|\/)(csv|CSV)$/i,
            maxFileSize: 10000000,
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

       //Load existing files:
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
            var inputs = data.context.find(':input');
        //    //if (inputs.filter('[required][value=""]').first().focus().length) {
        //    //    alert("Please Enter Description");
        //    //    return false;
        //    //}

        //    //var Remarks = $("#txtRemark").val().trim();
        //    //if (Remarks.length == 0) {
        //    //    alert("Please Enter Description");
        //    //    return false;
        //    //}

        //    //if (!Remarks.match("^[a-zA-Z0-9 ,.()-]+$")) {
        //    //    alert("Please Enter Valid Remarks,Can only contains AlphaNumeric values and [,.()-].");
        //    //    return false;
        //    //}

            data.formData = inputs.serializeArray();
        });

        $("#fileupload").bind("fileuploaddone", function (e, data) {
            if (!data.result.success) {
                alert(data.result.message);
                closeMonitorsInspectionDetails();
            }
            else {
                $("#divSuccess").show("slow");

                $("#tbCSVFilesList").trigger('reloadGrid');
            }
            $('#tblTraceMapList').trigger('reloadGrid');
        });

        $("#fileupload").bind('fileuploadfail', function (e, data) {
            //$("#tblPresentation").find('div.progress').find('div:eq(0)').css('width', '0px');
            //$("#tblPresentation").find('tr.template-upload').css('display', 'none');
            $("#divGlobalProgress").html("");
        });

        $('#fileupload').bind('fileuploadadd', function (e, data) {
            $("#divSuccess").hide("slow");

        });

        $('#fileupload').bind('fileuploaddestroy', function (e, data) {
            //alert('Destroy');
        });

    });


    ListPDFFiles($("#BlockCode").val());
});

function ListPDFFiles(BlockCode) {
    blockPage();
    jQuery("#tbCSVFilesList").jqGrid({
        url: '/ExistingRoads/ListInspMultipleCSVFiles',
        datatype: "json",
        mtype: "POST",
        colNames: ["Click Icon To View / Download CSV", "Delete", "Is Finalized", "Finalise"],
        colModel: [
                    { name: 'CSV', index: 'PDF', width: 125, sortable: false, align: "center", formatter: AnchorFormatter, search: false, editable: false },
                    { name: 'Delete', index: 'Delete', width: 60, sortable: false, align: "center", editable: false },
                    { name: 'IsFinalised', index: 'IsFinalised', width: 60, sortable: false, align: "center", editable: false },
                    { name: 'Finalise', index: 'Finalised', width: 60, sortable: false, align: "center", editable: false , hidden : true },
        ],
        postData: { "blockcode": BlockCode },
        pager: jQuery('#dvCSVFilesListPager'),
        rowList: [04, 08, 12],
        rowNum: 04,
        viewrecords: true,
        recordtext: '{2} record found',
        caption: "Files",
        height: 'auto',
        sortname: 'PDF',
        //autowidth: true,
        rownumbers: true,
        //editurl: "/QualityMonitoring/UpdateMultipleInspPDFDetails",
        loadComplete: function () {
            $('#tbCSVFilesList').jqGrid('setGridWidth', '600');
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

function EditPDFDetails(paramFileID) {
    $('#tbPDFFilesList').jqGrid('showCol', 'Save');
    jQuery("#tbPDFFilesList").editRow(paramFileID);
}

function SavePDFDetails(paramFileID) {

    jQuery("#tbPDFFilesList").saveRow(paramFileID, checksave);
}

function CancelSavePDFDetails(paramFileID) {
    $('#tbPDFFilesList').jqGrid('hideCol', 'Save');
    jQuery("#tbPDFFilesList").restoreRow(paramFileID);
}

function checksave(result) {
    $('#tbPDFFilesList').jqGrid('hideCol', 'Save');
    if (result.responseText == "true") {
        alert('Details updated successfully.');
        return true;
    }
    else if (result.responseText != "") {
        alert(result.responseText.replace('"', "").replace('"', ""));
        return false;
    }
}

function ValidatePDFDescription(value, colname) {

    //alert((/^[a-zA-Z0-9  ,.()-]+$/).test(value));
    //alert((/^[a-zA-Z0-9  ,.()-]+$/).test(value)  + " "+ value);

    if (value.trim().length == 0) {
        return ["Please Enter Description."];
    }
    else if ((/^[a-zA-Z0-9 ,.()-]+$/).test(value)) {
        return ["Invalid Description,Only Alphabets and Numbers are allowed."];
    }
    else {
        return [true, ""];
    }
}

function downloadFileFromAction(paramurl) {
    window.location = paramurl;
}

function AnchorFormatter(cellvalue, options, rowObject) {
    var url = "/ExistingRoads/DownloadMultipleCSVFile/" + cellvalue;
    return "<a title='Click here to Download file' href='#' onclick=downloadFileFromAction('" + url + "'); return false;> <img style='height:16px;width:16px' height='16' width='16' border=0 src='../../Content/images/csv_icon.png' /></a>";
}

function DeleteCSVFileDetails(id) {
    if (confirm("Are you sure to delete the CSV and it's details ? ")) {

        $.ajax({
            url: "/ExistingRoads/DeleteCSVFileDetails/",
            type: "POST",
            cache: false,
            data: {
                FileID: id, value: Math.random()
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
                    $("#NumberofPdfs").val(parseFloat($("#NumberofFiles").val()) - 1);
                    alert("PDF and it's Details Deleted Succesfully.");
                    closeMonitorsInspectionDetails();
                    $("#tbPDFFilesList").trigger('reloadGrid');
                    $('#tblTraceMapList').trigger('reloadGrid');
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



function submitcsvFile()
{
    $.ajax({
        url: "/ExistingRoads/GetCSVValue/",
        type: "POST",
        cache: false,
        data: {
            formdata: $("#fileupload").serialize()
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
            if (!response.success) {

                alert(response.message)
                $("#divSuccess").hide();
            }
            else {
                alert(response.ErrorMessage);
            }
        }
    });
}
