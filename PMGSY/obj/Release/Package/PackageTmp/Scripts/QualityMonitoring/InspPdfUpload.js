

$(document).ready(function () {
    $(function () {
        'use strict';
        // Initialize the jQuery File Upload widget:
        $('#fileupload').fileupload({
            maxNumberOfFiles: 1 - parseInt($("#NumberofPdfs").val()),
            acceptFileTypes: /(\.|\/)(pdf|PDF)$/i,
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
            var inputs = data.context.find(':input');
            if (inputs.filter('[required][value=""]').first().focus().length) {
                alert("Please Enter Description");
                return false;
            }

            var Remarks = $("#txtRemark").val().trim();
            if (Remarks.length == 0) {
                alert("Please Enter Description");
                return false;
            }

            if (!Remarks.match("^[a-zA-Z0-9 ,.()-]+$")) {
                //alert("Please Enter Valid Remarks,Can only contains AlphaNumeric values and [,.()-].");
                alert("Please Enter Valid Remarks,Can only contains AlphaNumeric values and ,.()-");
                return false;
            }

            data.formData = inputs.serializeArray();
        });

        $("#fileupload").bind("fileuploaddone", function (e, data) {
            $("#divSuccess").show("slow");

            $("#tbPDFFilesList").trigger('reloadGrid');
            $("#tbMonitorsInspectionList").trigger('reloadGrid');
            $("#tb2TierSqcInspList").trigger('reloadGrid');
            $("#tb3TierSqcInspList").trigger('reloadGrid');
            $("#tb3TierInspectionList").trigger('reloadGrid');
            $("#tb3TierSqcInspList").trigger('reloadGrid');

            $("#tblPresentation tbody tr").remove();
            $("#divGlobalProgress").html("");
        });

        $("#fileupload").bind('fileuploadfail', function (e, data) {
            //$("#tblPresentation").find('div.progress').find('div:eq(0)').css('width', '0px');
            $("#tblPresentation").find('tr.template-upload').css('display', 'none');  //Changed 28-02-2022
            $("#divGlobalProgress").html("");
        });

        $('#fileupload').bind('fileuploadadd', function (e, data) {
            $("#divSuccess").hide("slow");

        });

        $('#fileupload').bind('fileuploaddestroy', function (e, data) {
            //alert('Destroy');
        });

    });

    ListPDFFiles($("#QM_OBSERVATION_ID").val());
});

function ListPDFFiles(QM_OBSERVATION_ID) {
    blockPage();
    jQuery("#tbPDFFilesList").jqGrid({
        url: '/QualityMonitoring/ListInspPDFFiles',
        datatype: "json",
        mtype: "POST",
        colNames: ["PDF", "Description", "Uploaded By", "Edit", "Delete", "Action"],
        colModel: [
                    { name: 'PDF', index: 'PDF', width: 125, sortable: false, align: "center", formatter: AnchorFormatter, search: false, editable: false },
                    { name: 'Description', index: 'Description', width: 350, sortable: false, align: "center", editable: true, editoptions: { maxlength: 255 }, editrules: { custom: true, custom_func: ValidatePDFDescription } },
                    { name: 'UploadedBy', index: 'UploadedBy', width: 150, sortable: false, align: "center", editable: false },
                    { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center", editable: false },
                    { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center", editable: false },
                    { name: 'Save', index: 'Save', width: 80, sortable: false, align: "center", editable: false, hidden: true }
        ],
        postData: { "QM_OBSERVATION_ID": QM_OBSERVATION_ID },
        pager: jQuery('#dvPDFFilesListPager'),
        rowList: [04, 08, 12],
        rowNum: 04,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Files Details",
        height: 'auto',
        sortname: 'PDF',
        //autowidth: true,
        rownumbers: true,
        editurl: "/QualityMonitoring/UpdateInspPDFDetails",
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
    var url = "/QualityMonitoring/DownloadInspFile/" + cellvalue;
    return "<a href='#' onclick=downloadFileFromAction('" + url + "'); return false;> <img style='height:16px;width:16px' height='16' width='16' border=0 src='../../Content/images/PDF.ico' /> </a>";
}



function DeletePDFFileDetails(QM_OBSERVATION_ID, FILE_NAME) {

    if (confirm("Are you sure to delete the PDF and it's details ? ")) {

        $.ajax({
            url: "/QualityMonitoring/DeleteInspFileDetails/",
            type: "POST",
            cache: false,
            data: {
                QM_OBSERVATION_ID: QM_OBSERVATION_ID, FILE_NAME: FILE_NAME, value: Math.random()
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
                    $("#NumberofFiles").val(parseFloat($("#NumberofFiles").val()) - 1);
                    alert("PDF and it's Details Deleted Succesfully.");
                    $("#tbPDFFilesList").trigger('reloadGrid');
                    $("#rdoPdf").trigger("click");


                    $("#tbMonitorsInspectionList").trigger('reloadGrid');
                    $("#tb2TierSqcInspList").trigger('reloadGrid');
                    $("#tb3TierSqcInspList").trigger('reloadGrid');
                    $("#tb3TierInspectionList").trigger('reloadGrid');
                    $("#tb3TierSqcInspList").trigger('reloadGrid');

                    //$("#ui-accordion-accordionMonitorsInspection-header-0").hide();
                    //$("#divMonitorsInspCorrectionDetails").hide();

                    closeMonitorsInspectionDetails();







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