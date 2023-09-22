$(document).ready(function () {

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

    $("#btnSubmit").click(function () {
        if (validate()) {
            $.ajax({
                type: "POST",
                url: "/UserManager/AddLevelCombination/",
                async: false,
                beforeSend: function () {
                    blockPage();
                },
                data: $("#frmLevelComb").serialize(),
                error: function (xhr, status, error) {
                    unblockPage();
                    alert(xhr.responseText);
                    $('#ErrorMessageBox').show();
                    document.getElementById('ErrorMessage').innerHTML = (xhr.responseText);
                    $("#ErrorMessage").stop().show('slow');
                },
                success: function (data) {
                    
                    $('#ErrorMessageBox').hide();
                    alert('Level Combination Added Successfully.');
                    
                    blockPage();
                    $('#menuDetailsAccordionDiv').hide();
                    $('#userDetailsAccordionDiv').hide();
                    $("#addDetailsDiv").load("/UserManager/ShowRoleList", function () { unblockPage(); });
                    $('#addDetailsDiv').show();
                }
            });
        }
    });


});

function validate() {
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


