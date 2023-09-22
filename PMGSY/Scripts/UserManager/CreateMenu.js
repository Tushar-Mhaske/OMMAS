$(document).ready(function () {
    $.validator.unobtrusive.parse($('#createMenuForm'));

    //On load, disable all Combination Level Dropdowns
    $("#MenuCombinationLevelList1").prop("disabled", true);
    $("#MenuCombinationLevelList2").prop("disabled", true);
    $("#MenuCombinationLevelList3").prop("disabled", true);
    
    

    //Spinner for Sequence number
    $("#spinner").spinner({
        min: 1,
        max: 10,
        minWidth: 150
    });

    //set value for Edit option
    if ($("#hidSequence").val().length != 0) {
        $("#spinner").val($("#hidSequence").val());
    }


    $("#spinnerHSequence").spinner({
        min: 1,
        max: 50,
        minWidth: 150
    });

    //set value for Edit option
    if ($("#hidHorizontalSequence").val().length != 0) {
        $("#spinnerHSequence").val($("#hidHorizontalSequence").val());
    }
    

    //---------------------------------------------------




    //Multiselect for selecting different level groups
    //---------------------------------------------------
    $("#LevelGroupList").multiselect({
        minWidth: 150,
        position: {
            my: 'left bottom',
            at: 'left top'
        }
    });

    $("#LevelGroupList").multiselect("uncheckAll");
    
    //---------------------------------------------------




    //---------------Change of Menu Drpodowns ----------------

    $("#ParentID").change(function () {

        $("#MenuCombinationLevelList1").empty();
        $("#MenuCombinationLevelList1").val(0);

        $("#MenuCombinationLevelList2").empty();
        $("#MenuCombinationLevelList2").val(0);

        if ($("#ParentID").val() == -1)
        {
            $('#radio1').attr('checked', 'checked');
            $("#MenuCombinationLevelList1").prop("disabled", true);
            $("#MenuCombinationLevelList2").prop("disabled", true);
            $("#MenuCombinationLevelList3").prop("disabled", true);

            $("#MenuCombinationLevelList3").empty();
            $("#MenuCombinationLevelList3").val(0);

            $("#MenuCombinationLevelList1").append("<option value='0'>Select Combination Level</option>");
            $("#MenuCombinationLevelList2").append("<option value='0'>Select Combination Level</option>");
            $("#MenuCombinationLevelList3").append("<option value='0'>Select Combination Level</option>");
        }

        if ($("#ParentID").val() > -1) {
            if ($("#MenuCombinationLevelList1").length > 0) {
                $.ajax({
                    url: '/Menu/PopulateParentMenu',
                    type: 'POST',
                    data: { selectedMenu: $("#ParentID").val(), value: Math.random() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#MenuCombinationLevelList1").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                    },
                    error: function (err) {
                        alert("error " + err);
                    }
                });
            }
        }//if for ParentID Ends here



        if ($("#ParentID").val() > -1) {


            if ($("#MenuCombinationLevelList2").length > 0) {
                $.ajax({
                    url: '/Menu/PopulateMenuListForCombination',
                    type: 'POST',
                    data: { selectedMenu: $("#ParentID").val(), value: Math.random() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#MenuCombinationLevelList2").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                    },
                    error: function (err) {
                        alert("error " + err);
                    }
                });
            }
        } //if for ParentID ends here

    });//parent ID Change ends here


 
    $("#MenuCombinationLevelList2").change(function () {

        $("#MenuCombinationLevelList3").empty();
        $("#MenuCombinationLevelList3").val(0);

        if ($("#MenuCombinationLevelList2").val() == 0) {
            $("#MenuCombinationLevelList3").append("<option value='0'>Select Combination Level</option>");
        }

        if ($("#MenuCombinationLevelList2").val() > 0) {
            if ($("#MenuCombinationLevelList3").length > 0) {
                $.ajax({
                    url: '/Menu/PopulateMenuListForCombination',
                    type: 'POST',
                    data: { selectedMenu: $("#MenuCombinationLevelList2").val(), value: Math.random() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#MenuCombinationLevelList3").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                    },
                    error: function (err) {
                        alert("error " + err);
                    }
                });
            }
        }
    });

    //-----------------------------------------------------


    //Radio button set for selection of Vertical Position of menu item
    //--------------------------------------------------------------
    $("#radio").buttonset();
  
    //listen for click event on the radio buttons
    $("#radio input[type=radio]").live("click", function () {
        //iterate through each button and switch icons on the checked button
        $("#radio input[type=radio]").each(function () {
            //test if button is checked
            if ($(this).attr("checked")) {

                //set value of checked button to Hidden field 
                $("#hidVerticalLevel").val($(this).val());

                //call to menuCombinationEnableDisable()
                menuCombinationEnableDisable($(this).val());
  
            } 
            //IMPORTANT: you have to use "refresh" to rest the button with the new style.
            $("#radio").buttonset("refresh");
        });
    });

    
    ///condition for Edit Menu options
    if ($("#hidVerticalLevel").val().length != 0) {
        $('#radio' + $("#hidVerticalLevel").val()).attr('checked', 'checked');

        //call to menuCombinationEnableDisable()
        menuCombinationEnableDisable($('#radio' + $("#hidVerticalLevel").val()).val());

        $("#radio").buttonset("refresh");
    }
    else {
        //Default value selected is 1
        $("#hidVerticalLevel").val(1);
    }

    //--------------------------------------------------------------


    $("#btnCancel").click(function () {
        $("#divEditMenuList").hide();
    });


    //Add menu button click
    //--------------------------------------------------------------
    $("#btnAddMenu").click(function () {
        //alert($(this).val());
        if (validate()) {
            if ($(this).val() == "Save") {
            $.ajax({
                url: "/Menu/CreateMenu",
                type: "POST",
                data: $('#createMenuForm').serialize(),
                cache: false,
                beforeSend: function () {
                    blockPage();
                },
                error: function (xhr, status, error) {
                    unblockPage();
                    alert("Request can not be processed at this time,please try after some time!!!");
                    return false;
                },
                success: function (response) {
                    if (response.Success) {
                        alert("Menu created successfully.");
                        $("#addDetailsDiv").load("/UserManager/ShowMenuList", function () { unblockPage(); });
                        $('#addDetailsDiv').show();
                    }
                    else {
                        $("#divError").show("slow");
                        $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);
                    }
                    unblockPage();
                }
            }); //ajax ends here  

         }
         else if ($(this).val() == "Update") {

                $.ajax({
                    url: "/Menu/EditMenu",
                    type: "POST",
                    data: $('#createMenuForm').serialize(),
                    cache: false,
                    beforeSend: function () {
                        blockPage();
                    },
                    error: function (xhr, status, error) {
                        unblockPage();
                        alert("Request can not be processed at this time,please try after some time!!!");
                        return false;
                    },
                    success: function (response) {
                        if (response.Success) {
                            alert("Menu updated successfully.");
                            $("#addDetailsDiv").load("/UserManager/ShowMenuList", function () { unblockPage(); });
                            $('#addDetailsDiv').show();
                            $("#tblMenuList").trigger("reloadGrid");
                        }
                        else {
                            $("#divError").show("slow");
                            $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);
                        }
                        unblockPage();
                    }
                }); //ajax ends here
            }  

        }//if of validate ends here
        
    }); //button click ends here
    //--------------------------------------------------------------

}); // End of doc.ready()




function validate() {
 
    //validate all fields
    //---------------------------------------------------------------------
    if ($.trim($("#MenuName").val()) == "")
    {
        alert("Please enter Menu Name");
        return false;
    }

    if ($("#spinner").spinner("value") == null) {
        alert("Please choose appropriate sequence number");
        return false;
    }

    if ($("#hidVerticalLevel").val() == 2) {
        if ($("#MenuCombinationLevelList1").val() == 0)
        {
            alert("Please choose appropriate combination level for parent");
            return false;
        }
    }

    if ($("#hidVerticalLevel").val() == 3) {

        if ($("#MenuCombinationLevelList1").val() == 0) {
            alert("Please choose appropriate combination level for parent");
            return false;
        }

        if ($("#MenuCombinationLevelList2").val() == 0) {
            alert("Please choose appropriate combination level for child 1");
            return false;
        }
    }

    if ($("#hidVerticalLevel").val() == 4) {

        if ($("#MenuCombinationLevelList1").val() == 0) {
            alert("Please choose appropriate combination level for parent");
            return false;
        }

        if ($("#MenuCombinationLevelList2").val() == 0) {
            alert("Please choose appropriate combination level for child 1");
            return false;
        }

        if ($("#MenuCombinationLevelList3").val() == 0) {
            alert("Please choose appropriate combination level for child 2");
            return false;
        }
    }


    //---------------------------------------------------------------------


    //Get all selected values for Group Code
    if ($('#LevelGroupList :selected').length > 0) {
        //build an array of selected values
        var selectednumbers = [];
        $('#LevelGroupList :selected').each(function (i, selected) {
            selectednumbers[i] = $(selected).val();

            //append selected values as comma seperated and assign to hidden field
            if (i == 0) {
                $("#hidLevelGroupCode").val(selectednumbers[i]);
            }
            else {
                $("#hidLevelGroupCode").val($("#hidLevelGroupCode").val() + "," + selectednumbers[i]);
            }

            $("#showLevelError").html("");
            $("#showLevelError").removeClass("field-validation-error");
        });
    }
    else {
            $("#showLevelError").html("Map at least one of the Levels");
            $("#showLevelError").addClass("field-validation-error");
            return false;
    }


    //Set Combination Value
    var level1 = "";
    var level2 = "";
    var level3 = "";
    var level4 = "";
    var combinationLevelStr = "";
    if ($("#hidVerticalLevel").val() == 1) {

        combinationLevelStr = "0,0,0,";

       

    } else if ($("#hidVerticalLevel").val() == 2) {
        combinationLevelStr = "0,0," + $("#MenuCombinationLevelList1").val() + ",";

        //$("#ParentId").val($("#MenuCombinationLevelList1").val());

    } else if ($("#hidVerticalLevel").val() == 3) {

        combinationLevelStr = "0," + $("#MenuCombinationLevelList1").val() + "," + $("#MenuCombinationLevelList2").val() + ",";

        //$("#ParentId").val($("#MenuCombinationLevelList2").val());

    } else if ($("#hidVerticalLevel").val() == 4) {

        combinationLevelStr = "0," + $("#MenuCombinationLevelList1").val() + "," + $("#MenuCombinationLevelList2").val() + "," + $("#MenuCombinationLevelList3").val() + ",";

        //$("#ParentId").val($("#MenuCombinationLevelList3").val());
    }


    $("#hidMenucombinationCode").val(combinationLevelStr);
    $("#hidSequence").val($("#spinner").spinner("value"));
    
    if ($("#btnAddMenu").val() === "Save") {
        $("#hidHorizontalSequence").val(0);
    }
    else
    {
        $("#hidHorizontalSequence").val($("#spinnerHSequence").spinner("value"));
    }
    

    if ($("#ParentID").val() == -1)
    {
        $("#ParentID").val(0);
    }
    else
    {
        $("#ParentID").val($("#ParentID").val());
    }
       


    return true;

} //validate() ends here




//-------------------------------------------------------------------------------------------------

function menuCombinationEnableDisable(value)
{
    //Conditionally enble/disable dropdowns & set values
    if (value == 1) {
        $("#MenuCombinationLevelList1").prop("disabled", true);
        $("#MenuCombinationLevelList2").prop("disabled", true);
        $("#MenuCombinationLevelList3").prop("disabled", true);

        $("#MenuCombinationLevelList1").val(-1);
        $("#MenuCombinationLevelList2").val(0);
        $("#MenuCombinationLevelList3").val(0);
    }

    if (value == 2) {
        $("#MenuCombinationLevelList1").prop("disabled", false);

        $("#MenuCombinationLevelList2").prop("disabled", true);
        $("#MenuCombinationLevelList3").prop("disabled", true);

        $("#MenuCombinationLevelList2").val(0);
        $("#MenuCombinationLevelList3").val(0);
    }

    if (value == 3) {
        $("#MenuCombinationLevelList1").prop("disabled", false);
        $("#MenuCombinationLevelList2").prop("disabled", false);

        $("#MenuCombinationLevelList3").prop("disabled", true);
        $("#MenuCombinationLevelList3").val(0);


    }

    if (value == 4) {
        $("#MenuCombinationLevelList1").prop("disabled", false);
        $("#MenuCombinationLevelList2").prop("disabled", false);
        $("#MenuCombinationLevelList3").prop("disabled", false);
    }
}


//-------------------------------------------------------------------------------------------------
