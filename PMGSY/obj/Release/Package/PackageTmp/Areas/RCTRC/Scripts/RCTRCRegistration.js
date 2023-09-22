$(document).ready(function ()
{
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
                url: '/RCTRC/RCTRC/AddCdRCTRCRegistration/',
                async: false,
                data: $("#frmAddCdWorks").serialize(),
                success: function (data) {
                    if (data.success==true) {
                    
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
                    else if (data.success == false)
                    {
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
                     if (data.success==true) {
                         alert(data.message);
                     
                     
                         $("#btnCreateNew").show();
                         $("#cdWorksDetails").hide('slow');
                         $('#cdWorksType').trigger('reloadGrid');
                     }
                     else if (data.success==false) {
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



    $("#StateList_StateListRoadDetails").change(function () {
        loadDistrict($("#StateList_StateListRoadDetails").val());

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
        url: '/RCTRC/RCTRC/GetRCTRCRegistrationList',
        datatype: "json",
        mtype: "POST",
       // postData: { RSACode: RSACode },
        colNames: ['Name', 'Birth Date', 'Designation', 'Joining Date', 'Deputatoin (In Months)', 'Mobile', 'Email','State Name','Place of Deputation (District)' , 'Date of Graduation Completion', 'Graduation Degree', 'Date of Post Graduation Completion', 'Post Graduation Degreee', 'Computer at Home ?', 'Computer at Office ?', 'Delete'],
        colModel: [
                            { name: 'RCTRC_Contact_Name', index: 'RCTRC_Contact_Name', height: 'auto', width: 150, align: "left", search: false },
                            { name: 'RCTRC_Contact_DOB', index: 'RCTRC_Contact_DOB', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'RCTRC_Contact_Designation_Text', index: 'RCTRC_Contact_Designation_Text', height: 'auto', width: 150, align: "left", search: false },
                            { name: 'RCTRC_Contact_DOJ', index: 'RCTRC_Contact_DOJ', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'RCTRC_Contact_Deputatoin', index: 'RCTRC_Contact_Deputatoin', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'RCTRC_Contact_Mobile', index: 'RCTRC_Contact_Mobile', height: 'auto', width: 120, align: "left", search: false },
                            { name: 'RCTRC_Contact_eMail', index: 'RCTRC_Contact_eMail', height: 'auto', width: 150, align: "left", search: false },
                               { name: 'STATE', index: 'STATE', height: 'auto', width: 100, align: "left", search: false },
                                  { name: 'DISTRICT', index: 'DISTRICT', height: 'auto', width: 100, align: "left", search: false },
                        

                            { name: 'RCTRC_Contact_Grad_Comp', index: 'RCTRC_Contact_Grad_Comp', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'RCTRC_Contact_Graduation_Text', index: 'RCTRC_Contact_Graduation_Text', height: 'auto', width: 150, align: "left", search: false },
                            { name: 'RCTRC_Contact_PG_Comp', index: 'RCTRC_Contact_PG_Comp', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'RCTRC_Contact_PG_Text', index: 'RCTRC_Contact_PG_Text', height: 'auto', width: 150, align: "left", search: false },
                            { name: 'RCTRC_Contact_CompAtHome', index: 'RCTRC_Contact_CompAtHome', height: 'auto', width: 60, align: "center", search: false },
                            { name: 'RCTRC_Contact_CompAtOffice', index: 'RCTRC_Contact_CompAtOffice', height: 'auto', width: 60, align: "center", search: false },
                                                   
                           
                           
                             
                              
                            { name: 'e', index: 'e', height: 'auto', width: 400, align: "left", search: false , hidden:true}



        ],
        pager: jQuery('#pagerCDWorksList'),
        rowNum: 5,
        rowList: [5, 10],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'RCTRC_Contact_Name',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; TNA Registration Details",
        height: 'auto',
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function () {
            //$("#tbCDWorksList #pagerCDWorksList").css({ height: '31px' });

            //$("#pagerCDWorksList_left").html("<input type='button' style='margin-left:27px' id='idFinalizeExistingRoad' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'RedirectFinalizeRoad();return false;' value='Finalize Audit'/>");

            //unblockPage();
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
                if (response.Success)
                {
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




function loadDistrict(statCode) {
    $("#DistrictList_StateListRoadDetails").val(0);
    $("#DistrictList_StateListRoadDetails").empty();
    //$("#BlockList_StateListRoadDetails").val(0);
    //$("#BlockList_StateListRoadDetails").empty();
    //$("#BlockList_StateListRoadDetails").append("<option value='0'>All Blocks</option>");

    if (statCode > 0) {
        if ($("#DistrictList_StateListRoadDetails").length > 0) {
            $.ajax({
                url: '/RCTRC/RCTRC/DistrictDetails',
                type: 'POST',
                data: { "StateCode": statCode },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#DistrictList_StateListRoadDetails").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                 
                    if ($("#Mast_District_Code").val() > 0) {
                        $("#DistrictList_StateListRoadDetails").val($("#Mast_District_Code").val());
                      
                        $("#DistrictList_StateListRoadDetails").trigger('change');
                    }


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    }
    else {

        $("#DistrictList_StateListRoadDetails").append("<option value='0'>All Districts</option>");
        //$("#BlockList_StateListRoadDetails").empty();
        //$("#BlockList_StateListRoadDetails").append("<option value='0'>All Blocks</option>");

    }
}