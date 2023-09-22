$(document).ready(function () {

    $("#ddlStates").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlStates").find(":selected").val() },
                           "#ddlDistricts", "/LockUnlock/GetDistrictsByStateCode?stateCode=" + $('#ddlStates option:selected').val());


        //setTimeout(function () {
        //    $("#ddlDistricts").find("option[value='0']").remove();
        //    $("#ddlDistricts").append("<option value='0' selected>All Districts</option>");
        //}, 500);
    });

    $("#ddlDistricts").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlStates").find(":selected").val() },
                           "#ddlBlocks", "/LockUnlock/GetBlocksByDistrictCode?districtCode=" + $('#ddlDistricts option:selected').val());

        //setTimeout(function () {
        //    $("#ddlBlocks").find("option[value='0']").val().remove();
        //    $("#ddlBlocks").append("<option value='0' selected>All Blocks</option>");
        //}, 500);

    });

    $("#btnViewDetails").click(function () {

        if ($("#frmFilterDetails").valid()) {
            LoadUnlockReport();
        }
    });


});
function FillInCascadeDropdown(map, dropdown, action) {
    var message = '';

    $(dropdown).empty();
    blockPage();
    $.post(action, map, function (data) {
        $.each(data, function () {

            if (this.Selected == true) {
                $(dropdown).append("<option selected value=" + this.Value + ">" + this.Text + "</option>");
            }
            else
            {
                if (dropdown == "#ddlDistricts" && this.Value == 0) {
                    this.Text = "All Districts";
                }
                else if (dropdown == "#ddlBlocks" && this.Value == 0)
                {
                    this.Text = "All Blocks"
                }
                $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
            }
        });

    }, "json");
    unblockPage();
}
function LoadUnlockReport()
{
    $("#tbrptDetails").jqGrid('GridUnload');
    jQuery("#tbrptDetails").jqGrid({
        url: '/LockUnlock/GetUnlockReportList',
        datatype: "json",
        mtype: "POST",
        postData: { StateCode: $("#ddlStates option:selected").val(), DistrictCode: $("#ddlDistricts option:selected").val(), PMGSYScheme: $("#ddlSchemes option:selected").val(), BlockCode: $("#ddlBlocks option:selected").val(), ModuleCode: $("#ddlModule option:selected").val() },
        colNames: ['State', 'District', 'Block', 'Unlock Level', 'Unlock Data', "Unlock Start Date", 'Unlock End Date'],
        colModel: [
                            { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 180, align: "left", sortable: true, align: 'center' },
                            { name: 'MAST_DISTRICT_NAME', index: 'MAST_DISTRICT_NAME', height: 'auto', width: 100, sortable: true, align: "left", align: 'center' },
                            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 150, sortable: true, align: "left", align: 'center' },
                            { name: 'UNLOCK_LEVEL', index: 'UNLOCK_LEVEL', height: 'auto', width: 100, sortable: true, align: "left", align: 'center' },
                            { name: 'UNLOCK_DATA', index: 'UNLOCK_DATA', width: 200, sortable: true, align: 'left' },
                            { name: 'IMS_UNLOCK_START_DATE', index: 'IMS_UNLOCK_START_DATE', width: 110, sortable: true, align: 'center' },
                            { name: 'IMS_UNLOCK_END_DATE', index: 'IMS_UNLOCK_END_DATE', width: 110, sortable: true, align: 'center' },
        ],
        pager: jQuery('#dvpgrptDetails'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_STATE_NAME',
        sortorder: "asc",
        caption: "Unlocked " + $("#ddlModule option:selected").text() + " List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        hidegrid: false,
        loadComplete: function () {
        },
        loadError: function (xhr, ststus, error) {
            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
            }
        }

    }); //end of grid
}