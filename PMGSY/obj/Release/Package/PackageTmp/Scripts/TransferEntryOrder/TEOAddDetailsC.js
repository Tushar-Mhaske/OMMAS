var conC = "N";
var aggC = "N";
var sancyrC = "N";
var pkgC = "N";
var roadC = "N";

var transactionIdC = "";//for validation of total amount

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

    if ($("#ddlContractorC").find('option').length != 0 && $("#ddlAgreementC").find('option').length != 0 && $("#ddlRoadC").find('option').length != 0) {

        if (($("#ddlContractorC").val() != 0 && $("#ddlContractorC").val() != "" && $("#ddlContractorC").val() != null) && ($("#ddlAgreementC").val() != "0" && $("#ddlAgreementC").val() != "" && $("#ddlAgreementC").val() != null) && ($("#ddlRoadC").val() != "0" && $("#ddlRoadC").val() != "" && $("#ddlRoadC").val() != null)) {
            $.ajax({
                url: "/TEO/IsFinalPayment/" + $("#ddlContractorC").val() + "$" + $("#ddlAgreementC").val() + "$" + $("#ddlRoadC").val(),
                type: "POST",
                data: { BillID: billId },
                async: false,
                cache: false,
                success: function (data) {
                    if (data == "1") {
                        $("#trHeadIsFinalPayC").show();
                        $("#HeadIsFinalPayC").attr('checked', 'checked');
                        $("#HeadIsFinalPayC").attr('disabled', 'disabled');
                    }
                    else {
                        $("#trHeadIsFinalPayC").show();
                        $("#HeadIsFinalPayC").attr('checked', false);
                       // $("#HeadIsFinalPayC").attr('disabled', false);
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert('Error occurred while processing your request');
                   // alert(xhr.responseText);
                }
            });
        }
    }
    else if ($("#ddlHeadContractorC").find('option').length != 0 && $("#ddlHeadAgreementC").find('option').length != 0 && $("#ddlHeadRoadC").find('option').length != 0) {
        if ($("#ddlHeadContractorC").val() != "0" && $("#ddlHeadContractorC").val() != "" && $("#ddlHeadAgreementC").val() != "0" && $("#ddlHeadAgreementC").val() != "" && $("#ddlHeadRoadC").val() != "0" && $("#ddlHeadRoadC").val() != "") {
           $.ajax({
                url: "/TEO/IsFinalPayment/" + $("#ddlHeadContractorC").val() + "$" + $("#ddlHeadAgreementC").val() + "$" + $("#ddlHeadRoadC").val(),
                type: "POST",
                data: { BillID: billId },
                async: false,
                cache: false,
                success: function (data) {
                    if (data == "1") {
                        $("#trHeadIsFinalPayC").show();
                        $("#HeadIsFinalPayC").attr('checked', 'checked');
                        $("#HeadIsFinalPayC").attr('disabled', 'disabled');
                    }
                    else {
                        $("#trHeadIsFinalPayC").show();
                        $("#HeadIsFinalPayC").attr('checked', false);
                        //$("#HeadIsFinalPayC").attr('disabled', false);
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    //alert(xhr.responseText);
                    alert('Error occurred while processing your request');
                }
            });
        }
    }

    $("#ddlDistrictC").change(function () {
        if ($("#ddlContractorC").css('display') != "none") {
            FillInCascadeDropdown(null, '#ddlContractorC', "/TEO/PopulateContractor/" + $("#ddlDistrictC").val());
            $("#ddlContractorC").trigger('change');
        }
        if ($("#trddlDPIUC").css('display') != "none") {
            FillInCascadeDropdown(null, '#ddlDPIUC', "/TEO/PopulateDPIU/" + $("#ddlDistrictC").val()+"$N");

        } 
    });

    $("#ddlContractorC").change(function () {
 
        $("#divTEODetailsErrorC").hide("slide");
        $("#divTEODetailsErrorC span:eq(1)").html('');
        if ($("#ddlContractorC").val() != "" && $("#ddlContractorC").val() != null && $("#ddlContractorC").val()!=0) {
            $.ajax({
                url: "/Receipt/GetContractorName/" + $("#ddlContractorC").val(),
                type: "POST",
                async: false,
                cache: false,
                success: function (data) {
                    if ($("#ddlAgreementC").css('display') != "none") {
                        if ($("#ddlDistrictC").val() == 0) {
                            $("#divTEODetailsErrorC").show("slide");
                            $("#divTEODetailsErrorC span:eq(1)").html('<strong>Alert: </strong>Please Select District to get Agreement Details');
                            return false;
                        }
                        else {
                            var districtCode = 0;
                            if ($("#trddlDistrictC").css('display') != "none") {
                                districtCode = $("#ddlDistrictC").val();
                            }
                            FillInCascadeDropdown(null, '#ddlAgreementC', "/TEO/PopulateAgreement/" + districtCode + "$" + $("#ddlContractorC").val());

                        }
                    }
                    $("#ConSupNameC").text(data);
                    $("#trConSupNameC").show('slow');
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    //alert(xhr.responseText);
                    alert('Error occurred while processing your request');
                }
            });
        }
        else {
            $("#ConSupNameC").text("");
            $("#trConSupNameC").hide('slow');
            $("#ddlAgreementC").empty();
            $("#ddlRoadC").empty();
            $("#trHeadIsFinalPayC").hide('slow');
            $("#ddlAgreementC").append("<option value=0>Select Agreement</option>");
            $("#ddlRoadC").append("<option value=0>Select Road</option>");
        }

    });

    $("#ddlSancYearC").change(function () {
        if ($("#trddlPackageC").css('display') != "none") {
            if ($("#trddlRoadC").css('display') != "none") {
                $("#ddlRoadC").empty();
                $("#ddlRoadC").append("<option value='0'>Select Road</option>");
            }
            FillInCascadeDropdown(null, '#ddlPackageC', "/TEO/PopulatePackage/" + $("#ddlSancYearC").val());
        }
    });

    $("#ddlPackageC").change(function () {
        if ($("#trddlRoadC").css('display') != "none") {
            FillInCascadeDropdown(null, '#ddlRoadC', "/TEO/PopulateRoad/0$" + $("#ddlPackageC").val() + "$0$" + $("#ddlContractorC option:selected").val() + "$" + $("#ddlHeadC option:selected").val() + "?" + $.param({ AGREEMENT_NUMBER: $("#ddlAgreementC option:selected").text() }));
           // $("#trHeadIsFinalPayC").show();
        }
    });

    $("#ddlAgreementC").change(function () {
        if ($("#ddlAgreementC").val() > 0) {
            if ($("#trddlRoadC").css('display') != "none") {
                FillInCascadeDropdown(null, '#ddlRoadC', "/TEO/PopulateRoad/" + $("#ddlAgreementC").val() + "$0" + "$0$" + $("#ddlContractorC option:selected").val() + "?" + $.param({AGREEMENT_NUMBER : $("#ddlAgreementC option:selected").text()}));
                $("#trHeadIsFinalPayC").show();
            }
        }
        else {
            $("#ddlRoadC").empty();
            $("#trHeadIsFinalPayC").hide('slow'); 
            $("#ddlRoadC").append("<option value=0>Select Road</option>");
        }
    });

    $("#ddlRoadC").change(function () {

        if ($(this).val() != "0")
        {
            if ($("#ddlContractorC").val() != "0" && $("#ddlContractorC").val() != "" && $("#ddlContractorC").val() != null && $("#ddlAgreementC").val() != "0" && $("#ddlAgreementC").val() != "" && $("#ddlAgreementC").val() != null && $("#ddlRoadC").val != "0" && $("#ddlRoadC").val != "" && $("#ddlRoadC").val != null) {
                $.ajax({
                    //url: "/TEO/IsFinalPayment/" + $("#ddlContractorC").val() + "$" + $("#ddlAgreementC").val() + "$" + $("#ddlRoadC").val(),//commented by Vikram
                    url: "/TEO/IsFinalPayment?id=" + $("#ddlContractorC").val() + "$" + $("#ddlAgreementC").val() + "$" + $("#ddlRoadC").val() ,
                    type: "POST",
                    async: false,
                    cache: false,
                    data:{BillID:billId},
                    success: function (data) {
                        if (data == "1")
                        {
                            $("#trHeadIsFinalPayC").show('slow');
                            $("#HeadIsFinalPayC").attr('checked', 'checked');
                            $("#HeadIsFinalPayC").attr('disabled', 'disabled');
                        }
                        else
                        {
                           
                            $("#trHeadIsFinalPayC").show('slow');
                            $("#HeadIsFinalPayC").attr('checked', false);
                          //  $("#HeadIsFinalPayC").attr('disabled', false);
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
            $("#trHeadIsFinalPayC").hide();                     
        }
    });

    $("#ddlHeadC").change(function () {

       
        if ($(this).val() != "0")
        {
            //added by Koustubh Nakate on 30/09/2013 to prvent same head selection for TEO 
            var headID = $(this).val();
            // $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            //working code but commeneted due to change in requirement

            /*$.blockUI({ message: 'Check for Same Account Head...' });
            $.ajax({
                type: 'GET',
                url: '/TEO/CheckHeadAlreadyExist/',
                async: false,
                cache: false,
                data: { BillID: billId, HeadID: $("#ddlHeadC").val(), CreditDebit:"D" },
                success: function (data) {
                   
                    if (data.exist == true) {
                        
                      
                        $("#ddlHeadC").find("option[value='" + headID + "']").remove();
                        alert('You can not select same account head.');
                        $("#ddlHeadC").val(0);
                      
                        $.unblockUI();
                        return false;
                    }

                    $.unblockUI(); 
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    $.unblockUI();
                }
            })*/
            


         //   alert('a');

            hideAllSelectionC();
            $.ajax({
                url: "/TEO/GetHeadwiseDesignParams/" + billId + "/" + $("#ddlHeadC").val(),
                type: "POST",
                async: false,
                cache: false,
                success: function (data) {

                    conC = "N";
                    aggC = "N";
                    sancyrC = "N";
                    pkgC = "N";
                    roadC = "N";

                    if (data.CON_REQ == "Y") {
                        if ($("#ddlDistrictC").css('display') != "none" && $("#ddlDistrictC").val() != 0)
                        {
                            FillInCascadeDropdown(null, '#ddlHeadContractorC', "/TEO/PopulateContractor/" + $("#ddlDistrictC").val());
                            $("#ddlHeadContractorC").trigger('change');
                        }
                        else
                        {
                            FillInCascadeDropdown(null, '#ddlHeadContractorC', "/TEO/PopulateContractor/");
                        }
                        $("#trddlHeadContractorC").show('slow');
                        conC = "Y";
                    }
                    if (data.AGREEMENT_REQ == "Y") {
                        $("#trddlHeadAgreementC").show('slow');
                        aggC = "Y";
                    }
                    if (data.SANCYEAR_REQ == "Y") {
                        FillInCascadeDropdown(null, '#ddlHeadSancYearC', "/TEO/PopulateSancYear/");
                        $("#trddlHeadSancYearC").show('slow');
                        $("#ddlHeadSancYearC").trigger('change');
                        sancyrC = "Y";
                    }
                    if (data.PKG_REQ == "Y") {
                        
                        $("#trddlHeadPackageC").show('slow');
                        //$("#ddlHeadPackageC").trigger('change');
                        FillInCascadeDropdown(null, '#ddlHeadPackageC', "/TEO/PopulatePackage/" + 0);
                        $("#ddlHeadPackageC").trigger('change');
                        $("#ddlHeadRoadC").empty();
                        $("#ddlHeadRoadC").append("<option value='0'>Select Road</option>");
                        pkgC = "Y";
                    }
                    if (data.ROAD_REQ == "Y")
                    {
                        $("#trddlHeadRoadC").show('slow');
                        //FillInCascadeDropdown(null, '#ddlHeadRoadC', "/TEO/PopulateRoad/0$" + 0);
                        $("#trHeadIsFinalPayC").show();
                        roadC = "Y";
                    } else
                    {
                        if (!$("#ddlRoadC").is(":visible") && !$("#ddlHeadRoadC").is(":visible")) {
                            $("#trHeadIsFinalPayC").hide('slow');
                        }
                        else if ($("#ddlRoadC").is(":visible") || $("#ddlHeadRoadC").is(":visible")) {
                            $("#trHeadIsFinalPayC").show();
                        }
                        else if (!$("#ddlRoadC").is(":visible") || !$("#ddlHeadRoadC").is(":visible")) {
                            $("#trHeadIsFinalPayC").hide('slow');
                        }
                        
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
            $("#ddlHeadContractorC").empty();
            $("#trddlHeadContractorC").css('display', 'none');
            $("#HeadConSupNameC").text("");
            $("#trHeadConSupNameC").hide('slow');
            $("#ddlHeadAgreementC").empty();
            $("#trddlHeadAgreementC").css('display', 'none');
            $("#ddlHeadSancYearC").empty();
            $("#trddlHeadSancYearC").css('display', 'none');
            $("#ddlHeadPackageC").empty();
            $("#trddlHeadPackageC").css('display', 'none');
            $("#ddlHeadRoadC").empty();
            $("#trddlHeadRoadC").css('display', 'none');
            $("#FINAL_PAYMENT").attr('checked', false);
            $("#trHeadIsFinalPayC").css('display', 'none');
           

           
        }
    });

    $("#ddlHeadContractorC").change(function () {
        $("#divTEODetailsErrorC").hide("slide");
        $("#divTEODetailsErrorC span:eq(1)").html('');
        if ($("#ddlHeadContractorC").val() != "" && $("#ddlHeadContractorC").val() != null) {
            $.ajax({
                url: "/Receipt/GetContractorName/" + $("#ddlHeadContractorC").val(),
                type: "POST",
                async: false,
                cache: false,
                success: function (data) {
                    if (aggC == "Y") {
                        if ($("#ddlDistrictC").val() == 0) {
                            $("#divTEODetailsErrorC").show("slide");
                            $("#divTEODetailsErrorC span:eq(1)").html('<strong>Alert: </strong>Please Select District to get Agreement Details');
                            return false;
                        }
                        else {
                            FillInCascadeDropdown(null, '#ddlHeadAgreementC', "/TEO/PopulateAgreement/" + $("#ddlDistrictC").val() + "$" + $("#ddlHeadContractorC").val());

                        }
                    }
                    $("#HeadConSupNameC").text(data);
                    $("#trHeadConSupNameC").show('slow');
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    //alert(xhr.responseText);
                    alert('Error occurred while processing your request');
                }
            });
        }
        else {
            $("#HeadConSupNameC").text("");
            $("#trHeadConSupNameC").hide('slow');
            $("#ddlHeadAgreementC").empty();
            $("#ddlHeadRoadC").empty();
        }

    });

    $("#ddlHeadAgreementC").change(function () {

       
        if (roadC == "Y")
        {
            //FillInCascadeDropdown(null, '#ddlHeadRoadC', "/TEO/PopulateRoad/" + $("#ddlHeadAgreementC option:selected").val() + "$0");
            FillInCascadeDropdown(null, '#ddlHeadRoadC', "/TEO/PopulateRoad/" + $("#ddlHeadAgreementC option:selected").val() + "$" + $("#ddlHeadPackageC option:selected").val() + "$0$" + $("#ddlHeadContractorC option:selected").val() + "?" + $.param({ AGREEMENT_NUMBER: $("#ddlHeadAgreementC option:selected").text() }));
            $("#trHeadIsFinalPayC").show();
        }

        

    });

    $("#ddlHeadSancYearC").change(function () {

        //new change done by Vikram on 24-09-2013

        if ($("#btnEditTEOCreditDetails").is(':visible')) {
            //$("#ddlHeadD").trigger('change');
            pkgC = 'Y';
            roadC = 'Y';
        }
        //end of change

        if (pkgC == "Y") {
            if (roadC == "Y") {
                $("#ddlHeadRoadC").empty();
            }
            FillInCascadeDropdown(null, '#ddlHeadPackageC', "/TEO/PopulatePackage/" + $("#ddlHeadSancYearC").val());
        }
    });

    $("#ddlHeadPackageC").change(function () {

        //new change done by Vikram on 24-09-2013

        if ($("#btnEditTEOCreditDetails").is(':visible')) {
            //$("#ddlHeadD").trigger('change');
            pkgC = 'Y';
            roadC = 'Y';
        }
        //end of change


        if (roadC == "Y") {

            $("#ddlHeadRoadC").empty();
            //if ($("#ddlHeadPackageC").val() > 0) {
            //FillInCascadeDropdown(null, '#ddlHeadRoadC', "/TEO/PopulateRoad/0$" + $("#ddlHeadPackageC").val());
            FillInCascadeDropdown(null, '#ddlHeadRoadC', "/TEO/PopulateRoad/0" + "$" + $("#ddlHeadPackageC option:selected").val() + "$" + $("#ddlHeadSancYearC option:selected").val() +"$" + $("#ddlHeadContractorC option:selected").val() + "$" + $("#ddlHeadC option:selected").val() + "?" + $.param({ AGREEMENT_NUMBER: $("#ddlHeadAgreementC option:selected").text() }));
            //FillInCascadeDropdown(null, '#ddlHeadRoadC', "/TEO/PopulateRoad/0$" + $("#ddlHeadPackageC option:selected").val() + "$" + $("#ddlHeadSancYearC option:selected").val());
            //}
            $("#trHeadIsFinalPayC").show();
        }
    });

    $("#ddlHeadRoadC").change(function () {
        if ($(this).val() != "0") {
            if ($("#ddlHeadContractorC").val() != "0" && $("#ddlHeadContractorC").val() != "" && $("#ddlHeadAgreementC").val() != "0" && $("#ddlHeadAgreementC").val() != "" && $("#ddlHeadRoadC").val != "0" && $("#ddlHeadRoadC").val != "") {
                $.ajax({
                    url: "/TEO/IsFinalPayment/" + $("#ddlHeadContractorC").val() + "$" + $("#ddlHeadAgreementC").val() + "$" + $("#ddlHeadRoadC").val(),
                    type: "POST",
                    async: false,
                    data: { BillID: billId },
                    cache: false,
                    success: function (data) {
                        if (data == "1") {
                            $("#trHeadIsFinalPayC").show();
                            $("#HeadIsFinalPayC").attr('checked', 'checked');
                            $("#HeadIsFinalPayC").attr('disabled', 'disabled');
                        }
                        else {
                            $("#trHeadIsFinalPayC").show();
                            $("#HeadIsFinalPayC").attr('checked', false);
                          //  $("#HeadIsFinalPayC").attr('disabled', false);
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        //alert(xhr.responseText);
                        alert('Error occurred while processing your request');
                        //display the is finalize checkbox
                        $("#trHeadIsFinalPayC").hide('slow');
                    }
                });
            }
        }
        else {
            $("#FINAL_PAYMENT").attr('checked', false);
            $("#trHeadIsFinalPayC").hide();
        }
    });

    $.validator.unobtrusive.parse($('#frmTEOAddDetailsC'));


    $("#btnResetC").click(function (event) {
        
        //event.preventDefault();

        if($("#trddlHeadC").is(':visible'))
        {
            $("#ddlHeadC").val(0);
            $("#ddlHeadC").trigger('change');
        }

        $("#ddlHeadContractorC").parent('td').find('span').html("");
        $("#ddlHeadAgreementC").parent('td').find('span').html("");
        $("#ddlHeadSancYearC").parent('td').find('span').html("");
        $("#ddlHeadPackageC").parent('td').find('span').html("");
        $("#ddlHeadRoadC").parent('td').find('span').html("");
        $("#AMOUNTC").parent('td').find('span').html("");
        $("#ddlDistrictC").parent('td').find('span').html("");
        $("#ddlDPIUC").parent('td').find('span').html("");
        $("#ddlContractorC").parent('td').find('span').html("");
        $("#ddlAgreementC").parent('td').find('span').html("");
        $("#ddlSancYearC").parent('td').find('span').html("");
        $("#ddlPackageC").parent('td').find('span').html("");
        $("#ddlRoadC").parent('td').find('span').html("");
        $("#divTEODetailsErrorC").hide("slide");
        $("#divTEODetailsErrorC span:eq(1)").html("");

        if (!$("#ddlContractorC").is(':disabled')) {
            $('#ConSupNameC').text('');
            $("#trConSupNameC").hide('slow');
            $("#ddlAgreementC").empty();
            $("#ddlRoadC").empty();
            $("#trHeadIsFinalPayC").hide('slow');
            //$("#ddlAgreementC").append("<option value=0>Select Agreement</option>");
            //$("#ddlRoadC").append("<option value=0>Select Road</option>");

        }


        $('#frmTEOAddDetailsC option').each(function () {

            $(this).removeAttr('selected');
            $("#NARRATION").val('');
            $("#AMOUNTC").val('');
        });
        

        LoadTEODetailsGrid($("#tblTEOMasterGrid").getDataIDs()[0]); //new change done by Vikram

    });


    var showConDDl = false;
    //event to save the credit teo details
    $("#btnSaveTEOCreditDetails").click(function (evt) {
        
        evt.preventDefault();
        var formStatus = $('#frmTEOAddDetailsC').valid();
        var status = validateCreditDetails($("#AMOUNTC").val(), amountValC);
        
        var agrmntDisabled = false;
        var contractorDisabled = false;
        if ($("#ddlContractorC").is(':disabled'))
        {
            contractorDisabled = true;
        }

        if ($("#ddlAgreementC").is(':disabled')) {
            agrmntDisabled = true;
        }

        if (formStatus && status)
        {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $("#HeadIsFinalPayC").attr('disabled', false);
            $("#ddlDistrictC").attr('disabled', false);
            $("#ddlDPIUC").attr('disabled', false);
            $("#ddlContractorC").attr('disabled', false);
            $("#ddlHeadContractorC").attr('disabled', false);
            $("#ddlAgreementC").attr('disabled', false);
            $("#ddlHeadAgreementC").attr('disabled', false);
            $("#ddlRoadC").attr('disabled', false);
            $("#ddlHeadRoadC").attr('disabled', false);
            $("#ddlHeadC").attr('disabled', false);

            
            if ($("#trddlContractorC").is(":visible"))
            {
                showConDDl = true;
            }

            $.ajax({
                url: "/TEO/AddCreditTEODetails/" + $("#tblTEOMasterGrid").getDataIDs()[0],
                type: "POST",
                async: false,
                cache: false,
                data: $("#frmTEOAddDetailsC").serialize(),
                success: function (data) {
                    $.unblockUI();
                    if (!data.success || data.success == "undefined")
                    {
                        if (data.message == "undefined" || data.message == null) {
                            
                            if (data.status == -555)
                            {
                                $("#divTEODetailsErrorC").show("slide");
                                $("#divTEODetailsErrorC span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                            }
                            
                            $("#loadTEOCreditDetails").html(data);

                            $("#ddlHeadC").val(0);
                            $.each($("select"), function ()
                            {
                                //new change done by Vikram on 10-10-2013
                                if ($(this).find('option').length > 0) {
                                    if (($(this).attr('id')).slice(-1) == 'D') {
                                    }
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
                                    //contractor based on credit  transaction head
                                    if ($("#ddlHeadContractorC").find('option').length >= 1) {
                                        contID = $("#ddlHeadContractorC").val();
                                    }
                                        //contractor based on master transaction head
                                    else if ($("#ddlContractorC").find('option').length >= 1) {
                                        contID = $("#ddlContractorC").val();
                                    }
                                    //getting contractor name 
                                    $.ajax({
                                        url: "/Receipt/GetContractorName/" + contID,
                                        type: "POST",
                                        async: false,
                                        cache: false,
                                        success: function (data) {
                                            if ($("#ddlHeadContractorC").find('option').length >= 1)
                                            {
                                                $("#HeadConSupNameC").text(data);
                                                $("#trHeadConSupNameC").show('slow');
                                            }
                                            else if ($("#ddlContractorC").find('option').length >= 1)
                                            {
                                                $("#ConSupNameC").text(data);
                                                $("#trConSupNameC").show('slow');
                                            }
                                        },
                                        error: function (xhr, ajaxOptions, thrownError) {
                                            //alert(xhr.responseText);
                                            alert('Error occurred while processing your request');
                                        }
                                    });
                                    if (vpDistRepeat == "Y") {
                                        //$("#ddlDistrictC").attr('disabled', 'disabled');
                                    }
                                    if (vpDPIURepeat == "Y") {
                                        //$("#ddlDPIUC").attr('disabled', 'disabled');
                                    }
                                    if (vpContRepeat == "Y" || vpSupRepeat == "Y") {
                                        $("#ddlContractorC").attr('disabled', 'disabled');
                                        $("#ddlHeadContractorC").attr('disabled', 'disabled');
                                    }
                                    if (vpAggRepeat == "Y") {
                                        $("#ddlAgreementC").attr('disabled', 'disabled');
                                        $("#ddlHeadAgreementC").attr('disabled', 'disabled');
                                    }
                                    if (vpRoadRepeat == "Y") {
                                        $("#ddlRoadC").attr('disabled', 'disabled');
                                        $("#ddlHeadRoadC").attr('disabled', 'disabled');
                                    }
                                    if (vpHeadRepeat == "Y") {
                                        $("#ddlHeadC").attr('disabled', 'disabled');
                                    }
                                },
                                error: function (xhr, ajaxOptions, thrownError) {

                                }
                            });
                            /*******************************************/


                          
                            $("#ddlContractorC").attr('disabled', false);
                           // $("#ddlHeadContractorC").attr('disabled', false);
                            $("#ddlAgreementC").attr('disabled', false);
                           // $("#ddlHeadAgreementC").attr('disabled', false);
                           // $("#ddlRoadC").attr('disabled', false);
                           // $("#ddlHeadRoadC").attr('disabled', false);
                           // $("#ddlHeadC").attr('disabled', false);



                            //$("#ddlHeadC").trigger('change');
                            return false;
                        }
                        else {
                            if (agrmntDisabled == true) {
                                $("#ddlAgreementC").attr('disabled', true);
                            }
                            if (contractorDisabled == true) {
                                $("#ddlContractorC").attr('disabled', true);
                            }
                            $("#HeadIsFinalPayC").attr('disabled', true)
                            $("#divTEODetailsErrorC").show("slide");
                            $("#divTEODetailsErrorC span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                        }
                        return false;
                    }
                    else
                    {
                        $("#divTEODetailsErrorC").hide("slide");

                        $("#divTEODetailsErrorC span:eq(1)").html('');

                        if ($("#trHeadConSupNameC").css('display') != "none") {
                            $("#trHeadConSupNameC").hide('slow');
                            $("#HeadConSupNameC").text("");
                           
                        }
                        $("#btnResetC").trigger('click');
                        //$("#ddlHeadC").trigger('change');

                        //  alert("isMulTxn" + isMulTxn);



                        LoadTEODetailsGrid($("#tblTEOMasterGrid").getDataIDs()[0]);

                        alert("Credit Details Added");
                        if (showConDDl == true)
                        {
                            
                            $("#trddlContractorC").show();
                        }
                        else
                        {
                            $("#trddlContractorC").hide();
                        }

                        //$("#btnResetC").trigger('click');
                        
                        //Added By Abhishek kamble to Hide Cform start
                        if (_fundType == "M" && isMulTxn == "N") {
                            $("#loadTEOCreditDetails").hide();
                        }
                        //Added By Abhishek kamble to Hide Cform end


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

    $("#btnCancelTEOCreditDetails").click(function () {

        if ($("#divFinalizeTEO").css('display') != "none") {
            $("#loadTEOCreditDetails").html('');
            $("#loadTEODebitDetails").html('');
        }
        else {          
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/TEO/TEODetails/" + $("#tblTEOMasterGrid").getDataIDs()[0] + "/C",
                type: "POST",
                async: false,
                cache: false,
                success: function (data) {
                    $.unblockUI();

                    $("#loadTEOCreditDetails").html(data);
                    $.each($("select"), function () {
                        if ($(this).find('option').length >= 1) {
                            $('#tr' + $(this).attr('id')).show();
                        }
                    });

                    //new change done by Vikram
                   // return false;
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
                                if ($("#ddlContractorC").val() > 0) {
                                    $("#ddlContractorC").attr('disabled', 'disabled');
                                }
                                if ($("#ddlHeadContractorC").val() > 0) {
                                    $("#ddlHeadContractorC").attr('disabled', 'disabled');
                                }
                            }
                            if (vpAggRepeat == "Y") {

                                if ($("#ddlAgreementC").val() > 0) {
                                    $("#ddlAgreementC").attr('disabled', 'disabled');
                                }
                                if ($("#ddlHeadAgreementC").val() > 0) {
                                    $("#ddlHeadAgreementC").attr('disabled', 'disabled');
                                }
                            }
                            if (vpRoadRepeat == "Y") {
                                if ($("#ddlRoadC").val() > 0) {
                                    $("#ddlRoadC").attr('disabled', 'disabled');
                                }
                                if ($("#ddlHeadRoadC").val() > 0) {
                                    $("#ddlHeadRoadC").attr('disabled', 'disabled');
                                }
                            }


                            var contID = 0;

                            if ($("#ddlHeadContractorC").find('option').length >= 1) {
                                contID = $("#ddlHeadContractorC").val();
                            }
                            else if ($("#ddlContractorC").find('option').length >= 1) {
                                contID = $("#ddlContractorC").val();
                            }

                            $.ajax({
                                url: "/Receipt/GetContractorName/" + contID,
                                type: "POST",
                                async: false,
                                cache: false,
                                success: function (data) {
                                    if ($("#ddlHeadContractorC").find('option').length >= 1) {
                                        $("#HeadConSupNameC").text(data);
                                        $("#trHeadConSupNameC").show('slow');
                                    }
                                    else if ($("#ddlContractorC").find('option').length >= 1) {
                                        $("#ConSupNameC").text(data);
                                        $("#trConSupNameC").show('slow');
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

                    //end of change

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
        //LoadTEODetailsGrid($("#tblTEOMasterGrid").getDataIDs()[0]); //new change done by Vikram

    });

    //event for editing details
    $("#btnEditTEOCreditDetails").click(function (evt) {

        evt.preventDefault();

        if (transactionIdC != 0 && transactionIdC != "")
        {
            transId = transactionIdC;
        }

        var formStatus = $('#frmTEOAddDetailsC').valid();
        var status = validateCreditDetails($("#AMOUNTC").val(), amountValC_Credit);
        if (formStatus && status)
        {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $("#HeadIsFinalPayC").attr('disabled', false);
            $("#ddlDistrictC").attr('disabled', false);
            $("#ddlDPIUC").attr('disabled', false);
            $("#ddlContractorC").attr('disabled', false);
            $("#ddlHeadContractorC").attr('disabled', false);
            $("#ddlAgreementC").attr('disabled', false);
            $("#ddlHeadAgreementC").attr('disabled', false);
            $("#ddlRoadC").attr('disabled', false);
            $("#ddlHeadRoadC").attr('disabled', false);
            $("#ddlHeadC").attr('disabled', false);
            $.ajax({
                url: "/TEO/EditCreditTEODetails/" + transId,
                type: "POST",
                async: false,
                cache: false,
                data: $("#frmTEOAddDetailsC").serialize(),
                success: function (data) {
                    $.unblockUI();

                    if (!data.success || data.success == "undefined") {
                        
                        //new change done by Vikram on 16-10-2013
                        if (data.status == -555) {
                            $("#divTEODetailsErrorC").show("slide");
                            $("#divTEODetailsErrorC span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                        }
                        //end of change
                        

                        if (data.message == "undefined" || data.message == null) {
                            $("#loadTEOCreditDetails").html(data);
                            $("#ddlHeadC").val(0);
                            $.each($("select"), function () {
                                if ($(this).find('option').length > 0) {
                                    $('#tr' + $(this).attr('id')).show();
                                }
                            });
                            var userdata = $("#tblTEODetailsGrid").getGridParam('userData');
                            if (userdata.multipleTrans == 'N')
                            {
                                if($("#ddlDistrictC").is(':visible'))
                                {
                                    $("#ddlDistrictC").attr('disabled',true);
                                }

                                if ($("#ddlDPIUC").is(':visible')) {
                                    $("#ddlDPIUC").attr('disabled', true);
                                }
                                                                
                                //Added By Abhishek kamble 21Nov2014 to disable ddl for MF start
                                if (_fundType == "M") {
                                    $("#HeadIsFinalPayC").attr('disabled', true);
                                    $("#ddlDistrictC").attr('disabled', true);
                                    $("#ddlDPIUC").attr('disabled', true);
                                    $("#ddlContractorC").attr('disabled', true);
                                    $("#ddlHeadContractorC").attr('disabled', true);
                                    $("#ddlAgreementC").attr('disabled', true);
                                    $("#ddlHeadAgreementC").attr('disabled', true);
                                    $("#ddlRoadC").attr('disabled', true);
                                    $("#ddlHeadRoadC").attr('disabled', true);
                                    $("#ddlHeadC").attr('disabled', true);
                                }
                                //Added By Abhishek kamble 21Nov2014 to disable ddl for MF end 
                            }

                            $("#ddlHeadC").trigger('change');
                            return false;
                        }
                        else {
                            
                            $("#divTEODetailsErrorC").show("slide");
                            $("#divTEODetailsErrorC span:eq(1)").html('<strong>Alert: </strong>' + data.message);

                        }
                        return false;
                    }
                    else {
                        $("#divTEODetailsErrorC").hide("slide");
                        $("#divTEODetailsErrorC span:eq(1)").html('');
                        if ($("#trHeadConSupNameC").css('display') != "none") {
                            $("#trHeadConSupNameC").hide('slow');
                            $("#HeadConSupNameC").text("");
                        }
                        $("#btnCancelTEOCreditDetails").trigger('click');
                        LoadTEODetailsGrid($("#tblTEOMasterGrid").getDataIDs()[0]);
                        alert("Credit Details Updated");
                        return false;
                    }

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    $.unblockUI();

                    alert('Error occurred while processing your request');
                    //alert(xhr.responseText);
                }
            });
        }
     
    });

    if (transIdC != 0 && transIdC != "")
    {
        transactionIdC = transId;
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

function hideAllSelectionC() {
    $("#trddlHeadContractorC").hide('slow');
    $("#trHeadConSupNameC").hide('slow');
    $("#HeadConSupNameC").text("");
    $("#trddlHeadAgreementC").hide('slow');
    $("#trddlHeadSancYearC").hide('slow');
    $("#trddlHeadPackageC").hide('slow');
    $("#trddlHeadRoadC").hide('slow');
    $("#divTEODetailsErrorC").hide("slide");
    $("#divTEODetailsErrorC span:eq(1)").html('');
    $("#trHeadIsFinalPayC").hide();
}

function validateCreditDetails(amountToValidate, oldValue) {
    $("#divTEODetailsErrorC").hide("slide");
    $("#divTEODetailsErrorC span:eq(1)").html('');
    var CrDrDetails = $("#tblTEODetailsGrid").jqGrid('getCol', 'CreditDebit', false);
    var CrCount = 0;
    if (CrDrDetails.length != 0) {
        for (var i = 0; i < CrDrDetails.length; i++) {
            if (CrDrDetails[i] == "Credit") {
                CrCount = CrCount + 1;
            }
        }       
    }

   /* if (isMulTxn == "N" && CrCount == 1)
    {
        $("#divTEODetailsErrorC").show("slide");
        $("#divTEODetailsErrorC span:eq(1)").html('<strong>Alert: </strong>Only single transaction entry allowed');
        return false;
    }
    else
    {
        $("#divTEODetailsErrorC").hide("slide");
        $("#divTEODetailsErrorC span:eq(1)").html('');
    }*/
    
    var masterAmount = $("#tblTEOMasterGrid").jqGrid('getCell', $("#tblTEOMasterGrid").getDataIDs()[0], 'GrossAmount');
    var detailsAmount = 0;
    if (isDetailsGridLoaded) {

        detailsAmount = parseFloat(($("#tblTEODetailsGrid").jqGrid('getCol', 'CAmount', false, "sum") - oldValue)) + parseFloat(amountToValidate);
        //added by abhishek kamble 21-10-2013
        //if (oldValue == 0)
        //{
        //    detailsAmount = amountToValidate;
        //}
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
            if (data.DIST_REQ == "Y" && $("#ddlDistrictC").val() == "0") {
                $("#ddlDistrictC").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlDistrictC">District Required</span>').show();
                $("#ddlDistrictC").addClass("input-validation-error");
                statusFlag = false;
            }
            if (data.DPIU_REQ == "Y" && $("#ddlDPIUC").val() == "0") {
                $("#ddlDPIUC").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlDPIUC">DPIU Required</span>').show();
                $("#ddlDPIUC").addClass("input-validation-error");
                statusFlag = false;
            }
            if (data.CON_REQ == "Y" && $("#ddlContractorC").val() == "0") {
                $("#ddlContractorC").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlContractorC">Contractor Required</span>').show();
                $("#ddlContractorC").addClass("input-validation-error");
                statusFlag = false;
            }
            if (data.SUP_REQ == "Y" && $("#ddlContractorC").val() == "0") {
                $("#ddlContractorC").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlContractorC">Supplier Required</span>').show();
                $("#ddlContractorC").addClass("input-validation-error");
                statusFlag = false;
            }
            if (data.AGREEMENT_REQ == "Y" && $("#ddlAgreementC").val() == "0") {
                $("#ddlAgreementC").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlAgreementC">Agreement Required</span>').show();
                $("#ddlAgreementC").addClass("input-validation-error");
                statusFlag = false;
            }
            if (data.SANCYEAR_REQ == "Y" && $("#ddlSancYearC").val() == "0") {
                $("#ddlSancYearC").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlSancYearC">Sanction Year Required</span>').show();
                $("#ddlSancYearC").addClass("input-validation-error");
                statusFlag = false;
            }
            if (data.PKG_REQ == "Y" && $("#ddlPackageC").val() == "0") {
                $("#ddlPackageC").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlPackageC">Package Required</span>').show();
                $("#ddlPackageC").addClass("input-validation-error");
                statusFlag = false;
            }
            else
            {
                $("#ddlPackageC").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlPackageC">Package Required</span>').hide();
            }
            if (data.ROAD_REQ == "Y" && $("#ddlRoadC").val() == "0") {
                $("#ddlRoadC").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlRoadC">Road Required</span>').show();
                $("#ddlRoadC").addClass("input-validation-error");
                statusFlag = false;
            }
            

            if (parseFloat(masterAmount) < parseFloat(detailsAmount)) {
                //$("#divTEODetailsErrorC").show("slide");
                //$("#divTEODetailsErrorC span:eq(1)").html('<strong>Alert: </strong> Invalid Amount, Amount exceeds master amount');
                $("#AMOUNTC").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="AMOUNTC">Total Details Amount Exceeds Master Amount</span>').show();
                $("#AMOUNTC").addClass("input-validation-error");
                statusFlag = false;
            }

            // alert(isMulTxn +" "+ parseFloat(masterAmount) +" "+parseFloat(detailsAmount));
            //added by amol jadhav //if multiple transaction is not allowed details amount must be equal to 
            //if (isMulTxn == "N" && (parseFloat(masterAmount) != parseFloat(detailsAmount)))
            //{
            //    $("#AMOUNTC").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="AMOUNTC">Details Amount should be equal to master amount as multiple transaction is not allowed</span>').show();
            //    $("#AMOUNTC").addClass("input-validation-error");
            //    statusFlag = false;

            //}
        },
        error: function (xhr, ajaxOptions, thrownError) {
            //alert(xhr.responseText);
            alert('Error occurred while processing your request');
        }
    });

    if (conC == "Y" && $("#ddlHeadContractorC").val() == "0") {
        $("#ddlHeadContractorC").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlHeadContractorC">Contractor Required</span>').show();
        $("#ddlHeadContractorC").addClass("input-validation-error");
        statusFlag = false;
    }
    if (aggC == "Y" && $("#ddlHeadAgreementC").val() == "0") {
        $("#ddlHeadAgreementC").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlHeadAgreementC">Agreement Required</span>').show();
        $("#ddlHeadAgreementC").addClass("input-validation-error");
        statusFlag = false;
    }
    if (sancyrC == "Y" && $("#ddlHeadSancYearC").val() == "0") {
        $("#ddlHeadSancYearC").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlHeadSancYearC">Sanction Year Required</span>').show();
        $("#ddlHeadSancYearC").addClass("input-validation-error");
        statusFlag = false;
    }
    if (pkgC == "Y" && $("#ddlHeadPackageC").val() == "0") {
        $("#ddlHeadPackageC").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlHeadPackageC">Package Required</span>').show();
        $("#ddlHeadPackageC").addClass("input-validation-error");
        statusFlag = false;
    }
    if (roadC == "Y" && $("#ddlHeadRoadC").val() == "0") {
        $("#ddlHeadRoadC").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlHeadRoadC">Road Required</span>').show();
        $("#ddlHeadRoadC").addClass("input-validation-error");
        statusFlag = false;
    }

    return statusFlag;
}


