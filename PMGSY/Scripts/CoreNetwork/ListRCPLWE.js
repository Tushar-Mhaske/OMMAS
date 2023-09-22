var scheme;
var title;
$(document).ready(function () {
    scheme = $("#Scheme").val();
    title = "RCPLWE Road";
    $.validator.unobtrusive.parse('#frmAddRCPLWE');

    $("input").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    LoadNetworkGrid();

    // List Core Network Click
    $("#btnListRCPLWE").click(function () {

        if (($('#ddlBlocks').val() == null) || ($('#ddlBlocks').val() < 0))
        {
            alert('Please select block');
            return false;
        }

        blockPage();
        $('#tbProposalList').jqGrid('GridUnload');
        searchDetails();
        $("#networkCategory").jqGrid('setGridState', 'visible');
        unblockPage();
    });

    if (!($("#divSearchNetwork").is(":visible"))) {
        $("#divSearchNetwork").show();
        $("#divSearchNetwork").load("/CoreNetwork/SearchNetworks");
        $("#btnSearch").hide();
    }

    $("#idFilterDiv").click(function () {
        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#filterForm").toggle("slow");
    });

    //Add Core network
    $("#btnAddRCPLWE").click(function () {

        var blockCode = $('#ddlBlocks option:selected').val();

        $("#accordion div").html("");
        $("#accordion h3").html(
                "<a href='#' style= 'font-size:.9em;' >" + title + "</a>" +
                '<a href="#" style="float: right;">' +
                '<img class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
                );
        $('#accordion').show('fold', function () {
            blockPage();
            $("#divAddForm").load("/CoreNetwork/AddEditRCPLWE?blockCode=" + blockCode, function () {
                $.validator.unobtrusive.parse($('#frmAddRCPLWE'));
                unblockPage();
            });
            $('#divAddForm').show('slow');
            $("#divAddForm").css('height', 'auto');
        });
        $("#networkCategory").jqGrid('setGridState', 'hidden');
        $("#networkCategory").setGridParam('hidegrid', false);
    });

    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $("#gs_PLAN_RD_NAME").attr('placeholder', 'Enter Road Name to Search');


    $("#ddlStates").change(function () {
        FillInCascadeDropdown({ userType: $("#ddlStates").find(":selected").val() },
                   "#ddlDistricts", "/CoreNetwork/GetDistrictByState?stateCode=" + $('#ddlStates option:selected').val());
    });

    $("#ddlDistricts").change(function () {
        FillInCascadeDropdown({ userType: $("#ddlDistricts").find(":selected").val() },
                   "#ddlBlocks", "/CoreNetwork/GetBlocksByDistrictRCPLWE?districtCode=" + $('#ddlDistricts option:selected').val());
    });


});

function showFilter() {
    if ($('#filterForm').is(":hidden")) {
        $("#filterForm").show('slow');
        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s");
    }
}

function CloseProposalDetails() {
    $('#accordion').hide('slow');
    $('#divAddForm').hide('slow');
    $("#networkCategory").jqGrid('setGridState', 'visible');
    showFilter();
}

function FormatColumn2(cellvalue, options, rowObject) {

    if (cellvalue.toString() == "") {
        return "<center><span class='ui-icon ui-icon-locked ui-align-center'></span></center>";
    }
    else {
        return "<center><span class='ui-icon ui-icon-pencil' title='Edit Core Network Details' onClick ='editNetworkData(\"" + cellvalue.toString() + "\");'></span></center>";
    }
}

function FormatColumn3(cellvalue, options, rowObject) {

    if (cellvalue.toString() == "") {
        return "<center><span class='ui-icon ui-icon-locked ui-align-center'></span></center>";
    }
    else {
        return "<center><span class='ui-icon ui-icon-trash' title='Delete Core Network Details' onClick ='deleteNetworkData(\"" + cellvalue.toString() + "\");'></span></center>";
    }
}

function FormatColumn(cellvalue, options, rowObject) {

    if (cellvalue.toString() == "") {
        return "<center><span class='ui-icon ui-icon-locked ui-align-center'></span></center>";
    }
    else {
        return "<center><span  class='ui-icon ui-icon-plusthick' title='Enter Habitation Details' onClick ='editHabitationDetails(\"" + cellvalue.toString() + "\");'>Habitation</span></center>";
    }

}


function FormatColumn4(cellvalue, options, rowObject) {

    if (cellvalue.toString() == "") {
        return "<center><span class='ui-icon ui-icon-locked ui-align-center'></span></center>";
    }
    else {
        return "<center><span  class='ui-icon ui-icon-plusthick' title='Upload Details' onClick ='UploadFile(\"" + cellvalue.toString() + "\");'></span></center>";
    }

}

function FormatColumn1(cellvalue, options, rowObject) {

    return "<center><span class='ui-icon ui-icon-zoomin' title='Core Network Details' onClick ='detailsCoreNetwork(\"" + cellvalue.toString() + "\");'></span></center>";
}

function LoadNetworkGrid() {
    
    jQuery("#networkCategory").jqGrid('GridUnload');

    jQuery("#networkCategory").jqGrid({
        url: '/CoreNetwork/GetRCPLWEList',
        datatype: "json",
        mtype: "POST",
        postData: { blockCode: $("#ddlBlocks option:selected").val(), roadType: $("#ddlRoute option:selected").val(), categoryCode: $("#ddlCategory option:selected").val(), districtCode: $("#ddlDistricts option:selected").val(), stateCode: $("#ddlStates option:selected").val() },
        colNames: ['RCPLWE Road System Id', 'Route Type', 'Road No.', 'Road Name', 'DRRP Road Code', 'Road From', 'Road To', 'Start Chainage [In Km]', 'End Chainage [In Km]', 'Length [In Km]', 'RCPLWE Total Road Length [In Km]', 'Habitation', 'Upload', 'Map DRRP Road', 'View', 'Edit', 'Delete'],
        colModel: [
                            { name: 'CNCode', index: 'CNCode', height: 'auto', width: 60, align: "left", sortable: true, search: true },
                            { name: 'PLAN_RD_ROUTE', index: 'PLAN_RD_ROUTE', height: 'auto', width: 70, align: "left", search: false, hidden: true },
                            { name: 'PLAN_CN_ROAD_NUMBER', index: 'PLAN_CN_ROAD_NUMBER', height: 'auto', width: 50, align: "left", search: false, hidden: true },
                            { name: 'PLAN_RD_NAME', index: 'PLAN_RD_NAME', height: 'auto', width: 220, align: "left", search: true },
                            { name: 'ER_ROAD_NUMBER', index: 'ER_ROAD_NUMBER', height: 'auto', width: 80, align: "left", search: false },
                            { name: 'PLAN_RD_FROM', index: 'PLAN_RD_FROM', height: 'auto', width: 140, align: "left", search: false },
                            { name: 'PLAN_RD_TO', index: 'PLAN_RD_TO', height: 'auto', width: 140, align: "left", search: false },
                            { name: 'PLAN_RD_FROM_CHAINAGE', index: 'PLAN_RD_FROM_CHAINAGE', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'PLAN_RD_TO_CHAINAGE', index: 'PLAN_RD_TO_CHAINAGE', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'PLAN_RD_LENGTH', index: 'PLAN_RD_LENGTH', height: 'auto', width: 70, align: "center", search: false },
                            { name: 'PLAN_RD_TOTAL_LENGTH', index: 'PLAN_RD_TOTAL_LENGTH', height: 'auto', width: 70, align: "center", search: false, },
                            { name: 'Habitation', index: 'Habitation', height: 'auto', width: 70, align: "left", sortable: false, search: false, hidden: (($("#RoleCode").val() == 22 || $("#RoleCode").val() == 54) ? false : true) },
                            { name: 'c', width: 50, sortable: false, resize: false, align: "center", search: false, hidden: (($("#RoleCode").val() == 22 || $("#RoleCode").val() == 54) ? false : true) },
                            { name: 'mapRoad', width: 60, sortable: false, resize: false, align: "center", search: false, hidden: (($("#RoleCode").val() == 22 || $("#RoleCode").val() == 54) ? false : true) /*hidden:true*/ },
                            { name: 'Details', index: 'Details', height: 'auto', width: 50, align: "left", sortable: false, search: false },
                            { name: 'a', width: 50, sortable: false, resize: false, align: "center", search: false, hidden: (($("#RoleCode").val() == 22 || $("#RoleCode").val() == 54) ? false : true) },
                            { name: 'b', width: 50, sortable: false, resize: false, align: "center", search: false, hidden: (($("#RoleCode").val() == 22 || $("#RoleCode").val() == 54) ? false : true) }
        ],
        pager: jQuery('#pager').width(20),
        rowNum: 10,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortorder: "desc",
        sortname: 'PLAN_CN_ROAD_CODE',
        caption: "&nbsp;&nbsp; RCPLWE Road List",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function () {
            $("#networkCategory #pager").css({ height: '31px' });
            if (($("#RoleCode").val() == 22 || $("#RoleCode").val() == 54)) {
                $("#pager_left").html("<input type='button' style='margin-left:27px' id='idFinalizeNetwork' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'FinalizeCoreNetwork();return false;' value='Finalize " + title + "'/>")
            }
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

    jQuery("#networkCategory").jqGrid('filterToolbar', {
        autosearch: true,
        searchOnEnter: true,
        defaultSearch: false
    });

}

$("#gs_CNCode").attr('placeholder', 'System Id').attr('maxlength', '10');

$("#gs_CNCode").keypress(function (event) {
    //isNumericKeyStroke(e);
    var returnValue = false;
    var keyCode = (event.which) ? event.which : event.keyCode;
    if (((keyCode >= 48) && (keyCode <= 57)) || (keyCode == 46) || (keyCode == 8) || (keyCode == 9) || (keyCode == 37) || (keyCode == 39))// All numerics
    {
        returnValue = true;
    }
    if (event.returnValue)
        event.returnValue = returnValue;
    //alert(returnValue);
    return returnValue;
})

function editNetworkData(urlparameter) {
    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >" + title + " Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divAddForm").load('/CoreNetWork/EditRCPLWE/' + urlparameter, function (data) {
            unblockPage();
            if (data.success == false) {
                alert("Error occurred while processing the request.");
            }
            else {
                $.validator.unobtrusive.parse($('#divAddForm'));
            }
        });
        $('#divAddForm').show('slow');
        $("#divAddForm").css('height', 'auto');
    });
    $("#networkCategory").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');
}

function deleteNetworkData(urlparameter) {
    if (confirm("Are you sure you want to delete RCPLWE Details?")) {
        $.ajax({
            type: 'POST',
            url: '/CoreNetWork/DeleteRCPLWE/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    $("#networkCategory").trigger('reloadGrid');
                    //$('#divAddForm').show();
                }
                else {
                    alert("RCPLWE details can not be deleted as Habitation or File details are added to this road or RCPLWE is used in Proposal.");
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("Error occurred while processing the request.");
            }
        });
    }
    else {
        return false;
    }
}

function editHabitationDetails(urlparameter) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Mapped Habitations</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
            );
    $('#accordion').show('fold', function () {
        blockPage();
        $("#divAddForm").load('/CoreNetWork/ListHabitationsRCPLWE/' + urlparameter, function (data) {
            unblockPage();
            if (data.success == false) {
                alert("Error occurred while processing the request.");
            }
            else {
                $.validator.unobtrusive.parse($('#divAddForm'));
            }
        });
        $('#divAddForm').show('slow');
        $("#divAddForm").css('height', 'auto');
    });
    $("#networkCategory").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');
}

function detailsCoreNetwork(urlparameter) {
    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >" + title + " Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divAddForm").load('/CoreNetWork/DetailsCoreNetworkRCPLWE/' + urlparameter, function (data) {
            unblockPage();
            if (data.success == false) {
                alert("Error occurred while processing the request.");
            }
            else {
                $.validator.unobtrusive.parse($('#divAddForm'));
            }
        });
        $('#divAddForm').show('slow');
        $("#divAddForm").css('height', 'auto');
    });

    $("#networkCategory").jqGrid('setGridState', 'hidden');

    $('#idFilterDiv').trigger('click');
}

function searchDetails() {
    
    $('#networkCategory').setGridParam({
        url: '/CoreNetwork/GetRCPLWEList', datatype: 'json'
    });
    $('#networkCategory').jqGrid("setGridParam", { "postData": { blockCode: $('#ddlBlocks option:selected').val(), roadType: $("#ddlRoute option:selected").val(), categoryCode: $('#ddlCategory option:selected').val(), districtCode: $("#ddlDistricts option:selected").val(), stateCode: $("#ddlStates option:selected").val() } });
    $('#networkCategory').trigger("reloadGrid", [{ page: 1 }]);
}

function UploadFile(urlParameter) {
    jQuery('#networkCategory').jqGrid('setSelection', urlParameter);
    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Upload Files</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {

        $("#divAddForm").load('/CoreNetWork/FileUpload/' + urlParameter, function (data) {
            if (data.success == false) {
                alert("Error occurred while processing the request.");
            }
            else {
                $.validator.unobtrusive.parse($('#divAddForm'));
            }
        });
        $('#divAddForm').show('slow');
        $("#divAddForm").css('height', 'auto');
    });

    $("#networkCategory").jqGrid('setGridState', 'hidden');

    $('#idFilterDiv').trigger('click');
}
function FinalizeCoreNetwork() {

    if ($('#networkCategory').jqGrid('getGridParam', 'selrow')) {

        var myGrid = $('#networkCategory'),
        selectedRowId = myGrid.jqGrid('getGridParam', 'selrow'),
        cellValue = myGrid.jqGrid('getCell', selectedRowId, 'Block');

        detailsCoreNetwork(selectedRowId);
    }
    else {
        alert("Please select the record to finalize the RCPLWE.");
        return false;
    }
}

//new change done by Vikram on 04 Feb 2014 for PMGSY Scheme 2
function MapOtherCandidateRoad(urlparameter) {
    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >DRRP Road Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divAddForm").load('/CoreNetWork/MapOtherCandidateRoadViewRCPLWE/' + urlparameter, function (data) {
            unblockPage();
            if (data.success == false) {
                alert("Error occurred while processing the request.");
            }
            else {
                $.validator.unobtrusive.parse($('#divAddForm'));
            }
        });
        $('#divAddForm').show('slow');
        $("#divAddForm").css('height', 'auto');
    });
    $("#networkCategory").jqGrid('setGridState', 'hidden');
    $('#idFilterDiv').trigger('click');
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