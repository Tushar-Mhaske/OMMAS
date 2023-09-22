$(document).ready(function () {
    ListPDFFilesUnfreezeWorkDetails($("#RoadCode").val());

});


function ListPDFFilesUnfreezeWorkDetails(urlparameter) {
    debugger
    //


    blockPage();
    jQuery("#tbPDFFilesList").jqGrid({
        url: '/GPSVTSInstallationDetails/GPSVTSDetails/ListPDFFilesUnfreezeWorkDetails',
        datatype: "json",
        mtype: "POST",
        colNames: ["PDF", "Description", "UPLOAD DATE"],
        colModel: [
            { name: 'PDF', index: 'PDF', width: 125, sortable: false, align: "center", formatter: AnchorFormatter, search: false, editable: false },
            { name: 'Description', index: 'Description', width: 350, sortable: false, align: "center", editable: false },
            { name: 'UPLOAD_DATE', index: 'UPLOAD_DATE', width: 350, sortable: false, align: "center", editable: false },

        ],
        postData: { "parameter": urlparameter },
        pager: jQuery('#dvPDFFilesListPager'),
        rowList: [04, 08, 12],
        rowNum: 04,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Files",
        height: 'auto',
        sortname: 'PDF',
        rownumbers: true,


        loadComplete: function (data) {

        },
        //
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


    unblockPage();

    $('#dvPDFFiles').show();
}