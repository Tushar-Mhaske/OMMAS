/// <reference path="../jquery-1.9.1-vsdoc.js" />
/// <reference path="../i18n/jquery.jqGrid.src.js" />
$(document).ready(function () {
 

    $("#btnUpload").click(function () {
        saveFile($("#TicketNo").val());
    });

    jQuery("#btnCancel").click(function () {
        $('#accordion').hide('slow');
        $('#dvticketlayout').hide('slow');
        $("#tbTicketList").jqGrid("setGridState", "visible");
        $("#tbAllTicketList").jqGrid("setGridState", "visible");
    });

    DisplayTicketFileList();
});

// changed by rohit borse on 20-07-2022
function DisplayTicketFileList() {
    $("#tbFileList").jqGrid('GridUnload');
    $("#tbFileList").jqGrid({
        url: '/Ticket/GetTicketFileList',
        mtype: "POST",
        datatype: "json",
        colNames: ['File Name', 'Upload Date', 'Uploaded By', 'Download','Delete'],
        colModel: [
            { name: 'FileName', index: "FileName", width: 130, sortable: false, resizable: false, align: 'center' },
            { name: 'UploadDate', index: "UploadDate", width: 130, sortable: false, resizable: false, align: 'center' },
            { name: 'UploadedBy', index: "UploadedBy", width: 130, sortable: false, resizable: false, align: 'center' },
            { name: 'Download', index: "Download", width: 130, sortable: false, resizable: false, align: 'center' },
            { name: 'Delete', index: "Delete", width: 130, sortable: false, resizable: false, align: 'center' },
        ],
        //  loadonce: true,
        postData: { TicketNo:$("#TicketNo").val() },
        pager: "#dvFilePager",
        width: 'auto',
        height: 'auto',
        rowNum: 5,
        rowList: [5, 10, 15],
        viewrecords: true,
        recordtext: "{2} records found",
        sortorder: "asc",
        caption: "&nbsp;&nbsp;File List",
        sortname: "FileName",
        rownumbers: true,
        shrinkToFit: true,
        loadComplete: function (data) {
        }
    });
};


//added by rohit borse on 20-07-2022
function deleteUploadedFile(id) {
    if (confirm("Are you sure you want to delete file ?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: '/Ticket/DeleteTicketFile/' + id,
            datatype: "json",
            type: 'GET',
            success: function (response) {

                if (response) {

                    alert(response.message);
                    DisplayTicketFileList();
                }
                $.unblockUI();
            },
            error: function () {
                alert('Error occured while processing your request');
                DisplayTicketFileList();
                $.unblockUI();
                return false;
            },
        });
    }
}

function saveFile(ticketNo)
{
    var extension = $('#ticketfile').val().split('.').pop().toLowerCase();        
    if ($('#ticketfile').val() == "")
    {
        alert("Please select document before uploading..");
        return false;
    }


    if ($.inArray(extension, ['pdf', 'png', 'jpg', 'jpeg']) == -1) {
        alert('Only pdf, jpg, jpeg, png files are allowed');
        $('#ticketfile').val('');
        return false;
    }
    var FileSize = $('#ticketfile').get(0).files[0].size / 1024 / 1024; // in MB
    if (FileSize > 4) {
        alert('File size should be upto 4 MB');
        $('#ticketfile').val(''); //for clearing with Jquery
        return false;
    }
    var formData = new FormData();
    formData.append("TicketFile", $('#ticketfile').get(0).files[0])
    formData.append("ticketNo", ticketNo)


    var uploadedFileCount = $("#tbFileList").jqGrid('getGridParam', 'records');
    var uploadingConfirmationMsg = "Are you sure you want to upload file ?";
       if (uploadedFileCount > 0)
    {
        uploadingConfirmationMsg = "Are you sure you want to upload more files ? ";
    }

    if (confirm(uploadingConfirmationMsg)) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: '/Ticket/UploadTicketFile',
            type: 'POST',
            cache: false,
            async: false,
            contentType: false,
            processData: false,
            //contentType: "multipart/form-data",
            data: formData,
            success: function (response) {
                alert(response.message);
                if (response.success) {
                    $('#ticketfile').val("");
                    $('#dvViewAgreementMasterBankGuarantee').hide('slow');
                    $('#tbFileList').trigger('reloadGrid');
                }
                $.unblockUI();
            },
            error: function () {

                $('#ticketfile').val("");
                alert('Error occured while processing your request');
                $.unblockUI();
                return false;
            },
        });
    }
    else
    {
        /// if file upload cancel confirm then remove selected file
        $('#ticketfile').val('');
    }
}