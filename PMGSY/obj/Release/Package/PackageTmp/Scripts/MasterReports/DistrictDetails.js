$(function () {
  
    if ($("#MAST_STATE_CODE").val() > 0) {

        $("#StateList_DistrictDetails").attr("disabled", "disabled");
    }
    $("#DistrictDetailsButton").click(function () {
        var stateCode = $("#StateList_DistrictDetails").val();
        var iapDistrict = $("#IAP_DISTRICT_DistrictDetails").val();
        var pmgsyIncluded = $("#PMGSY_INCLUDED_DistrictDetails").val();
        var activeType = $("#ActiveType_DistrictDetails").val();

        DistrictMasterReportsListing(stateCode, iapDistrict, pmgsyIncluded,activeType);
    });
    $("#DistrictDetailsButton").trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});



function DistrictMasterReportsListing(stateCode, iapDistrict, pmgsyIncluded, activeType) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#DistrictDetailsTable").jqGrid('GridUnload');

    $("#DistrictDetailsTable").jqGrid({
        url: '/MasterReports/DistrictDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name','District Name', 'PMGSY Included', 'IAP District','Active'],
        colModel: [
            { name: 'MAST_STATE_NAME', width: 300, align: 'left', height: 'auto' },
            { name: 'MAST_DISTRICT_NAME', width: 300, align: 'left', height: 'auto' },
            { name: 'MAST_PMGSY_INCLUDED', width: 100, align: 'left', height: 'auto' },
            { name: 'MAST_IAP_DISTRICT', width: 100, align: 'left', height: 'auto' },
            { name: 'MAST_DISTRICT_ACTIVE', width: 100, align: 'left', height: 'auto' }

        ],
        postData: { "StateCode": stateCode, "IAP_DISTRICT": iapDistrict, "PMGSY_INCLUDED": pmgsyIncluded, "ActiveType": activeType },
        pager: $("#DistrictDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '100%',
        viewrecords: true,
        caption: 'District Details',
        loadComplete: function () {
            $('#DistrictDetailsTable_rn').html('Sr.<br/>No.');

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