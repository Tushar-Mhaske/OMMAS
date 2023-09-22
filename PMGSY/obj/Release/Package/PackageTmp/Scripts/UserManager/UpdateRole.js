$(document).ready(function () {

    $("#btnUpdateRole").click(function () {

        if (validate()) {
            $.ajax({
                url: "/UserManager/EditRole",
                type: "POST",
                data: $('#EditRoleForm').serialize(),
                cache: false,

                error: function (xhr, status, error) {

                    alert("Request can not be processed at this time,please try after some time!!!");
                    return false;
                },

                success: function (response) {
                    if (response.Success) {
                        alert("Role Updated successfully.");
                        $('#mainDiv').load("/UserManager/RoleMaster");
                    }
                    else {
                        $('#mainDiv').html(response);
                    }

                }
            });
        }
    });

    $('input[name="categories"]').click(function () {
        if ($(this).is(':checked')) {
            $("#showLevelError").html("");
            $("#showLevelError").removeClass("field-validation-error");
        }
    });

});

function validate() {
    if ($('input[name="categories"]:checked').length > 0) {
        return true;
    }
    else {
        $("#showLevelError").html("Map At least Level");
        $("#showLevelError").addClass("field-validation-error");
        $('input[name="categories"]').addClass("input-validation-error");
        return false;
    }
    return true;
}
