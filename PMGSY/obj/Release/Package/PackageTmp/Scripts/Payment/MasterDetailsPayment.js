
//this file is intermediate file which takes care of payment transaction details, populating payment amount table, loading master and details grid for data entry

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
var transEdit = false;

var MasterTxnId = 0;//Added By Abhishek kamble 25Nov2014 to use hide / show Agreement on deduction side 

$(document).ready(function () {

    blockPage();

    loadPaymentMasterGrid("view", Bill_ID);

    //alert("Master Details Payment.js opeartion " + opeartion)
    $('#HideShowTransaction').click(function () {
        
            $("#TransactionForm").toggle('slow', function () { });

          $('#iconSpan').toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
                    
            //check if cash amount to enter is 0 ;make it readoly
          if (parseFloat(TotalAmtToEnterDedAmount) == 0) {
                $("#AMOUNT_C").val(0).prop("readonly", 'readonly');
                $("#cashAmtTr").hide();
            }
            else {
                $("#AMOUNT_C").val(0).removeAttr("readonly");
                $("#cashAmtTr").show();
            }


    });
    
    if (opeartion == "A")
    {
        blockPage();

        $('#MasterData').load('/Payment/AddEditMasterPayment/' + month + '$' + year, function () {
                       
            $("#masterListGrid").hide();

            $("#trShowHideLinkTable").hide();

            $("#MasterDataEntryDiv").show('slow');

            unblockPage();
          });
    }
    else if (opeartion == "E") {

        blockPage();
       
        //load payment grid
        loadPaymentGrid(Bill_ID);

        GetAmountTableDetails(Bill_ID);

        //load payment and deduction form
      //  $('#PaymentDeductionData').load('/Payment/PartialTransaction/' + Bill_ID, function () { });

        //load master data entry form
        blockPage();
        $('#MasterData').load('/Payment/EditMasterPayment/' + Bill_ID, function () {
                        
           
                //reload the master entry grid on transaction page to show master data 
                $("#PaymentMasterList").jqGrid().setGridParam
                       ({ url: '/Payment/ListMasterPaymentDetailsForDataEntry/' + Bill_ID, datatype: "json", page: 1 }).trigger("reloadGrid");
                               
                //show transaction form
                //$("#trnsShowtable").show('slow');

               // $("#TransactionForm").show('slow');


                //if (parseFloat(TotalAmtToEnterDedAmount) == 0) {
                //    $("#AMOUNT_C").val(0).prop("readonly", 'readonly');
                //    $("#cashAmtTr").hide();
                //}
                //else {
                //    $("#AMOUNT_C").val(0).removeAttr("readonly");
                //    $("#cashAmtTr").show();
                //}

                changeNarrationPay = true;
                changeNarrationDed = true;
                unblockPage();
           
         });
      
    }

 });//doc ready


//function to edit the transaction payment
function EditTransactionPayment(urlparam)
{
    $('#PaymentDeductionData').load('/Payment/EditTransactionDetails/' + urlparam, function (data) {

        transEdit = true;
        TriggerWhenError = true;
        //store the amounts to be edited for edit operation validation
        ChqAmountToEdit = parseFloat($("#AMOUNT_Q").val());
        CashAmountToEdit = parseFloat($("#AMOUNT_C").val());
        dedAmountToEdit = parseFloat($("#AMOUNT_D").val());


        //hide the funalize table
        // $('#tblFinalize').hide('slow');

        //show payment/deduction data entry form
        $('#PaymentDeductionData').show('slow');

       
        //if cash amount to enter is 0 make it readonle
        if (TotalAmtToEnterDedAmount == 0)
        {
            $("#cashAmtTr").hide();
            $("#AMOUNT_C").val(0).prop("readonly", 'readonly');
        }
        else
        {

            $("#AMOUNT_C").removeAttr("readonly");
            $("#cashAmtTr").show();
        }
             
        if ($("#AMOUNT_D").val() != "" || $("#AMOUNT_D").val() != 0)
        {
            if (Bill_finalized == "N" || Bill_finalized == "" || Bill_finalized == null) {
                changeNarrationDed = false;
                $("#btnDeductionUpdate").show();
                $("#btnDeductionCancel").show();
                $("#btnDeductionSubmit").hide();
                $("#btnDeductionReset").hide();
                $("#HEAD_ID_D").trigger('change');
                changeNarrationDed = true;
            }
        
        }

        //if cheque amount is not empty it is payment transaction
        if ($("#AMOUNT_Q").val() != "" || $("#AMOUNT_Q").val() != 0)
        {
          
            if (Bill_finalized == "N" || Bill_finalized == "" || Bill_finalized == null)
            {

                $("#btnPaymentUpdate").show();
                $("#btnPaymentCancel").show();
                $("#btnPaymentSubmit").hide();
                $("#btnPaymentReset").hide();
            }

            changeNarrationPay = false;

            $("#HEAD_ID_P").trigger('change');
               
            changeNarrationPay = true;
            
                
            //if road then show is final payment
            if ($("#IMS_PR_ROAD_CODE").val() != 0)
            {
                // for showing final payment option to only Construction of New Works and Upgradation of New Works
                if ($("#HEAD_ID_P option:selected").val() == "48$Q" || $("#HEAD_ID_P option:selected").val() == "49$Q" || $("#HEAD_ID_P option:selected").val() == "114$Q" || $("#HEAD_ID_P option:selected").val() == "115$Q" || $("#HEAD_ID_P option:selected").val() == "1543$Q") {
                    $(".final").show('slow');
                }
                else {
                    $(".final").hide();
                }
            }

            //while editing  disable the agreement codes if it has some value
            if ($("#IMS_AGREEMENT_CODE_C") != null &&  $("#IMS_AGREEMENT_CODE_C").val() != 0)
            {
                //if contractor for transaction is required dont disable the agrrement selection
                 
                if ($('#conTractorTR').is(":visible"))
                {
                        
                    $("#IMS_AGREEMENT_CODE_C").removeAttr('disabled');
                }
                else {

                    $("#IMS_AGREEMENT_CODE_C").attr('disabled', 'disabled');
                }
            }

            if ($("#IMS_AGREEMENT_CODE_S") != null && $("#IMS_AGREEMENT_CODE_S").val() != 0) {
                $("#IMS_AGREEMENT_CODE_S").attr('disabled', 'disabled');
            }

            if ($("#IMS_AGREEMENT_CODE_DED") != null && $("#IMS_AGREEMENT_CODE_DED").val() != 0) {
                $("#IMS_AGREEMENT_CODE_DED").attr('disabled', 'disabled');
            }
               
        }


        if ($("#HEAD_ID_P").is(':disabled')) {
        //Commented By Abhishek kamble because Naration is reset at Edit mode 20Nov2014
        //    $("#HEAD_ID_P").trigger('change');
        }


        //Added By Abhishek kamble 10-Apr-2014 To Set Agreement Drop down       
        $("#IMS_AGREEMENT_CODE_C").val($("#hdnSelectedAgreementForContrator").val());      
        $("#IMS_AGREEMENT_CODE_S").val($("#hdnSelectedAgreementForSupplier").val());
      
        unblockPage();
    });
}

//function to delete transaction payment
function DeleteTransactionPayment(urlParam)
{
    
  if(confirm("Are you sure you want to delete the  payment ?"))
      {

      blockPage();

        $.ajax({
            type: "POST",
            url: "/payment/DeleteTransactionPaymentDetails/" + urlParam,
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
                                      
                        //reload the payment Grid 
                        $("#PaymentGridDivList").jqGrid().setGridParam
                           ({ url: '/Payment/GetPaymentDetailList/' + data.master_Bill_Id, datatype: "json", page: 1 }).trigger("reloadGrid");

                        GetAmountTableDetails(data.master_Bill_Id);

                        $(':input', '#PaymentTransactionForm').not(':button, :submit, :reset, :hidden').val('').removeAttr('selected');

                       // resetDetailsForm();

                        clearValidation($("#PaymentTransactionForm"));

                     

                        $("#HeadDescTr").hide('slow');

                        alert("Payment Details Deleted Successfully.");
                        $("#HeadDescTr").hide();
                        if ($('#HEAD_ID_P').is(':disabled')) {
                            $('#HEAD_ID_P').attr('disabled', false);
                        }


                        //new change done by Vikram 

                        $("#PaymentDeductionData").load('/Payment/PartialTransaction/' + data.master_Bill_Id, function () {

                            $("#HEAD_ID_P").trigger('change');

                            if (parseFloat(TotalAmtToEnterDedAmount) == 0) {
                                $("#AMOUNT_C").val(0).prop("readonly", 'readonly');
                                $("#cashAmtTr").hide();
                            }
                            else {
                                $("#AMOUNT_C").val(0).removeAttr("readonly");
                                $("#cashAmtTr").show();
                            }

                        });

                        //end of change

                     //added by Koustubh Nakate on 28/09/2013

                        $("#btnPaymentUpdate").hide();
                        $("#btnPaymentCancel").hide();
                        $("#btnPaymentReset").show();
                        $("#btnPaymentSubmit").show();

                    }
                    else {

                        //reload the Deduction grid 
                        $("#PaymentGridDivList").jqGrid().setGridParam
                          ({ url: '/Payment/GetPaymentDetailList/' + data.master_Bill_Id, datatype: "json", page: 1 }).trigger("reloadGrid");

                        GetAmountTableDetails(data.master_Bill_Id);

                        $(':input', '#DeductionTransactionForm').not(':button, :submit, :reset, :hidden').val('').removeAttr('selected');
                        //resetDetailsForm();

                        clearValidation($("#DeductionTransactionForm"));

                        $("#headDescDedTR").hide('slow');

                        alert("Deduction Details Deleted Successfully.");

                        if ($('#HEAD_ID_D').is(':disabled')) {
                            $('#HEAD_ID_D').attr('disabled', false);
                        }

                        $("#headDescDedTR").hide();

                        //added by Koustubh Nakate on 28/09/2013
                        $("#btnDeductionUpdate").hide();
                        $("#btnDeductionSubmit").show();
                        $("#btnDeductionReset").show();
                        $("#btnDeductionCancel").hide();
                    }

                    //$('#conTractorTR').hide();
                    //$('#HeadDescTr').hide();
                    //$("#headDescDedTR").hide();
                  

                     return false;
                }
                else if (data.result == -1) {
                    alert("Finalized entry can not be deleted .");
                    return false;
                }
                else if (data.result == -2)
                {
                    alert("Asset Details has been entered for the selected  transaction type for this payment.Please delete the asset details first .");
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

//function to view finalized payments transaction details
function ViewTransactionPayment(urlParam)
{
    return false;
   /*
    blockPage();

    $('#PaymentDeductionData').load('/Payment/EditTransactionDetails/' + urlParam, function (data) {

        $('#tblFinalize').hide();

        $('#PaymentDeductionData').show('slow');
    

        //while editing  disable the agreement codes if it has some value
        if ($("#IMS_AGREEMENT_CODE_C") != null && $("#IMS_AGREEMENT_CODE_C").val() != 0) {
            $("#IMS_AGREEMENT_CODE_C").attr('disabled', 'disabled');
        }

        if ($("#IMS_AGREEMENT_CODE_S") != null && $("#IMS_AGREEMENT_CODE_S").val() != 0) {
            $("#IMS_AGREEMENT_CODE_S").attr('disabled', 'disabled');
        }

        if ($("#IMS_AGREEMENT_CODE_DED") != null && $("#IMS_AGREEMENT_CODE_DED").val() != 0) {
            $("#IMS_AGREEMENT_CODE_DED").attr('disabled', 'disabled');
        }
               

        $("#HEAD_ID_P").trigger('change');

        $("#btnPaymentUpdate").hide();

        $("#btnPaymentSubmit").hide();

        $("#btnPaymentCancel").hide();

        $("#btnDeductionUpdate").hide();

        $("#btnDeductionSubmit").hide();

        $("#btnDeductionCancel").hide();


        $("#PaymentTransactionForm :input ").prop("readonly", 'readonly');
        $("#DeductionTransactionForm :input ").prop("readonly", 'readonly');
      
        $("#PaymentTransactionForm  select").prop("disabled", true);
        $("#DeductionTransactionForm  select").prop("disabled", true);

        unblockPage();
    });

    */
}

//function to calculate the amounts
function GetAmountTableDetails(param_Bill_ID)
{
  
    blockPage();
    $.ajax({
        type: "POST",
        datatype: "json",
        url: "/payment/GetAmountBalanceDetails/" + param_Bill_ID,
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
                
                if ( data.DiffGrossAmount == 0 && data.DiffChqAmount == 0 & data.DiffDedAmount == 0 && data.DiffGrossAmount==0) {
                  
                   

                        
                    //If Condition Added By Abhishek kamble 16-Apr-2014
                    //if (((data.TransactionId == 47) || (data.TransactionId == 737)||(data.TransactionId == 1484))) {

                    //Avinash for RCPLWE
                    if (((data.TransactionId == 47) || (data.TransactionId == 737) || (data.TransactionId == 1484) || (data.TransactionId == 1788) || (data.TransactionId == 1974))) {

                        if (data.IsDetailsEntered == true) {                         
                            $('#PaymentDeductionData').hide();

                            if (data.VoucherFinalized == "N") {   //show finalize 
                                $('#tblFinalize').show('slow');
                            }

                        } else if (data.IsDetailsEntered == false) {
                            $('#tblFinalize').hide('slow');
                            $('#PaymentDeductionData').show();
                        }
                    }
                    else {
                        
                        //Orignal Code
                        $('#PaymentDeductionData').hide();
                        if (data.VoucherFinalized == "N") {   //show finalize 
                            $('#tblFinalize').show('slow');

                        }
                    }
                    
                     } else
                {
                    

                            $('#tblFinalize').hide('slow');
                            $('#PaymentDeductionData').show('slow');
                        }
                                          
               //if all amounts to entered is 0,i.e. no amount is entered yet then enable the agreement /payment head (if multiple trans is no for master ) to add 
                 if (data.TotalAmtEnteredChqAmount == 0 && data.TotalAmtEnteredCachAmount == 0 && data.TotalAmtEnteredDedAmount==0 )
                        {
                            if ($("#TXN_ID") != null) {
                                $("#TXN_ID").removeAttr('disabled');
                            }
                               
                            if ($("#IMS_AGREEMENT_CODE_S") != null)
                            {
                                $("#IMS_AGREEMENT_CODE_S").removeAttr('disabled');
                            }
                            if ($("#IMS_AGREEMENT_CODE_C") != null)
                            {
                               
                                $("#IMS_AGREEMENT_CODE_C").removeAttr('disabled');
                            }

                            if ($("#IMS_AGREEMENT_CODE_DED") != null && data.TotalAmtToEnterChqAmount == 0)
                            {
                                $("#IMS_AGREEMENT_CODE_DED").removeAttr('disabled');
                            }
                                              
                            if ($("#HEAD_ID_P") != null)
                            {
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



function BlockTransactionPayment()
{
    alert("This Entry cant be allowed to edit. Please Delete this entry and make new entry");
    return false;

}
var payGridWidth = 0;

//function to load payment & deduction grid of the transaction
function loadPaymentGrid(param_Bill_ID) {
          
    blockPage();
   
    jQuery("#PaymentGridDivList").jqGrid({

        url: '/Payment/GetPaymentDetailList/' + param_Bill_ID,
        datatype: 'json',
        mtype: 'POST',
        height: 'auto',
        rowNum: 1000,
        //width:$("#gview_PaymentMasterList").width(),
        autowidth: true,
        pginput: false,
        pgbuttons: false,
        colNames: ['Payment/Deduction', 'Tr.No', 'Head Code', 'Transaction type', 'Contractor Company Name', 'Agreement', 'Road', 'Cash <br/>/Cheque', 'Amount (In Rs.)', 'Narration', 'Edit', 'Delete', 'status'],
        colModel: [
             {
                 name: 'Pay_Ded',
                 index: 'Pay_Ded',
                 width:120,
                 align: "left",
                 sortable: false
              

             },

            {
                name: 'Tr_No',
                index: 'Tr_No',
                width: 55,
                align: "left",
                sortable: false,
                hidden:true

            },

            {
                name: 'head_Id',
                index: 'head_Id',
                width: 35,
                align: "left",
                sortable:false
              

            },

            {
                name: 'Account_Head',
                index: 'Account_Head',
                width:150,
                align: "left",
                sortable: false
              

            },


             {
                 name: 'Contractor_Name',
                 index: 'ContractorName',
                 width: 110,
                 align: "left",
                 sortable: false,
                 hidden: (fundType == 'A' ? true : false)

             },

            {
                name: 'Agreemnt',
                index: 'Agreemnt',
                width: 110,
                align: "left",
                sortable: false,
                hidden:(fundType == 'A'?true:false)

            },
            {
                name: 'Road',
                index: 'Road',
                width:150,
                align: "left",
                sortable: false,
                hidden: (fundType == 'A' ? true : false)

            },

             {
                 name: 'Cash_Cheque',
                 index: 'Cash_Cheque',
                 width: 60,
                 align: "left",
                 sortable: false


             },

             {
                 name: 'Amount',
                 index: 'Amount',
                 width: 75,
                 align: "right",
                 sortable: false


             },
        {
            name: 'Narration',
            index: 'Narration',
            width:320,
            align: "left",
            sortable: false

        },
            {
                name: 'Edit',
                index: 'Edit',
                width: 50,
                align: "Center",
                sortable: false

            }, {
                name: 'Delete',
                index: 'Delete',
                width: 50,
                align: "Center",
                sortable: false

            },
            {
                name: 'Status',
                index: 'Status',
                width: 0,
                align: "Center",
                sortable: false,
                hidden: false
               

            }


        ],
        pager: "#PaymentGridDivpager",
        viewrecords: true,
        loadComplete:function()
        {
            unblockPage();
           
            $('#PaymentGridDivList').jqGrid('setGridWidth', $("#gview_PaymentMasterList").width());

        },
        loadError: function(xhr, st, err) {
            unblockPage();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            return false;
        },
        sortname: 'Tr_No',
        grouping: true,
        pgbuttons: false,
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



//function to load payment master grid on data entry page

function loadPaymentMasterGrid(mode, bill_id) {

    jQuery("#PaymentMasterList").jqGrid({

        urlString: '/Payment/ListMasterPaymentDetailsForDataEntry/' + bill_id + "/" + Math.random(),
        datatype: 'json',
        mtype: 'POST',
        height: 'auto',
        //rowNum: 10,
        postData: { 'mode': function () { return mode }, 'months': function () { return $('#BILL_MONTH').val() }, 'year': function () { return $('#BILL_YEAR').val() } },
        rownumbers: true,
        //width: 1150,
        autowidth: true,
        pginput: false,
        pgbuttons:false,
        //shrinkToFit: false,
       // rowList: [10, 20, 30],
        colNames: ['Voucher Number', 'Voucher Date', 'Cash/Cheque', 'Transaction Type', 'Cheque/Epay/Advice Number', 'Cheque/Epay/Advice Date', 'Contractor/Payee Name', 'Agreement Number', 'Cheque Amount (In Rs.)', 'Cash Amount (In Rs.)', 'Gross Amount (In Rs.)', 'Finalize', 'Edit', 'Delete'],
        colModel: [
            {
                name: 'Voucher_Number',
                index: 'Voucher_Number',
                width: 15,
                align: "center", sortable: false


            },
            {
                name: 'voucher_date',
                index: 'auth_sig_name',
                width: 15,
                align: "center", sortable: false


            },
             {
                 name: 'Cash_Cheque',
                 index: 'Cash_Cheque',
                 width: 15,
                 align: "center",
                 sortable: false



             },

             {
                 name: 'Transaction_type',
                 index: 'Transaction_type',
                 width: 30,
                 align: "center",
                 sortable: false


             },
        {
            name: 'cheque_number',
            index: 'cheque_number',
            width: 15,
            align: "center",
            sortable: false

        },
            {
                name: 'cheque_Date',
                index: 'cheque_Date',
                width: 15,
                align: "Center",
                sortable: false

            }, {
                name: 'Payee_Name',
                index: 'Payee_Name',
                width: 30,
                align: "Center", sortable: false

            },
            {
                name: 'Agreement_Number',
                index: 'Agreement_Number',
                width: 30,
                align: "Center",
                hidden: true,
                hidedlg: true, sortable:false


            }, {
                name: 'cheque_amount',
                index: 'cheque_amount',
                width: 20,
                align: "right",
                sortable: false

            },
            {
                name: 'Cash_Amount',
                index: 'Cash_Amount',
                width: 10,
                align: "right",
                sortable: false

            },
            {
                name: 'Gross_Amount',
                index: 'Gross_Amount',
                width: 20,
                align: "right",
                sortable: false

            },

            {
                name: 'Finalize',
                index: 'Finalize',
                width: 8,
                align: "center",
                hidden: true,
                hidedlg: true,
                sortable: false

            },
               {
                   name: 'Action',
                   index: 'Action',
                   width: 8,
                   align: "Center",
                   sortable: false

               },
                {
                    name: 'Delete',
                    index: 'Delete',
                    width: 8,
                    align: "Center",
                    sortable: false

                }

        ],
        pager: "#PaymentMasterListpager",
        viewrecords: true,
        loadComplete: function (data) {
            unblockPage();
            payGridWidth = $("#gview_PaymentMasterList").width();
            $('#PaymentGridDivList').jqGrid('setGridWidth', payGridWidth);

            MasterTxnId = data.userdata["TxnId"];//Added By Abhishek kamble 25Nov2014 to use hide / show Agreement on deduction side 

            //Added By Abhishek Kamble 11-Nov-2013
            $('#PaymentMasterList_rn').html('Sr.<br/>No.');
        },
        //loadError: function (xhr, st, err) {
        //    unblockPage();
        //    $('#errorSpan').text(xhr.responseText);
        //    $('#divError').show('slow');
        //    return false;
        //},
        sortname: 'voucher_date',
        sortorder: "asc",
        caption: "Master Payment Details"
    });
    //jQuery("#PaymentList").jqGrid('setFrozenColumns');
}


