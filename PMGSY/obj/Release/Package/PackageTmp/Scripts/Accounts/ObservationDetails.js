/// <reference path="../jquery-1.9.1-vsdoc.js" />
$(document).ready(function () {

    $('#spCollapseIconCN').click(function () {
        $('#frmObservationDetails').get(0).reset();
        $('#observationDiv').hide('slow');
    });
 
    $.validator.unobtrusive.parse($('#frmObservationDetails'));
    $('#btnSave').click(function () {
        if ($('#hdnMasterObId').val() == "") {
            if ($('#frmObservationDetails').valid() && $('#frmATRDetails').valid())
            {
                SaveObservationDetails();
            }
        }
        else {
                if ($('#frmObservationDetails').valid()) {
                    SaveObservationDetails();
                }
            }
         
    });

   $('#ATRFile').on('change', function () {
        myfile = $(this).val();
        if (myfile == "" || myfile == undefined)
        {
            alert('Please select file.');
            return false;
        }
        var ext = myfile.split('.').pop();
        //  alert("File extension :" + ext.toLowerCase());
        if (ext.toLowerCase() != "pdf") {
            alert("Only pdf file is allowed.");
            $(this).val('');
            return false;
        }
        var fileSizeKb = $(this)[0].files[0].size; // file size in Kb
        var fileSizeMb = fileSizeKb / 1048576;
        if (fileSizeMb > 4) {
            alert("File size should be less than or equal to 4 MB.")
            $(this).val('');
            return false;
        }

    })
   $('#btnUpload').click(function () {
       
       UploadATRFile();
      
   });

   $('#btnReset').click(function () {

       $('#frmObservationDetails').get(0).reset();
       $('#dvErrorMessage').hide();
   });

   $("#obsrvFile").dialog({
       autoOpen: false,
       height: 'auto',
       width: '693',
       modal: true,
       title: 'Observation File List',

   });
});

function SaveObservationDetails() {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: '/Accounts/SaveObservationDetails?state=' + $('#lstStates').val() + '&agency=' + $('#lstAgency').val() + '&year=' + $('#lstYear').val(),
        method: 'POST',
        cache: false,
        beforeSend: function () { },
        data: $('#frmObservationDetails').serialize(), //$('#frmObservationDetails').serialize(),
        dataType: 'json',
        success: function (jsonData, status, xhr) {
            if (!jsonData.success) {
                $('#dvErrorMessage').show('slow');
                $('#message').text(jsonData.message);
            }
            else {
                if ($('#btnAdd').is(":visible")) { //check SRRDA OR NRRDA
                    if ($('#lstStates option:visible').length > 1) {   //For Topic Inition from NRRDA
                        if (confirm(jsonData.message + ' .Do you want to add another observation?')) {
                            $('#frmObservationDetails').get(0).reset();
                        }
                        else {
                            $('#frmObservationDetails').get(0).reset();
                            $('#imgCloseAgreementDetails').trigger('click');//close balance sheet
                            $('#spCollapseIconCN').trigger('click'); // close the form 
                        }
                    }
                    else {
                        //REply From NRRDA
                        alert(jsonData.message);
                        $('#frmObservationDetails').get(0).reset();
                        $('#imgCloseAgreementDetails').trigger('click');//close balance sheet
                        $('#spCollapseIconCN').trigger('click'); // close the form 
                    }
                } else {
                    //for SRRDA
                    alert(jsonData.message);
                    $('#frmObservationDetails').get(0).reset();
                    $('#imgCloseAgreementDetails').trigger('click');//close balance sheet
                    $('#spCollapseIconCN').trigger('click'); // close the form 

                }
            }
            jQuery("#tbAccAtrList").trigger('reloadGrid');
            $.unblockUI();
        },
        error: function (xhr, status, err) {
            alert("Error Occured while procesing your request");
            $.unblockUI();
        }
    });

   
}



function UploadATRFile()
{
    myfile = $("#ATRFile").val();
    if (myfile == "" || myfile == undefined) {
        alert('Please select file.');
        return false;
    }

    var formadata = new FormData();
    var fileUpload = $("#ATRFile").get(0);
    var FileATR = fileUpload.files[0]

    formadata.append("FileATR", FileATR);
    formadata.append("MasterObId", $('#masterObId').val());
    formadata.append("__RequestVerificationToken", $('#frmObservationDetails input[name=__RequestVerificationToken]').val());
 
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        url: '/Accounts/UploadATRFile',
        method: 'POST',
        cache: false,
        beforeSend: function () { },
        data: formadata, //$('#frmObservationDetails').serialize(),
        dataType: 'json',
        processData: false,
        contentType: false,
        success: function (jsonData, status, xhr) {
                // $('#frmObservationDetails').get(0).reset();
                if (!jsonData.success)
                    alert(jsonData.message)
                else
                {
                    if (confirm(jsonData.message + ". Do you want to upload another file"))
                    {
                        $('#frmObservationDetails').get(0).reset();
                    }
                    else
                    {
                        $('#imgCloseAgreementDetails').trigger('click'); // close the form   
                        $('#spCollapseIconCN').trigger('click');
                    }
                }
                    
              //  jQuery("#tbAccAtrFileList").trigger('reloadGrid');

            $.unblockUI();
        },
        error: function (xhr, status, err) {
            alert("Error Occured while procesing your request");
            $.unblockUI();
        }
    });

}


function ViewFileList()
{
   

    if ($('#obsrvFile').is(":hidden"))
    {
        $('#obsrvFile').show();
    }
    var urlparameter = $('#masterObIdforFileList').val();

    $("#tbAccAtrFileList").jqGrid('GridUnload');
    $grid = $("#tbAccAtrFileList");
    $("#tbAccAtrFileList").jqGrid({
        url: '/Accounts/GetATRFileList/',
        mtype: "POST",
        datatype: "json",
        //colNames: ['Observ Id', 'Subject', 'Observation', 'Observation By', 'Observation Date', 'Reply', 'Delete'],
        colModel: [
           // { label: 'ObservID', name: 'ObservID', key: true, width: 75, hidden: true },
            { label: 'State', name: 'State', width: 100, sortable: false, resizable: false, align: 'center' },
           // { label: 'File Name', name: 'FileName', width: 200, sortable: true, resizable: false, align: 'left' },
            { label: 'Uploaded By', name: 'UploadedBy', width: 180, sortable: false, resizable: false, align: 'center' },
            { label: 'Upload Date', name: 'UploadDate', width: 150, sortable: false, resizable: false, align: 'center' },
            { label: 'View', name: 'View', width: 90, sortable: false, resizable: false, align: 'center' },
            { label: 'Delete', name: 'Delete', width: 90, sortable: false, resizable: false, align: 'center' },
            { label: 'OtherDetails', name: 'OtherDetails', width: 90, sortable: false, resizable: false, hidden:true },
        ],
        postData: { id: urlparameter, __RequestVerificationToken: $('#frmATRDetails input[name=__RequestVerificationToken]').val() },
        pager: $('#dvAccAtrFilePager'),
        width: 'auto',
        height: '100%',
        rowNum: 10,
        rowList: [10, 15, 20],
     // hidegrid: true,
        height: 'auto',
        viewrecords: true,
        recordtext: "{2} records found",
        sortorder: "asc",
        caption: "ATR File List",
        sortname: "UploadDate",
        rownumbers: true,
        shrinkToFit: true,
        loadComplete: function (data) {
            /* for close [cross] button on header 
            $('#gview_tbAccAtrFileList .ui-jqgrid-titlebar-close>span').click(function () {
                 $('#obsrvFile').hide();
                
                $('#obsrvFile').hide('slow');
            }); */

            $("#obsrvFile").dialog('open');  //
        },
        onHeaderClick: function () {
            
        }
    });
    //$('#gview_' + $.jgrid.jqID($grid[0].id) + ' .ui-jqgrid-titlebar-close>span')
    //         .removeClass('ui-icon-circle-triangle-n')
    //         .addClass('ui-icon-closethick');
}


function DeleteATRFile(urlparameter)
{
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    if (!confirm('Are you sure to delete file?'))
    {
        $.unblockUI();
        return false;
    }
    $.ajax({
        url: '/Accounts/DeleteATRFIle/' + urlparameter,
        method: 'POST',
        cache: false,
        beforeSend: function () { },
        dataType: 'json',
        data:AddAntiForgeryToken( { __RequestVerificationToken: $('#frmATRDetails input[name=__RequestVerificationToken]').val() }),
        success: function (jsonData, status, xhr) {
            alert(jsonData.message);
            if (jsonData.success)
                jQuery("#tbAccAtrFileList").trigger('reloadGrid');
            $.unblockUI();
        },
        error: function (xhr, status, err) {
            alert("Error Occured while procesing your request");
            $.unblockUI();
        }
    });

}

AddAntiForgeryToken = function (data) {
    data.__RequestVerificationToken = $('#frmATRDetails input[name=__RequestVerificationToken]').val();
    return data;
};