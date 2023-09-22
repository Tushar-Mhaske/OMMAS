
$(function () {

    if ($("#MAST_STATE_CODE").val() > 0) {

        $("#StateList_ContractorClassTypeDetails").attr("disabled", "disabled");
    }
    $("#ContractorClassTypeStateButton").click(function () {
        var stateCode = $("#StateList_ContractorClassTypeDetails").val();

        ContractorClassTypeReportsListing(stateCode);
    });
    $("#ContractorClassTypeStateButton").trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});



function ContractorClassTypeReportsListing(stateCode) {
    $("#ContractorClassTypeDetailsTable").jqGrid("GridUnload");
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#ContractorClassTypeDetailsTable").jqGrid({
        url: '/MasterReports/ContractorClassTypeDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name','Contractor Class Type'],
        colModel: [
            { name: "MAST_STATE_NAME", width: 150, align: 'left', height: 'auto' },
            { name: "MAST_CON_CLASS_TYPE_NAME", width: 150, align: 'left', height: 'auto' }
        ],
        postData: { "StateCode": stateCode },
        pager: $("#ContractorClassTypeDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '580',
        viewrecords: true,
        caption: 'Contractor Type Details Details',
        loadComplete: function () {
            $('#ContractorClassTypeDetailsTable_rn').html('Sr.<br/>No.');

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