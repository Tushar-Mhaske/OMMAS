$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmRoleHomePageMapping'));


    $("#ModuleID").change(function () {
        $("#SubModuleID").val(0);
        $("#SubModuleID").empty();
        $("#SubModuleID").append("<option value='0'>Select Sub Module</option>");
        //if ($(this).val() == 0) {

        //}

        if ($("#ModuleID").val() > 0) {

            if ($("#SubModuleID").length > 0) {

                $.ajax({
                    url: '/UserManager/GetSubModules',
                    type: 'POST',
                    data: { moduleId: $("#ModuleID").val(), value: Math.random() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#SubModuleID").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }
        }

    });





    //utton click for Home Page setting
    $('#btnAddHomePage').click(function (evt) {


        if ($(this).val() == "Save") {
            if ($('#frmRoleHomePageMapping').valid()) {
                $.ajax({
                    url: '/UserManager/RoleHomePage',
                    type: "POST",
                    cache: false,
                    data: $("#frmRoleHomePageMapping").serialize(),
                    beforeSend: function () {
                        blockPage();
                    },
                    error: function (xhr, status, error) {
                        unblockPage();
                        alert(error);
                        alert("Request can not be processed at this time,please try after some time!!!");
                        return false;
                    },

                    success: function (response) {
                        if (response.Success) {
                            alert("Home page assigned succesfully");
                            $("#addDetailsDiv").load("/UserManager/RoleHomePage");
                        }
                        else {
                            //$('#addDetailsDiv').html(response);
                            alert(response.Failure);
                        }
                        unblockPage();
                    }
                });
            }
        }   //Save Ends Here
        else if ($(this).val() == "Update") {

            if ($('#frmRoleHomePageMapping').valid()) {
                $.ajax({
                    url: '/UserManager/EditRoleHomePage',
                    type: "POST",
                    cache: false,
                    data: $("#frmRoleHomePageMapping").serialize(),
                    beforeSend: function () {
                        blockPage();
                    },
                    error: function (xhr, status, error) {
                        unblockPage();
                        alert(error);
                        alert("Request can not be processed at this time,please try after some time!!!");
                        return false;
                    },

                    success: function (response) {
                        if (response.Success) {
                            alert("Home page updated succesfully");
                            $("#tblRoleHomePageList").trigger("reloadGrid");
                        }
                        else {
                            alert("Error in home page updation");
                            //$('#addDetailsDiv').html(response);
                        }
                        unblockPage();
                    }
                });
            }
        }
    });//btnAddHomePage ends here





    $("#btnReset").click(function () {
        $('#Mast_District_Code').children('option:not(:first)').remove();
        $('#RoleID').children('option:not(:first)').remove();
        $('#Admin_DPIU_Code').children('option:not(:first)').remove();
    });


    CreateUserListGrid();
    CreateRoleListGrid();
    CreateMenuListGrid();
    CreateRoleHomePageListGrid();



    $("#btnCreateUser").click(function () {

        $('#divEditUserList').show('fold', function () {
            blockPage();
            $("#divEditUserList").load("/UserManager/Create", function () { unblockPage(); });
            $('#divEditUserList').show();
        });

    });

    $("#btnCreateRole").click(function () {

        $('#divEditRoleList').show('fold', function () {
            blockPage();
            $("#divEditRoleList").load("/UserManager/CreateRole", function () { unblockPage(); });
            $('#divEditRoleList').show();
        });

    });


    $("#btnCreateMenu").click(function () {

        $('#divEditMenuList').show('fold', function () {
            blockPage();
            $("#divEditMenuList").load("/Menu/CreateMenu", function () { unblockPage(); });
            $('#divEditMenuList').show();
        });

    });


    $("#btnDisplayHomePageFrm").click(function () {

        //Clear the contents of Div
        $('#formDiv div').html('');

        //Set Text of Clicked href
        $("#formDiv h3").html(
                            '<a id="roleMenuMappingLink" href="#">' + $("#roleHomePageLink").text() +
                            '<a href="#" style="float: right;">' +
                            '<img src="" class="ui-icon ui-icon-closethick" onclick="closeUpdateDetails();" /></a>'
                            );


        $('#formDiv').show('fold', function () {
            blockPage();
            $('#menuDetailsAccordionDiv').hide();
            $('#userDetailsAccordionDiv').hide();
            $("#addDetailsDiv").load("/UserManager/RoleHomePage", function () { unblockPage(); });
            $('#addDetailsDiv').show();
        });
    });

    //----------------Usere Log  Report Detail--------------------------------------

    $("#btnViewLogDetails").click(function () {
        CreateUserLogListGrid();
    });

    $("#ddRole_UserLogDetails").change(function () {
        
        LoadRolewiseUser($("#ddRole_UserLogDetails").val(), $("#ddState_UserLogDetails").val());

    });
    $("#ddState_UserLogDetails").change(function () {
        LoadRolewiseUser($("#ddRole_UserLogDetails").val(), $("#ddState_UserLogDetails").val());

    });
    //if ($('#btnViewLogDetails').is(":visible")) {       
    //    $("#btnViewLogDetails").trigger('click');
    //}

    //----------------------------End--------------------------------------
    //----------Usere Log Access  Report Detail---------------------

    $("#btnViewLogAccessDetails").click(function () {
        CreateUserLogAccessListGrid();
    });
    //if ($('#btnViewLogAccessDetails').is(":visible")) {
    //    $("#btnViewLogAccessDetails").trigger('click');
    //}
    //----------------------------End--------------------------------------

    //-------------------- Elmah_Error Details Starts--------------------------


    $('#btnViewErrorDetails').click(function () {
        CreateElmah_Error();

    });
    if ($('#btnViewErrorDetails').is(":visible")) {
        $("#btnViewErrorDetails").trigger('click');
    }
    //-------------------- Elmah_Error Details Ends--------------------------


 


    //Added by Rohit J.

    $('#btnDelete').click(function (e) {

        var errorId = $("#tblErrorList").jqGrid('getGridParam', 'selarrrow');

        if (errorId != '') {
            $('#EncryptedBlockCodes').val(errorId);
            //        alert($('#EncryptedBlockCodes').val());


            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/UserManager/DeleteErrorRecord?id=" + errorId,
                type: "POST",
                dataType: "json",
                //data: id:,
                success: function (data) {

                    alert(data.message);
                    //$("#tbMapMLAConstituencyBlockList").jqGrid('resetSelection');
                    $("#tblErrorList").trigger('reloadGrid');
                    //$('#ddlSearchDistrict').val('0');

                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }

            });
        }
        else {

            alert('Please select at least one record.');
        }


    });


    $.validator.unobtrusive.parse($('#frmRefreshdata'));

    $("#btnRefreshDetails").click(function () {

        //setTimeout(this, 100);
        //timeout: 10000
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        if ($('#frmRefreshdata').valid())
        {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
             $.ajax({
                url: '/UserManager/AccountRefreshDataPostMethod',
                type: 'POST',
                catche: false,
                data: $("#frmRefreshdata").serialize(),
                async: false,
                success: function (response)
                {
                   

                    if (response.status) {
                        alert("Data Refreshed Successfully..!!");
                    }
                    else {
                        alert("Sorry, data not refreshed.Please try Again Later..");
                    }
                    //$("#loadReport").html(response);
                    $.unblockUI();


                },
                error: function ()
                {
                    $.unblockUI();
                    alert("Sorry, data not refreshed.Please try Again Later..");
                    return false;
                },
            });

        }
        else
        {
             $.unblockUI();
        }
        $.unblockUI();
    });






    // ---------------------End------------------------
}); //doc.ready() ends here



//Validate User Name & Password
function encrypt() {
    if ($('#UserName').val() != '') {
        $('#hiddenPassword').val(hex_md5($('#UserName').val()));
        return true;
    }
}//validate() ends here




//-----------------------------------------------------------------
// User List
//-----------------------------------------------------------------

function CreateUserListGrid() {
    //Admin Home Page -- Edit User

    var pageWidth = $("#tblUserList").parent().width() - 100;
   
    $("#tblUserList").jqGrid({
        url: '/UserManager/UserList',
        datatype: "json",
        mtype: "POST",
        loadError: function (r, st, error) {
            $("#message").html("status is " + r.status);
        },
        height: 'auto',
        rowNum: 30,
        colNames: ["User Name", "Level", "Default Role", "State", "District", "Department", "Mapped User", "Switch Login", "Active", "Lock / Unlock", "IsActive", "IsLocked", "Map Roles", "Menu Rights", "Map Independent Users", "Reset Password", "Edit"],
        colModel: [
                     { name: 'UserName', index: 'UserName', width:( pageWidth * (9 / 100)), align: "left" },
                     { name: 'LevelName', index: 'LevelName', width: (pageWidth * (8 / 100)), align: "left" },
                     { name: 'RoleName', index: 'RoleName', width: (pageWidth * (8 / 100)), align: "left" },
                     { name: 'State', index: 'State', width: (pageWidth * (8 / 100)), align: "left" },
                     { name: 'District', index: 'District', width: (pageWidth * (10 / 100)), align: "left" },
                     { name: 'Department', index: 'Department', width: (pageWidth * (10 / 100)), align: "left" },
                     { name: 'MappedUser', index: 'MappedUser', width: (pageWidth * (8 / 100)), align: "left" },
                     { name: 'SwitchLogin', index: 'SwitchLogin', width: (pageWidth * (5 / 100)), align: "left", search: false, hidden: $("#UserId").val() == 321 ? true : false, sortable: false },
                     { name: 'IsActive', index: 'IsActive', width: (pageWidth * (5 / 100)), align: "center", search: false, sortable: false },
                     { name: 'IsLocked', index: 'IsLocked', width: (pageWidth * (5 / 100)), align: "center", search: false, sortable: false },
                     { name: 'IsActiveVal', index: 'IsActiveVal', width: (pageWidth * (5 / 100)), align: "center", hidden: true, sortable: false },
                     { name: 'IsLockedVal', index: 'IsLockedVal', width: (pageWidth * (5 / 100)), align: "center", hidden: true, sortable: false },
                     { name: 'MapRoles', index: 'MapRoles', width: (pageWidth * (5 / 100)), align: "center", search: false, sortable: false },
                     { name: 'UpdateMenuRights', width: (pageWidth * (5 / 100)), index: 'UpdateMenuRights', align: "left", search: false, sortable: false },
                     { name: 'MapIndependentUsers', width: (pageWidth * (5 / 100)), index: 'MapIndependentUsers', align: "left", search: false, sortable: false },
                     { name: 'ResetPass', index: 'ResetPass', width: (pageWidth * (5 / 100)), align: "left", search: false, sortable: false },
                     { name: 'Edit', index: 'Edit', width: (pageWidth * (5 / 100)), align: "center", search: false, sortable: false }
        ],
        viewrecords: true,
        rownumbers: true,
        rowNum: 5,
        rowList: [5, 10, 15, 20],
        pager: '#divUserListPager',
        sortname: 'UserName',
        sortorder: 'asc',
        //autoWidth: true,
        width:'80%',
        shrinkToFit: false,
        loadComplete: function (rowid) {
            //Hide Title bar
            $(".ui-jqgrid-titlebar").hide();

            $("#gs_UserName").attr('placeholder', 'Search here...');
            $("#gs_LevelName").attr('placeholder', 'Search here...');
            $("#gs_RoleName").attr('placeholder', 'Search here...');
            $("#gs_State").attr('placeholder', 'Search here...');
            $("#gs_District").attr('placeholder', 'Search here...');
            $("#gs_Department").attr('placeholder', 'Search here...');

            var aEdit = $(this).find('a[id^=aEdit]')
            $.each(aEdit, function (index) {
                if (!$(this).hasClass('clickBound')) {
                    $(this).click(function () {
                        var flag = confirm('Are you sure to update user details?');
                        if (flag) {
                            var curUserId = $(this).closest('tr').attr('id');
                            EditUserViewList(curUserId);
                        }
                        return false;
                    });
                    $(this).addClass('clickBound');
                }
            });

            var aLock = $(this).find('a[id^=aLock]')
            $.each(aLock, function (index) {
                if (!$(this).hasClass('clickBound')) {
                    $(this).click(function () {
                        var curUserId = $(this).closest('tr').attr('id');
                        cellValue = $("#tblUserList").jqGrid('getCell', curUserId, 'IsLockedVal');
                        var confirmMessage = "Are you sure to update status?";
                        if (cellValue == "Yes") {
                            confirmMessage = "Are you sure to unlock user?";
                        }
                        else {
                            confirmMessage = "Are you sure to lock user?";
                        }
                        var flag = confirm(confirmMessage);
                        if (flag) {
                            //alert(curUserId);
                            UpdateLockUnlock(curUserId);
                        }
                        return false;
                    });
                    $(this).addClass('clickBound');
                }
            });


            var aActive = $(this).find('a[id^=aActive]')
            $.each(aActive, function (index) {
                if (!$(this).hasClass('clickBound')) {
                    $(this).click(function () {
                        var curUserId = $(this).closest('tr').attr('id');
                        cellValue = $("#tblUserList").jqGrid('getCell', curUserId, 'IsActiveVal');
                        var confirmMessage = "Are you sure to update status?";
                        if (cellValue == "Yes") {
                            confirmMessage = "Are you sure to deactivate user?";
                        }
                        else {
                            confirmMessage = "Are you sure to activate user?";
                        }
                        var flag = confirm(confirmMessage);
                        if (flag) {
                            UpdateActiveDeactive(curUserId);
                        }
                        return false;
                    });
                    $(this).addClass('clickBound');
                }
            });
        },
        beforeSelectRow: function (rowid, e) {
            var $link = $('a', e.target);
            if (e.target.tagName.toUpperCase() === "A" || $link.length > 0) {
                $(this).jqGrid('setSelection', rowid);
                // link exist in the item which is clicked
                return false;
            }
            return true;
        },
        caption: "User List"
    });

    $("#tblUserList").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true });
   // $("#tblUserList").setGridWidth($(window).width());
}//function CreateUserListGrid() ends here
//-------------------------------------------------------------------------------------------------------


function CreateRoleListGrid() {
    //Admin Home Page -- Edit Role
     
    var pageWidth = $("#tblRoleList").parent().width() - 100;
    $("#tblRoleList").jqGrid({
        url: '/UserManager/RoleList',
        datatype: "json",
        mtype: "POST",
        loadError: function (r, st, error) {
            $("#message").html("status is " + r.status);
        },
        height: 'auto',
        rowNum: 30,
        colNames: ["Role", "", "Level", "Active", "Remarks", "Map Menu", "Edit"],
        colModel: [
                     { name: 'RoleName', index: 'RoleName', width: (pageWidth * (12 / 100)), align: "left" },
                     { name: 'LevelID', index: 'LevelID', width: (pageWidth * (7 / 100)), align: "left", hidden: true, search: false },
                     { name: 'LevelName', index: 'LevelName', width: (pageWidth * (12 / 100)), align: "left", search: false },
                     { name: 'IsActive', index: 'IsActive', width: (pageWidth * (12 / 100)), align: "center", search: false, sortable: false },
                     { name: 'Remarks', index: 'Remarks', width: (pageWidth * (20 / 100)), align: "left", search: false, sortable: false },
                     { name: 'MapMenu', index: 'MapMenu', width: (pageWidth * (12 / 100)), align: "left", search: false, sortable: false },
                     { name: 'Edit', index: 'Edit', width: (pageWidth * (12 / 100)), align: "center", search: false, sortable: false }
        ],
        viewrecords: true,
        autoWidth: true,
        rownumbers: true,
        rowNum: 5,
        rowList: [5, 10, 15, 20],
        pager: '#divRoleListPager',
        sortname: 'RoleName',
        sortorder: 'asc',
        loadComplete: function (rowid) {
            //Hide Title bar
            $(".ui-jqgrid-titlebar").hide();

            $("#gs_RoleName").attr('placeholder', 'Search here...');

            var aEdit = $(this).find('a[id^=aEdit]')
            $.each(aEdit, function (index) {
                if (!$(this).hasClass('clickBound')) {
                    $(this).click(function () {
                        var flag = confirm('Are you sure to update role details?');
                        if (flag) {
                            var curRoleId = $(this).closest('tr').attr('id');
                            EditRoleViewList(curRoleId);
                        }
                        return false;
                    });
                    $(this).addClass('clickBound');
                }
            });
        },
        beforeSelectRow: function (rowid, e) {
            var $link = $('a', e.target);
            if (e.target.tagName.toUpperCase() === "A" || $link.length > 0) {
                $(this).jqGrid('setSelection', rowid);
                // link exist in the item which is clicked
                return false;
            }
            return true;
        },
        caption: "Role List"
    });

    $("#tblRoleList").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true });
    //$("#tblRoleList").jqGrid('navGrid', '#divRoleListPager', { edit: false, add: false, del: false });

}//Function CreateRoleListGrid()  ends here

//-------------------------------------------------------------------------------------------------------


function CreateMenuListGrid() {
    //Admin Home Page -- Edit Role
    var pageWidth = $("#tblMenuList").parent().width() - 100;
   
    $("#tblMenuList").jqGrid({
        url: '/UserManager/MenuList',
        datatype: "json",
        mtype: "POST",
        loadError: function (r, st, error) {
            $("#message").html("status is " + r.status);
        },
        height: 'auto',
        rowNum: 30,
        colNames: ["Menu Id", "Menu", "ParentID", "Parent", "Sequence", "Vertical Position", "Active", "Combination Code", "Map Levels", "Edit"],
        colModel: [
                     { name: 'MenuID', index: 'MenuID', width: (pageWidth * (12/100)), align: "center", search: false },
                     { name: 'MenuName', index: 'MenuName', width: (pageWidth * (12 / 100)), align: "left" },
                     { name: 'ParentID', index: 'ParentID', width: (pageWidth * (12 / 100)), align: "left", hidden: true, search: false },
                     { name: 'ParentName', index: 'ParentName', width: (pageWidth * (12 / 100)), align: "left", search: false },
                     { name: 'Sequence', index: 'Sequence', width: (pageWidth * (8 / 100)), align: "center", search: false, sortable: false },
                     { name: 'VerticalLevel', index: 'VerticalLevel', width: (pageWidth * (8 / 100)), align: "center", search: false },
                     { name: 'IsActive', index: 'IsActive', width: (pageWidth * (8 / 100)), align: "center", search: false, sortable: false },
                     { name: 'MenucombinationCode', index: 'MenucombinationCode', width: (pageWidth * (12 / 100)), align: "center", search: false, sortable: false },
                     { name: 'MapLevels', index: 'MapLevels', width: (pageWidth * (8 / 100)), align: "left", search: false, sortable: false },
                     { name: 'Edit', index: 'Edit', width: (pageWidth * (8 / 100)), align: "center", search: false, sortable: false }
        ],
        viewrecords: true,
        rownumbers: true,
        rowNum: 5,
        rowList: [5, 10, 15, 20],
        pager: '#divMenuListPager',
        sortname: 'MenuName',
        sortorder: 'asc',
        loadComplete: function (rowid) {
            //Hide Title bar
            $(".ui-jqgrid-titlebar").hide();

            $("#gs_MenuName").attr('placeholder', 'Search here...');

            var aEdit = $(this).find('a[id^=aEdit]')
            $.each(aEdit, function (index) {
                if (!$(this).hasClass('clickBound')) {
                    $(this).click(function () {
                        var flag = confirm('Are you sure to update menu details?');
                        if (flag) {
                            var curMenuId = $(this).closest('tr').attr('id');

                            EditMenuViewList(curMenuId);
                        }
                        return false;
                    });
                    $(this).addClass('clickBound');
                }
            });
        },
        beforeSelectRow: function (rowid, e) {
            var $link = $('a', e.target);
            if (e.target.tagName.toUpperCase() === "A" || $link.length > 0) {
                $(this).jqGrid('setSelection', rowid);
                // link exist in the item which is clicked
                return false;
            }
            return true;
        },
        caption: "Role List"
    });

    $("#tblMenuList").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true });
    //$("#tblMenuList").jqGrid('navGrid', '#divMenuListPager', { edit: false, add: false, del: false });

}//function CreateMenuListGrid() ends here



function CreateRoleHomePageListGrid() {
    //Admin Home Page -- Edit Role
    $("#tblRoleHomePageList").jqGrid({
        url: '/UserManager/RoleHomePageList',
        datatype: "json",
        mtype: "POST",
        loadError: function (r, st, error) {
            $("#message").html("status is " + r.status);
        },
        height: 'auto',
        rowNum: 30,
        colNames: ["RoleID", "Role", "HomePageId", "Home Page", "ParentID", "Edit"],
        colModel: [
                     { name: 'RoleID', index: 'RoleID', width: 80, align: "center", hidden: true, search: false },
                     { name: 'RoleName', index: 'RoleName', width: 160, align: "left" },
                     { name: 'HomePageId', index: 'HomePageId', width: 160, align: "left", hidden: true, search: false },
                     { name: 'ModuleName', index: 'ModuleName', width: 160, align: "left", search: false },
                     { name: 'ParentID', index: 'ParentID', width: 160, align: "left", hidden: true, search: false },
                     { name: 'Edit', index: 'Edit', width: 100, align: "center", search: false }
        ],
        viewrecords: true,
        rownumbers: true,
        rowNum: 10,
        rowList: [10, 20, 30],
        pager: '#divRoleHomePagePager',
        sortname: 'RoleName',
        sortorder: 'asc',
        loadComplete: function (rowid) {
            //Hide Title bar
            //$(".ui-jqgrid-titlebar").hide();

            $("#gs_RoleName").attr('placeholder', 'Search here...');

            $("#gview_tblRoleHomePageList > .ui-jqgrid-titlebar").hide();
            var aEdit = $(this).find('a[id^=aEdit]')
            $.each(aEdit, function (index) {
                if (!$(this).hasClass('clickBound')) {
                    $(this).click(function () {
                        var flag = confirm('Are you sure to update Home Page?');
                        if (flag) {
                            var curHomePageId = $(this).closest('tr').attr('id');
                            EditRoleHomePageList(curHomePageId);
                        }
                        return false;
                    });
                    $(this).addClass('clickBound');
                }
            });
        },
        beforeSelectRow: function (rowid, e) {
            var $link = $('a', e.target);
            if (e.target.tagName.toUpperCase() === "A" || $link.length > 0) {
                $(this).jqGrid('setSelection', rowid);
                // link exist in the item which is clicked
                return false;
            }
            return true;
        },
        caption: "Role-HomePage List"
    });

    $("#tblRoleHomePageList").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true });

}//function CreateRoleHomePageListGrid() ends here


//function to show User log Detail
function CreateUserLogListGrid() {
    //Admin Home Page -- Edit Role
    var roleCode = $("#ddRole_UserLogDetails option:selected").val();
    var stateCode = $("#ddState_UserLogDetails option:selected").val();
    var userCode = $("#ddUser_UserLogDetails option:selected").val();
    var year = $("#ddYear_UserLogDetails option:selected").val();
    var month = $("#ddMonth_UserLogDetails option:selected").val();

    if (roleCode > 0) {      
            if (year > 0) {
                if (month > 0) {
                    $("#tblLogList").jqGrid('GridUnload');
                    $("#tblLogList").jqGrid({
                        url: '/UserManager/UserLogReportListing',
                        datatype: "json",
                        mtype: "POST",
                        loadError: function (r, st, error) {
                            $("#message").html("status is " + r.status);
                        },
                        height: 'auto',
                        rowNum: 30,
                        colNames: ["User Name", "Login Date Time", "Logout Date Time", "IP Address", "Duration"],
                        colModel: [
                                     { name: 'UserName', index: 'UserName', width: 80, search: false },
                                     { name: 'LoginDateTime', index: 'LoginDateTime', width: 160, align: "center" },
                                     { name: 'LogoutDateTime', index: 'LogoutDateTime', width: 160, align: "center", search: false },
                                     { name: 'IpAddress', index: 'IpAddress', width: 160, align: "left", search: false },
                                     { name: 'Duration', index: 'Duration', width: 160, align: "left", search: false }
                        ],
                        postData: { "RoleCode": roleCode, "StateCode": stateCode,"UserCode": userCode,"Year": year, "Month": month },
                        viewrecords: true,
                        rownumbers: true,
                        rowNum: 10,
                        rowList: [10, 20, 30],
                        pager: '#divLogListPager',
                        sortname: 'UserName',
                        sortorder: 'asc',
                        loadComplete: function () {

                        },

                    });

                }
                else {
                    alert("Please select Month.");
                }
            }
            else {
                alert("Please select Year.");
            }        
       
    } else {
        alert("Please select Role.");
    }

}//function CreateUserLogListGrid() ends here

//District Change Fill Block DropDown List
function LoadRolewiseUser(roleId, stateCode) {
    $("#ddUser_UserLogDetails").val(0);
    $("#ddUser_UserLogDetails").empty();

    if (roleId> 0) {
        if ($("#ddUser_UserLogDetails").length > 0) {
            $.ajax({
                url: '/UserManager/PopulateRoleWiseUSer',
                type: 'POST',
                data: { "RoleCode": roleId, "StateCode": stateCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#ddUser_UserLogDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    } else {
        $("#ddUser_UserLogDetails").append("<option value='0'>All User</option>");
    }
}
// --------------------Delete link code on Elmah_Error Screen Start----------------------
function DeleteUserErrorLog(urlParam) {
    if (confirm("Are you sure you want to delete this record?")) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: "/UserManager/DeleteErrorRecord?id=" + urlParam,
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    $("#tblErrorList").trigger('reloadGrid');

                    $.unblockUI();
                }
                else {
                    alert(data.message);
                    $.unblockUI();
                }


            },
            error: function (xht, ajaxOptions, throwError)
            { alert(xht.responseText); $.unblockUI(); }

        });
    }
    else {
        return false;
    }
}



// --------------------Delete link code on Elmah_Error Screen End----------------------

//-------------------------Elmah_Error Function  Starts Here------------------
function CreateElmah_Error() {
    //Admin Home Page -- Edit Role
    var year = $("#ddYear_UserErrorDetails option:selected").val();
    var month = $("#ddMonth_UserErrorDetails option:selected").val();
    if (year > 0) {
        if (month > 0) {
            $("#tblErrorList").jqGrid('GridUnload');
            $("#tblErrorList").jqGrid({

                url: '/UserManager/ErrorList',
                datatype: "json",
                mtype: "POST",
                loadError: function (r, st, error) {
                    $("#message").html("status is " + r.status);
                },
                height: 'auto',
                rowNum: 30,
                colNames: ["User", "Message", "Source", "Type", "Host", "Status Code", "Delete"],
                colModel: [
                             { name: 'User', index: 'User', width: 80, search: false },
                             { name: 'Message', index: 'Message', width: 300, align: "left" },
                             { name: 'Source', index: 'Source', width: 160, align: "left", search: false },
                             { name: 'Type', index: 'Type', width: 160, align: "left", search: false },
                             { name: 'Host', index: 'Host', width: 100, align: "left", search: false },
                             { name: 'Status Code', index: 'Status Code', width: 80, align: "center", search: false },
                             { name: 'Delete', index: 'Host', width: 50, align: "center", search: false },
                ],
                postData: { "Year": year, "Month": month },
                viewrecords: true,
                rownumbers: true,
                rowNum: 10,
                rowList: [10, 20, 30],
                pager: '#divLogErrorPager',
                sortname: 'User',
                sortorder: 'asc',
                multiselect: true,
                loadComplete: function () {

                },

            });

        }
        else {
            alert("Please select Month.");
        }
    }
    else {
        alert("Please select Year.");
    }


}
//-------------------------Elmah_Error Function Ends Heare--------------------

//-----------------------------End--------------------------------------------------------------------------

function CreateUserLogAccessListGrid() {
    //Admin Home Page -- Edit Role
    var moduleCode = $("#ddModule_UserLogAccessDetails option:selected").val();
    var year = $("#ddYear_UserLogAccessDetails option:selected").val();
    var month = $("#ddMonth_UserLogAccessDetails option:selected").val();
    if (moduleCode > 0) {
        if (year > 0) {
            if (month > 0) {
                $("#tblLogAccessList").jqGrid('GridUnload');
                $("#tblLogAccessList").jqGrid({
                    url: '/UserManager/UserLogAccessReportListing',
                    datatype: "json",
                    mtype: "POST",
                    loadError: function (r, st, error) {
                        $("#message").html("status is " + r.status);
                    },
                    height: 'auto',
                    rowNum: 30,
                    colNames: ["User Name", "Module Name", "Request Type", "URL Access", "IP Address", "Date Time"],
                    colModel: [
                                 { name: 'UserName', index: 'UserName', width: 80, search: false },
                                 { name: 'ModuleName', index: 'ModuleName', width: 160, align: "center" },
                                 { name: 'RequestType', index: 'RequestType', width: 50, align: "center", search: false },
                                 { name: 'URLAccessed', index: 'URLAccessed', width: 230, align: "left", search: false },
                                 { name: 'IPAddress', index: 'IPAddress', width: 120, align: "left", search: false },
                                 { name: 'TimeStamp', index: 'TimeStamp', width: 120, align: "left", search: false }

                    ],
                    postData: { "ModuleCode": moduleCode, "Year": year, "Month": month },
                    viewrecords: true,
                    rownumbers: true,
                    rowNum: 10,
                    rowList: [10, 20, 30],
                    pager: '#divLogAccessListPager',
                    sortname: 'UserName',
                    sortorder: 'asc',
                    loadComplete: function () {

                    },

                });

            }
            else {
                alert("Please select Month.");
            }
        }
        else {
            alert("Please select Year.");
        }
    }
    else {
        alert("Please select Module.");
    }

}//function CreateUserLogAccessListGrid() ends here


//---------------------------------End----------------------------------------------------------------------
//function to edit Users
function EditUserViewList(userId) {
    // Alert(userId);
    $.post('/Home/encryptUrl/', { id1: userId, id2: Math.random() }, function (key, value) {
        // alert(key);
        $.ajax({
            url: '/UserManager/Edit/' + key,
            type: "GET",
            async: false,
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
                unblockPage();
                $('#ErrorMessage').html(xhr.responseText);
                $('#ui-widget').show();
                $("#ErrorMessage").stop().show('slow');
                alert("ErrorMessage");
            },
            success: function (data) {
                $('#ErrorMessage').html("");
                $('#ui-widget').hide();

                $("#divEditUserList").html(data);
                $("#divEditUserList").show();
                unblockPage();
            }
        });
    }, "json");
}




//function to edit Roles
function EditRoleViewList(roleId) {
    // Alert(roleId);
    $.post('/Home/encryptUrl/', { id1: roleId, id2: Math.random() }, function (key, value) {
        // alert(key);
        $.ajax({
            url: '/UserManager/EditRole/' + key,
            type: "GET",
            async: false,
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
                unblockPage();
                $('#ErrorMessage').html(xhr.responseText);
                $('#ui-widget').show();
                $("#ErrorMessage").stop().show('slow');
                alert("ErrorMessage");
            },
            success: function (data) {
                $('#ErrorMessage').html("");
                $('#ui-widget').hide();

                $("#divEditRoleList").html(data);
                $("#divEditRoleList").show();
                unblockPage();
            }
        });
    }, "json");
}



//function to edit Menus
function EditMenuViewList(menuId) {
    //Alert(menuId);
    $.post('/Home/encryptUrl/', { id1: menuId, id2: Math.random() }, function (key, value) {
        // alert(key);
        $.ajax({
            url: '/Menu/EditMenu/' + key,
            type: "GET",
            async: false,
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
                unblockPage();
                $('#ErrorMessage').html(xhr.responseText);
                $('#ui-widget').show();
                $("#ErrorMessage").stop().show('slow');
                alert("ErrorMessage");
            },
            success: function (data) {
                $('#ErrorMessage').html("");
                $('#ui-widget').hide();

                $("#divEditMenuList").html(data);
                $("#divEditMenuList").show();

                unblockPage();
            }
        });
    }, "json");
}




//function to edit Menus
function EditRoleHomePageList(homePageId) {
    $.post('/Home/encryptUrl/', { id1: homePageId, id2: Math.random() }, function (key, value) {
        $.ajax({
            url: '/UserManager/EditRoleHomePage/' + key,
            type: "GET",
            async: false,
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
                unblockPage();
                $('#ErrorMessage').html(xhr.responseText);
                $('#ui-widget').show();
                $("#ErrorMessage").stop().show('slow');
                alert("ErrorMessage");
            },
            success: function (data) {
                $('#ErrorMessage').html("");
                $('#ui-widget').hide();

                $("#addDetailsDiv").html("");
                $("#addDetailsDiv").html(data);
                //$("#divRoleHomePageList").show();

                unblockPage();
            }
        });
    }, "json");
}




//function to Lock Unlock User
function UpdateLockUnlock(userId) {
    $.post('/Home/encryptUrl/', { id1: userId, id2: Math.random() }, function (key, value) {
        // alert(key);
        $.ajax({
            url: '/UserManager/LockUnLockUser/' + key,
            type: "GET",
            async: false,
            datatype: "json",
            error: function (xhr, status, error) {
                $('#ErrorMessage').html(xhr.responseText);
                $('#ui-widget').show();
                $("#ErrorMessage").stop().show('slow');
                alert("ErrorMessage");
            },
            success: function (data) {
                $('#ErrorMessage').html("");
                $('#ui-widget').hide();
                alert(data.success);
                $("#tblUserList").trigger("reloadGrid");
            }
        });
    }, "json");
}



//function to activate-Deactivate User
function UpdateActiveDeactive(userId) {
    $.post('/Home/encryptUrl/', { id1: userId, id2: Math.random() }, function (key, value) {
        // alert(key);
        $.ajax({
            url: '/UserManager/UpdateActiveStatus/' + key,
            type: "GET",
            async: false,
            datatype: "json",
            error: function (xhr, status, error) {
                $('#ErrorMessage').html(xhr.responseText);
                $('#ui-widget').show();
                $("#ErrorMessage").stop().show('slow');
                alert("ErrorMessage");
            },
            success: function (data) {
                $('#ErrorMessage').html("");
                $('#ui-widget').hide();
                alert(data.success);
                $("#tblUserList").trigger("reloadGrid");
            }
        });
    }, "json");
}

//-------------------------------------------------------------------------------------------------------

///Must be implemented later
function ShowUserRoleMapping(id) {

    $('#divEditUserList').html("");
    $('#divEditUserList').show('fold', function () {
        blockPage();
        $("#divEditUserList").load('/Menu/UserRoleMapping/' + id, function () { unblockPage(); });
        $('#divEditUserList').show();
    });

    jQuery('#tblRoleList').jqGrid('setSelection', id);
}

function ShowRoleMenuMapping(id) {

    $('#divEditRoleList').html("");
    $('#divEditRoleList').show('fold', function () {
        blockPage();
        $("#divEditRoleList").load('/Menu/GetMenuRoleMapping/' + id, function () { unblockPage(); });
        $('#divEditRoleList').show();
    });

    jQuery('#tblRoleList').jqGrid('setSelection', id);
}

function ShowMenuLevelMapping(id) {

    $('#divEditMenuList').html("");
    $('#divEditMenuList').show('fold', function () {
        blockPage();
        $("#divEditMenuList").load('/Menu/GetMenuLevelMapping/' + id, function () { unblockPage(); });
        $('#divEditMenuList').show();
    });

    jQuery('#tblMenuList').jqGrid('setSelection', id);
}

function UpdateMenuRights(id) {

    $('#divEditUserList').html("");
    $('#divEditUserList').show('fold', function () {
        blockPage();
        $("#divEditUserList").load('/Menu/MenuRights/' + id, function () { unblockPage(); });
        $('#divEditUserList').show();
    });

    jQuery('#tblRoleList').jqGrid('setSelection', id);
}


function MapIndependentUsers(id) {

    $('#divEditUserList').html("");
    $('#divEditUserList').show('fold', function () {
        blockPage();
        $("#divEditUserList").load('/UserManager/IndependentUsersMapping/' + id, function () { unblockPage(); });
        $('#divEditUserList').show();
    });

    jQuery('#tblRoleList').jqGrid('setSelection', id);
}


function SwitchLogin(id) {

    if (confirm("Are you sure to switch your role as choosen user?")) {
        //window.history.forward();
        //function noBack() { window.history.forward(); }
        window.location.href = '/UserManager/SwitchAdminAsUser?id=' + id;
    }
    else {
        return;
    }
}



function ResetPassword(userId) {
    if (confirm("Password will be reset as username. Are you sure?")) {
        $.post('/Home/encryptUrl/', { id1: userId, id2: Math.random() }, function (key, value) {
            $.ajax({
                url: '/UserManager/ResetPassword/' + key,
                type: "POST",
                async: false,
                datatype: "json",
                error: function (xhr, status, error) {
                    $('#ErrorMessage').html(xhr.responseText);
                    $('#ui-widget').show();
                    $("#ErrorMessage").stop().show('slow');
                    alert("ErrorMessage");
                },
                success: function (data) {
                    $('#ErrorMessage').html("");
                    $('#ui-widget').hide();
                    alert(data.success);
                    $("#tblUserList").trigger("reloadGrid");
                }
            });
        }, "json");
    }
    else {
        return;
    }
}



//-------------------------------------------------------------------------------------------------------