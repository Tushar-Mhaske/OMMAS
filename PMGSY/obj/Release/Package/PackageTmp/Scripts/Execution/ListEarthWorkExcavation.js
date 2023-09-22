$(document).ready(function () {

    LoadEarthworksGrid();

});

// to load execution details
function LoadEarthworksGrid() {
    //alert($("#EncrProposalCode").val());
    jQuery("#tbExcavationList").jqGrid('GridUnload');
    jQuery("#tbExcavationList").jqGrid({
        url: '/Execution/GetEarthworkExcavationDetailsList',
        datatype: "json",
        mtype: "GET",
        //postData: { yearCode: $("#ddlImsYear option:selected").val(), blockCode: $('#ddlMastBlockCode option:selected').val(), batchCode: $("#ddlImsBatch option:selected").val(), streamCode: $("#ddlImsStreams option:selected").val(), proposalCode: $("#ddlImsProposalTypes").val() },
        postData: { proposalCode: $("#EncrProposalCode").val() },
        colNames: ['Month', 'Year', 'Execution Item', 'Item Progress', 'Ground Progress', 'First Floor Progress', 'Second Floor Progress', 'Third Floor Progress', 'Covered Parking', 'Approach Road', 'Edit', 'Delete'
        ],
        colModel: [
                            { name: 'Month', index: 'Month', height: 'auto', width: 80, align: "left", search: false },
                            { name: 'Year', index: 'Year', height: 'auto', width: 80, align: "left", search: false },
                            { name: 'ExecItem', index: 'ExecItem', height: 'auto', width: 80, align: "left", search: false, hidden: true },
                            { name: 'ItemProgress', index: 'ItemProgress', height: 'auto', width: 70, sortable: true, align: "center" },
                            { name: 'GroundFloor', index: 'GroundFloor', height: 'auto', width: 70, sortable: true, align: "center", hidden: true },
                            { name: 'FirstFloor', index: 'FirstFloor', height: 'auto', width: 70, sortable: true, align: "center", hidden: true },
                            { name: 'SecondFloor', index: 'SecondFloor', height: 'auto', width: 70, sortable: true, align: "center", hidden: true },
                            { name: 'ThirdFloor', index: 'ThirdFloor', height: 'auto', width: 80, align: "center", search: false, hidden: true },
                            { name: 'CoveredParking', index: 'CoveredParking', height: 'auto', width: 250, align: "left", search: true, hidden: true },
                            { name: 'ApproachRoad', index: 'ApproachRoad', height: 'auto', width: 100, align: "right", search: true, hidden: true },
                            { name: 'a', width: 50, sortable: false, resize: false, height: 'auto', align: "center", search: false },
                            { name: 'b', width: 50, sortable: false, resize: false, height: 'auto', align: "center", search: false },
                            //{ name: 'c', width: 50, sortable: false, resize: false, formatter: FormatColumn3, align: "center", search: false },
        ],
        pager: jQuery('#pagerExcavation').width(20),
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "Month",
        sortorder: "desc",
        caption: "&nbsp;&nbsp; Earthworks Excavation PCC Details List",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function (data) {
            $("#tbExcavationList #pagerExcavation").css({ height: '40px' });
            //$("#pagerExcavation_left").html("<label style='margin-left:8%;'><b>Note: </b>Financial Progress entry through Technical Module has been restricted.<label/>")
            //$("#pagerExcavation_left").html("<input type='button' style='margin-left:8%' id='idAddPhysicaRoad' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddPhysicalRoadProgress(" + + ");return false;' value='Add Road Progress'/>")

            $("#pagerExcavation_left").html("<input type='button' style='margin-left:27px' id='btnAddImage' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddEarthworkDetails(); return false;' value='Add Earthwork Excavation PCC'/>")
            $('#tbExcavationList').jqGrid('setGridWidth', '1200');//abhinav
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

function AddEarthworkDetails() {

    $.ajax({
        type: 'GET',
        url: '/Execution/EarthWorkExcavationLayout/' + $("#EncrProposalCode").val(),
        //data: $("#frmEarthworkExcavation").serialize(),
        async: false,
        cache: false,
        success: function (data) {
            $("#AdEditEarthwork").html('');
            $("#AdEditEarthwork").html(data);
            if (data.success) {
                alert(data.message);
                // $("#tbCDWorksList").trigger('reloadGrid');
                //LoadEarthworksGrid();
                //$("#btnResetCDWorksDetails").trigger('click');

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


function EditExcavationProgress(urlparameter) {

    $.ajax({
        type: 'GET',
        url: '/Execution/EarthWorkExcavationLayout/' + urlparameter,
        //data: $("#frmEarthworkExcavation").serialize(),
        async: false,
        cache: false,
        success: function (data) {
            $("#AdEditEarthwork").html('');
            $("#AdEditEarthwork").html(data);
            if (data.success) {
                alert(data.message);
                // $("#tbCDWorksList").trigger('reloadGrid');
                //LoadEarthworksGrid();
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

function DeleteExcavationProgress(urlParam) {
    if (confirm("Are you sure you want to delete Earthwork details?")) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: "/Execution/DeleteEarthworkExcavationDetails/" + urlParam,
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    if (data.success) {
                        alert(data.message);
                        // $("#tbCDWorksList").trigger('reloadGrid');
                        //$("#btnResetCDWorksDetails").trigger('click');
                        //$("#divAddCDWorks").html('');

                        $("#AdEditEarthwork").html('');
                        LoadEarthworksGrid();
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