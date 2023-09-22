/*
 * Purpose:- 1) Existing Road.js is used to show Existing road list and for Filter Search
 * 
 */
$(document).ready(function () {

    blockPage();

    var MAST_BLOCK_CODE = $("#ddlBlocks option:selected").val();
    var MAST_ROAD_CAT_CODE = $("#ddlRoadCategory option:selected").val();

    //display Existing Road List
    //LoadExistingRoads(MAST_BLOCK_CODE, MAST_ROAD_CAT_CODE);

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
        if ($('#RoleCode').val() == 25 || $('#RoleCode').val() == 65 || $('#RoleCode').val() == 36) {//Changes by SAMMED A. PATIL for mordviewuser
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
        LoadExistingRoads($("#ddlBlocks option:selected").val(), $("#ddlRoadCategory option:selected").val());
        //SearchExistingRoadList();
        CloseExistingRoadsDetails();
        unblockPage();
    });//end Search

    $("#gs_MAST_ER_ROAD_NAME").attr('placeholder', 'Enter Road Name to Search');


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
    
    $("#dvShiftVillage").dialog({
        autoOpen: false,
        height: 'auto',
        width: "450",
        modal: true,
        title: 'Shift Details'
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
        url: '/ExistingRoads/GetExistingRoadsListITNO/',
        datatype: "json",
        mtype: "POST",
        colNames: ['Existing Road System Id', 'Road Category', 'Road Number', "Road Name", 'Road Type', 'Road Length [in Km]', "Road Owner", "Included in Core Network", "CD Works", "Surface Types", "Habitations", "Traffic Intensity", "CBR Value", "Candidate Road Details", "PMGSY1 DRRP Roads", "Edit", "Delete", "Shift"],
        colModel: [
                        { name: 'ERCode', index: 'HabitationCode', height: 'auto', width: 60, align: "left", sortable: true, search: false },
                        { name: 'MAST_ROAD_SHORT_DESC', index: 'MAST_ROAD_SHORT_DESC', width: 60, sortable: true, align: "left", search: false }, //New
                        { name: 'MAST_ER_ROAD_NUMBER', index: 'MAST_ER_ROAD_NUMBER', width: 90, sortable: true, align: "left", search: false },
                        { name: 'MAST_ER_ROAD_NAME', index: 'MAST_ER_ROAD_NAME', width: 200, sortable: true, align: "left", search: true },
                        { name: 'MAST_ER_ROAD_TYPE', index: 'MAST_ER_ROAD_TYPE', width: 90, sortable: true, align: "left", search: false }, //New
                        { name: 'MAST_ER_ROAD_LENGTH', index: 'MAST_ER_ROAD_LENGTH', width: 100, sortable: true, align: "left", search: false },
                        { name: 'MAST_ER_ROAD_OWNER', index: 'MAST_ER_ROAD_OWNER', width: 80, sortable: true, align: "left", search: false },
                        { name: 'MAST_CORE_NETWORK', index: 'MAST_CORE_NETWORK', width: 97, sortable: true, align: "center", search: false },
                        { name: 'a', width: 70, sortable: false, resize: false, align: "center", sortable: false, search: false },
                        { name: 'SurfaceTypes', width: 70, sortable: false, resize: false, align: "center", sortable: false, search: false },
                        { name: 'HabitationsMapped', width: 70, sortable: false, resize: false, falign: "center", sortable: false, search: false },
                        { name: 'TrafficIntensity', width: 70, sortable: false, resize: false, align: "center", sortable: false, search: false },
                        { name: 'CBRValue', width: 70, sortable: false, resize: false, align: "center", sortable: false, search: false },
                        { name: 'ShowDetails', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false },
                        { name: 'MapDRRP', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false,hidden: true },
                        { name: 'Edit', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false /*hidden: ($("#RoleCode").val() == 22 ? false : true)*/ }, /*Changes by SAMMED A. PATIL for mordviewuser*/
                        //{ name: 'Delete', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false, hidden: (($("#RoleCode").val() == 22 || $("#RoleCode").val() == 25) ? ($("#PMGSYScheme").val() == "2" ? true : false) : true) },
                        { name: 'Delete', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false },
                                    { name: 'Shift', width: 130, sortable: false, resize: false, formatter: FormatColumnShift1, align: "center", search: false },
        ],
        postData: { "MAST_BLOCK_CODE": MAST_BLOCK_CODE, "MAST_ROAD_CAT_CODE": MAST_ROAD_CAT_CODE, districtCode: $("#ddlDistricts option:selected").val(), stateCode: $("#ddlStates option:selected").val() },
        pager: jQuery('#dvExistingRoadsListPager'),
        rowNum: 10,
        sortorder: "desc",
        sortname: 'MAST_ER_ROAD_CODE',
        rowList: [5, 10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Existing Roads List",
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
            "<a href='#' style= 'font-size:.9em;' >CD Works Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseExistingRoadsDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();

        //$("#divExistingRoadsForm").load('/ExistingRoads/CdWorkAddEdit/' + urlparamater, function () {
        $("#divExistingRoadsForm").load('/ExistingRoads/ListCdWorks?id=' + urlparamater, function () {
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
            "<a href='#' style= 'font-size:.9em;' >Surface Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseExistingRoadsDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();

        //$("#divExistingRoadsForm").load('/ExistingRoads/SurfaceAddEdit/' + urlparamater, function () {
        $("#divExistingRoadsForm").load('/ExistingRoads/ListSurfaceTypes?id=' + urlparamater, function () {
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
            "<a href='#' style= 'font-size:.9em;' >Map Habitations</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseExistingRoadsDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        //$("#divAddForm").load('/CoreNetwork/DetailsCoreNetwork?id=' + urlparameter, function () {
        $("#divExistingRoadsForm").load('/ExistingRoads/ListHabitations/' + urlparameter, function () {
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
            "<a href='#' style= 'font-size:.9em;' >Traffic Intensity Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseExistingRoadsDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();

        $("#divExistingRoadsForm").load('/ExistingRoads/TrafficIntensity/' + urlparamater, function () {

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
            "<a href='#' style= 'font-size:.9em;' >CBR Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img class="ui-icon ui-icon-closethick" onclick="CloseExistingRoadsDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();

        $("#divExistingRoadsForm").load('/ExistingRoads/CBRAddEdit/' + urlparamater, function () {
            $.validator.unobtrusive.parse($('#frmCBRValue'));
            unblockPage();
        });
        $("#tbExistingRoadsList").jqGrid('setGridState', 'hidden');
        $('#divExistingRoadsForm').show('slow');
        $("#divExistingRoadsForm").css('height', 'auto');
    });
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
        $("#divExistingRoadsForm").load('/ExistingRoads/ViewExistingRoadsITNO/' + id, function () {
            $.validator.unobtrusive.parse($('#divExistingRoadsForm'));
            unblockPage();
        });
        $("#divExistingRoadsForm").show("slow");
        $("#divExistingRoadsForm").css('height', 'auto');
    });

    $("#tbExistingRoadsList").jqGrid('setGridState', 'hidden');
}

// diplay Existing Road Data entery form in Edit mode
function DeleteDetailsITNO(id) {

    // var MAST_BLOCK_CODE = $("#ddlBlocks option:selected").val();
    // var MAST_ROAD_CAT_CODE = $("#ddlRoadCategory option:selected").val();


    if (confirm("Are you sure to delete existing road details ? ")) {
        $.ajax({
            url: '/ExistingRoads/DeleteExistingRoadsMainITNO/' + id,
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
        $("#divExistingRoadsForm").load('/ExistingRoads/ViewExistingRoadDetails/' + id, function () {
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
        url: '/ExistingRoads/GetExistingRoadsListITNO', datatype: 'json'
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
                if (this.Value == "-1") {
                    $(dropdown).append("<option selected value=" + "0" + ">" + "Select Block" + "</option>");
                }
                else {
                    $(dropdown).append("<option selected value=" + this.Value + ">" + this.Text + "</option>");
                }
            }
            else {
                if (this.Value == "-1") {
                    $(dropdown).append("<option selected value=" + "0" + ">" + "Select Block" + "</option>");
                }
                else {
                    $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
                }
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
        url: '/ExistingRoads/CheckLockStatus/',
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

function FormatColumnShift1(cellvalue, options, rowObject) {
    if (cellvalue == "") {
        return "<center><table><tr><td  style='border:none'><a href='#' title='Shift Details'>_</a></td></tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none'><a href='#' title='Shift Details' onClick ='ShiftVillage1(\"" + cellvalue.toString() + "\");' >Shift</a></td></tr></table></center>";
    }
}

function ShiftVillage1(parameter) {
    debugger;
    var id = parameter;

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $('#dvShiftVillage').empty();

    $("#dvShiftVillage").load("/ExistingRoads/ERShiftDetailsGet/" + id, function () {
        $("#dvShiftVillage").dialog('open');
        $.unblockUI();
    })

}