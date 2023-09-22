$(document).ready(function () {

    $.validator.unobtrusive.parse('#searchPropAddLength');

    //disabled enter key
    $("input").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    //state dropdown change event
    $("#ddlState").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlState").find(":selected").val() },
                   "#ddlDistrict", "/Proposal/GetDistrictByState?stateCode=" + $('#ddlState option:selected').val());
    });

    $("#ddlDistrict").change(function () {
        FillInCascadeDropdown({ userType: $("#ddlDistrict").find(":selected").val() },
                "#ddlMastBlockCode", "/Proposal/PopulateBlocks?districtCode=" + $("#ddlDistrict option:selected").val() + "");
        //$("#ddlImsPackages").val(0);
        //$("#ddlImsPackages").empty();
        //$("#ddlImsPackages").append("<option value='All'>All Packages</option>");
        //    FillInCascadeDropdown({ userType: $("#ddlDistrict").find(":selected").val() },
        //               "#ddlImsPackages", "/Proposal/GetPackageByState?yearCode=" + $('#ddlImsYear option:selected').val() + "&districtCode=" + $("#ddlDistrict option:selected").val() + "");
    });

    LoadProposalAdditionalLengthGrid();

    //list button click
    $("#btnListPropAddLength").click(function () {

        //validateFilter();
        $("#tbPropExecutionList").jqGrid('GridUnload');
        LoadProposalAdditionalLengthGrid();
    });

    $("#ddlImsYear").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlImsYear").find(":selected").val() },
                    "#ddlImsPackages", "/Proposal/GetPackagesByYearandBlock?sanctionYear=" + $('#ddlImsYear option:selected').val() + "&blockCode=" + $('#ddlMastBlockCode option:selected').val());
    });

    $("#ddlMastBlockCode").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlMastBlockCode").find(":selected").val() },
                    "#ddlImsPackages", "/Proposal/GetPackagesByYearandBlock?sanctionYear=" + $('#ddlImsYear option:selected').val() + "&blockCode=" + $('#ddlMastBlockCode option:selected').val());
    });

    //add accordion
    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    //filter view hide click
    $("#idFilterDiv").click(function () {
        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#searchPropAddCost").toggle("slow");
    });


});

// to load PropAddCost details
function LoadProposalAdditionalLengthGrid() {

    jQuery("#tbPropExecutionList").jqGrid({
        url: '/Proposal/GetProposalAdditionalLengthList',
        datatype: "json",
        mtype: "POST",
        postData: { stateCode: $('#ddlState option:selected').val(), districtCode: $('#ddlDistrict option:selected').val(), blockCode: $('#ddlMastBlockCode option:selected').val(), yearCode: $("#ddlImsYear option:selected").val(), packageCode: $("#ddlImsPackages option:selected").val(), proposalCode: $("#ddlImsProposalTypes").val(), batchCode: $('#ddlBatchs option:selected').val(), collaboration: $('#ddlCollaborations option:selected').val(), upgradationType: $('#ddlUpgradations option:selected').val() },
        colNames: ['State', 'District', 'Block', 'Year', 'Batch', 'Package No.', 'Road Name', 'Road/LSB Cost (In Lacs)', 'Road Length(in Kms)/LSB Length(int Mtrs)', 'Maintenance Cost(in Lacs)', 'Is Approved', 'Additional Length'],
        colModel: [
                            { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 80, align: "left", search: false, hidden: true },
                            { name: 'MAST_DISTRICT_NAME', index: 'MAST_DISTRICT_NAME', height: 'auto', width: 80, align: "left", search: false },
                            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 80, align: "left", search: false },
                            { name: 'Year', index: 'Year', width: 70, sortable: true, align: "center" },
                            { name: 'Batch', index: 'Batch', width: 70, sortable: true, align: "center" },
                            { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'PLAN_RD_NAME', index: 'PLAN_RD_NAME', height: 'auto', width: 250, align: "left", search: true },
                            { name: 'ROAD_COST', index: 'ROAD_COST', height: 'auto', width: 100, align: "right", search: true },
                            { name: 'ROAD_LENGTH', index: 'ROAD_LENGTH', height: 'auto', width: 100, align: "right", search: true },
                            { name: 'MAINTENANCE_COST', index: 'MAINTENANCE_COST', height: 'auto', width: 100, align: "right", search: true },
                            { name: 'IMS_IS_MRD_APPROVED', index: 'IMS_IS_MRD_APPROVED', height: 'auto', width: 50, align: "center", search: true },
                            { name: 'a', width: 50, sortable: false, resize: false, align: "center", search: false },
        ],
        pager: jQuery('#pagerPropAddLength').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "PLAN_RD_NAME",
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Proposal Additional Length Details List",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function (data) {
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }
    });

}

//populates result according to changed value
function FillInCascadeDropdown(map, dropdown, action) {
    var message = '';

    $(dropdown).empty();

    $.post(action, map, function (data) {
        $.each(data, function () {

            if (this.Selected == true) {
                $(dropdown).append("<option selected value=" + this.Value + ">" + this.Text + "</option>");
            }
            else {
                $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
            }
        });

    }, "json");
}

//returns the view of Physical progress of Proposal
function AddAdditionalLengthDetail(urlparameter) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Add Additional Length Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="ClosePropAddLengthDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divAddPropAddLength").load('/Proposal/AddAdditionLength?id=' + urlparameter, function () {
            //$.validator.unobtrusive.parse($('#frmAddPhysicalRoad'));
            unblockPage();
        });
        $('#divAddPropAddLength').show('slow');
        $("#divAddPropAddLength").css('height', 'auto');
    });
    $("#tbPropExecutionList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');

}



//close the accordion of physical and financial details
function ClosePropAddLengthDetails() {

    $("#accordion").hide('slow');
    $("#divAddPropAddLength").hide('slow');
    $("#tbPropExecutionList").jqGrid('setGridState', 'visible');
    ShowFilter();
}
//show the filter view 
function ShowFilter() {

    $("#divSearchPropAddLength").show('slow');
    $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s");
    $('#idFilterDiv').trigger('click');
}



