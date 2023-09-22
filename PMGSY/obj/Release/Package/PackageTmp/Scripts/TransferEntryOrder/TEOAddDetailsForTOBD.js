var conD = "N";
var aggD = "N";
var sancyrD = "N";
var pkgD = "N";
var roadD = "N";

$(document).ready(function () {


    $.unblockUI();

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
                      //  $("#HeadIsFinalPayD").attr('disabled', false);
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
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
                    alert(xhr.responseText);
                }
            });
        }
    }

    
    var txnID = $('#parentTxnId').val();

    var recCount = jQuery("#tblTEODetailsGrid").getGridParam('reccount');

  
    //modified by abhishek kamble 17Mar2015 
    //if ((txnID == 164 || txnID == 165 || txnID == 1194 || txnID == 1195) && $("#ddlDistrictC").val() == 0 && $("#ddlDistrictD").val() == 0 && recCount==0) {
    if ((txnID == 164 || txnID == 1195) && $("#ddlDistrictC").val() == 0 && $("#ddlDistrictD").val() == 0 && recCount==0) {
        $('#frmTEOAddDetailsForTOB').find("select,input,textarea,button").attr('disabled', true);
        $('#trddlDistrictC').find("select").attr('disabled', false);

    }




    //Added By Abhishek kamble to populate District using state selection drop down   1 Oct 2014 start

    $("#ddlStateD").change(function () {

        var StateCodeD = $("#ddlStateD").val();

        $.ajax({

            type: "POST",
            url: "/TEO/PopulateDistricts/" + StateCodeD,
            datatype: 'json',
            async: false,
            cache: false,
            success: function (data) {
                $("#ddlDistrictD").empty();
                
                $.each(data, function () {


                    $("#ddlDistrictD").append("<option value=" + this.Value + ">" + this.Text + "</option>");
                    //if (this.Value == DPIUC) {

                    //    $("#ddlDPIUC").append("<option value=" + this.Value + " selected=true>" + this.Text + "</option>");
                    //}
                    //else {
                    //    $("#ddlDPIUC").append("<option value=" + this.Value + ">" + this.Text + "</option>");
                    //}
                });
            },
            error: function (xhr, ajaxOptions, thrownError) {

            }

        });

    });

    //Added By Abhishek kamble to populate District drop down 1 Oct 2014 end


    $("#ddlDistrictD").change(function () {

        if ($("#divTEODetailsErrorForTOB").is(':visible')) {
            $("#divTEODetailsErrorForTOB").hide("slide");
            $("#divTEODetailsErrorForTOB span:eq(1)").html('');
        }

        var districtCode = $("#ddlDistrictD").val();

        //added by Koustubh Nakate on 17/10/2013 top check district is shifted or not
        var districtCodeC=$("#ddlDistrictC").val();
        var txnId=$('#parentTxnId').val();


        if (vpDistRepeat == 'N' && $("#ddlDistrictC").val() != 0 && districtCode == $("#ddlDistrictC").val() && (txnId != 1664 && txnId != 1665))//(txnId != 1664 && txnId != 1665) added by Abhishek kamble 6Jan2016
        {
            //            alert("test : "+txnId);

            $("#ddlDistrictD").val(0);
            alert('Please select different district.');
            //old
            //FillInCascadeDropdown(null, '#ddlDPIUD', "/TEO/PopulateDPIU/" + 0);

            FillInCascadeDropdown(null, '#ddlDPIUD', "/TEO/PopulateDPIU/" + (isStateWiseBalanceTransfer?0 + "$N":0));

            return;
        }
        //Modified by Abhishek kamble 17Mar2015
        //if (districtCodeC > 0 && districtCode > 0 && (txnId == 164 || txnId == 165 || txnId == 1195 || txnId == 1194) && districtCodeC != districtCode) {
        if (districtCodeC > 0 && districtCode > 0 && (txnId == 164 || txnId == 1195 ) && districtCodeC != districtCode) {
        // alert("test2");

            $.ajax({
                type: 'GET',
                url: '/TEO/CheckIsDistrictShifted',
                async: false,
                cache: false,
                data: { DistrictC: districtCodeC , DistrictD:districtCode},
                success: function (data) {

                    if (data.exist == true) {

                        //if ($('#ddlHeadD').val() == 0) {
                           
                        //}
                        //else {

                        //}

                        $('#frmTEOAddDetailsForTOB').find("select,input,textarea,button").not("#HeadIsFinalPayC,#HeadIsFinalPayD ").attr('disabled', false);

                        //Added By Abhishek kamble disabled ddl start

                        $("#ddlStateC").attr("disabled","disabled");
                        $("#ddlStateD").attr("disabled", "disabled");
                        //Added By Abhishek kamble disabled ddl end

                    }
                    else {
                        $("#ddlDistrictD").val(0);
                        alert("District is not shifted.Please select other district.");
                        $('#frmTEOAddDetailsForTOB').find("select,input,textarea,button").attr('disabled', true);
                        $('#trddlDistrictC').find("select").attr('disabled', false);
                        $('#trddlDistrictD').find("select").attr('disabled', false);
                        $('#ddlHeadC').val(0);
                        $('#ddlHeadD').val(0);
                        $('#ddlHeadC').trigger('change');
                        $('#ddlHeadD').trigger('change');

                        return;
                    }


                },
                error: function (xhr, ajaxOptions, thrownError) {

                }
            })

        }
        else if (districtCode == 0 && (txnId == 164 || txnId == 165 || txnId == 1195 || txnId == 1194)) {
            $('#frmTEOAddDetailsForTOB').find("select,input,textarea,button").attr('disabled', true);
            $('#trddlDistrictC').find("select").attr('disabled', false);
            $('#trddlDistrictD').find("select").attr('disabled', false);
            return;

        }


       

        if ($("#ddlContractorD").css('display') != "none") {
            FillInCascadeDropdown(null, '#ddlContractorD', "/TEO/PopulateContractor/" + $("#ddlDistrictD").val());
            $("#ddlContractorD").trigger('change');

            FillInCascadeDropdown(null, '#ddlHeadContractorD', "/TEO/PopulateContractor/" + $("#ddlDistrictD").val());
           // $("#ddlHeadContractorD").trigger('change');

        }
        if ($("#trddlDPIUD").css('display') != "none") {
            //old
            //FillInCascadeDropdown(null, '#ddlDPIUD', "/TEO/PopulateDPIU/" + $("#ddlDistrictD").val());
            FillInCascadeDropdown(null, '#ddlDPIUD', "/TEO/PopulateDPIU/" + (isStateWiseBalanceTransfer ? $("#ddlDistrictD").val() + "$N" : $("#ddlDistrictD").val()));

        }


       

        //if (vpDistRepeat == 'Y') {
        //    $("#ddlDistrictC").val(districtCode);
        //    $("#ddlDistrictC").attr('disabled', true);
        //    $("#ddlDistrictC").trigger('change');
        //}
        //else {
        //    $("#ddlDistrictC").attr('disabled', false);
        //}
    });

    if ($("#ddlDistrictD").val() == 0) {
        $("#ddlDPIUD").empty();
        $("#ddlDPIUD").append("<option value='0' >Select DPIU </option>");
    }

    $("#ddlDPIUD").change(function () {

        var DPIU = $("#ddlDPIUD").val();


        //if (vpDPIURepeat == 'Y') {
        //    $("#ddlDPIUC").val(DPIU);
        //    $("#ddlDPIUC").attr('disabled', true);
        //}
        //else {
        //    $("#ddlDPIUC").attr('disabled', false);
        //}

        if (DPIU > 0 && vpDPIURepeat == 'N' && DPIU == $("#ddlDPIUC").val() && (txnId != 1664 && txnId != 1665))//Added by Abhishek kamble 6Jan2016 for head20.01
        {

            alert('Please select different PIU Name.');

            $("#ddlDPIUD").val(0);

        }

    });


    $("#ddlContractorD").change(function () {
        $("#divTEODetailsErrorD").hide("slide");
        $("#divTEODetailsErrorD span:eq(1)").html('');
        if ($("#ddlContractorD").val() != "" && $("#ddlContractorD").val() != null) {
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
                            if ($("#trddlDistrictD").css('display') != "none") {
                                districtCode = $("#ddlDistrictD").val();
                            }
                            $('#ddlAgreementD').empty();
                           
                            FillInCascadeDropdown(null, '#ddlAgreementD', "/TEO/PopulateAgreement/" + districtCode + "$" + $("#ddlContractorD").val());

                        }
                    }
                    $("#ConSupNameD").text(data);
                    $("#trConSupNameD").show('slow');

                    //if (vpContRepeat == 'Y' || vpSupRepeat == "Y") {
                    //    $("#ddlContractorC").val($("#ddlContractorD").val());
                    //    $("#ddlContractorC").attr('disabled', true);
                    //    $("#ddlContractorC").trigger('change');
                    //}
                    //else {
                    //    $("#ddlContractorC").attr('disabled', false);

                    //}
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                }
            });
        }
        else {
            $("#ConSupNameD").text("");
            $("#trConSupNameD").hide('slow');
            $("#ddlAgreementD").empty();
            $("#ddlRoadD").empty();
        }

    });

    $("#ddlSancYearD").change(function () {
        if ($("#trddlPackageD").css('display') != "none") {
            if ($("#ddlRoadD").css('display') != "none") {
                $("#ddlRoadD").empty();
            }
            FillInCascadeDropdown(null, '#ddlPackageD', "/TEO/PopulatePackage/" + $("#ddlSancYearD").val());
        }
    });

    $("#ddlPackageD").change(function () {
        if ($("#trddlRoadD").css('display') != "none") {
            
            //FillInCascadeDropdown(null, '#ddlRoadD', "/TEO/PopulateRoad/0$" + $("#ddlPackageD").val());
            FillInCascadeDropdown(null, '#ddlRoadD', "/TEO/PopulateRoad/0$" + $("#ddlPackageD option:selected").val() + "$0$" + $("#ddlContractorD option:selected").val() + "?" + $.param({ AGREEMENT_NUMBER: $("#ddlAgreementD option:selected").text() }));
            // $("#trHeadIsFinalPayD").show();
        }
    });

    $("#ddlAgreementD").change(function () {
        if ($("#trddlRoadD").is(":visible")) {
            
            //FillInCascadeDropdown(null, '#ddlRoadD', "/TEO/PopulateRoad/" + $("#ddlAgreementD").val() + "$0");
            FillInCascadeDropdown(null, '#ddlRoadD', "/TEO/PopulateRoad/" + $("#ddlAgreementD option:selected").val() + "$0$0$" + $("#ddlContractorD option:selected").val() + "?" + $.param({ AGREEMENT_NUMBER: $("#ddlAgreementD option:selected").text() }));
            $("#trHeadIsFinalPayD").show();
        }

       // alert(vpAggRepeat);

        if ($("#ddlAgreementD").val() > 0 && vpAggRepeat == 'N' && $("#ddlAgreementD").val() == $("#ddlAgreementC").val()) {

            alert('Please select different agreement.');

            $("#ddlAgreementD").val(0);

        }

        //if (vpAggRepeat == "Y") {
        //    $("#ddlAgreementC").val($("#ddlAgreementD").val());
        //    $("#ddlAgreementC").attr('disabled', true);
        //    $("#ddlAgreementC").trigger('change');

        //}
        //else {
        //    $("#ddlAgreementC").attr('disabled', false);

        //}
    });

    $("#ddlRoadD").change(function () {
        if ($(this).val() != "0") {
            if ($("#ddlContractorD").val() != "0" && $("#ddlContractorD").val() != "" && $("#ddlContractorD").val() != null && $("#ddlAgreementD").val() != "0" && $("#ddlAgreementD").val() != "" && $("#ddlAgreementD").val() != null && $("#ddlRoadD").val != "0" && $("#ddlRoadD").val != "" && $("#ddlRoadD").val != null) {
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
                          //  $("#HeadIsFinalPayD").attr('disabled', false);
                        }

                        //if (vpRoadRepeat == "Y") {
                        //    $("#ddlRoadC").val($("#ddlRoadD").val());
                        //    $("#ddlRoadC").attr('disabled', true);
                        //    $("#ddlRoadC").trigger('change');
                        //}
                        //else {
                        //    $("#ddlRoadC").attr('disabled', false);

                        //}
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.responseText);
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
                        $("#trddlHeadSancYearD").show('slow');
                        sancyrD = "Y";
                    }
                    if (data.PKG_REQ == "Y") {
                        $("#trddlHeadPackageD").show('slow');
                        pkgD = "Y";
                    }
                    if (data.ROAD_REQ == "Y") {
                        $("#trddlHeadRoadD").show('slow');
                        $("#trHeadIsFinalPayD").show();
                        roadD = "Y";
                        if ($("#ddlContractorD").css('display') != "none") {
                            if ($("#ddlAgreementD").css('display') != "none") {
                                FillInCascadeDropdown(null, '#ddlHeadRoadD', "/TEO/PopulateRoad/" + $("#ddlAgreementD").val() + "$0" + "$0$" + $("#ddlContractorD option:selected").val() + "?" + $.param({ AGREEMENT_NUMBER: $("#ddlAgreementD option:selected").text() }));
                            }
                        }
                        else {
                            if ($("#ddlHeadPackageD").css('display') != "none") {
                                FillInCascadeDropdown(null, '#ddlHeadRoadD', "/TEO/PopulateRoad/0" + "$" + $("#ddlHeadPackageD option:selected").val() + "$0$" + $("#ddlHeadContractorD option:selected").val() + "?" + $.param({ AGREEMENT_NUMBER: $("#ddlHeadAgreementD option:selected").text() }));
                            }

                        }
                        
                    }
                    else {
                        if (!$("#ddlRoadD").is(":visible") && !$("#ddlHeadRoadD").is(":visible")) {
                            $("#trHeadIsFinalPayD").hide('slow');
                        }
                        else {
                            $("#trHeadIsFinalPayD").show();
                        }
                    }


                    //if (vpHeadRepeat == 'Y') {
                    //    $("#ddlHeadC").val($("#ddlHeadD").val());
                    //    $("#ddlHeadC").attr('disabled', true);
                    //    $("#ddlHeadC").trigger('change');
                    //}
                    //else {
                    //    $("#ddlHeadC").attr('disabled', false);
                    //}

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
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

        $("#divTEODetailsErrorForTOB").hide("slide");
        $("#divTEODetailsErrorForTOB span:eq(1)").html('');

        if ($("#ddlHeadContractorD").val() != 0 ) {
            $.ajax({
                url: "/Receipt/GetContractorName/" + $("#ddlHeadContractorD").val(),
                type: "POST",
                async: false,
                cache: false,
                success: function (data) {
                    //  alert(aggD);

                    if ($('#ddlHeadAgreementD').is(":visible") && aggD == "N") {
                        aggD = "Y";
                    }

                    if (aggD == "Y") {
                        if ($("#ddlDistrictD").val() == 0) {
                            $("#divTEODetailsErrorForTOB").show("slide");
                            $("#divTEODetailsErrorForTOB span:eq(1)").html('<strong>Alert: </strong>Please Select Debit District to get Agreement Details');

                            $("#ddlHeadContractorD").val(0);
                            return false;
                        }
                        else {
                            $("#ddlHeadAgreementD").empty();
                            
                            FillInCascadeDropdown(null, '#ddlHeadAgreementD', "/TEO/PopulateAgreement/" + $("#ddlDistrictD").val() + "$" + $("#ddlHeadContractorD").val());

                        }
                    }
                    $("#HeadConSupNameD").text(data);
                    $("#trHeadConSupNameD").show('slow');
                    

                    //if (vpContRepeat == 'Y' || vpSupRepeat == "Y") {

                    //    $("#ddlHeadContractorC").val($("#ddlHeadContractorD").val());
                    //    $("#ddlHeadContractorC").attr('disabled', true);
                    //    $("#ddlHeadContractorC").trigger('change');
                    //}
                    //else {
                    //    $("#ddlHeadContractorC").attr('disabled', false);
                    //}

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
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

        if ($('#ddlHeadRoadD').is(":visible") && roadD == "N") {
            roadD = "Y";
        }

        if (roadD == "Y") {
            
            //SancYearD parameter added and checked for undefined and null.ROads not populate because "year"  parameters not pass curreclty as "$" seperated for function PopulateRoad in file TEOAddDetailsForTOB.js now parameter is correctly pass.
            //Modified by Abhishek 13Aug2014
            var SancYearD = $("#ddlHeadSancYearD").val();
            if ($("#ddlHeadSancYearD").val() === "undefined" || $("#ddlHeadSancYearD").val()==null)
            {
                SancYearD = 0;
            }

            //FillInCascadeDropdown(null, '#ddlHeadRoadD', "/TEO/PopulateRoad/" + $("#ddlHeadAgreementD").val() + "$0");
            FillInCascadeDropdown(null, '#ddlHeadRoadD', "/TEO/PopulateRoad/" + $("#ddlHeadAgreementD option:selected").val() + "$0$" + SancYearD + "$" + $("#ddlHeadContractorD option:selected").val() + "?" + $.param({ AGREEMENT_NUMBER: $("#ddlHeadAgreementD option:selected").text() }));
            $("#trHeadIsFinalPayD").show();
        }

        //if (vpAggRepeat == "Y") {

        //    $("#ddlHeadAgreementC").val($("#ddlHeadAgreementD").val());
        //    $("#ddlHeadAgreementC").attr('disabled', true);
        //    $("#ddlHeadAgreementC").trigger('change');
        //}
        //else {

        //    $("#ddlHeadAgreementC").attr('disabled', false);
        //}

    });

    $("#ddlHeadSancYearD").change(function () {
        if (pkgD == "Y") {
            if (roadD == "Y") {
                $("#ddlHeadRoadD").empty();
            }
            //FillInCascadeDropdown(null, '#ddlHeadPackageD', "/TEO/PopulatePackage/" + $("#ddlHeadSancYearD").val());
            FillInCascadeDropdown(null, '#ddlHeadPackageD', "/TEO/PopulatePackage/" + $("#ddlHeadSancYearD").val() + "$" + $("#ddlDistrictD").val());
        }
    });

    $("#ddlHeadPackageD").change(function () {
        if (roadD == "Y") {           
            //alert('b');
            //FillInCascadeDropdown(null, '#ddlHeadRoadD', "/TEO/PopulateRoad/0$" + $("#ddlHeadPackageD").val());
            FillInCascadeDropdown(null, '#ddlHeadRoadD', "/TEO/PopulateRoad/0" + "$" + $("#ddlHeadPackageD option:selected").val() + "$" + $("#ddlHeadSancYearD option:selected").val() + "$" +$("#ddlHeadContractorD option:selected").val() + "?" + $.param({ AGREEMENT_NUMBER: $("#ddlHeadAgreementD option:selected").text() }));
          //  FillInCascadeDropdown(null, '#ddlHeadRoadD', "/TEO/PopulateRoad/0" + "$" + $("#ddlHeadPackageD option:selected").val() + "$" + $("#ddlHeadSancYearD option:selected").val() + "$" + $("#ddlHeadContractorD option:selected").val() + "$" + $.param({ AGREEMENT_NUMBER: $("#ddlHeadAgreementD option:selected").text() }));
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
                          //  $("#HeadIsFinalPayD").attr('disabled', false);
                        }

                        //if (vpRoadRepeat == "Y") {

                        //    $("#ddlHeadRoadC").val($("#ddlHeadRoadD").val());
                        //    $("#ddlHeadRoadC").attr('disabled', true);
                        //    $("#ddlHeadRoadC").trigger('change');
                        //}
                        //else {

                        //    $("#ddlHeadRoadC").attr('disabled', false);
                        //}
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.responseText);
                    }
                });
            }
        }
        else {
            $("#FINAL_PAYMENT").attr('checked', false);
            $("#trHeadIsFinalPayD").hide();
        }
    });

   
    //$("#btnResetD").click(function () {
    //    $("#ddlHeadContractorD").parent('td').find('span').html("");
    //    $("#ddlHeadAgreementD").parent('td').find('span').html("");
    //    $("#ddlHeadSancYearD").parent('td').find('span').html("");
    //    $("#ddlHeadPackageD").parent('td').find('span').html("");
    //    $("#ddlHeadRoadD").parent('td').find('span').html("");
    //    $("#AMOUNTD").parent('td').find('span').html("");
    //    $("#ddlDistrictD").parent('td').find('span').html("");
    //    $("#ddlDPIUD").parent('td').find('span').html("");
    //    $("#ddlContractorD").parent('td').find('span').html("");
    //    $("#ddlAgreementD").parent('td').find('span').html("");
    //    $("#ddlSancYearD").parent('td').find('span').html("");
    //    $("#ddlPackageD").parent('td').find('span').html("");
    //    $("#ddlRoadD").parent('td').find('span').html("");
    //    $("#divTEODetailsErrorD").hide("slide");
    //    $("#divTEODetailsErrorD span:eq(1)").html("");
    //});

    
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
    $("#divTEODetailsErrorForTOB").hide("slide");
    $("#divTEODetailsErrorForTOB span:eq(1)").html('');

    $("#trHeadIsFinalPayD").hide('slow');
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
                $("#ddlContractorD").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlContractorD">Company Name is Required</span>').show();
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


            if (parseFloat(masterAmount) < parseFloat(detailsAmount)) {
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
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
        }
    });

    if (conD == "Y" && $("#ddlHeadContractorD").val() == "0") {
        $("#ddlHeadContractorD").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlHeadContractorD">Company Name is Required</span>').show();
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

















