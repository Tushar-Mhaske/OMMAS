var oldAssetLib = $("#AMOUNT").val();

$(function () {
    $.validator.unobtrusive.adapters.add('isdetailsamountgreater', ['obtype'], function (options) {
        options.rules['isdetailsamountgreater'] = options.params;
    });

    $.validator.addMethod("isdetailsamountgreater", function (value, element, params) {
        
        if (isGridLoaded) {
            var totalAssetLib = 0;
            var totalGrossAmount = 0;

            var firstRow = $("#tblOBList").getRowData($("#tblOBList").getDataIDs()[0]);
            var secondRow = $("#tblOBList").getRowData($("#tblOBList").getDataIDs()[1]);

            

            if ($('input:radio[name="' + params.obtype + '"]:checked').val() == "D") {
                if (firstRow['OBType'] == "Assets") {
                    totalAssetLib = firstRow["DetailsAmount"];
                    totalGrossAmount = firstRow["MasterAmount"];

                    
                }
                else {
                    totalAssetLib = secondRow["DetailsAmount"];
                    totalGrossAmount = secondRow["MasterAmount"];
                }
            }
            else if ($('input:radio[name="' + params.obtype + '"]:checked').val() == "C") {
                if (firstRow['OBType'] == "Liabilities") {
                    totalAssetLib = firstRow["DetailsAmount"];
                    totalGrossAmount = firstRow["MasterAmount"];
                }
                else {
                    totalAssetLib = secondRow["DetailsAmount"];
                    totalGrossAmount = secondRow["MasterAmount"];
                }
            }
            
            if (oldAssetLib == "") {
                return (totalGrossAmount >= (parseFloat(value) + parseFloat(totalAssetLib)));
            }
            else {
                return (totalGrossAmount >= (parseFloat(value) + (parseFloat(totalAssetLib) - parseFloat(oldAssetLib))));
            }            
        }       

    }, function (params, element) {
        if ($('input:radio[name="' + params.obtype + '"]:checked').val() == "D") {
            return "Total Asset details amount must be less than gross amount";
        }
        else {
            return "Total Liability details amount must be less than gross amount";
        }
    });

    $.validator.unobtrusive.adapters.add('isdropdownselected', ['parentdropdown'], function (options) {
        options.rules['isdropdownselected'] = options.params;
    });

    $.validator.addMethod("isdropdownselected", function (value, element, params) {

        if ($("#ddlAgreement").css('display') != "none" && $("#ddlAgreement").val() == "0" && params.parentdropdown == "Agreement Name") {
            return false;
        }
        if ($("#ddlContractor").css('display') != "none" && $("#ddlContractor").val() == "0" && params.parentdropdown == "Company Name") {
            return false;
        }
        if ($("#ddlDPIU").css('display') != "none" && $("#ddlDPIU").val() == "0" && params.parentdropdown == "PIU Name") {
            return false;
        }
        if ($("#ddlRoad").css('display') != "none" && $("#ddlRoad").val() == "0" && params.parentdropdown == "Road Name") {
            return false;
        }
        if ($("#ddlPackage").css('display') != "none" && $("#ddlPackage").val() == "0" && params.parentdropdown == "Package") {
            return false;
        }
        if ($("#ddlSancYear").css('display') != "none" && $("#ddlSancYear").val() == "0" && params.parentdropdown == "Sanction Year") {
            return false;
        }
        return true;       

    }, function (params, element) {
        if ($("#ddlAgreement").css('display') != "none" && $("#ddlAgreement").val() == "0" && params.parentdropdown == "Agreement Name") {
            return "Please Select Aggrement";
        }
        if ($("#ddlContractor").css('display') != "none" && $("#ddlContractor").val() == "0" && params.parentdropdown == "Company Name") {
            return "Please Select Company";
        }
        if ($("#ddlDPIU").css('display') != "none" && $("#ddlDPIU").val() == "0" && params.parentdropdown == "PIU Name") {
            return "Please Select PIU";
        }
        if ($("#ddlRoad").css('display') != "none" && $("#ddlRoad").val() == "0" && params.parentdropdown == "Road Name") {
            return "Please Select Road";
        }
        if ($("#ddlPackage").css('display') != "none" && $("#ddlPackage").val() == "0" && params.parentdropdown == "Package") {
            return "Please Select Package";
        }
        if ($("#ddlSancYear").css('display') != "none" && $("#ddlSancYear").val() == "0" && params.parentdropdown == "Sanction Year") {
            return "Please Select Year";
        }
    });

    $.validator.unobtrusive.adapters.add('iseditabletrans', ["obtype"], function (options) {
        options.rules['iseditabletrans'] = options.params;
    });

    $.validator.addMethod("iseditabletrans", function (value, element, params) {
        if ($("#ddlSubTrans option:selected").attr('class') == "X") {
            return false;
        }
        else {
            return true;
        }
        return true;
    }, function (params, element) {
        return "Invalid Sub Transaction for OB Entry";
    });
});



var con = "N";
var agg = "N";
var piu = "N";
var sancyr = "N";
var pkg = "N";
var road = "N";

$(document).ready(function () {
    
    $.validator.unobtrusive.parse($('#frmAddOBDetails'));

    $("#btnlblBackToList").show('slow');

    $("#DivIcoDetailsOB").click(function () {
        if ($(this).hasClass('ui-icon-circle-triangle-n')) {
            $("#tblOBDetails").hide('slide');
            $(this).removeClass('ui-icon-circle-triangle-n');
            $(this).addClass('ui-icon-circle-triangle-s');
        }
        else {
            $("#tblOBDetails").show('slide');
            $(this).removeClass('ui-icon-circle-triangle-s');
            $(this).addClass('ui-icon-circle-triangle-n');
        }
    });

    if (assetLib == "A") {
        $("#rdoAsset").attr("checked", "checked");
    }
    else {
        $("#rdoLib").attr("checked", "checked");
    }

    $("#rdoAsset").change(function () {
        $("#NARRATION").val("");
        $("#trDetailsHeadDesc").find('td:eq(2)').text("");
        if ($(this).attr('checked') == "checked") {
            $("#ddlSubTrans").empty();
            $("#ddlSubTrans").append("<option value='0'>Select Subtransaction</option>");
            assetLib = "A";
            if (isTrans == "") {
                FillInCascadeDropdown(null, '#ddlTrans', "/OB/PopulateSubTransaction/0$" + assetLib + "$A");
            }
            else {
                FillInCascadeDropdown(null, '#ddlTrans', "/OB/PopulateSubTransaction/0$" + assetLib + "$E");
            }

        }

        //added by abhishek kamble 21-10-2013
        resetOBForm();

    });

    $("#rdoLib").change(function () {
        $("#NARRATION").val("");
        $("#trDetailsHeadDesc").find('td:eq(2)').text("");
        if ($(this).attr('checked') == "checked") {
            $("#ddlSubTrans").empty();
            $("#ddlSubTrans").append("<option value='0'>Select Subtransaction</option>");
            assetLib = "L";
            if (isTrans == "") {
                FillInCascadeDropdown(null, '#ddlTrans', "/OB/PopulateSubTransaction/0$" + assetLib + "$A");
            }
            else {
                FillInCascadeDropdown(null, '#ddlTrans', "/OB/PopulateSubTransaction/0$" + assetLib + "$E");
            }
        }

        //added by abhishek kamble 21-10-2013
        resetOBForm();

    });
    
    $("#ddlTrans").change(function () {
        
        if ($("#ddlTrans").val() != "0") {
            $("#NARRATION").val("");
            $("#trDetailsHeadDesc").find('td:eq(2)').text("");
            if (isTrans == "") {
                FillInCascadeDropdown(null, '#ddlSubTrans', "/OB/PopulateSubTransaction/" + $("#ddlTrans").val() + "$" + assetLib + "$A");
            }
            else {
                FillInCascadeDropdown(null, '#ddlSubTrans', "/OB/PopulateSubTransaction/" + $("#ddlTrans").val() + "$" + assetLib + "$E");
            }
        }
        else {
            $("#ddlSubTrans").empty();
            $("#ddlSubTrans").append("<option value='0' selected=true>Select Sub Transaction</option>");

            //added by koustubh nakate to hide rows as per head selection
            $("#trDetailsHeadDesc").find('td:eq(2)').text("");

            if ($('#trddlContractor').is(':visible')) {
                $('#trddlContractor').hide('slow');
            }
            if ($('#trddlContractorName').is(':visible')) {
                $("#trddlContractorName").hide('slow');
            }
            if ($('#trddlAgreement').is(':visible')) {
                $("#trddlAgreement").hide('slow');
            }
            if ($('#trddlSancYear').is(':visible')) {
                $("#trddlSancYear").hide('slow');
            }
            if ($('#trddlPackage').is(':visible')) {
                $("#trddlPackage").hide('slow');
            }
            if ($('#trddlRoad').is(':visible')) {
                $("#trddlRoad").hide('slow');
            }
            if ($('#trIsFinalPay').is(':visible')) {
                $("#trIsFinalPay").hide('slow');
            }
        }
    });

    $("#ddlSubTrans").change(function () {
        //if ($("#ddlSubTrans option:selected").attr('class') == "X") {
        //    $("#trSubTransInfo").show('slow');
        //}
        //else {
        //    $("#trSubTransInfo").hide('slow');
        //}
        $.ajax({
            url: "/OB/GetDetailsDesignParams/" + $("#ddlSubTrans").val(),
            type: "POST",
            async: false,
            cache: false,
            success: function (data) {

                $("#NARRATION").val("");
                $("#trDetailsHeadDesc").find('td:eq(2)').text("");
                $.ajax({
                    url: "/OB/GetNarration/" +billId,
                    type: "POST",
                    data:
                           {
                               'transId': $("#ddlSubTrans").val()
                           },
                    async: false,
                    cache: false,
                    success: function (data) {
                        $("#NARRATION").val(data.split('$')[0]);
                        $("#trDetailsHeadDesc").find('td:eq(2)').text(data.split('$')[1]);
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert("Error while getting narration for head");
                    }
                });

                con = "N";
                agg = "N";
                piu = "N";
                sancyr = "N";
                pkg = "N";
                road = "N";
                
                if (data.CON_REQ == "Y") {
                    FillInCascadeDropdown(null, '#ddlContractor', "/OB/PopulateContractor/C");
                    $("#trddlContractor").show('slow');

                    //added by koustubh nakate on 22/07/2013 to hide contractor name row
                    $("#trddlContractorName").hide('slow');
                    con = "Y";
                }
                else if (data.SUP_REQ == "Y") {
                    FillInCascadeDropdown(null, '#ddlContractor', "/OB/PopulateContractor/S");
                    $("#trddlContractor").show('slow');

                    //added by koustubh nakate on 22/07/2013 to hide contractor name row
                    $("#trddlContractorName").hide('slow');

                    con = "Y";
                }
                else {
                    $("#ddlContractor").empty();
                    $("#ddlContractor").append("<option value='0' selected=true>Select Contractor</option>");
                    $("#trddlContractor").hide('slow');

                    //added by koustubh nakate on 22/07/2013 to hide contractor name row
                    $("#trddlContractorName").hide('slow');
                }

                if (data.AGREEMENT_REQ == "Y") {
                    //if ($("#ddlContractor").val() != "0") {
                    //    FillInCascadeDropdown(null, '#ddlAgreement', "/OB/PopulateAgreement/" + $("#ddlContractor").val());
                    //}
                    $("#trddlAgreement").show('slow');
                    $("#ddlAgreement").empty();
                    $("#ddlAgreement").append("<option value='0' selected=true>Select Agreement</option>");
                    agg = "Y";
                }
                else {
                    $("#ddlAgreement").empty();
                    $("#ddlAgreement").append("<option value='0' selected=true>Select Agreement</option>");
                    $("#trddlAgreement").hide('slow');
                }

                if (data.SANC_YEAR == "Y") {
                    FillInCascadeDropdown(null, '#ddlSancYear', "/TEO/PopulateSancYear/");
                    $("#trddlSancYear").show('slow');
                    //$("#ddlSancYear").empty();
                    //$("#ddlSancYear").append("<option value='0' selected=true>Select Year</option>");
                    sancyr = "Y";
                }
                else {
                    $("#ddlSancYear").empty();
                    $("#ddlSancYear").append("<option value='0' selected=true>Select Year</option>");
                    $("#trddlSancYear").hide('slow');
                }

                if (data.PKG_REQ == "Y") {
                    $("#trddlPackage").show('slow');
                    pkg = "Y";
                    $("#ddlPackage").empty();
                    $("#ddlPackage").append("<option value='0' selected=true>Select Package</option>");
                }
                else {
                    $("#ddlPackage").empty();
                    $("#ddlPackage").append("<option value='0' selected=true>Select Package</option>");
                    $("#trddlPackage").hide('slow');
                }

                if (data.ROAD_REQ == "Y") {
                    $("#trddlRoad").show('slow');
                    if ($("#ddlSubTrans option:selected").val() == 549 || $("#ddlSubTrans option:selected").val() == 550) {
                        $("#trIsFinalPay").show();
                    }
                    else {
                        $("#trIsFinalPay").hide();
                    }
                    $("#ddlRoad").empty();
                    $("#ddlRoad").append("<option value='0' selected=true>Select Road</option>");
                    road = "Y";
                }
                else {
                    $("#ddlRoad").empty();
                    $("#ddlRoad").append("<option value='0' selected=true>Select Road</option>");
                    $("#trddlRoad").hide('slow');
                    $("#trIsFinalPay").hide();
                }

                if (data.PIU_REQ == "Y") {
                    FillInCascadeDropdown(null, '#ddlDPIU', "/OB/PopulateDPIU/");
                    $("#trddlDPIU").show('slow');
                    piu = "Y";
                }
                else {
                    $("#ddlDPIU").empty();
                    $("#ddlDPIU").append("<option value='0' selected=true>Select DPIU</option>");
                    $("#trddlDPIU").hide('slow');
                }


                //added by koustubh nakate on 19/07/2013 for change label name
                
                var FundType = $('#FundType').val();

                
                if ($("#ddlSubTrans option:selected").val() == 538 && FundType == 'P') {
                    $('#lblNARRATION').text('Payee Name');
                }
                else if (($("#ddlSubTrans option:selected").val() == 995 || $("#ddlSubTrans option:selected").val() == 1121) && FundType == 'A') {
                    
                    $('#lblNARRATION').text('Payee Name');
                }
                else if ($("#ddlSubTrans option:selected").val() == 943 && FundType == 'M') {
                    $('#lblNARRATION').text('Payee Name');
                }
                else {
                    $('#lblNARRATION').text('Narration');
                }

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                unblockPage();
            }
        });
    });

    $("#ddlContractor").change(function(){
        if ($("#ddlContractor").val() != "" && $("#ddlContractor").val() != null) {
            $.ajax({
                url: "/Receipt/GetContractorName/" + $("#ddlContractor").val(),
                type: "POST",
                async: false,
                cache: false,
                success: function (data) {
                    if ($("#ddlAgreement").css('display') != "none") {
                        FillInCascadeDropdown(null, '#ddlAgreement', "/OB/PopulateAgreement/"+$("#ddlContractor").val());
                    }
                    $("#tdContractorName").text(data);
                    $("#trddlContractorName").show('slow');
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                }
            });
        }
        else {
            $("#trddlContractorName").hide('slow');
            $("#tdContractorName").text("");
            $("#ddlAgreement").empty();
            $("#ddlRoad").empty();
            $("#trIsFinalPay").hide();
        }
    });

    $("#ddlSancYear").change(function () {
        if ($("#trddlPackage").css('display') != "none") {
            if ($("#trddlRoad").css('display') != "none") {
                $("#ddlRoad").empty();
            }
            FillInCascadeDropdown(null, '#ddlPackage', "/TEO/PopulatePackage/" + $("#ddlSancYear").val());
        }
    });

    $("#ddlPackage").change(function () {
        if ($("#trddlRoad").css('display') != "none") {
            FillInCascadeDropdown(null, '#ddlRoad', "/TEO/PopulateRoad/0$" + $("#ddlPackage").val() + "$0$" + $("#ddlContractor option:selected").val() + "$" + $("#ddlSubTrans option:selected").val() + "?" + $.param({ AGREEMENT_NUMBER: $("#ddlAgreement option:selected").text() }));
            if ($("#ddlSubTrans option:selected").val() == 549 || $("#ddlSubTrans option:selected").val() == 550) {
                $("#trIsFinalPay").show();
            }
            else { $("#trIsFinalPay").hide(); }
        }
    });

    $("#ddlAgreement").change(function () {
        if ($(this).val() > "0") {
            if ($("#trddlRoad").css('display') != "none") {
                FillInCascadeDropdown(null, '#ddlRoad', "/TEO/PopulateRoad/" + $("#ddlAgreement option:selected").val() + "$0" + "$0$" + $("#ddlContractor option:selected").val() + "$" + $("#ddlSubTrans option:selected").val() + "?" + $.param({ AGREEMENT_NUMBER: $("#ddlAgreement option:selected").text() }));
                if ($("#ddlSubTrans option:selected").val() == 549 || $("#ddlSubTrans option:selected").val() == 550) {
                    $("#trIsFinalPay").show();
                }
                else {
                    $("#trIsFinalPay").hide();
                }
            }
        }
        else {
            //modified by abhishek kamble 22-10-2013
            $("#ddlRoad").empty();
            $('#ddlRoad').append("<option value=0>Select Road</option>");
        }
    });

    $("#ddlRoad").change(function () {
        if ($(this).val() != "0") {
            if ($("#ddlSubTrans option:selected").val() == 549 || $("#ddlSubTrans option:selected").val() == 550 || $("#ddlSubTrans option:selected").val() == 541 || $("#ddlSubTrans option:selected").val() == 542) {

                $.ajax({
                    type: 'POST',
                    url: '/OB/IsFinalPayment/' + billId + "/" + $("#ddlRoad option:selected").val(),
                    async: false,
                    cache: false,
                    success: function (data) {
                        if (data.success == true) {
                            $("#chkIsFinPay").attr('disabled', 'disabled');
                            $("#chkIsFinPay").attr('checked', true);
                        }
                        else {
                            $("#chkIsFinPay").attr('disabled', false);
                            $("#chkIsFinPay").attr('checked', false);
                        }
                    },
                    error: function () { }
                });



                $("#trIsFinalPay").show();
            }
            else {
                $("#trIsFinalPay").hide();
            }
        }
        else {
            $("#chkIsFinPay").attr('checked', false);
            $("#trIsFinalPay").hide();
        }
    });

    $("#btnAddOBDetails").click(function (evt) {

        evt.preventDefault();
        var formStatus = $('#frmAddOBDetails').valid();
        var status = true; //validateAddReceiptDetails($("#AMOUNT").val(), 0);
        if (formStatus && status) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $("#chkIsFinPay").attr('disabled', false);
            var headDesc = $('#tdHeadDesc').html();
            $.ajax({
                url: "/OB/AddOBDetails/" + billId,
                type: "POST",
                async: false,
                cache: false,
                data: $("#frmAddOBDetails").serialize(),
                success: function (data) {
                    $.unblockUI();

                    if (!data.success) {
                        if (data.message == "undefined" || data.message == null) {
                            $("#loadOBDetails").html(data);
                            $('#tdHeadDesc').html(headDesc);
                            $.each($("select"), function () {
                                if ($(this).find('option').length >= 1) {
                                    if ($(this).attr('id') == "ddlContractor") {
                                        $.ajax({
                                            url: "/Receipt/GetContractorName/" + $("#ddlContractor").val(),
                                            type: "POST",
                                            async: false,
                                            cache: false,
                                            success: function (data) {
                                                $("#tdContractorName").text(data);
                                                $("#trddlContractorName").show('slow');
                                            },
                                            error: function (xhr, ajaxOptions, thrownError) {
                                                alert(xhr.responseText);
                                            }
                                        });
                                    }
                                    if ($(this).attr('id') == "ddlRoad") {
                                        if ($("#ddlSubTrans option:selected").val() == 549 || $("#ddlSubTrans option:selected").val() == 550) {
                                            $("#trIsFinalPay").show();
                                        }
                                    }
                                    $('#tr' + $(this).attr('id')).show();
                                }
                            });
                        }
                        else {
                            $("#divOBDetailsError").show("slide");
                            $("#divOBDetailsError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                        }
                        return false;
                    }
                    else {
                        $("#divOBDetailsError").hide("slide");
                        $("#divOBDetailsError span:eq(1)").html('');
                        $("#btnResetOBDetails").trigger('click');
                        LoadGrid();
                        LoadOBDetailsGrid(billId);

                        //new change done by Vikram on 12-09-2013
                        var transNo; 
                        if (assetLib == 'A') {
                            transNo = 1;
                        }
                        else
                        {
                            transNo = 2;
                        }

                        $("#loadOBDetails").load("/OB/AddOBDetails/" + billId + "/" + transNo, function () {

                            $("#divOBChart").html("").hide();
                            $("#tblOBChart").html("").hide();
                            $("#divOBMasterWrapper").hide();
                            $("#loadOBDetails").show('slow');
                            $("#loadOBMaster").hide('slow');
                            $("#btnlblBackToList").show('slow');
                            $("#divOBDetailsWrapper").show();
                            //$("#tblOBList").jqGrid("setGridState", 'hidden');
                            $("#tblOBDetailsGrid").jqGrid("setGridState", 'visible');
                            $("#btnlblBackToList").show('slow');
                            return false;


                        });
                        // end of change
                            
                            
                        $("#trddlContractor").hide('slow');
                        $("trddlContractorName").hide('slow');
                        $("#trddlAgreement").hide('slow');
                        $("#trddlDPIU").hide('slow');
                        //if (assetLib == "A") {
                        //    $("#rdoAsset").attr("checked", "checked");
                        //    $("#rdoAsset").trigger("change");
                        //}
                        //else {
                        //    $("#rdoLib").attr("checked", "checked");
                        //    $("#rdoLib").trigger("change");
                        //}
                        alert("Transaction Details Added.");
                        return false;
                    }

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();

                }
            });
        }
    });

    $("#btnEditOBDetails").click(function (evt) {

        evt.preventDefault();
        var formStatus = $('#frmAddOBDetails').valid();
        var status = true; //validateAddReceiptDetails($("#AMOUNT").val(), 0);
        if (formStatus && status) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            var headDesc = $('#tdHeadDesc').html();
            $("#rdoAsset").attr('disabled', false);
            $("#rdoLib").attr('disabled', false);
            $("#chkIsFinPay").attr('disabled', false);
            $.ajax({
                url: "/OB/AddOBDetails/" + isTrans,
                type: "POST",
                async: false,
                cache: false,
                data: $("#frmAddOBDetails").serialize(),
                success: function (data) {
                    $.unblockUI();

                    if (!data.success) {
                        if (data.message == "undefined" || data.message == null) {
                            $("#loadOBDetails").html(data);
                            $('#tdHeadDesc').html(headDesc);

                            $("#ddlSubTrans > option").each(function () {
                                //alert(this.toString());
                                if (this.text.substring(0, 1) == "$") {
                                    this.text = this.text.substring(1);
                                    $(this).addClass("X");
                                    $(this).css("color", "#b83400");
                                }
                            });
                            $.each($("select"), function () {
                                if ($(this).find('option').length >= 1) {
                                    if ($(this).attr('id') == "ddlContractor") {
                                        $.ajax({
                                            url: "/Receipt/GetContractorName/" + $("#ddlContractor").val(),
                                            type: "POST",
                                            async: false,
                                            cache: false,
                                            success: function (data) {
                                                $("#tdContractorName").text(data);
                                                $("#trddlContractorName").show('slow');
                                            },
                                            error: function (xhr, ajaxOptions, thrownError) {
                                                alert(xhr.responseText);
                                            }
                                        });
                                    }
                                    $('#tr' + $(this).attr('id')).show();
                                }
                            });
                        }
                        else {
                            $("#divOBDetailsError").show("slide");
                            $("#divOBDetailsError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                        }

                        $("#rdoAsset").attr('disabled', true);
                        $("#rdoLib").attr('disabled', true);
                        return false;

                    }
                    else {
                        $("#divOBDetailsError").hide("slide");
                        $("#divOBDetailsError span:eq(1)").html('');
                       // $("#btnCancelOBDetails").trigger('click');
                        LoadOBDetailsGrid(billId);
                        $("#trddlContractor").hide('slow');
                        $("trddlContractorName").hide('slow');
                        $("#trddlAgreement").hide('slow');
                        $("#trddlDPIU").hide('slow');

                        var urlParam = $('#EncryptedParam').val();
                        var transNo = $('#TransNo').val();
                        //alert(urlParam);
                        //alert(transNo);
                        $("#loadOBDetails").load("/OB/AddOBDetails/" + urlParam + "/"+transNo  , function () {
                            //$("#DivIcoOBTrans").trigger('click');
                            $("#divOBChart").html("").hide();
                            $("#tblOBChart").html("").hide();
                            $("#divOBMasterWrapper").hide();
                            $("#loadOBDetails").show('slow');
                            $("#loadOBMaster").hide('slow');
                            $("#btnlblBackToList").show('slow');
                            $("#divOBDetailsWrapper").show();
                            //LoadOBDetailsGrid(urlParam);
                            //$("#tblOBList").jqGrid("setGridState", 'hidden');
                          //  $("#tblOBDetailsGrid").jqGrid("setGridState", 'visible');                    
                        });

                        alert("Transaction Details Updated.");
                        return false;
                    }

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $("#rdoAsset").attr('disabled', true);
                    $("#rdoLib").attr('disabled', true);
                    $.unblockUI();

                }
            });
        }

    });
    
    $("#btnResetOBDetails").click(function () {
        $("#divOBDetailsError").hide("slow");
        $('#tdHeadDesc').html('');
    });

    $("#btnCancelOBDetails").click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $("#loadOBDetails").hide('slow');
        $.unblockUI();

    });

    $("#btnFinalizeOB").unbind().click(function () {

        var finalize = confirm("Are you sure you want to finalize OB Entry?");
        if (finalize)
        {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/OB/FinalizeOB/" + billId,
                type: "POST",
                async: false,
                cache: false,
                success: function (data) {
                    $.unblockUI();

                    if (data.success) {
                        alert("OB Entry Finalize");
                        $("#mainDiv").load("/OB/OpeningBalance/");
                        return false;
                    }
                    else {
                        alert(data.message);
                        return false;
                    }
                }
            });
            $.unblockUI();
        }
        else
        {
            return false;
        }

        return false;
    });

});

function FillInCascadeDropdown(map, dropdown, action) {

    $(dropdown).empty()

    $.post(action, map, function (data) {
        ddvalues = data;
        $.each(data, function () {
            if (this.Text.toString().substring(0,1) == "$") {
                this.Text = this.Text.toString().substring(1);
                $(dropdown).append("<option style='color:#b83400' class='X' value=" + this.Value + ">" + this.Text + "</option>");
            }
            else {
                $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
            }
        });

    }, "json");
}

function EditOBDetails(urlParam)
{
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $("#loadOBDetails").load("/OB/AddOBDetails/" + urlParam, function ()
    {
        $.unblockUI();

        $.each($("select"), function () {
            if ($(this).find('option').length > 0) {
                if ($(this).attr('id') == "ddlRoad") {
                    if ($("#ddlSubTrans option:selected").val() == 549 || $("#ddlSubTrans option:selected").val() == 550 || $("#ddlSubTrans option:selected").val() == 541 || $("#ddlSubTrans option:selected").val() == 542) {
                        if ($("#chkIsFinPay").is(':checked'))
                        {
                            $("#chkIsFinPay").attr('disabled', 'disabled');
                        }
                        $("#trIsFinalPay").show();
                    }
                }
                $('#tr' + $(this).attr('id')).show();
            }
        });
        if ($("#trddlContractor").css('display') != "none")
        {
            $.ajax({
                url: "/Receipt/GetContractorName/" + $("#ddlContractor").val(),
                type: "POST",
                async: false,
                cache: false,
                success: function (data) {
                    $("#tdContractorName").text(data);
                    $("#trddlContractorName").show('slow');
                    $.unblockUI();

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                }
            });
        }
        $("#loadOBDetails").show('slow');
        
        $("#rdoAsset").attr('disabled', true);
        $("#rdoLib").attr('disabled', true);
 
        //get the narration for the head
       
        $("#trDetailsHeadDesc").find('td:eq(2)').text("");
        $.ajax({
            url: "/OB/GetNarration/" + billId,
            type: "POST",
            data:
                   {
                       'transId': $("#ddlSubTrans").val()
                   },
            async: false,
            cache: false,
            success: function (data) {
               
                $("#trDetailsHeadDesc").find('td:eq(2)').text(data.split('$')[1]);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("Error while getting narration for head");
            }
        });


      //  $("#ddlSubTrans").trigger('change');
    });
 

}

function DeleteOBDetails(urlParam)
{

    if (confirm("Are you sure you want to delete OB details?")) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/OB/DeleteOBDetails/" + urlParam,
            type: "POST",
            async: false,
            cache: false,
            success: function (data) {
                $.unblockUI();

                if (data.success) {
                    LoadOBDetailsGrid(billId);
                    alert("Opening Balance Transaction Details Deleted");
                    if ($('#btnEditOBDetails').is(':visible')) {
                        $("#loadOBDetails").hide('slow');
                    }
                    
                    //new change done by Vikram as the validation fail if we keep the add view as it is after deleting the entry from details list 
                    $("#loadOBDetails").load("/OB/AddOBDetails/" + billId + "/" + "1", function () {
                        $.each($("select"), function () {
                            if ($(this).find('option').length > 0) {
                                if ($(this).attr('id') == "ddlRoad") {
                                    if ($("#ddlSubTrans option:selected").val() == 549 || $("#ddlSubTrans option:selected").val() == 550) {
                                        $("#trIsFinalPay").show();
                                    }
                                }
                                $('#tr' + $(this).attr('id')).show();
                            }
                        });
                        if ($("#trddlContractor").css('display') != "none") {
                            $.ajax({
                                url: "/Receipt/GetContractorName/" + $("#ddlContractor").val(),
                                type: "POST",
                                async: false,
                                cache: false,
                                success: function (data) {
                                    $("#tdContractorName").text(data);
                                    $("#trddlContractorName").show('slow');
                                },
                                error: function (xhr, ajaxOptions, thrownError) {
                                    alert(xhr.responseText);
                                }
                            });
                        }
                        $("#loadOBDetails").show('slow');
                        LoadGrid();
                    });
                    $.unblockUI();

                    return false;
                }
                else {
                    alert(data.message);
                    return false;
                }
            }
        });
    }
    else {

        return false;
    }
}

function resetOBForm()
{
    $('#AMOUNT').val('');

    if ($('#trddlContractor').is(':visible'))
    {
        $('#ddlContractor option:first').attr('selected', 'selected');
        $("#trddlContractor").hide('slow');     
    }
    
    if ($('#trddlDPIU').is(':visible')) {
        $('#ddlDPIU option:first').attr('selected', 'selected');
        $("#trddlDPIU").hide('slow');

    }

    if ($('#trddlContractorName').is(':visible')) {
        $("#trddlContractorName").hide('slow');
    }
    
    if ($('#trddlAgreement').is(':visible')) {
        $('#ddlAgreement option:first').attr('selected', 'selected');
        $("#trddlAgreement").hide('slow');
    }

    if ($('#trddlSancYear').is(':visible')) {
        $('#ddlSancYear option:first').attr('selected', 'selected');
        $("#trddlSancYear").hide('slow');
    }

    if ($('#trddlPackage').is(':visible')) {
        $('#ddlPackage option:first').attr('selected', 'selected');
        $("#trddlPackage").hide('slow');
    }

    if ($('#trddlRoad').is(':visible')) {
        $('#ddlRoad option:first').attr('selected', 'selected');
        $("#trddlRoad").hide('slow');
    }

    if ($('#trIsFinalPay').is(':visible')) {
        $("#trIsFinalPay").hide('slow');
    }
}