

$(document).ready(function () {
    ///Changes for RCPLWE Map Scheme for STA
    $("#dvMapScheme").dialog({
        autoOpen: false,
        height: 'auto',
        width: '820',
        modal: true,
        title: 'Map PMGSY Scheme'
    });

    $.ajax({
        url: "/Master/SearchTechnicalAgency/",
        type: "GET",
        dataType: "html",
        success: function (data) {
            $("#dvSearchAgency").html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {

            alert(xhr.responseText);
        }

    });

    $('#btnAddAgency').click(function (e) {

        if ($("#dvSearchAgency").is(":visible")) {
            $('#dvSearchAgency').hide('slow');
        }

        //if (!$("#dvAgencyDetails").is(":visible")) {

        $("#dvAgencyDetails").load("/Master/AddAdminTechnicalAgency/");

        $('#dvAgencyDetails').show('slow');

        $('#btnAddAgency').hide();
        $('#btnSearchView').show();

        //}

    });

    $('#btnViewAgency').click(function () {

        if ($("#dvSearchAgency").is(":visible")) {
            $('#dvSearchAgency').hide('slow');
        }

        //if (!$("#dvAgencyDetails").is(":visible")) {

        $("#dvAgencyDetails").load("/Master/ViewAdminTechnicalAgencyMapping/");


        $('#dvAgencyDetails').show('slow');

        $('#btnAddAgency').hide('slow');
        $('#btnSearchView').show('slow');

        //}
    });

    $('#btnSearchView').click(function (e) {


        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        if ($("#dvAgencyDetails").is(":visible")) {
            $('#dvAgencyDetails').hide('slow');

            $('#btnSearchView').hide('slow');
            $('#btnAddAgency').show('slow');

        }

        if (!$("#dvSearchAgency").is(":visible")) {

            $('#dvSearchAgency').load('/Master/SearchTechnicalAgency/', function () {

                //LoadGrid();
                $("#tblDistrictAgencyList").jqGrid('GridUnload');
                $('#tblList').trigger('reloadGrid');
                //setTimeout(function () {
                //    $("#tblList").trigger('reloadGrid');
                //}, 200);


                var data = $('#tblList').jqGrid("getGridParam", "postData");


                if (!(data === undefined)) {

                    $('#AgencyType').val(data.AgencyType);
                    $('#AgencyName').val(data.AgencyName);
                }
                $('#dvSearchAgency').show('slow');

            });
        }
        $.unblockUI();
    });

});



function LoadGrid() {
    $("#tblDistrictAgencyList").jqGrid('GridUnload');
    $("#tblList").jqGrid('GridUnload');
    $('#tblList').jqGrid({

        url: '/Master/GetAdminTechnicalAgencyDetails',
        datatype: "json",
        mtype: "POST",
        colNames: ['Technical Agency Name', 'Contact Name', 'Designation', 'Level', 'Map States/Districts', 'Mapped States/Districts', 'Action', 'View'], //, 'Address', 'State Name', 'District Name', 'Contact No 1', 'Contact No 2', 'FAX', 'Mobile Number', 'Email', 'Website', 'Remark'
        colModel: [
                           { name: 'TAName', index: 'TAName', height: 'auto', width: 200, align: "left", sortable: true },
                           { name: 'ContactName', index: 'ContactName', height: 'auto', width: 150, align: "left", sortable: true },
                           { name: 'TADesignation', index: 'TADesignation', height: 'auto', width: 150, align: "left", sortable: true },
                           { name: 'Level', index: 'Level', height: 'auto', width: 80, align: "left", sortable: true },
                           { name: 'Map', width: 90, sortable: false, resize: false, align: "center" },
                           { name: 'Mapped', width: 90, sortable: false, resize: false, align: "center" },
                           { name: 'a', width: 60, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false },
                           { name: 'View', width: 60, sortable: false, resize: false, formatter: FormatColumnView, align: "center", sortable: false }

        ],
        postData: { AgencyType: $('#AgencyType option:selected').val(), AgencyName: $('#AgencyName').val() },
        pager: jQuery('#divPager'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'Level,TAName',
        sortorder: "asc",
        caption: "Technical Agency List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function (data) {
            $('#divPager_left').html("<span class='ui-icon ui-icon-info ui-align-center'>Mapped District facility is only available for Other Agencies.</span>");
        },
    });
}
function editMasterStream(urlParam) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    if ($("#dvMappedAgencyStateDetails").is(":visible")) {
        $('#dvMappedAgencyStateDetails').hide('slow');
    }
    if ($("#dvMapAgencyDistrictDetails").is(":visible")) {
        $('#dvMapAgencyDistrictDetails').hide('slow');
    }
    if ($("#dvMapAgencyStateDetails").is(":visible")) {
        $('#dvMapAgencyStateDetails').hide('slow');
    }
    $.ajax({
        url: "/Master/EditAdminTechnicalAgency/" + urlParam,
        type: "GET",
        dataType: "html",
        async: false,
        catche: false,
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if ($("#dvSearchAgency").is(":visible")) {
                $('#dvSearchAgency').hide('slow');
            }
            $('#btnAddAgency').hide();
            $('#btnSearchView').show();
            $("#dvAgencyDetails").html(data);
            $("#dvAgencyDetails").show();
            $("#ADMIN_TA_NAME").focus();
            $('#trAddNewSearch').show();
            $.unblockUI();
        },
        error: function (xht, ajaxOptions, throwError) {

            alert(xht.responseText);
            $.unblockUI();
        }

    });

}

function viewMasterStream(urlParam) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    if ($("#dvMappedAgencyStateDetails").is(":visible")) {
        $('#dvMappedAgencyStateDetails').hide('slow');
    }
    if ($("#dvMapAgencyDistrictDetails").is(":visible")) {
        $('#dvMapAgencyDistrictDetails').hide('slow');
    }
    if ($("#dvMapAgencyStateDetails").is(":visible")) {
        $('#dvMapAgencyStateDetails').hide('slow');
    }
    $.ajax({
        url: "/Master/ViewAdminTechnicalAgency/" + urlParam,
        type: "GET",
        dataType: "html",
        async: false,
        catche: false,
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if ($("#dvSearchAgency").is(":visible")) {
                $('#dvSearchAgency').hide('slow');
            }
            $('#btnAddAgency').hide();
            $('#btnSearchView').show();
            $("#dvAgencyDetails").html(data);
            $("#dvAgencyDetails").show();
            $("#ADMIN_TA_NAME").focus();
            $('#trAddNewSearch').show();
            $.unblockUI();
        },
        error: function (xht, ajaxOptions, throwError) {

            alert(xht.responseText);
            $.unblockUI();
        }

    });

}

function deleteMasterStream(urlParam) {
    $("#alertMsg").hide(1000);
    if (confirm("Are you sure you want to delete Technical Agency details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        if ($("#dvMappedAgencyStateDetails").is(":visible")) {
            $('#dvMappedAgencyStateDetails').hide('slow');
        }
        if ($("#dvMapAgencyDistrictDetails").is(":visible")) {
            $('#dvMapAgencyDistrictDetails').hide('slow');
        }
        if ($("#dvMapAgencyStateDetails").is(":visible")) {
            $('#dvMapAgencyStateDetails').hide('slow');
        }
        $.ajax({

            url: "/Master/DeleteAdminTechnicalAgency/" + urlParam,
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    if ($("#dvAgencyDetails").is(":visible")) {
                        $('#dvAgencyDetails').hide('slow');
                        $('#btnSearchView').hide('slow');
                        $('#btnAddAgency').show('slow');
                    }
                    if (!$("#dvSearchAgency").is(":visible")) {
                        $("#dvSearchAgency").show('slow');
                        $("#tblList").trigger('reloadGrid');
                    }
                    else {
                        $("#tblList").trigger('reloadGrid');
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

        if (!$("#dvAgencyDetails").is(':visible')) {
            $('#btnSearchView').trigger('click');
            $('#dvSearchAgency').show();
            $('#trAddNewSearch').show();
        }
    }
    else {
        return false;
    }
}

function FormatColumn(cellvalue, options, rowObject) {

    switch ($("#roleCode").val()) {
        case "25":
            return "<span class='ui-icon ui-icon-locked ui-align-center'>";
            break;
        case "36":
            return "<span class='ui-icon ui-icon-locked ui-align-center'>";
            break;
        default:
            return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit Technical Agency Details' onClick ='editMasterStream(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete Technical Agency Details' onClick =deleteMasterStream(\"" + cellvalue.toString() + "\");></span></td></tr></table></center>";
            break;
    }


}

function FormatColumnView(cellvalue, options, rowObject) {

    // alert(cellvalue);
    if (cellvalue != '') {
        return "<center><table><tr><td  style='border:none'><span title='View Technical Agency Details' class='ui-icon ui-icon-zoomin' onClick=viewMasterStream('" + cellvalue.toString() + "'); '></span></td></tr></table></center>";

    }
    else {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Not View' ></span></td></tr></table></center>";
    }


}
//added by ujjwal saket on 29-10-2013 for State Delete
function FormatColumnDeleteState(cellvalue, options, rowObject) {

    // alert(cellvalue);
    if (cellvalue != '') {
        return "<a href='#' title='Click here to delete mapped state' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteMappedState('" + cellvalue.toString() + "'); return false;'>Delete State</a>";

    }
    else {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Not Yet Enabled' ></span></td></tr></table></center>";
    }
}

//added by ujjwal saket on 29-10-2013 for District Delete
function FormatColumnDeleteDistrict(cellvalue, options, rowObject) {

    // alert(cellvalue);
    if (cellvalue != '') {
        return "<a href='#' title='Click here to delete mapped state' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteMappedDistrict('" + cellvalue.toString() + "'); return false;'>Delete State</a>";

    }
    else {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Not Yet Enabled' ></span></td></tr></table></center>";
    }
}


//added by ujjwal saket on 29-10-2013 for finalizing the mapped state
function FormatColumnMappedState(cellvalue, options, rowObject) {

    // alert(cellvalue);
    if (cellvalue != '') {
        return "<center><table><tr><td  style='border:none'><a href='#' title='Click here to Finalize Mapped State' onClick ='FinalizeMappedState(\"" + cellvalue.toString() + "\");' >Finalize</a></td></tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }
}

function FinalizeMappedState(parameter) {

    if (confirm("Are you sure you want to Finalize mapped state?")) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/Master/FinalizeMappedStateAgency/" + parameter,
            type: "POST",
            async: false,
            cache: false,
            dataType: 'json',
            success: function (data) {

                if (data.success == true) {

                    alert("Mapped State Finalized successfully");
                    $("#tbMappedAgencyStateDistrictList").trigger('reloadGrid');
                    $.unblockUI();
                }
                else if (data.success == false) {

                    alert("Mapped State can not be Finalized.");
                    $.unblockUI();
                }

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
//finish addition

//added by ujjwal saket on 29-10-2013 for finalizing the mapped District
function FormatColumnMappedDistrict(cellvalue, options, rowObject) {

    // alert(cellvalue);
    if (cellvalue != '') {
        return "<center><table><tr><td  style='border:none'><a href='#' title='Click here to Finalize Mapped State' onClick ='FinalizeMappedDistrict(\"" + cellvalue.toString() + "\");' >Finalize</a></td></tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }
}

function FinalizeMappedDistrict(parameter) {

    if (confirm("Are you sure you want to Finalize mapped state?")) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/Master/FinalizeMappedDistrictAgency/" + parameter,
            type: "POST",
            async: false,
            cache: false,
            dataType: 'json',
            success: function (data) {

                if (data.success == true) {

                    alert("Mapped District Finalized successfully");
                    $("#tbMappedAgencyStateDistrictList").trigger('reloadGrid');
                    $.unblockUI();
                }
                else if (data.success == false) {

                    alert("Mapped District can not be Finalized.");
                    $.unblockUI();
                }

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
//finish addition

function MapState(parameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        url: "/Master/MapAgencyStates/" + parameter,
        type: "GET",
        async: false,
        cache: false,

        success: function (data) {
            if ($("#dvMappedAgencyStateDetails").is(":visible")) {
                $('#dvMappedAgencyStateDetails').hide('slow');
            }
            if ($("#dvMapAgencyDistrictDetails").is(":visible")) {
                $('#dvMapAgencyDistrictDetails').hide('slow');
            }
            $("#dvMapAgencyStateDetails").html(data);
            $('#dvMapAgencyStateDetails').show('slow');
            $('#trAddNewSearch').hide();
            $('#dvSearchAgency').hide();
            $('#dvAgencyDetails').hide();
            $('#tblList').jqGrid("setGridState", "hidden");
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();
        }

    });
}

function MapDistrict(parameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        url: "/Master/MapAgencyDistricts/" + parameter,
        type: "GET",
        async: false,
        cache: false,

        success: function (data) {

            if ($("#dvMappedAgencyStateDetails").is(":visible")) {
                $('#dvMappedAgencyStateDetails').hide('slow');
            }


            if ($("#dvMapAgencyStateDetails").is(":visible")) {
                $('#dvMapAgencyStateDetails').hide('slow');
            }

            $("#dvMapAgencyDistrictDetails").html(data);
            $('#dvMapAgencyDistrictDetails').show('slow');
            $('#trAddNewSearch').hide();
            $('#dvSearchAgency').hide();
            $('#dvAgencyDetails').hide();
            $('#tblList').jqGrid("setGridState", "hidden");
            $.unblockUI();

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();
        }
    });
}

function MappedState(parameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        url: "/Master/MappedAgencyStateandDistricts/" + parameter,
        type: "GET",
        async: false,
        cache: false,

        success: function (data) {
            if ($("#dvMapAgencyStateDetails").is(":visible")) {
                $('#dvMapAgencyStateDetails').hide('slow');
            }
            if ($("#dvMapAgencyDistrictDetails").is(":visible")) {
                $('#dvMapAgencyDistrictDetails').hide('slow');
            }
            $("#dvMappedAgencyStateDetails").html(data);
            $('#dvMappedAgencyStateDetails').show('slow');
            LoadMappedAgencyStates();
            $('#trAddNewSearch').hide();
            $('#dvSearchAgency').hide();
            $('#dvAgencyDetails').hide();
            $('#tblList').jqGrid("setGridState", "hidden");
            $.unblockUI();

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();
        }

    });
}

function MappedDistrict(parameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        url: "/Master/MappedAgencyStateandDistricts/" + parameter,
        type: "GET",
        async: false,
        cache: false,
        success: function (data) {
            if ($("#dvMapAgencyStateDetails").is(":visible")) {
                $('#dvMapAgencyStateDetails').hide('slow');
            }
            if ($("#dvMapAgencyDistrictDetails").is(":visible")) {
                $('#dvMapAgencyDistrictDetails').hide('slow');
            }
            $("#dvMappedAgencyStateDetails").html(data);
            $('#dvMappedAgencyStateDetails').show('slow');
            LoadMappedAgencyDistricts();
            $('#trAddNewSearch').hide();
            $('#dvSearchAgency').hide();
            $('#dvAgencyDetails').hide();


            $('#tblList').jqGrid("setGridState", "hidden");

            $.unblockUI();

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();
        }

    });
}

function LoadMappedAgencyStates() {

    jQuery("#tbMappedAgencyStateDistrictList").jqGrid({
        url: '/Master/GetStateDetailsList_Mapped',
        datatype: "json",
        mtype: "POST",
        postData: { AgencyCode: $('#EncryptedAgencyCode_Mapped').val(), StateCode: $("#StateCode").val() },
        colNames: ['AdminId', 'State Name', 'State/UT', 'State Type', 'Start Date', 'End Date', 'Finalize', 'Disable', 'Action'],//'State Short Name' ,'Census Code'
        colModel: [
                            { name: 'AdminId', index: 'AdminId', hidden: true },
                            { name: 'StateName', index: 'StateName', height: 'auto', width: 130, align: "left", sortable: true },
                            { name: 'StateUT', index: 'StateUT', height: 'auto', width: 100, sortable: true, align: "left" },
                            { name: 'StateType', index: 'StateType', width: 100, sortable: true },

                           // { name: 'NICStateCode', index: 'NICStateCode', width: 50, sortable: false, hidden: false },
                            { name: 'StartDate', index: 'StartDate', width: 140, sortable: false, align: 'center' },
                            { name: 'EndDate', index: 'EndDate', width: 140, sortable: false, align: 'center' }, //formatter: FormatColumnStateEndDate, },
                            { name: 'Finalize', index: 'Finalize', width: 60, sortable: false, resize: false, formatter: FormatColumnMappedState, align: "center", resizable: false },
                            { name: 'IsDisable', index: 'IsDisable', width: 80, sortable: false, hidden: false, sortable: false, align: 'center' },
                            { name: 'Delete', index: 'Delete', width: 50, sortable: false, hidden: false, formatter: FormatColumnDeleteState }
        ],
        pager: jQuery('#dvMappedAgencyStateDistrictListPager'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "State List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        hidegrid: false,
        sortname: 'StateUT,StateName',
        sortorder: "asc",
        cmTemplate: { title: false },
        loadComplete: function () {
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

    });
}

function LoadMappedAgencyDistricts() {

    jQuery("#tbMappedAgencyStateDistrictList").jqGrid({
        url: '/Master/GetDistrictDetailsList_Mapped_Agency',
        datatype: "json",
        mtype: "POST",
        postData: { AgencyCode: $('#EncryptedAgencyCode_Mapped').val() },
        colNames: ['AdminId', 'District Name', 'State Name', 'Is PMGSY Included', 'Is IAP District', 'Start Date', 'End Date', 'Finalize', 'Disable', 'Action', 'Map Scheme'],
        colModel: [
                            { name: 'AdminId', index: 'AdminId', hidden: true },
                            { name: 'DistrictName', index: 'DistrictName', height: 'auto', width: 150, align: "left", sortable: true },
                            { name: 'StateName', index: 'StateName', height: 'auto', width: 120, align: "left", sortable: true },
                            { name: 'IsPMGSYIncluded', index: 'IsPMGSYIncluded', width: 60, sortable: true },
                            { name: 'IsIAPDistrict', index: 'IsIAPDistrict', width: 60, sortable: true },
                            { name: 'StartDate', index: 'StartDate', width: 140, sortable: false, align: 'center' },
                            { name: 'EndDate', index: 'EndDate', width: 140, sortable: false, align: 'center' },//formatter: FormatColumnDistrictEndDate, },
                            { name: 'Finalize', index: 'Finalize', width: 60, sortable: false, resize: false, formatter: FormatColumnMappedDistrict, align: "center", resizable: false },
                            { name: 'IsDisable', index: 'IsDisable', width: 80, sortable: false, hidden: false, sortable: false, align: 'center' },
                            { name: 'Delete', index: 'Delete', width: 50, sortable: false, hidden: false, formatter: FormatColumnDeleteDistrict },
                             ///Changes for RCPLWE Additional column for Map Scheme for STA
                            { name: 'MapScheme', index: 'MapScheme', width: 50, sortable: false, align: "center", hidden: false, }
        ],
        pager: jQuery('#dvMappedAgencyStateDistrictListPager'),
        rowNum: 15,

        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "District List",
        height: 'auto',

        autowidth: true,
        rownumbers: true,
        hidegrid: false,
        sortname: 'StateName,StateUT',
        sortorder: "asc",
        cmTemplate: { title: false },
        loadComplete: function () {
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

    });
}

//added by Ujjwal Saket on 8/1/2012 to update district end date
function DistrictEndDateDetails(urlparameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        url: "/Master/AddEndDateDistrict/" + urlparameter,
        type: "GET",
        async: false,
        cache: false,
        dataType: "html",
        //data: { tokenId : parameter },
        success: function (data) {


            $("#dvEndDateDialogDistrict").html(data);
            $("#dvEndDateDialogDistrict").show();
            $("#dvEndDateDialogDistrict").dialog("open");
            //$("#dvRescheduleAppointmentDetails").show();
            $.unblockUI();


        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();


        }
    });

}//finish addition

//added by Ujjwal Saket on 8/1/2014 to update state end date
function StateEndDateDetails(urlparameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        url: "/Master/AddEndDateState/" + urlparameter,
        type: "GET",
        async: false,
        cache: false,
        dataType: "html",
        //data: { tokenId : parameter },
        success: function (data) {


            $("#dvEndDateDialogState").html(data);
            $("#dvEndDateDialogState").show();
            $("#dvEndDateDialogState").dialog("open");
            //$("#dvRescheduleAppointmentDetails").show();
            $.unblockUI();


        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();


        }
    });



}//finish addition

function DeleteMappedState(urlparameter) {

    if (confirm("Are you sure you want to delete mapped state?")) {
        $.ajax({
            type: 'POST',
            url: '/Master/DeleteMappedState/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {
                    alert("Mapped state deleted successfully");
                    $("#tbMappedAgencyStateDistrictList").trigger('reloadGrid');
                }
                else if (data.success == false) {
                    alert("Mapped state details is in use and can not be deleted.");
                }
                else {

                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                //alert(xhr.responseText);
            }
        });
    }
    else {
        return false;
    }
}

function DeleteMappedDistrict(urlparameter) {
    if (confirm("Are you sure you want to delete mapped district?")) {
        $.ajax({
            type: 'POST',
            url: '/Master/DeleteMappedDistrict/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {
                    alert("Mapped district deleted successfully");
                    $("#tbMappedAgencyStateDistrictList").trigger('reloadGrid');
                }
                else if (data.success == false) {
                    alert("Mapped district details is in use and can not be deleted.");
                }
                else {

                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                //alert(xhr.responseText);
            }
        });
    }
    else {
        return false;
    }
}
///Changes for RCPLWE Map Scheme for STA
function MapPMGSYScheme(urlparameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        type: 'GET',
        url: '/Master/MapPMGSYScheme/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {

            $("#dvMapScheme").html(data);

            $("#dvMapScheme").dialog('open');

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }
    })
}
