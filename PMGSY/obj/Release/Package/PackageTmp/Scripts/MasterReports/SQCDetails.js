
$(function () {

    if ($("#MAST_STATE_CODE").val() > 0) {

        $("#StateList_SQCDetails").attr("disabled", "disabled");
    }
    $("#SQCDetailsButton").click(function () {
        var stateCode = $("#StateList_SQCDetails").val();
        var activeStatus = $("#ActiveList_SQCDetails").val();
        SQCReportsListing(activeStatus, stateCode);
    });
    $("#SQCDetailsButton").trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');

   
});



function SQCReportsListing(activeStatus, stateCode) {
    $("#SQCDetailsTable").jqGrid("GridUnload");
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#SQCDetailsTable").jqGrid({
        url: '/MasterReports/SQCDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name ','QC Name ','Designation','Address','Contact ','Active Status','Active End Date'],
        colModel:[
            {name: "MAST_STATE_NAME ", width: 150, align: 'left', height: 'auto'},
            {name: "ADMIN_QC_NAME ", width: 150, align: 'left', height: 'auto'},
            {name: "MAST_DESIG_NAME", width: 150, align: 'left', height: 'auto'},
            {name: "ADMIN_QC_ADDRESS", width: 150, align: 'left', height: 'auto'}, 
            { name: "ADMIN_QC_CONTACT", width: 150, align: 'left', height: 'auto' },
            {name: "ADMIN_ACTIVE_STATUS", width: 150, align: 'left', height: 'auto'},
            {name: "ADMIN_ACTIVE_ENDDATE", width: 150, align: 'left', height: 'auto'},
        ],
        postData: {"StateCode": stateCode,"ActiveStatus": activeStatus},
        pager: $("#SQCDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '580',
        viewrecords: true,
        caption: 'SQC Details',
        loadComplete: function () {
            $('#SQCDetailsTable_rn').html('Sr.<br/>No.');
            $.unblockUI();
        },
        loadError: function (xhr, status, error) {
            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
            $.unblockUI();
        }
    });
}