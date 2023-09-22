
$(function () {
    if ($("#MAST_STATE_CODE").val() > 0) {

        $("#StateList_TechnicalAgencyStateMapDetails").attr("disabled", "disabled");
    }
    $("#TechnicalAgencyStateMapDetailsButton").click(function () {
        var stateCode = $("#StateList_TechnicalAgencyStateMapDetails").val();
        var taType = $("#TATypeList_TechnicalAgencyStateMapDetails").val();
        var active = $("#Active_TechnicalAgencyStateMapDetails").val();
        TechnicalAgencyStateMappingReportsListing(stateCode, taType,active);
    });
    $("#TechnicalAgencyStateMapDetailsButton").trigger('click');

    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});



function TechnicalAgencyStateMappingReportsListing(stateCode, taType, active) {
    $("#TechnicalAgencyStateMapDetailsTable").jqGrid("GridUnload");
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#TechnicalAgencyStateMapDetailsTable").jqGrid({
        url: '/MasterReports/TechnicalAgencyStateMappingDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['TA Type', 'Name', 'Designation', 'Contact Person','State','District','Start Date','End Date','Active'],
        colModel: [
            { name: "ADMIN_TA_TYPE", width: 100, align: 'left', height: 'auto' },
            { name: "ADMIN_TA_NAME", width: 300, align: 'left', height: 'auto' },
            { name: "MAST_DESIG_NAME", width: 200, align: 'left', height: 'auto' },
            { name: "ADMIN_TA_CONTACT_NAME", width: 200, align: 'left', height: 'auto' },
            { name: "MAST_STATE_NAME", width: 120, align: 'left', height: 'auto' },
            { name: "MAST_DISTRICT_NAME", width: 120, align: 'left', height: 'auto' },
            { name: "MAST_START_DATE", width: 100, align: 'left', height: 'auto' },
            { name: "MAST_END_DATE", width: 100, align: 'left', height: 'auto' },
            { name: "MAST_IS_ACTIVE", width: 50, align: 'left', height: 'auto' }
        ],
        postData: { "StateCode": stateCode, "TaType": taType, "Active": active },
        pager: $("#TechnicalAgencyStateMapDetailsPager"),
        pgbuttons: true,
        sortname: 'ADMIN_TA_TYPE',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '580',
        viewrecords: true,
        caption: 'Technical Agency Mapping Details',
        loadComplete: function () {
            $('#TechnicalAgencyStateMapDetailsTable_rn').html('Sr.<br/>No.');
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