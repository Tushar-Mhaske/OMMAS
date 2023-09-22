$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmPackageWiseReport'));


    $("#btnViewPackageWiseDetails").click(function () {


        if ($('#frmPackageWiseReport').valid()) {
            $("#loadPackageWiseReport").html("");

            if ($("#StateList_PackageWiseDetails").is(":visible")) {

                $("#StateName").val($("#StateList_PackageWiseDetails option:selected").text());
            }

            if ($("#DistrictList_PackageWiseDetails").is(":visible")) {

                //$('#DistrictList_PackageWiseDetails').attr("disabled", false);
                $("#DistName").val($("#DistrictList_PackageWiseDetails option:selected").text());
            }
            if ($("#BlockList_PackageWiseDetails").is(":visible")) {

                $("#BlockName").val($("#BlockList_PackageWiseDetails option:selected").text());
            }

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/MaintenanceSSRSReport/MaintenanceSSRSReport/RcplweBalanceReport/',
                type: 'POST',
                catche: false,
                data: $("#frmPackageWiseReport").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#loadPackageWiseReport").html(response);

                },
                error: function () {
                    $.unblockUI();
                    alert("An Error");
                    return false;
                },
            });

        }

    });

    // $("#btnViewPackageWiseDetails").trigger('click');

    //if ($('#Mast_State_Code').val() > 0) {
    //    $("#btnViewPackageWiseDetails").trigger('click');
    //}

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvSearchParameter").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});



//District Change Fill Block DropDown List
