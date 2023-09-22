/*
 * Purpose:- 1) Existing Road.js is used to show Existing road list and for Filter Search
 * 
 */
$(document).ready(function () {

    $("#ddlRoadCategory option:selected").attr("selected", false);

    blockPage();
    checkLockStatus();
    var MAST_BLOCK_CODE = $("#ddlBlocks option:selected").val();
    var MAST_ROAD_CAT_CODE = $("#ddlRoadCategory option:selected").val();

    //display Existing Road List
    LoadExistingRoads(MAST_BLOCK_CODE, MAST_ROAD_CAT_CODE);

    //show/hide filter search 
    $("#idFilterDiv").click(function () {
        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#divFilterForm").toggle("slow");
    });

    //display Existing Road data entry form
    $('#btnAddExistingRoad').click(function () {
        if ($('#RoleCode').val() == 25) {
            if (parseInt($("#ddlStates option:selected").val()) <= 0) {
                alert('Please select state');
                return false;
            }
            if (parseInt($("#ddlDistricts option:selected").val()) <= 0) {
                alert('Please select District');
                return false;
            }
            if (parseInt($("#ddlBlocks option:selected").val()) <= 0) {
                alert('Please select Block');
                return false;
            }
        }
        var mastBlockCode = $("#ddlBlocks").val();

        $("#accordion div").html("");

        $("#accordion h3").html(
                    "<a href='#' style= 'font-size:.9em;' >Add  Existing Roads Details</a>" +
                    '<a href="#" style="float: right;">' +
                    '<img class="ui-icon ui-icon-closethick" onclick="CloseExistingRoadsDetails();" /></a>'
                    );

        $('#accordion').show('fold', function () {
            blockPage();
            //$("#divExistingRoadsForm").load('/ExistingRoads/AddEditExistingRoads/' + mastBlockCode, function () {
            $("#divExistingRoadsForm").load('/ExistingRoads/AddEditExistingRoads/' + $('#ddlStates').val() + "$" + $('#ddlDistricts').val() + "$" + mastBlockCode, function () {
                $.validator.unobtrusive.parse($('#divExistingRoadsForm'));
                unblockPage();
            });

            $('#divExistingRoadsForm').show('slow');
            $("#divExistingRoadsForm").css('height', 'auto');
        });

        $("#tbExistingRoadsList").jqGrid('setGridState', 'hidden');
        //$("#tbExistingRoadsList").setGridParam('hidegrid', false);
    });//end AddForm

    $('#ddlBlocks').change(function () {
        if ($('#RoleCode').val() != 25) {
            checkLockStatus();
        }
    });

    //// display existing road grid by filter search criteria
    $("#btnListExistingRoads").click(function () {
        if ($('#RoleCode').val() == 25 || $('#RoleCode').val() == 65) {//Changes by SAMMED A. PATIL for mordviewuser
            if (parseInt($("#ddlStates option:selected").val()) <= 0) {
                alert('Please select state');
                return false;
            }
            if (parseInt($("#ddlDistricts option:selected").val()) <= 0) {
                alert('Please select District');
                return false;
            }
            if (parseInt($("#ddlBlocks option:selected").val()) <= 0) {
                alert('Please select Block');
                return false;
            }
        }
        blockPage();
        SearchExistingRoadList();
        CloseExistingRoadsDetails();
        unblockPage();
    });//end Search

    $("#gs_MAST_ER_ROAD_NAME").attr('placeholder', 'Enter Road Name to Search');

    $("#gs_ERCode").attr('placeholder', 'System Id').attr('maxlength', '10');

    $("#gs_ERCode").keypress(function (event) {
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



    $(function () {
        $("#accordion").accordion({
            //fillSpace: true,
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });


    $("#ddlStates").change(function () {
        if (parseInt($("#ddlStates option:selected").val()) <= 0) {
            alert('Please select state');
            return false;
        }
        else {
            FillInCascadeDropdown({ userType: $("#ddlStates").find(":selected").val() },
                       "#ddlDistricts", "/CoreNetwork/GetDistrictByState?stateCode=" + $('#ddlStates option:selected').val());
        }
    });

    $("#ddlDistricts").change(function () {
        FillInCascadeDropdown({ userType: $("#ddlDistricts").find(":selected").val() },
                   "#ddlBlocks", "/CoreNetwork/GetBlocksByDistrict?districtCode=" + $('#ddlDistricts option:selected').val());
    });



    $("#dvShiftDRRPBlockPMGSY3").dialog({
        autoOpen: false,
        height: 'auto',
        width: "450",
        modal: true,
        title: 'Shift DRRP Block'
    });

});

//show existing road filter search
function showFilter() {
    if ($('#divFilterForm').is(":hidden")) {
        $("#divFilterForm").show("slow");
        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s");
    }
}

//this function is used to close Data entry form and show the Grid
function CloseExistingRoadsDetails() {
    $('#accordion').hide('slow');
    $('#divExistingRoadsForm').hide('slow');
    $("#tbExistingRoadsList").jqGrid('setGridState', 'visible');
    $("#tbExistingRoadsList").trigger('reloadGrid');
    showFilter();
}

//display Existing Road list
function LoadExistingRoads(MAST_BLOCK_CODE, MAST_ROAD_CAT_CODE) {

    jQuery("#tbExistingRoadsList").jqGrid('GridUnload');

    jQuery("#tbExistingRoadsList").jqGrid({
        url: '/ExistingRoads/GetExistingRoadsPMGSY3List/',
        datatype: "json",
        mtype: "GET",
        colNames: ['Existing Road System Id', 'Road Category', 'Road Number', "Road Name", 'Road Type', 'Road Length [in Km]', "Road Owner", "Included in Core Network", "CD Works", "Surface Types", "Habitations", "Traffic Intensity", "CBR Value","Shift DRRP", "View", "PMGSY1 DRRP Roads", "Edit", "Delete"],
        colModel: [
                        { name: 'ERCode', index: 'ERCode', height: 'auto', width: 60, align: "left", sortable: true, search: true },
                        { name: 'MAST_ROAD_SHORT_DESC', index: 'MAST_ROAD_SHORT_DESC', width: 60, sortable: true, align: "left", search: false }, //New
                        { name: 'MAST_ER_ROAD_NUMBER', index: 'MAST_ER_ROAD_NUMBER', width: 90, sortable: true, align: "left", search: false },
                        { name: 'MAST_ER_ROAD_NAME', index: 'MAST_ER_ROAD_NAME', width: 200, sortable: true, align: "left", search: true },
                        { name: 'MAST_ER_ROAD_TYPE', index: 'MAST_ER_ROAD_TYPE', width: 90, sortable: true, align: "left", search: false }, //New
                        { name: 'MAST_ER_ROAD_LENGTH', index: 'MAST_ER_ROAD_LENGTH', width: 100, sortable: true, align: "left", search: false },
                        { name: 'MAST_ER_ROAD_OWNER', index: 'MAST_ER_ROAD_OWNER', width: 80, sortable: true, align: "left", search: false },
                        { name: 'MAST_CORE_NETWORK', index: 'MAST_CORE_NETWORK', width: 97, sortable: true, align: "center", search: false, hidden: true },
                        { name: 'a', width: 70, sortable: false, resize: false, align: "center", sortable: false, search: false, hidden: ($("#RoleCode").val() == 22 ? false : true) },
                        { name: 'SurfaceTypes', width: 70, sortable: false, resize: false, align: "center", sortable: false, search: false, hidden: ($("#RoleCode").val() == 22 ? false : true) },
                        { name: 'HabitationsMapped', width: 70, sortable: false, resize: false, falign: "center", sortable: false, search: false, hidden: ($("#RoleCode").val() == 22 ? false : true) },
                        { name: 'TrafficIntensity', width: 70, sortable: false, resize: false, align: "center", sortable: false, search: false, hidden: ($("#RoleCode").val() == 22 ? false : true) },
                        { name: 'CBRValue', width: 70, sortable: false, resize: false, align: "center", sortable: false, search: false, hidden: ($("#RoleCode").val() == 22 ? false : true) },
                      // Shift DRRP to new Block and District

                        { name: 'ShiftDRRPPMGSY3', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false, hidden: ($("#StateCodeInSession").val() == 15 || $("#StateCodeInSession").val() == 37) ? false : true },

                        { name: 'ShowDetails', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false },
                        { name: 'MapDRRP', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false, hidden: (($("#PMGSYScheme").val() == "2" && ($("#RoleCode").val() == 25 || $("#RoleCode").val() == 22)) ? false : true) /*hidden: true*/ },
                        { name: 'Edit', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false, /*hidden: ($("#RoleCode").val() == 22 ? false : true)*/ hidden: ($("#RoleCode").val() == 65) ? true : false }, /*Changes by SAMMED A. PATIL for mordviewuser*/
                        //{ name: 'Delete', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false, hidden: (($("#RoleCode").val() == 22 || $("#RoleCode").val() == 25) ? ($("#PMGSYScheme").val() == "2" ? true : false) : true) },
                        {
                            name: 'Delete', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false,
                            hidden: (($("#RoleCode").val() == 25) ? ($("#PMGSYScheme").val() == "2" ? true : false) :($("#RoleCode").val() == 22) ? false : true)
                        },
        ],
        postData: { "MAST_BLOCK_CODE": MAST_BLOCK_CODE, "MAST_ROAD_CAT_CODE": MAST_ROAD_CAT_CODE, districtCode: $("#ddlDistricts option:selected").val(), stateCode: $("#ddlStates option:selected").val() },
        pager: jQuery('#dvExistingRoadsListPager'),
        rowNum: 10,
        sortorder: "desc",
        sortname: 'MAST_ER_ROAD_CODE',
        rowList: [5, 10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Existing Roads List for PMGSY3",
        height: 'auto',
        autowidth: true,
        cmTemplate: { title: false },
        rownumbers: true,
        loadComplete: function () {
            $("#tbExistingRoadsList #dvExistingRoadsListPager").css({ height: '31px' });
            if ($("#RoleCode").val() == 22) {
                $("#dvExistingRoadsListPager_left").html("<input type='button' style='margin-left:27px' id='idFinalizeExistingRoad' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'RedirectFinalizeExistingRoad();return false;' value='Finalize Existing Road'/>");
            }
            unblockPage();
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Session Timeout !!!");
                window.location.href = "/Login/LogIn";
            }
        }

    }); //end of grid

    $("#tbExistingRoadsList").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true });

}

//Existing road grid fromat column
function FormatColumnCDWorks(cellvalue, options, rowObject) {

    if (cellvalue == "") {
        return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-locked ui-align-center' title='CD Works'></span></center>";

    } else {
        return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-plusthick ui-align-center' title='CD Works' onClick ='CDWorks(\"" + cellvalue.toString() + "\");'></span></center>";
    }
}
//Existing road grid fromat column
function FormatColumnSurfaceTypes(cellvalue, options, rowObject) {

    if (cellvalue == "") {
        return "<center><span  style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-locked ui-align-center' title='Surface Types'></span></center>";

    } else {
        return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-plusthick ui-align-center' title='Surface Types' onClick ='SurfaceTypes(\"" + cellvalue.toString() + "\");'></span></center>";
    }
}
//Existing road grid fromat column
function FormatColumnHabitationsMapped(cellvalue, options, rowObject) {

    if (cellvalue == "") {
        return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-locked ui-align-center' title='Mapped Habitations'></span></center>";

    } else {
        return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-plusthick ui-align-center' title='Mapped Habitations ' onClick ='HabitationsMapped(\"" + cellvalue.toString() + "\");'></span></center>";
    }
}
//Existing road grid fromat column
function FormatColumnTrafficIntensity(cellvalue, options, rowObject) {

    if (cellvalue == "") {
        return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-locked ui-align-center' title='Traffic Intensity'></span></center>";

    } else {
        return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-plusthick ui-align-center' title='Traffic Intensity' onClick ='TrafficIntensity(\"" + cellvalue.toString() + "\");'></span></center>";
    }
}
//Existing road grid fromat column
function FormatColumnCBRValue(cellvalue, options, rowObject) {

    if (cellvalue == "") {
        return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-locked ui-align-center' title='CBR Value'></span></center>";

    } else {
        return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-plusthick ui-align-center' title='CBR Value' onClick ='CBRValue(\"" + cellvalue.toString() + "\");'></span></center>";
    }

}
//Existing road grid fromat column
function FormatColumnShowDetails(cellvalue, options, rowObject) {
    return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-zoomin ui-align-center' title='Click here to get full details of road' onClick ='ShowDetails(\"" + cellvalue.toString() + "\");'></span></center>";
}
//Existing road grid fromat column
function FormatColumnEdit(cellvalue, options, rowObject) {
    if (cellvalue == "") {
        return "<center><span style='border-color:white;cursor:pointer;'class='ui-icon ui-icon-locked ui-align-center' title='Click here to Edit Existing Road Details'></span></center>";

    } else {
        return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-pencil ui-align-center' title='Click here to Edit Existing Road Details' onClick ='EditDetails(\"" + cellvalue.toString() + "\");'></span></center>";
    }
}
//Existing road grid fromat column
function FormatColumnDelete(cellvalue, options, rowObject) {

    if (cellvalue == "") {
        return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-locked ui-align-center'></span></center>";

    } else {
        return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-trash ui-align-center' title='Click here to delete Existing Road Details' onClick ='DeleteDetails(\"" + cellvalue.toString() + "\");'></span></center>";
    }
}

//displays Cdwork Data entry form and cdWork Grid
function CDWorks(urlparamater) {
    jQuery('#tbExistingRoadsList').jqGrid('setSelection', urlparamater);

    $("#accordion div").html("");

    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >CD Works Details PMGSY3</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseExistingRoadsDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();

        //$("#divExistingRoadsForm").load('/ExistingRoads/CdWorkAddEdit/' + urlparamater, function () {
        $("#divExistingRoadsForm").load('/ExistingRoads/ListCdWorksPMGSY3?id=' + urlparamater, function () {
            $.validator.unobtrusive.parse($('#frmCdWorks'));

            unblockPage();
        });
        $("#tbExistingRoadsList").jqGrid('setGridState', 'hidden');
        $('#divExistingRoadsForm').show('slow');
        $("#divExistingRoadsForm").css('height', 'auto');
    });
}

//displays SurfaceTypes Data entry form and SurfaceTypes Grid
function SurfaceTypes(urlparamater) {

    jQuery('#tbExistingRoadsList').jqGrid('setSelection', urlparamater);

    $("#accordion div").html("");

    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Surface Details PMGSY3</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseExistingRoadsDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();

        //$("#divExistingRoadsForm").load('/ExistingRoads/SurfaceAddEdit/' + urlparamater, function () {
        $("#divExistingRoadsForm").load('/ExistingRoads/ListSurfaceTypesPMGSY3?id=' + urlparamater, function () {
            $.validator.unobtrusive.parse($('#frmSurfaceType'));
            unblockPage();
        });

        $.validator.unobtrusive.parse($('#frmSurfaceType'));
        unblockPage();
        $("#tbExistingRoadsList").jqGrid('setGridState', 'hidden');
        $('#divExistingRoadsForm').show('slow');
        $("#divExistingRoadsForm").css('height', 'auto');
    });


}

//displays HabitationsMapped Data entry form and HabitationsMapped Grid
function HabitationsMapped(urlparameter) {
    // alert("Habitations Mapped");

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Map Habitations PMGSY3</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseExistingRoadsDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        //$("#divAddForm").load('/CoreNetwork/DetailsCoreNetwork?id=' + urlparameter, function () {
        $("#divExistingRoadsForm").load('/ExistingRoads/ListHabitationsPMGSY3/' + urlparameter, function () {
            $.validator.unobtrusive.parse($('#divExistingRoadsForm'));
            unblockPage();
        });
        $('#divExistingRoadsForm').show('slow');
        $("#divExistingRoadsForm").css('height', 'auto');
    });

    $("#tbExistingRoadsList").jqGrid('setGridState', 'hidden');

    $('#idFilterDiv').trigger('click');


}

//displays TrafficIntensity Data entry form and TrafficIntensity Grid
function TrafficIntensity(urlparamater) {

    jQuery('#tbExistingRoadsList').jqGrid('setSelection', urlparamater);

    $("#accordion div").html("");

    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Traffic Intensity Details PMGSY3</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseExistingRoadsDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();

        $("#divExistingRoadsForm").load('/ExistingRoads/ListTrafficIntensityPMGSY3/' + urlparamater, function () {

            $.validator.unobtrusive.parse($('#frmTrafficIntensity'));
            unblockPage();
        });
        $("#tbExistingRoadsList").jqGrid('setGridState', 'hidden');
        $('#divExistingRoadsForm').show('slow');
        $("#divExistingRoadsForm").css('height', 'auto');
    });

}

//displays CBRValue Data entry form and CBRValue Grid
function CBRValue(urlparamater) {

    jQuery('#tbExistingRoadsList').jqGrid('setSelection', urlparamater);

    $("#accordion div").html("");

    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >CBR Details PMGSY3</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseExistingRoadsDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();

        $("#divExistingRoadsForm").load('/ExistingRoads/ListCBRPMGSY3/' + urlparamater, function () {
            $.validator.unobtrusive.parse($('#frmCBRValue'));
            unblockPage();
        });
        $("#tbExistingRoadsList").jqGrid('setGridState', 'hidden');
        $('#divExistingRoadsForm').show('slow');
        $("#divExistingRoadsForm").css('height', 'auto');
    });
}


// Shift DRRP 27 Jan 2021
function ShiftDRRPToNewBlockAndDistrictPMGSY3(parameter) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $('#dvShiftDRRPBlockPMGSY3').empty();
    $("#dvShiftDRRPBlockPMGSY3").load("/ExistingRoads/ShiftDRRPBlockPMGSY3?id=" + parameter, function () {

        $("#dvShiftDRRPBlockPMGSY3").dialog('open');
        $.unblockUI();
    })

}



// diplay Existing Road Data entery form in Edit mode
function EditDetails(id) {

    //$.validator.unobtrusive.parse($('frmCreateExistingRoad'));

    $("#accordion div").html("");
    $("#accordion h3").html(
                   "<a href='#' style= 'font-size:.9em;' >Edit  Existing Road Details</a>" +
                   '<a href="#" style="float: right;">' +
                   '<img class="ui-icon ui-icon-closethick" onclick="CloseExistingRoadsDetails();" /></a>'
                   );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divExistingRoadsForm").load('/ExistingRoads/EditExistingRoads/' + id, function () {
            $.validator.unobtrusive.parse($('#divExistingRoadsForm'));
            $("#MAST_ER_ROAD_NAME").focus();

            unblockPage();
        });
        $("#divExistingRoadsForm").show("slow");
        $("#divExistingRoadsForm").css('height', 'auto');
    });

    $("#tbExistingRoadsList").jqGrid('setGridState', 'hidden');

}

//show the Existing Road Detail information
function ShowDetails(id) {
    $("#accordion div").html("");
    $("#accordion h3").html(
                   "<a href='#' style= 'font-size:.9em;' >Existing Roads Details</a>" +
                   '<a href="#" style="float: right;">' +
                   '<img class="ui-icon ui-icon-closethick" onclick="CloseExistingRoadsDetails();" /></a>'
                   );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divExistingRoadsForm").load('/ExistingRoads/ViewExistingRoadsPMGSY3/' + id, function () {
            $.validator.unobtrusive.parse($('#divExistingRoadsForm'));
            unblockPage();
        });
        $("#divExistingRoadsForm").show("slow");
        $("#divExistingRoadsForm").css('height', 'auto');
    });

    $("#tbExistingRoadsList").jqGrid('setGridState', 'hidden');
}

// diplay Existing Road Data entery form in Edit mode
function DeleteDetails(id) {

    // var MAST_BLOCK_CODE = $("#ddlBlocks option:selected").val();
    // var MAST_ROAD_CAT_CODE = $("#ddlRoadCategory option:selected").val();


    if (confirm("Are you sure to delete existing road details ? ")) {
        $.ajax({
            url: '/ExistingRoads/DeleteExistingRoads/' + id,
            type: "POST",
            cache: false,
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            },
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    $("#tbExistingRoadsList").trigger('reloadGrid');
                    //$('#tbExistingRoadsList').jqGrid('GridUnload');
                    // LoadExistingRoads(MAST_BLOCK_CODE, MAST_ROAD_CAT_CODE);
                }
                else {
                    if (response.errorMessage != "" || response.errorMessage != undefined || response.errorMessage != null) {
                        alert(response.message)
                    }
                    else {
                        alert("Error Occured while processing your request.");
                    }

                }
                unblockPage();
            }
        });
    } else {
        return;
    }
}

//Click event of Finalize Existing Road
function RedirectFinalizeExistingRoad() {
    if ($('#tbExistingRoadsList').jqGrid('getGridParam', 'selrow')) {

        var myGrid = $('#tbExistingRoadsList');
        selectedRowId = myGrid.jqGrid('getGridParam', 'selrow'); //get selected recored id
        //cellValue = myGrid.jqGrid('getCell', selectedRowId, 'MAST_ER_ROAD_NUMBER'); //get cell value
        ShowExistingRoadDetails(selectedRowId);
    }
    else {
        alert("Please Select the Existing Road recored to finalize it.");
        return false;
    }
}

//show the Details of Existing Road 
function ShowExistingRoadDetails(id) {

    $("#accordion div").html("");
    $("#accordion h3").html(
                   "<a href='#' style= 'font-size:.9em;' >Existing Roads Details</a>" +
                   '<a href="#" style="float: right;">' +
                   '<img class="ui-icon ui-icon-closethick" onclick="CloseExistingRoadsDetails();" /></a>'
                   );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divExistingRoadsForm").load('/ExistingRoads/FinalizeDRRPPMGSY3/' + id, function () {
            $.validator.unobtrusive.parse($('#divExistingRoadsForm'));
            unblockPage();
        });
        $("#divExistingRoadsForm").show("slow");
        $("#divExistingRoadsForm").css('height', 'auto');
    });

    $("#tbExistingRoadsList").jqGrid('setGridState', 'hidden');
}


function SearchExistingRoadList() {
    var MAST_BLOCK_CODE = $("#ddlBlocks option:selected").val();
    var MAST_ROAD_CAT_CODE = $("#ddlRoadCategory option:selected").val();

    $('#tbExistingRoadsList').setGridParam({
        url: '/ExistingRoads/GetExistingRoadsPMGSY3List', datatype: 'json'
    });
    $('#tbExistingRoadsList').jqGrid("setGridParam", { "postData": { MAST_BLOCK_CODE: MAST_BLOCK_CODE, MAST_ROAD_CAT_CODE: MAST_ROAD_CAT_CODE, districtCode: $("#ddlDistricts option:selected").val(), stateCode: $("#ddlStates option:selected").val() } });
    $('#tbExistingRoadsList').trigger("reloadGrid", [{ page: 1 }]);
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

//displays Cdwork Data entry form and cdWork Grid
function MapDRRPPMGSY1(urlparamater) {
    //alert(urlparamater);
    jQuery('#tbExistingRoadsList').jqGrid('setSelection', urlparamater);

    $("#accordion div").html("");

    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Map DRRP Road for PMGSY-I</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseExistingRoadsDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();

        //$("#divExistingRoadsForm").load('/ExistingRoads/CdWorkAddEdit/' + urlparamater, function () {
        $("#divExistingRoadsForm").load('/ExistingRoads/MapDRRPLayout/' + urlparamater, function () {
            //$.validator.unobtrusive.parse($('#frmCdWorks'));

            unblockPage();
        });
        $("#tbExistingRoadsList").jqGrid('setGridState', 'hidden');
        $('#divExistingRoadsForm').show('slow');
        $("#divExistingRoadsForm").css('height', 'auto');
    });
}

//
function checkLockStatus() {

    $.ajax({

        type: 'POST',
        url: '/ExistingRoads/CheckLockStatusPMGSY3/',
        async: false,
        cache: false,
        data: { blockCode: $('#ddlBlocks option:selected').val(), },
        success: function (data) {
            if (data.status == false) {
                $('#btnAddExistingRoad').hide('slow');
            }
            else {
                $('#btnAddExistingRoad').show('slow');
            }
        }


    });
}