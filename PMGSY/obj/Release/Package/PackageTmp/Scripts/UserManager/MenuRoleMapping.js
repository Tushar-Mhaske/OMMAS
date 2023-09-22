//-----------------------------------------------File Header-------------------------------------------------
/* 
    Name            :  MenuRoleMapping.js
    Path            :   ~\Scripts\MenuRoleMapping.js
    Description     :   This file is used to write JavaScript and jQuery functions related to Role menu Mapping                       
    Methods         :   $(document).ready(...)
                        $('#ddlRole').change(...)
                        loadGrid(...)
                        addMenuItem(...)
                        deleteMenuItem(...)
    Author          :  Amol U. Jadhav
    Edited By       :  Shyam Yadav
    Organization    :  C-DAC,E-GOV
    Modified        :  04/04/2013        
*/
//-----------------------------------------------------------------------------------------------------------

var parentClick = 0; //used to check if the grid is to be loaded for the first time or not

//jQuery event that is triggered after the DOM is ready
$(document).ready(function () {

    //highlight alternate rows of the tables
    loadGrid($('#tblParent'), '/Menu/GetMenuItems/' + $('#RoleId').val() + '/' + 0);

});






//function that will initialize the grid
function loadGrid(grid, urlPath) {
    grid.GridUnload();

    grid.jqGrid({
        url: urlPath,
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Menu Item', 'Active'],
        colModel: [
                      { name: 'MenuItem', index: 'MenuItem', width: 100, align: 'left', sortable: false },
                      { name: 'Active', index: 'Active', width: 50, align: 'center', sortable: false },
        ],
        autowidth: true,
        sortname: 'MenuItem',
        rowNum: -1,
        height: "auto",
        viewrecords: true,
        rowNum: 10,
        rowList: [10, 20, 30],
        pager: '#divtblParentPager',
        imgpath: '/Content/themes/steel/images',
        emptyrecords: "no records to display..",
        loadComplete: function () {

            //find the links to map role and menu item
            var aAdds = $(this).find('a[id^=aAdd]')
            $.each(aAdds, function (index) {
                if (!$(this).hasClass('clickBound')) {
                    $(this).click(function () {
                        if ($('#RoleId').val() == "0") {
                            Alert('Please select a Role');
                        }
                        else {
                            var flag = confirm('Are you sure you want to map the selected Menu item to the Role?');
                            if (flag) {
                                var curMenuId = $(this).closest('tr').attr('id');
                                var flgChildren = confirm('Do you want to add the child menu items as well?');
                                if (flgChildren) {
                                    addMenuItem(curMenuId, $(this), 'Y');
                                }
                                else {
                                    addMenuItem(curMenuId, $(this), 'N');
                                }
                            }
                        }
                        return false;
                    });
                    $(this).addClass('clickBound');
                }
            });

            //find the links to unmap role and menu item
            var aDeletes = $(this).find('a[id^=aDelete]')
            $.each(aDeletes, function (index) {
                if (!$(this).hasClass('clickBound')) {
                    $(this).click(function () {
                        if ($('#RoleId').val() == "0") {
                            Alert('Please select a Role');
                        }
                        else {
                            var flag = confirm('Are you sure you want to unmap the selected Menu item from the Role?');
                            if (flag) {
                                var curMenuId = $(this).closest('tr').attr('id');
                                deleteMenuItem(curMenuId, $(this));
                            }
                        }
                        return false;
                    });
                    $(this).addClass('clickBound');
                }
            });
        },
        treeGrid: true,
        treeGridModel: 'adjacency',
        ExpandColumn: 'MenuItem',
        Caption:"Tree"
    });
}

//function to map a menu item
function addMenuItem(menuId, aLink, allChildren) {
    //Alert($('#RoleId').val() + " -- " + menuId + " -- " + allChildren);
    $.post('/Home/encryptUrl/', { id1: $('#RoleId').val() + '$' + menuId + '$' + allChildren, id2: Math.random() }, function (key, value) {
        $.ajax({
            url: '/Menu/MapRoleWithMenuItem/' + key,
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
                    Alert('Role successfully mapped with Menu Item');

                    if (allChildren == 'N') {
                        //change the class of the anchor tag
                        aLink.removeClass('ui-icon-plusthick').addClass('ui-icon-circle-close');

                        //unbind the current click event
                        aLink.unbind('click');

                        //bind a new click event
                        aLink.click(function () {

                            if ($('#RoleId').val() == "0") {
                                Alert('Please select a Role');
                            }
                            else {
                                var flag = confirm('Are you sure you want to unmap the selected Menu item from the Role? Please note this will also unmap child menu items (if any)');
                                if (flag) {
                                    deleteMenuItem(menuId, $(this));
                                }
                            }
                            return false;

                        });
                    }
                    else if (allChildren == 'Y') {
                        //unload and load the grid
                        $('#tblParent').GridUnload();
                        loadGrid($('#tblParent'), '/Menu/GetMenuItems/' + $('#RoleId').val() + '/' + 0);
                    }
                }
                else if (data == "2") {
                    Alert('Could not map Role as Parent Menu Item has not yet been mapped');
                }
                else {
                    Alert('Could not map Role with Menu Item');
                }
            }
        });
    }, "json");
}

//function to unmap a menu item
function deleteMenuItem(menuId, aLink) {
    $.post('/Home/encryptUrl/', { id1: $('#RoleId').val() + '$' + menuId, id2: Math.random() }, function (key, value) {
        $.ajax({
            url: '/Menu/UnmapMenuFromRole/' + key,
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
                    Alert('Role successfully unmapped from Menu Item');

                    //unload and load the grid
                    $('#tblParent').GridUnload();
                    loadGrid($('#tblParent'), '/Menu/GetMenuItems/' + $('#RoleId').val() + '/' + 0);
                                        
                }
                else {
                    Alert('Could not unmap Role from Menu Item');
                }
            }
        });
    }, "json");
}


