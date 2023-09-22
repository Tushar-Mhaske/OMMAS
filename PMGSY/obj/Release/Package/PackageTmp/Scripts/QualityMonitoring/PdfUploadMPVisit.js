

$(document).ready(function () {
    $(function () {
        'use strict';
        // Initialize the jQuery File Upload widget:
        $('#fileupload').fileupload({
            maxNumberOfFiles: $('#Role').val() == 22 ? (1 - parseInt($("#NumberofPdfs").val())) : 10,
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

            data.formData = inputs.serializeArray();
        });

        $("#fileupload").bind("fileuploaddone", function (e, data) {
            $("#divSuccess").show("slow");
            $("#tbPDFFilesList").trigger('reloadGrid');
            //$("#tbSTASRRDAPDFFilesList").trigger('reloadGrid');
            $("#tblPresentation tbody tr").remove();
            $("#divGlobalProgress").html("");


            //alert($("#NumberofPdfs").val());
            $("#NumberofPdfs").val(parseFloat($("#NumberofPdfs").val()) + 1);
            if ($("#NumberofPdfs").val() >= 1) {
                $("#fileupload").hide();
            }
            else {
                $("#fileupload").show();

            }
        });

        $("#fileupload").bind('fileuploadfail', function (e, data) {
            //$("#tblPresentation").find('div.progress').find('div:eq(0)').css('width', '0px');
            //$("#tblPresentation").find('tr.template-upload').css('display', 'none');
            $("#divGlobalProgress").html("");
        });

        $('#fileupload').bind('fileuploadadd', function (e, data) {
            $("#divSuccess").hide("slow");

           // $("#NumberofPdfs").val(parseFloat($("#NumberofPdfs").val()) + 1);



        });

        $('#fileupload').bind('fileuploaddestroy', function (e, data) {
            //alert('Destroy');
        });

    });

    ListPDFFilesMPVisit($("#MP_VISIT_ID").val());
    //ListSTASRRDAPDFFiles($("#IMS_PR_ROAD_CODE").val());
});

function ListPDFFilesMPVisit(MP_VISIT_ID) {
    blockPage();
    jQuery("#tbPDFFilesList").jqGrid({
        url: '/QualityMonitoring/ListPDFFileMPVisit',
        datatype: "json",
        mtype: "POST",
        colNames: ["PDF", "Delete"],
        colModel: [
                    { name: 'PDF', index: 'PDF', width: 250, sortable: false, align: "center", formatter: AnchorFormatter, search: false, editable: false },
                    //{ name: 'Description', index: 'Description', width: 350, sortable: false, align: "center", editable: true, editoptions: { maxlength: 255 }, editrules: { custom: true, custom_func: ValidatePDFDescription } },
                   // { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center", editable: false },
                    { name: 'Delete', index: 'Delete', width: 170, sortable: false, align: "center", editable: false },
                   // { name: 'Save', index: 'Save', width: 80, sortable: false, align: "center", editable: false, hidden: true }
        ],
        postData: { "MP_VISIT_ID": MP_VISIT_ID },
        pager: jQuery('#dvPDFFilesListPager'),
        rowList: [04, 08, 12],
        rowNum: 04,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Files",
        height: 'auto',
        sortname: 'PDF',
        //autowidth: true,
        rownumbers: true,
       // editurl: "/Proposal/UpdatePDFDetailsMpVisit",
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


function downloadFileFromAction(paramurl) {
    window.location = paramurl;
}

function AnchorFormatter(cellvalue, options, rowObject) {
    var url = "/QualityMonitoring/DownloadMPVisitFiles/" + cellvalue;
    return "<a href='#' onclick=downloadFileFromAction('" + url + "'); return false;> <img style='height:16px;width:16px' height='16' width='16' border=0 src='../../Content/images/PDF.ico' /> </a>";
}

function DeletePDFFileDetails(FILE_ID_MP_VISIT_ID, FILE_NAME) {

    if (confirm("Are you sure to delete the PDF and it's details ? ")) {

        $.ajax({
            url: "/QualityMonitoring/DeleteMPVisitFilesDetails/",
            type: "POST",
            cache: false,
            data: {
                FILE_ID_MP_VISIT_ID: FILE_ID_MP_VISIT_ID, FILE_NAME: FILE_NAME, IS_PDF: 'Y', value: Math.random()
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
                    $("#NumberofPdfs").val(parseFloat($("#NumberofPdfs").val()) - 1);
                    alert("PDF and it's Details Deleted Succesfully.");
                    $("#rdoPdf").trigger("click");
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