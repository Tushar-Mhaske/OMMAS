//jQuery event that is triggered after the DOM is ready
$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmMenuRightsMapping'));

    $('#divUserProfile').accordion(
    {
        fillSpace: true
    });

   

    //$("#UserList").change(function () {
    //    $("#RoleList").val(0);
    //    $("#RoleList").empty();

    //    if ($("#UserList").val() == 0)
    //    {
    //        $("#RoleList").append("<option value='0'>Select Role</option>");
    //    }
    //    //Hide User profile DIV
    //    $("#divUserProfile").hide();

    //    $('#tblParent').GridUnload();

    //    if ($("#UserList").val() > 0) {
    //        if ($("#RoleList").length > 0) {
    //            $.ajax({
    //                url: '/Menu/GetUserRoles',
    //                type: 'POST',
    //                data: { selectedUser: $("#UserList").val(), value: Math.random() },
    //                success: function (jsonData) {
    //                    for (var i = 0; i < jsonData.length; i++) {
    //                        $("#RoleList").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
    //                    }
    //                },
    //                error: function (err) {
    //                    alert("error " + err);
    //                }
    //            });
    //        }
    //    }
    //});


    //load the grid when the user selects a role
    $('#RoleList').change(function () {
       
        if ($('#RoleList').val() == 0) {
            Alert("Please select role for updation of menu rights.")
        }
        else {
                $('#tblParent').GridUnload();
                loadGrid($('#tblParent'), '/Menu/GetMenuRights/' + $('#UserID').val() + '/' + $('#RoleList').val());

                //call to User profile
                $.ajax({
                    url: '/UserManager/GetUserProfile',
                    type: 'POST',
                    data: { id: $("#UserID").val(), value: Math.random() },
                    beforeSend: function () {
                        blockPage();
                    },
                    error: function (xhr, status, error) {
                        unblockPage();
                        alert("Request can not be processed at this time,please try after some time!!!");
                        return false;
                    },
                    success: function (response) {

                        $('#tblUserProfile').html('');
                        $('#tblUserProfile').height('100%');
                        $('#tblUserProfile').html(response);
                        $("#divUserProfile").show();

                        if ($("#UserID").val() == 0) {
                            $("#divUserProfile").hide();
                        }
                        unblockPage();

                    }
                }); //ajax ends here
                

            }
    });

});





//function that will initialize the grid
function loadGrid(grid, urlPath) {
    //Alert(urlPath);
    grid.jqGrid({
        url: urlPath,
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Menu Item', 'Add', 'Update', 'Delete', 'Add'],
        colModel: [
                      { name: 'MenuName', index: 'MenuName', width: 250, align: 'left', sortable: false },
                      {
                         name: 'chkboxAdd', index: 'chkboxAdd', sortable: true, width: 100, align: 'center',
                          formatter: "checkbox", formatoptions: {disabled : false}, editable: true,
                          edittype: "checkbox", editoptions: { value: "Yes:No" }
                      },
                      {
                          name: 'chkboxEdit', index: 'chkboxEdit', sortable: true, width: 100, align: 'center',
                          formatter: "checkbox", formatoptions: { disabled: false }, editable: true,
                          edittype: "checkbox", editoptions: { value: "Yes:No" }
                      },
                      {
                          name: 'chkboxDelete', index: 'chkboxDelete', sortable: true, width: 100, align: 'center',
                          formatter: "checkbox", formatoptions: { disabled: false }, editable: true,
                          edittype: "checkbox", editoptions: { value: "Yes:No" }
                      },
                      { name: 'Save', index: 'Save', width: 100, align: 'center', sortable: false }

        ],
        height: 'auto',
        viewrecords: true,
        rownumbers: true,
        rowNum: 10,
        rowList: [10, 20, 30],
        pager: '#divMenuRightsGrid',
        sortname: 'MenuName',
        sortorder: 'asc',
        caption: "Update User Rights",
        emptyrecords: "no records to display..",
        loadComplete: function () {
            $(".ui-jqgrid-titlebar").hide();
            //find the links to update menu rights
            var aAdds = $(this).find('a[id^=aAdd]')
            $.each(aAdds, function (index) {
                if (!$(this).hasClass('clickBound')) {
                    $(this).click(function () {
                        if ($('#UserID').val() == "0") {
                            Alert('Please select a user');
                        }
                        else {
                            var flag = confirm('Are you sure to update rights?');
                            if (flag) {
                                var curMenuId = $(this).closest('tr').attr('id');

                                var dataFromTheRow = jQuery('#tblParent').jqGrid('getRowData', curMenuId);


                                var isAdd = jQuery('#tblParent').jqGrid('getCell', curMenuId, 'chkboxAdd');
                                var isEdit = jQuery('#tblParent').jqGrid('getCell', curMenuId, 'chkboxEdit');
                                var isDelete = jQuery('#tblParent').jqGrid('getCell', curMenuId, 'chkboxDelete');
                                
                                addMenuItem(curMenuId, isAdd, isEdit, isDelete);
                            }
                        }
                        return false;
                    });
                    $(this).addClass('clickBound');
                }
            });
        }

    });
}



//function to map a menu item rights
function addMenuItem(menuId, isAdd, isEdit, isDelete) {
    //Alert($('#UserList').val() +  " -- " + $('#RoleList').val() +  " -- " + menuId + " -- " + isAdd + " -- " + isEdit + " -- " + isDelete);
    $.post('/Home/encryptUrl/', { id1: $('#UserID').val() + '$' + $('#RoleList').val() + '$' + menuId + '$' + isAdd + '$' + isEdit + '$' + isDelete, id2: Math.random() }, function (key, value) {
        $.ajax({
            url: '/Menu/UpdateMenuRights/' + key,
            type: "POST",
            async: false,
            error: function (xhr, status, error) {
                $('#ErrorMessage').html(xhr.responseText);
                $('#ui-widget').show();
                $("#ErrorMessage").stop().show('slow');
            },
            success: function (data) {
                $('#ErrorMessage').html("");
                $('#ui-widget').hide();
                if (data == "1") {
                    alert('Menu rights updated successfully');
                }
                else {
                    alert('Error occurred while updation of Menu rights');
                }
            }
        });
    }, "json");
}
