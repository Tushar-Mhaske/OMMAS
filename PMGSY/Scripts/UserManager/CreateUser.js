$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmCreateUser'));

   // $('#btnSubmit') text change on update case used on Create.cshtml Page 

    $("#btnResetCreateUser").click(function () {
        $("#RoleID").val(0);
        $("#RoleID").empty();
        $("#RoleID").append("<option value='0'>Select Role</option>");

        $('#Mast_District_Code').children('option:not(:first)').remove();
        $('#RoleID').children('option:not(:first)').remove();
        $('#Admin_DPIU_Code').children('option:not(:first)').remove();

        if ($('#RoleID').val() == 7) {
            $("input,select").removeClass("input-validation-error");
            $('.field-validation-error').html('');
            $('tr.sqmUser').show();
            $('tr.onlyUser').hide();
        } else {
            $("input,select").removeClass("input-validation-error");
            $('.field-validation-error').html('');
            $('tr.sqmUser').hide();
            $('tr.onlyUser').show();
        }
    });


    $("#Mast_State_Code").change(function () {
        $("#Mast_District_Code").val(0);
        $("#Mast_District_Code").empty();
       
        $("#Admin_ND_Code").val(0);
        $("#Admin_ND_Code").empty();
        //alert($(this).val());
        if ($(this).val() == 0) {
            $("#Mast_District_Code").append("<option value='0'>Select District</option>");
            $("#Admin_ND_Code").append("<option value='0'>Select Department</option>");
        }
       
        //$("#Mast_District_Code").append("<option value='0'>Select District</option>");
        //$("#Admin_ND_Code").append("<option value='0'>Select Department</option>");

        //populate districts
        if ($("#Mast_State_Code").val() > 0) {
            if ($("#RoleID").val() == 7) {
                showsqmUserTable();
            }
            if ($("#Mast_District_Code").length > 0) {
                $.ajax({
                    url: '/CommonFilters/GetDistricts',
                    type: 'POST',
                    data: { selectedState: $("#Mast_State_Code").val(), value: Math.random() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#Mast_District_Code").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                    },
                    error: function (err) {
                        alert("error " + err);
                    }
                });
            }


            //populate ND Code
            setTimeout(function () {
                PopulateNDList();
            }, 1000);
           
        }//State code If ends here
    });

    function PopulateNDList() {
        //alert($("#Mast_District_Code").val())
        var districtCode = 0;
        districtCode=($('#Mast_District_Code option').size() == 0) ? districtCode : $("#Mast_District_Code").val();      
        $("#Admin_ND_Code").val(0);
        $("#Admin_ND_Code").empty();
        $("#Admin_ND_Code").append("<option value='0'>Select Department</option>");
        if ($("#Admin_ND_Code").length > 0) {
            $.ajax({
                url: '/UserManager/GetNDCode',
                type: 'POST',
                data: { selectedState: $("#Mast_State_Code").val(), selectedDistrict: districtCode, selectedLevel: $("#LevelID").val(), selectedRole: $("#RoleID").val(), value: Math.random() },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#Admin_ND_Code").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                },
                error: function (err) {
                    alert("error " + err);
                }
            });
        }
    }
    $("#Mast_District_Code").change(function () {
        $("#Admin_DPIU_Code").val(0);
        $("#Admin_DPIU_Code").empty();
        $("#Admin_DPIU_Code").append("<option value='0'>Select DPIU</option>");

        if ($("#Mast_District_Code").val() > 0) {
            if ($("#Admin_DPIU_Code").length > 0) {
                $.ajax({
                    url: '/UserManager/GetDPIU',
                    type: 'POST',
                    data: { selectedDistrict: $("#Mast_District_Code").val(), value: Math.random() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#Admin_DPIU_Code").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                    },
                    error: function (err) {
                        alert("error " + err);
                    }
                });
            }
        }
        PopulateNDList();
    });



    $("#LevelID").change(function () {
        $("#RoleID").val(0);
        $("#RoleID").empty();
        if ($(this).val() == 0) {
            $("#RoleID").append("<option value='0'>Select Role</option>");
        }

        $("#Mast_State_Code").val(0);
        $("#Mast_State_Code").empty();
        if ($(this).val() == 0) {
            $("#Mast_State_Code").append("<option value='0'>Select State</option>");
        }

        if ($("#LevelID").val() > 0) {

            if ($("#RoleID").length > 0) {

                $.ajax({
                    url: '/UserManager/GetRolesList',
                    type: 'POST',
                    data: { selectedLevel: $("#LevelID").val(), value: Math.random() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#RoleID").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }
        }

        //Populate States as per selected Level (Only applicable for State, Dist)
        if ($("#LevelID").val() > 0) {

            if ($("#Mast_State_Code").length > 0) {

                $.ajax({
                    url: '/UserManager/PopulateStates',
                    type: 'POST',
                    data: { selectedLevel: $("#LevelID").val(), value: Math.random() },
                    success: function (jsonData) {
                        for (var i = 0; i < jsonData.length; i++) {
                            $("#Mast_State_Code").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }
        }


    });

    $('#RoleID').change(function () {
        showsqmUserTable();
    });


    $("#SQMID").change(function () {
        if ($("#SQMID").val() > 0) {
            GetSQMUserName($("#Mast_State_Code").val(), $("#RoleID").val(), $("#SQMID").val());
        }
        else {
            $("#UserName").val("");
        }
       
    });
    $("#btnCancel").click(function () {
        $("#divEditUserList").hide();
    });
    $('#RoleID').change(function () {
        PopulateNDList();
    });

    //Button Submit fot User Creation
    $('#btnSubmit').click(function (evt) {
        encrypt();
        evt.preventDefault();
        $("#UserName").removeAttr('disabled');        
        $('#txtUserName').attr("disabled", false);
        if ($("#txtUserName").is(":visible")) {
            $("#UserName").val($('#txtUserName').val());
        }
        //Alert($(this).val());
        if ($(this).val() == "Save") {
            if ($('#frmCreateUser').valid()) {
                $.ajax({
                    url: '/UserManager/Create',
                    type: "POST",
                    cache: false,
                    data: $("#frmCreateUser").serialize(),
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
                            alert("User created succesfully");
                            $("#addDetailsDiv").load("/UserManager/ShowUserList", function () { unblockPage(); });
                            $('#addDetailsDiv').show();
                        }
                        else {
                            $("#divError").show("slow");
                            $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);
                            $('#txtUserName').attr("disabled", true);

                        }
                        unblockPage();
                    }
                });
            }
        }   //Save Ends Here
        else if ($(this).val() == "Update") {

            if ($('#frmCreateUser').valid()) {
                $.ajax({
                    url: '/UserManager/Edit',
                    type: "POST",
                    cache: false,
                    data: $("#frmCreateUser").serialize(),
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
                            alert("User updated succesfully");
                            $("#addDetailsDiv").load("/UserManager/ShowUserList", function () { unblockPage(); });
                            $('#addDetailsDiv').show();
                            $("#tblUserList").trigger("reloadGrid");
                        }
                        else {
                            $("#divError").show("slow");
                            $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.ErrorMessage);
                            $('#txtUserName').attr("disabled", true);
                        }
                        unblockPage();
                    }
                });
            }
        }
    });//btnSubmit ends here





}); //doc.ready() ends here

//District Change Fill Block DropDown List
function populateSQM(stateCode,roleId) {
    $("#SQMID").val(0);
    $("#SQMID").empty();

    if (roleId > 0) {
        if ($("#SQMID").length > 0) {
            $.ajax({
                url: '/UserManager/PopulateMonitors',
                type: 'POST',
                data: {"isPopulateFirstSelect":true,"qmType":"S" ,"stateCode": stateCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#SQMID").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }                 

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    } else {
        $("#SQMID").append("<option value='0'>Select SQM</option>");
    }
}

function GetSQMUserName(stateCode,roleId,sqmId)
{
    // $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
    if (roleId == 7) {
        $("#txtUserName").val("");
        $.ajax({
            url: '/UserManager/GetSQMUserName/',
            type: 'POST',
            catche: false,
            // data: $("#frmAnaDataProposal").serialize(),
            data: { "selectedState": stateCode, "selectedSQM": sqmId, "qmType": "S" },
            async: false,
            success: function (response) {                           
                          
                $("#txtUserName").val(response.data);
            },
            error: function () {
                //$.unblockUI();
                //alert("An Error");
                $("#txtUserName").val("");
                return false;
            },
        });
    }
    else {
        $("#txtUserName").val("");

    }
}

function showsqmUserTable() {
    $("input,select").removeClass("input-validation-error");
    $('.field-validation-error').html('');
    $("#RoleID").val() > 0
    {
        $('#txtUserName').val("")
        if ($("#RoleID").val() == 7) /*SQM User */ {
            if ($("#Mast_State_Code").val() > 0) {
                $('tr.sqmUser').show();
                $('tr.onlyUser').hide();
                //$('.sqmUser').show();
                //$('.onlyUser').hide();
                populateSQM($("#Mast_State_Code").val(), $("#RoleID").val());

            }
            else {
                alert("Please select State");
            }

        }
        else {
            $('tr.sqmUser').hide();
            $('tr.onlyUser').show();

            //$('.sqmUser').hide();
            //$('.onlyUser').show();
        }
    }
}