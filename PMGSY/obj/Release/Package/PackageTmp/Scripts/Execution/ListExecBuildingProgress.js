$(document).ready(function () {

    //$.validator.unobtrusive.parse('#divAddPhysicalRoad');

    LoadBUILDINGExecutionGrid();

    //list button click
    $("#btnListExecution1").click(function () {
        
        //validateFilter();
        $("#tbExecutionList").jqGrid('GridUnload');
        LoadBUILDINGExecutionGrid();
    });

    //add accordion
    $(function () {
        //$("#accordion").accordion("destroy");    // Removes the accordion bits
        //$("#accordion").empty();                // Clears the contents

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
function LoadBUILDINGExecutionGrid() {
    jQuery("#tbExecutionList").jqGrid('GridUnload');
    jQuery("#tbExecutionList").jqGrid({
        url: '/Execution/GetExecBuildingProposalList',
        datatype: "json",
        mtype: "GET",
        //postData: { yearCode: $("#ddlImsYear option:selected").val(), blockCode: $('#ddlMastBlockCode option:selected').val(), batchCode: $("#ddlImsBatch option:selected").val(), streamCode: $("#ddlImsStreams option:selected").val(), proposalCode: $("#ddlImsProposalTypes").val() },
        postData: { yearCode: $("#ddlImsYear option:selected").val(), blockCode: $('#ddlMastBlockCode option:selected').val(), packageCode: $("#ddlImsPackages option:selected").val(), proposalCode: $("#ddlImsProposalTypes").val(), batch: $('#ddlBatchs option:selected').val(), collaboration: $('#ddlCollaborations option:selected').val(), upgradationType: $('#ddlUpgradations option:selected').val() },
        colNames: ['State', 'District', 'Block', 'Year', 'Batch', 'Package No.', 'Building Name', 'Total Cost (In Lacs)', 'Earthwork Excavation and PCC', 'Foundation', 'Superstructure'
                    //, 'Road Length(in Kms)/LSB Length(int Mtrs)', 'Maintenance Cost(in Lacs)', 'Convergence of Technology', 'CDWorks', 'Physical', 'Habitation', 'Technology', 'Financial', 'Images', 'Remarks', 'Executing Officer', 'Road Safety',                              'View', 'View Location'
        ],
        colModel: [
                            { name: 'State', index: 'State', height: 'auto', width: 80, align: "left", search: false, hidden: true, },
                            { name: 'District', index: 'District', height: 'auto', width: 80, align: "left", search: false, hidden: true, },
                            { name: 'MAST_BLOCK_NAME', index: 'MAST_BLOCK_NAME', height: 'auto', width: 80, align: "left", search: false },
                            { name: 'Year', index: 'Year', width: 70, sortable: true, align: "center" },
                            { name: 'Batch', index: 'Batch', width: 70, sortable: true, align: "center" },
                            { name: 'IMS_PACKAGE_ID', index: 'IMS_PACKAGE_ID', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'PLAN_RD_NAME', index: 'PLAN_RD_NAME', height: 'auto', width: 250, align: "left", search: true },
                            { name: 'ROAD_COST', index: 'ROAD_COST', height: 'auto', width: 100, align: "right", search: true },
                            { name: 'a', width: 50, sortable: false, resize: false, formatter: FormatColumn1, align: "center", search: false },
                            { name: 'b', width: 50, sortable: false, resize: false, formatter: FormatColumn2, align: "center", search: false },
                            { name: 'c', width: 50, sortable: false, resize: false, formatter: FormatColumn3, align: "center", search: false },
        ],
        pager: jQuery('#pagerExecution').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "PLAN_RD_NAME",
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Building Execution Details List",
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

function AddEarthworkExcavationDetails(urlparameter) {
    $("#Urlparameter").val(urlparameter);

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Earthwork Excavation PCC</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseExecutionDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divAddExecution").unload();
        $("#divAddExecution").load('/Execution/ListEarthWorkExcavation/' + urlparameter, function (data) {
            $.validator.unobtrusive.parse($('#frmListEarthworkExcavation'));
            unblockPage();
            if (data.success == false) {
                alert(data.message);
            }
            $('#tbExcavationList').trigger('reloadGrid');//added by abhinav
        });
        $('#divAddExecution').show('slow');
        $("#divAddExecution").css('height', 'auto');
    });
    $("#tbExecutionList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');
}

function AddFoundationDetails(urlparameter) {
    $("#Urlparameter").val(urlparameter);

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Foundation Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseExecutionDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divAddExecution").unload();
        $("#divAddExecution").load('/Execution/ListFoundationDetails/' + urlparameter, function (data) {
            $.validator.unobtrusive.parse($('#frmListFoundation'));
            unblockPage();
            if (data.success == false) {
                alert(data.message);
            }
            $('#tbExcavationList').trigger('reloadGrid');//added by abhinav
        });
        $('#divAddExecution').show('slow');
        $("#divAddExecution").css('height', 'auto');
    });
    $("#tbExecutionList").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');
}

function AddSuperstructureDetails(urlparameter) {
    
    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Superstructure Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseExecutionDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divAddExecution").unload();
        $("#divAddExecution").load('/Execution/ListSuperstructureDetails/' + urlparameter, function (data) {
            $.validator.unobtrusive.parse($('#frmListSuperstructure'));
            unblockPage();
            if (data.success == false) {
                alert(data.message);
            }
            $('#tbExcavationList').trigger('reloadGrid');//added by abhinav
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
        return "<center><span  class='ui-icon ui-icon-plusthick' title='Click here to upload images' onClick ='AddEarthworkExcavationDetails(\"" + cellvalue.toString() + "\");'></span></center>";
    }

}
function FormatColumn2(cellvalue, options, rowObject) {

    if (cellvalue.toString() == "") {
        return "<center><span class='ui-icon ui-icon-locked ui-align-center'></span></center>";
    }
    else if (cellvalue.toString() == "-") {
        return "-";
    }
    else {
        return "<center><span  class='ui-icon ui-icon-plusthick' title='Click here to add remark' onClick ='AddFoundationDetails(\"" + cellvalue.toString() + "\");'></span></center>";
    }

}
function FormatColumn3(cellvalue, options, rowObject) {

    if (cellvalue.toString() == "") {
        return "<center><span class='ui-icon ui-icon-locked ui-align-center'></span></center>";
    }
    else if (cellvalue.toString() == "-") {
        return "-";
    }
    else {
        return "<center><span  class='ui-icon ui-icon-plusthick' title='Click here to add executing officer details' onClick ='AddSuperstructureDetails(\"" + cellvalue.toString() + "\");'></span></center>";
    }

}

//close the accordion of physical and financial details
function CloseExecutionDetails() {

    $("#accordion").hide('slow');
    $("#divAddExecution").html('');
    $("#divAddExecution").hide('slow');
    $("#tbExecutionList").trigger('reloadGrid');
    $("#tbExecutionList").jqGrid('setGridState', 'visible');
    ShowFilter();
}
//show the filter view 
function ShowFilter() {

    $("#divSearchExecution").show('slow');
    $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s");
    $('#idFilterDiv').trigger('click');
}