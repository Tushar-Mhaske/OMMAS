$(document).ready(function () {
    $(function () {
        'use strict';
        // Initialize the jQuery File Upload widget:
        $('#fileuploadpdf').fileupload({
            maxNumberOfFiles: 5 - parseInt($("#NumberofPdfs").val()),
            acceptFileTypes: /(\.|\/)(pdf|PDF)$/i,
            //maxFileSize: 10000000,
            maxFileSize: 4000000,
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
            url: $('#fileuploadpdf').fileupload('option', 'url'),
            dataType: 'json',
            context: $('#fileuploadpdf')[0]
        }).done(function (result) {
            $(this).fileupload('option', 'done')
                .call(this, null, { result: result });
        });

        // For validation
        $('#fileuploadpdf').bind('fileuploadsubmit', function (e, data) {
            $("#divErrorPdf").hide('slow');
            $("#divSuccesspdf").hide('slow');
            //var inputs = data.context.find(':input');
            //if (inputs.filter('[required][value=""]').first().focus().length) {
            //    alert("Please Enter Description");
            //    return false;
            //}

            //var Remarks = $("#txtRemark").val().trim();
            //if (Remarks.length == 0) {
            //    alert("Please Enter Description");
            //    return false;
            //}

            //if (!Remarks.match("^[a-zA-Z0-9 ,.()-]+$")) {
            //    alert("Please Enter Valid Remarks,Can only contains AlphaNumeric values and [,.()-].");
            //    return false;
            //}

            //data.formData = inputs.serializeArray();

            var flagRemarks = true;

            $("#tblPresentationPdf textarea[name='PdfDescription[]']").each(function () {


                var regExRemarks = /^[a-zA-Z0-9 a-zA-Z0-9 ,.()-]+$/;
                var Remarks = $(this).val().trim();

                if (Remarks == "") {

                    flagRemarks = false;
                    //alert("Please Enter Image Description");
                    $("#divErrorPdf").html("Please Enter Valid Image Description,Can only contains AlphaNumeric values and [,.()-].   value");
                    $("#divErrorPdf").show('slow');

                    $(this).focus();
                    return false;
                }

                if (!regExRemarks.test(Remarks)) {
                    //alert("Please Enter Valid Image Description,Can only contains AlphaNumeric values and [,.()-].   value " + Remarks);

                    $("#divErrorPdf").html("Please Enter Valid Image Description,Can only contains AlphaNumeric values and [,.()-].   value");
                    $("#divErrorPdf").show('slow');

                    flagRemarks = false;
                    return false;
                }

            });

            var inputs = data.context.find(':input');
            var text = data.context.find('textarea');
            data.formData = inputs.serializeArray();


            //if (flagChainage && flagRemarks) {
            if (flagRemarks) {
                return true;
            }
            return false;
        });

        $("#fileuploadpdf").bind("fileuploaddone", function (e, data) {
            $("#divErrorPdf").hide("slow");
            $("#divSuccesspdf").show("slow");
            $("#tbNewsPDFFilesList").trigger('reloadGrid');
            $("#tblPresentation tbody tr").remove();
            $("#divGlobalProgressPdf").html("");
        });

        $("#fileuploadpdf").bind('fileuploadfail', function (e, data) {
            //$("#tblPresentation").find('div.progress').find('div:eq(0)').css('width', '0px');
            //$("#tblPresentation").find('tr.template-upload').css('display', 'none');
            $("#divGlobalProgressPdf").html("");
        });

        $('#fileuploadpdf').bind('fileuploadadd', function (e, data) {
            $("#divSuccesspdf").hide("slow");

        });

        $('#fileuploadpdf').bind('fileuploaddestroy', function (e, data) {
            //alert('Destroy');
        });

    });

    ListFiles($("#News_Id").val());
});

$(document).ready(function () {
    //alert("load");

    ListFiles($("#News_Id").val());
});

// List of Uploaded Files
function ListFiles(News_Id) {
    //alert(News_Id);
    blockPage();
    jQuery("#tbNewsPDFFilesList").jqGrid({
        url: '/NewsDetails/ListFiles',
        datatype: "json",
        mtype: "POST",
        //postData: { News_Id: News_Id },
        colNames: ["PDF", "Description", "Save", "Edit", "Delete", "Action"],
        colModel: [
                    { name: 'PDF', index: 'PDF', width: 125, sortable: false, align: "center", formatter: AnchorFormatter, search: false, editable: false },
                    { name: 'Description', index: 'Description', width: 350, sortable: false, align: "center", editable: true, editoptions: { maxlength: 255 }, editrules: { custom: true, custom_func: ValidatePDFDescription } },
                    { name: 'Save', index: 'Save', width: 80, sortable: false, align: "center", editable: false, hidden: true },
                    { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center", editable: false },
                    { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center", editable: false },
                    { name: 'Action', index: 'Save', width: 80, sortable: false, align: "center", editable: false, hidden: true }
        ],
        postData: { "News_Id": News_Id },
        pager: jQuery('#dvNewsPDFFilesListPager'),
        rowNum: 5,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "PDF Files",
        height: 'auto',
        sortname:'PDF',
        rownumbers: true,
        editurl: "/NewsDetails/UpdatePDFDetails",
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
    //alert(paramFileID);

    jQuery("#tbNewsPDFFilesList").editRow(paramFileID);
    $('#tbNewsPDFFilesList').jqGrid('showCol', 'Save');
}

function SavePDFDetails(paramFileID) {
    //alert("Save");
    jQuery("#tbNewsPDFFilesList").saveRow(paramFileID, checksave);
}

function CancelSavePDFDetails(paramFileID) {
    //alert("Cancel Save");
    $('#tbNewsPDFFilesList').jqGrid('hideCol', 'Save');
    jQuery("#tbNewsPDFFilesList").restoreRow(paramFileID);
}

function checksave(result) {
    $('#tbNewsPDFFilesList').jqGrid('hideCol', 'Save');
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

function DownloadFile(paramFileName) {
    //alert("Download");
    $.ajax({
        url: '/NewsDetails/DownloadFile',
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
    window.location = paramurl;
}

function AnchorFormatter(cellvalue, options, rowObject) {

    var url = "/NewsDetails/DownloadFile/" + cellvalue;

    return "<a href='#' title='Click here to Download the File' onclick=downloadFileFromAction('" + url + "'); return false;> <img style='height:16px;width:16px' height='20' width='20' border=0 src='../../Content/images/pdf.png' /> </a>";
}

function DeleteFileDetails(param, NEWS_FILE_NAME) {
    //alert(param + "," + NEWS_FILE_NAME);
    if (confirm("Are you sure to Delete the File and File Details ? ")) {
        $.ajax({
            url: "/NewsDetails/DeleteFileDetails/",
            type: "POST",
            cache: false,
            data: {
                News_ID: param, ISPF_TYPE: "P", NEWS_FILE_NAME: NEWS_FILE_NAME //PLAN_CN_ROAD_CODE: PLAN_CN_ROAD_CODE, PLAN_FILE_NAME: PLAN_FILE_NAME
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
                    $("#tbNewsPDFFilesList").trigger('reloadGrid');
                    $("#NumberOfFiles").val(parseFloat($("#NumberOfFiles").val()) - 1);
                    alert("File Details Deleted Succesfully.");
                    $("#rdbtnFile").trigger("click");
                    //$("#divAddForm").load('/NewDetails/NewsFileUpload/' + $("#News_Id").val(), function () {
                    //    $.validator.unobtrusive.parse($('#fileupload'));
                    //    unblockPage();
                    //});
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