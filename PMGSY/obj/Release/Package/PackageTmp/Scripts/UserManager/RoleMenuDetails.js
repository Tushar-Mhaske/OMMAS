$(document).ready(function () {


    //Admin Home Page -- Role menu Mapping details
    $("#RoleMenuMappingDetailsDiv").jqGrid({
        url: '/UserManager/RoleMenuMappingDetails',
        datatype: "json",
        mtype: "POST",
        postData: { roleId: $('#hidRoleId').val(), levelId: $('#hidLevelId').val() },
        loadError: function (r, st, error) {
            $("#message").html("status is " + r.status);
        },

        height: 'auto',
        colNames: ["ParentId", "Links", "MenuId", "Tasks Available", "Rights Permitted", "RoleId", "Role Name"],
        colModel: [
             { name: 'ParentID', index: 'ParentId', width: 80, align: "center", hidden: true },
             { name: 'ParentName', index: 'ParentName', width: 200, align: "center", hidden: true },
             { name: 'MenuID', index: 'MenuId', width: 50, align: "center", hidden: true },
             { name: 'MenuName', index: 'MenuName', width: 300, align: "left" },
             { name: 'RightsPermitted', index: 'RightsPermitted', width: 180, align: "left" },
             { name: 'RoleID', index: 'RoleId', width: 80, align: "center", hidden: true },
             { name: 'RoleName', index: 'RoleName', width: 300, align: "center", hidden: true }
        ],
        viewrecords: true,
        rownumbers: true,
        rowNum: 5,
        rowList: [5, 10, 15],
        pager: '#divMenuMappingPager',
        sortname: 'MenuName',
        sortorder: 'asc',
        grouping: true,
        groupingView: {
            groupField: ['RoleName'],
            groupText: ['<b>{0}</b>'],
            groupColumnShow: [false]
            //groupCollapse: true,
        },
        emptyrecords: "no records to display..",
        loadComplete: function (rowid) {
            ////To hide Heading of columns
            // var grid = $("#RoleMenuMappingDetailsDiv");
            // var gview = grid.parents("div.ui-jqgrid-view");
            // gview.children("div.ui-jqgrid-hdiv").hide();

            //Hide Title bar
            $(".ui-jqgrid-titlebar").hide();
        },
        caption: "Menus"
    });







    //Admin Home Page -- Role User Mapping details
    $("#RoleUserMappingDetailsDiv").jqGrid({
        url: '/UserManager/RoleUserMappingDetails',
        datatype: "json",
        mtype: "POST",
        postData: { roleId: $('#hidRoleId').val(), levelId: $('#hidLevelId').val() },
        loadError: function (r, st, error) {
            $("#message").html("status is " + r.status);
        },

        height: 'auto',
        rowNum: 30,
        //colNames: ["Name", "UserName", "City", "Creation Date", "Active"],
        colNames: ["UserName", "Creation Date", "Active"],
        colModel: [
             //{ name: 'FullName', index: 'FullName', width: 180, align: "left" },
             { name: 'UserName', index: 'UserName', width: 200, align: "left" },
             //{ name: 'City', index: 'City', width: 100, align: "left" },
             { name: 'CreationDate', index: 'CreationDate', width: 150, align: "center" },
             { name: 'IsActive', index: 'IsActive', width: 100, align: "center" }
        ],
        viewrecords: true,
        rownumbers: true,
        rowNum: 10,
        rowList: [10, 20, 30],
        pager: '#divRoleUserMappingPager',
        sortname: 'UserName',
        sortorder: 'asc',
        loadComplete: function (rowid) {
            ////To hide Heading of columns
            // var grid = $("#RoleMenuMappingDetailsDiv");
            // var gview = grid.parents("div.ui-jqgrid-view");
            // gview.children("div.ui-jqgrid-hdiv").hide();
            //Hide Title bar
            $(".ui-jqgrid-titlebar").hide();
        },
        caption: "Menus"
    });



});