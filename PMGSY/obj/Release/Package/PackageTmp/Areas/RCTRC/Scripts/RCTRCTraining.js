$(document).ready(function () {
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
    $('#txtSD').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Select Start Date of Training',
        maxDate: "0D",
        minDate: "-" + Math.floor(days) + "D",
        buttonImageOnly: true,
        buttonText: 'Start Date of Training',
        changeMonth: true,
        changeYear: true,
        onSelect: function (selectedDate) {
            $('#txtSD').trigger('blur');
        }
    });

    // txtDOJ

    $('#txtED').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Select End Date of Training',
        maxDate: "0D",
        minDate: "-" + Math.floor(days) + "D",
        buttonImageOnly: true,
        buttonText: 'End Date of Training',
        changeMonth: true,
        changeYear: true,
        onSelect: function (selectedDate) {
            $('#txtED').trigger('blur');
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

    $('#ddlContactPerson_View option[value= -1]').text("All Contact Persons");
    $('#ddlContactPerson_View option[value= -1]').val("0");
    
    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    LoadInspDetailsList($("#ddlContactPerson option:selected").val()) // Load_1
 

    $("#btnSave").click(function (e) {

        if ($("#frmAddCdWorks").valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            if (confirm("Are you sure to save this details ? Once saved, can not be updated or deleted.")) {
                $.ajax({
                    type: 'POST',
                    url: '/RCTRC/RCTRC/AddRCTRCTraining/',
                    async: false,
                    data: $("#frmAddCdWorks").serialize(),
                    success: function (data) {
                        if (data.success == true) {

                            alert(data.message);
                            ClearDetails();

                            $('#cdWorksType').trigger('reloadGrid');
                            //     LoadInspDetailsList();

                            //$('#tbCDWorksList').trigger('reloadGrid');

                            LoadInspDetailsList(-1);
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



    $("#spCollapseIconCN").click(function () {

        if ($("#dvAddNewCdWorksDetails").is(":visible")) {
            $("#dvAddNewCdWorksDetails").hide("slow");

            // $("#btnCreateNew").show();
        }
        else {
            $("#dvAddNewCdWorksDetails").show("slow");

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
});

function ClearDetails() {
    $('#MAST_CDWORKS_NAME').val('');
    $('#MAST_CDWORKS_CODE').val('');


    $('#dvErrorMessage').hide('slow');
    $('#message').html('');
}


function LoadInspDetailsList(PersonCode) {

    $('#tbCDWorksList').jqGrid('GridUnload');
    var personIDSwitch = PersonCode;

    jQuery("#tbCDWorksList").jqGrid({
        url: '/RCTRC/RCTRC/GetRCTRCTrainingList',
        datatype: "json",
        mtype: "POST",
        postData: { postdataPersonID: personIDSwitch },
        colNames: ['Contact Person', 'Training Topic', 'Training Host', 'Training Duration', 'Training Start Date', 'Training End Date'],
        colModel: [
                            { name: 'Contact_Person', index: 'Contact_Person', height: 'auto', width: 315, align: "left", search: false },
                            { name: 'RCTRC_CONTACT_TRG_TOPIC', index: 'RCTRC_CONTACT_TRG_TOPIC', height: 'auto', width: 315, align: "left", search: false },
                            { name: 'RCTRC_CONTACT_TRG_HOST', index: 'RCTRC_CONTACT_TRG_HOST', height: 'auto', width: 300, align: "left", search: false },

                            { name: 'RCTRC_CONTACT_TRG_DURATION', index: 'RCTRC_CONTACT_TRG_DURATION', height: 'auto', width: 300, align: "center", search: false },
                            { name: 'RCTRC_CONTACT_TRG_START_DATE', index: 'RCTRC_CONTACT_TRG_START_DATE', height: 'auto', width: 270, align: "center", search: false },
                            { name: 'RCTRC_CONTACT_TRG_END_DATE', index: 'RCTRC_CONTACT_TRG_START_DATE', height: 'auto', width: 255, align: "center", search: false },

                           // { name: 'e', index: 'e', height: 'auto', width: 400, align: "left", search: false, hidden: true },



        ],
        pager: jQuery('#pagerCDWorksList'),
        rowNum: 5,
        rowList: [5, 10],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'RCTRC_Contact_Name',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; TNA Training Details",
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



// Load_3

function LoadInspDetailsListSearch(PersonCode) {
    $('#tbCDWorksList').jqGrid('GridUnload');
    var personIDSwitch = PersonCode;
    jQuery("#tbCDWorksList").jqGrid({
        url: '/RCTRC/RCTRC/GetRCTRCTrainingList',
        datatype: "json",
        mtype: "POST",
        postData: { postdataPersonID: personIDSwitch },
        colNames: ['Contact Person', 'Training Topic', 'Training Host', 'Training Duration', 'Training Start Date', 'Training End Date'],
        colModel: [
                            { name: 'Contact_Person', index: 'Contact_Person', height: 'auto', width: 315, align: "left", search: false },
                            { name: 'RCTRC_CONTACT_TRG_TOPIC', index: 'RCTRC_CONTACT_TRG_TOPIC', height: 'auto', width: 315, align: "left", search: false },
                            { name: 'RCTRC_CONTACT_TRG_HOST', index: 'RCTRC_CONTACT_TRG_HOST', height: 'auto', width: 300, align: "left", search: false },

                            { name: 'RCTRC_CONTACT_TRG_DURATION', index: 'RCTRC_CONTACT_TRG_DURATION', height: 'auto', width: 300, align: "center", search: false },
                            { name: 'RCTRC_CONTACT_TRG_START_DATE', index: 'RCTRC_CONTACT_TRG_START_DATE', height: 'auto', width: 270, align: "center", search: false },
                            { name: 'RCTRC_CONTACT_TRG_END_DATE', index: 'RCTRC_CONTACT_TRG_START_DATE', height: 'auto', width: 255, align: "center", search: false },

                           // { name: 'e', index: 'e', height: 'auto', width: 400, align: "left", search: false, hidden: true },



        ],
        pager: jQuery('#pagerCDWorksList'),
        rowNum: 5,
        rowList: [5, 10],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'RCTRC_Contact_Name',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; TNA Training Details",
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