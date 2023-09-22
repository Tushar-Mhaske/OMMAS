$(document).ready(function () {

    LoadSuperstructureGrid();

});

// to load execution details
function LoadSuperstructureGrid() {
    //alert($("#EncrProposalCode").val());
    jQuery("#tbSuperstructureList").jqGrid('GridUnload');
    jQuery("#tbSuperstructureList").jqGrid({
        url: '/Execution/GetSuperstructureDetailsList',
        datatype: "json",
        mtype: "GET",
        //postData: { yearCode: $("#ddlImsYear option:selected").val(), blockCode: $('#ddlMastBlockCode option:selected').val(), batchCode: $("#ddlImsBatch option:selected").val(), streamCode: $("#ddlImsStreams option:selected").val(), proposalCode: $("#ddlImsProposalTypes").val() },
        postData: { proposalCode: $("#EncrProposalCode").val() },
        colNames: ['Month', 'Year', 'Execution Item', 'Item Progress', 'Ground Progress', 'First Floor Progress', 'Second Floor Progress', 'Third Floor Progress', 'Covered Parking', 'Approach Road', 'Edit', 'Delete'
        ],
        colModel: [
                            { name: 'Month', index: 'Month', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'Year', index: 'Year', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'ExecItem', index: 'ExecItem', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'ItemProgress', index: 'ItemProgress', height: 'auto', width: 80, sortable: true, align: "center", hidden: true },
                            { name: 'GroundFloor', index: 'GroundFloor', height: 'auto', width: 80, sortable: true, align: "center" },
                            { name: 'FirstFloor', index: 'FirstFloor', height: 'auto', width: 80, sortable: true, align: "center" },
                            { name: 'SecondFloor', index: 'SecondFloor', height: 'auto', width: 80, sortable: true, align: "center" },
                            { name: 'ThirdFloor', index: 'ThirdFloor', height: 'auto', width: 80, align: "center", search: false },
                            { name: 'CoveredParking', index: 'CoveredParking', height: 'auto', width: 80, align: "center", search: true },
                            { name: 'ApproachRoad', index: 'ApproachRoad', height: 'auto', width: 100, align: "center", search: true },
                            { name: 'a', width: 50, sortable: false, resize: false, height: 'auto', align: "center", search: false, hidden:true },
                            { name: 'b', width: 50, sortable: false, resize: false, height: 'auto', align: "center", search: false },
                            //{ name: 'c', width: 50, sortable: false, resize: false, formatter: FormatColumn3, align: "center", search: false },
        ],
        pager: jQuery('#pagerSuperstructure').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "Month",
        sortorder: "desc",
        caption: "&nbsp;&nbsp; Superstructure Details List",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function (data) {
            $("#tbSuperstructureList #pagerSuperstructure").css({ height: '40px' });
            //$("#pagerSuperstructure_left").html("<label style='margin-left:8%;'><b>Note: </b>Financial Progress entry through Technical Module has been restricted.<label/>")
            //$("#pagerSuperstructure_left").html("<input type='button' style='margin-left:8%' id='idAddPhysicaRoad' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddPhysicalRoadProgress(" + + ");return false;' value='Add Road Progress'/>")

            $("#pagerSuperstructure_left").html("<input type='button' style='margin-left:27px' id='btnAddImage' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddSuperstructureProgressDetails(); return false;' value='Add Superstructure'/>")
            $('#tbSuperstructureList').jqGrid('setGridWidth', '1200');//abhinav
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

function AddSuperstructureProgressDetails() {

    $.ajax({
        type: 'GET',
        url: '/Execution/SuperstructureLayout/' + $("#EncrProposalCode").val(),
        //data: $("#frmSuperstructureSuperstructure").serialize(),
        async: false,
        cache: false,
        success: function (data) {
            $("#dvEditSuperstructure").html('');
            $("#dvEditSuperstructure").html(data);
            if (data.success) {
                alert(data.message);
                // $("#tbCDWorksList").trigger('reloadGrid');
                //LoadSuperstructuresGrid();
                //$("#btnResetCDWorksDetails").trigger('click');
                $("#tbExecutionList").trigger('reloadGrid');
            }
            else {
                //alert(data.message);
                $("#divError").show();
                $("#divError").html('<strong>Alert : </strong>' + data.message);
            }
        },
        error: function () {
            alert("Request can not be processed at this time.");
        }
    })
}

function EditSuperstructureProgress(urlparameter) {

    $.ajax({
        type: 'GET',
        url: '/Execution/SuperstructureLayout/' + urlparameter,
        //data: $("#frmSuperstructure").serialize(),
        async: false,
        cache: false,
        success: function (data) {
            $("#dvEditSuperstructure").html('');
            $("#dvEditSuperstructure").html(data);
            if (data.success) {
                alert(data.message);
                // $("#tbCDWorksList").trigger('reloadGrid');
                //LoadSuperstructureGrid();
                //$("#btnResetCDWorksDetails").trigger('click');
                $("#tbExecutionList").trigger('reloadGrid');
            }
            else {
                //alert(data.message);
                $("#divError").show();
                $("#divError").html('<strong>Alert : </strong>' + data.message);
            }
        },
        error: function () {
            alert("Request can not be processed at this time.");
        }
    })
}

function DeleteSuperstructureProgress(urlParam) {
    if (confirm("Are you sure you want to delete Superstructure details?")) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: "/Execution/DeleteSuperstructureDetails/" + urlParam,
            type: "POST",
            dataType: "json",
            data: { __RequestVerificationToken: $("#frmListSuperstructure input[name=__RequestVerificationToken]").val() },
            success: function (data) {
                if (data.success) {
                    if (data.success) {
                        alert(data.message);
                        // $("#tbCDWorksList").trigger('reloadGrid');
                        //$("#btnResetCDWorksDetails").trigger('click');
                        //$("#divAddCDWorks").html('');

                        $("#dvEditSuperstructure").html('');
                        LoadSuperstructureGrid();
                        $("#tbExecutionList").trigger('reloadGrid');
                    }
                    else {
                        alert(data.message);
                        $("#divError").show();
                        $("#divError").html('<strong>Alert : </strong>' + data.message);
                    }
                }
                else {
                    alert(data.message);
                }
                $.unblockUI();
            },
            error: function (xht, ajaxOptions, throwError) {
                alert('Error occured');
                $.unblockUI();
            }
        });
    }
    else {
        return false;
    }
}