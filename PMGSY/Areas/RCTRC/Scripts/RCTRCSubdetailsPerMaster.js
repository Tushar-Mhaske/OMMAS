$(document).ready(function ()
{
    $.validator.unobtrusive.parse('#frmAddCdWorks1');

    // Date Inspection date
    var agrdate = $('#Agreementdate').text().split('/');;
    var Agdate = new Date(agrdate[2], (parseInt(agrdate[1]) - 1), agrdate[0]);
    //  alert(Agdate)pagerCDWorksList
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
        minDate: "-" + Math.floor(days) + "D",
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
        minDate: "-" + Math.floor(days) + "D",
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
        minDate: "-" + Math.floor(days) + "D",
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
        minDate: "-" + Math.floor(days) + "D",
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

 //   LoadInspDetailsList();

    LoadInspDetailsList($("#ddlContactPerson option:selected").val()) // Load_1
    $("#btnSave1").click(function (e) {

        if ($("#frmAddCdWorks1").valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            if (confirm("Are you sure to save this details ? Once saved, can not be updated or deleted.")) {
                $.ajax({
                    type: 'POST',
                    url: '/RCTRC/RCTRC/AddSubdetails/',
                    async: false,
                    data: $("#frmAddCdWorks1").serialize(),
                    success: function (data) {
                        if (data.success == true) {

                            alert(data.message);
                            ClearDetails1();

                            $('#cdWorksType').trigger('reloadGrid');
                            //     LoadInspDetailsList();

                            $('#tbCDWorksList1').trigger('reloadGrid');


                            $('#btnReset1').trigger('click');

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

    $("#btnReset1").click(function () {
        ClearDetails1();

    });

    $("#btnSearch").click(function () {
        $('#tbCDWorksList').trigger('reloadGrid');
        LoadInspDetailsListSearch($("#ddlContactPerson_View option:selected").val()); // Load_2
    });



    //$("#spCollapseIconCN").click(function () {

    //    if ($("#cdWorksDetails").is(":visible")) {
    //        $("#cdWorksDetails").hide("slow");

    //        $("#btnCreateNew").show();
    //    }
    //});


    $("#spCollapseIconCN1").click(function () {

        if ($("#dvAddNewCdWorksDetails1").is(":visible")) {
            $("#dvAddNewCdWorksDetails1").hide("slow");

            // $("#btnCreateNew").show();
        }
        else {
            $("#dvAddNewCdWorksDetails1").show("slow");

        }
    });


    $("#spCollapseIconCN_View").click(function () {

        if ($("#dvAddNewCdWorksDetails_View").is(":visible")) {
            $("#dvAddNewCdWorksDetails_View").hide("slow");

            // $("#btnCreateNew").show();
        }
        else {
            $("#dvAddNewCdWorksDetails_View").show("slow");

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

        if ($("#frmAddCdWorks1").valid()) {
             $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
             $.ajax({
                 type: 'POST',
                 url: '/Master/EditCdWorksType/',
                 async: false,
                 data: $("#frmAddCdWorks1").serialize(),
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

    //$("#ddlMasterWork").change(function () {
    //    var masterCode = $("#ddlMasterWork").val();
    //    showHide(masterCode);

    //});

});

//function showHide(MasterCode)
//{
//    if (MasterCode == 1)
//    {
//        blockPage();
//        $("#divNewForm").load('/RCTRC/RCTRC/RCTRCRegistration', function () {
//            //$.validator.unobtrusive.parse($('#frmMaintenanceInspection'));
//            unblockPage();
//        });
      
//    }
//    else
//    {
//        blockPage();
//        $("#divNewForm").html('');
//        unblockPage();
//    }

//}



function ClearDetails1() {
    $('#MAST_CDWORKS_NAME').val('');
    $('#MAST_CDWORKS_CODE').val('');
  

    $('#dvErrorMessage').hide('slow');
    $('#message').html('');
}


function LoadInspDetailsList(PersonCode) {
    

    $('#tbCDWorksList').jqGrid('GridUnload');
    var personIDSwitch = PersonCode;

    jQuery("#tbCDWorksList1").jqGrid({
        url: '/RCTRC/RCTRC/GetSubdetailsList',
        datatype: "json",
        mtype: "POST",
        postData: { postdataPersonID: personIDSwitch },
       // postData: { RSACode: RSACode },
        colNames: ['Person Name', 'Master Work Area Element Name', 'Key Area Element Name','Grade','Ability To Work'],
        colModel: [
                            { name: 'Person_NAME', index: 'Person_NAME', height: 'auto', width: 400, align: "left", search: false },
                            { name: 'MasterElement_Name', index: 'MasterElement_Name', height: 'auto', width: 250, align: "left", search: false },
                            { name: 'SubElement_Name', index: 'SubElement_Name', height: 'auto', width: 250, align: "left", search: false },
                            { name: 'Grade', index: 'Grade', height: 'auto', width: 250, align: "center", search: false },
                            { name: 'Ability', index: 'Ability', height: 'auto', width: 250, align: "center", search: false },
                            //{ name: 'e', index: 'e', height: 'auto', width: 400, align: "left", search: false , hidden:true},



        ],
        pager: jQuery('#pagerCDWorksList1'),
        rowNum: 10,
        rowList: [10, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'RCTRC_Contact_Name',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; TNA  Key Areas of Work (Subdetails) List",
        height: 'auto',
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function () {
            //$("#tbCDWorksList #pagerCDWorksList1").css({ height: '31px' });

            //$("#pagerCDWorksList1_left").html("<input type='button' style='margin-left:27px' id='idFinalizeExistingRoad' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'RedirectFinalizeRoad();return false;' value='Finalize Audit'/>");

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
                    $('#tbCDWorksList1').trigger('reloadGrid');

                }
                else {
                    alert(response.ErrorMessage)
                    $('#tbCDWorksList1').trigger('reloadGrid');
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


function LoadInspDetailsListSearch(PersonCode) {
   

    $('#tbCDWorksList1').jqGrid('GridUnload');
    var personIDSwitch = PersonCode;

    jQuery("#tbCDWorksList1").jqGrid({
        url: '/RCTRC/RCTRC/GetSubdetailsList',
        datatype: "json",
        mtype: "POST",
        postData: { postdataPersonID: personIDSwitch },
        // postData: { RSACode: RSACode },
        colNames: ['Person Name', 'Master Work Area Element Name', 'Key Area Element Name', 'Grade', 'Ability To Work'],
        colModel: [
                            { name: 'Person_NAME', index: 'Person_NAME', height: 'auto', width: 400, align: "left", search: false },
                            { name: 'MasterElement_Name', index: 'MasterElement_Name', height: 'auto', width: 250, align: "left", search: false },
                            { name: 'SubElement_Name', index: 'SubElement_Name', height: 'auto', width: 250, align: "left", search: false },
                            { name: 'Grade', index: 'Grade', height: 'auto', width: 250, align: "center", search: false },
                            { name: 'Ability', index: 'Ability', height: 'auto', width: 250, align: "center", search: false },
                            //{ name: 'e', index: 'e', height: 'auto', width: 400, align: "left", search: false , hidden:true},



        ],
        pager: jQuery('#pagerCDWorksList1'),
        rowNum: 10,
        rowList: [10, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'RCTRC_Contact_Name',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; TNA  Key Areas of Work (Subdetails) List",
        height: 'auto',
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function () {
            //$("#tbCDWorksList #pagerCDWorksList1").css({ height: '31px' });

            //$("#pagerCDWorksList1_left").html("<input type='button' style='margin-left:27px' id='idFinalizeExistingRoad' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'RedirectFinalizeRoad();return false;' value='Finalize Audit'/>");

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
