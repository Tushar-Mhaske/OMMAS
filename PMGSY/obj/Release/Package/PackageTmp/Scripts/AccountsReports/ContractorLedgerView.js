
$(document).ready(function () {
    var val ;
   // alert(districtCode);
    $("#ddlPIU").change(function () {
        $.blockUI({ message: '<h4><label style="font-weight:normal">loading Contractor...</label> ' });
        val = $("#ddlPIU").val();
        $.ajax({
            type: 'POST',
            url: "/AccountsReports/PopulateContractor?PIUCode=" + val,
            async: false,
            success: function (data) {
                $.unblockUI();
                $("#ddlContractor").empty();
                $.each(data, function () {
                    $("#ddlContractor").append("<option value=" + this.Value + ">" +
                                                            this.Text + "</option>");

                });

                $.unblockUI();
            }

        });


    });

    $("#ddlContractor").change(function () {
        $.blockUI({ message: '<h4><label style="font-weight:normal">loading Aggrements...</label> ' });
        var val = $("#ddlContractor").val() + "," + $("#ddlPIU").val();
        $.ajax({
            type: 'POST',
            url: "/AccountsReports/PopulateAggrement?id=" + val,
            async: false,
            success: function (data) {
                $.unblockUI();
                $("#ddlAggrement").empty();
                $.each(data, function () {
                    $("#ddlAggrement").append("<option value=" + this.Value + ">" +
                                                            this.Text + "</option>");

                });

                $.unblockUI();
            }

        });


    });

    $("#btnView").click(function () {


        $("#PIUName").val($("#ddlPIU option:selected").text());
        $("#ContCmpName").val($("#ddlContractor option:selected").text());
        $("#AggrementNo").val($("#ddlAggrement option:selected").text());

        if ($("#ddlPIU").val() == "0") {
            alert('Please Select DPIU');
            return false;
        }
        else if ($("#ddlContractor").val() == "0") {
            alert('Please Select Contractor');
            return false;
        }
        else if ($("#ddlAggrement").val() == "0") {
            alert('Please Select Aggrement');
            return false;
        }

        else {
            $.ajax({
                url: "/AccountsReports/ContractorLedgerView/",
                type: "POST",
                async: false,
                cache: false,
                data: $("#frmContLedger").serialize(),
                success: function (data) {
                    $("#ContLedgerDetails").html(data);
                    $("#dvDetails").hide("slow");
                }
            });
        }
    });

    $("#spCollapseIconS").click(function () {

        if ($("#dvDetails").is(":visible")) {
            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");
            $("#dvDetails").slideToggle(300);
        }
        else {
            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");
            $("#dvDetails").slideToggle(300);
        }

    });
});