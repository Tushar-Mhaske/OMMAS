$(function () {
    'use strict';

    // Initialize the jQuery File Upload widget:
    $('#fileupload').fileupload({
        maxNumberOfFiles: 1 ,//- $("#NumberofFiles").val(),
        acceptFileTypes: /(\.|\/)(pdf|PDF)$/i,
        maxFileSize: 5242880,
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

        $("#tbListFundReleaseFiles").trigger('reloadGrid');
        $("#tblPresentation tbody tr").remove();
        $("#divGlobalProgress").html("");
    });
});
$(document).ready(function () {
    
    var stateCode = $("#MAST_STATE_CODE").val();
    var adminCode = $("#ADMIN_NO_CODE").val();
    var fundType = $("#MAST_FUND_TYPE").val();
    var fundingAgency = $("#MAST_FUNDING_AGENCY_CODE").val();
    var yearCode = $("#MAST_YEAR").val();
    var transactionCode = $("#MAST_TRANSACTION_NO").val();
    var releaseType = $("#MAST_RELEASE_TYPE").val();


    ListFiles(stateCode, adminCode, fundType, fundingAgency, yearCode, transactionCode, releaseType);

    if (parseInt($("#NumberofFiles").val()) == 1) {
        $("#btnAddFiles").click(function () {
            $("#divError").html("Only one file is allowed to upload.Please delete previous one to upload another.");
            $("#divError").show('slow');
            return false;
        })
        $("#btnAddFiles").attr('class', 'btn btn-success fileinput-button disabled');
    }

    $("#btnCancel").click(function () {

        CloseProposalDetails();

    });

});
function ListFiles(stateCode, adminCode, fundType, fundingAgency, yearCode, transactionCode,releaseType) {

    blockPage();
    jQuery("#tbListFundReleaseFiles").jqGrid({
        url: '/Fund/ListFundReleaseFiles',
        datatype: "json",
        mtype: "POST",
        colNames: ["File", "Delete"],
        colModel: [
                    { name: 'PDF', index: 'PDF', width: 300, sortable: false, align: "center", formatter: AnchorFormatter, search: false, editable: false },
                    { name: 'Delete', index: 'Delete', width: 100, sortable: false, align: "center", editable: false },
        ],
        postData: { "STATE_CODE": stateCode, "ADMIN_CODE": adminCode, "FUND_TYPE": fundType, "FUNDING_AGENCY": fundingAgency, "YEAR": yearCode, "TRANSACTION": transactionCode,"RELEASE_TYPE": releaseType},
        pager: jQuery('#dvPagerReleaseFiles'),
        rowNum: 5,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Files",
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
    });


}
function AnchorFormatter(cellvalue, options, rowObject) {

    var url = "/Fund/DownloadFundAllocationFile/" + cellvalue;

    if (cellvalue == "") {
        return "<span class='ui-icon ui-icon-locked ui-align-center'></span>";
    }
    else {
        return "<a href='#' onclick=downloadFileFromUrlAction('" + url + "'); return false;> <img style='height:16px;width:16px' height='20' width='20' border=0 src='../../Content/images/PDF.ico' /> </a>";
    }
}
function DeleteFundReleaseFile(PLAN_FILE_NAME) {

    $("#divSuccess").hide("slow");
    if (confirm("Are you sure to delete the File ? ")) {
        $.ajax({
            url: "/Fund/DeleteFundReleaseFile/" + PLAN_FILE_NAME,
            type: "POST",
            cache: false,
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
                    $("#tbListFundReleaseFiles").trigger('reloadGrid');
                    $("#NumberofFiles").val(parseInt($("#NumberofFiles").val()) - 1);
                    alert("File Details Deleted Succesfully.");
                    $("#divAddFundReleaseForm").load('/Fund/FileUploadFundRelease/' + $("#UrlParameter").val(), function (e) {
                        $.validator.unobtrusive.parse($('#divAddFundReleaseForm'));
                        unblockPage();
                    });
                }
                else {
                    alert(response.message);
                }
            }
        });
    }
    else {
        return;
    }
}
function downloadFileFromUrlAction(paramurl) {
    $("#divSuccess").hide("slow");
    window.location = paramurl;
}