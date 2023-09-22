$(document).ready(function () {
    $.validator.unobtrusive.parse('searchExecution');

    //add accordion
    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    //state dropdown change event
    $("#ddlMastStateCode").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlMastStateCode").find(":selected").val() },
                   "#ddlMastDistrictCode", "/Execution/GetDistrictByStateCode?stateCode=" + $('#ddlMastStateCode option:selected').val());
    });

    $("#ddlMastDistrictCode").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlMastDistrictCode").find(":selected").val() },
                   "#ddlMastBlockCode", "/Execution/GetBlockByDistrict?districtCode=" + $("#ddlMastDistrictCode option:selected").val() + "");
    });

    //list button click
    $("#btnListExecution").click(function () {

        //validateFilter();
        $("#tbExecutionList").jqGrid('GridUnload');
        LoadExecutionGrid();
    });
});

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

// to load execution details
function LoadExecutionGrid() {

    jQuery("#tbExecutionList").jqGrid('GridUnload');
    jQuery("#tbExecutionList").jqGrid({
        url: '/Execution/GetExecutionProgressListMRD',
        datatype: "json",
        mtype: "GET",
        //postData: { yearCode: $("#ddlImsYear option:selected").val(), blockCode: $('#ddlMastBlockCode option:selected').val(), batchCode: $("#ddlImsBatch option:selected").val(), streamCode: $("#ddlImsStreams option:selected").val(), proposalCode: $("#ddlImsProposalTypes").val() },
        postData: { yearCode: $("#ddlImsYear option:selected").val(), districtCode: $("#ddlMastDistrictCode option:selected").val(), blockCode: $('#ddlMastBlockCode option:selected').val(), packageCode: $("#ddlImsPackages option:selected").val(), proposalCode: $("#ddlImsProposalTypes").val(), batch: $('#ddlBatchs option:selected').val(), collaboration: $('#ddlCollaborations option:selected').val(), upgradationType: $('#ddlUpgradations option:selected').val() },
        colNames: ['Block', 'Year', 'Batch', 'Package No.', 'Road Name', 'Road/LSB Cost (In Lacs)', 'Road Length(in Kms)/LSB Length(int Mtrs)', 'Maintenance Cost(in Lacs)', 'Convergence of Technology', 'CDWorks', 'Physical', 'Habitation', 'Technology', 'Financial', 'Images', 'Remarks', 'Executing Officer', 'Road Safety', 'View'],
        colModel: [
                            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 80, align: "left", search: false },
                            { name: 'Year', index: 'Year', width: 70, sortable: true, align: "center" },
                            { name: 'Batch', index: 'Batch', width: 70, sortable: true, align: "center" },
                            { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'PLAN_RD_NAME', index: 'PLAN_RD_NAME', height: 'auto', width: 250, align: "left", search: true },
                            { name: 'ROAD_COST', index: 'ROAD_COST', height: 'auto', width: 100, align: "right", search: true },
                            { name: 'ROAD_LENGTH', index: 'ROAD_LENGTH', height: 'auto', width: 100, align: "right", search: true },
                            { name: 'MAINTENANCE_COST', index: 'MAINTENANCE_COST', height: 'auto', width: 100, align: "right", search: true },
                            
                            { name: 'TechnologyConvergence', width: 70, sortable: false, resize: false, align: "center", search: false, hidden: true },
                            { name: 'IMS_NO_OF_CDWORKS', index: 'IMS_NO_OF_CDWORKS', height: 'auto', width: 50, align: "center", search: false, hidden: true },
                            { name: 'a', width: 50, sortable: false, resize: false, align: "center", search: false, hidden : $('#hdnRoleCode').val() == 65 ? true : false },
                            { name: 'HabitationDetails', width: 50, sortable: false, resize: false, align: "center", search: false, hidden: true },
                            { name: 'g', width: 50, sortable: false, resize: false, align: "center", search: false, hidden: true },
                            { name: 'b', width: 50, sortable: false, resize: false, /*formatter: FormatColumn,*/ align: "center", search: false, hidden:true },
                            { name: 'c', width: 50, sortable: false, resize: false, /*formatter: FormatColumn1,*/ align: "center", search: false, hidden: true },
                            { name: 'd', width: 50, sortable: false, resize: false, /*formatter: FormatColumn2,*/ align: "center", search: false, hidden: true },
                            { name: 'e', width: 50, sortable: false, resize: false, /*formatter: FormatColumn3,*/ align: "center", search: false, hidden: true },
                            { name: 'RoadDetails', width: 50, sortable: false, resize: false, align: "center", search: false, hidden: true },
                            { name: 'f', width: 50, sortable: false, resize: false, align: "center", search: false },

        ],
        pager: jQuery('#pagerExecution').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "PLAN_RD_NAME",
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Execution Details List",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function (data) {
            $("#tbExecutionList #pagerExecution").css({ height: '40px' });
            $("#pagerExecution_left").html("<label style='margin-left:8%;'><b>Note: </b>Financial Progress entry through Technical Module has been restricted.<label/>")
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


//returns the view of Physical progress of Proposal
function AddPhysicalDetails(urlparameter) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Add Physical Road Progress Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseExecutionDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divAddExecution").load('/Execution/ListPhysicalDetailsMRD?urlparameter=' + urlparameter, function () {
            $.validator.unobtrusive.parse($('#frmAddPhysicalRoad'));
            unblockPage();
        });
        $('#divAddExecution').show('slow');
        $("#divAddExecution").css('height', 'auto');
    });
    $("#tbExecutionList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');
}

//returns the view of Physical progress of bridges
function AddPhysicalLSBDetails(urlparameter) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Add Physical LSB Progress Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseExecutionDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divAddExecution").unload();
        $("#divAddExecution").load('/Execution/ListLSBPhysicalDetails?urlparameter=' + urlparameter, function () {
            $.validator.unobtrusive.parse($('#frmAddPhysicalLSB'));
            unblockPage();
        });
        $('#divAddExecution').show('slow');
        $("#divAddExecution").css('height', 'auto');
    });
    $("#tbExecutionList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');
}

//close the accordion of physical and financial details
function CloseExecutionDetails() {

    $("#accordion").hide('slow');
    $("#divAddExecution").hide('slow');
    $("#tbExecutionList").jqGrid('setGridState', 'visible');
    ShowFilter();
}

//show the filter view 
function ShowFilter() {

    $("#divSearchExecution").show('slow');
    $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s");
    $('#idFilterDiv').trigger('click');
}

function ViewExecutionDetails(urlparameter) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Execution Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseExecutionDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divAddExecution").unload();
        $("#divAddExecution").load('/Execution/ViewExecutionProgressDetails?urlparameter=' + urlparameter, function (data) {
            $.validator.unobtrusive.parse($('#divAddExecution'));
            unblockPage();
            if (data.success == false) {
                alert(data.message);
            }
        });
        $('#divAddExecution').show('slow');
        $("#divAddExecution").css('height', 'auto');
    });
    $("#tbExecutionList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');
}
