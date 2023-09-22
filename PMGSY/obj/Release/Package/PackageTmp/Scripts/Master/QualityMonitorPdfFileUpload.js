$(function () {
    'use strict';
    
    //alert("hiuh"+ $("#NumberofFiles").val());
    // Initialize the jQuery File Upload widget:
    $('#fileupload').fileupload({
        maxNumberOfFiles: 1 - $("#NumberofFiles").val(),
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
            return false;
        }

        data.formData = inputs.serializeArray();       
    });

    $("#fileupload").bind("fileuploaddone", function (e, data) {
        $("#divSuccess").show("slow");
        $("#tbPDFFilesList").trigger('reloadGrid');
        $("#tblPresentation tbody tr").remove();
        $("#divGlobalProgress").html("");
    });

    $("#fileupload").bind('fileuploadfail', function (e, data) {
        //$("#tblPresentation").find('div.progress').find('div:eq(0)').css('width', '0px');
        //$("#tblPresentation").find('tr.template-upload').css('display', 'none');
        //$("#divGlobalProgress").html("");
    });


});

$(document).ready(function () {
    

    
    ListPDFFiles($("#ADMIN_QualityMonitor_CODE").val());
});

function ListPDFFiles(ADMIN_QualityMonitor_CODE) {
    

    blockPage();
    jQuery("#tbPDFFilesList").jqGrid({
        url: '/Master/ListPDFFiles',
        datatype: "json",
        mtype: "POST",
        colNames: ["PDF","Delete"],
        colModel: [
                    { name: 'PDF', index: 'PDF', width: 250, sortable: false, align: "center", formatter: AnchorFormatter, search: false, editable: false },
                    { name: 'Delete', index: 'Delete', width: 130, sortable: false, align: "center", editable: false },
        ],
        postData: { "ADMIN_QM_CODE": ADMIN_QualityMonitor_CODE },
        pager: jQuery('#dvPDFFilesListPager'),
        rowNum: 4,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Quality Monitor PDF File",
        height: 'auto',
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

function downloadFileFromAction(paramurl){
    window.location = paramurl;
}

function AnchorFormatter(cellvalue, options, rowObject) {    
    var url = "/Master/DownloadFile/" + cellvalue;    
    return "<a href='#' title='Click here to download file' onclick=downloadFileFromAction('" + url + "'); return false;> <img style='height:16px;width:16px' height='16' width='16' border=0 src='../../Content/images/PDF.ico' /> </a>";
}

function DeletePDFFileDetails(ADMIN_QM_CODE, FILE_NAME) {

    if (confirm("Are you sure to delete the Quality Monitor PDF? ")) {

        $.ajax({
            url: "/Master/DeleteFileDetails/",
            type: "POST",
            cache: false,
            data: {
                ADMIN_QM_CODE: ADMIN_QM_CODE, FILE_NAME: FILE_NAME, FILE_TYPE: 'D', value: Math.random()
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
                    $("#tbFilesList").trigger('reloadGrid');
                    $("#NumberofFiles").val(parseFloat($("#NumberofFiles").val()) - 1);
                    alert("Quality Monitor PDF Deleted Succesfully.");
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