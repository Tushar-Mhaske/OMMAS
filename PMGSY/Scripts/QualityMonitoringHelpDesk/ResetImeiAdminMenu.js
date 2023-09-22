
//Added By Hrishiksh To provide Functionality of reset Imei in CQCAdmin Role

/// <reference path="../jquery-1.9.1.js" />
/// <reference path="../jquery-1.7.2.intellisense.js" />

//*************CQCAdmin Reset IMEI Details***************

function ResetIMEIDetailsGridAdminMenu() {
    jQuery("#tblQmIMEINoResetDetailsAdmin").jqGrid('setGridState', 'hidden');
    // $('#tblQmIMEINoResetDetailsAdmin').trigger('reloadGrid');
    //$('#divQmIMEINoResetDetails').show();
    $('#tblQmIMEINoResetDetailsAdmin').jqGrid('GridUnload');
    $('#tblQmIMEINoResetDetailsAdmin').jqGrid({
        url: '/QualityMonitoringHelpDesk/ListUsersIMEINoDetail/',
        datatype: 'json',
        mtype: "POST",
        colNames: ['User Id', 'User Name', 'Monitor Name', 'Monitor Type', 'IMEI No.'/*, 'Application Mode'*/, 'Reset IMEI'/*, 'Update'*/, 'Reset Count', "View"],
        colModel: [
            { name: 'UserId', index: 'ADMIN_QM_CODE', width: 150, align: 'left', hidden: true },
            { name: 'UserName', index: 'UserName', width: 250, align: 'left', search: true, sortable: true },
            { name: 'MonitorName', index: 'MonitorName', width: 250, align: 'left', search: true, sortable: false },
            { name: 'QMType', index: 'QMType', width: 100, align: 'center', search: true, sortable: false, search: true },
            { name: 'ImeiNo', index: 'ImeiNo', height: 'auto', width: 220, align: "center", sortable: false, search: true },
            //{ name: 'AppMode', index: 'AppMode', height: 'auto', width: 100, align: "center", sortable: true, search: true },
            { name: 'reset', index: 'reset', height: 'auto', width: 100, align: "center", sortable: false, search: false },
            //{ name: 'update', index: 'update', width: 100, sortable: false, resize: false, align: "left", sortable: false, search: false },
            { name: 'resetCount', index: 'reset', width: 100, sortable: false, resize: false, align: 'center', search: false, hidden: $("#RoleCode").val() == 5 ? false : true },
            { name: 'viewDetails', index: 'view', width: 100, sortable: false, resize: false, align: 'center', search: false, hidden: $("#RoleCode").val() == 5 ? false : true }
        ],
        //postData: { "qm": $("#qmTypeBroadNotList").val(), "State": State },
        pager: jQuery('#divQmIMEINoResetDetailsAdmin'),
        rowNum: 2147483647,
        // rowList: [5, 10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'UserName',
        sortorder: "desc",
        //caption: 'Reset IMEI Details',  //give name to jqgrid
        height: 'auto',
        rownumbers: true,
        //hidegrid: true,     //show small up button to close that jqgrid (like toggle() method in jquery)
        hidegrid: false,     //disappres small up button to close that jqgrid (like toggle() method in jquery)
        autowidth: true,
        emptyrecords: 'No Records Found',
        loadComplete: function () {
            $('#tblQmIMEINoResetDetailsAdmin_rn').html('Sr.<br/>No.');
            $("#gs_UserName").attr('placeholder', 'Search here...');
            $("#gs_MonitorName").attr('placeholder', 'Search here...');
            $("#gs_QMType").attr('placeholder', 'Search here...');
            $("#gs_ImeiNo").attr('placeholder', 'Search here...');
            $("#gs_AppMode").attr('placeholder', 'Search here...');
        },

        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {

                alert(xht.responseText);
                window.location.href = "Login/login";
            }
            else {

                alert("Some Problem Occured Please Try Again");
            }
        }
    });

    $("#tblQmIMEINoResetDetailsAdmin").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true })
}

//To Reset IMEI Number -----------------------
function ResetIMEIMobDetail(id) {

    if (confirm("Are you sure you want to reset IMEI Number?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/QualityMonitoringHelpDesk/UpdateIMEINoResetDetails/" + id,
            type: "GET",
            dataType: "json",
            success: function (data) {

                //  $("#mainDiv").html(data);

                if (data.success == true) {

                    alert(data.message);

                    $('#tblQmIMEINoResetDetailsAdmin').trigger('reloadGrid');

                }
                else if (data.success == false) {
                    if (data.message != "") {

                        alert('false');
                        // $('#dvErrorMessage').show('slow');
                        // $('#spnErrMessage').html(data.message);

                    }

                }
                else {
                    // $("#divNotificationForm").html(data);
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

/*
//To Update IMEI Number ---------
function UpdateIMEIApplicationMode(id) {

    var split = id.split("$");
    var FromMode = "";
    var ToMode = "";
    var substr = split[2].substr(0, 1);
    if (substr == "D") {
        FromMode = "DEMO";
        ToMode = "LIVE";
    }
    else {
        FromMode = "LIVE";
        ToMode = "DEMO";
    }

    if (confirm("Are you sure you want to change Application Mode  From " + FromMode + " To " + ToMode + " ?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/QualityMonitoringHelpDesk/UpdateIMEIApplicationMode/" + id,
            type: "GET",
            dataType: "json",
            success: function (data) {

                //  $("#mainDiv").html(data);

                if (data.success == true) {

                    alert(data.message);

                    $('#tblQmIMEINoResetDetailsAdmin').trigger('reloadGrid');

                }
                else if (data.success == false) {
                    if (data.message != "") {

                        alert('false');
                        // $('#dvErrorMessage').show('slow');
                        // $('#spnErrMessage').html(data.message);

                    }

                }
                else {
                    // $("#divNotificationForm").html(data);
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
*/



function myResetIMEIDialogBox(id, userName) {

    var r = confirm('Are you Sure to Reset The IMEI Number of ' + userName + ' !!!')
    if (r == true) {

        if (reloadResetIMEIData(id)) {
            alert('The Imei Number of Mrs./Mr.' + userName + ' has been Reset Successfully ');
        }
        //else {
        //    jAlert('error', 'Sorry, something Went Wrong!!!!', 'Error Dialog');

        //}
    }
    else {
        $(this).hide();
    }

}

function reloadResetIMEIData(id) {
    return jQuery.ajax({
        url: "/QualityMonitoringHelpDesk/StoreIMEINumber/",
        data: { AdminQmCode: id },
        cache: false,
        type: 'POST',
        success: function (result) {
            if (result) {

                // alert(result);
                jQuery("#tblQmIMEINoResetDetailsAdmin").jqGrid('setGridParam', { url: "/QualityMonitoringHelpDesk/ListUsersIMEINoDetail/", page: 1 }).trigger("reloadGrid");
                //jQuery("#tblQmIMEINoResetDetails").jqGrid('setGridState', 'visible');
                return true;

            }
            else {
                return false;

            }

        }
    });
}

//------On click of search icon to show the IMEI reset information into another grid------

//function ShowIMEIResetDetails(userid,username) {
function ShowIMEIResetDetails(userId) {
    //var arr = useridandusername.split("$");
    $("#tblShowIMEIDetails").trigger("reloadGrid");
    jQuery("#tblShowIMEIDetails").jqGrid("GridUnload");

    gridSlidsUPDown();  //--close the list of IMEI No (Grid name-Reset IMEI Details

    $("#divShowIMEIResetDetailsGrid").show(); //load resetIMEI Grid Details (Grid Name-IMEI Reset Details)
    jQuery("#tblShowIMEIDetails").jqGrid('setGridState', 'hidden');
    //$("#tblShowIMEIDetails").trigger("reloadGrid");

    jQuery("#tblShowIMEIDetails").jqGrid(
        {
            url: "/QualityMonitoringHelpDesk/GetIMEIResetDetails",
            datatype: 'json',
            mtype: 'POST',
            postData: {  //key:val pair -id send to controller
                selectedUserID: function () {
                    return userId;  //this "selectedUserID" will accept ussing formCollection in Controller->DAL
                }
                /* ,
                   userName: function ()
                   {
                       return arr[1];
                   }*/
            },
            colNames: ['User Id', 'Audit Date And Time '],
            colModel: [
                { name: 'UserId', index: 'userid', width: 100, align: 'center', hidden: true, search: false, sortable: false },
                //{ name: 'UserName', index: 'username', width: 100, align: 'center', search: false, sortable: false },
                //{ name: 'AuditDate', index: 'auditdate', width: 100, align: 'center', search: false, sortable: true },
                { name: 'AuditDate', index: 'auditdate', width: 100, align: 'center', search: false, sortable: false },
                //{ name: 'AuditUser', index: 'audituser', width: 100, align: 'center', search: false, sortable: false }
            ],
            index: "UserId", //serialization no will show with this
            pager: jQuery("#bottomNavDivShowIMEIReset"),   //for bottom  details
            rowNum: 5,
            rowList: [5, 10, 15, 20],
            cache: false,
            rownumbers: true,
            recordtext: '{2} record found',
            height: 'auto',
            sortname: 'AuditDate', //-
            sortorder: "desc",  //for sorting type -This will accept through formCollection in Controller->DAL
            viewrecords: true,
            autowidth: true,
            hidegrid: false, //if true show small up button to close that jqgrid (like toggle() method in jquery)
            emptyrecords: 'No Records Found',
            caption: "IMEI Reset Date Information", //Name for Grid,
            loadComplete: function () {
                $("#tblShowIMEIDetails_rn").html("Sr.<br/>No.");  //-- after loading grid Sr.No Will add--

                //$("#divShowIMEIResetDetailsGrid span:first-child").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-closethick").attr("id","closeIcon"); //add class and id dyanmically to span tag
                //$("#divShowIMEIResetDetailsGrid span:first-child").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-closethick");

            },
            /*            gridComplete: function () {
                            alert("gridcomplete")
                            $("#divShowIMEIResetDetailsGrid").append("<span class='ui-icon-closethick'>hi</span>");
                        },*/
            loadError: function (xhr, status, error) {

                if (xhr.responseText == "session expired") {

                    alert(xht.responseText);
                    window.location.href = "Login/login";
                }
                else {

                    alert("Some Problem Occured. Please Try Again");
                }
            }

        }).trigger("reloadGrid");

    //used for pager (bottom nav)
    $("#bottomNavDivShowIMEIReset").jqGrid("navGrid", "#bottomNavDivShowIMEIReset", { add: false, edit: false, del: false, search: false, refresh: false, sort: true }).trigger("reloadGrid");

};//end ShowIMEIResetDetails()


// function to toggle reset IMEI JQgrid UP-Down
//--close the list of IMEI No (Grid name-Reset IMEI Details)
function gridSlidsUPDown() {

    //$("#closeGridId").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");
    $("#closeGridId").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");
    $("#qmIMEINoRestGridsAdmin").slideToggle(300);

}

///change the up down button in grid-Reset IMEI Details
$("#divForGrid").click(function () {

    if ($("#qmIMEINoRestGridsAdmin").is(":visible")) {

        $("#closeGridId").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

        $(this).next("#qmIMEINoRestGridsAdmin").slideToggle(300);

    }

    else {
        $("#closeGridId").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

        $(this).next("#qmIMEINoRestGridsAdmin").slideToggle(300);

        $("#divShowIMEIResetDetailsGrid").hide();
    }
});



//******End Reset IMEI Details**************
//Hrishikesh reset imei end

$(document).ready(function () {
    $('#qmIMEINoRestGridsAdmin').show();

    ResetIMEIDetailsGridAdminMenu(); //call function

});