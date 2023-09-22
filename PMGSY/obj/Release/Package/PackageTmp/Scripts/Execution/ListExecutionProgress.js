$(document).ready(function () {

    $.validator.unobtrusive.parse('#divAddPhysicalRoad');

    //disabled enter key
    $("input").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    //state dropdown change event
    $("#ddlState").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlState").find(":selected").val() },
                   "#ddlDistrict", "/Execution/GetDistrictByState?stateCode=" + $('#ddlState option:selected').val());
    });

    $("#ddlDistrict").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlDistrict").find(":selected").val() },
                   "#ddlPackage", "/Execution/GetPackageByState?yearCode=" + $('#ddlYear option:selected').val() + "&districtCode=" + $("#ddlDistrict option:selected").val() + "");
    });

    LoadExecutionGrid();

    //list button click
    $("#btnListExecution").click(function () {

        //validateFilter();
        $("#tbExecutionList").jqGrid('GridUnload');
        LoadExecutionGrid();
    });

    $("#ddlImsYear").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlImsYear").find(":selected").val() },
                    "#ddlImsPackages", "/Execution/GetPackagesByYearandBlock?sanctionYear=" + $('#ddlImsYear option:selected').val() + "&blockCode=" + $('#ddlMastBlockCode option:selected').val());
    });

    $("#ddlMastBlockCode").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlMastBlockCode").find(":selected").val() },
                    "#ddlImsPackages", "/Execution/GetPackagesByYearandBlock?sanctionYear=" + $('#ddlImsYear option:selected').val() + "&blockCode=" + $('#ddlMastBlockCode option:selected').val());
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
        $("#searchExecution").toggle("slow");
    });


});

// to load execution details
function LoadExecutionGrid() {
    jQuery("#tbExecutionList").jqGrid('GridUnload');
    jQuery("#tbExecutionList").jqGrid({
        url: '/Execution/GetExecutionProgressList',
        datatype: "json",
        mtype: "POST",
        //postData: { yearCode: $("#ddlImsYear option:selected").val(), blockCode: $('#ddlMastBlockCode option:selected').val(), batchCode: $("#ddlImsBatch option:selected").val(), streamCode: $("#ddlImsStreams option:selected").val(), proposalCode: $("#ddlImsProposalTypes").val() },
        postData: { yearCode: $("#ddlImsYear option:selected").val(), blockCode: $('#ddlMastBlockCode option:selected').val(), packageCode: $("#ddlImsPackages option:selected").val(), proposalCode: $("#ddlImsProposalTypes").val(), batch: $('#ddlBatchs option:selected').val(), collaboration: $('#ddlCollaborations option:selected').val(), upgradationType: $('#ddlUpgradations option:selected').val() },
        colNames: ['Block', 'Year', 'Batch', 'Package No.', 'Road Name', 'Road/LSB Cost (In Lacs)', 'Road Length(in Kms)/LSB Length(int Mtrs)', 'Maintenance Cost(in Lacs)', 'Convergence of Technology', 'CDWorks', 'Physical', 'Habitation', 'Technology', 'Financial', 'Images', 'Remarks', 'Executing Officer', 'Road Safety', 'View', 'View Location'],
        colModel: [
                            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 80, align: "left", search: false },
                            { name: 'Year', index: 'Year', width: 70, sortable: true, align: "center" },
                            { name: 'Batch', index: 'Batch', width: 70, sortable: true, align: "center" },
                            { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'PLAN_RD_NAME', index: 'PLAN_RD_NAME', height: 'auto', width: 250, align: "left", search: true },
                            { name: 'ROAD_COST', index: 'ROAD_COST', height: 'auto', width: 100, align: "right", search: true },
                            { name: 'ROAD_LENGTH', index: 'ROAD_LENGTH', height: 'auto', width: 100, align: "right", search: true },
                            { name: 'MAINTENANCE_COST', index: 'MAINTENANCE_COST', height: 'auto', width: 100, align: "right", search: true },
                            //{ name: 'IMS_NO_OF_BRIDGEWRKS', index: 'IMS_NO_OF_BRIDGEWRKS', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'TechnologyConvergence', width: 70, sortable: false, resize: false, align: "center", search: false },
                            { name: 'IMS_NO_OF_CDWORKS', index: 'IMS_NO_OF_CDWORKS', height: 'auto', width: 50, align: "center", search: false },
                            { name: 'a', width: 50, sortable: false, resize: false, align: "center", search: false },
                            { name: 'HabitationDetails', width: 50, sortable: false, resize: false, align: "center", search: false },
                            { name: 'g', width: 50, sortable: false, resize: false, align: "center", search: false },
                            { name: 'b', width: 50, sortable: false, resize: false, formatter: FormatColumn, align: "center", search: false },
                            { name: 'c', width: 50, sortable: false, resize: false, formatter: FormatColumn1, align: "center", search: false },
                            { name: 'd', width: 50, sortable: false, resize: false, formatter: FormatColumn2, align: "center", search: false },
                            { name: 'e', width: 50, sortable: false, resize: false, formatter: FormatColumn3, align: "center", search: false },
                            { name: 'RoadDetails', width: 50, sortable: false, resize: false, align: "center", search: false },
                            { name: 'f', width: 50, sortable: false, resize: false, align: "center", search: false },
                            { name: 'h', width: 50, sortable: false, resize: false, align: "center", search: false }
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
function AddPhysicalDetails(urlparameter) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Add Physical Road Progress Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseExecutionDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divAddExecution").load('/Execution/ListPhysicalDetails?urlparameter=' + urlparameter, function () {
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
//returns the view of Financial details
function AddFinancialDetails(urlparameter) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Add Financial Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseExecutionDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divAddExecution").unload();
        $("#divAddExecution").load('/Execution/ListFinancialDetails?urlparameter=' + urlparameter, function () {
            $.validator.unobtrusive.parse($('#frmAddFinancialProgress'));
            unblockPage();
        });
        $('#divAddExecution').show('slow');
        $("#divAddExecution").css('height', 'auto');
    });
    $("#tbExecutionList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');
}
function AddCDWorksDetails(urlparameter) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Add/Edit CDWorks Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseExecutionDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divAddExecution").unload();
        $("#divAddExecution").load('/Execution/ListCDWorksDetails?urlparameter=' + urlparameter, function () {
            $.validator.unobtrusive.parse($('#frmAddCDWorks'));
            unblockPage();
        });
        $('#divAddExecution').show('slow');
        $("#divAddExecution").css('height', 'auto');
    });
    $("#tbExecutionList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');
}

function AddRemarksDetails(urlparameter) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Add/Edit Remarks Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseExecutionDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divAddExecution").unload();
        $("#divAddExecution").load('/Execution/ListRemarkDetails?urlparameter=' + urlparameter, function (data) {
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


function AddUploadDetails(urlparameter) {
    $("#Urlparameter").val(urlparameter);

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Upload Images</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseExecutionDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divAddExecution").unload();
        $("#divAddExecution").load('/Execution/FileUpload/' + urlparameter, function (data) {
            $.validator.unobtrusive.parse($('#fileupload'));
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

function AddExecutingOfficerDetails(urlparameter) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Add/Edit Executing Officer Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseExecutionDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divAddExecution").unload();
        $("#divAddExecution").load('/Execution/ListExecutingOfficerDetails?urlparameter=' + urlparameter, function (data) {
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

function FormatColumn1(cellvalue, options, rowObject) {

    if (cellvalue.toString() == "") {
        return "<center><span class='ui-icon ui-icon-locked ui-align-center'></span></center>";
    }
    else {
        return "<center><span  class='ui-icon ui-icon-plusthick' title='Click here to upload images' onClick ='AddUploadDetails(\"" + cellvalue.toString() + "\");'></span></center>";
    }

}
function FormatColumn2(cellvalue, options, rowObject) {

    if (cellvalue.toString() == "") {
        return "<center><span class='ui-icon ui-icon-locked ui-align-center'></span></center>";
    }
    else {
        return "<center><span  class='ui-icon ui-icon-plusthick' title='Click here to add remark' onClick ='AddRemarksDetails(\"" + cellvalue.toString() + "\");'></span></center>";
    }

}
function FormatColumn3(cellvalue, options, rowObject) {

    if (cellvalue.toString() == "") {
        return "<center><span class='ui-icon ui-icon-locked ui-align-center'></span></center>";
    }
    else {
        return "<center><span  class='ui-icon ui-icon-plusthick' title='Click here to add executing officer details' onClick ='AddExecutingOfficerDetails(\"" + cellvalue.toString() + "\");'></span></center>";
    }

}

// Added on 1 August 2018
function ViewLocation(urlparameter) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Execution Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseExecutionDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divAddExecution").unload();
        $("#divAddExecution").load('/Execution/ViewLocationDetails?urlparameter=' + urlparameter, function (data) {
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

function FormatColumn(cellvalue, options, rowObject) {

    //if (cellvalue.toString() == "") {
    //    return "<center><span class='ui-icon ui-icon-locked ui-align-center'></span></center>";
    //}
    //else {
    //    return "<center><span  class='ui-icon ui-icon-plusthick' title='Click here to add financial details' onClick ='AddFinancialDetails(\"" + cellvalue.toString() + "\");'></span></center>";
    //}
    return "<center><span title='Financial Progress entry through Technical Module has been restricted.' class='ui-icon ui-icon-locked ui-align-center'></span></center>";
}


function AddTechnologyDetails(urlparameter) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Technology Progress Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseExecutionDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divAddExecution").unload();
        $("#divAddExecution").load('/Execution/ListTechnologyDetails?urlparameter=' + urlparameter, function () {
            //$.validator.unobtrusive.parse($('#frmAddCDWorks'));
            unblockPage();
        });
        $('#divAddExecution').show('slow');
        $("#divAddExecution").css('height', 'auto');
    });
    $("#tbExecutionList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');
}

//close the accordion of technology details
function CloseTechnologyDetails() {

    $("#accordion").hide('slow');
    $("#divAddExecution").hide('slow');
    $("#tbExecutionList").jqGrid('setGridState', 'visible');
    ShowFilter();
}

//returns the view of Physical progress of Proposal
function AddHabitationDetails(urlparameter) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Add Habitation Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseExecutionDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divAddExecution").load('/Execution/ListHabitationDetails?urlparameter=' + urlparameter, function () {
            //$.validator.unobtrusive.parse($('#frmAddPhysicalRoad'));
            unblockPage();
        });
        $('#divAddExecution').show('slow');
        $("#divAddExecution").css('height', 'auto');
    });
    $("#tbExecutionList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');

}

//returns the view of Convergence of Technology
function AddTechnologyConvergenceDetails(urlparameter) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Add Technology Convergence Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseExecutionDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();

        //         $("#divAddExecution").load('/Proposal/ListTechnologyDetails?id=' + urlparameter, { Convergence: 'Y' }, function () {

        $("#divAddExecution").load('/Proposal/ListTechnologyDetails/' + urlparameter, { Convergence: 'Y' }, function () {
            //$.validator.unobtrusive.parse($('#frmAddPhysicalRoad'));
            unblockPage();
        });
        $('#divAddExecution').show('slow');
        $("#divAddExecution").css('height', 'auto');
    });
    $("#tbExecutionList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');
}

//returns the view of Road Safety
function AddRoadSafety(urlparameter) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Add Road Safety Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseExecutionDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divAddExecution").load('/Execution/RoadSafetyLayout?urlparameter=' + urlparameter, function () {
            //$.validator.unobtrusive.parse($('#frmAddPhysicalRoad'));
            unblockPage();
        });
        $('#divAddExecution').show('slow');
        $("#divAddExecution").css('height', 'auto');
    });
    $("#tbExecutionList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');
}