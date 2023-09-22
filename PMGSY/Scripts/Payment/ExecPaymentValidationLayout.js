//validate DPIU required at SRRDA && Mord level
jQuery.validator.addMethod("isdpiurequired", function (value, element) {
    //alert($("#ddlDPIU option:selected").val() == 0);
    if ($("#ddlDPIU option:selected").val() < 0) {
        return false;
    }
    else {
        return true;
    }
}, "");

$(document).ready(function () {

    $.validator.unobtrusive.parse($("#frmExecPaymentValidationLayout"));

    var date = new Date();
    var toDate = new Date((parseInt(date.getMonth()) == 11 ? parseInt(date.getFullYear() + 1) : date.getFullYear()),
                            (parseInt(date.getMonth()) == 11 ? 1 : parseInt(date.getMonth() + 2)),
                          0);
    //var toDate = parseInt(date.getMonth()) == 11 ? (new Date(parseInt(date.getFullYear() + 1), 0, 0)) : (new Date(parseInt(date.getFullYear() + 1), parseInt(date.getMonth() + 2), 0));
    console.log('date=' + date);
    console.log('toDate=' + toDate);
    console.log('getMonth=' + parseInt(date.getMonth()));
    console.log('getFullYear=' + parseInt(date.getFullYear()));

    $('#txtFromDt').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a start date',
        buttonImageOnly: true,
        buttonText: 'Start Date',
        changeMonth: true,
        changeYear: true,
        //minDate: $('#agreementDate').val(),
        maxDate: new Date(),
        //minDate: new Date(),//startDate,
        //minDate: new Date(currentYear, currentMonth, currentDate),
        onSelect: function (selectedDate) {
            $("#txtToDt").datepicker("option", "minDate", selectedDate);
            $(function () {
                $('#txtFromDt').focus();
                $('#txtToDt').focus();
            })
            $('#txtFromDt').trigger('blur');
        }
    });

    $('#txtToDt').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a start date',
        buttonImageOnly: true,
        buttonText: 'Start Date',
        changeMonth: true,
        changeYear: true,
        //minDate: $('#agreementDate').val(),
        maxDate: toDate,
        //minDate: new Date(),//startDate,
        //minDate: new Date(currentYear, currentMonth, currentDate),
        onSelect: function (selectedDate) {
            //$("#txtNewsPublishEnd").datepicker("option", "minDate", selectedDate);
            //$(function () {
            //    $('#txtNewsPublishSt').focus();
            //    $('#txtNewsPublishEnd').focus();
            //})
            $('#txtToDt').trigger('blur');
        }
    });

    if ($("#rdoSRRDA").is(":checked") /*&& ($("#levelId").val() != 5)*/) {
        //Add Validation rule for dpiu required
        $("#ddlDPIU").trigger('blur');

    }

    $("#rdoSRRDA").click(function () {
        $("#ddlDPIU").hide("slow");
        $("#ddlDPIU option:first").attr("selected", "selected");

        $("#ddlDPIU").trigger('blur');
        $('#erDPIU').text('');

        $("#rdoAllRd").trigger('click');

        //hide report category button
        //$("#cat_icon").hide();
    });

    $("#rdoDPIU").click(function () {
        $("#ddlDPIU").show("slow");

        $("#rdoAllRd").trigger('click');
        //hide report category button
        //$("#cat_icon").hide();
    });


    if ($("#rdoDPIU").is(":checked") /*&& ($("#levelId").val() != 5)*/) {
        //Add Validation rule for dpiu required
        $("#ddlDPIU").rules('add', {
            isdpiurequired: true,
            messages: {
                isdpiurequired: 'Please select DPIU'
            }
        });
    }

    $("#rdoAllRd").click(function () {
        $(".SelRoad").hide("slow");

        $("#ddlYear").val('0');
        $("#ddlRoad").empty();
        $("#ddlRoad").append("<option value='0'>Select Road</option>");

        //hide report category button
        //$("#cat_icon").hide();
    });

    $("#rdoSelectRd").click(function () {
        $(".SelRoad").show("slow");
        //hide report category button
        //$("#cat_icon").hide();
    });

    $(function () {
        if ($("#rdoSRRDA").is(":checked")) {
            $('#ddlSRRDA').trigger('change');
            $("#ddlDPIU").hide();
        }

        if ($("#rdoAllRd").is(":checked")) {
            $('#rdoAllRd').trigger('click');
            $("#ddlDPIU").hide();
        }
    });

    PopulateDPIU();

    $("#ddlSRRDA").change(function () {
        $("#rdoAllRd").trigger('click');
        PopulateDPIU();
    });

    $("#ddlDPIU").change(function () {
        $("#rdoAllRd").trigger('click');
    });

    $('#btnSubmit').click(function () {
        //alert(1);
        if ($("#rdoDPIU").is(":checked") /*&& ($("#levelId").val() != 5)*/) {
            //alert(2);
            //Add Validation rule for dpiu required
            $("#ddlDPIU").rules('add', {
                isdpiurequired: true,
                messages: {
                    isdpiurequired: 'Please select DPIU'
                }
            });
        }



        //$('#ddlYear').rules('remove', "range");

        //alert(!$("#ddlYear").is(":visible"));
        /*if (!$("#ddlYear").is(":visible")) {

            //$('#ddlYear').removeClass("{required:true,messages:{required:'required field'}}")

            //$('#ddlYear').removeClass("{range:true,messages:{range:'Please select Sanction Year.'}}")

            $('#ddlYear').rules('remove');
            //$('#ddlYear').rules('remove', "range");
        }*/
        //if (!$("#ddlRoad").is(":visible")) {
        //    $('#ddlRoad').rules('remove', "required");
        //    $('#ddlRoad').rules('remove', "range");
        //}

        //alert($("#frmExecPaymentValidationLayout").valid());
        if (!$("#frmExecPaymentValidationLayout").valid()) {
            return false;
        }


        $.ajax({
            url: "/Payment/AddValidationDetails/",
            cache: false,
            type: "POST",
            async: false,
            data: $("#frmExecPaymentValidationLayout").serialize(),
            success: function (data) {
                if (data.status == true) {
                    alert(data.message);
                    $('#btnReset').trigger('click');

                    //$('#tbExecTechnologyList').trigger('reloadGrid');
                    //$('#divAddEditTechnologyProgressDetails').html('');
                    //$('#divAddEditTechnologyProgressDetails').hide('slow');
                }
                else {
                    alert(data.message);
                    
                    //$("#divError").show();
                    //$("#divError").html('<strong>Alert : </strong>' + data.message);
                }
            },
            error: function () {
                alert("error");
            }
        })
    });

    $('#ddlYear').change(function () {
        if ($('#ddlYear option:selected').val() > 0) {
            PopulateRoads();
        }
        else {
            $("#ddlRoad").empty();
            $("#ddlRoad").append("<option value='0'>Select Road</option>");
        }
    });

    $('#btnReset').click(function () {
        $("#rdoAllRd").trigger('click');
        $("#rdoSRRDA").trigger('click');
    });

});

function PopulateDPIU() {
    var adminNdCode = $('#ddlSRRDA option:selected').val();
    $.ajax({
        url: '/Payment/PopulateDPIUForCashBook/' + adminNdCode,
        type: 'GET',
        catche: false,
        error: function (xhr, status, error) {
            alert('An Error occured while processig your request.')
            return false;
        },
        success: function (data) {
            $('#ddlDPIU').empty();
            $.each(data, function () {
                $('#ddlDPIU').append("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
        }
    });
}

function PopulateRoads() {
    var adminNdCode = $('#ddlSRRDA option:selected').val();

    //alert('rd= ' + $("#rdoSRRDA").is(":checked"));

    $.ajax({
        url: '/Payment/PopulateRoads/',
        type: 'GET',
        catche: false,
        data: { adminNdCode: $("#rdoSRRDA").is(":checked") ? $("#ddlSRRDA option:selected").val() : $("#ddlDPIU option:selected").val(), srrdaDpiu: $("#rdoSRRDA").is(":checked") ? 'S' : 'D', Year: $("#ddlYear").val() },
        error: function (xhr, status, error) {
            alert('An Error occured while processig your request.')
            return false;
        },
        success: function (data) {
            $('#ddlRoad').empty();
            $.each(data, function () {
                $('#ddlRoad').append("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
        }
    });
}

function SelectAllRoads() {
    $("#rdoAllRd").trigger('click');
    
}