$(document).ready(function () {

    $('#btnUpdate').click(function (e) {

        e.preventDefault();
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $("#ddlStates").attr("disabled", false);
        $("#ddlDistricts").attr("disabled", false);

        $.ajax({
            url: "/BlockMasterDataEntry/Edit",
            type: "POST",
            dataType: "html",
            data: $("form").serialize(),
            success: function (data) {

                $("#mainDiv").html(data);
                $.unblockUI();

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }

        });
    });

    $('#btnCancel').click(function (e) {

        $.ajax({
            url: "/BlockMasterDataEntry/Index",
            type: "GET",
            dataType: "html",
            success: function (data) {
                $("#mainDiv").html(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
            }

        });
    });


    $("#ddlStates").change(function () {

        FillInCascadeDropdown({ userType: $("#ddlStates").find(":selected").val() },
                    "#ddlDistricts", "/BlockMasterDataEntry/GetDistrictsByStateCode?stateCode=" + $('#ddlStates option:selected').val());

    }); //end function District change
});

function FillInCascadeDropdown(map, dropdown, action) {

    $(dropdown).empty();
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.post(action, map, function (data) {
        $.each(data, function () {
            $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
        });
    }, "json");
    $.unblockUI();
} //end FillInCascadeDropdown()