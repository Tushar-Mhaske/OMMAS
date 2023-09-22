var con = "N";
var agg = "N";
var dpiu = "N";
var sup = "N";
var road = "N";

var conD = "N";
var aggD = "N";
var supD = "N";
var mtxnD = "N";
var cashchqD = "N";
var BillID;

//var IsDPIURequired = true;

$(function () {

    $.validator.unobtrusive.adapters.add('isdropdownselected', ['parentdropdown'], function (options) {
        options.rules['isdropdownselected'] = options.params;
        options.messages['isdropdownselected'] = options.message;
    });


    //If Condition Added By Abhishek kamble to remove Please select DPIU validation for txn 1210 and 1542    
    if (((parseInt(($("#ddlTransDetails option:selected").val().split('$')[0])) != 1210) || (parseInt(($("#ddlTransDetails option:selected").val().split('$')[0])) == 1542))) {        
        $.validator.addMethod("isdropdownselected", function (value, element, params) {
            var parentVal = $('#' + params.parentdropdown + '').val();
            if (value == "0") {
                return true;
            }
            else if (value == "") {
                return false;
            }
             return true; 
        });
    }
    //$.validator.unobtrusive.adapters.add('iseditabletrans', ["obtype"], function (options) {
    //    options.rules['iseditabletrans'] = options.params;
    //});

    //$.validator.addMethod("iseditabletrans", function (value, element, params) {
        
    //    if ($("#ddlTransDetails option:selected").attr('class') == "X") {
    //        return false;
    //    }
    //    else {
    //        return true;
    //    }
    //    return true;
    //}, function (params, element) {
    //    return "Invalid Sub Transaction for OB Entry";
    //});

    //added by Koustubh Nakate on 11/10/2013 for imprest voucher validation

    jQuery.validator.addMethod("isimprestrequired", function (value, element) {
 
        if (value == 0) {
            return false;
        }
        else {
            return true;
        }

    }, "");

});

$(document).ready(function () {

    
    //Added By Abhishek kamble 1-jan-2014
    if (($("#AMOUNT").val() == "") && IsMultiTranAllowed=="N")
    {
        $("#AMOUNT").val(ReceiptGrossAmount);
    }

    var contractorName = '';
    var agreement = '';
    var oldNarration = '';
    var newNarration = '';
    var narrationAfterContractor = '';

    var isImprestRequired = false;


    var selTransId = $("#ddlTransDetails").val();
   // alert(selTransId);
    if (selTransId != "" && selTransId != "0" && $('#NARRATION').val()=='')//changes by koustubh nakate on 18/07/2013 for narration edit
    {
        $("#NARRATION").val("");
        $("#trDetailsHeadDesc").find('td:eq(2)').text("");
        $.ajax({
            url: "/Receipt/GetNarration/" + $("#tblMasterReceiptList").getDataIDs()[0],
            type: "POST",
            async: false,
            cache: false,
            data:
                {
                    'transId': $("#ddlTransDetails").val().split('$')[0]
                },
            success: function (data) {
                $("#NARRATION").val(data.split('$')[0]);
                $("#trDetailsHeadDesc").find('td:eq(2)').text(data.split('$')[1]);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("Error while processing request");
            }
        });
    }
    if (selTransId == "179$C" || selTransId == "1191$C" || selTransId == "1190$C" || selTransId == "572$C") { //changes 472 to 572 by Koustubh Nakate
        $("#trImprest").show();
    }
    else {
        $("#trImprest").hide();
    }


    if (masterBillID == null) {
        masterBillID = $("#tblMasterReceiptList").getDataIDs()[0];
    }
    
    
    //setTimeout(function () {
        $.ajax({
            url: "/Receipt/GetMasterDesignParamsByBillId/" + masterBillID,//$("#tblMasterReceiptList").getDataIDs()[0],
            type: "POST",
            async: false,
            cache: false,
            success: function (data) {
                conD = "N";
                aggD = "N";
                supD = "N";
                mtxnD = "N";
                cashchqD = "N";

                if (data.CON_REQ == "Y") {
                    conD = "Y";
                }
                if (data.AGREEMENT_REQ == "Y") {
                    aggD = "Y";
                }
                if (data.SUP_REQ == "Y") {
                    supD = "Y";
                }
                if (data.MTXN_REQ == "Y") {
                    mtxnD = "Y";
                }
                cashchqD = data.CASH_CHQ == "Y";
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                unblockPage();
            }
        });
    //}, 200);

    $("#ddlTransDetails > option").each(function () {
       if (this.text.substring(0, 1) == "$") {
            this.text = this.text.substring(1);
            $(this).addClass("X");
            $(this).css("color", "#b83400");
        }
    });

    $("#ddlTransDetails").change(function () {
        
        //alert('a');

        if ($(this).val() != 0) {
            var ddlTrans = $(this).val().split('$')[0]; 
            if (ddlTrans == 179 || ddlTrans == 572 || ddlTrans == 1191 || ddlTrans == 1190) { //changes by koustubh Nakate on 11/10/2013
                $("#trImprest").show();
                isImprestRequired = true;
            }
            else {
                $("#trImprest").hide();
                isImprestRequired = false;
            }
            con = "N";
            agg = "N";
            dpiu = "N";
            sup = "N";
            road = "N";
            $.ajax({
                url: "/Receipt/GetDetailsDesignMode/"+ $("#ddlTransDetails").val(),
                type: "POST",
                async: false,
                cache: false,
                success: function (data) {

                  
                    $("#trDetailsHeadDesc").find('td:eq(2)').text("");
                    $.ajax({
                        url: "/Receipt/GetNarration/" + $("#tblMasterReceiptList").getDataIDs()[0],
                        type: "POST",
                        async: false,
                        cache: false,
                        data:
                            {
                                'transId' : $("#ddlTransDetails").val().split('$')[0]
                            },
                        success: function (data) {
                            if ($('#btnAddReceiptDetails').is(':visible')) {
                             
                                $("#NARRATION").val("");
                                $("#NARRATION").val(data.split('$')[0]);
                            }
                            oldNarration = data.split('$')[0];//$("#NARRATION").val();
                            $("#trDetailsHeadDesc").find('td:eq(2)').text(data.split('$')[1]);
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            alert("Error while processing request");
                        }
                    });

                    if (data.CON_REQ == "Y") {
                        if (isTrans == "Y" || isTrans == "N") {
                            $("#ddlContractor").val("0");
                            $("#ddlContractor").trigger('change');
                        }
                        con = "Y";
                        $("#trContractor").show('slow');
                    }
                    else {
                        $("#trContractor").hide('slow');
                        $("trContractorName").hide('slow');
                    }

                    if (data.AGREEMENT_REQ == "Y") {
                        if (isTrans == "Y" || isTrans == "N") {
                            $("#ddlContractor").val("0");
                        }
                        agg = "Y";
                        $("#trAgreement").show('slow');
                    }
                    else {
                        $("#trAgreement").hide('slow');
                    }
                    if (data.PIU_REQ == "Y") {
                        dpiu = "Y";
                        $("#trDPIU").show('slow');
                        //new change done by Vikram on 08 Jan 2014
                        //Modified by Abhishek kamble to populate PIU for 1542
                        if ((ddlTrans == 1210) || (ddlTrans == 1542)) {
                                FillInCascadeDropdown(null, "#ddlDPIU", "/Payment/GetPIU/" + masterBillID);
                            }
                            else {
                                FillInCascadeDropdown(null, "#ddlDPIU", "/Receipt/GetPIU/" + masterBillID);
                            }
                       
                        //end of change
                        
                    }
                    else {
                        $("#trDPIU").hide('slow');
                    }
                    if (data.SUP_REQ == "Y")
                    {
                        sup = "Y";
                    }
                    if (data.ROAD_REQ == "Y") {
                        road = "Y";
                    }
                    return false;
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                }
            });
        }
        else {
            $("#trImprest").hide();
            $("#NARRATION").val("");
            $("#trDetailsHeadDesc").find('td:eq(2)').text("");
            $("#trContractor").hide('slow');
            $("trContractorName").hide('slow');
            $("#trAgreement").hide('slow');
            $("#trDPIU").hide('slow');
        }
    });

    $("#ddlUnSettledVouchers").change(function () {
        if ($(this).val() != "0") {
            $.ajax({
                url: "/Receipt/GetImprestAmount/" + $("#ddlUnSettledVouchers").val(),
                type: "POST",
                async: false,
                cache: false,
                success: function (data) {
                    $("#AMOUNT").val(data);
                    
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                }
            });
        }
        else {
            $("#AMOUNT").val("");
        }
    });

  
    $("#ddlContractor").change(function () {       
        var contId = 0;
        if ($(this).val() != null || $(this).val() != undefined)
        {
            contId = $(this).val();
        }
        if (contId != 0) {
            $.ajax({
                url: "/Receipt/GetContractorName/" + contId,
                type: "POST",
                async: false,
                cache: false,
                success: function (data) {
                    //$("#trContractorName").find("td:eq(2)").html(data);
                    if ($("#ddlContractor").val() == "0") {
                        $("#trContractorName").hide();
                        $("#tdContractorName").text('');
                    }
                    else {
                        contractorName = data;
                        $("#tdContractorName").text(data);
                        $("#trContractorName").show('slow');

                       

                       // newNarration = $("#NARRATION").val();
                        newNarration = oldNarration + ' M/S ' + contractorName;
                        narrationAfterContractor = newNarration;
                        $("#NARRATION").val('');
                        $("#NARRATION").val(newNarration);
                    }
                    FillInCascadeDropdown(null, '#ddlAgreement', "/Receipt/PopulateAgreement/" + $("#ddlContractor").val());
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                }
            });
        }
        else {
            $("#ddlAgreement").empty();
            $("#ddlAgreement").append("<option value='0' selected=true>Select Agreement</option>");

            $("#NARRATION").val(oldNarration);
        }
    });

    //added by koustubh nakate on 18/07/2013 for narration generation

    $("#ddlAgreement").change(function () {
        
        if ($("#ddlAgreement option:selected").val() > 0) {
            agreement = $("#ddlAgreement option:selected").text();

            newNarration = $("#NARRATION").val();
            newNarration = narrationAfterContractor + ' against ' + agreement;
            $("#NARRATION").val('');
            $("#NARRATION").val(newNarration);
        }
        else {
            $("#NARRATION").val('');
            $("#NARRATION").val(narrationAfterContractor);
        }

    });

    if (isTrans == "Y")
    {
        //LoadDetailsReceiptGrid($("#tblMasterReceiptList").getDataIDs()[0]);

        LoadDetailsReceiptGrid(masterBillID);
    }
    else if (isTrans.split('/').length != 3)
    {
        $("#ddlTransDetails").trigger('change');
    }
    
    $.validator.unobtrusive.parse($('#frmAddReceiptDetails'));

    //$("#btnAddReceiptDetails").click(function (evt) {
        $("#btnAddReceiptDetails").click(function (evt) {    
       // evt.preventDefault();

        if (isImprestRequired == true) {
            $('#ddlUnSettledVouchers').rules('add', {
                isimprestrequired: true,
                messages: {
                    isimprestrequired: 'Unsettled Imprest Voucher is Required'
                }
            });
        }
        else {
            $('#ddlUnSettledVouchers').rules("remove", "messages");
        }


        var formStatus = $('#frmAddReceiptDetails').valid();
        var status = validateAddReceiptDetails($("#AMOUNT").val(), 0);

       


        if (formStatus && status) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
     
            $.ajax({
                url: "/Receipt/AddReceiptDetails/"+ $("#tblMasterReceiptList").getDataIDs()[0] ,
                type: "POST",
                async: false,
                cache: false,
                data: $("#frmAddReceiptDetails").serialize(),
                success: function (data) {
                    $.unblockUI();

                    if (!data.success) {
                        if (data.message == "undefined" || data.message == null) {
                            $("#loadReceiptDetails").html(data);
                        }
                        else {
                            $("#divReceiptDetailsError").show("slide");
                            $("#divReceiptDetailsError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                        }
                        return false;
                    }
                    else {

                      
                        BillID = data.encBillId;
                        $("#divReceiptDetailsError").hide("slide");
                        $("#divReceiptDetailsError span:eq(1)").html('');
                        $("#btnResetReceiptDetails").trigger('click');
                        LoadDetailsReceiptGrid(data.encBillId);
                       // LoadDetailsReceiptGrid($("#tblMasterReceiptList").getDataIDs()[0]);
                        $("#trContractor").hide('slow');
                        $("trContractorName").hide('slow');
                        $("#trAgreement").hide('slow');
                        $("#trDPIU").hide('slow');
                        $('#trImprest').hide('slow');
                        alert("Receipt Details added.");
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

    $("#btnEditReceiptDetails").click(function (evt) {
        evt.preventDefault();

        if (isImprestRequired == true) {
            $('#ddlUnSettledVouchers').rules('add', {
                isimprestrequired: true,
                messages: {
                    isimprestrequired: 'Unsettled Imprest Voucher is Required'
                }
            });
        }
        else {
            $('#ddlUnSettledVouchers').rules("remove", "messages");
        }

        var formStatus = $('#frmAddReceiptDetails').valid();
        var status = validateAddReceiptDetails($("#AMOUNT").val(), amountVal);
        if (formStatus && status) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/Receipt/EditReceiptDetails/" + isTrans,
                type: "POST",
                async: false,
                cache: false,
                data: $("#frmAddReceiptDetails").serialize(),
                success: function (data) {
                    $.unblockUI();

                    if (!data.success) {
                        if (data.message == "undefined" || data.message == null) {
                            $("#loadReceiptDetails").html(data);
                            $("#divReceiptDetailsError").hide("slide");
                            $("#divReceiptDetailsError span:eq(1)").html('');
                        }
                        else {
                            $("#divReceiptDetailsError").show("slide");
                            $("#divReceiptDetailsError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                        }
                        return false;
                    }
                    else {
                        $("#divReceiptDetailsError").hide("slide");
                        $("#divReceiptDetailsError span:eq(1)").html('');
                        //$("#loadAddEditReceiptTrans").html(data);
//commented by abhishek kamble 11-dec-2013
 //                       $("#btnCancelReceiptDetails").trigger('click');
                        LoadDetailsReceiptGrid($("#tblMasterReceiptList").getDataIDs()[0]);
                        $("#trContractor").hide('slow');
                        $("trContractorName").hide('slow');
                        $("#trAgreement").hide('slow');
                        $("#trDPIU").hide('slow');
                        $('#trImprest').hide('slow');
                        alert("Receipt Details updated.");
                        //added by abhishek kamble 11-dec-2013
                        $("#btnCancelReceiptDetails").trigger('click');
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

    $("#btnCancelReceiptDetails").click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        if ($("#mulTxnMsg").css('display') != "none") {
            $("#loadReceiptDetails").html("");
        }
        else {
            $("#loadReceiptDetails").load("/Receipt/ReceiptDetails/" + $("#tblMasterReceiptList").getDataIDs()[0]);
        }
        $.unblockUI();
    });

    $("#btnResetReceiptDetails").click(function () {
       // $("#trContractorName").hide();

      //  alert('a');

        $("#tdContractorName").text('');
        $("#divReceiptDetailsError").hide("slide");
        $("#divReceiptDetailsError span:eq(1)").html('');
        $(".field-validation-error").each(function () {
           $(this).html("");
        });
        $("#trDetailsHeadDesc").find('td:eq(2)').text('');

        $("#trContractor").hide('slow');
        $("trContractorName").hide('slow');
        $("#trAgreement").hide('slow');
        $("#trDPIU").hide('slow');
        $('#trImprest').hide('slow');

    });
    

    //new change done by Vikram as the above ajax call was getting id as undefined
    //setTimeout(function () {
    //    $.ajax({
    //        url: "/Receipt/GetMasterDesignParamsByBillId/" + billId,//$("#tblMasterReceiptList").getDataIDs()[0], //,
    //        type: "POST",
    //        async: false,
    //        cache: false,
    //        success: function (data) {
    //            conD = "N";
    //            aggD = "N";
    //            supD = "N";
    //            mtxnD = "N";
    //            cashchqD = "N";

    //            if (data.CON_REQ == "Y") {
    //                conD = "Y";
    //            }
    //            if (data.AGREEMENT_REQ == "Y") {
    //                aggD = "Y";
    //            }
    //            if (data.SUP_REQ == "Y") {
    //                supD = "Y";
    //            }
    //            if (data.MTXN_REQ == "Y") {
    //                mtxnD = "Y";
    //            }
    //            cashchqD = data.CASH_CHQ == "Y";
    //        },
    //        error: function (xhr, ajaxOptions, thrownError) {
    //            alert(xhr.responseText);
    //            unblockPage();
    //        }
    //    });
    //}, 200);



});


function FillInCascadeDropdown(map, dropdown, action) {

    $(dropdown).empty()

    $.post(action, map, function (data) {
        ddvalues = data;
        $.each(data, function () {
            //new change done by Vikram on 08 Jan 2014
            if (this.Selected == true) {
                $(dropdown).append("<option selected value=" + this.Value + ">" + this.Text + "</option>");
            }
            else {

                //end of change

                $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
            }
        });

    }, "json");
}


function validateAddReceiptDetails(amountToValidate,oldValue)
{
    var statusFlag = true;
    var masterRow = $("#tblMasterReceiptList").getRowData($("#tblMasterReceiptList").getDataIDs()[0]);
    var transName = masterRow['TransactionName'];

    $("#divReceiptDetailsError").hide("slide");
    $("#divReceiptDetailsError span:eq(1)").html('');

    if (oldValue == 0 && jQuery('#tblDetailsReceiptList').jqGrid('getGridParam', 'reccount') > 0 && mtxnD == "N") {
        $("#divReceiptDetailsError span:eq(1)").html('<strong>Alert: </strong>Multiple Transaction Entry Prohibited for Master Transaction \'' + transName + '\'');
        $("#divReceiptDetailsError").show("slide");
        statusFlag = false;
        return false;
    }
    else {
        var conId = 0;
        var aggId = 0;
        var deptId = 0;

        if (con == "Y" || agg == "Y" || dpiu == "Y") {
            $.ajax({
                url: "/Receipt/GetTransactionDetails/" + $("#tblMasterReceiptList").getDataIDs()[0],
                type: "POST",
                async: false,
                cache: false,
                success: function (data) {
                    
                    if (data.Con != null) {
                        conId = data.Con;
                    }
                    if (data.Agg != null) {
                        aggId = data.Agg;
                    }
                    if (data.Dept != null) {
                        deptId = data.Dept;
                    }                    
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                }
            });
        }
        
        if (con == "Y") {
            if ($("#ddlContractor").val() == "0") {
                $("#ddlContractor").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlContractor">Contractor Required</span>').show();
                $("#ddlContractor").addClass("input-validation-error");
                statusFlag = false;
            }
            //else if (conId != 0 && $("#ddlContractor option[value=" + conId + "]").length > 0) {
            //    $("#ddlContractor").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlContractor">Please select \'' + $("#ddlContractor option[value=" + conId + "]").text() + '\'</span>').show();
            //    $("#ddlContractor").addClass("input-validation-error");
            //    statusFlag = false;

            //}
            //else if (conId != 0 && $("#ddlContractor").val() != conId) {
            //    $("#ddlContractor").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlContractor">Please select appropriate Company Name</span>').show();
            //    $("#ddlContractor").addClass("input-validation-error");
            //    statusFlag = false;
            //}
        }

        if (agg == "Y") {
            if ($("#ddlAgreement").val() == "0") {
                $("#ddlAgreement").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlAgreement">Agreement Required</span>').show();
                $("#ddlAgreement").addClass("input-validation-error");
                statusFlag = false;
            }
            //else if (aggId != 0 && $("#ddlAgreement option[value=" + aggId + "]").length > 0) {
            //    $("#ddlAgreement").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlAgreement">Please select \'' + $("#ddlAgreement option[value=" + aggId + "]").text() + '\'</span>').show();
            //    $("#ddlAgreement").addClass("input-validation-error");
            //    statusFlag = false;
            //}
            //else if (aggId != 0 && $("#ddlAgreement").val() != aggId) {
            //    $("#ddlAgreement").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlAgreement">Please select appropriate Aggrement</span>').show();
            //    $("#ddlAgreement").addClass("input-validation-error");
            //    statusFlag = false;
            //}
        }

        if (dpiu == "Y") {

            //If Condition Added By Abhishek kamble to remove Please select DPIU validation for txn 1210 and 1542            
            if (((parseInt(($("#ddlTransDetails option:selected").val().split('$')[0])) != 1210) && (parseInt(($("#ddlTransDetails option:selected").val().split('$')[0])) != 1542)))
            {
                if ($("#ddlDPIU").val() == "0") {
                    $("#ddlDPIU").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlDPIU">PIU Required</span>').show();
                    $("#ddlDPIU").addClass("input-validation-error");
                    statusFlag = false;
                }
            }
            //else if (deptId != 0 && $("#ddlDPIU option[value=" + dpiu + "]").length > 0) {
            //    $("#ddlDPIU").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlDPIU">Please select \'' + $("#ddlDPIU option[value=" + deptId + "]").text() + '\'</span>').show();
            //    $("#ddlDPIU").addClass("input-validation-error");
            //    statusFlag = false;
            //}
            //else if (deptId != 0 && $("#ddlDPIU").val() != deptId) {
            //    $("#ddlDPIU").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="ddlDPIU">Please select appropriate Deparment</span>').show();
            //    $("#ddlDPIU").addClass("input-validation-error");
            //    statusFlag = false;
            //}
        }
        

        masterAmount = $("#tblMasterReceiptList").jqGrid('getCell', $("#tblMasterReceiptList").getDataIDs()[0], 'GrossAmount');
        var detailsAmount = 0;

        if (isDetailsGridLoaded) {
            detailsAmount = parseFloat($("#tblDetailsReceiptList").jqGrid('getCol', 'Amount', false, "sum") - oldValue) + parseFloat(amountToValidate);
        }

        if (parseFloat(masterAmount) < detailsAmount) {
            $("#AMOUNT").parent('td').find('span').addClass('field-validation-error').html('<span class="" for="AMOUNT">Total Details Amount Exceeds Master Amount</span>').show();
            $("#AMOUNT").addClass("input-validation-error");
            statusFlag = false;
        }
    }
    return statusFlag;
}