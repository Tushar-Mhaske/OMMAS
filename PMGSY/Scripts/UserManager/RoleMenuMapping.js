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
        rowNum: 30,
        colNames: ["ParentId", "Links", "MenuId", "Tasks Available", "Rights Permitted", "RoleId", "Role Name"],
        colModel: [
             { name: 'ParentId', index: 'ParentId', width: 80, align: "center", hidden: true },
             { name: 'ParentName', index: 'ParentName', width: 180, align: "center" },
             { name: 'MenuId', index: 'MenuId', width: 50, align: "center", hidden: true },
             { name: 'MenuName', index: 'MenuName', width: 180, align: "left" },
             { name: 'RightsPermitted', index: 'RightsPermitted', width: 180, align: "left" },
             { name: 'RoleId', index: 'RoleId', width: 80, align: "center", hidden: true },
             { name: 'RoleName', index: 'RoleName', width: 180, align: "center", hidden: true }
             
        ],
        viewrecords: true,
        rownumbers: true,
        grouping: true,
        groupingView: {
            groupField: ['ParentName'],
            groupText: ['<b>{0}</b>'],
            //groupColumnShow: [false],
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

});