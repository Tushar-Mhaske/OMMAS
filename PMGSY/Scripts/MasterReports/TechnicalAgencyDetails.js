
$(function () {
    if ($("#MAST_STATE_CODE").val() > 0) {

        $("#StateList_TechnicalAgencyDetails").attr("disabled", "disabled");
    }
    $("#TechnicalAgencyDetailsButton").click(function () {
        var stateCode = $("#StateList_TechnicalAgencyDetails").val();
        var taType = $("#TATypeList_TechnicalAgencyDetails").val();

        TechnicalAgencyReportsListing(stateCode, taType);
    });
    $("#TechnicalAgencyDetailsButton").trigger('click');

    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});



function TechnicalAgencyReportsListing(stateCode, taType) {
    $("#TechnicalAgencyDetailsTable").jqGrid("GridUnload");
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    
    $("#TechnicalAgencyDetailsTable").jqGrid({
        url: '/MasterReports/TechnicalAgencyDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['TA Type','Name','Address','Contact','Designation','Contact Person'],
        colModel:[
            {name: "ADMIN_TA_TYPE", width: 150, align: 'left', height: 'auto'},
            {name: "ADMIN_TA_NAME", width: 150, align: 'left', height: 'auto'},
            {name: "ADMIN_TA_ADDRESS", width: 150, align: 'left', height: 'auto'},
            { name: "ADMIN_TA_CONTACT", width: 150, align: 'left', height: 'auto' },
            { name: "MAST_DESIG_NAME", width: 150, align: 'left', height: 'auto' },
            {name: "ADMIN_TA_CONTACT_NAME", width: 150, align: 'left', height: 'auto'}           
        ],
        postData: { "StateCode": stateCode, "TaType": taType },
        pager: $("#TechnicalAgencyDetailsPager"),
        pgbuttons: true,
        sortname: 'ADMIN_TA_TYPE',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '580',
        viewrecords: true,
        caption: 'Technical Agency Details',
        loadComplete: function () {
            $('#TechnicalAgencyDetailsTable_rn').html('Sr.<br/>No.');
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