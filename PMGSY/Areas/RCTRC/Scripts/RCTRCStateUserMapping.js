﻿$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmAddCdWorks');

    // Date Inspection date
    var agrdate = $('#Agreementdate').text().split('/');;
    var Agdate = new Date(agrdate[2], (parseInt(agrdate[1]) - 1), agrdate[0]);
    //  alert(Agdate)
    var end = new Date();
    //var diff = new Date(end - Agdate);
    var start = new Date(2000, 0, 1);

    var diff = new Date(end - start);
    var days = diff / 1000 / 60 / 60 / 24;
    // alert(Math.floor(days))
    $('#txtDOB').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose Date of Birth',
        maxDate: "0D",
        //  minDate: "-" + Math.floor(days) + "D",
        buttonImageOnly: true,
        buttonText: 'Date of Birth',
        changeMonth: true,
        changeYear: true,
        onSelect: function (selectedDate) {
            $('#txtDOB').trigger('blur');
        }
    });

    // txtDOJ

    $('#txtDOJ').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose Date of Joining Services',
        maxDate: "0D",
        //   minDate: "-" + Math.floor(days) + "D",
        buttonImageOnly: true,
        buttonText: 'Date of Joining Services',
        changeMonth: true,
        changeYear: true,
        onSelect: function (selectedDate) {
            $('#txtDOJ').trigger('blur');
        }
    });

    // txtGraduationDate
    $('#txtGraduationDate').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose Date of Joining Services',
        maxDate: "0D",
        //   minDate: "-" + Math.floor(days) + "D",
        buttonImageOnly: true,
        buttonText: 'Date of Graduation Completion',
        changeMonth: true,
        changeYear: true,
        onSelect: function (selectedDate) {
            $('#txtGraduationDate').trigger('blur');
        }
    });


    $('#txtPostGraduationDate').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose Date of Joining Services',
        maxDate: "0D",
        //    minDate: "-" + Math.floor(days) + "D",
        buttonImageOnly: true,
        buttonText: 'Date of Post Graduation Completion',
        changeMonth: true,
        changeYear: true,
        onSelect: function (selectedDate) {
            $('#txtPostGraduationDate').trigger('blur');
        }
    });


    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

      LoadInspDetailsList();

    $("#btnSave").click(function (e) {

        if ($("#frmAddCdWorks").valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            if (confirm("Are you sure to save this details ? Once saved, can not be updated or deleted.")) {
                $.ajax({
                    type: 'POST',
                    url: '/RCTRC/RCTRC/AddUserStateMapingDetails/',
                    async: false,
                    data: $("#frmAddCdWorks").serialize(),
                    success: function (data) {
                        if (data.success == true) {

                            alert(data.message);
                            ClearDetails();

                            $('#cdWorksType').trigger('reloadGrid');
                            //     LoadInspDetailsList();

                            $('#tbCDWorksList').trigger('reloadGrid');


                            $('#btnReset').trigger('click');

                            //$("#btnCreateNew").show();
                            //$("#cdWorksDetails").hide('slow');
                            //$('#cdWorksType').trigger('reloadGrid');
                            $.unblockUI();
                        }
                        else if (data.success == false) {
                            if (data.message != "") {
                                $('#message').html(data.message);
                                $('#dvErrorMessage').show('slow');
                                $.unblockUI();
                            }

                        }
                        else {
                            $("#cdWorksDetails").html(data);
                        }
                        $.unblockUI();
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.responseText);
                        $.unblockUI();
                    }
                })
            }
            $.unblockUI();


        }
    });

    $("#btnReset").click(function () {
        ClearDetails();

    });




    $("#spCollapseIconCN").click(function () {

        if ($("#dvAddNewCdWorksDetails").is(":visible")) {
            $("#dvAddNewCdWorksDetails").hide("slow");

            // $("#btnCreateNew").show();
        }
        else {
            $("#dvAddNewCdWorksDetails").show("slow");

        }
    });




    $("#btnCancel").click(function (e) {


        $("#btnCreateNew").show();
        $("#cdWorksDetails").hide('slow');

    })

    $("#MAST_CDWORKS_NAME").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

    $("#btnUpdate").click(function (e) {

        if ($("#frmAddCdWorks").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                type: 'POST',
                url: '/Master/EditCdWorksType/',
                async: false,
                data: $("#frmAddCdWorks").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);


                        $("#btnCreateNew").show();
                        $("#cdWorksDetails").hide('slow');
                        $('#cdWorksType').trigger('reloadGrid');
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#cdWorksDetails").html(data);
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }
            })
        }

    });



    $("#UserDetails").change(function ()
    {
    
        loadStateDetails($("#UserDetails").val());

    });



});

function ClearDetails() {
    $('#MAST_CDWORKS_NAME').val('');
    $('#MAST_CDWORKS_CODE').val('');


    $('#dvErrorMessage').hide('slow');
    $('#message').html('');
}


function LoadInspDetailsList() {


    jQuery("#tbCDWorksList").jqGrid({
        url: '/RCTRC/RCTRC/GetUserStateMappingList',
        datatype: "json",
        mtype: "POST",
        // postData: { RSACode: RSACode },
        colNames: ['Username', 'State Name'],
        colModel: [
                            { name: 'UserName', index: 'UserName', height: 'auto', width: 500, align: "left", search: false },
                            { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 500, align: "left", search: false },
                    

        ],
        pager: jQuery('#pagerCDWorksList'),
        rowNum: 15,
        rowList: [30,60,90,120,150],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'UserName',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; TNA State - User Mapping Details",
        height: 'auto',
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function () {
           
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert("Error Occured.");
                //alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }
    });


}



function DeleteRCTRCRegistrationDetails(id) {
    var token = $('input[name=__RequestVerificationToken]').val();
    debugger;
    if (confirm("Are you sure to delete details ?")) {
        $.ajax({
            url: '/RCTRC/RCTRC/DeleteRegdetails/' + id,
            type: "POST",
            cache: false,
            async: false,
            data: { "__RequestVerificationToken": token },
            success: function (response) {
                if (response.Success) {
                    alert("Details deleted successfully.");
                    $('#tbCDWorksList').trigger('reloadGrid');

                }
                else {
                    alert(response.ErrorMessage)
                    $('#tbCDWorksList').trigger('reloadGrid');
                }
                $.unblockUI();
            },
            error: function () {

                $.unblockUI();
                alert("Error : " + error);
                return false;
            }
        });

    }
}




function loadStateDetails(UserID) {
    ClearDetails();

    $('#cdWorksType').trigger('reloadGrid');
    //     LoadInspDetailsList();

    $('#tbCDWorksList').trigger('reloadGrid');


   // $('#btnReset').trigger('click');

    for (var i = 1; i <= 37; i++)
    {
        

        document.getElementById("item" +i).checked = false
        $("#item" + i).show();

    }

    if (UserID > 0) {
        if (true)
        {
            $.ajax({
                url: '/RCTRC/RCTRC/PopulateStateForUser',
                type: 'POST',
                data: { "userID": UserID },
                success: function (jsonData)
                {
                  

                    $.each(jsonData, function (i, item) {

                        document.getElementById("item" + item.Value).checked = false

                        $("#item" + item.Value).hide();
                    });

                    

                  

                },
                error: function (xhr, ajaxOptions, thrownError)
                {
                    alert("error occurred while processing your request.");
                    return false;
                }
            });
        }
    }
    else {

        //$("#DistrictList_StateListRoadDetails").append("<option value='0'>All Districts</option>");
        //$("#BlockList_StateListRoadDetails").empty();
        //$("#BlockList_StateListRoadDetails").append("<option value='0'>All Blocks</option>");

    }
}