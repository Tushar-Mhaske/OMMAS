
$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

$(document).ready(function () {

    $.ajax({
        url: "/LocationMasterDataEntry/SearchPanchayat",
        type: "GET",
        dataType: "html",
        success: function (data) {
            $("#dvSearchPanchayat").html(data);
            //$("#ddlSearchStates").trigger('change');

            //$("#ddlSearchDistrict").trigger('change');

            $('#btnSearch').trigger('click');


        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
        }

    });

    LoadGrid();

    $.unblockUI();


    $('#btnCreateNew').click(function (e) {

        if ($("#dvSearchPanchayat").is(":visible")) {
            $('#dvSearchPanchayat').hide('slow');
        }


        if ($("#dvMapPanchayatHabitationsDetails").is(":visible")) {
            $('#dvMapPanchayatHabitationsDetails').hide('slow');
        }

        if ($("#dvMappedPanchayatHabitationDetails").is(":visible")) {
            $('#dvMappedPanchayatHabitationDetails').hide('slow');
        }

        if (!$("#dvPanchayatDetails").is(":visible")) {
            $('#dvPanchayatDetails').load("/LocationMasterDataEntry/CreatePanchayat");
            $('#dvPanchayatDetails').show('slow');

            $('#btnCreateNew').hide();
            $('#btnSearchView').show();
        }
        $('#tbPanchyatList').jqGrid("setGridState", "visible");

    });

    $('#btnSearchView').click(function (e) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        if ($("#dvMapPanchayatHabitationsDetails").is(":visible")) {
            $('#dvMapPanchayatHabitationsDetails').hide('slow');
        }

        if ($("#dvMappedPanchayatHabitationDetails").is(":visible")) {
            $('#dvMappedPanchayatHabitationDetails').hide('slow');
        }


        $('#dvPanchayatDetails').hide('slow');

        $('#btnSearchView').hide();
        $('#btnCreateNew').show();

        //if ($("#dvPanchayatDetails").is(":visible")) {
        //    $('#dvPanchayatDetails').hide('slow');

        //    $('#btnSearchView').hide();
        //    $('#btnCreateNew').show();

        //}

        $('#tbPanchyatList').jqGrid("setGridState", "visible");

        if (!$("#dvSearchPanchayat").is(":visible")) {

            $('#dvSearchPanchayat').load('/LocationMasterDataEntry/SearchPanchayat', function () {

               // $('#tbPanchyatList').trigger('reloadGrid');

                var data = $('#tbPanchyatList').jqGrid("getGridParam", "postData");

                if (!(data === undefined)) {

                    $('#ddlSearchStates').val(data.stateCode);

                    FillInCascadeDropdown({ userType: $("#ddlSearchStates").find(":selected").val() },
                  "#ddlSearchDistrict", "/LocationMasterDataEntry/GetDistrictsByStateCode_Search?stateCode=" + $('#ddlSearchStates option:selected').val());

                    setTimeout(function () {

                        $('#ddlSearchDistrict').val(data.districtCode);

                        $('#ddlSearchBlocks').empty();
                        FillInCascadeDropdown({ userType: $("#ddlSearchDistrict").find(":selected").val() },
                                    "#ddlSearchBlocks", "/LocationMasterDataEntry/GetBlocksByDistrictCode_Search?districtCode=" + $('#ddlSearchDistrict option:selected').val());

                    }, 1000);


                    setTimeout(function () {

                        $('#ddlSearchBlocks').val(data.blockCode);

                    }, 2000);
                }
                $('#dvSearchPanchayat').show('slow'); 
            });
           
        }
        $.unblockUI();
       

    });


    $("#dvShiftPanchayat").dialog({
        autoOpen: false,
        height: 'auto',
        width: "450",
        modal: true,
        title: 'Shift Panchayat'
    });


});

function FormatColumn(cellvalue, options, rowObject) {


    //return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-pencil' title='Edit State Details' onClick ='EditStateDetails(\"" + cellvalue.toString() + "\");'></span></td><td style='border-color:white'><span class='ui-icon ui-icon-trash' title='Delete State Details' onClick ='DeleteStateDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";

   // return "<center><table><tr><td  style='border-color:white'><a href='#' title='Edit Panchayat Details' onClick ='EditPanchayatDetails(\"" + cellvalue.toString() + "\");'>Edit</a></td></tr></table></center>";

    if (cellvalue != '') {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit Panchayat Details' onClick ='EditPanchayatDetails(\"" + cellvalue.toString() + "\");'></span></td> <td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete Panchayat Details' onClick ='DeletePanchayatDetails(\"" + cellvalue.toString() + "\");'></span></td> </tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }
    
}

function FormatColumnShift(cellvalue, options, rowObject) {

    return "<center><table><tr><td  style='border:none'><a href='#' title='Shift Panchayat' onClick ='ShiftPanchayat(\"" + cellvalue.toString() + "\");' >Shift Panchayat</a></td></tr></table></center>";

}


function FormatColumnMap(cellvalue, options, rowObject) {

    return "<center><table><tr><td  style='border:none'><a href='#' title='Map Habitations' onClick ='MapHabitations(\"" + cellvalue.toString() + "\");' >Map Habitations</a></td></tr></table></center>";

}

function FormatColumnMapped(cellvalue, options, rowObject) {

    return "<center><table><tr><td  style='border:none'><a href='#' title='View Mapped Habitations' onClick ='MappedHabitations(\"" + cellvalue.toString() + "\");' >Mapped Habitations</a></td></tr></table></center>";

}

function EditPanchayatDetails(paramater) {

  //  alert(paramater);
    $.ajax({
        url: "/LocationMasterDataEntry/EditPanchayat/" + paramater,
        type: "GET",
        async: false,
        cache: false,
        //data: $("form").serialize(),
        success: function (data) {

            //$("#mainDiv").html(data);

            if ($("#dvSearchPanchayat").is(":visible")) {
                $('#dvSearchPanchayat').hide('slow');
            }

            if ($("#dvMapPanchayatHabitationsDetails").is(":visible")) {
                $('#dvMapPanchayatHabitationsDetails').hide('slow');
            }

            if ($("#dvMappedPanchayatHabitationDetails").is(":visible")) {
                $('#dvMappedPanchayatHabitationDetails').hide('slow');
            }

            $('#btnCreateNew').hide();
            $('#btnSearchView').show();
            $('#trAddNewSearch').show();
            $("#dvPanchayatDetails").html(data);
            $('#dvPanchayatDetails').show('slow');
            $("#MAST_PANCHAYAT_NAME").focus();

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
        }

    });
}


function LoadGrid() {

    //var pageWidth = $("#dvPanchyatList").parent().width();
    //alert(pageWidth);


    jQuery("#tbPanchyatList").jqGrid({
        url: '/LocationMasterDataEntry/GetPanchayatDetailsList',
        datatype: "local",//"json",
        mtype: "POST",
        colNames: ['Panchayat Name', 'Block Name', 'District Name', 'State Name', 'Map Habitations', 'Mapped Habitations', 'Shift Panchayat', 'Action','Lock Status'],
        colModel: [
                             { name: 'PanchyatName', index: 'PanchyatName', height: 'auto', width: 150, align: "left", sortable: true },
                             { name: 'BlockName', index: 'BlockName', height: 'auto', width: 150, align: "left", sortable: true },
                             { name: 'DistrictName', index: 'DistrictName', height: 'auto', width: 150, sortable: true, align: "left", hidden: false },
                             { name: 'StateName', index: 'StateName', height: 'auto', width: 170, sortable: false, align: "left", hidden: false },
                             { name: 'Map', width: 140, sortable: false, resize: false, formatter: FormatColumnMap, align: "center" },
                             { name: 'Mapped', width: 140, sortable: false, resize: false, formatter: FormatColumnMapped, align: "center" },
                             { name: 'Shift', width: 140, sortable: false, resize: false, formatter: FormatColumnShift, align: "center" ,hidden:false},
                            // { name: 'a', width: 60, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false },
                             { name: 'a', width: 60, sortable: false, resize: false,  align: "center", sortable: false },
                             { name: 'LockStatus', width: 50, sortable: true, resize: false,  align: "center", sortable: false,hidden: ($("#RoleCode").val() == 23 ? false : true) }


        ],
        pager: jQuery('#dvPanchyatListPager'),
        rowNum: 15,
        //altRows: true,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'DistrictName,BlockName,PanchyatName',
         sortorder: "asc",
        caption: "Panchayat List",
        height: 'auto',
        //width: 'auto',
        autowidth: true,
        rownumbers: true,
        //hidegrid: false,
        /*grouping: true,
        groupingView:
            {
                groupField: ['StateName', 'DistrictName', 'BlockName'],//'BlockName'
                groupColumnShow: [false, false, true],
                groupText: ['<b>State: {0}</b>', '<b>District: {0}</b>', '<b>Block: {0}</b>']
                //groupCollapse: true,
                //groupOrder: ['asc']
            },*/
        loadComplete: function (data) {
            var recordCount = jQuery('#tbPanchyatList').jqGrid('getGridParam', 'reccount');
            if (recordCount > 0) {
                var button = '<input type="button" id="btnFinalizeVillage" name="btnFinalizeVillage" value="Finalize" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only" title="Finalize State" tabindex="200" style="font-size:1em; margin-left:25px" onclick="FinalizePanchayat()" />'
                $('#dvPanchyatListPager_left').html(button);
            }
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                // alert(xhr.responseText);
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }

    }); //end of documents grid
}



function MapHabitations(parameter) {
    
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    //$('#btnSearchView').trigger('click');

    $.ajax({
        url: "/LocationMasterDataEntry/MapPanchayatHabitations/" + parameter,
        type: "GET",
        async: false,
        cache: false,

        success: function (data) {

            $("#dvMapPanchayatHabitationsDetails").html(data);
            $('#dvMapPanchayatHabitationsDetails').show('slow');
 
            
            $('#btnSearch_Map').trigger('click');

            $('#trAddNewSearch').hide();
            $('#dvSearchPanchayat').hide();
            $('#dvPanchayatDetails').hide();

            if ($("#dvMappedPanchayatHabitationDetails").is(":visible")) {
                $('#dvMappedPanchayatHabitationDetails').hide('slow');
            }

            //$('#tbMLAConstituencyList').setGridParam({ hidegrid: true });
            $('#tbPanchyatList').jqGrid("setGridState", "hidden");

            $.unblockUI();

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();
        }

    });


}


function ShiftPanchayat(parameter) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    if ($("#dvMapPanchayatHabitationsDetails").is(":visible")) {
        $('#dvMapPanchayatHabitationsDetails').hide('slow');
    }

    if ($("#dvMappedPanchayatHabitationDetails").is(":visible")) {
        $('#dvMappedPanchayatHabitationDetails').hide('slow');
    }

    $('#btnSearchView').trigger('click'); 
    $('#trAddNewSearch').show();

    $('#dvShiftPanchayat').empty();
    $("#dvShiftPanchayat").load("/LocationMasterDataEntry/ShiftPanchayat?id=" + parameter, function () {

        $("#dvShiftPanchayat").dialog('open');
        $.unblockUI();
    })

}
function DeletePanchayatDetails(urlparamater) {

  

    if (confirm("Are you sure you want to delete panchayat details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        if ($("#dvMapPanchayatHabitationsDetails").is(":visible")) {
            $('#dvMapPanchayatHabitationsDetails').hide('slow');
        }

        if ($("#dvMappedPanchayatHabitationDetails").is(":visible")) {
            $('#dvMappedPanchayatHabitationDetails').hide('slow');
        }
      
      
        $.ajax({
            url: "/LocationMasterDataEntry/DeletePanchayatDetails/" + urlparamater,
            type: "GET",
            dataType: "json",
            success: function (data) {

                if (data.success) {
                    alert(data.message);

                    //if ($("#dvSearchPanchayat").is(":visible")) {

                    //    $('#btnSearch').trigger('click');

                    //}
                    //else {
                    //    $('#tbPanchyatList').trigger('reloadGrid');
                    //}

                    if ($("#dvMapPanchayatHabitationsDetails").is(":visible")) {
                        $('#dvMapPanchayatHabitationsDetails').hide('slow');
                    }

                    if ($("#dvMappedPanchayatHabitationDetails").is(":visible")) {
                        $('#dvMappedPanchayatHabitationDetails').hide('slow');
                    }
                    $('#dvPanchayatDetails').hide('slow');
                    $('#btnSearchView').hide();
                    $('#btnCreateNew').show();
                    if (!$("#dvSearchPanchayat").is(":visible")) {
                        $("#dvSearchPanchayat").show('slow');
                        $('#tbPanchyatList').trigger('reloadGrid');
                    }
                    else {
                        $('#tbPanchyatList').trigger('reloadGrid');
                    }

                }
                else {
                    alert(data.message);
                }

                $.unblockUI();

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }

        });

        if (!$("#dvPanchayatDetails").is(':visible')) {
            $('#btnSearchView').trigger('click');
            $('#trAddNewSearch').show();
        }
    }
    else {
        return false;
    }
}

function MappedHabitations(parameter) {
    // alert(parameter);

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: "/LocationMasterDataEntry/MappedPanchayatHabitations/" + parameter,
        type: "GET",
        async: false,
        cache: false,

        success: function (data) {

            $("#dvMappedPanchayatHabitationDetails").html(data);
            $('#dvMappedPanchayatHabitationDetails').show('slow');

            $('#trAddNewSearch').hide();
            $('#dvSearchPanchayat').hide();
            $('#dvPanchayatDetails').hide();

            if ($("#dvMapPanchayatHabitationsDetails").is(":visible")) {
                $('#dvMapPanchayatHabitationsDetails').hide('slow');
            }

            $('#tbPanchyatList').jqGrid("setGridState", "hidden");

            $.unblockUI();

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();
        }

    });


}

function FinalizePanchayat() {
    var id = $('#tbPanchyatList').jqGrid('getGridParam', 'selrow');

    if ($('#tbPanchyatList').jqGrid('getGridParam', 'selrow')) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'POST',
            url: '/LocationMasterDataEntry/FinalizePanchayat/' + id,
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {
                    alert(data.message);
                    $("#tbPanchyatList").trigger('reloadGrid');
                    $.unblockUI();
                }
                else if (data.success == false) {
                    alert(data.message);
                    $.unblockUI();
                }
            },
            error: function () { $.unblockUI(); }
        });
    }
    else {
        alert('Please select Panchayat to finalize.');
    }
}