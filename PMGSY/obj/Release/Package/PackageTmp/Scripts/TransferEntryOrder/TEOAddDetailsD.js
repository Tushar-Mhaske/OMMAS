var conD = "N";
var aggD = "N";
var sancyrD = "N";
var pkgD = "N";
var roadD = "N";

var transactionIdD = "";

$(document).ready(function () {

    $(document).unbind('keydown').bind('keydown', function (event) {
        var doPrevent = false;
        if (event.keyCode === 8) {
            var d = event.srcElement || event.target;
            if ((d.tagName.toUpperCase() === 'INPUT' && (d.type.toUpperCase() === 'TEXT' || d.type.toUpperCase() === 'PASSWORD'))
                 || d.tagName.toUpperCase() === 'TEXTAREA') {
                doPrevent = d.readOnly || d.disabled;
            }
            else {
                doPrevent = true;
            }
        }

        if (doPrevent) {
            event.preventDefault();
        }
    })

    if ($("#ddlContractorD").find('option').length != 0 && $("#ddlAgreementD").find('option').length != 0 && $("#ddlRoadD").find('option').length != 0) {

        if ($("#ddlContractorD").val() != "0" && $("#ddlContractorD").val() != "" && $("#ddlContractorD").val() != null && $("#ddlAgreementD").val() != "0" && $("#ddlAgreementD").val() != "" && $("#ddlAgreementD").val() != null && $("#ddlRoadD").val() != "0" && $("#ddlRoadD").val() != "" && $("#ddlRoadD").val() != null) {
            $.ajax({
                url: "/TEO/IsFinalPayment/" + $("#ddlContractorD").val() + "$" + $("#ddlAgreementD").val() + "$" + $("#ddlRoadD").val(),
                type: "POST",
                async: false,
                cache: false,
                success: function (data) {
                    if (data == "1") {
                        $("#trHeadIsFinalPayD").show();
                        $("#HeadIsFinalPayD").attr('checked', 'checked');
                        $("#HeadIsFinalPayD").attr('disabled', 'disabled');
                    }
                    else {
                        $("#trHeadIsFinalPayD").show();
                        $("#HeadIsFinalPayD").attr('checked', false);
                       // $("#HeadIsFinalPayD").attr('disabled', false);
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    //alert(xhr.responseText);
                    alert('Error occurred while processing your request');
                }
            });
        }
    }
    else if ($("#ddlHeadContractorD").find('option').length != 0 && $("#ddlHeadAgreementD").find('option').length != 0 && $("#ddlHeadRoadD").find('option').length != 0) {
        if ($("#ddlHeadContractorD").val() != "0" && $("#ddlHeadContractorD").val() != "" && $("#ddlHeadContractorD").val() != null && $("#ddlHeadAgreementD").val() != "0" && $("#ddlHeadAgreementD").val() != "" && $("#ddlHeadAgreementD").val() != null && $("#ddlHeadRoadD").val() != "0" && $("#ddlHeadRoadD").val() != "" && $("#ddlHeadRoadD").val() != null) {

            $.ajax({
                url: "/TEO/IsFinalPayment/" + $("#ddlHeadContractorD").val() + "$" + $("#ddlHeadAgreementD").val() + "$" + $("#ddlHeadRoadD").val(),
                type: "POST",
                async: false,
                cache: false,
                success: function (data) {
                    if (data == "1") {
                        $("#trHeadIsFinalPayD").show();
                        $("#HeadIsFinalPayD").attr('checked', 'checked');
                        $("#HeadIsFinalPayD").attr('disabled', 'disabled');
                    }
                    else {
                        $("#trHeadIsFinalPayD").show();
                        $("#HeadIsFinalPayD").attr('checked', false);
                       // $("#HeadIsFinalPayD").attr('disabled', false);
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    //alert(xhr.responseText);
                    alert('Error occurred while processing your request');
                }
            });
        }
    }

    


    $("#ddlDistrictD").change(function () {
        if ($("#ddlContractorD").css('display') != "none") {
            FillInCascadeDropdown(null, '#ddlContractorD', "/TEO/PopulateContractor/" + $("#ddlDistrictD").val());
            $("#ddlContractorD").trigger('change');
        }
        if ($("#trddlDPIUD").css('display') != "none") {
            FillInCascadeDropdown(null, '#ddlDPIUD', "/TEO/PopulateDPIU/" + $("#ddlDistrictD").val()+"$N");
        }
    });

    $("#ddlContractorD").change(function () {
        $("#divTEODetailsErrorD").hide("slide");
        $("#divTEODetailsErrorD span:eq(1)").html('');
        if ($("#ddlContractorD").val() != "" && $("#ddlContractorD").val() != null && $("#ddlContractorD").val() != 0) {
            $.ajax({
                url: "/Receipt/GetContractorName/" + $("#ddlContractorD").val(),
                type: "POST",
                async: false,
                cache: false,
                success: function (data) {
                    if ($("#ddlAgreementD").css('display') != "none") {
                        if ($("#ddlDistrictD").val() == 0) {
                            $("#divTEODetailsErrorD").show("slide");
                            $("#divTEODetailsErrorD span:eq(1)").html('<strong>Alert: </strong>Please Select District to get Agreement Details');
                            return false;
                        }
                        else {
                            var districtCode = 0;
                            if ($("#trddlDistrictC").css('display') != "none") {
                                districtCode = $("#ddlDistrictD").val();
                            }
                            FillInCascadeDropdown(null, '#ddlAgreementD', "/TEO/PopulateAgreement/" + districtCode + "$" + $("#ddlContractorD").val());

                        }
                    }
                    $("#ConSupNameD").text(data);
                    $("#trConSupNameD").show('slow');
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    //alert(xhr.responseText);
                    alert('Error occurred while processing your request');
                }
            });
        }
        else {
            $("#ConSupNameD").text("");
            $("#trConSupNameD").hide('slow');
            $("#ddlAgreementD").empty();
            $("#ddlRoadD").empty();
            $("#trHeadIsFinalPayD").hide('slow');

            //commented by Vikram on 10-10-2013
            //$("#ddlAgreementD").append("<option value=0>Select Agreement</option>");
            //$("#ddlRoadD").append("<option value=0>Select Road</option>");
        }

    });

    $("#ddlSancYearD").change(function () {

        if ($("#trddlPackageD").css('display') != "none") {
            if ($("#ddlRoadD").css('display') != "none") {
                $("#ddlRoadD").empty();
                $("#ddlRoadD").append("<option value='0'>Select Road</option>");
            }
            FillInCascadeDropdown(null, '#ddlPackageD', "/TEO/PopulatePackage/" + $("#ddlSancYearD").val());
        }
    });

    $("#ddlPackageD").change(function () {

        alert($("#ddlHeadD option:selected").val());

        if ($("#trddlRoadD").css('display') != "none") {
            //FillInCascadeDropdown(null, '#ddlRoadD', "/TEO/PopulateRoad/0$" + $("#ddlPackageD").val());
            FillInCascadeDropdown(null, '#ddlRoadD', "/TEO/PopulateRoad/0$" + $("#ddlPackageD option:selected").val() + "$0$" + $("#ddlContractorD option:selected").val() + "$" + $("#ddlHeadD option:selected").val() + "?" + $.param({ AGREEMENT_NUMBER: $("#ddlAgreementD option:selected").text() }));
           // $("#trHeadIsFinalPayD").show();
        }

    });

    $("#ddlAgreementD").change(function () {

        if ($("#ddlAgreementD").val() > 0) {
            if ($("#trddlRoadD").css('display') != "none") {
                //FillInCascadeDropdown(null, '#ddlRoadD', "/TEO/PopulateRoad/" + $("#ddlAgreementD").val() + "$0");
                FillInCascadeDropdown(null, '#ddlRoadD', "/TEO/PopulateRoad/" + $("#ddlAgreementD option:selected").val() + "$0$0$" + $("#ddlContractorD option:selected").val() + "?" + $.param({ AGREEMENT_NUMBER: $("#ddlAgreementD option:selected").text() }));
                $("#trHeadIsFinalPayD").show();
            }
        }
        else {
            $("#ddlRoadD").empty();
            $("#trHeadIsFinalPayD").hide('slow');
            $("#ddlRoadD").append("<option value=0>Select Road</option>");
        }
    });

    $("#ddlRoadD").change(function () {
        if ($(this).val() != "0") {
            if ($("#ddlContractorD").val() != "0" && $("#ddlContractorD").val() != "" &&  $("#ddlContractorD").val() != null && $("#ddlAgreementD").val() != "0" && $("#ddlAgreementD").val() != "" && $("#ddlAgreementD").val() != null  && $("#ddlRoadD").val != "0" && $("#ddlRoadD").val != "" && $("#ddlRoadD").val != null) {
                $.ajax({
                    url: "/TEO/IsFinalPayment/" + $("#ddlContractorD").val() + "$" + $("#ddlAgreementD").val() + "$" + $("#ddlRoadD").val(),
                    type: "POST",
                    async: false,
                    cache: false,
                    success: function (data) {
                        if (data == "1") {
                            $("#trHeadIsFinalPayD").show('slow');
                            $("#HeadIsFinalPayD").attr('checked', 'checked');
                            $("#HeadIsFinalPayD").attr('disabled', 'disabled');
                        }
                        else {
                            $("#trHeadIsFinalPayD").show('slow');
                            $("#HeadIsFinalPayD").attr('checked', false);
                         //   $("#HeadIsFinalPayD").attr('disabled', false);
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert('Error occurred while processing your request');
                        //alert(xhr.responseText);
                    }
                });
            }
        }
        else {
            $("#FINAL_PAYMENT").attr('checked', false);
            $("#trHeadIsFinalPayD").hide();
        }
    });

    $("#ddlHeadD").change(function () {
        if ($(this).val() != "0") {

            //added by Koustubh Nakate on 30/09/2013 to prvent same head selection for TEO
            var headID = $(this).val();
          //  $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            //working code but commeneted due to change in requirement
            /*$.blockUI({ message: 'Check for Same Account Head...' });
            $.ajax({
                type: 'GET',
                url: '/TEO/CheckHeadAlreadyExist/',
                async: false,
                cache: false,
                data: { BillID: billId, HeadID: $("#ddlHeadD").val(), CreditDebit: "C" },
                success: function (data) {

                    if (data.exist == true) {

                       
                        $("#ddlHeadD").find("option[value='" + headID + "']").remove(); 
                        alert('You can not select same account head.');
                        $("#ddlHeadD").val(0);

                        $.unblockUI();
                        return false;
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    $.unblockUI();
                }
            })*/



           // alert('b');


            hideAllSelectionD();
            $.ajax({
                url: "/TEO/GetHeadwiseDesignParams/" + billId + "/" + $("#ddlHeadD").val(),
                type: "POST",
                async: false,
                cache: false,
                success: function (data) {
                    conD = "N";
                    aggD = "N";
                    sancyrD = "N";
                    pkgD = "N";
                    roadD = "N";

                    if (data.CON_REQ == "Y") {
                        if ($("#trddlDistrictD").css('display') != "none" && $("#ddlDistrictD").val() != 0) {
                            FillInCascadeDropdown(null, '#ddlHeadContractorD', "/TEO/PopulateContractor/" + $("#ddlDistrictD").val());
                            $("#ddlHeadContractorD").trigger('change');
                        }
                        else {
                            FillInCascadeDropdown(null, '#ddlHeadContractorD', "/TEO/PopulateContractor/");
                        }
                        $("#trddlHeadContractorD").show('slow');
                        conD = "Y";
                    }
                    if (data.AGREEMENT_REQ == "Y") {
                        $("#trddlHeadAgreementD").show('slow');
                        aggD = "Y";
                    }
                    if (data.SANCYEAR_REQ == "Y") {
                        FillInCascadeDropdown(null, '#ddlHeadSancYearD', "/TEO/PopulateSancYear/");
                        //$("#ddlHeadSancYearD").append("option value='0'>Select Year</option>");
                        $("#trddlHeadSancYearD").show('slow');
                        $("#ddlHeadSancYearD").trigger('change');


                        sancyrD = "Y";
                       

                    }
                    if (data.PKG_REQ == "Y") {
                        $("#trddlHeadPackageD").show('slow');
                        //FillInCascadeDropdown(null, '#ddlHeadPackageC', "/TEO/PopulatePackage/" + 0);
                        //$("#ddlHeadPackageC").trigger('change');
                        $("#ddlHeadPackageD").empty();
                        $("#ddlHeadPackageD").append("<option value='0'>Select</option>");
                        pkgD = "Y";
                    }
                    if (data.ROAD_REQ == "Y")
                    {
                       
                        $("#trddlHeadRoadD").show('slow');

                        if ($("#ddlHeadD option:selected").val() == 28 || $("#ddlHeadD option:selected").val() == 29) {
                            $("#trHeadIsFinalPayD").show();
                        }
                        else {
                            $("#trHeadIsFinalPayD").hide();
                        }
                        $("#ddlHeadRoadD").empty();
                        $("#ddlHeadRoadD").append("<option value='0'>Select</option>");
                        roadD = "Y";

                       
                    }
                    else {
                        if (!$("#ddlRoadD").is(":visible") && !$("#ddlHeadRoadD").is(":visible")) {
                            $("#trHeadIsFinalPayD").hide('slow');
                        }
                        else if ($("#ddlRoadD").is(":visible") || $("#ddlHeadRoadD").is(":visible")) {
                            $("#trHeadIsFinalPayD").show();
                        }
                        else if (!$("#ddlRoadD").is(":visible") || !$("#ddlHeadRoadD").is(":visible")) {
                            $("#trHeadIsFinalPayD").hide('slow');

                        }
                        else {
                            if ($("#ddlHeadD option:selected").val() == 28 || $("#ddlHeadD option:selected").val() == 29) {
                                $("#trHeadIsFinalPayD").show();
                            }
                            else {
                                $("#trHeadIsFinalPayD").hide();
                            }
                        }
                        //$("#trHeadIsFinalPayD").hide('slow');
                    }

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    //alert(xhr.responseText);
                    alert('Error occurred while processing your request');
                    unblockPage();
                }
            });
        }
        else {
            $("#ddlHeadContractorD").empty();
            $("#trddlHeadContractorD").css('display', 'none');
            $("#HeadConSupNameD").text("");
            $("#trHeadConSupNameD").hide('slow');
            $("#ddlHeadAgreementD").empty();
            $("#trddlHeadAgreementD").css('display', 'none');
            $("#ddlHeadSancYearD").empty();
            $("#trddlHeadSancYearD").css('display', 'none');
            $("#ddlHeadPackageD").empty();
            $("#trddlHeadPackageD").css('display', 'none');
            $("#ddlHeadRoadD").empty();
            $("#FINAL_PAYMENT").attr('checked', false);
            $("#trddlHeadRoadD").css('display', 'none');
            $("#trHeadIsFinalPayD").css('display', 'none');
        }
    });

    $("#ddlHeadContractorD").change(function () {
        $("#divTEODetailsErrorD").hide("slide");
        $("#divTEODetailsErrorD span:eq(1)").html('');
        if ($("#ddlHeadContractorD").val() != "" && $("#ddlHeadContractorD").val() != null) {
            $.ajax({
                url: "/Receipt/GetContractorName/" + $("#ddlHeadContractorD").val(),
                type: "POST",
                async: false,
                cache: false,
                success: function (data) {
                    if (aggD == "Y") {
                        if ($("#ddlDistrictD").val() == 0) {
                            $("#divTEODetailsErrorD").show("slide");
                            $("#divTEODetailsErrorD span:eq(1)").html('<strong>Alert: </strong>Please Select District to get Agreement Details');
                            return false;
                        }
                        else {
                            FillInCascadeDropdown(null, '#ddlHeadAgreementD', "/TEO/PopulateAgreement/" + $("#ddlDistrictD").val() + "$" + $("#ddlHeadContractorD").val());

                        }
                    }
                    $("#HeadConSupNameD").text(data);
                    $("#trHeadConSupNameD").show('slow');
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    //alert(xhr.responseText);
                    alert('Error occurred while processing your request');
                }
            });
        }
        else {
            $("#HeadConSupNameD").text("");
            $("#trHeadConSupNameD").hide('slow');
            $("#ddlHeadAgreementD").empty();
            $("#ddlHeadRoadD").empty();
        }

    });

    $("#ddlHeadAgreementD").change(function () {
        if (roadD == "Y") {

            
            var year = $("#ddlHeadSancYearD option:selected").val() === undefined ? "0$" : $("#ddlHeadSancYearD option:selected").val();
            

            //FillInCascadeDropdown(null, '#ddlHeadRoadD', "/TEO/PopulateRoad/" + $("#ddlHeadAgreementD").val() + "$0");
            //FillInCascadeDropdown(null, '#ddlHeadRoadD', "/TEO/PopulateRoad/" + $("#ddlHeadAgreementD option:selected").val() + "$0$" + $("#ddlHeadSancYearD option:selected").val() + $("#ddlHeadContractorD option:selected").val() + "?" + $.param({ AGREEMENT_NUMBER: $("#ddlHeadAgreementD option:selected").text() }));
            FillInCascadeDropdown(null, '#ddlHeadRoadD', "/TEO/PopulateRoad/" + $("#ddlHeadAgreementD option:selected").val() + "$0$" + year + $("#ddlHeadContractorD option:selected").val() + "?" + $.param({ AGREEMENT_NUMBER: $("#ddlHeadAgreementD option:selected").text() }));

            $("#trHeadIsFinalPayD").show();
        }
    });

    $("#ddlHeadSancYearD").change(function () {

        //new change done by Vikram on 24-09-2013

        if ($("#btnEditTEODebitDetails").is(':visible')) {
            //$("#ddlHeadD").trigger('change');
            pkgD = 'Y';
            roadD = 'Y';
        }
        //end of change

        if (pkgD == "Y") {
            if (roadD == "Y") {
                $("#ddlHeadRoadD").empty();
                $("#ddlHeadRoadD").append("<option value='0'>Select Road Name</option>");
            }
            FillInCascadeDropdown(null, '#ddlHeadPackageD', "/TEO/PopulatePackage/" + $("#ddlHeadSancYearD").val());
        }
    });

    $("#ddlHeadPackageD").change(function () {

        //new change done by Vikram on 24-09-2013

        if ($("#btnEditTEODebitDetails").is(':visible')) {
            //$("#ddlHeadD").trigger('change');
            pkgD = 'Y';
            roadD = 'Y';
        }
        //end of change

        if (roadD == "Y") {
            
            //FillInCascadeDropdown(null, '#ddlHeadRoadD', "/TEO/PopulateRoad/0$" + $("#ddlHeadPackageD").val() + "$" + $("#ddlHeadSancYearD").val());
            FillInCascadeDropdown(null, '#ddlHeadRoadD', "/TEO/PopulateRoad/0" + "$" + $("#ddlHeadPackageD option:selected").text() + "$" + $("#ddlHeadSancYearD option:selected").val() + "$" + $("#ddlHeadContractorD option:selected").val() + "$" + $("#ddlHeadD option:selected").val() + "?" + $.param({ AGREEMENT_NUMBER: $("#ddlHeadAgreementD option:selected").text() }));
           // $("#trHeadIsFinalPayD").show();
        }
    });

    $("#ddlHeadRoadD").change(function () {
        if ($(this).val() != "0") {
            if ($("#ddlHeadContractorD").val() != "0" && $("#ddlHeadContractorD").val() != "" && $("#ddlHeadAgreementD").val() != "0" && $("#ddlHeadAgreementD").val() != "" && $("#ddlHeadRoadD").val != "0" && $("#ddlHeadRoadD").val != "") {
                $.ajax({
                    url: "/TEO/IsFinalPayment/" + $("#ddlHeadContractorD").val() + "$" + $("#ddlHeadAgreementD").val() + "$" + $("#ddlHeadRoadD").val(),
                    type: "POST",
                    async: false,
                    cache: false,
                    success: function (data) {
                        if (data == "1") {
                            $("#trHeadIsFinalPayD").show();
                            $("#HeadIsFinalPayD").attr('checked', 'checked');
                            $("#HeadIsFinalPayD").attr('disabled', 'disabled');
                        }
                        else {
                            $("#trHeadIsFinalPayD").show();
                            $("#HeadIsFinalPayD").attr('checked', false);
                            // $("#HeadIsFinalPayD").attr('disabled', false);
                            // new change done by Vikram on 23-09-2013
                            $("#HeadIsFinalPayD").attr('disabled', 'disabled');
                            //end of change
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        //alert(xhr.responseText);
                        alert('Error occurred while processing your request');
                    }
                });
            }
        }
        else {
            $("#FINAL_PAYMENT").attr('checked', false);
            $("#trHeadIsFinalPayD").hide();
        }
    });

    $.validator.unobtrusive.parse($('#frmTEOAddDetailsD'));

    

    $("#btnResetD").click(function (event) {

        event.preventDefault();

        if ($("#trddlHeadD").is(':visible')) {
            $("#ddlHeadD").val(0);
            $("#ddlHeadD").trigger('change');
        }


        $("#ddlHeadContractorD").parent('td').find('span').html("");
        $("#ddlHeadAgreementD").parent('td').find('span').html("");
        $("#ddlHeadSancYearD").parent('td').find('span').html("");
        $("#ddlHeadPackageD").parent('td').find('span').html("");
        $("#ddlHeadRoadD").parent('td').find('span').html("");
        $("#AMOUNTD").parent('td').find('span').html("");
        $("#ddlDistrictD").parent('td').find('span').html("");
        $("#ddlDPIUD").parent('td').find('span').html("");
        $("#ddlContractorD").parent('td').find('span').html("");
        $("#ddlAgreementD").parent('td').find('span').html("");
        $("#ddlSancYearD").parent('td').find('span').html("");
        $("#ddlPackageD").parent('td').find('span').html("");
        $("#ddlRoadD").parent('td').find('span').html("");
        $("#divTEODetailsErrorD").hide("slide");
        $("#divTEODetailsErrorD span:eq(1)").html("");

        
        if (!$("#ddlContractorD").is(':disabled')) {
            $('#ConSupNameD').text('');
            $("#trConSupNameD").hide('slow');
            $("#ddlAgreementD").empty();
            $("#ddlRoadD").empty();
            $("#trHeadIsFinalPayD").hide('slow');

            //$("#ddlAgreementD").append("<option value=0>Select Agreement</option>");
            //$("#ddlRoadD").append("<option value=0>Select Road</option>");
        }

        $('#frmTEOAddDetailsD option').each(function () {

            $(this).removeAttr('selected');
            $("#NARRATION").val('');
            $("#AMOUNTD").val('');
        });
 

        LoadTEODetailsGrid($("#tblTEOMasterGrid").getDataIDs()[0]);


    });

    var showConDDlForDeduction = false;

    $("#btnSaveTEODebitDetails").click(function (evt) {

        evt.preventDefault();

        var agrmntDisabled = false;
        var contractorDisabled = false;
        if ($("#ddlContractorD").is(':disabled')) {
            contractorDisabled = true;
        }

        if ($("#ddlAgreementD").is(':disabled')) {
            agrmntDisabled = true;
        }

        var formStatus = $('#frmTEOAddDetailsD').valid();
        var status = validateDebitDetails($("#AMOUNTD").val(), amountValD);
        if (formStatus && status) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

                $("#HeadIsFinalPayD").attr('disabled', false);
                $("#ddlDistrictD").attr('disabled', false);
                $("#ddlDPIUD").attr('disabled', false);
                $("#ddlContractorD").attr('disabled', false);
                $("#ddlHeadContractorD").attr('disabled', false);
                $("#ddlAgreementD").attr('disabled', false);
                $("#ddlHeadAgreementD").attr('disabled', false);
                $("#ddlRoadD").attr('disabled', false);
                $("#ddlHeadRoadD").attr('disabled', false);
                $("#ddlHeadD").attr('disabled', false);

                if ($("#trddlContractorD").is(":visible")) {
                    showConDDlForDeduction = true;
                }

            $.ajax({
                url: "/TEO/AddDebitTEODetails/" + $("#tblTEOMasterGrid").getDataIDs()[0],
                type: "POST",
                async: false,
                cache: false,
                data: $("#frmTEOAddDetailsD").serialize(),
                success: function (data) {
                    $.unblockUI();

                    if (!data.success || data.success == "undefined")
                    {
                        //new change done by Vikram on 16-10-2013
                        if (data.status == -555) {
                            $("#divTEODetailsErrorD").show("slide");
                            $("#divTEODetailsErrorD span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                            $("#HeadIsFinalPayD").attr('disabled', 'disabled');
                        }
                        //end of change

                        if (data.message == "undefined" || data.message == null) {
                            $("#loadTEODebitDetails").html(data);
                            $("#ddlHeadD").val(0);
                            $.each($("select"), function () {
                                //if ($(this).find('option').length > 0) {
                                //    $('#tr' + $(this).attr('id')).show();
                                //}
                               
                                //new change done by Vikram on 10-10-2013
                                    if ($(this).find('option').length > 0) {
                                        if (($(this).attr('id')).slice(-1) == 'C') {
                                        }
                                            //else if (($(this).attr('id')) == '') { alert('a');}
                                        else {
                                                $('#tr' + $(this).attr('id')).show();
                                        }
                                    }
                                //end of change
                            });
                            
                            /******************************************/

                            var vpCHead = 0;
                            var vpDHead = 0;
                            var vpDistRepeat = "N";
                            var vpDPIURepeat = "N";
                            var vpContRepeat = "N";
                            var vpSupRepeat = "N";
                            var vpAggRepeat = "N";
                            var vpRoadRepeat = "N";
                            var vpHeadRepeat = "N";

                            $.ajax({
                                url: "/TEO/GetTEOValidationParams/" + billId,
                                type: "POST",
                                async: false,
                                cache: false,
                                success: function (data) {
                                    vpCHead = data.CHead;
                                    vpDHead = data.DHead;
                                    vpDistRepeat = data.District;
                                    vpDPIURepeat = data.DPIU;
                                    vpContRepeat = data.Contractor;
                                    vpSupRepeat = data.Supplier;
                                    vpAggRepeat = data.Agreement;
                                    vpRoadRepeat = data.Road;
                                    vpHeadRepeat = data.Head;
                                    var contID = 0;

                                    if ($("#ddlHeadContractorD").find('option').length >= 1) {
                                        contID = $("#ddlHeadContractorD").val();
                                    }
                                    else if ($("#ddlContractorD").find('option').length >= 1) {
                                        contID = $("#ddlContractorD").val();
                                    }

                                    $.ajax({
                                        url: "/Receipt/GetContractorName/" + contID,
                                        type: "POST",
                                        async: false,
                                        cache: false,
                                        success: function (data) {
                                            if ($("#ddlHeadContractorD").find('option').length >= 1) {
                                                $("#HeadConSupNameD").text(data);
                                                $("#trHeadConSupNameD").show('slow');
                                            }
                                            else if ($("#ddlContractorD").find('option').length >= 1) {
                                                $("#ConSupNameD").text(data);
                                                $("#trConSupNameD").show('slow');
                                            }
                                        },
                                        error: function (xhr, ajaxOptions, thrownError) {
                                            //alert(xhr.responseText);
                                            alert('Error occurred while processing your request');
                                        }
                                    });
                                    if (vpDistRepeat == "Y") {
                                        $("#ddlDistrictD").attr('disabled', 'disabled');
                                    }
                                    if (vpDPIURepeat == "Y") {
                                        $("#ddlDPIUD").attr('disabled', 'disabled');
                                    }
                                    if (vpContRepeat == "Y" || vpSupRepeat == "Y") {
                                        $("#ddlContractorD").attr('disabled', 'disabled');
                                        $("#ddlHeadContractorD").attr('disabled', 'disabled');
                                    }
                                    if (vpAggRepeat == "Y") {
                                        $("#ddlAgreementD").attr('disabled', 'disabled');
                                        $("#ddlHeadAgreementD").attr('disabled', 'disabled');
                                    }
                                    if (vpRoadRepeat == "Y") {
                                        $("#ddlRoadD").attr('disabled', 'disabled');
                                        $("#ddlHeadRoadD").attr('disabled', 'disabled');
                                    }
                                    if (vpHeadRepeat == "Y") {
                                        $("#ddlHeadD").attr('disabled', 'disabled');
                                    }
                                },
                                error: function (xhr, ajaxOptions, thrownError) {

                                }
                            });
                            /*******************************************/
                            //$("#ddlHeadD").trigger('change');

                            $("#ddlContractorD").attr('disabled', false);
                            //$("#ddlHeadContractorD").attr('disabled', false);
                            $("#ddlAgreementD").attr('disabled', false);

                            return false;
                        }
                        else {
                            if (agrmntDisabled == true) {
                                $("#ddlAgreementD").attr('disabled', true);
                            }
                            if (contractorDisabled == true) {
                                $("#ddlContractorD").attr('disabled', true);
                            }
                            $("#HeadIsFinalPayD").attr('disabled', true)
                            $("#divTEODetailsErrorD").show("slide");
                            $("#divTEODetailsErrorD span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                        }
                        return false;
                    }
                    else {

                        $("#divTEODetailsErrorD").hide("slide");

                        $("#divTEODetailsErrorD span:eq(1)").html('');

                        if ($("#trHeadConSupNameD").css('display') != "none") {
                            $("#trHeadConSupNameD").hide('slow');
                            $("#HeadConSupNameD").text("");
                        }
                        $("#btnResetD").trigger('click');
                     
                        //$("#ddlHeadD").trigger('change');

                        LoadTEODetailsGrid($("#tblTEOMasterGrid").getDataIDs()[0]);

                        alert("Debit Details Added");

                        if (showConDDlForDeduction) {
                            $("#trddlContractorD").show()
                        }
                        else {
                            $("#trddlContractorD").hide()
                        }

                       
                        //Added By Abhishek kamble to Hide Cform start
                        if (_fundType == "M" && isMulTxn == "N") {
                            $("#loadTEODebitDetails").hide();
                        }
                        //Added By Abhishek kamble to Hide Cform end

                        //$("#btnResetD").trigger('click');

                        return false;
                    }

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    $.unblockUI();
                    //alert(xhr.responseText);
                    alert('Error occurred while processing your request');
                }
            });
        }

    });

    $("#btnCancelTEODebitDetails").click(function () {

        if ($("#divFinalizeTEO").css('display') != "none") {
            $("#loadTEOCreditDetails").html('');
            $("#loadTEODebitDetails").html('');
        }
        else {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/TEO/TEODetails/" + $("#tblTEOMasterGrid").getDataIDs()[0] + "/D",
                type: "POST",
                async: false,
                cache: false,
                success: function (data) {
                    $.unblockUI();

                    $("#loadTEODebitDetails").html(data);
                    $.each($("select"), function () {
                        if ($(this).find('option').length >= 1) {
                            $('#tr' + $(this).attr('id')).show();
                        }
                    });

                    // new change done by Vikram 

                    $.ajax({
                        url: "/TEO/GetTEOValidationParams/" + billId,
                        type: "POST",
                        async: false,
                        cache: false,
                        success: function (data) {
                            vpCHead = data.CHead;
                            vpDHead = data.DHead;
                            vpDistRepeat = data.District;
                            vpDPIURepeat = data.DPIU;
                            vpContRepeat = data.Contractor;
                            vpSupRepeat = data.Supplier;
                            vpAggRepeat = data.Agreement;
                            vpRoadRepeat = data.Road;
                            vpHeadRepeat = data.Head;

                            if (vpContRepeat == "Y" || vpSupRepeat == "Y") {
                                $("#ddlContractorD").attr('disabled', 'disabled');
                                $("#ddlHeadContractorD").attr('disabled', 'disabled');
                            }
                            if (vpAggRepeat == "Y") {
                                $("#ddlAgreementD").attr('disabled', 'disabled');
                                $("#ddlHeadAgreementD").attr('disabled', 'disabled');
                            }
                            if (vpRoadRepeat == "Y") {
                                $("#ddlRoadD").attr('disabled', 'disabled');
                                $("#ddlHeadRoadD").attr('disabled', 'disabled');
                            }


                            var contID = 0;

                            if ($("#ddlHeadContractorD").find('option').length >= 1) {
                                contID = $("#ddlHeadContractorD").val();
                            }
                            else if ($("#ddlContractorD").find('option').length >= 1) {
                                contID = $("#ddlContractorD").val();
                            }

                            // alert('In MutTXN=Yes and debitContractor=' + contID);

                            $.ajax({
                                url: "/Receipt/GetContractorName/" + contID,
                                type: "POST",
                                async: false,
                                cache: false,
                                success: function (data) {
                                    if ($("#ddlHeadContractorD").find('option').length >= 1) {
                                        $("#HeadConSupNameD").text(data);
                                        $("#trHeadConSupNameD").show('slow');
                                    }
                                    else if ($("#ddlContractorD").find('option').length >= 1) {
                                        $("#ConSupNameD").text(data);
                                        $("#trConSupNameD").show('slow');
                                    }

                                },
                                error: function (xhr, ajaxOptions, thrownError) {
                                    //alert(xhr.responseText);
                                    alert('Error occurred while processing your request');
                                }
                            });



                        },
                        error: function (xhr, ajaxOptions, thrownError) {

                        }
                    });

                    //end of changes

                    LoadTEODetailsGrid($("#tblTEOMasterGrid").getDataIDs()[0]); //new change done by Vikram

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    $.unblockUI();

                    //alert(xhr.responseText);
                    alert('Error occurred while processing your request');
                    unblockPage();
                }
            });
        }

    });

    $("#btnEditTEODebitDetails").click(function (evt) {
        evt.preventDefault();

        if (transactionIdD != 0 && transactionIdD != "") {
            transId = transactionIdD;
        }


        var formStatus = $('#frmTEOAddDetailsD').valid();
        var status = validateDebitDetails($("#AMOUNTD").val(), amountValD_Debit);
        if (formStatus && status) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $("#HeadIsFinalPayD").attr('disabled', false);
            $("#ddlDistrictD").attr('disabled', false);
            $("#ddlDPIUD").attr('disabled', false);
            $("#ddlContractorD").attr('disabled', false);
            $("#ddlHeadContractorD").attr('disabled', false);
            $("#ddlAgreementD").attr('disabled', false);
            $("#ddlHeadAgreementD").attr('disabled', false);
            $("#ddlRoadD").attr('disabled', false);
            $("#ddlHeadRoadD").attr('disabled', false);
            $("#ddlHeadD").attr('disabled', false);
            $.ajax({
                url: "/TEO/EditDebitTEODetails/" + transId,
                type: "POST",
                async: false,
                cache: false,
                data: $("#frmTEOAddDetailsD").serialize(),
                success: function (data) {
                    $.unblockUI();

                    if (!data.success || data.success == "undefined") {

                        //new change done by Vikram on 16-10-2013
                        if (data.status == -555) {
                            $("#divTEODetailsErrorD").show("slide");
                            $("#divTEODetailsErrorD span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                        }
                        //end of change

                        if (data.message == "undefined" || data.message == null) {
                            $("#loadTEODebitDetails").html(data);
                            $("#ddlHeadD").val(0);
                            $.each($("select"), function () {
                                if ($(this).find('option').length > 0) {
                                    $('#tr' + $(this).attr('id')).show();
                                }
                            });

                            var userdata = $("#tblTEODetailsGrid").getGridParam('userData');
                            if (userdata.multipleTrans == 'N') {
                                if ($("#ddlDistrictD").is(':visible')) {
                                    $("#ddlDistrictD").attr('disabled', true);
                                }

                                if ($("#ddlDPIUD").is(':visible')) {
                                    $("#ddlDPIUD").attr('disabled', true);
                                }

                                //Added By Abhishek kamble 21Nov2014 to disable ddl for MF start
                                if (_fundType == "M") {
                                    $("#HeadIsFinalPayD").attr('disabled', true);
                                    $("#ddlDistrictD").attr('disabled', true);
                                    $("#ddlDPIUD").attr('disabled', true);
                                    $("#ddlContractorD").attr('disabled', true);
                                    $("#ddlHeadContractorD").attr('disabled', true);
                                    $("#ddlAgreementD").attr('disabled', true);
                                    $("#ddlHeadAgreementD").attr('disabled', true);
                                    $("#ddlRoadD").attr('disabled', true);
                                    $("#ddlHeadRoadD").attr('disabled', true);
                                    $("#ddlHeadD").attr('disabled', true);
                                }
                                //Added By Abhishek kamble 21Nov2014 to disable ddl for MF end 

                            }


                            $("#ddlHeadD").trigger('change');
                            return false;
                        }
                        else {
                            $("#divTEODetailsErrorD").show("slide");
                            $("#divTEODetailsErrorD span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                        }
                        return false;
                    }
                    else {
                        $("#divTEODetailsErrorD").hide("slide");
                        $("#divTEODetailsErrorD span:eq(1)").html('');
                        if ($("#trHeadConSupNameD").css('display') != "none") {
                            $("#trHeadConSupNameD").hide('slow');
                            $("#HeadConSupNameD").text("");
                        }
                        $("#btnCancelTEODebitDetails").trigger('click');
                        LoadTEODetailsGrid($("#tblTEOMasterGrid").getDataIDs()[0]);
                        alert("Debit Details Updated");
                        return false;
                    }

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    $.unblockUI();

                    //alert(xhr.responseText);
                    alert('Error occurred while processing your request');
                }
            });
        }

    });

    if (transIdD != 0 && transIdD != "") {
        transactionIdD = transIdD;
    }



});//Document.ready ends here

function FillInCascadeDropdown(map, dropdown, action) {

    $(dropdown).empty()

    $.post(action, map, function (data) {
        ddvalues = data;
        $.each(data, function () {

            $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
        });

    }, "json");
}


function hideAllSelectionD() {
    $("#trddlHeadContractorD").hide('slow');
    $("#trHeadConSupNameD").hide('slow');
    $("#HeadConSupNameD").text("");
    $("#trddlHeadAgreementD").hide('slow');
    $("#trddlHeadSancYearD").hide('slow');
    $("#trddlHeadPackageD").hide('slow');
    $("#trddlHeadRoadD").hide('slow');
    $("#divTEODetailsErrorD").hide("slide");
    $("#divTEODetailsErrorD span:eq(1)").html('');
}


function validateDebitDetails(amountToValidate, oldValue) {

    $("#divTEODetailsErrorD").hide("slide");
    $("#divTEODetailsErrorD span:eq(1)").html('');

    var CrDrDetails = $("#tblTEODetailsGrid").jqGrid('getCol', 'CreditDebit', false);
    var DrCount = 0;
    if (CrDrDetails.length != 0) {
        for (var i = 0; i < CrDrDetails.length; i++) {
            if (CrDrDetails[i] == "Debit") {
                DrCount = DrCount + 1;
            }
        }
    }

   /* if (isMulTxn == "N" && DrCount == 1) {
        $("#divTEODetailsErrorD").show("slide");
        $("#divTEODetailsErrorD span:eq(1)").html('<strong>Alert: </strong>Only single transaction entry allowed');
        return false;
    }
    else {
        $("#divTEODetailsErrorD").hide("slide");
        $("#divTEODetailsErrorD span:eq(1)").html('');
    }
    */
    var masterAmount = $("#tblTEOMasterGrid").jqGrid('getCell', $("#tblTEOMasterGrid").getDataIDs()[0], 'GrossAmount');
    var detailsAmount = 0;
  //  alert(isDetailsGridLoaded)
   // alert(amountToValidate)
    
    if (isDetailsGridLoaded) {

        detailsAmount = parseFloat(($("#tblTEODetailsGrid").jqGrid('getCol', 'DAmount', false, "sum") - oldValue)) + parseFloat(amountToValidate);
    }
    else {
        detailsAmount = amountToValidate;
    }
    var statusFlag = true;
    $.ajax({
        url: "/TEO/GetTransDesignParams/" + billId,
        type: "POST",
        async: false,
        cache: false,
        success: function (data) {
            if (data.DIST_REQ == "Y" && $("#ddlDistrictD").val() == "0") {
                $("#ddlDistrictD").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlDistrictD">District Required</span>').show();
                $("#ddlDistrictD").addClass("input-validation-error");
                statusFlag = false;
            }
            if (data.DPIU_REQ == "Y" && $("#ddlDPIUD").val() == "0") {
                $("#ddlDPIUD").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlDPIUD">DPIU Required</span>').show();
                $("#ddlDPIUD").addClass("input-validation-error");
                statusFlag = false;
            }
            if (data.CON_REQ == "Y" && $("#ddlContractorD").val() == "0") {
                $("#ddlContractorD").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlContractorD">Contractor Required</span>').show();
                $("#ddlContractorD").addClass("input-validation-error");
                statusFlag = false;
            }
            if (data.SUP_REQ == "Y" && $("#ddlContractorD").val() == "0") {
                $("#ddlContractorD").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlContractorD">Supplier Required</span>').show();
                $("#ddlContractorD").addClass("input-validation-error");
                statusFlag = false;
            }
            if (data.AGREEMENT_REQ == "Y" && $("#ddlAgreementD").val() == "0") {
                $("#ddlAgreementD").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlAgreementD">Agreement Required</span>').show();
                $("#ddlAgreementD").addClass("input-validation-error");
                statusFlag = false;
            }
            if (data.SANCYEAR_REQ == "Y" && $("#ddlSancYearD").val() == "0") {
                $("#ddlSancYearD").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlSancYearD">Sanction Year Required</span>').show();
                $("#ddlSancYearD").addClass("input-validation-error");
                statusFlag = false;
            }
            if (data.PKG_REQ == "Y" && $("#ddlPackageD").val() == "0") {
                $("#ddlPackageD").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlPackageD">Package Required</span>').show();
                $("#ddlPackageD").addClass("input-validation-error");
                statusFlag = false;
            }
            if (data.ROAD_REQ == "Y" && $("#ddlRoadD").val() == "0") {
                $("#ddlRoadD").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlRoadD">Road Required</span>').show();
                $("#ddlRoadD").addClass("input-validation-error");
                statusFlag = false;
            }


            if (parseFloat(masterAmount) < parseFloat(detailsAmount.toFixed(2))) {
             //   alert("masterAmount : " + masterAmount + "detailsAmount:  " + detailsAmount.toFixed(2));

                $("#AMOUNTD").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="AMOUNTD">Total Details Amount Exceeds Master Amount</span>').show();
                $("#AMOUNTD").addClass("input-validation-error");
                statusFlag = false;
            }

            ////added by amol jadhav //if multiple transaction is not allowed details amount must be equal to 
            //if (isMulTxn == "N" && parseFloat(masterAmount) != parseFloat(detailsAmount)) {
            //    $("#AMOUNTD").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="AMOUNTD">Details Amount should be equal to master amount as multiple transaction is not allowed</span>').show();
            //    $("#AMOUNTD").addClass("input-validation-error");
            //    statusFlag = false;

            //}
        },
        error: function (xhr, ajaxOptions, thrownError  ) {
            //alert(xhr.responseText);
            alert('Error occurred while processing your request');
        }
    });

    if (conD == "Y" && $("#ddlHeadContractorD").val() == "0") {
        $("#ddlHeadContractorD").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlHeadContractorD">Contractor Required</span>').show();
        $("#ddlHeadContractorD").addClass("input-validation-error");
        statusFlag = false;
    }
    if (aggD == "Y" && $("#ddlHeadAgreementD").val() == "0") {
        $("#ddlHeadAgreementD").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlHeadAgreementD">Agreement Required</span>').show();
        $("#ddlHeadAgreementD").addClass("input-validation-error");
        statusFlag = false;
    }
    if (sancyrD == "Y" && $("#ddlHeadSancYearD").val() == "0") {
        $("#ddlHeadSancYearD").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlHeadSancYearD">Sanction Year Required</span>').show();
        $("#ddlHeadSancYearD").addClass("input-validation-error");
        statusFlag = false;
    }
    if (pkgD == "Y" && $("#ddlHeadPackageD").val() == "0") {
        $("#ddlHeadPackageD").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlHeadPackageD">Package Required</span>').show();
        $("#ddlHeadPackageD").addClass("input-validation-error");
        statusFlag = false;
    }
    if (roadD == "Y" && $("#ddlHeadRoadD").val() == "0") {
        $("#ddlHeadRoadD").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlHeadRoadD">Road Required</span>').show();
        $("#ddlHeadRoadD").addClass("input-validation-error");
        statusFlag = false;
    }

    return statusFlag;
}


















