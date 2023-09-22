
$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

$(document).ready(function () {   
    $('#dvSearchRegion').load('/Master/SearchRegion');
    $('#btnCreateNew').click(function (e) {

        if ($("#dvSearchRegion").is(":visible")) {
            $('#dvSearchRegion').hide('slow');
        }

        if ($("#dvMapRegionDistrictsDetails").is(":visible")) {
            $('#dvMapRegionDistrictsDetails').hide('slow');
        }

        if ($("#dvMappedRegionDistrictsDetails").is(":visible")) {
            $('#dvMappedRegionDistrictsDetails').hide('slow');
        }

        if (!$("#dvRegionDetails").is(":visible")) {
            $("#dvRegionDetails").load("/Master/AddEditMasterRegion", function () {
                $('#dvRegionDetails').show('slow');
            });


            $('#btnCreateNew').hide();
            $('#btnSearchView').show();
        }
        $('#tblRegionList').jqGrid("setGridState", "visible");


    });

    $('#btnSearchView').click(function (e) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        if ($("#dvMapRegionDistrictsDetails").is(":visible")) {
            $('#dvMapRegionDistrictsDetails').hide('slow');
        }

        if ($("#dvMappedRegionDistrictsDetails").is(":visible")) {
            $('#dvMappedRegionDistrictsDetails').hide('slow');
        }

        $('#dvRegionDetails').hide('slow');

        $('#btnSearchView').hide();
        $('#btnCreateNew').show();


        $('#tblRegionList').jqGrid("setGridState", "visible");

        if (!$("#dvSearchRegion").is(":visible")) {

            $('#dvSearchRegion').load('/Master/SearchRegion', function () {
                var data = $('#tblRegionList').jqGrid("getGridParam", "postData");
                
                if (!(data === undefined)) {
                    $('#ddlSearchStates').val(data.stateCode);
                }

                $('#dvSearchRegion').show('slow');
            });

        }

        $.unblockUI();


    });



});

function LoadRegionGrid() {    

    $('#tblRegionList').jqGrid({
        url: '/Master/GetMasterRegionList/',
        datatype: 'json',
        mtype: "POST",
        colNames: ['Region Name', 'State Name', 'Map Districts', 'Mapped Districts', 'Action'],
        colModel: [
            { name: 'MAST_REGION_NAME', index: 'MAST_REGION_NAME', height: 'auto', width: 220, align: "left", sortable: true },
            { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 150, align: "left", sortable: true },
            { name: 'Map', width: 140, sortable: false, resize: false, formatter: FormatColumnMap, align: "center" }, //added by koustubh nakate on 04-05-2013
            { name: 'Mapped', width: 100, sortable: false, resize: false, formatter: FormatColumnMapped, align: "center" },//added by koustubh nakate on 13-05-2013
           { name: 'a', width: 60, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
        ],
        postData: { stateCode: $('#ddlSearchStates option:selected').val() },
        pager: jQuery('#divPagerRegion'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_STATE_NAME,MAST_REGION_NAME',
        sortorder: "asc",
        caption: 'Region List',
        height: '100%',
        autowidth: true,
        rownumbers: true,
        loadComplete: function () {
            $.unblockUI();
        },

        loadError: function (xhr, status, error) {

            $.unblockUI();
            if (xhr.responseText == "session expired") {

                alert(xht.responseText);
                window.location.href = "Login/Login";
            }
            else {
                alert("Invalid Data. Please Check and Try Again!");
            }
        }

    });

}
function FormatColumn(cellvalue, options, rowObject) {

    if (cellvalue != '') {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit Region Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete Region Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }
}


function editData(id) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: "/Master/EditMasterRegion/" + id,
        type: "GET",
        async: false,
        dataType: "html",
        catche: false,
        contentType: "application/json; charset=utf-8",
        success: function (data) {

            if ($("#dvMapRegionDistrictsDetails").is(":visible")) {
                $('#dvMapRegionDistrictsDetails').hide('slow');
            }

            if ($("#dvMappedRegionDistrictsDetails").is(":visible")) {
                $('#dvMappedRegionDistrictsDetails').hide('slow');
            }


            if ($("#dvSearchRegion").is(":visible")) {
                $('#dvSearchRegion').hide('slow');
            }
            $("#dvRegionDetails").show();
            $("#dvRegionDetails").html(data);
            $('#btnCreateNew').hide();
            $('#btnSearchView').show();
            $('#trAddNewSearch').show();

            $("#MAST_REGION_NAME").focus();

            $.unblockUI();
        },
        error: function (xht, ajaxOptions, throwError) {
            alert(xht.responseText);
            $.unblockUI();
        }
    });//ajax end
}

function deleteData(urlParam) {
    if (confirm("Are you sure you want to delete Region details?")) {

        if ($("#dvMapRegionDistrictsDetails").is(":visible")) {
            $('#dvMapRegionDistrictsDetails').hide('slow');
        }

        if ($("#dvMappedRegionDistrictsDetails").is(":visible")) {
            $('#dvMappedRegionDistrictsDetails').hide('slow');
        }


        $.ajax({
            url: "/Master/DeleteMasterRegion/" + urlParam,
            type: "GET",
            dataType: "json",
            success: function (data) {
                
                if (data.success) {
                    alert(data.message);
                    //if ($("#dvSearchRegion").is(":visible")) {

                    //    $('#btnSearch').trigger('click');

                    //}
                    //else {
                    //    $('#tblRegionList').trigger('reloadGrid');
                    //}                   
                    $('#dvRegionDetails').hide('slow');
                    $('#btnSearchView').hide();
                    $('#btnCreateNew').show();
                    if (!$("#dvSearchRegion").is(":visible")) {
                        $("#dvSearchRegion").show('slow');
                        $('#tblRegionList').trigger('reloadGrid');
                    } else {
                        $('#tblRegionList').trigger('reloadGrid');
                    }

                }
                else {

                    alert(data.message);
                }

                $.unblockUI();
            },
            error: function (xht, ajaxOptions, throwError) {
                alert(xht.responseText);
                $.unblockUI();
            }
        });

        if (!$("#dvRegionDetails").is(':visible')) {
            $('#btnSearchView').trigger('click');
            $('#trAddNewSearch').show();
        }
    }
    else {
        return false;
    }


}


function MapDistricts(parameter) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
           $('#dvMapRegionDistrictsDetails').load("/Master/MapRegionDistricts/" + parameter, function () {

        $('#dvRegionDetails').hide();

        LoadMapRegionDistricts();
        $('#tblRegionList').jqGrid("setGridState", "hidden");
        $('#dvMapRegionDistrictsDetails').show('slow');

        $('#trAddNewSearch').hide();
        $('#dvSearchRegion').hide();

        if ($("#dvMappedRegionDistrictsDetails").is(":visible")) {
            $('#dvMappedRegionDistrictsDetails').hide('slow');
        }

        $.unblockUI();
    });


}

function LoadMapRegionDistricts() {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    jQuery("#tbMapRegionDistrictList").jqGrid({
        url: '/Master/GetDistrictDetailsList_Mapping',
        datatype: "json",
        mtype: "POST",
        postData: { StateCode: $('#EncryptedStateCode').val() },
        colNames: ['District Id','District Name', 'State Name', 'Is Included In PMGSY', 'Is IAP District', 'Shift District', 'Action'],
        colModel: [
                            { name: 'MastDistrictId', index: 'MastDistrictId', height: 'auto', width: 90, sortable: true, align: "center" ,hidden:true},
                            { name: 'DistrictName', index: 'DistrictName', height: 'auto', width: 430, align: "left", sortable: true },
                            { name: 'StateName', index: 'StateName', height: 'auto', width: 170, sortable: false, align: "left", hidden: true },
                            { name: 'IsPMGSYIncluded', index: 'IsPMGSYIncluded', width: 200, sortable: true },
                            { name: 'IsIAPDistrict', index: 'IsIAPDistrict', width: 200, sortable: true },
                            { name: 'Shift', /*width: 140,*/ sortable: false, resize: false, align: "center", hidden: true },
                            { name: 'a', /*width: 80,*/ sortable: false, resize: false, align: "center", sortable: false, hidden: true }
        ],       
        pager: jQuery('#dvMapRegionDistrictListPager'),
        pginput: false,
        pgbuttons: false,
        rowNum: 0,
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'DistrictName',
        sortorder: "asc",
        caption: "District List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        hidegrid: false,
        multiselect: true,
        loadComplete: function () {

            var recordCount = jQuery('#tbMapRegionDistrictList').jqGrid('getGridParam', 'reccount');
          
            if (recordCount > 15) {

                $('#tbMapRegionDistrictList').jqGrid('setGridHeight', '320');

            }
            else {
                $('#tbMapRegionDistrictList').jqGrid('setGridHeight', 'auto');
            }
            $.unblockUI();
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
                
            }
        }

    }); //end of  grid

}

function FormatColumnMap(cellvalue, options, rowObject) {

    return "<center><table><tr><td  style='border:none'><a href='#' title='Map Districts' onClick ='MapDistricts(\"" + cellvalue.toString() + "\");' >Map Districts</a></td></tr></table></center>";

}

function FormatColumnMapped(cellvalue, options, rowObject) {

    return "<center><table><tr><td  style='border:none'><a href='#' title='View Mapped Districts' onClick ='MappedDistricts(\"" + cellvalue.toString() + "\");' >Mapped Districts</a></td></tr></table></center>";

}


function MappedDistricts(parameter) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });


    $.ajax({
        url: "/Master/MappedRegionDistricts/" + parameter,
        type: "GET",
        async: false,
        cache: false,
        success: function (data) {

            if ($("#dvMapRegionDistrictsDetails").is(":visible")) {
                $('#dvMapRegionDistrictsDetails').hide('slow');
            }

            $("#dvMappedRegionDistrictsDetails").html(data);
            $('#dvMappedRegionDistrictsDetails').show('slow');

            $('#trAddNewSearch').hide();
            $('#dvSearchRegion').hide();
            $('#dvRegionDetails').hide();

          

            $('#tblRegionList').jqGrid("setGridState", "hidden");

            $.unblockUI();

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();
        }

    });


}