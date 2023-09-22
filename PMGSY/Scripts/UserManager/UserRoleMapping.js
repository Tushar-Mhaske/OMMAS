//jQuery event that is triggered after the DOM is ready
$(document).ready(function () {

   
    $('#divUserProfile').accordion(
    {
        fillSpace: true
    });

    //Multiselect for selecting different level groups
    //---------------------------------------------------
    $("#RoleList").multiselect({
        minWidth: 150,
        height: 250,
        position: {
        my: 'left bottom',
        at: 'left top'
    }
    });

    $("#RoleList").multiselect("uncheckAll");


    var arr = "";
    if ($("#hidRoleStr").val().indexOf("$$") >= 0) {
        arr = $("#hidRoleStr").val().split("$$");

        $("#RoleList").val(arr);
        $("#RoleList").multiselect("refresh");
    }

    //---------------------------------------------------




    $("#btnAddRole").click(function () {
        if (validate()) {

            if (confirm("Are you sure to map selected roles to user?")) {
                $.ajax({
                    url: "/Menu/UserRoleMapping",
                    type: "POST",
                    data: $('#frmUserRoleMapping').serialize(),
                    cache: false,
                    error: function (xhr, status, error) {
                        alert("Request can not be processed at this time,please try after some time!!!");
                        return false;
                    },
                    success: function (response) {
                        if (response.Success) {
                            alert("Role mapped successfully.");
                            $('#menuDetailsAccordionDiv').hide();
                            $('#userDetailsAccordionDiv').hide();
                            $("#addDetailsDiv").load("/UserManager/ShowUserList", function () { unblockPage(); });
                            $('#addDetailsDiv').show();
                        }
                        else {
                            $("#divuserRoleMappingError").show("slow");
                            $("#divuserRoleMappingError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);
                        }
                    }
                });
            }
        }
    });//Click ends here


});


function validate() {
    
    //Get all selected values for Group Code
    if ($('#RoleList :selected').length > 0) {
        //build an array of selected values
        var selectednumbers = [];
        $('#RoleList :selected').each(function (i, selected) {
            selectednumbers[i] = $(selected).val();
            
            //Set Hideen varible value for Level Id
            if ($("#hidRoleId").val() == "")
                $("#hidRoleId").val(selectednumbers[i]);
            else
                $("#hidRoleId").val($("#hidRoleId").val() + "," + selectednumbers[i]);

            //Alert($("#hidLevelId").val());
            $("#showRoleError").html("");
            $("#showRoleError").removeClass("field-validation-error");
        });

        return true;
    }
    else {
        $("#showRoleError").html("Map at least one of the role");
        $("#showRoleError").addClass("field-validation-error");
        return false;
    }

}