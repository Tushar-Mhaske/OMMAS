
$(document).ready(function () {
    
    LoadClearanceFileLetterList();

});

function LoadClearanceFileLetterList() {
   
    // $("#tblFileDownloadClearanceDetail").jqGrid('GridUnload');
    jQuery("#tblFileDownloadClearanceDetail").jqGrid({
        url: '/Proposal/GetMrdClearenceFileList/',
        datatype: "json",
        mtype: "POST",
        colNames: ['Clearance Letter (pdf)', 'Road List (pdf) ', 'Road List (excel)'],
        colModel: [

                    { name: 'MRD_CLEARANCE_PDF_FILE', index: 'MRD_CLEARANCE_PDF_FILE', height: 'auto', width: 60, align: "center", sortable: false },
                    { name: 'MRD_ROAD_PDF_FILE', index: 'MRD_ROAD_PDF_FILE', height: 'auto', width: 60, align: "center", sortable: false },
                    { name: 'MRD_ROAD_EXCEL_FILE', index: 'MRD_ROAD_EXCEL_FILE', height: 'auto', width: 60, align: "center", sortable: false },
          ],
        postData: { "ClearanceCodeEncrypted": $('#ClearanceCodeEncrypted').val() },
        pager: jQuery('#divPagerFileDownloadCleranceDetail'),
        rowNum: 1,
        rowList: [],
        viewrecords: false,
        recordtext: '{2} records found',
        sortname: 'MAST_STATE_NAME',
        sortorder: "asc",
        caption: "Clearance File List",
        height: 'auto',
        autowidth: true,
        pgbuttons: false,
        pgtext: null,
        //width:'250',
        shrinkToFit: false,
        rownumbers: true,
        loadComplete: function () {
            $("#tblFileDownloadClearanceDetail").jqGrid('setGridWidth', $("#MrdClearenceLetterSearchDetails").width()-200, true);
           
            $.unblockUI();


        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")

            }
        },




    });
}

function DownLoadFile(paramurl) {

    url = "/Proposal/DownloadMrdClearenceLetter/" + paramurl,

    //window.location = url;


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

    //window.location = paramurl;
}

function DeleteFile(urlparameter) {
    if (confirm("Are you sure you want to delete Clearance file details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'POST',
            url: '/Proposal/EditDeleteMrdClearenceFile/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    $('#tblFileDownloadClearanceDetail').trigger('reloadGrid');
                    $.unblockUI();
                }
                else {

                    alert(data.message);
                    $.unblockUI();
                }

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }
        });



    }
    else {
        return false;
    }

}

