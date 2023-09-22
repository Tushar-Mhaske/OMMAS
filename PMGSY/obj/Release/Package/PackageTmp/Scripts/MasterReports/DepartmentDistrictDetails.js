
$(function () {

    if ($("#MAST_STATE_CODE").val() > 0) {

        $("#MAST_Dep_District_State").attr("disabled", "disabled");
    }
    $("#DepartmentDistrictDetailsButton").click(function () {

        var stateCode = $("#MAST_Dep_District_State").val();
        var agencyCode = $("#MAST_Dep_District_Agency").val();
        var officeType = $("#MAST_Dep_District_OfficeType").val();

        DepartmentDistrictReportsListing(stateCode, agencyCode, officeType);
    });

    $("#MAST_Dep_District_State").change(function () {

        $("#MAST_Dep_District_Agency").val(0);
        $("#MAST_Dep_District_Agency").empty();

        $("#MAST_Dep_District_Agency").append("<option value='0'>All</option>");

        if ($(this).val() > 0) {
            if ($("#MAST_Dep_District_Agency").length > 0) {
                $.ajax({
                    url: '/MasterReports/DepartmentDistrictDetails',
                    type: 'POST',
                    data: { "StateCode": $(this).val() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#MAST_Dep_District_Agency").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
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
            //  $("#MAST_Dep_District_Agency").empty();
            $("#MAST_Dep_District_Agency").append("<option value='0'>All</option>");



        }
    });

    $("#MAST_Dep_District_State").trigger('change');
    $("#DepartmentDistrictDetailsButton").trigger('click');

    $('#tblRptContents').bind('resize', function () {
        resizeJqGrid();
    }).trigger('resize');
});

function DepartmentDistrictReportsListing(stateCode, agencyCode, officeType) {
    $("#DepartmentDistrictDetailsTable").jqGrid("GridUnload");
    $("#DepartmentDistrictDetailsTable").jqGrid({
        url: '/MasterReports/DepartmentDistrictDetailsListing',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['State Name', 'Agency Name', 'SRRDA Name', 'Office TYPE', 'SRRDA Admin Name', 'SRRDA District Name'],
        colModel: [
            { name: "MAST_STATE_NAME", width: '200', align: 'center', height: 'auto' },
            { name: "MAST_AGENCY_NAME", width: '200', align: 'center', height: 'auto' },
            { name: "MAST_PARENT_ND_NAME", width: '200', align: 'center', height: 'auto' },
            { name: "MAST_ND_TYPE", width: '200', align: 'center', height: 'auto' },
            { name: "ADMIN_ND_NAME", width: '200', align: 'center', height: 'auto' },
            { name: "MAST_DISTRICT_NAME", width: '200', align: 'center', height: 'auto' }
        ],
        postData: { "StateCode": stateCode, "AgencyCode": agencyCode, "OfficeType": officeType },
        pager: $("#DepartmentDistrictDetailsPager"),
        pgbuttons: true,
        sortname: 'MAST_STATE_NAME',
        sortorder: 'asc',
        rowNum: 2147483647,
        recordtext: '{2} records found',
        rownumbers: true,
        autowidth: true,
        height: '580',
        viewrecords: true,
        caption: 'Department District Report',
        loadComplete: function () {
            $('#DepartmentDistrictDetailsTable_rn').html('Sr.<br/>No.');
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