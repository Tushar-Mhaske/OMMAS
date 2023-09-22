$(document).ready(function () {

    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    
    $("#frmRequestDocument").on('submit', function (event) {

        if ($('#frmRequestDocument').valid())
        {
            event.stopPropagation(); // Stop stuff happening call double avoid to action
            event.preventDefault(); // call double avoid to action

            var form_data = new FormData();
            
            $.each($("input[type='file']"), function () {

                var id = $(this).attr('id');
                var objFiles = $("input#" + id).prop("files");
                form_data.append(id, (objFiles[0]));
            });

            var data = $("#frmRequestDocument").serializeArray();

            for (var i = 0; i < data.length; i++) {
                form_data.append(data[i].name, data[i].value);
            }

            $.ajax({

                type: 'POST',
                url: '/OnlineFund/UploadDocuments/',
                data: form_data,
                cache: false,
                processData: false, 
                contentType: false,
                success: function (data) {
                    if (data.Success == true) {
                        alert('Documents uploaded successfully');
                        UploadDetails($("#EncryptedRequestId").val());
                    }
                    else if (data.Success == false)
                    {
                        $('#errmessage').html(data.ErrorMessage);
                        $('#dvErrorMessage').show('slow');
                    }
                    else {
                        alert(data.ErrorMessage);
                    }
                },
                error: function () { }

            });
        }

    });

    if ($('#DocumentBefore').val() == "Y" || $('#RoleCode').val() != 2)
    {
        LoadDocumentDetails();
        $('#dvErrorMessage').hide();
    }

    if ($('#RoleCode').val() == 2)
    {
        LoadDocumentDetails();
    }

});
function LoadDocumentDetails()
{
    $("#tblstDocuments").jqGrid('GridUnload');

    jQuery("#tblstDocuments").jqGrid({
        url: '/OnlineFund/GetListofDocumentsUploaded',
        datatype: "json",
        mtype: "POST",
        postData: { EncryptedRequestId: $("#EncryptedRequestId").val() },
        colNames: ['Document Name', 'File Name', 'Upload Date', 'Remarks', 'Download','Delete'],
        colModel: [

                            { name: 'DocumentName', index: 'DocumentName', height: 'auto', width: 300, align: "left", search: false },
                            { name: 'FileName', index: 'FileName', height: 'auto', width: 150, align: "center", search: false },
                            { name: 'UploadDate', index: 'UploadDate', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'Remarks', index: 'Remarks', height: 'auto', width: 300, align: "center", search: false },
                            { name: 'Download', index: 'Download', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'Delete', index: 'Delete', height: 'auto', width: 50, align: "center", search: false },

        ],
        pager: jQuery('#pglstDocuments'),
        rowNum: 10,
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'DocumentName',
        sortorder: "desc",
        caption: 'Document List', 
        height: 'auto',
        hidegrid: true,
        rownumbers: true,
        autowidth: false,
        shrinkToFit: false,
        cmTemplate: { title: false },
        grouping: false,
        loadComplete: function (data) {
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
            }
        }
    });
}
function Download(id)
{
    window.location = '/OnlineFund/DownloadFile/' + id;
}
function DeleteDocument(id)
{
    $.ajax({

        type: 'POST',
        url: '/OnlineFund/DeleteFileDetails?id=' + id,
        cache: false,
        async: false,
        success: function (data) {
            if (data.Success == true) {
                alert('Document deleted successfully.');
                UploadDetails($("#EncryptedRequestId").val());
            }
            else if (data.Success == false) {
                alert(data.ErrorMessage);
            }
        },
        error: function () {
        }
    });
}