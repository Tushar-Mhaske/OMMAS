$(document).ready(function () {

    LoadFoundationGrid();

});

// to load execution details
function LoadFoundationGrid() {
    //alert($("#EncrProposalCode").val());
    jQuery("#tbFoundationList").jqGrid('GridUnload');
    jQuery("#tbFoundationList").jqGrid({
        url: '/Execution/GetFoundationDetailsList',
        datatype: "json",
        mtype: "GET",
        //postData: { yearCode: $("#ddlImsYear option:selected").val(), blockCode: $('#ddlMastBlockCode option:selected').val(), batchCode: $("#ddlImsBatch option:selected").val(), streamCode: $("#ddlImsStreams option:selected").val(), proposalCode: $("#ddlImsProposalTypes").val() },
        postData: { proposalCode: $("#EncrProposalCode").val() },
        colNames: ['Month', 'Year', 'Execution Item', 'Item Progress', 'Ground Progress', 'First Floor Progress', 'Second Floor Progress', 'Third Floor Progress', 'Covered Parking', 'Approach Road', 'Edit', 'Delete'
        ],
        colModel: [
                            { name: 'Month', index: 'Month', height: 'auto', width: 80, align: "left", search: false },
                            { name: 'Year', index: 'Year', height: 'auto', width: 80, align: "left", search: false },
                            { name: 'ExecItem', index: 'ExecItem', height: 'auto', width: 80, align: "left", search: false },
                            { name: 'ItemProgress', index: 'ItemProgress', width: 70, height: 'auto', sortable: true, align: "center" },
                            { name: 'GroundFloor', index: 'GroundFloor', width: 70, height: 'auto', sortable: true, align: "center", hidden: true },
                            { name: 'FirstFloor', index: 'FirstFloor', width: 70, height: 'auto', sortable: true, align: "center", hidden: true },
                            { name: 'SecondFloor', index: 'SecondFloor', width: 70, height: 'auto', sortable: true, align: "center", hidden: true },
                            { name: 'ThirdFloor', index: 'ThirdFloor', height: 'auto', width: 80, align: "center", search: false, hidden: true },
                            { name: 'CoveredParking', index: 'CoveredParking', height: 'auto', width: 250, align: "left", search: true, hidden: true },
                            { name: 'ApproachRoad', index: 'ApproachRoad', height: 'auto', width: 100, align: "right", search: true, hidden: true },
                            { name: 'a', width: 50, sortable: false, resize: false, height: 'auto', align: "center", search: false },
                            { name: 'b', width: 50, sortable: false, resize: false, height: 'auto', align: "center", search: false },
                            //{ name: 'c', width: 50, sortable: false, resize: false, formatter: FormatColumn3, align: "center", search: false },
        ],
        pager: jQuery('#pagerFoundation').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "Month",
        sortorder: "desc",
        caption: "&nbsp;&nbsp; Foundation Details List",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function (data) {
            $("#tbFoundationList #pagerFoundation").css({ height: '40px' });
            //$("#pagerFoundation_left").html("<label style='margin-left:8%;'><b>Note: </b>Financial Progress entry through Technical Module has been restricted.<label/>")
            //$("#pagerFoundation_left").html("<input type='button' style='margin-left:8%' id='idAddPhysicaRoad' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddPhysicalRoadProgress(" + + ");return false;' value='Add Road Progress'/>")

            $("#pagerFoundation_left").html("<input type='button' style='margin-left:27px' id='btnAddImage' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddFoundationProgressDetails(); return false;' value='Add Foundation'/>")
            $('#tbFoundationList').jqGrid('setGridWidth', '1200');//abhinav
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

function AddFoundationProgressDetails() {

    $.ajax({
        type: 'GET',
        url: '/Execution/FoundationLayout/' + $("#EncrProposalCode").val(),
        //data: $("#frmFoundationFoundation").serialize(),
        async: false,
        cache: false,
        success: function (data) {
            $("#dvEditFoundation").html('');
            $("#dvEditFoundation").html(data);
            if (data.success) {
                alert(data.message);
                // $("#tbCDWorksList").trigger('reloadGrid');
                //LoadFoundationsGrid();
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


function EditFoundationProgress(urlparameter) {

    $.ajax({
        type: 'GET',
        url: '/Execution/FoundationLayout/' + urlparameter,
        //data: $("#frmFoundation").serialize(),
        async: false,
        cache: false,
        success: function (data) {
            $("#dvEditFoundation").html('');
            $("#dvEditFoundation").html(data);
            if (data.success) {
                alert(data.message);
                // $("#tbCDWorksList").trigger('reloadGrid');
                //LoadFoundationGrid();
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

function DeleteFoundationProgress(urlParam) {
    if (confirm("Are you sure you want to delete Foundation details?")) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: "/Execution/DeleteFoundationDetails/" + urlParam,
            type: "POST",
            dataType: "json",
            data: { __RequestVerificationToken: $("#frmListFoundation input[name=__RequestVerificationToken]").val() },
            success: function (data) {
                if (data.success) {
                    if (data.success) {
                        alert(data.message);
                        // $("#tbCDWorksList").trigger('reloadGrid');
                        //$("#btnResetCDWorksDetails").trigger('click');
                        //$("#divAddCDWorks").html('');

                        $("#dvEditFoundation").html('');
                        LoadFoundationGrid();
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