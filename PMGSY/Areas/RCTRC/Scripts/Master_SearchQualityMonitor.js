$(document).ready(function () {

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });
    $("#ddlSearchQmTypes").val("S");
    if ($('#HiddenstateCode').val() == 0) {
        $("#ddlSearchStates").val($("#ddlSearchStates")[0].options[1].value);
       // $("#ddlSearchStates").trigger('change');
    }
    if (($('#HiddenstateCode').val() > 0) && ($("#roleCode").val() == 8 || $("#roleCode").val() == 69)) {

        $("#ddlSearchQmTypes").val("S");
        $("#ddlSearchQmTypes").attr("disabled", true);

        $("#ddlSearchStates").val($('#HiddenstateCode').val());
        $("#ddlSearchStates").attr("disabled", true);
        //$("#ddlSearchStates").trigger('change');
    }

    //if (($("#roleCode").val() == 5)) {
    //    $("#ddlSearchQmTypes").val("S");
    //    $("#ddlSearchQmTypes").attr("disabled", true);
    //   // $("#ddlSearchStates").trigger('change');
    //}


    //Changed by deendayal on 31/7/2017 
    $('#ddlSearchQmTypes').change(function () {
        var qmType = $('#ddlSearchQmTypes').val();

        if (qmType == "" || qmType == "I") {

            $('#lblcadrestate').text("Cadre State");
        }
        else {
            $('#lblcadrestate').text("State");
        }

        if (qmType != "") {
            $.ajax({
                url: "/Master/GetCadreStates",
                type: "POST",
                data: { QMType: $('#ddlSearchQmTypes').val() },
                success: function (data) {
                    if (data != null) {
                        $('#ddlSearchStates option').each(function () {
                            $(this).remove();
                        });

                        // $('#ddlSearchStates').append("<option value=0>All States</option>");
                        $.each(data, function () {
                            $('#ddlSearchStates').append("<option value=" + this.Value + ">" + this.Text + "</option>");
                        });


                    }
                    else {
                        alert("Something went wrong");
                    }
                },
                error: function () {

                },
            });

        }

    });

  
    $("#dvhdSearch").click(function () {
        if ($("#dvSearchParameter").is(":visible")) {
            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");
            $(this).next("#dvSearchParameter").slideToggle(300);
        }
        else {
            $("#spCollapseIconS").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");
            $(this).next("#dvSearchParameter").slideToggle(300);
        }
    });

    $('#btnSearch').click(function (e) {
        SearchDetails();
    });


});

function SearchDetails() {
    // alert("s2");

//    alert("val= " + $('#ddlSearchQmTypes').val())

    $('#tblQualityMonitorListDetails').setGridParam({
        url: '/RCTRC/RCTRC/GetMasterQualityMonitorList', datatype: 'json'
    });
    //$('#ddlSearchQmTypes option:selected').val()
    $('#tblQualityMonitorListDetails').jqGrid("setGridParam", { "postData": { QmTypeName: "S", stateCode: $('#ddlSearchStates option:selected').val(), districtCode: $('#ddlSearchDistrict option:selected').val(), isEmpanelled: $('#ddlSearchEmpanelled option:selected').val(), firstName: $('#gs_ADMIN_QM_FNAME').val() } });

    $('#tblQualityMonitorListDetails').trigger("reloadGrid", [{ page: 1 }]);
}

function FillInCascadeDropdown(map, dropdown, action) {

    $(dropdown).empty();
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.post(action, map, function (data) {
        $.each(data, function () {
           
            $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
        });
    }, "json");
    $.unblockUI();
}

