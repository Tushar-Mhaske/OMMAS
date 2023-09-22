
var diffChqAmount = 0
var DiffCachAmount = 0
var DiffDedAmount = 0
var DiffGrossAmount = 0
var ChqAmountToEdit = 0;
var CashAmountToEdit = 0;
var dedAmountToEdit = 0;
var TotalAmtToEnterDedAmount = 0;
var TotalAmtToEnterChqAmount = 0;
var OnlyCash;//variable to check if transactionis of cash or deduction cash
var authTransEdit = false; //keep track of whether edit operation of transaction


$(document).ready(function () {
   
    blockPage();

    $('#HideShowTransaction').click(function () {
       
        $("#TransactionForm").toggle('slow', function () { });

        $('#iconSpan').toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
      
        
    });


    if (opeartion == "A") {

        blockPage();
          

        loadAuthorizationGrid(null);

        $('#AuthMasterEntryDiv').load('/Authorization/GetAddMasterAuthorization/' + month + '$' + year + "$" + opeartion, function () {
             
            $('#btnSubmit').show();
            $('#MasterGrid').hide();
            $('#AuthBalance').hide();
            $('#trShowHideLinkTable').hide();
                        
            unblockPage();
        });
    }
    else if (opeartion == "E") {

        blockPage();

        //load the atuhorization request
        loadAuthorizationGrid(Bill_id);

        blockPage();
        //get amount balances
        GetAuthorizationAmountDetails(Bill_id);

        blockPage();

        $('#AuthMasterEntryDiv').load('/Authorization/GetAddMasterAuthorization/' + month + '$' + year + "$" + opeartion, function () {

            blockPage();

            $('#btnSubmit').show();
            $('#MasterGrid').show();
            $('#AuthBalance').show();
            $('#MasterDataEntryForm').hide();

            //hide the transaction form
           // $("#TransactionForm").hide();

           
           

            unblockPage();

        });

       
    }

});//doc ready


//function to calculate the amounts of authorization
function GetAuthorizationAmountDetails(param_Auth_id) {

    blockPage();
    $.ajax({
        type: "POST",
        url: "/Authorization/GetAuthorizationAmountBalance/" + param_Auth_id,
        // async: false,
        error: function (xhr, status, error) {
            unblockPage();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            return false;
        },
        success: function (data) {
            unblockPage();
            $('#divError').hide('slow');
            $('#errorSpan').html("");
            $('#errorSpan').hide();

            if (data.Success == true) {


                $("#TotalAmtToEnterChqAmount").text(parseFloat(data.TotalAmtToEnterChqAmount).toFixed(2));
                $("#TotalAmtToEnterCachAmount").text(parseFloat(data.TotalAmtToEnterCachAmount).toFixed(2));
                $("#TotalAmtToEnterDedAmount").text(parseFloat(data.TotalAmtToEnterDedAmount).toFixed(2));
                $("#TotalAmtToEnterGrossAmount").text(parseFloat(data.TotalAmtToEnterGrossAmount).toFixed(2));


                $("#TotalAmtEnteredChqAmount").text(parseFloat(data.TotalAmtEnteredChqAmount).toFixed(2));
                $("#TotalAmtEnteredCachAmount").text(parseFloat(data.TotalAmtEnteredCachAmount).toFixed(2));
                $("#TotalAmtEnteredDedAmount").text(parseFloat(data.TotalAmtEnteredDedAmount).toFixed(2));
                $("#TotalAmtEnteredGrossAmount").text(parseFloat(data.TotalAmtEnteredGrossAmount).toFixed(2));

                $("#DiffChqAmount").text(parseFloat(data.DiffChqAmount).toFixed(2));
                $("#DiffCachAmount").text(parseFloat(data.DiffCachAmount).toFixed(2));
                $("#DiffDedAmount").text(parseFloat(data.DiffDedAmount).toFixed(2));
                $("#DiffGrossAmount").text(parseFloat(data.DiffGrossAmount).toFixed(2));


                diffChqAmount = parseFloat(data.DiffChqAmount).toFixed(2);
                DiffCachAmount = parseFloat(data.DiffCachAmount).toFixed(2);
                DiffDedAmount = parseFloat(data.DiffDedAmount).toFixed(2);
                DiffGrossAmount = parseFloat(data.DiffGrossAmount).toFixed(2);

                TotalAmtToEnterDedAmount = parseFloat(data.TotalAmtToEnterDedAmount).toFixed(2);
                TotalAmtToEnterChqAmount = parseFloat(data.TotalAmtToEnterChqAmount).toFixed(2);
                OnlyCash = data.CashPayment;


                //if cash amount in master payment is 0 make cash readonly
                if (data.TotalAmtToEnterDedAmount == 0) {
                    $("#AMOUNT_C").val(0).prop("readonly", 'readonly');
                    $("#cashAmtTr").hide();
                }
                else {
                    $("#cashAmtTr").show();
                    $("#AMOUNT_C").val(0).removeAttr("readonly");
                }

                //if no more amount to enter

                if (data.DiffGrossAmount == 0 && data.DiffChqAmount == 0 & data.DiffDedAmount == 0 && data.DiffGrossAmount == 0) {


                    if (data.VoucherFinalized == "N") {       //show finalize 
                        $('#tblFinalize').show('slow')
                    }

                    $('#AuthDetailsEntryDiv').hide();
                } else {
                    $('#tblFinalize').hide('slow');
                    $('#AuthDetailsEntryDiv').show('slow');
                }

                //if all amounts to entered is 0,i.e. no amount is entered yet then enable the agreement /payment head (if multiple trans is no for master ) to add 
                if (data.TotalAmtEnteredChqAmount == 0 && data.TotalAmtEnteredCachAmount == 0 && data.TotalAmtEnteredDedAmount == 0) {
                    if ($("#TXN_ID") != null) {
                        $("#TXN_ID").removeAttr('disabled');
                    }

                    if ($("#IMS_AGREEMENT_CODE_S") != null) {
                        $("#IMS_AGREEMENT_CODE_S").removeAttr('disabled');
                    }
                    if ($("#IMS_AGREEMENT_CODE_C") != null) {
                        $("#IMS_AGREEMENT_CODE_C").removeAttr('disabled');
                    }

                    if ($("#IMS_AGREEMENT_CODE_DED") != null && data.TotalAmtToEnterChqAmount == 0) {
                        $("#IMS_AGREEMENT_CODE_DED").removeAttr('disabled');
                    }

                    if ($("#HEAD_ID_P") != null) {
                        $("#HEAD_ID_P").val(0);
                        $("#HEAD_ID_P").removeAttr('disabled');
                    }


                }
                //for cash transacton 

            }

            else {

                alert("Error while Getting remaining amount to enter ");
                return false;
            }
        }
    }); //end of ajax

}

//function to load the master authorization grid
function loadAuthorizationGrid(urlparam) {
    jQuery("#AuthorizationList").jqGrid({

        url: '/Authorization/ListAuthorizationRequestForDataEntry/' + urlparam,
        datatype: 'json',
        mtype: 'POST',
        height: 'auto',
        rowNum: 15,
        rownumbers: true,
        //width: 1150,
        autowidth: true,
        pginput: false,
        pgbuttons: false,
        //shrinkToFit: false,
        rowList: [15, 20, 30],
        colNames: ['Authorization Number ', 'Authorization Date','Transaction Type',"Contractor Name ",  'Amount (In Rs.)', 'Cash Amount (In Rs.)', 'Gross Amount (In Rs.)',  'Edit', 'Delete'],
        colModel: [

            {
                name: 'Auth_Number',
                index: 'Auth_Number',
                width: 50,
                align: "center",
                frozen: true

            },
            {
                name: 'Auth_date',
                index: 'Auth_date',
                width: 80,
                align: "center",
                frozen: true,

            },
             {
                 name: 'Trans_type',
                 index: 'Trans_type',
                 width: 120,
                 align: "left",
                 frozen: true,

             },
              {
                  name: 'ContractorName',
                  index: 'ContractorName',
                  width: 120,
                  align: "left",
                  frozen: true,

              },

              {
                  name: 'ChequeAmount',
                  index: 'ChequeAmount',
                  width: 100,
                  align: "right"

              },
            {
                name: 'CashAmount',
                index: 'CashAmount',
                width: 80,
                align: "right"

            },
            {
                name: 'GrossAmount',
                index: 'GrossAmount',
                width: 80,
                align: "right"

            },

          

              {
                  name: 'Edit',
                  index: 'Edit',
                  width: 40,
                  align: "Center"

              },
               {
                   name: 'Delete',
                   index: 'Delete',
                   width: 50,
                   align: "Center"

               }


        ],
        pager: "#MasterAuthpager",
        viewrecords: true,
        loadError: function (xhr, st, err) {
            unblockPage();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            return false;
        },
        loadComplete: function (xhr, st, err) {            
            $("#AuthorizationList").jqGrid('setLabel', "rn", "Sr.</br> No");

            unblockPage();
            payGridWidth = $("#gview_AuthorizationList").width();
            $('#PaymentGridDivList').jqGrid('setGridWidth', payGridWidth);

        },
        sortname: 'Auth_Number',
        sortorder: "asc",
        caption: "Authorization Request Details"
    });


    //jQuery("#").jqGrid('setFrozenColumns');
    jQuery("#AuthorizationList").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [
          { startColumnName: 'Edit', numberOfColumns: 2, titleText: 'Action' }

        ]
    });

}

//function to load the transaction details grid
function loadPaymentGrid(param_Bill_ID) {   
    blockPage();
    jQuery("#PaymentGridDivList").jqGrid({

        url: '/Authorization/GetPaymentDetailList/' + param_Bill_ID,
        datatype: 'json',
        mtype: 'POST',
        height: 'auto',
        rowNum: 1000,
        //width:$("#gview_PaymentMasterList").width(),
        autowidth: true,
        pginput: false,
        pgbuttons: false,
        //shrinkToFit: false,
        //rowList: [15, 20, 30],
        colNames: ['Payment/Deduction', 'Tr.No', 'Head Code', 'Transaction type', 'Contractor Name' ,'Agreement', 'Road', 'Cash/Cheque', 'Amount (In Rs.)', 'Narration', 'Edit', 'Delete'],
        colModel: [
             {
                 name: 'Pay_Ded',
                 index: 'Pay_Ded',
                 width: 120,
                 align: "left"
             },

            {
                name: 'Tr_No',
                index: 'Tr_No',
                width: 55,
                align: "left"
            },

            {
                name: 'head_Id',
                index: 'head_Id',
                width: 80,
                align: "left"


            },

            {
                name: 'Account_Head',
                index: 'Account_Head',
                width: 150,
                align: "left"


            },

            {
                name: 'Contractor',
                index: 'Contractor',
                width: 150,
                align: "left"


            },

            {
                name: 'Agreemnt',
                index: 'Agreemnt',
                width: 110,
                align: "left"
              

            },
            {
                name: 'Road',
                index: 'Road',
                width: 150,
                align: "left"
               

            },

             {
                 name: 'Cash_Cheque',
                 index: 'Cash_Cheque',
                 width: 100,
                 align: "left"
                


             },

             {
                 name: 'Amount',
                 index: 'Amount',
                 width: 60,
                 align: "right"
               

             },
        {
            name: 'Narration',
            index: 'Narration',
            width: 170,
            align: "left"

        },
            {
                name: 'Edit',
                index: 'Edit',
                width: 50,
                align: "Center"

            }, {
                name: 'Delete',
                index: 'Delete',
                width: 50,
                align: "Center"

            }


        ],
        pager: "#PaymentGridDivpager",
        viewrecords: true,
        loadComplete: function () {            
            unblockPage();
            $('#PaymentGridDivList').jqGrid('setGridWidth', $("#gview_AuthorizationList").width());

        },
        loadError: function (xhr, st, err) {
            unblockPage();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            return false;
        },
        sortname: 'Tr_No',
        grouping: true,
        groupingView: {
            groupField: ['Pay_Ded'],
            groupColumnShow: [false],
            groupText: ['<b>{0}</b>']
            //,groupCollapse: true
        },
        sortorder: "asc",
        caption: "Transaction Details"
    });



}

//function to delete authorization transaction payment
function DeleteTransactionPayment(urlParam) {
    if (confirm("Are you sure you want to delete the  authorization transaction  ?")) {

        blockPage();

        $.ajax({
            type: "POST",
            url: "/Authorization/DeleteTransactionPaymentDetails/" + urlParam,
            // async: false,
            error: function (xhr, status, error) {
                unblockPage();
                $('#errorSpan').text(xhr.responseText);
                $('#divError').show('slow');
                return false;
            },
            success: function (data) {
                unblockPage();
                $('#divError').hide('slow');
                $('#errorSpan').html("");
                $('#errorSpan').hide();

                if (data.Success == 1) {

                    if (data.TransactionType == "P") {

                        $("#PaymentGridDivList").jqGrid().setGridParam
                        ({ url: '/Authorization/GetPaymentDetailList/' + urlParam, datatype: "json", page: 1 }).trigger("reloadGrid");

                        GetAuthorizationAmountDetails(data.master_Bill_Id);

                        $(':input', '#PaymentTransactionForm').not(':button, :submit, :reset, :hidden').val('').removeAttr('selected');

                        //resetDetailsForm();

                        clearValidation($("#PaymentTransactionForm"));

                        $("#btnPaymentUpdate").hide();
                        $("#btnPaymentSubmit").show();
                        $("#btnPaymentCancel").hide();
                        $("#btnPaymentReset").show();

                        $("#HeadDescTr").hide();
                        if ($('#HEAD_ID_P').is(':disabled')) {
                            $('#HEAD_ID_P').attr('disabled', false);
                        }

                        

                        alert("Authorization Details Deleted Successfully.");

                    }
                    else {

                        //reload the Deduction grid 
                        $("#PaymentGridDivList").jqGrid().setGridParam
                        ({ url: '/Authorization/GetPaymentDetailList/' + urlParam, datatype: "json", page: 1 }).trigger("reloadGrid");

                        GetAuthorizationAmountDetails(data.master_Bill_Id);

                        $(':input', '#DeductionTransactionForm').not(':button, :submit, :reset, :hidden').val('').removeAttr('selected');
                        resetDetailsForm();

                        $("#btnDeductionUpdate").hide();
                        $("#btnDeductionSubmit").show();
                        $("#btnDeductionReset").show();
                        $("#btnDeductionCancel").hide();

                        clearValidation($("#DeductionTransactionForm"));

                        alert("Deduction Details Deleted Successfully.");

                    }
                    return false;
                }
                else if (data.result == -1) {
                    alert("Finalized entry can not be deleted .");
                    return false;
                }
                else {

                    alert("Error while deleting Master Payment ");
                    return false;
                }
            }
        }); //end of ajax
    }

}

//function to edit the Authorization transaction payment
function EditTransactionPayment(urlparam) {
    blockPage();
    $('#AuthDetailsEntryDiv').load('/Authorization/EditTransactionDetails/' + urlparam, function (data) {
         authTransEdit = true;
        TriggerWhenError = true;
        //store the amounts to be edited for edit operation validation
        ChqAmountToEdit = parseFloat($("#AMOUNT_Q").val());
        CashAmountToEdit = parseFloat($("#AMOUNT_C").val());
        dedAmountToEdit = parseFloat($("#AMOUNT_D").val());

        //hide the funalize table
        // $('#tblFinalize').hide('slow');

        //show payment/deduction data entry form
        $('#AuthDetailsEntryDiv').show('slow');

        //if cash amount to enter is 0 make it readonle
        if (TotalAmtToEnterDedAmount == 0) {
            $("#cashAmtTr").hide();
            $("#AMOUNT_C").val(0).prop("readonly", 'readonly');
        }
        else {
            $("#AMOUNT_C").removeAttr("readonly");
            $("#cashAmtTr").show();
        }
        
        if ($("#AMOUNT_D").val() != "" || $("#AMOUNT_D").val() != 0) {
            if (Bill_finalized == "N" || Bill_finalized == "" || Bill_finalized == null) {
                changeNarrationDedAuth = false;
                $("#btnDeductionUpdate").show();
                $("#btnDeductionSubmit").hide();
                $("#btnDeductionReset").hide();
                $("#btnDeductionCancel").show();
                $("#HEAD_ID_D").trigger('change');
                changeNarrationDedAuth = true;

            }

        }
        //if cheque amount is not empty it is payment transaction
        if ($("#AMOUNT_Q").val() != "" || $("#AMOUNT_Q").val() != 0) {

            if (Bill_finalized == "N" || Bill_finalized == "" || Bill_finalized == null) {

                $("#btnPaymentUpdate").show();
                $("#btnPaymentSubmit").hide();
                $("#btnPaymentCancel").show();
                $("#btnPaymentReset").hide();
            }
            changeNarrationPayAuth = false;

            $("#HEAD_ID_P").trigger('change');

            changeNarrationPayAuth = true;

            //if road then show is final payment
            if ($("#IMS_PR_ROAD_CODE").val() != 0)
            {
                // for showing final payment option to only Construction of New Works and Upgradation of New Works
                if ($("#HEAD_ID_P option:selected").val() == "48$Q" || $("#HEAD_ID_P option:selected").val() == "49$Q" || $("#HEAD_ID_P option:selected").val() == "114$Q" || $("#HEAD_ID_P option:selected").val() == "115$Q") {
                    $(".final").show('slow');
                }
                else {
                    $(".final").hide();
                }
            }

            //while editing  disable the agreement codes if it has some value

            if ($("#IMS_AGREEMENT_CODE_C") != null && $("#IMS_AGREEMENT_CODE_C").val() != 0) {
                $("#IMS_AGREEMENT_CODE_C").attr('disabled', 'disabled');
                //new change done by Vikram on 30-09-2013

                //$("#IMS_AGREEMENT_CODE_C").attr('disabled', false);
                //end of change
            }


             if ($("#IMS_AGREEMENT_CODE_DED") != null && $("#IMS_AGREEMENT_CODE_DED").val() != 0) {
                $("#IMS_AGREEMENT_CODE_DED").attr('disabled', 'disabled');
            }

        }
        unblockPage();
    });
}


//function to view finalized payments transaction details
function ViewTransactionPayment(urlparam) {

    return false;
   /* blockPage();

    $('#AuthDetailsEntryDiv').load('/Authorization/EditTransactionDetails/' + urlparam, function (data) {

        TriggerWhenError = true;
        //store the amounts to be edited for edit operation validation
        ChqAmountToEdit = parseFloat($("#AMOUNT_Q").val());
        CashAmountToEdit = parseFloat($("#AMOUNT_C").val());
        dedAmountToEdit = parseFloat($("#AMOUNT_D").val());

        //hide the funalize table
        // $('#tblFinalize').hide('slow');

        //show payment/deduction data entry form
        $('#AuthDetailsEntryDiv').show('slow');

        //if cash amount to enter is 0 make it readonly
        if (TotalAmtToEnterDedAmount == 0) {
            $("#cashAmtTr").hide();
            $("#AMOUNT_C").val(0).prop("readonly", 'readonly');
        }
        else {
            $("#AMOUNT_C").removeAttr("readonly");
            $("#cashAmtTr").show();
        }

        if ($("#AMOUNT_D").val() != "" || $("#AMOUNT_D").val() != 0) {
            if (Bill_finalized == "N" || Bill_finalized == "" || Bill_finalized == null) {
                $("#btnDeductionUpdate").show();
                $("#btnDeductionSubmit").hide();
            }

        }
        //if cheque amount is not empty it is payment transaction
        if ($("#AMOUNT_Q").val() != "" || $("#AMOUNT_Q").val() != 0) {

            if (Bill_finalized == "N" || Bill_finalized == "" || Bill_finalized == null) {

                $("#btnPaymentUpdate").show();
                $("#btnPaymentSubmit").hide();
            }

            $("#HEAD_ID_P").trigger('change');

            //if road then show is final payment
            if ($("#IMS_PR_ROAD_CODE").val() != 0)
            {
                $(".final").show();
            }
       }

        $("#PaymentTransactionForm :input ").prop("readonly", 'readonly');
        $("#DeductionTransactionForm :input ").prop("readonly", 'readonly');

        $("#PaymentTransactionForm  select").prop("disabled", true);
        $("#DeductionTransactionForm  select").prop("disabled", true);


        unblockPage();
    });

    */
}