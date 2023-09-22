$(document).ready(function () {

    $("#btnAddRole").click(function () {
        window.location.replace("/UserManager/CreateRole");
    });
});

function delRole(urlParam)
{
   
    Confirm('Are you sure you want to delete Role?', function (value) {
        if (value) {
            $.ajax({
                url: "/UserManager/DeleteRole/" + urlParam,
                type: "POST",
                cache: false,
                async: false,
                datatype: "JSON",
                error: function (xhr, status, error) {
                    alert("Request can not be processed at this time,please try after some time!!!");
                    return false;
                },
                success: function (response) {
                    if (response.Success) {
                        alert("Role deleted Successfully.");
                        window.location.replace("/UserManager/RoleMaster");
                        return false;
                    }
                }
            });

        }
    });
}