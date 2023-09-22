
$(document).ready(function () {
    $.validator.unobtrusive.parse($('#createRoleForm'));

    //Multiselect for selecting different level groups
    //---------------------------------------------------
    $("#LevelMaster").multiselect({
        minWidth: 150,
        position: {
            my: 'left bottom',
            at: 'left top'
        }
    });

    $("#LevelMaster").multiselect("uncheckAll");

    //---------------------------------------------------

    $("#btnCancel").click(function () {
        $("#divEditRoleList").hide();
    });



    $("#btnAddRole").click(function () {
        if (validate()) {

            if ($(this).val() == "Save") {
                $.ajax({
                    url: "/UserManager/CreateRole",
                    type: "POST",
                    data: $('#createRoleForm').serialize(),
                    beforeSend: function () {
                        blockPage();
                    },
                    error: function (xhr, status, error) {
                        unblockPage();
                        alert(error);
                        alert("Request can not be processed at this time,please try after some time!!!");
                        return false;
                    },
                    cache: false,
                    success: function (response) {
                        if (response.Success) {
                            alert("Role created successfully.");
                            //$("#addDetailsDiv").load("/UserManager/CreateRole");
                            $("#addDetailsDiv").load("/UserManager/ShowRoleList", function () { unblockPage(); });
                            $('#addDetailsDiv').show();
                        }
                        else {
                            $("#divError").show("slow");
                            $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);
                        }
                        unblockPage();
                    }
                });
            }
            else if ($(this).val() == "Update") {
    
                    $.ajax({
                        url: '/UserManager/EditRole',
                        type: "POST",
                        cache: false,
                        data: $("#createRoleForm").serialize(),
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
                                alert("Role updated succesfully");
                                $("#addDetailsDiv").load("/UserManager/ShowRoleList", function () { unblockPage(); });
                                $('#addDetailsDiv').show();
                                $("#tblRoleList").trigger("reloadGrid");
                            }
                            else {
                                $("#divError").show("slow");
                                $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);
                            }
                            unblockPage();
                        }
                    });
             }

        }

    });//Click ends here

    
});//doc.ready() ends here

function validate()
{
    //Get all selected values for Group Code
    if ($('#LevelMaster :selected').length > 0) {
        $("#hidLevelId").val('');
        //build an array of selected values
        var selectednumbers = [];
        $('#LevelMaster :selected').each(function (i, selected) {
            selectednumbers[i] = $(selected).val();

            //Set Hideen varible value for Level Id
            if ($("#hidLevelId").val() == "")
                $("#hidLevelId").val(selectednumbers[i]);
            else
                $("#hidLevelId").val($("#hidLevelId").val() + "," + selectednumbers[i]);


            //Alert($("#hidLevelId").val());
            $("#showLevelError").html("");
            $("#showLevelError").removeClass("field-validation-error");
        });

        return true;
    }
    else {

        $("#showLevelError").html("Map at least one of the Levels");
        $("#showLevelError").addClass("field-validation-error");
        return false;
    }

}//validate ends here


