
$(document).ready(function () {

    if ($("#MAST_STATE_CODE").val() > 0) {

        $("#StateList_QualityMonitorDetails").attr("disabled", "disabled");
    }
    $("#QualityMonitorDetailsButton").click(function () {
        var stateCode = $("#StateList_QualityMonitorDetails").val();
        var qmType = $("#QMTypeList_QualityMonitorDetails").val();
        var activeType = $("#ActiveList_QualityMonitorDetails").val();

        QualityMonitorReportsListing(stateCode, qmType, activeType);
    });
    $("#QualityMonitorDetailsButton").trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});



function QualityMonitorReportsListing(stateCode, qmType, activeType) {
    $("#QualityMonitorDetailsTable").jqGrid("GridUnload");
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#QualityMonitorDetailsTable").jqGrid({
        url: '/MasterReports/QualityMonitorDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State','Monitor Type', 'QM Name', 'Designation', 'Address', 'Contact', 'Pan', 'Empanelled', 'Empanelled Year'],
        colModel:[
            { name: "MAST_STATE_NAME", width: 150, align: 'left', height: 'auto' },
             { name: "ADMIN_QM_TYPE", width: 150, align: 'left', height: 'auto' },
            { name: "ADMIN_QM_NAME", width: 150, align: 'left', height: 'auto' },
            { name: "MAST_DESIG_NAME", width: 150, align: 'left', height: 'auto' },
            { name: "ADMIN_QM_ADDRESS", width: 150, align: 'left', height: 'auto' },
            { name: "ADMIN_QM_CONTACT", width: 150, align: 'left', height: 'auto' },
            { name: "ADMIN_QM_PAN", width: 150, align: 'left', height: 'auto' },
            { name: "ADMIN_QM_EMPANELLED", width: 150, align: 'left', height: 'auto' },
            { name: "ADMIN_QM_EMPANELLED_YEAR", width: 150, align: 'left', height: 'auto' }
        ],
        postData: { "StateCode": stateCode, "QmType": qmType, "ActiveType": activeType },
        pager: $("#QualityMonitorDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '580',
        viewrecords: true,
        caption: 'Quality Monitor Details',
        loadComplete: function () {
            $('#QualityMonitorDetailsTable_rn').html('Sr.<br/>No.');
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