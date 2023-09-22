var conC = "N";
var aggC = "N";
var sancyrC = "N";
var pkgC = "N";
var roadC = "N";
var isSaved = false;
var isValidCredit = true;
var isValidDebit = true;
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

    if ($("#ddlContractorC").find('option').length != 0 && $("#ddlAgreementC").find('option').length != 0 && $("#ddlRoadC").find('option').length != 0) {
        if (($("#ddlContractorC").val() != 0 && $("#ddlContractorC").val() != "" && $("#ddlContractorC").val() != null) && ($("#ddlAgreementC").val() != "0" && $("#ddlAgreementC").val() != "" && $("#ddlAgreementC").val() != null) && ($("#ddlRoadC").val() != "0" && $("#ddlRoadC").val() != "" && $("#ddlRoadC").val() != null)) {
            $.ajax({
                url: "/TEO/IsFinalPayment/" + $("#ddlContractorC").val() + "$" + $("#ddlAgreementC").val() + "$" + $("#ddlRoadC").val(),
                type: "POST",
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
                    alert(xhr.responseText);
                }
            });
        }
    }
    else if ($("#ddlHeadContractorC").find('option').length != 0 && $("#ddlHeadAgreementC").find('option').length != 0 && $("#ddlHeadRoadC").find('option').length != 0) {
        if ($("#ddlHeadContractorC").val() != "0" && $("#ddlHeadContractorC").val() != "" && $("#ddlHeadAgreementC").val() != "0" && $("#ddlHeadAgreementC").val() != "" && $("#ddlHeadRoadC").val() != "0" && $("#ddlHeadRoadC").val() != "") {
            $.ajax({
                url: "/TEO/IsFinalPayment/" + $("#ddlHeadContractorC").val() + "$" + $("#ddlHeadAgreementC").val() + "$" + $("#ddlHeadRoadC").val(),
                type: "POST",
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
                    alert(xhr.responseText);
                }
            });
        }
    }


    //Added By Abhishek kamble to populate District using state selection drop down   1 Oct 2014 start
    //$(function () {
    //    $("#ddlStateC").trigger("change");
    //});
    //$("#ddlStateC").change(function () {

    //    var StateCodeC = $("#ddlStateC").val();



    //    $.ajax({

    //        type: "POST",
    //        url: "/TEO/PopulateDistricts/" + StateCodeC,
    //        datatype: 'json',
    //        async: false,
    //        cache: false,
    //        success: function (data) {
    //            $("#ddlDistrictC").empty();
    //            $.each(data, function () {
    //                $("#ddlDistrictC").append("<option value=" + this.Value + ">" + this.Text + "</option>");
    //                //if (this.Value == DPIUC) {

    //                //    $("#ddlDPIUC").append("<option value=" + this.Value + " selected=true>" + this.Text + "</option>");
    //                //}
    //                //else {
    //                //    $("#ddlDPIUC").append("<option value=" + this.Value + ">" + this.Text + "</option>");
    //                //}
    //            });
    //        },
    //        error: function (xhr, ajaxOptions, thrownError) {

    //        }

    //    });

    //});

    //Added By Abhishek kamble to populate District drop down 1 Oct 2014 end


    $("#ddlDistrictC").change(function () {


        var districtCode = $("#ddlDistrictC").val();

        var districtCodeD = $("#ddlDistrictD").val();


        var txnId = $('#parentTxnId').val();

        if (vpDistRepeat == 'N' && $("#ddlDistrictD").val() != 0 && districtCode == $("#ddlDistrictD").val()) {
            $("#ddlDistrictC").val(0);
            alert('Please select different district.');
            FillInCascadeDropdown(null, '#ddlDPIUC', "/TEO/PopulateDPIU/" + 0);
            return;
        }



        if (districtCodeD > 0 && districtCode > 0 && (txnId == 164 || txnId == 165 || txnId == 1195 || txnId == 1194) && districtCodeD != districtCode) {
            //alert("test1");

            $.ajax({
                type: 'GET',
                url: '/TEO/CheckIsDistrictShifted',
                async: false,
                cache: false,
                data: { DistrictC: districtCode, DistrictD: districtCodeD },
                success: function (data) {

                    if (data.exist == true) {

                        $('#frmTEOAddDetailsForTOB').find("select,input,textarea,button").not("#HeadIsFinalPayC,#HeadIsFinalPayD").attr('disabled', false);
                    }
                    else {
                        $("#ddlDistrictD").val(0);
                        $("#ddlDistrictC").val(0);
                        alert("District is not shifted.Please select other district.");
                        $('#frmTEOAddDetailsForTOB').find("select,input,textarea,button").attr('disabled', true);
                        $('#trddlDistrictC').find("select").attr('disabled', false);
                        $('#trddlDistrictD').find("select").attr('disabled', false);
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

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                type: 'GET',
                url: '/TEO/CheckForTransactionAlreadyExist/',
                async: false,
                cache: false,
                data: { BillID: billId },
                success: function (data) {

                    if (data.exist == true) {


                        //Added By Abhishek kamble to set state D start
                        // alert("test2 : "+_stateCodeD);

                        $("#ddlStateD").val(data.StateD);

                        $("#ddlStateD").trigger("change");

                        setTimeout(function () {
                            $("#ddlStateC").attr("disabled", "disabled");
                            $("#ddlStateD").attr("disabled", "disabled");

                        }, 1000);

                        //Added By Abhishek kamble to set state D end

                        $("#ddlDistrictC").val(data.DistrictC);
                        $("#ddlDistrictD").val(data.DistrictD);



                        //  FillInCascadeDropdown(null, '#ddlDPIUC', "/TEO/PopulateDPIU/" + data.DistrictC);

                        //   FillInCascadeDropdown(null, '#ddlDPIUD', "/TEO/PopulateDPIU/" + data.DistrictD);


                        var DPIUC = data.PIUC;
                        var DPIUD = data.PIUD;

                        $("#ddlDPIUC").empty();
                        $("#ddlDPIUD").empty();
                        $.ajax({

                            type: "POST",
                            url: "/TEO/PopulateDPIU/" + data.DistrictC,
                            datatype: 'json',
                            async: false,
                            cache: false,
                            success: function (data) {
                                $.each(data, function () {
                                    if (this.Value == DPIUC) {

                                        $("#ddlDPIUC").append("<option value=" + this.Value + " selected=true>" + this.Text + "</option>");
                                    }
                                    else {
                                        $("#ddlDPIUC").append("<option value=" + this.Value + ">" + this.Text + "</option>");
                                    }
                                });
                            },
                            error: function (xhr, ajaxOptions, thrownError) {

                            }



                        });

                        //Commented By Abhishek kamble for State selection for TOB 1 Oct 2014 start

                        $.ajax({

                            type: "POST",
                            url: "/TEO/PopulateDPIU/" + (isStateWiseBalanceTransfer ? data.DistrictD + "$N" : data.DistrictD),//"$N" added by Abhishek to populate DPIU for State Balance transfer
                            datatype: 'json',
                            async: false,
                            cache: false,
                            success: function (data) {
                                $.each(data, function () {
                                    if (this.Value == DPIUD) {

                                        $("#ddlDPIUD").append("<option value=" + this.Value + " selected=true>" + this.Text + "</option>");
                                    }
                                    else {
                                        $("#ddlDPIUD").append("<option value=" + this.Value + ">" + this.Text + "</option>");
                                    }
                                });
                            },
                            error: function (xhr, ajaxOptions, thrownError) {

                            }



                        });

                        //Commented By Abhishek kamble for State selection for TOB 1 Oct 2014 end


                        if (vpContRepeat == "Y" && $('#ddlContractorC').is(":visible")) {


                            /*FillInCascadeDropdown(null, '#ddlContractorC', "/TEO/PopulateContractor/" + data.DistrictC);

                            FillInCascadeDropdown(null, '#ddlContractorD', "/TEO/PopulateContractor/" + data.DistrictD);*/
                            var contractorC = data.ContractorC;

                            $("#ddlContractorC").empty();
                            $("#ddlContractorD").empty();
                            $.ajax({

                                type: "POST",
                                url: "/TEO/PopulateContractor/" + data.DistrictC,
                                datatype: 'json',
                                async: false,
                                cache: false,
                                success: function (data) {
                                    $.each(data, function () {
                                        if (this.Value == contractorC) {

                                            $("#ddlContractorC").append("<option value=" + this.Value + " selected=true>" + this.Text + "</option>");
                                        }
                                        else {
                                            $("#ddlContractorC").append("<option value=" + this.Value + ">" + this.Text + "</option>");
                                        }
                                    });
                                },
                                error: function (xhr, ajaxOptions, thrownError) {

                                }



                            });

                            //Commented By Abhishek kamble for State selection for TOB 1 Oct 2014 start

                            $.ajax({

                                type: "POST",
                                url: "/TEO/PopulateContractor/" + data.DistrictD,
                                datatype: 'json',
                                async: false,
                                cache: false,
                                success: function (data) {
                                    $.each(data, function () {
                                        if (this.Value == contractorC) {

                                            $("#ddlContractorD").append("<option value=" + this.Value + " selected=true>" + this.Text + "</option>");
                                        }
                                        else {
                                            $("#ddlContractorD").append("<option value=" + this.Value + ">" + this.Text + "</option>");
                                        }
                                    });
                                },
                                error: function (xhr, ajaxOptions, thrownError) {

                                }

                            });

                            $("#ddlAgreementD").empty();
                            FillInCascadeDropdown(null, '#ddlAgreementD', "/TEO/PopulateAgreement/" + data.DistrictD + "$" + data.ContractorC);

                            //Commented By Abhishek kamble for State selection for TOB 1 Oct 2014 end

                            $("#ddlAgreementC").empty();
                            FillInCascadeDropdown(null, '#ddlAgreementC', "/TEO/PopulateAgreement/" + data.DistrictC + "$" + data.ContractorC);



                            //setTimeout(function () {

                            //    $("#ddlContractorC").val(data.ContractorC);
                            //    $("#ddlContractorD").val(data.ContractorC);

                            //}, 3000);

                            $("#ddlContractorC").attr('disabled', 'disabled');

                            //Commented By Abhishek kamble for State selection for TOB 1 Oct 2014 start
                            $("#ddlContractorD").attr('disabled', 'disabled');
                            //Commented By Abhishek kamble for State selection for TOB 1 Oct 2014 end


                            $.ajax({
                                url: "/Receipt/GetContractorName/" + data.ContractorC,
                                type: "POST",
                                async: false,
                                cache: false,
                                success: function (data) {

                                    $('#trConSupNameC').show();
                                    $('#ConSupNameC').text(data);

                                    $('#trConSupNameD').show();
                                    $('#ConSupNameD').text(data);


                                },
                                error: function (xhr, ajaxOptions, thrownError) {
                                    alert(xhr.responseText);
                                }
                            });

                        }

                        //setTimeout(function () {    

                        //    $("#ddlDPIUC").val(data.PIUC);
                        //    $("#ddlDistrictD").val(data.DistrictD);
                        //    $("#ddlDPIUD").val(data.PIUD);

                        //}, 3000);


                        $("#ddlDistrictC").attr('disabled', 'disabled');
                        $("#ddlDPIUC").attr('disabled', 'disabled');

                        //Commented By Abhishek kamble for State selection for TOB 1 Oct 2014 start
                        $("#ddlDistrictD").attr('disabled', 'disabled');
                        $("#ddlDPIUD").attr('disabled', 'disabled');
                        //Commented By Abhishek kamble for State selection for TOB 1 Oct 2014 end

                        $('#frmTEOAddDetailsForTOB').find("select,input,textarea,button").not("#HeadIsFinalPayC,#HeadIsFinalPayD,#ddlDistrictC,#ddlDPIUC,#ddlDistrictD,#ddlDPIUD ").attr('disabled', false);


                        $.unblockUI();
                    }


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    $.unblockUI();
                }
            })
            $.unblockUI();
            return;
        }




        if ($("#ddlContractorC").css('display') != "none") {
            FillInCascadeDropdown(null, '#ddlContractorC', "/TEO/PopulateContractor/" + $("#ddlDistrictC").val());
            $("#ddlContractorC").trigger('change');
        }
        if ($("#trddlDPIUC").css('display') != "none") {
            FillInCascadeDropdown(null, '#ddlDPIUC', "/TEO/PopulateDPIU/" + $("#ddlDistrictC").val());
        }

        if (vpDistRepeat == 'Y') {

            //added by Koustubh Nakate on 14/10/2013 

            var recCount = jQuery("#tblTEODetailsGrid").getGridParam('reccount');

            //            alert(recCount);

            if (recCount == 0) {
                //Commented By Abhishek kamble for State selection for TOB 1 Oct 2014 start
                $("#ddlDistrictD").val(districtCode);
                $("#ddlDistrictD").attr('disabled', true);
                $("#ddlDistrictD").trigger('change');
                //Commented By Abhishek kamble for State selection for TOB 1 Oct 2014 start
            }

            else {

                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

                $.ajax({
                    type: 'GET',
                    url: '/TEO/CheckForTransactionAlreadyExist/',
                    async: false,
                    cache: false,
                    data: { BillID: billId },
                    success: function (data) {

                        if (data.exist == true) {


                            $("#ddlDistrictC").val(data.DistrictC);
                            $("#ddlDistrictD").val(data.DistrictD);


                            //$("#ddlDistrictC").trigger('change');

                            //  FillInCascadeDropdown(null, '#ddlDPIUC', "/TEO/PopulateDPIU/" + data.DistrictC);

                            //   FillInCascadeDropdown(null, '#ddlDPIUD', "/TEO/PopulateDPIU/" + data.DistrictD);

                            var DPIUC = data.PIUC;
                            var DPIUD = data.PIUD;

                            $("#ddlDPIUC").empty();
                            $("#ddlDPIUC").empty();
                            $.ajax({

                                type: "POST",
                                url: "/TEO/PopulateDPIU/" + data.DistrictC,
                                datatype: 'json',
                                async: false,
                                cache: false,
                                success: function (data) {
                                    $.each(data, function () {
                                        if (this.Value == DPIUC) {

                                            $("#ddlDPIUC").append("<option value=" + this.Value + " selected=true>" + this.Text + "</option>");
                                        }
                                        else {
                                            $("#ddlDPIUC").append("<option value=" + this.Value + ">" + this.Text + "</option>");
                                        }
                                    });
                                },
                                error: function (xhr, ajaxOptions, thrownError) {

                                }



                            });

                            $.ajax({

                                type: "POST",
                                url: "/TEO/PopulateDPIU/" + (isStateWiseBalanceTransfer ? data.DistrictD + "$N" : data.DistrictD),
                                datatype: 'json',
                                async: false,
                                cache: false,
                                success: function (data) {
                                    $.each(data, function () {
                                        if (this.Value == DPIUD) {

                                            $("#ddlDPIUD").append("<option value=" + this.Value + " selected=true>" + this.Text + "</option>");
                                        }
                                        else {
                                            $("#ddlDPIUD").append("<option value=" + this.Value + ">" + this.Text + "</option>");
                                        }
                                    });
                                },
                                error: function (xhr, ajaxOptions, thrownError) {

                                }



                            });





                            if (vpContRepeat == "Y" && $('#ddlContractorC').is(":visible")) {

                                $("#ddlAgreementD").empty();
                                //alert($("#ddlAgreementD > option").length);
                                FillInCascadeDropdown(null, '#ddlAgreementD', "/TEO/PopulateAgreement/" + data.DistrictD + "$" + data.ContractorC);

                                $("#ddlAgreementC").empty();
                                FillInCascadeDropdown(null, '#ddlAgreementC', "/TEO/PopulateAgreement/" + data.DistrictC + "$" + data.ContractorC);

                                /* FillInCascadeDropdown(null, '#ddlContractorC', "/TEO/PopulateContractor/" + data.DistrictC);
 
                                 FillInCascadeDropdown(null, '#ddlContractorD', "/TEO/PopulateContractor/" + data.DistrictD);
 
                                 setTimeout(function () {
 
                                     $("#ddlContractorC").val(data.ContractorC);
                                     $("#ddlContractorD").val(data.ContractorC);
 
                                 }, 3000);*/

                                var contractorC = data.ContractorC;

                                $("#ddlContractorC").empty();
                                $("#ddlContractorD").empty();
                                $.ajax({

                                    type: "POST",
                                    url: "/TEO/PopulateContractor/" + data.DistrictC,
                                    datatype: 'json',
                                    async: false,
                                    cache: false,
                                    success: function (data) {
                                        $.each(data, function () {
                                            if (this.Value == contractorC) {

                                                $("#ddlContractorC").append("<option value=" + this.Value + " selected=true>" + this.Text + "</option>");
                                            }
                                            else {
                                                $("#ddlContractorC").append("<option value=" + this.Value + ">" + this.Text + "</option>");
                                            }
                                        });
                                    },
                                    error: function (xhr, ajaxOptions, thrownError) {

                                    }



                                });


                                $.ajax({

                                    type: "POST",
                                    url: "/TEO/PopulateContractor/" + data.DistrictD,
                                    datatype: 'json',
                                    async: false,
                                    cache: false,
                                    success: function (data) {
                                        $.each(data, function () {
                                            if (this.Value == contractorC) {

                                                $("#ddlContractorD").append("<option value=" + this.Value + " selected=true>" + this.Text + "</option>");
                                            }
                                            else {
                                                $("#ddlContractorD").append("<option value=" + this.Value + ">" + this.Text + "</option>");
                                            }
                                        });
                                    },
                                    error: function (xhr, ajaxOptions, thrownError) {

                                    }



                                });




                                $("#ddlContractorC").attr('disabled', 'disabled');
                                $("#ddlContractorD").attr('disabled', 'disabled');


                                $.ajax({
                                    url: "/Receipt/GetContractorName/" + data.ContractorC,
                                    type: "POST",
                                    async: false,
                                    cache: false,
                                    success: function (data) {

                                        $('#trConSupNameC').show();
                                        $('#ConSupNameC').text(data);

                                        $('#trConSupNameD').show();
                                        $('#ConSupNameD').text(data);


                                    },
                                    error: function (xhr, ajaxOptions, thrownError) {
                                        alert(xhr.responseText);
                                    }
                                });

                            }




                            /* setTimeout(function () {
 
                                 $("#ddlDPIUC").val(data.PIUC);
                                 $("#ddlDistrictD").val(data.DistrictD);
                                 $("#ddlDPIUD").val(data.PIUD);
 
                             }, 3000);*/

                            $("#ddlDistrictC").attr('disabled', 'disabled');
                            $("#ddlDPIUC").attr('disabled', 'disabled');
                            $("#ddlDistrictD").attr('disabled', 'disabled');
                            $("#ddlDPIUD").attr('disabled', 'disabled');


                            $.unblockUI();
                        }


                    },
                    error: function (xhr, ajaxOptions, thrownError) {

                    }
                })
            }

        }
        else if (txnId != 164 && txnId != 165 && txnId != 1195 && txnId != 1194) {

            $("#ddlDistrictD").attr('disabled', false);

            //Added By Abhishek kamble 7-Oct-2014 for State Balance transfer start
            $("#ddlStateD").attr('disabled', false);
            //$("#ddlStateD").trigger("change");
            //Added By Abhishek kamble 7-Oct-2014 for State Balance transfer end


            // $("#ddlDistrictD").find("option[value='" + districtCode + "']").remove();

        }
        else if (districtCode > 0 && (txnId || 164 && txnId || 165 && txnId || 1195 && txnId || 1194) && !$("#ddlDistrictC").is(":disabled")) {

            $("#ddlDistrictD").attr('disabled', false);

            //Added By Abhishek kamble 7-Oct-2014 for State Balance transfer start
            $("#ddlStateD").attr('disabled', false);
            //$("#ddlStateD").trigger("change");
            //Added By Abhishek kamble 7-Oct-2014 for State Balance transfer end
        }

        $('#ddlHeadC').val(0).trigger('change');

        $('#ddlHeadD').val(0).trigger('change');

        /*  setTimeout(function () {
  
              $('#ddlHeadC').trigger('change');
              $('#ddlHeadD').trigger('change');
             
          }, 1000);*/

        if (txnId != 164 && txnId != 165 && txnId != 1195 && txnId != 1194) {
            $("#ddlHeadC").attr('disabled', false);
            $("#ddlHeadD").attr('disabled', false);
        }


    });


    // alert($("#ddlDistrictC").val());

    if ($("#ddlDistrictC").val() == 0) {
        $("#ddlDPIUC").empty();
        $("#ddlDPIUC").append("<option value='0' >Select DPIU </option>");
    }


    $("#ddlDPIUC").change(function () {

        var DPIU = $("#ddlDPIUC").val();


        if (vpDPIURepeat == 'Y') {
            //Commented By Abhishek kamble for State selection for TOB 1 Oct 2014 start
            $("#ddlDPIUD").val(DPIU);
            $("#ddlDPIUD").attr('disabled', true);
            //Commented By Abhishek kamble for State selection for TOB 1 Oct 2014 start
        }
        else {
            $("#ddlDPIUD").attr('disabled', false);
        }

    });

    $("#ddlContractorC").change(function () {
        $("#divTEODetailsErrorForTOB").hide("slide");
        $("#divTEODetailsErrorForTOB span:eq(1)").html('');
        if ($("#ddlContractorC").val() != "" && $("#ddlContractorC").val() != null) {
            $.ajax({
                url: "/Receipt/GetContractorName/" + $("#ddlContractorC").val(),
                type: "POST",
                async: false,
                cache: false,
                success: function (data) {
                    if ($("#ddlAgreementC").css('display') != "none") {

                        //if ($("#ddlDistrictC").is(":disabled")) {
                        //    $("#ddlDistrictC").attr('disabled', false);
                        //}

                        if ($("#ddlDistrictC").val() == 0) {
                            $("#divTEODetailsErrorForTOB").show("slide");
                            $("#divTEODetailsErrorForTOB span:eq(1)").html('<strong>Alert: </strong>Please Select District to get Agreement Details');
                            return false;
                        }
                        else {
                            var districtCode = 0;
                            if ($("#trddlDistrictC").css('display') != "none") {
                                districtCode = $("#ddlDistrictC").val();
                            }


                            $("#ddlAgreementC").empty();
                            FillInCascadeDropdown(null, '#ddlAgreementC', "/TEO/PopulateAgreement/" + districtCode + "$" + $("#ddlContractorC").val());

                        }
                    }
                    $("#ConSupNameC").text(data);
                    $("#trConSupNameC").show('slow');


                    if (vpContRepeat == 'Y' || vpSupRepeat == "Y") {
                        //Commented By Abhishek kamble for State selection for TOB 1 Oct 2014 start
                        $("#ddlContractorD").val($("#ddlContractorC").val());
                        $("#ddlContractorD").attr('disabled', true);
                        $("#ddlContractorD").trigger('change');
                        //Commented By Abhishek kamble for State selection for TOB 1 Oct 2014 start
                    }
                    else {
                        $("#ddlContractorD").attr('disabled', false);

                    }

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                }
            });
        }
        else {
            $("#ConSupNameC").text("");
            $("#trconSupNameC").hide('slow');
            $("#ddlAgreementC").empty();
            $("#ddlRoadC").empty();
        }

    });

    $("#ddlSancYearC").change(function () {
        if ($("#trddlPackageC").css('display') != "none") {
            if ($("#trddlRoadC").css('display') != "none") {
                $("#ddlRoadC").empty();
            }
            FillInCascadeDropdown(null, '#ddlPackageC', "/TEO/PopulatePackage/" + $("#ddlSancYearC").val());
        }
    });

    $("#ddlPackageC").change(function () {
        if ($("#trddlRoadC").css('display') != "none") {
            //FillInCascadeDropdown(null, '#ddlRoadC', "/TEO/PopulateRoad/0$" + $("#ddlPackageC").val());
            FillInCascadeDropdown(null, '#ddlRoadC', "/TEO/PopulateRoad/0$" + $("#ddlPackageC").val() + "$0$" + $("#ddlContractorC option:selected").val() + "?" + $.param({ AGREEMENT_NUMBER: $("#ddlAgreementC option:selected").text() }));
            // $("#trHeadIsFinalPayC").show();
        }
    });

    $("#ddlAgreementC").change(function () {

        if ($("#trddlRoadC").is(":visible")) {
            //FillInCascadeDropdown(null, '#ddlRoadC', "/TEO/PopulateRoad/" + $("#ddlAgreementC").val() + "$0");
            FillInCascadeDropdown(null, '#ddlRoadC', "/TEO/PopulateRoad/" + $("#ddlAgreementC").val() + "$0" + "$0$" + $("#ddlContractorC option:selected").val() + "?" + $.param({ AGREEMENT_NUMBER: $("#ddlAgreementC option:selected").text() }));
            $("#trHeadIsFinalPayC").show();
        }

        if (vpAggRepeat == "Y") {
            $("#ddlAgreementD").val($("#ddlAgreementC").val());
            $("#ddlAgreementD").attr('disabled', true);
            $("#ddlAgreementD").trigger('change');
        }
        else {
            $("#ddlAgreementD").attr('disabled', false);

        }


    });

    $("#ddlRoadC").change(function () {
        if ($(this).val() != "0") {
            if ($("#ddlContractorC").val() != "0" && $("#ddlContractorC").val() != "" && $("#ddlContractorC").val() != null && $("#ddlAgreementC").val() != "0" && $("#ddlAgreementC").val() != "" && $("#ddlAgreementC").val() != null && $("#ddlRoadC").val != "0" && $("#ddlRoadC").val != "" && $("#ddlRoadC").val != null) {
                $.ajax({
                    url: "/TEO/IsFinalPayment/" + $("#ddlContractorC").val() + "$" + $("#ddlAgreementC").val() + "$" + $("#ddlRoadC").val(),
                    type: "POST",
                    async: false,
                    cache: false,
                    success: function (data) {
                        if (data == "1") {
                            $("#trHeadIsFinalPayC").show('slow');
                            $("#HeadIsFinalPayC").attr('checked', 'checked');
                            $("#HeadIsFinalPayC").attr('disabled', 'disabled');
                        }
                        else {

                            $("#trHeadIsFinalPayC").show('slow');
                            $("#HeadIsFinalPayC").attr('checked', false);
                            // $("#HeadIsFinalPayC").attr('disabled', false);
                        }


                        if (vpRoadRepeat == "Y") {
                            $("#ddlRoadD").val($("#ddlRoadC").val());
                            $("#ddlRoadD").attr('disabled', true);
                            $("#ddlRoadD").trigger('change');
                        }
                        else {
                            $("#ddlRoadD").attr('disabled', false);

                        }

                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.responseText);
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



        if ($(this).val() != "0") {
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
                        if ($("#ddlDistrictC").css('display') != "none" && $("#ddlDistrictC").val() != 0) {
                            FillInCascadeDropdown(null, '#ddlHeadContractorC', "/TEO/PopulateContractor/" + $("#ddlDistrictC").val());
                            $("#ddlHeadContractorC").trigger('change');
                        }
                        else {
                            FillInCascadeDropdown(null, '#ddlHeadContractorC', "/TEO/PopulateContractor/");
                        }
                        $("#trddlHeadContractorC").show('slow');
                        conC = "Y";
                    }
                    if (data.AGREEMENT_REQ == "Y") {

                        $('#ddlHeadAgreementC').val(0);
                        $("#trddlHeadAgreementC").show('slow');
                        aggC = "Y";
                    }
                    if (data.SANCYEAR_REQ == "Y") {
                        FillInCascadeDropdown(null, '#ddlHeadSancYearC', "/TEO/PopulateSancYear/");
                        $("#trddlHeadSancYearC").show('slow');
                        sancyrC = "Y";
                    }
                    if (data.PKG_REQ == "Y") {
                        $("#trddlHeadPackageC").show('slow');
                        pkgC = "Y";
                    }
                    if (data.ROAD_REQ == "Y") {
                        $("#trddlHeadRoadC").show('slow');
                        $("#trHeadIsFinalPayC").show();
                        roadC = "Y";
                        $('#ddlHeadRoadC').val(0);
                       
                        if ($("#ddlContractorC").css('display') != "none") {
                            if ($("#ddlAgreementC").css('display') != "none") {
                                FillInCascadeDropdown(null, '#ddlHeadRoadC', "/TEO/PopulateRoad/" + $("#ddlAgreementC").val() + "$0" + "$0$" + $("#ddlContractorC option:selected").val() + "?" + $.param({ AGREEMENT_NUMBER: $("#ddlAgreementC option:selected").text() }));
                            }
                        }
                        else {
                            if ($("#ddlHeadPackageC").css('display') != "none") {
                                FillInCascadeDropdown(null, '#ddlHeadRoadC', "/TEO/PopulateRoad/0" + "$" + $("#ddlHeadPackageC option:selected").val() + "$0$" + $("#ddlHeadContractorC option:selected").val() + "?" + $.param({ AGREEMENT_NUMBER: $("#ddlHeadAgreementC option:selected").text() }));
                            }

                        }
                    }
                    else {
                        if (!$("#ddlRoadC").is(":visible") && !$("#ddlHeadRoadC").is(":visible")) {
                            $("#trHeadIsFinalPayC").hide('slow');
                        }
                        else {
                            $("#trHeadIsFinalPayC").show();
                        }
                    }

                    if (vpHeadRepeat == 'Y') {
                        $("#ddlHeadD").val($("#ddlHeadC").val());
                        $("#ddlHeadD").attr('disabled', true);
                        $("#ddlHeadD").trigger('change');
                    }
                    else {
                        $("#ddlHeadD").attr('disabled', false);
                    }


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
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
        $("#divTEODetailsErrorForTOB").hide("slide");
        $("#divTEODetailsErrorForTOB span:eq(1)").html('');

        if ($("#ddlHeadContractorC").val() != 0) { //&& $("#ddlHeadContractorC").val() != null
            $.ajax({
                url: "/Receipt/GetContractorName/" + $("#ddlHeadContractorC").val(),
                type: "POST",
                async: false,
                cache: false,
                success: function (data) {
                    //alert(aggC);

                    // $("#ddlDistrictC").attr('disabled', false);

                    if ($('#ddlHeadAgreementC').is(":visible") && aggC == "N") {
                        aggC = "Y";
                    }

                    if (aggC == "Y") {
                        if ($("#ddlDistrictC").val() == 0) {
                            $("#divTEODetailsErrorForTOB").show("slide");
                            $("#divTEODetailsErrorForTOB span:eq(1)").html('<strong>Alert: </strong>Please Select District to get Agreement Details');
                            return false;
                        }
                        else {
                            FillInCascadeDropdown(null, '#ddlHeadAgreementC', "/TEO/PopulateAgreement/" + $("#ddlDistrictC").val() + "$" + $("#ddlHeadContractorC").val());

                        }
                    }
                    $("#HeadConSupNameC").text(data);
                    $("#trHeadConSupNameC").show('slow');

                    //alert(vpContRepeat + vpSupRepeat);

                    if (vpContRepeat == 'Y' || vpSupRepeat == "Y") {


                        $("#ddlHeadContractorD").val($("#ddlHeadContractorC").val());
                        $("#ddlHeadContractorD").attr('disabled', true);

                        $("#ddlHeadContractorD").trigger('change');
                        //setTimeout(function () {

                        //    alert('a');

                        //}, 500);


                    }
                    else {
                        $("#ddlHeadContractorD").attr('disabled', false);
                    }
                    $('#ddlHeadRoadC').val(0);
                    $('#ddlHeadRoadD').val(0);
                    //$("#ddlDistrictC").attr('disabled', true);
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
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

        //  alert(roadC);
        var headAgreementID = $("#ddlHeadAgreementC").val();

        if ($('#ddlHeadRoadC').is(":visible") && roadC == "N") {
            roadC = "Y";
        }

        if (roadC == "Y") {
            //FillInCascadeDropdown(null, '#ddlHeadRoadC', "/TEO/PopulateRoad/" + headAgreementID + "$0");
            FillInCascadeDropdown(null, '#ddlHeadRoadC', "/TEO/PopulateRoad/" + $("#ddlHeadAgreementC option:selected").val() + "$" + $("#ddlHeadPackageC option:selected").val() + "$0$" + $("#ddlHeadContractorC option:selected").val() + "?" + $.param({ AGREEMENT_NUMBER: $("#ddlHeadAgreementC option:selected").text() }));
            $("#trHeadIsFinalPayC").show();

        }
        if (vpAggRepeat == "Y") {

            $("#ddlHeadAgreementD").val(headAgreementID);
            $("#ddlHeadAgreementD").attr('disabled', true);
            $("#ddlHeadAgreementD").trigger('change');
        }
        else {

            $("#ddlHeadAgreementD").attr('disabled', false);
        }



    });

    $("#ddlHeadSancYearC").change(function () {
        if (pkgC == "Y") {
            if (roadC == "Y") {
                $("#ddlHeadRoadC").empty();
            }
            //FillInCascadeDropdown(null, '#ddlHeadPackageC', "/TEO/PopulatePackage/" + $("#ddlHeadSancYearC").val());
            FillInCascadeDropdown(null, '#ddlHeadPackageC', "/TEO/PopulatePackage/" + $("#ddlHeadSancYearC").val() + "$" + $("#ddlDistrictC").val());

        }
    });

    $("#ddlHeadPackageC").change(function () {
        if (roadC == "Y") {
            //FillInCascadeDropdown(null, '#ddlHeadRoadC', "/TEO/PopulateRoad/0$" + $("#ddlHeadPackageC").val());
            FillInCascadeDropdown(null, '#ddlHeadRoadC', "/TEO/PopulateRoad/0" + "$" + $("#ddlHeadPackageC option:selected").val() + "$0$" + $("#ddlHeadContractorC option:selected").val() + "?" + $.param({ AGREEMENT_NUMBER: $("#ddlHeadAgreementC option:selected").text() }));
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

                        if (vpRoadRepeat == "Y") {

                            $("#ddlHeadRoadD").val($("#ddlHeadRoadC").val());
                            $("#ddlHeadRoadD").attr('disabled', true);
                            $("#ddlHeadRoadD").trigger('change');
                        }
                        else {

                            $("#ddlHeadRoadD").attr('disabled', false);
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.responseText);
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

    $.validator.unobtrusive.parse($('#frmTEOAddDetailsForTOB'));


    $("#btnResetCD").click(function (e) {

        //$("#ddlDistrictC").val(0);
        //   e.preventDefault();
        //for credit
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

        $("#HeadConSupNameC").text('');

        //for debit

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

        $("#divTEODetailsErrorForTOB").hide("slide");
        $("#divTEODetailsErrorForTOB span:eq(1)").html("");

        $("#HeadConSupNameD").text('');



        if ($("#ddlDistrictC").is(':disabled')) {

            //alert('a');
            $("#ddlDistrictC").attr('disabled', false);

        }

        if ($("#ddlDPIUC").is(':disabled')) {
            $("#ddlDPIUC").attr('disabled', false);

        }

        if ($("#ddlContractorC").is(':disabled')) {
            $("#ddlContractorC").attr('disabled', false);

            $('#ConSupNameC').text('');
            $('#ConSupNameD').text('');
        }

        //alert($('#ddlHeadC').val());
        /*  if (isSaved == false) {
              //$("#ddlDistrictC").val(0).trigger('change');
              //$("#ddlHeadC").val(0).trigger('change');
              //$("#ddlHeadD").val(0).trigger('change');
  
              $("#ddlDistrictC").trigger('change');
              $("#ddlHeadC").val(0).trigger('change');
              $("#ddlHeadD").val(0).trigger('change');
  
          }*/


        setTimeout(function () {

            if (isSaved == false) {

                $("#ddlDistrictC").change();
                $("#ddlHeadC").change();
                $("#ddlHeadD").change();

            }

        }, 1000);

        isSaved = false;

    });


    $('#AMOUNTC').blur(function () {

        if (parseFloat($('#AMOUNTC').val()) > 0) {

            $('#AMOUNTD').val($('#AMOUNTC').val());
        }
        else {
            $('#AMOUNTD').val('');
        }


    });

    var showConDDl = false;

    var showConDDlForDeduction = false;

    //event to save the credit and debit teo details
    $("#btnSaveTEOCreditDebitDetails").click(function (evt) {


        //Added By Abhishek kamble to set State D start
        _stateCodeD = $("#ddlStateD").val();
        $("#ddlStateC").attr("disabled", false);
        $("#ddlStateD").attr("disabled", false);
        //Added By Abhishek kamble to set State D end

        evt.preventDefault();
        var formStatus = $('#frmTEOAddDetailsForTOB').valid();
        var statusC = validateCreditDetails($("#AMOUNTC").val(), amountValC);

        var statusD = validateDebitDetails($("#AMOUNTD").val(), amountValD);


        if (formStatus == true && statusC == true && statusD == true) {
            //for credit

            ValidateCreditDPIUForMonthClosed($("#ddlDPIUC").val(), billId);
            ValidateDebitDPIUForMonthClosed($("#ddlDPIUD").val(), billId);

            if (isValidCredit == false) {
                return false;
            }

            if (isValidDebit == false) {
                return false;
            }

            var isDistrictDisabled = false;
            var isDPIUDisabled = false;
            if ($("#ddlDistrictD").is(":disabled")) {
                isDistrictDisabled = true;

            }
            if ($("#ddlDPIUD").is(":disabled")) {
                isDPIUDisabled = true;
            }



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

            //for debit
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


            var txnId = $('#parentTxnId').val();

            if ($("#trddlContractorC").is(":visible")) {
                showConDDl = true;
            }

            if ($("#trddlContractorD").is(":visible")) {
                showConDDlForDeduction = true;
            }
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/TEO/AddCreditDebitTEODetailsForTOB/" + $("#tblTEOMasterGrid").getDataIDs()[0],
                type: "POST",
                async: false,
                cache: false,
                data: $("#frmTEOAddDetailsForTOB").serialize(),
                success: function (data) {
                    $.unblockUI();

                    //alert(data.success);
                    //alert(data.message);

                    if (data.success == false || data.success === undefined) {





                        //  alert('inside if');
                        if (data.message === undefined || data.message == null) {

                            $("#loadTEOCreditDebitDetails").html(data);

                            //Commented By Abhishek kamble for State selection for TOB 9 Oct 2014 start
                            $("#ddlStateC").attr('disabled', true);
                            $("#ddlStateD").attr('disabled', true);
                            //Commented By Abhishek kamble for State selection for TOB 9 Oct 2014 start


                            $.each($("select"), function () {
                                if ($(this).find('option').length > 0) {
                                    $('#tr' + $(this).attr('id')).show();
                                }
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

                                    // alert(vpDistRepeat);


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
                                            alert(xhr.responseText);
                                        }
                                    });
                                    if (vpDistRepeat == "Y") {


                                        $("#ddlDistrictC").attr('disabled', 'disabled');



                                    }
                                    if (vpDPIURepeat == "Y") {
                                        $("#ddlDPIUC").attr('disabled', 'disabled');
                                    }
                                    if (vpContRepeat == "Y" || vpSupRepeat == "Y") {
                                        if (txnId != 164 && txnId != 165 && txnId != 1195 && txnId != 1194) {
                                            $("#ddlContractorC").attr('disabled', 'disabled');
                                            $("#ddlHeadContractorC").attr('disabled', 'disabled');
                                        }
                                    }
                                    if (vpAggRepeat == "Y") {
                                        if (txnId != 164 && txnId != 165 && txnId != 1195 && txnId != 1194) {
                                            $("#ddlAgreementC").attr('disabled', 'disabled');
                                            $("#ddlHeadAgreementC").attr('disabled', 'disabled');
                                        }
                                    }
                                    if (vpRoadRepeat == "Y") {
                                        if (txnId != 164 && txnId != 165 && txnId != 1195 && txnId != 1194) {
                                            $("#ddlRoadC").attr('disabled', 'disabled');
                                            $("#ddlHeadRoadC").attr('disabled', 'disabled');
                                        }
                                    }
                                    if (vpHeadRepeat == "Y") {
                                        if (txnId != 164 && txnId != 165 && txnId != 1195 && txnId != 1194) {
                                            $("#ddlHeadC").attr('disabled', 'disabled');
                                        }
                                    }


                                    //for debit


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
                                            alert(xhr.responseText);
                                        }
                                    });

                                    //  alert(vpDistRepeat);

                                    //Commented By Abhishek kamble for State selection for TOB 1 Oct 2014 start

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

                                    $("#HeadIsFinalPayD").attr('disabled', true);
                                    //Commented By Abhishek kamble for State selection for TOB 1 Oct 2014 end

                                    $("#HeadIsFinalPayC").attr('disabled', true);
                                    //Commented By Abhishek kamble for State selection for TOB 1 Oct 2014 start

                                    if (isDistrictDisabled == true) {
                                        $("#ddlDistrictD").attr('disabled', 'disabled');
                                    }
                                    if (isDPIUDisabled == true) {
                                        $("#ddlDPIUD").attr('disabled', 'disabled');
                                    }
                                    //Commented By Abhishek kamble for State selection for TOB 1 Oct 2014 end

                                },
                                error: function (xhr, ajaxOptions, thrownError) {

                                }
                            });
                            /*******************************************/



                            return false;
                        }
                        else {


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



                                    //for debit


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
                                            alert(xhr.responseText);
                                        }
                                    });

                                    //  alert(vpDistRepeat);


                                    //Commented By Abhishek kamble for State selection for TOB 1 Oct 2014 start

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

                                    $("#HeadIsFinalPayD").attr('disabled', true);
                                    //Commented By Abhishek kamble for State selection for TOB 1 Oct 2014 end

                                    $("#HeadIsFinalPayC").attr('disabled', true);

                                    //Commented By Abhishek kamble for State selection for TOB 1 Oct 2014 start

                                    if (isDistrictDisabled == true) {

                                        $("#ddlDistrictD").attr('disabled', 'disabled');

                                    }
                                    if (isDPIUDisabled == true) {
                                        $("#ddlDPIUD").attr('disabled', 'disabled');
                                    }
                                    //Commented By Abhishek kamble for State selection for TOB 1 Oct 2014 end

                                },
                                error: function (xhr, ajaxOptions, thrownError) {

                                }
                            });
                            /*******************************************/



                            $("#divTEODetailsErrorForTOB").show("slide");
                            $("#divTEODetailsErrorForTOB span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                            return false;
                        }



                        //$("#ddlDistrictD").attr('disabled', false);
                        //$("#ddlDPIUD").attr('disabled', false);
                        //$("#ddlContractorD").attr('disabled', false);
                        //$("#ddlHeadContractorD").attr('disabled', false);
                        //$("#ddlAgreementD").attr('disabled', false);
                        //$("#ddlHeadAgreementD").attr('disabled', false);
                        //$("#ddlRoadD").attr('disabled', false);
                        //$("#ddlHeadRoadD").attr('disabled', false);
                        //$("#ddlHeadD").attr('disabled', false);

                        //$('#parentTxnId').val(txnId);

                        //Commented By Abhishek kamble for State selection for TOB 1 Oct 2014 start                                                
                        //                        $("#ddlStateD").trigger("change");
                        //Commented By Abhishek kamble for State selection for TOB 1 Oct 2014 end




                        return false;
                    }
                    else {
                        $("#divTEODetailsErrorForTOB").hide("slide");

                        $("#divTEODetailsErrorForTOB span:eq(1)").html('');

                        if ($("#trHeadConSupNameC").css('display') != "none") {
                            $("#trHeadConSupNameC").hide('slow');
                            $("#HeadConSupNameC").text("");

                        }

                        if ($("#trHeadConSupNameD").css('display') != "none") {
                            $("#trHeadConSupNameD").hide('slow');
                            $("#HeadConSupNameD").text("");
                        }

                        isSaved = true;

                        $("#btnResetCD").trigger('click');

                        alert("Credit and Debit Details Added.");

                        //$("#ddlHeadC").trigger('change');

                        // alert("isMulTxn" + isMulTxn);


                        LoadTEODetailsGrid($("#tblTEOMasterGrid").getDataIDs()[0]);



                        if (showConDDl == true) {
                            $("#trddlContractorC").show()
                        }
                        else {
                            $("#trddlContractorC").hide()
                        }

                        if (showConDDlForDeduction == true) {
                            $("#trddlContractorD").show()
                        }
                        else {
                            $("#trddlContractorD").hide()
                        }

                        // $('#parentTxnId').val(txnId);
                        return false;
                    }


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    $.unblockUI();

                    alert(xhr.responseText);
                }
            });
        }
        else {//Else added by abhishek kamble to disable state dropdowns 9 oct 2014

            //Added By Abhishek kamble to set State D start            
            $("#ddlStateC").attr("disabled", "disabled");
            $("#ddlStateD").attr("disabled", "disabled");
            //Added By Abhishek kamble to set State D end
        }

    });

    $("#btnCancelTEOCreditDebitDetails").click(function () {

        if ($("#divFinalizeTEO").is(':visible')) {
            $("#loadTEOCreditDebitDetails").html('');
        }
        else {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/TEO/TEODetailsForTOB/" + $("#tblTEOMasterGrid").getDataIDs()[0],
                type: "POST",
                async: false,
                cache: false,
                success: function (data) {
                    $("#loadTEOCreditDebitDetails").html(data);
                    $.each($("select"), function () {
                        if ($(this).find('option').length >= 1) {
                            $('#tr' + $(this).attr('id')).show();
                        }
                    });

                    $.unblockUI();

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }
            });
        }

    });

    //event for editing details
    $("#btnEditTEOCreditDetails").click(function (evt) {

        evt.preventDefault();
        var formStatus = $('#frmTEOAddDetailsC').valid();
        var status = validateCreditDetails($("#AMOUNTC").val(), amountValC);
        if (formStatus && status) {
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
                        if (data.message == "undefined" || data.message == null) {
                            $("#loadTEOCreditDetails").html(data);
                            $.each($("select"), function () {
                                if ($(this).find('option').length > 0) {
                                    $('#tr' + $(this).attr('id')).show();
                                }
                            });
                            $("#ddlHeadC").trigger('change');
                            return false;
                        }
                        else {
                            $("#divTEODetailsErrorForTOB").show("slide");
                            $("#divTEODetailsErrorForTOB span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                        }
                        return false;
                    }
                    else {
                        $("#divTEODetailsErrorForTOB").hide("slide");
                        $("#divTEODetailsErrorForTOB span:eq(1)").html('');
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

                    alert(xhr.responseText);
                }
            });
        }

    });



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
    $("#divTEODetailsErrorForTOB").hide("slide");
    $("#divTEODetailsErrorForTOB span:eq(1)").html('');
    $("#trHeadIsFinalPayC").hide('slow');

}

function validateCreditDetails(amountToValidate, oldValue) {
    $("#divTEODetailsErrorForTOB").hide("slide");
    $("#divTEODetailsErrorForTOB span:eq(1)").html('');
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
         $("#divTEODetailsErrorForTOB").show("slide");
         $("#divTEODetailsErrorForTOB span:eq(1)").html('<strong>Alert: </strong>Only single transaction entry allowed');
         return false;
     }
     else
     {
         $("#divTEODetailsErrorForTOB").hide("slide");
         $("#divTEODetailsErrorForTOB span:eq(1)").html('');
     }*/

    var masterAmount = $("#tblTEOMasterGrid").jqGrid('getCell', $("#tblTEOMasterGrid").getDataIDs()[0], 'GrossAmount');

    var detailsAmount = 0;
    if (isDetailsGridLoaded) {
        //old code
        //detailsAmount = parseFloat(($("#tblTEODetailsGrid").jqGrid('getCol', 'CAmount', false, "sum") - oldValue)) + parseFloat(amountToValidate);

        //New Code Added By Abhishek kamble 14 Oct 2014 start
        var userdata = $("#tblTEODetailsGrid").getGridParam('userData');
        detailsAmount = parseFloat((userdata._TotalCAmount - oldValue)) + parseFloat(amountToValidate);
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
                $("#ddlContractorC").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlContractorC">Company Name is Required</span>').show();
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
            if (data.ROAD_REQ == "Y" && $("#ddlRoadC").val() == "0") {
                $("#ddlRoadC").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlRoadC">Road Required</span>').show();
                $("#ddlRoadC").addClass("input-validation-error");
                statusFlag = false;
            }
            if (parseFloat(masterAmount) < parseFloat(detailsAmount)) {
                //$("#divTEODetailsErrorForTOB").show("slide");
                //$("#divTEODetailsErrorForTOB span:eq(1)").html('<strong>Alert: </strong> Invalid Amount, Amount exceeds master amount');
                $("#AMOUNTC").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="AMOUNTC">Total Details Amount Exceeds Master Amount</span>').show();
                $("#AMOUNTC").addClass("input-validation-error");
                statusFlag = false;
            }


        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
        }
    });

    if (conC == "Y" && $("#ddlHeadContractorC").val() == "0") {
        $("#ddlHeadContractorC").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlHeadContractorC">Company Name is Required</span>').show();
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


function validateDebitDetails(amountToValidate, oldValue) {

    $("#divTEODetailsErrorForTOB").hide("slide");
    $("#divTEODetailsErrorForTOB span:eq(1)").html('');

    var CrDrDetails = $("#tblTEODetailsGrid").jqGrid('getCol', 'CreditDebit', false);
    var DrCount = 0;
    if (CrDrDetails.length != 0) {
        for (var i = 0; i < CrDrDetails.length; i++) {
            if (CrDrDetails[i] == "Debit") {
                DrCount = DrCount + 1;
            }
        }
    }
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
function ValidateCreditDPIUForMonthClosed(adminCode, billID) {

    $.ajax({
        type: 'POST',
        url: '/TEO/ValidateDPIUMonths?id=' + adminCode + "@" + billID,
        cache: false,
        async: false,
        success: function (data) {
            if (data.IsValid == false) {
                alert("The DPIU on Credit side has closed the selected month and year.");
                isValidCredit = false
                return false;
            }
            else {
                isValidCredit = true;
            }
        },
        error: function () { }
    });
}
function ValidateDebitDPIUForMonthClosed(adminCode, billID) {
    $.ajax({
        type: 'POST',
        url: '/TEO/ValidateDPIUMonths?id=' + adminCode + "@" + billID,
        cache: false,
        async: false,
        success: function (data) {
            if (data.IsValid == false) {
                isValidDebit = false;
                alert("The DPIU on Debit side has closed the selected month and year.");
                return false;
            }
            else {
                isValidDebit = true;
            }
        },
        error: function () { }
    });
}