/*

 * 
 */
$(document).ready(function () {

    blockPage();

    var MAST_BLOCK_CODE = $("#ddlBlocks option:selected").val();
    var MAST_ROAD_CAT_CODE = $("#ddlRoadCategory option:selected").val();



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
           // checkLockStatus();
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

        if (parseInt($("#ddlScheme option:selected").val()) <= 0) {
            alert('Please select Scheme');
            return false;
        }

        blockPage();
        LoadExistingRoads($("#ddlBlocks option:selected").val(), $("#ddlScheme option:selected").val());
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
                       "#ddlDistricts", "/ExistingRoads/GetDistrictByStateForShifting?stateCode=" + $('#ddlStates option:selected').val());
        }
    });

    $("#ddlDistricts").change(function () {
        FillInCascadeDropdown({ userType: $("#ddlDistricts").find(":selected").val() },
                   "#ddlBlocks", "/ExistingRoads/GetInactiveBlocksByDistrictForShifting?districtCode=" + $('#ddlDistricts option:selected').val());
    });

    $("#dvShiftVillage").dialog({
        autoOpen: false,
        height: 'auto',
        width: "450",
        modal: true,
        title: 'Shift Details'
    });


});


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


function LoadExistingRoads(MAST_BLOCK_CODE, SCHEME_CODE) {

    jQuery("#tbExistingRoadsList").jqGrid('GridUnload');

    jQuery("#tbExistingRoadsList").jqGrid({
        url: '/ExistingRoads/GetERShiftList/',
        datatype: "json",
        mtype: "POST",
        //   colNames: ['Existing Road System Id', 'Road Category', 'Road Number', "Road Name", 'Road Type', 'Road Length [in Km]', "Road Owner", "Included in Core Network", "CD Works", "Surface Types", "Habitations", "Traffic Intensity", "CBR Value", "Candidate Road Details", "PMGSY1 DRRP Roads", "Edit", "Delete",'Shift'],
        colNames: ['Existing Road System Id', 'Road Category', 'Road Number', 'Scheme',"Road Name", 'Road Type', 'Road Length [in Km]', "Road Owner", "Included in Core Network", 'Shift'],
        colModel: [
                        { name: 'MAST_ER_ROAD_CODE', index: 'MAST_ER_ROAD_CODE', height: 'auto', width: 60, align: "left", sortable: true, search: false },
                        { name: 'MAST_ROAD_SHORT_DESC', index: 'MAST_ROAD_SHORT_DESC', width: 60, sortable: true, align: "left", search: false }, //New
                        { name: 'MAST_ER_ROAD_NUMBER', index: 'MAST_ER_ROAD_NUMBER', width: 90, sortable: true, align: "left", search: false },
                        { name: 'MAST_PMGSY_SCHEME', index: 'MAST_PMGSY_SCHEME', width: 90, sortable: true, align: "left", search: false },
                        { name: 'MAST_ER_ROAD_NAME', index: 'MAST_ER_ROAD_NAME', width: 200, sortable: true, align: "left", search: true },
                        { name: 'MAST_ER_ROAD_TYPE', index: 'MAST_ER_ROAD_TYPE', width: 90, sortable: true, align: "left", search: false }, //New
                        { name: 'MAST_ER_ROAD_LENGTH', index: 'MAST_ER_ROAD_LENGTH', width: 100, sortable: true, align: "left", search: false },
                        { name: 'MAST_ER_ROAD_OWNER', index: 'MAST_ER_ROAD_OWNER', width: 80, sortable: true, align: "left", search: false },
                        { name: 'MAST_CORE_NETWORK', index: 'MAST_CORE_NETWORK', width: 97, sortable: true, align: "center", search: false },
                        { name: 'Shift', width: 130, sortable: false, resize: false, formatter: FormatColumnShift1, align: "center" ,search: false}

        ],
        postData: { "MAST_BLOCK_CODE": MAST_BLOCK_CODE, "SCHEME_CODE_DETAILS": SCHEME_CODE, districtCode: $("#ddlDistricts option:selected").val(), stateCode: $("#ddlStates option:selected").val() },
        pager: jQuery('#dvExistingRoadsListPager'),
        rowNum: 10,
        sortorder: "desc",
        sortname: 'MAST_ER_ROAD_CODE',
        rowList: [5, 10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Existing Roads List in Inactive Block",
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

function FormatColumnShift1(cellvalue, options, rowObject) {
    return "<center><table><tr><td  style='border:none'><a href='#' title='Shift Details' onClick ='ShiftVillage1(\"" + cellvalue.toString() + "\");' >Shift</a></td></tr></table></center>";
}


function ShiftVillage1(parameter) {

    var id = parameter;

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $('#dvShiftVillage').empty();

    $("#dvShiftVillage").load("/ExistingRoads/ERShiftDetailsGet/" + id, function () {
        $("#dvShiftVillage").dialog('open');
        $.unblockUI();
    })

    //$.ajax({
    //    url: '/ExistingRoads/ShiftDetails/' + id,
    //    type: "GET",
    //    cache: false,
    //    async: false,
    //  //  data: { "__RequestVerificationToken": token },
    //    success: function (response)
    //    {
    //        $("#dvLoadHere").html(response); // Load Form in One Div

    //        $("#dvLoadHere").dialog('open'); // Use Loaded form as a dialog

    //    },
    //    error: function ()
    //    {
    //       // $.unblockUI();
    //        alert("Error : " + error);
    //        return false;
    //    }
    //});


}


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

