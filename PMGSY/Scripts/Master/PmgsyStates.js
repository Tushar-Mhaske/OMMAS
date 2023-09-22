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
              
            },
            success: function (data) {
                console.log(data);
                $('#departmentDD1').empty();
                $.each(data, function () {
                    $('#departmentDD1').append("<option value=" + this.Value + ">" + this.Text + "</option>");
                   

                });

            }
        });

    });

});

$('#btnAddPmgsyStates').click(function (e) {


    if ($('#ddlPmgsyStates').val() == null) {
        alert("Please select atleast a single State.");
        return false;
    }


    if ($('#frmSearchSQC').valid()) {
     
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: "/Master/AddState/",
            type: "POST",
          
            data: $("#frmSearchSQC").serialize(),
            success: function (data) {
                if (data.success==true) 
                {
                    alert("State Details saved successfully");
                    $('#tblList').trigger('reloadGrid');
                    $("#dvDetails").load("/Master/AddPmgsyStates", function () {
                        $('#dvDetails').show();
                        $('#btnCreateNewSQC').hide();
                        $('#btnSearchViewSQC').show();
                        $.unblockUI();
                    });
                }
                else if (data.success==false) 
                {
                    alert('An Error occured while processig your request.');
                }
                else 
                {
                }
                $.unblockUI();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }

        });

    }
});
















