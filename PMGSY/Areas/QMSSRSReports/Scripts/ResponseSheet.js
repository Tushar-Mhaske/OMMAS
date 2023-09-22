jQuery.validator.addMethod("datecomparefieldvalidator", function (value, element, param) {

    //var fromDate = parseInt($('#ddlFromYearItemwiseInspections').val());
    //var toDate = parseInt($('#ddlToYearItemwiseInspections').val());

    //var fromMonth = parseInt($('#ddlFromMonthItemwiseInspections').val());
    //var toMonth = parseInt($('#ddlToMonthItemwiseInspections').val());

    //if (fromDate == toDate) {
    //    if (fromMonth > toMonth) {
    //        return false;
    //    }
    //    else {
    //        return true;
    //    }
    //}
    //else {
    //    if (fromDate > toDate) {
    //        return false;
    //    }
    //    else {
    //        return true;
    //    }
    //}
    //return true;
});

jQuery.validator.unobtrusive.adapters.addBool("datecomparefieldvalidator");

$(document).ready(function () {


    // #Added For Searchable Dropdown on 24-01-2023 
    $("#ddlMonitor").chosen();

    $.validator.unobtrusive.parse($('#frmQMItemwiseInspectionsLayout'));

    $("#ddlStatesItemwiseInspections").change(function () {
        $('#ddlDistrictsItemwiseInspections').empty();

        $.ajax({
            url: '/QMSSRSReports/QMSSRSReports/DistrictDetailsQR',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: { stateCode: $("#ddlStatesItemwiseInspections").val(), value: Math.random() },
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    if (jsonData[i].Value == 2) {
                        $("#ddlDistrictsItemwiseInspections").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlDistrictsItemwiseInspections").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                }

                $.unblockUI();
            },
            error: function (err) {
                //alert("error " + err);
                $.unblockUI();
            }
        });
    });

    $("#btnViewItemwiseInspectionsDetails").click(function () {

        if ($('#frmQMItemwiseInspectionsLayout').valid()) {
            $('#StateName').val($('#ddlStatesItemwiseInspections option:selected').text());
            $('#GradingItemName').val($('#ddlItemsInItemwiseInspections option:selected').text());
            $('#GradingItemName').val($('#GradingItem option:selected').text());
            $('#DistName').val($('#ddlDistrictsItemwiseInspections option:selected').text());

            //$("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
            //$("#frmQMItemwiseInspectionsLayout").toggle("slow");

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            $.ajax({
                url: '/QMSSRSReports/QMSSRSReports/ResponseReport/',
                type: 'POST',
                catche: false,
                data: $("#frmQMItemwiseInspectionsLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    $("#dvLoadItemwiseInspectionsReport").html(response);
                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
        }
    });

    $("#btnViewItemwiseInspectionsDetails").trigger('click');
    

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#frmQMItemwiseInspectionsLayout").toggle("slow");

    });
});


