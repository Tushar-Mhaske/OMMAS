
$(document).ready(function () {
    if ($("#MAST_STATE_CODE").val() > 0) {

        $("#StateList_NodalOfficerDetails").attr("disabled", "disabled");
    }
    $("#NodalOfficerDetailsButton").click(function () {
        var stateCode = $("#StateList_NodalOfficerDetails").val();
        var agencyCode = $("#AgencyList_NodalOfficerDetails").val();
        var officeType = $("#OfficeList_NodalOfficerDetails").val();
        var activeType = $("#ActiveList_NodalOfficerDetails").val();

        NodalOfficerReportsListing(stateCode, agencyCode, officeType, activeType);
    });
    $("#StateList_NodalOfficerDetails").change(function () {

        $("#AgencyList_NodalOfficerDetails").val(0);
        $("#AgencyList_NodalOfficerDetails").empty();

        $("#AgencyList_NodalOfficerDetails").append("<option value='0'>All Agency</option>");

        if ($(this).val() > 0) {
            if ($("#AgencyList_NodalOfficerDetails").length > 0) {
                $.ajax({
                    url: '/MasterReports/NodalOfficerDetails',
                    type: 'POST',
                    data: { "StateCode": $(this).val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#AgencyList_NodalOfficerDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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
            //  $("#AgencyList_NodalOfficerDetails").empty();
            $("#AgencyList_NodalOfficerDetails").append("<option value='0'>All Agency</option>");



        }
    });

    $("#StateList_NodalOfficerDetails").trigger('change');
    $("#NodalOfficerDetailsButton").trigger('click');
    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');

  
});



function NodalOfficerReportsListing(stateCode, agencyCode, officeType, activeType) {
    $("#NodalOfficerDetailsTable").jqGrid("GridUnload");
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#NodalOfficerDetailsTable").jqGrid({
        url: '/MasterReports/NodalOfficerDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name','Agency Name','Parent Office Name','Office Type','Office Name','Nodal Officer Name','Designation','Address','Contact','Level',
            'Empanelled','Start Date','End Date'],
        colModel:[
            {name: "MAST_STATE_NAME", width: 150, align: 'left', height: 'auto'},
            {name: "MAST_AGENCY_NAME", width: 150, align: 'left', height: 'auto'},
            {name: "MAST_PARENT_ND_NAME", width: 150, align: 'left', height: 'auto'},
            {name: "MAST_ND_TYPE", width: 150, align: 'left', height: 'auto'},
            {name: "ADMIN_ND_NAME", width: 150, align: 'left', height: 'auto'},
            {name: "ADMIN_NO_NAME", width: 150, align: 'left', height: 'auto'},
            {name: "MAST_DESIG_NAME", width: 150, align: 'left', height: 'auto'},
            {name: "ADMIN_NO_ADDRESS", width: 150, align: 'left', height: 'auto'},
            {name: "ADMIN_NO_CONTACT", width: 150, align: 'left', height: 'auto'},
            {name: "ADMIN_NO_LEVEL", width: 150, align: 'left', height: 'auto'},
            {name: "ADMIN_ACTIVE_STATUS", width: 150, align: 'left', height: 'auto'},
            {name: "ADMIN_ACTIVE_START_DATE", width: 150, align: 'left', height: 'auto'},
            {name: "ADMIN_ACTIVE_END_DATE", width: 150, align: 'left', height: 'auto'}
        ],
        postData: { "StateCode": stateCode, "AgencyCode": agencyCode, "OfficeType": officeType,"ActiveType":activeType },
        pager: $("#NodalOfficerDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        rownumbers: true,
        autowidth: true,
        height: '580',
        viewrecords: true,
        caption: 'Nodel Officer Details',
        loadComplete: function () {
            $('#NodalOfficerDetailsTable_rn').html('Sr.<br/>No.');
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