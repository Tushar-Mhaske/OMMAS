

$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmAddSoilType');


    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    // Added 4 March 2020
    //$('#MANE_AGREEMENT_DATE').datepicker({

    //    dateFormat: 'dd/mm/yy',
    //    showOn: "button",
    //    buttonImage: "/Content/Images/calendar_2.png",
    //    buttonImageOnly: true,
    //    changeMonth: true,
    //    changeYear: true,
    //    buttonText: 'Agreement Date',
    //    onSelect: function (selectedDate) {
    //        $("#MANE_MAINTENANCE_START_DATE").datepicker("option", "minDate", selectedDate);
    //        $("#MANE_CONSTR_COMP_DATE").datepicker("option", "maxDate", selectedDate);
    //        $("#MANE_HANDOVER_DATE").datepicker("option", "minDate", selectedDate);
    //    },
    //    onClose: function () {

    //        $(this).focus().blur();
    //    }
    //});

    $('#MANE_CONSTR_COMP_DATE').attr('readonly', 'readonly');
    $('#MANE_MAINTENANCE_START_DATE').attr('readonly', 'readonly');
    $('#MANE_MAINTENANCE_END_DATE').attr('readonly', 'readonly');


    $('#MANE_CONSTR_COMP_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        buttonText: 'Agreement Date',
        maxDate: '0',

        onSelect: function (selectedDate) {




            //    $("#MANE_CONSTR_COMP_DATE").datepicker("option", "minDate", $("#MANE_AGREEMENT_DATE").val());



            var startDate = selectedDate.split('/');
            var day = startDate[0];
            var month = startDate[1];
            var year = startDate[2];
            var nextYear = parseInt(year) //+ 5;

            if (day < 9) {
                var nextDay = "0" + (parseInt(day) + 1);
            }
            else {
                var nextDay = parseInt(day) + 1;
            }
            var nextDate = nextDay + "/" + month + "/" + nextYear;
            $('#MANE_MAINTENANCE_START_DATE').val(nextDate);




            var startDate = selectedDate.split('/');
            var day = startDate[0];
            var month = startDate[1];
            var year = startDate[2];
            var nextYear = parseInt(year) + 5;

            if (day < 9) {
                var nextDay = "0" + (parseInt(day) + 1);
            }
            else {
                var nextDay = parseInt(day) + 1;
            }
            var nextDate = nextDay + "/" + month + "/" + nextYear;
            $('#MANE_MAINTENANCE_END_DATE').val(nextDate);




        },
        onClose: function () {

            $(this).focus().blur();
        }
    });


    $('#MANE_MAINTENANCE_START_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        buttonText: 'Maintenance Start Date',
        maxDate: '0',
        onSelect: function (selectedDate) {
            //  $("#MANE_MAINTENANCE_START_DATE").datepicker("option", "minDate", $("#MANE_CONSTR_COMP_DATE").val());



            var startDate = selectedDate.split('/');
            var day = startDate[0];
            var month = startDate[1];
            var year = startDate[2];
            var nextYear = parseInt(year) + 5;
            var nextDate = day + "/" + month + "/" + nextYear;
            $('#MANE_MAINTENANCE_END_DATE').val(nextDate);
        },
        onClose: function () {

            $(this).focus().blur();
        }
    });

    $('#MANE_MAINTENANCE_END_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        buttonText: 'Maintenance Start Date',
        onSelect: function (selectedDate) {

            //     $("#MANE_MAINTENANCE_END_DATE").datepicker("option", "minDate", $("#MANE_MAINTENANCE_START_DATE").val());

        },
        onClose: function () {

            $(this).focus().blur();
        }
    });


    $("#MAST_SOIL_TYPE_NAME").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });





    $("#spCollapseIconCN").click(function () {

        if ($("#soilDetails").is(":visible")) {
            $("#soilDetails").hide("slow");

            $("#btnCreateNew").show();
        }
    });

    $("#btnCancel").click(function (e) {
        $("#emargUpdateForm").html();
        $("#emargUpdateForm").hide('slow');



        //$("#btnCreateNew").show();
        //$('#soilDetails').hide('slow');

    })

    $("#btnReset").click(function (e) {

        ClearDetails();
    });

    $("#btnUpdate").click(function (e) {

        //$('#MANE_VALUE_WORK_DONE').rules('remove', 'Required');
        // alert("Update")

        if ($("#MANE_COMPLETED_LENGTH").val() == null || $("#MANE_COMPLETED_LENGTH").val() == 0) {
            alert("Completed Length can not be  0");
            return false;
        }
        var str1 = "Others";
        var TrafficeIntensity = $("#ddlTrafficeTypeList option:selected").text();
        var n = str1.localeCompare(TrafficeIntensity);

        if (n == 0) {
            alert("Please select another Traffic Type. It can not be Others.");
            return false;
        }


        if ($("#frmAddSoilType").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                type: 'POST',
                url: '/MaintenanceAgreement/UpdateEmargDetailsPostDLP/',
                async: false,
                data: $("#frmAddSoilType").serialize(),
                success: function (data) {
                    if (data.success == true) {

                        $("#emargUpdateForm").html();
                        $("#emargUpdateForm").hide('slow');
                        alert(data.message);

                        LoadCompletedRoads();

                        //$("#btnCreateNew").show();
                        //$('#soilDetails').hide('slow');
                        // $('#tbProposedRoadList').trigger('reloadGrid');
                    }
                    else if (data.success == false) {
                        if (data.message != "") {


                            alert(data.message);
                            $("#emargUpdateForm").html();
                            $("#emargUpdateForm").hide('slow');


                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#soilDetails").html(data);
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
    $('#MAST_SOIL_TYPE_CODE').val('');
    $('#MAST_SOIL_TYPE_NAME').val('');

    $('#dvErrorMessage').hide('slow');
    $('#message').html('');
}