/*
 * Purpose:- 1)Financial Year Target.js is used to show Existing road list and for Filter Search
 * 
 */
$(document).ready(function () {
    debugger;

    //blockPage();


    var State = $("#ddlStates option:selected").val();
    var Year = $("#ddlYear option:selected").val();

    //display Existing Road List
    LoadFinancialYearTarget();

    ////show/hide filter search 
    //$("#idFilterDiv").click(function () {
    //    $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
    //    $("#divFilterForm").toggle("slow");
    //});

    //display Existing Road data entry form
    $('#btnAddFinancialTarget').click(function () {
        debugger;
        var state = $('#ddlStates option:selected').val();
        var year = $('#ddlYear option:selected').val();
                //if ($('#RoleCode').val() == 25) {
            if (parseInt($("#ddlStates option:selected").val()) <= 0) {
                alert('Please select state');
                return false;
            }
            if (parseInt($("#ddlYear option:selected").val()) <= 0) {
                alert('Please select Year');
                return false;
            }
            
        // }
            $.ajax({
                url: '/Master/ValidationOnAddForm/?state=' + state + '&year=' + year,
                type: "POST",
                contentType: "application/json; charset=utf-8",
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
                    if (response != null) {
                        if (response.success) {
                            var state = $("#ddlStates option:selected").text();
                            var year = $("#ddlYear").val();

                            $("#divFinancialYearTargetForm").html("");

                            $("#accordion h3").html(
                                        "<a href='#' style= 'font-size:.9em;' >Add  Financial Year Target Details</a>" +
                                        '<a href="#" style="float: right;">' +
                                        '<img class="ui-icon ui-icon-closethick" onclick="CloseFinancialYearTargetDetails();" /></a>'
                                        );

                            $('#accordion').show('fold', function () {
                                blockPage();
                                //    //$("#divExistingRoadsForm").load('/ExistingRoads/AddEditExistingRoads/' + mastBlockCode, function () {
                                $("#divFinancialYearTargetForm").load('/Master/AddEditFinancialYearTarget/?state=' + encodeURIComponent(state) + '&year=' + year); //, function () {
                                $.validator.unobtrusive.parse($('#divFinancialYearTargetForm'));
                                unblockPage();
                                // });

                                $('#divFinancialYearTargetForm').show('slow');
                                $("#divFinancialYearTargetForm").css('height', 'auto');
                            });

                            $("#tbFinancialYearTargetList").jqGrid('setGridState', 'hidden');
                            $("#tbFinancialYearTargetList").setGridParam('hidegrid', false);
                        }
                        else {
                            alert(response.message);
                            // $("#tbFinancialYearTargetList").trigger('reloadGrid');
                            //$('#tbExistingRoadsList').jqGrid('GridUnload');
                            // LoadExistingRoads(MAST_BLOCK_CODE, MAST_ROAD_CAT_CODE);                      
                        }
                    }
                    unblockPage();
                }
            });

       
    });//end AddForm

    //$('#ddlBlocks').change(function () {
    //    if ($('#RoleCode').val() != 25) {
    //        checkLockStatus();
    //    }
    //});

    //// display existing road grid by filter search criteria
    $("#btnListExistingFinancialTarget").click(function () {
        //if ($('#RoleCode').val() == 25 || $('#RoleCode').val() == 65) {//Changes by SAMMED A. PATIL for mordviewuser
            if (parseInt($("#ddlStates option:selected").val()) <= 0) {
                alert('Please select state');
                return false;
            }
            if (parseInt($("#ddlYear option:selected").val()) <= 0) {
                alert('Please select Year');
                return false;
            }
            LoadFinancialYearTarget();
       // }
        //blockPage();
       // SearchExistingRoadList();
        //CloseFinancialYearTargetDetails();
        //unblockPage();
    });//end Search

  
    $(function () {
        $("#accordion").accordion({
            //fillSpace: true,
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
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
function CloseFinancialYearTargetDetails() {
    $('#accordion').hide('slow');
    $('#divFinancialYearTargetForm').hide('slow');
    $("#tbFinancialYearTargetList").jqGrid('setGridState', 'visible');
    $("#tbFinancialYearTargetList").trigger('reloadGrid');
    showFilter();
}

//display Existing Road list
function LoadFinancialYearTarget() {
    debugger;

    jQuery("#tbFinancialYearTargetList").jqGrid('GridUnload');

    jQuery("#tbFinancialYearTargetList").jqGrid({
        url: '/Master/GetFinancialYearTargetList/',
        datatype: "json",
        mtype: "POST",
        colNames: ['TARGET_ID_PMGSY', 'Target Month', "Mast Scheme", 'Length Target', 'Habitation Target', 'Delete'],  //, "Edit", "Delete" 'State', 'Target Year',
        colModel: [
                       { name: 'TARGET_ID_PMGSY', index: 'TARGET_ID_PMGSY', height: 'auto', width: 60, align: "left", sortable: false, search: false, hidden: true },
                       // { name: 'STATE', index: 'STATE', width: 120, sortable: false, align: "left", search: false }, //New
                       // { name: 'TARGET_YEAR', index: 'TARGET_YEAR', width: 60, sortable: false, align: "left", search: false },
                        { name: 'TARGET_MONTH', index: 'TARGET_MONTH', width: 60, sortable: false, align: "center", search: false },
                        { name: 'MAST_SCHEME', index: 'MAST_SCHEME', width: 60, sortable: false, align: "center", search: false }, //New
                        { name: 'LENGTH_TARGET', index: 'LENGTH_TARGET', width: 80, sortable: false, align: "center", search: false },
                        { name: 'HABITATION_TARGET', index: 'HABITATION_TARGET', width: 80, sortable: false, align: "center", search: false },
                      //  { name: 'ShowDetails', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false },
                        //{ name: 'MapDRRP', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false, hidden: (($("#PMGSYScheme").val() == "2" && ($("#RoleCode").val() == 25 || $("#RoleCode").val() == 22)) ? false : true) /*hidden: true*/ },
                        //{ name: 'Edit', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false, /*hidden: ($("#RoleCode").val() == 22 ? false : true)*/ hidden: ($("#RoleCode").val() == 65) ? true : false }, /*Changes by SAMMED A. PATIL for mordviewuser*/
                        ////{ name: 'Delete', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false, hidden: (($("#RoleCode").val() == 22 || $("#RoleCode").val() == 25) ? ($("#PMGSYScheme").val() == "2" ? true : false) : true) },
                        {
                            name: 'Delete', width: 40, sortable: false, resize: false, align: "center", sortable: false, search: false,  /*hidden: (($("#RoleCode").val() == 25) ? ($("#PMGSYScheme").val() == "2" ? true : false) :
                              ($("#RoleCode").val() == 22) ? false : true)*/
                        },
        ],
        postData: { state: $('#ddlStates option:selected').val(), year: $('#ddlYear option:selected').val()  },
        pager: jQuery('#dvFinancialYearTargetListPager'),
        rowNum: 10,
        //sortorder: "desc",
        //sortname: 'MAST_ER_ROAD_CODE',
        rowList: [ 10, 20, 30,40,50],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Financial Year Target List",
        height: 'auto',
        autowidth: true,
        cmTemplate: { title: false },
        rownumbers: true,
        loadonce: true,
        loadComplete: function () {
            $("#tbFinancialYearTargetList #dvFinancialYearTargetListPager").css({ height: '31px' });
            //if ($("#RoleCode").val() == 22) {
            //    $("#dvExistingRoadsListPager_left").html("<input type='button' style='margin-left:27px' id='idFinalizeExistingRoad' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'RedirectFinalizeExistingRoad();return false;' value='Finalize Existing Road'/>");
            //}
            unblockPage();
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!");
                //alert("Session Timeout !!!");
                //window.location.href = "/Login/LogIn";
            }
        }

    }); //end of grid

   // $("#tbExistingRoadsList").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true });

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
//function FormatColumnDelete(cellvalue, options, rowObject) {

//    //if (cellvalue == "") {
//    //    return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-locked ui-align-center'></span></center>";

//    //} else {
//        return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-trash ui-align-center' title='Click here to delete Existing Road Details' onClick ='DeleteDetails(\"" + cellvalue.toString() + "\");'></span></center>";
//  //  }
//}


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



// diplay Existing Road Data entery form in Edit mode
function DeleteDetails(pmgsyId) {

    // var MAST_BLOCK_CODE = $("#ddlBlocks option:selected").val();
    // var MAST_ROAD_CAT_CODE = $("#ddlRoadCategory option:selected").val();


    if (confirm("Are you sure to delete Financial details ? ")) {
        debugger;
        $.ajax({
            url: '/Master/DeleteFinancialYearTarget/' + pmgsyId,
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
                    $("#tbFinancialYearTargetList").trigger('reloadGrid');
                   // $('#tbFinancialYearTargetList').jqGrid('GridUnload');
                    // LoadExistingRoads(MAST_BLOCK_CODE, MAST_ROAD_CAT_CODE);
                    LoadFinancialYearTarget();
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

