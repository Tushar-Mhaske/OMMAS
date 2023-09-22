$(document).ready(function () {
    //alert($("#RoleCode").val());
    CreateUserListGrid();
});

function CreateUserListGrid() {
    var act;

    //if($("#RoleCode").val() == "36")
    //{
    //    act = 'getITNOUserList';
    //}
    //else
    //{
    //    act = 'getITNOUserList';
    //}
    //Admin Home Page -- Edit User
    $("#tblITNOUserList").jqGrid({
        
        url: '/UserManager/getITNOUserList',
        datatype: "json",
        mtype: "POST",
        loadError: function (r, st, error) {
            $("#message").html("status is " + r.status);
        },
        height: 'auto',
        rowNum: 30,
        colNames: ["User Name", "Level", "Default Role", "State", "District", "Department", "Mapped User", "Lock / Unlock", "IsLocked", "Reset Password"],
        colModel: [
                     { name: 'UserName', index: 'UserName', width: 140, align: "left" },
                     { name: 'LevelName', index: 'LevelName', width: 70, align: "left" },
                     { name: 'RoleName', index: 'RoleName', width: 130, align: "left" },
                     { name: 'State', index: 'State', width: 80, align: "left" },
                     { name: 'District', index: 'District', width: 80, align: "left" },
                     { name: 'Department', index: 'Department', width: 300, align: "left" },
                     { name: 'MappedUser', index: 'MappedUser', width: 120, align: "left" },
                     
                     { name: 'IsLocked', index: 'IsLocked', width: 45, align: "center", search: false, sortable: false },
                     { name: 'IsLockedVal', index: 'IsLockedVal', width: 40, align: "center", hidden: true, sortable: false },
                     { name: 'ResetPass', index: 'ResetPass', width: 60, align: "left", search: false, sortable: false }
                     
        ],
        viewrecords: true,
        rownumbers: true,
        rowNum: 20,
        rowList: [15, 20, 25, 30],
        pager: '#divITNOUserListPager',
        sortname: 'UserName',
        sortorder: 'asc',
        //autoWidth: true,
        width: 1100,
        shrinkToFit: false,
        loadComplete: function (rowid) {
            //Hide Title bar
            //$(".ui-jqgrid-titlebar").hide();

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
                        cellValue = $("#tblITNOUserList").jqGrid('getCell', curUserId, 'IsLockedVal');
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

    $("#tblITNOUserList").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true });

}//function CreateUserListGrid() ends here

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
                $("#tblITNOUserList").trigger("reloadGrid");
            }
        });
    }, "json");
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
                    $("#tblITNOUserList").trigger("reloadGrid");
                }
            });
        }, "json");
    }
    else {
        return;
    }
}