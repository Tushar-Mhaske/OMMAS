////added By Pradip Patil 2-jan-2017

$(document).ready(function () {
 
    populateDPIU();
    $('#btnAccView').click(function () {
         if ($('#frmAccountant').valid()) {
            $('#AccUserTable').setGridParam({
                url: '/Accountant/AccUserList', datatype: 'json'
            });

            $('#AccUserTable').jqGrid("setGridParam", { "postData": { roleCode: $('#lstRole option:selected').val(), stateCode: $('#lstSRRDA option:selected').val(), PIUCode: $('#lstDPIU option:selected').val() } });

            $('#AccUserTable').trigger("reloadGrid", [{ page: 1 }]);
        }
    });
 
    $("#lstSRRDA").change(function () {
        populateDPIU();
    });
    createAccUserListGrid();
});

function populateDPIU() {

    var adminNdCode = $('#lstSRRDA option:selected').val();
    $.ajax({
        url: '/Accountant/PopulateDPIU/',
        type: 'POST',
        data:{id : adminNdCode},
        catche: false,
        error: function (xhr, status, error) {
            alert('An Error occured while processig your request.')
            return false;
        },
        success: function (data) {
            $('#lstDPIU').empty();
            $.each(data, function () {
                $('#lstDPIU').append("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
        }
    });


}


function createAccUserListGrid() {
    // Home page (Accountant)
    var pageWidth = $("#AccUserTable").parent().width() - 100;
  
    $("#AccUserTable").jqGrid({
        url: '/Accountant/AccUserList',
        datatype: "json",
        mtype: "POST",
        loadError: function (xhr,status,error) {
            if (xhr.responseText == "session expired") {

                alert(xht.responseText);
                window.location.href = "Login/login";
            }
            else {
                alert("Invalid Data. Please Check and Try Again!");
            }
        },
        height: 'auto',
        colNames: ["User Name", "Level", "Default Role", "State", "District", "Department", "Mapped User", "Switch Login"],
        colModel: [
                     { name: 'UserName', index: 'UserName', width: (pageWidth * (12 / 100)), align: "left" },
                     { name: 'LevelName', index: 'LevelName', width: (pageWidth * (12 / 100)), align: "left" },
                     { name: 'RoleName', index: 'RoleName', width: (pageWidth * (12 / 100)), align: "left" },
                     { name: 'State', index: 'State', width: (pageWidth * (12 / 100)), align: "left" },
                     { name: 'District', index: 'District', width: (pageWidth * (12 / 100)), align: "left" },
                     { name: 'Department', index: 'Department', width: (pageWidth * (12 / 100)), align: "left" },
                     { name: 'MappedUser', index: 'MappedUser', width: (pageWidth * (10 / 100)), align: "left" },
                     { name: 'SwitchLogin', index: 'SwitchLogin', width: (pageWidth * (8 / 100)), align: "left", search: false, hidden: $("#UserId").val() == 321 ? true : false, sortable: false }
                    
        ],
        viewrecords: true,
        rownumbers: true,
        postData: { roleCode: $('#lstRole option:selected').val(), stateCode: $('#lstSRRDA option:selected').val(), PIUCode: $('#lstDPIU option:selected').val() },
        rowNum: 10,
        rowList: [10, 20, 30, 40],
        pager: '#AccUserPager',
        sortname: 'UserName',
        sortorder: 'asc',
        //autoWidth: true,
        width: '90%',
        shrinkToFit: false,
        loadComplete: function (rowid) {
            
            $("#gs_UserName").attr('placeholder', 'Search here...');
            $("#gs_LevelName").attr('placeholder', 'Search here...');
            $("#gs_RoleName").attr('placeholder', 'Search here...');
            $("#gs_State").attr('placeholder', 'Search here...');
            $("#gs_District").attr('placeholder', 'Search here...');
            $("#gs_Department").attr('placeholder', 'Search here...');
           // $("#gs_MappedUser").attr('placeholder', 'Search here...');
        },
        beforeSelectRow: function (rowid, e) {
            var $link = $('a', e.target);
            if (e.target.tagName.toUpperCase() === "A" || $link.length > 0) {
                $(this).jqGrid('setSelection', rowid);
                return false;
            }
            return true;
        },
        caption: "User List"
    });

    $("#AccUserTable").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, ignoreCase: true });
 
}


function switchUserLogin(id) {

    if (confirm("Are you sure to switch your role as choosen user?")) {
        window.location.href = '/UserManager/SwitchAdminAsUser?id=' + id;
        
    }
    else {
        return;
    }
}