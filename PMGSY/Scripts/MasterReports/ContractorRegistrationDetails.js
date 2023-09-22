
$(function () {
    if ($("#MAST_STATE_CODE").val() > 0) {

        $("#StateList_ContractorRegistrationDetails").attr("disabled", "disabled");
    }
    $("#ContractorRegistrationStateButton").click(function () {
        var stateCode = $("#StateList_ContractorRegistrationDetails").val();
        var activeStatus = $("#ActiveStatus_ContractorRegistrationStatus").val();
        var registrationStatus = $("#RegistrationStatus_ContractorRegistrationDetails").val();

        ContractorRegistrationReportsListing(activeStatus, registrationStatus, stateCode);
    });
    $("#ContractorRegistrationStateButton").trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});



function ContractorRegistrationReportsListing(activeStatus, registrationStatus, stateCode) {
    $("#ContractorRegistrationDetailsTable").jqGrid("GridUnload");
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#ContractorRegistrationDetailsTable").jqGrid({
        url: '/MasterReports/ContractorRegistrationDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Contractor/Supplier', 'Company Name', 'Contractor Name', 'PAN', 'Status', 'Registration No', 'Contractor Class Type',
            'Valid From', 'Valid To', 'State Name', 'Registered Office', 'Registration Status'],
        colModel: [
        { name: "MAST_CON_SUP_FLAG", width: 150, align: 'left', height: 'auto' },
        { name: "MAST_CON_COMPANY_NAME", width: 150, align: 'left', height: 'auto' },
        { name: "MAST_CON_NAME", width: 150, align: 'left', height: 'auto' },
        { name: "MAST_CON_PAN", width: 150, align: 'left', height: 'auto' },
        { name: "MAST_CON_STATUS", width: 150, align: 'left', height: 'auto' },
        { name: "MAST_CON_REG_NO", width: 150, align: 'left', height: 'auto' },
        { name: "MAST_CON_CLASS_TYPE_NAME", width: 150, align: 'left', height: 'auto' },
        { name: "MAST_CON_VALID_FROM", width: 150, align: 'left', height: 'auto', sorttype: 'date', formatter: 'date', formatoptions: { srcformat: 'ISO8601Long', newformat: 'd/m/Y' } },
        { name: "MAST_CON_VALID_TO", width: 150, align: 'left', height: 'auto', sorttype: 'date', formatter: 'date', formatoptions: { srcformat: 'ISO8601Long', newformat: 'd/m/Y' } },
        { name: "MAST_STATE_NAME", width: 150, align: 'left', height: 'auto' },
        { name: "MAST_REG_OFFICE", width: 150, align: 'left', height: 'auto' },
        { name: "MAST_REG_STATUS", width: 150, align: 'left', height: 'auto' }
        ],
        postData: { "StateCode": stateCode, "ActiveStatus": activeStatus, "RegistrationStatus": registrationStatus },
        pager: $("#ContractorRegistrationDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '580',
        viewrecords: true,
        caption: 'Contractor Registration Details',
        loadComplete: function () {
            $('#ContractorRegistrationDetailsTable_rn').html('Sr.<br/>No.');

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