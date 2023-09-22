$(document).ready(function () {

    ///Changed by SAMMED A. PATIL on 21JULY2017 to allow editing of latest record
    var currentDate = new Date();
    var currDay = currentDate.getDate();
    //var startDate = parseInt(currDay) <= 5 ? new Date(currentDate.getFullYear(), parseInt(currentDate.getMonth()) == 1 ? 12 : parseInt(currentDate.getMonth() - 1), 1) : new Date(currentDate.getFullYear(), currentDate.getMonth(), 1);
    var startDate = parseInt(currDay) <= 30 ? new Date(currentDate.getFullYear(), parseInt(currentDate.getMonth()) == 1 ? 12 : parseInt(currentDate.getMonth() - 1), 1) : new Date(currentDate.getFullYear(), currentDate.getMonth(), 1);

    var date = new Date();
    var firstDay = new Date(date.getFullYear(), date.getMonth() - 1, 1);
    var lastDay = new Date(date.getFullYear(), date.getMonth(), 0);

    //if max Date is Zero it is defaultly taking Today's Date
    var maxDate = (date.getMonth() == 3 && parseInt(currDay) <= 30) ? lastDay : 0;

    $(":input").bind("keypress", function (e) {
        if (e.keyvalue == 13) {
            return false;
        }
    })

    $("#lblCompletionDate").hide();
    $("#lblRequired").hide();
    $("#tdCompletionDate").hide();

    $('#ddlYear').change(function () {
        var curDate = new Date();
        var year = curDate.getFullYear();
        var month = curDate.getMonth();
        month = parseInt(month) + 1;
        if (parseInt($('#ddlYear').val()) == parseInt(year) - 1) {
            $('#ddlMonth').val(parseInt(month == 1 ? 12 : parseInt(month) - 1));
        }
        else {
            $('#ddlMonth').val(parseInt(month));
        }
    });

    $('#ddlMonth').change(function () {
        var curDate = new Date();
        var year = curDate.getFullYear();
        var month = curDate.getMonth();
        month = parseInt(month) + 1;

        if (parseInt(month) == 1) {
            if (parseInt($('#ddlMonth option:selected').val()) == 12) {
                $('#ddlYear').val(parseInt(year) - 1);
            }
            else {
                $('#ddlYear').val(parseInt(year));
            }
        }
    });


    //save the physical lsb progress details
    $("#btnAddLSBDetails").click(function () {

        var curDate = new Date();
        var month = curDate.getMonth();
        month = parseInt(month) + 1;
        var year = curDate.getFullYear();
        if ($("#ddlYear").val() > year) {
            alert('Year should not be greater than current year.');
            return false;
        }

        if ($("#ddlMonth").val() > month && $("#ddlYear").val() == year) {

            alert('Month and Year exceeds the current date.');
            return false;
        }

        isCompleted = $("#ddlWorkStatus option:selected").val();

        if (isCompleted == 'C') {
            if ($("#txtCompletionDate").val() == "") {
                $("#msgDateValidation").show();
                $("#msgDateValidation").html("<span style='color:red'>Please enter Progress Completion Date.</span>");
                return false;
            }
        }

        if (isCompleted == 'C') {
            if (validateDate() == false) {
                return false;
            }
        }

        if ($("#frmAddPhysicalLSB").valid()) {
            //blockpage();
            $.ajax({
                type: 'POST',
                url: '/Execution/AddPhysicalLSBDetails',
                data: $("#frmAddPhysicalLSB").serialize(),
                async: false,
                cache: false,
                success: function (data) {
                    //unblockPage();
                    if (data.success) {
                        alert(data.message);
                        $("#tbLSBPhysicalRoadList").trigger('reloadGrid');
                        $("#btnResetRoadDetails").trigger('click');
                        $("#frmAddPhysicalLSB").trigger('reload');
                        $("#divAddPhysicalLSB").html('');
                    }
                    else {
                        //alert(data.message);
                        $("#divError").show();
                        $("#divError").html('<strong>Alert : </strong>' + data.message);
                    }
                },
                error: function () {
                    alert("Request can not be processed at this time.");
                }
            })
        }
    });

    //update road details button click
    $("#btnUpdateLSBDetails").click(function () {

        var curDate = new Date();
        var month = curDate.getMonth();
        month = parseInt(month) + 1;
        var year = curDate.getFullYear();
        if ($("#ddlYear").val() > year) {
            alert('Year should not be greater than current year.');
            return false;
        }

        if ($("#ddlMonth").val() > month && $("#ddlYear").val() == year) {

            alert('Month and Year exceeds the current date.');
            return false;
        }

        isCompleted = $("#ddlWorkStatus option:selected").val();

        if (isCompleted == 'C') {
            if ($("#txtCompletionDate").val() == "") {
                $("#msgDateValidation").show();
                $("#msgDateValidation").html("<span style='color:red'>Please enter Progress Completion Date.</span>");
                return false;
            }
        }

        if (isCompleted == 'C') {
            if (validateDate() == false) {
                return false;
            }
        }

        if ($("#frmAddPhysicalLSB").valid()) {
            $.ajax({
                type: 'POST',
                url: '/Execution/EditPhysicalLSBDetails/',
                data: $("#frmAddPhysicalLSB").serialize(),
                async: false,
                cache: false,
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        $("#tbLSBPhysicalRoadList").trigger('reloadGrid');
                        $("#frmAddPhysicalLSB").removeData();
                        $("#divAddPhysicalLSB").html('');
                    }
                    else {
                        $("#divError").show();
                        $("#divError").html('<strong>Alert : </strong>' + data.message);
                    }
                },
                error: function () {
                    alert("Request can not be processed at this time.");
                }
            })
        }
    });

    $("#btnCancelLSBDetails").click(function () {
        $("#divAddLSBProgress").hide("slow");
    });


    $('#imgCloseProgressDetails').click(function () {

        $("#divAddLSBProgress").hide("slow");
        $("#divError").hide("slow");

    });

    $("#ddlWorkStatus").change(function () {
        if ($("#ddlWorkStatus option:selected").val() == 'C') {
            ShowDateOption();
        }
        else {
            HideDateOption();
        }
    });

    $('#txtCompletionDate').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a date',
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        maxDate: maxDate,
        minDate: startDate,
        onSelect: function (selectedDate) {

        }
    });

    if ($("#Operation").val() == "E") {
        if ($("#ddlWorkStatus option:selected").val() == 'C') {
            $("#ddlWorkStatus").trigger('change');
        }
    }

});
function ShowDateOption() {
    $("#lblCompletionDate").show('slow');
    $("#lblRequired").show('slow');
    $("#txtCompletionDate").show('slow');
    $("#tdCompletionDate").show('slow');
    $(".ui-datepicker-trigger").show('slow');
}
function HideDateOption() {
    $("#lblCompletionDate").hide();
    $("#lblRequired").hide();
    $("#txtCompletionDate").hide();
    $("#tdCompletionDate").hide();
    $(".ui-datepicker-trigger").hide();
}
function validateDate() {

    var paymentDate = $("#txtCompletionDate").val();
    var dateParts = paymentDate.split('/');
    var month = parseInt(dateParts[1]);
    var year = parseInt(dateParts[2]);

    if (parseInt(month) != parseInt($("#ddlMonth").val())) {
        alert('Month must match the month of completion date.');
        return false;
    }

    if (parseInt(year) != parseInt($("#ddlYear").val())) {
        alert('Year must match the year of completion date.');
        return false;
    }

}