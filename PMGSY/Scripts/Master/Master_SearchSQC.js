$(document).ready(function () {

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });
    $('#ddlSearchStatusSQ').val("Y");
    if ($('#stateCode').val() > 0) {
        $("#ddlSearchStatesSQ").val($('#stateCode').val());
        $("#ddlSearchStatesSQ").attr("disabled", true);
        //$("#ddlSearchStates").trigger('change');
    } else {
        if ($("#ddlSearchStatesSQ").val() == 0) {
            $("#ddlSearchStatesSQ").val($("#ddlSearchStatesSQ")[0].options[1].value);
            // $('#ddlSearchStates').trigger('change');
        }
    }

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


    $('#btnSearchSQ').click(function (e) {
        SearchDetails();
    });

    $("#ddlSearchStatesSQ").change(function () {
        var adminNdCode = $('#ddlSearchStatesSQ option:selected').val();
        $.ajax({
            url: '/Master/GetDepartmentSearch/' + adminNdCode,
            type: 'GET',
            catche: false,
            error: function (xhr, status, error) {
                alert('An Error occured while processig your request.')
                return false;
                //console.log(adminNdCode);
            },
            success: function (data) {
                console.log(data);
                $('#departmentDD1').empty();
                $.each(data, function () {
                    $('#departmentDD1').append("<option value=" + this.Value + ">" + this.Text + "</option>");
                    //console.log(this.Value+" "+this.Text);
                    
                });
           
                //alert("complete");
            }
        });

    });

});

function SearchDetails() {

    $('#tblList').setGridParam({ url: '/Master/GetAdminSqcDetails', datatype: 'json' });

    $('#tblList').jqGrid("setGridParam", { "postData": { stateCode: $('#ddlSearchStatesSQ option:selected').val(), status: $('#ddlSearchStatusSQ option:selected').val(), adminNdCode: $('#departmentDD1 Option:selected').val() } });

    $('#tblList').trigger("reloadGrid", [{ page: 1 }]);

}



