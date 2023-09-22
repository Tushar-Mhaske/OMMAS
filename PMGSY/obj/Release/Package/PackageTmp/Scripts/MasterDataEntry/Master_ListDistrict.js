
$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

$(document).ready(function () {

    $('#dvSearchDistrict').load('/LocationMasterDataEntry/SearchDistrict');
   // $('#btnSearch').trigger("click");
    $('#btnSearchView').click(function (e) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        if ($("#dvDistrictDetails").is(":visible")) {
            $('#dvDistrictDetails').hide('slow');

            $('#btnSearchView').hide();
            $('#btnCreateNew').show();
            
        }

        if (!$("#dvSearchDistrict").is(":visible")) {

            $('#dvSearchDistrict').load('/LocationMasterDataEntry/SearchDistrict', function (e) {

                // $('#tbDistrictList').trigger('reloadGrid');

                var data = $('#tbDistrictList').jqGrid("getGridParam", "postData");

                if (!(data === undefined)) {
                    $('#ddlSearchStates').val(data.stateCode);
                }

            });
            $('#dvSearchDistrict').show('slow');

        }

        $.unblockUI();



    });

 
    //jQuery("#tbDistrictList").jqGrid({
    //    url: '/LocationMasterDataEntry/GetDistrictDetailsList',
    //    datatype: "json",
    //    mtype: "POST",
       
    //    colNames: ['District Name', 'State Name', 'Is Included In PMGSY ', 'Is IAP District','Shift District','Action'],
    //    colModel: [
    //                        { name: 'DistrictName', index: 'DistrictName', height: 'auto', width: 200, align: "left", sortable: true },
    //                        { name: 'StateName', index: 'StateName', height: 'auto', width: 170, sortable: true, align: "left" },
    //                        { name: 'IsPMGSYIncluded', index: 'IsPMGSYIncluded', width: 140, sortable: true },
    //                        { name: 'IsIAPDistrict', index: 'IsIAPDistrict', width: 100, sortable: true },
    //                        { name: 'Shift', width: 120, sortable: false, resize: false, formatter: FormatColumnShift, align: "center" },
    //                        { name: 'a', width: 80, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
    //    ],
    //    pager: jQuery('#dvDistrictListPager'),
    //    rowNum: 15, 
    //  //  altRows: true,
    //    rowList: [ 10, 15, 20, 30],
    //    viewrecords: true,
    //    recordtext: '{2} records found',
    //    sortname: 'StateName,DistrictName',
    //    sortorder: "asc",
    //    caption: "District List",
    //    height: 'auto',
    //    // width: 'auto',
    //    autowidth: true,
    //    rownumbers: true,
    //    hidegrid: false,
    //    /*grouping: true,
    //    groupingView:
    //        {
    //            groupField: ['StateName'],
    //            groupColumnShow: [false],
    //            groupText: ['<b>State: {0}</b>'],
    //            //groupCollapse: true,
    //            groupOrder: ['asc']
    //        },*/
    //    loadComplete: function () {

    //    },
    //    loadError: function (xhr, ststus, error) {

    //        if (xhr.responseText == "session expired") {
    //            alert(xhr.responseText);
    //            window.location.href = "/Login/Login";
    //        }
    //        else {
    //            // alert(xhr.responseText);
    //            alert("Invalid data.Please check and Try again!")
    //            //  window.location.href = "/Login/LogIn";
    //        }
    //    }

    //}); //end of  grid

    $.unblockUI();

    $('#btnCreateNew').click(function (e) {
        if ($("#dvSearchDistrict").is(":visible")) {
            $('#dvSearchDistrict').hide('slow');
        }

        if (!$("#dvDistrictDetails").is(":visible")) { 
            $('#dvDistrictDetails').load("/LocationMasterDataEntry/CreateDistrict");
            $('#dvDistrictDetails').show('slow');
           // $('#ddlStates').val($('#ddlSearchStates').val());
            $('#btnCreateNew').hide();
            $('#btnSearchView').show();
        }


    });

 

    
    $("#dvShiftDistrict").dialog({
        autoOpen: false,
        height: 'auto',//"200",
        width:  "450",
        modal: true,
        title: 'Shift District'
    });


   

});

function FormatColumn(cellvalue, options, rowObject) {


    //return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-pencil' title='Edit State Details' onClick ='EditStateDetails(\"" + cellvalue.toString() + "\");'></span></td><td style='border-color:white'><span class='ui-icon ui-icon-trash' title='Delete State Details' onClick ='DeleteStateDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";

  //  return "<center><table><tr><td  style='border-color:white'><a href='#' title='Edit District Details' onClick ='EditDistrictDetails(\"" + cellvalue.toString() + "\");'>Edit</a></td></tr></table></center>";

    if (cellvalue != '') {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit District Details' onClick ='EditDistrictDetails(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete District Details' onClick ='DeleteDistrictDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }
}

function FormatColumnShift(cellvalue, options, rowObject) {

    return "<center><table><tr><td  style='border:none'><a href='#' title='Shift District' onClick ='ShiftDistrict(\"" + cellvalue.toString() + "\");' >Shift District</a></td></tr></table></center>";

}

function EditDistrictDetails(paramater) {

    //alert(paramater);
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        url: "/LocationMasterDataEntry/EditDistrict/" + paramater,
        type: "GET", 
        async: false,
        cache: false,
        //data: $("form").serialize(),
        success: function (data) {

            // $("#mainDiv").html(data);

            if ($("#dvSearchDistrict").is(":visible")) {
                $('#dvSearchDistrict').hide('slow');
            }

            $('#btnCreateNew').hide();
            $('#btnSearchView').show();
            
            $("#dvDistrictDetails").html(data);
            $('#dvDistrictDetails').show('slow');
            $("#MAST_DISTRICT_NAME").focus();

            $.unblockUI();

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();
        }

    });
}

function ShiftDistrict(parameter) {
    
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $('#dvShiftDistrict').empty();
    $("#dvShiftDistrict").load("/LocationMasterDataEntry/ShiftDistrict?id=" + parameter, function () {

        $("#dvShiftDistrict").dialog('open');
        $.unblockUI();    
    })

}

function DeleteDistrictDetails(urlparamater) {

    

    if (confirm("Are you sure you want to delete district details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/LocationMasterDataEntry/DeleteDistrictDetails/" + urlparamater,
            type: "GET",
            dataType: "json",      
            success: function (data) {

                if (data.success) {
                    alert(data.message);
                    if ($("#dvDistrictDetails").is(":visible")) {
                        $('#dvDistrictDetails').hide('slow');

                        $('#btnSearchView').hide();
                        $('#btnCreateNew').show();

                    }
                    if (!$("#dvSearchDistrict").is(":visible")) {
                        $('#dvSearchDistrict').show('slow');
                         $('#tbDistrictList').trigger('reloadGrid');
                        //loadGrid();
                    }
                    else {
                        $('#tbDistrictList').trigger('reloadGrid');

                    }
                    //if ($("#dvSearchDistrict").is(":visible")) {

                    //    $('#btnSearch').trigger('click');
                        
                    //}
                    //else {
                    //    $('#tbDistrictList').trigger('reloadGrid');
                    //}
                    
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
    }
    else {
        return false;
    }
}

function loadGrid() {
    $("#tbDistrictList").jqGrid('GridUnload');
    jQuery("#tbDistrictList").jqGrid({
        url: '/LocationMasterDataEntry/GetDistrictDetailsList',
        datatype: "json",
        mtype: "POST",
        postData:{ stateCode: $('#ddlSearchStates option:selected').val() },
        colNames: ['District Id','District Name', 'State Name', 'Is Included In PMGSY ', 'Is IAP District', 'Active' ,'Shift District', 'Action','Lock Status'],
        colModel: [
                            { name: 'MastDistrictId', index: 'MastDistrictId', height: 'auto', width:90, sortable: true, align: "center" },
                            { name: 'DistrictName', index: 'DistrictName', height: 'auto', width: 200, align: "left", sortable: true },
                            { name: 'StateName', index: 'StateName', height: 'auto', width: 170, sortable: true, align: "left" },
                            { name: 'IsPMGSYIncluded', index: 'IsPMGSYIncluded', width: 140, sortable: true },
                            { name: 'IsIAPDistrict', index: 'IsIAPDistrict', width: 80, sortable: true },
                            { name: 'Active', index: 'Active', width: 80, sortable: true, align: "center" },
                            { name: 'Shift', width: 120, sortable: false, resize: false, formatter: FormatColumnShift, align: "center", hidden: ($("#RoleCode").val() == 23 ? false : true) }, //RoleCode=23 MasterAdmin
                            //{ name: 'a', width: 80, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
                            { name: 'a', width: 80, sortable: false, resize: false, align: "center", sortable: false },
                            { name: 'lockStatus',index: 'lockStatus', width: 80, sortable: true, resize: false, align: "center", hidden: ($("#RoleCode").val() == 23 ? false : true) }

        ],
        pager: jQuery('#dvDistrictListPager'),
        rowNum: 15,
        //  altRows: true,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'StateName,DistrictName,lockStatus',
        sortorder: "asc",
        caption: "District List",
        height: 'auto',
        // width: 'auto',
        autowidth: true,
        rownumbers: true,
        hidegrid: false,
        /*grouping: true,
        groupingView:
            {
                groupField: ['StateName'],
                groupColumnShow: [false],
                groupText: ['<b>State: {0}</b>'],
                //groupCollapse: true,
                groupOrder: ['asc']
            },*/
        loadComplete: function (data) {
            var recordCount = jQuery('#tbDistrictList').jqGrid('getGridParam', 'reccount');
            if (recordCount > 0) {

                var button = '<input type="button" id="btnFinalizeDistrict" name="btnFinalizeDistrict" value="Finalize" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only" title="Finalize State" tabindex="200" style="font-size:1em; margin-left:25px" onclick="FinalizeDistrict()" />'
                $('#dvDistrictListPager_left').html(button);

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

    });

}
function FinalizeDistrict() {
    var id = $('#tbDistrictList').jqGrid('getGridParam', 'selrow');

    if ($('#tbDistrictList').jqGrid('getGridParam', 'selrow')) {

        $.ajax({
            type: 'POST',
            url: '/LocationMasterDataEntry/FinalizeDistrict/' + id,
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {
                    alert('District Finalized Successfully.');
                    $("#tbDistrictList").trigger('reloadGrid');
                }
                else if (data.success == false) {
                    alert('Error occurred while processing your request.');
                }
            },
            error: function () { }
        });
    }
    else {
        alert('Please select district to finalize.');
    }
}