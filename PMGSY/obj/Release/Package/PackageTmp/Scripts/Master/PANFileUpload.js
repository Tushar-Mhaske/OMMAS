﻿

$(document).ready(function () {
    $(function () {
        'use strict';
        // Initialize the jQuery File Upload widget:
        $('#fileupload').fileupload({
            maxNumberOfFiles: 1 - parseInt($("#NumberofFiles").val()),
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

            $("#tbPANFileList").trigger('reloadGrid');
            $("#tblQualityMonitorListDetails").trigger('reloadGrid');

            $("#tblPresentation tbody tr").remove();
            $("#divGlobalProgress").html("");
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

    ListPDFFiles($("#hdnQMCode").val());
});

function ListPDFFiles(ADMIN_QM_CODE) {
    blockPage();
    jQuery("#tbPANFileList").jqGrid({
        url: '/Master/ListPANFiles',
        datatype: "json",
        mtype: "POST",
        colNames: ["PDF", "Delete"],
        colModel: [
                    { name: 'PDF', index: 'PDF', width: 225, sortable: false, align: "center", formatter: AnchorFormatter, search: false, editable: false },
                    { name: 'Delete', index: 'Delete', width: 225, sortable: false, align: "center", search: false, editable: false }

        ],
        postData: { "ADMIN_QM_CODE": ADMIN_QM_CODE },
        pager: jQuery('#dvPANFileListPager'),
        rowList: [04, 08, 12],
        rowNum: 04,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Files",
        height: 'auto',
        sortname: 'PDF',
        //autowidth: true,
        rownumbers: true,
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
    var url = "/Master/DownloadPANFile/" + cellvalue;
    return "<a href='#' onclick=downloadFileFromAction('" + url + "'); return false;> <img style='height:16px;width:16px' height='16' width='16' border=0 src='../../Content/images/PDF.ico' /> </a>";
}

function DeletePANFileDetails(ADMIN_QM_CODE, FILE_NAME) {

    if (confirm("Are you sure to delete the PAN File and it's details ? ")) {

        $.ajax({
            url: "/Master/DeletePANFileDetails/",
            type: "POST",
            cache: false,
            data: {
                ADMIN_QM_CODE: ADMIN_QM_CODE, FILE_NAME: FILE_NAME, value: Math.random()
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
                    alert("PAN and it's Details Deleted Succesfully.");
                    $("#tbPANFileList").trigger('reloadGrid');
                    $("#tblQualityMonitorListDetails").trigger('reloadGrid');
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