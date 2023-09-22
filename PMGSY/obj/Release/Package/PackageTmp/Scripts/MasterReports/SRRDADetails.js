
$(document).ready(function () {

    if ($("#MAST_STATE_CODE").val() > 0) {

        $("#StateList_SRRDADetails").attr("disabled", "disabled");
    }
    $("#SRRDADetailsButton").click(function () {
        var stateCode = $("#StateList_SRRDADetails").val();
        var agencyCode = $("#AgencyList_SRRDADetails").val();
        var officeType = $("#OfficeList_SRRDADetails").val();

        SRRDAReportsListing(stateCode, agencyCode, officeType);
    });

    $("#StateList_SRRDADetails").change(function () {

        $("#AgencyList_SRRDADetails").val(0);
        $("#AgencyList_SRRDADetails").empty();

        $("#AgencyList_SRRDADetails").append("<option value='0'>All Agency</option>");

        if ($(this).val() > 0) {
            if ($("#AgencyList_SRRDADetails").length > 0) {
                $.ajax({
                    url: '/MasterReports/SRRDADetails',
                    type: 'POST',
                    data: { "StateCode": $(this).val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#AgencyList_SRRDADetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }
        }
        else {
            //  $("#AgencyList_SRRDADetails").empty();
            $("#AgencyList_SRRDADetails").append("<option value='0'>All Agency</option>");



        }
    });

    $("#StateList_SRRDADetails").trigger('change');
    $("#SRRDADetailsButton").trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');

  
});







function SRRDAReportsListing(stateCode,agencyCode,officeType) {
    $("#SRRDADetailsTable").jqGrid("GridUnload");
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    
    $("#SRRDADetailsTable").jqGrid({
        url: '/MasterReports/SRRDADetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name', 'Agency Name', 'Parent Office', 'Office Type', 'Name', 'Address', 'Contact', 'TAN Number', 'Remarks'],
        colModel:[
            { name: "MAST_STATE_NAME", width: 150, align: 'left', height: 'auto' },
            { name: "MAST_AGENCY_NAME", width: 150, align: 'left', height: 'auto' },
            { name: "MAST_PARENT_ND_NAME", width: 150, align: 'left', height: 'auto' },
            { name: "MAST_ND_TYPE", width: 150, align: 'left', height: 'auto' },
            { name: "ADMIN_ND_NAME", width: 150, align: 'left', height: 'auto' },
            { name: "ADMIN_ND_ADDRESS", width: 150, align: 'left', height: 'auto' },
            { name: "ADMIN_ND_CONTACT", width: 150, align: 'left', height: 'auto' },
            { name: "ADMIN_ND_TAN_NO", width: 150, align: 'left', height: 'auto' },
            { name: "ADMIN_ND_REMARKS", width: 150, align: 'left', height: 'auto' }
        ],
        postData: { "StateCode": stateCode, "AgencyCode": agencyCode, "OfficeType": officeType },
        pager: $("#SRRDADetailsPager"),
        pgbuttons: true,
        sortname: 'ADMIN_ND_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '580',
        viewrecords: true,
        caption: 'SRRDA/DPIU Details',
        loadComplete: function () {
            $('#SRRDADetailsTable_rn').html('Sr.<br/>No.');
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