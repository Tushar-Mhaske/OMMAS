/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   ATRUpload.js
        * Description   :   Upload ATR Details
        * Author        :   Shyam Yadav 
        * Creation Date :   10/Jun/2013
 **/


$(document).ready(function () {   

    $(function () {
        'use strict';
        // Initialize the jQuery File Upload widget:
        $('#fileupload').fileupload({
            // maxNumberOfFiles: 1 - parseInt($("#NumberofFiles").val()),
            maxNumberOfFiles:100,
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

        $("#fileupload").bind('fileuploadfail', function (e, data) {
            //$("#tblPresentation").find('div.progress').find('div:eq(0)').css('width', '0px');
            //$("#tblPresentation").find('tr.template-upload').css('display', 'none');
            // $("#divGlobalProgress").html("");
        });

        $("#fileupload").bind("fileuploaddone", function (e, data) {
            $("#divSuccess").show("slow");
           // $("#tbPDFFilesList").trigger('reloadGrid');          
            ListPDFFiles($("#PUBLICATION_CODE").val());
            $("#tblPresentation tbody tr").remove();
            $("#divGlobalProgress").html("");
        });

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
        });

     

      
        $('#fileupload').bind('fileuploadadd', function (e, data) {
            $("#divSuccess").hide("slow");

        });

        $('#fileupload').bind('fileuploaddestroy', function (e, data) {
            //alert('Destroy');
        });

    });  
    setTimeout(function () {
        ListPDFFiles($("#PUBLICATION_CODE").val());
    }, 1000);
    //if ($('#publicationFinalized').val() == "Y") {
       
    //    $('input[type=file]').attr('disabled', true);
    //}
});

function ListPDFFiles(publicationCode) {

    $("#tbPDFFilesList").jqGrid('GridUnload');
    blockPage();
    jQuery("#tbPDFFilesList").jqGrid({
        url: '/Publication/ListPublicationFile',
        datatype: "json",
        mtype: "POST",
        colNames: ["Publication File","Upload Date"],
        colModel: [
                    { name: 'PUBLICATION_FILE_CODE', index: 'PUBLICATION_FILE_CODE', width: 400, sortable: false, align: "center", formatter: AnchorFormatter, search: false, editable: false },
                    { name: 'PUBLICATION_UPLOAD_DATE', index: 'PUBLICATION_UPLOAD_DATE', width: 400, sortable: false, align: "center" }
        ],
        postData: { "id": publicationCode },
        pager: jQuery('#dvPDFFilesListPager'),
        rowList: [04, 08, 12],
        rowNum: 04,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Files",
        height: 'auto',
        autowidth: true,
        sortname: 'PDF',
        rownumbers: true,
        //editurl: "",
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

//function downloadFileFromAction(paramurl) {
    
//    window.location = paramurl;
//}
function downloadFileFromAction(paramurl) {
    var url = paramurl;
    $.ajax({
        url: url,
        aysnc: false,
        catche: false,
        error: function (xhr, status, msg) {
            alert("An Error occured while processing your request.");
            return false;
        },
        success: function (responce) {

            if (responce.Success == "false") {
                alert("File not available.");
                return false;
            }
            else {
                window.location = url;
            }
        }
    });

}

function AnchorFormatter(cellvalue, options, rowObject) {
  
    var url = "/Publication/DownloadFile/" + cellvalue;
    return "<a href='#' onclick=downloadFileFromAction('" + url + "'); return false;> <img style='height:16px;width:16px' title='Download' height='16' width='16' border=0 src='../../Content/images/PDF.ico' /> </a>";
}

