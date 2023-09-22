$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmConnectivityStatusLayout'));

    $('#ddlState').change(function () {
        $("#ddlDistrict").empty();
        FillInCascadeDropdown({ userType: $("#ddlDistrict").find(":selected").val() },
                    "#ddlDistrict", "/Master/PopulateDistricts?param=" + $('#ddlState option:selected').val());
    });


    function FillInCascadeDropdown(map, dropdown, action) {
        var message = '';

        $(dropdown).empty();
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.post(action, map, function (data) {
            $.each(data, function () {
                $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
        }, "json");
        $.unblockUI();

    } //end FillInCascadeDropdown()

    $("#idFilterDiv").click(function () {

        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#divConnectivityStatusLayout").toggle("slow");

    });

    $('#btnView').click(function () {
        if ($("#frmConnectivityStatusLayout").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });

            $('#hdnStateCode').val($("#ddlState").val());
            $('#hdnDistCode').val($("#ddlDistrict").val());

            

            $.ajax({
                url: '/Master/Form3ConnectivityStatus/',
                type: 'POST',
                //catche: false,
                data: $("#frmConnectivityStatusLayout").serialize(),
                //async: false,
                success: function (response) {
                    $.unblockUI();
                    //$('#dvLoadButtons').show('slow');

                    
                    $("#dvLoadReport").html(response);
                    //alert($('#hdnflag').val());
                    if ($('#hdnflag').val() == "N") {
                        $('#dvLoadButtons').show('slow');
                        $('#dvLoadSaveButton').hide('slow');
                    }
                    else {
                        $('#dvLoadSaveButton').show('slow');
                        $('#dvLoadButtons').hide('slow');
                    }
                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
            //if ($("#flag").val() == "N") {
            //    alert(1);
            //    $('#divConnectivityStatus1').show('slow');
            //    $('#divConnectivityStatus2').hide('slow');
            //}
            //else {
            //    $('#divConnectivityStatus1').hide('slow');
            //    $('#divConnectivityStatus2').show('slow');
            //}
            $.unblockUI();
        }
    });


    $('#btnAdd').click(function () {
        if ($("#frmConnectivityStatusLayout").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            
            $('#hdnStateCode').val($("#ddlState").val());
            $('#hdnDistCode').val($("#ddlDistrict").val());

            $.ajax({
                url: '/Master/Form3ConnectivityStatus/',
                type: 'POST',
                catche: false,
                data: $("#frmConnectivityStatusLayout").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    //$('#dvLoadButtons').show('slow');
                    $("#dvLoadReport").html(response);
                    $('#txtt11').prop("disabled", false);
                    $('#txtt12').prop("disabled", false);
                    $('#txtt13').prop("disabled", false);
                    $('#txtt14').prop("disabled", false);
                    $('#txtt1El499').prop("disabled", false);
                    $('#txtt1El249').prop("disabled", false);

                    $('#txtt21').prop("disabled", false);
                    $('#txtt22').prop("disabled", false);
                    $('#txtt23').prop("disabled", false);
                    $('#txtt24').prop("disabled", false);
                    $('#txtt2El499').prop("disabled", false);
                    $('#txtt2El249').prop("disabled", false);

                    $('#txtt41').prop("disabled", false);
                    $('#txtt42').prop("disabled", false);
                    $('#txtt43').prop("disabled", false);
                    $('#txtt44').prop("disabled", false);
                    $('#txtt4El499').prop("disabled", false);
                    $('#txtt4El249').prop("disabled", false);

                    $('#txtt51').prop("disabled", false);
                    $('#txtt52').prop("disabled", false);
                    $('#txtt53').prop("disabled", false);
                    $('#txtt54').prop("disabled", false);
                    $('#txtt5El499').prop("disabled", false);
                    $('#txtt5El249').prop("disabled", false);

                    $('#btnSubmit').show('slow');
                    $('#btnReset').show('slow');
                    $('#btnCancel').show('slow');

                    $('#txtt11').focus();
                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
            $.unblockUI();
        }
    });

    $('#btnUpdate').click(function () {
        $('#txtt11').prop("disabled", false);
        $('#txtt12').prop("disabled", false);
        $('#txtt13').prop("disabled", false);
        $('#txtt14').prop("disabled", false);

        $('#txtt1El499').prop("disabled", false);
        $('#txtt1El249').prop("disabled", false);

        $('#txtt21').prop("disabled", false);
        $('#txtt22').prop("disabled", false);
        $('#txtt23').prop("disabled", false);
        $('#txtt24').prop("disabled", false);

        $('#txtt2El499').prop("disabled", false);
        $('#txtt2El249').prop("disabled", false);

        $('#txtt41').prop("disabled", false);
        $('#txtt42').prop("disabled", false);
        $('#txtt43').prop("disabled", false);
        $('#txtt44').prop("disabled", false);

        $('#txtt4El499').prop("disabled", false);
        $('#txtt4El249').prop("disabled", false);

        $('#txtt51').prop("disabled", false);
        $('#txtt52').prop("disabled", false);
        $('#txtt53').prop("disabled", false);
        $('#txtt54').prop("disabled", false);

        $('#txtt5El499').prop("disabled", false);
        $('#txtt5El249').prop("disabled", false);
        
        $('#btnSubmit').show('slow');
        $('#btnReset').show('slow');
        $('#btnCancel').show('slow');

        $('#txtt11').focus();
    });

    $('#btnDelete').click(function () {
        if (confirm("Are you sure you want to Delete?")) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });

            $.ajax({
                url: '/Master/DeleteConnectivityStatus/',
                type: 'POST',
                catche: false,
                data: $("#frmConnectivityStatus").serialize(),
                async: false,
                success: function (response) {
                    $.unblockUI();
                    
                    if (response.status == true) {
                        alert('Records deleted successfully');
                        $('#btnReset').click();


                        $.ajax({
                            url: '/Master/Form3ConnectivityStatus/',
                            type: 'POST',
                            //catche: false,
                            data: $("#frmConnectivityStatusLayout").serialize(),
                            //async: false,
                            success: function (response) {
                                $.unblockUI();
                                //$('#dvLoadButtons').show('slow');


                                $("#dvLoadReport").html(response);
                                //alert($('#hdnflag').val());
                                if ($('#hdnflag').val() == "N") {
                                    $('#dvLoadButtons').show('slow');
                                    $('#dvLoadSaveButton').hide('slow');
                                }
                                else {
                                    $('#dvLoadSaveButton').show('slow');
                                    $('#dvLoadButtons').hide('slow');
                                }
                            },
                            error: function () {
                                $.unblockUI();
                                alert("Error ocurred");
                                return false;
                            },
                        });

                    }
                    else {
                        alert('Error occured on delete');
                    }
                },
                error: function () {
                    $.unblockUI();
                    alert("Error ocurred");
                    return false;
                },
            });
            //if ($("#flag").val() == "N") {
            //    alert(1);
            //    $('#divConnectivityStatus1').show('slow');
            //    $('#divConnectivityStatus2').hide('slow');
            //}
            //else {
            //    $('#divConnectivityStatus1').hide('slow');
            //    $('#divConnectivityStatus2').show('slow');
            //}
            $.unblockUI();
        }
    });


});
