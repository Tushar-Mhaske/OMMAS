/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   ATRUpload.js
        * Description   :   Upload ATR Details
        * Author        :   Shyam Yadav 
        * Creation Date :   10/Jun/2013
 **/


$(document).ready(function () {

    $('#ATRFile').on('change', missingFileUpload);

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
            alert(result.success);
            $(this).fileupload('option', 'done')
                .call(this, null, { result: result });
        });

        // For validation
        $('#fileupload').bind('fileuploadsubmit', function (e, data) {
            //var inputs = data.context.find(':input');
            //if (inputs.filter('[required][value=""]').first().focus().length) {
            //    alert("Please Enter Description");
            //    return false;
            //}

            //var Remarks = $("#txtRemark").val();

            //if (!Remarks.match("^[a-zA-Z0-9  ,.()-]+$")) {
            //    alert("Please Enter Valid Remarks,Can only contains AlphaNumeric values and [,.()-].");
            //    return false;
            //}

            //data.formData = inputs.serializeArray();

            //e.preventDefault();

            //var form_data = new FormData();

            //var objMissingImageFile = $("#ATRFile").prop("files");
            //form_data.append("file", objMissingImageFile[0]);
            //console.log("file size :"+objMissingImageFile[0].size);

            //form_data.append("qmATRFileId", $("#qmATRFileId").val());
            //form_data.append("qmATRObsId", $("#qmATRObsId").val());
            
            
            //form_data.append("__RequestVerificationToken", $("#fileupload input[name=__RequestVerificationToken]").val());

            //var data = $("#fileupload").serialize();

            //$.ajax({
            //    processData: false,
            //    contentType: false,
            //    type: "POST",
            //    url: "/QualityMonitoring/UploadARMissingPDFFile/",
            //    data: form_data,//$("#fileupload").serialize(), // serializes the form's elements.
            //    //context: form_data,//$('#fileupload')[0],
            //    success: function (data) {
            //        alert(data.success); // show response from the php script.
            //    }
            //});

            
            
        });

        $("#fileupload").bind("fileuploaddone", function (e, data) {
            e.preventDefault(); //stop default behaviour!
            //console.log(data);
            //alert(data.jqXHR.responseText.success);
            
            //alert(e.success);

            if (JSON.parse(data.jqXHR.responseText).success == true) {
                $("#divSuccess").show("slow");
                $("#tbPDFFilesList").trigger('reloadGrid');
                //ListPDFFiles($("#qmATRObsId").val());
                $("#tb3TierATRList").trigger('reloadGrid');
                viewATRDetails();
                $("#tblPresentation tbody tr").remove();
                $("#divGlobalProgress").html("");
            }
            else {
                alert(JSON.parse(data.jqXHR.responseText).message);
            }
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
    //alert($("#qmATRObsId").val());
    ListPDFFiles($("#qmATRObsId").val());
});

function ListPDFFiles(QM_OBSERVATION_ID) {
    blockPage();
    jQuery("#tbPDFFilesList").jqGrid({
        url: '/QualityMonitoring/ListPDFFiles',
        datatype: "json",
        mtype: "POST",
        colNames: ["ATR File"],
        colModel: [
                    { name: 'PDF', index: 'PDF', width: 400, sortable: false, align: "center", formatter: AnchorFormatter, search: false, editable: false }
        ],
        postData: { "QM_OBSERVATION_ID": QM_OBSERVATION_ID },
        pager: jQuery('#dvPDFFilesListPager'),
        rowList: [04, 08, 12],
        rowNum: 04,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Files",
        height: 'auto',
        //autowidth: true,
        sortname: 'PDF',
        rownumbers: true,
        editurl: "/Proposal/UpdatePDFDetails",
        loadComplete: function () {
            //$("#gview_tbPDFFilesList > .ui-jqgrid-titlebar").hide();
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
    if ((/^[a-zA-Z0-9 ,.()-]+$/).test(value)) {
        return [" Invalid Description,Only Alphabets and Numbers are allowed."];
    }
    else {
        return [true, ""];
    }
}

function downloadFileFromAction(paramurl) {
    window.location = paramurl;
}

function AnchorFormatter(cellvalue, options, rowObject) {
    var url = "/QualityMonitoring/DownloadFile/" + cellvalue;
    return cellvalue == "" ? "-" : "<a href='#' onclick=downloadFileFromAction('" + url + "'); return false;> <img style='height:16px;width:16px' height='16' width='16' border=0 src='../../Content/images/PDF.ico' /> </a>";
}

function missingFileUpload(event) {

    ATRFile = event.target.files;

    if (ATRFile != null) {

        //$('#lblMissingImage').text($('#MissingImageFile').val());
        //$('#imgMissingImage').show('slow');
        //$('#MissingImageFile').hide('slow');
    }
}