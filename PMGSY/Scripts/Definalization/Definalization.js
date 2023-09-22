
jQuery.validator.addMethod("RequiredDrp", function (value, element) {

    // do not check for State
    //alert(parseFloat($("#LevelID").val()));

    if (parseFloat(value) == 0) {
        return false;
    }

    if (value == "")
    { return false; }

   
    else {
        return true;
    }

}, "Invalid Cheque Number");



$(document).ready(function () {

    $.validator.unobtrusive.parse($("#selectionForm"));
       
    $(function () {


        $("#VoucherDetailsDialog").dialog({
            autoOpen: false,
            // height:550,
            width: '90%',
            modal: true,
            show: {
                effect: "blind",
                duration: 1000
            },
            hide: {
                effect: "explode",
                duration: 1000
            }

        });
    });

    $("#rdDPIU").on("click", function () {

        $("#DPIU").show('slow');

        $('#DPIU').rules('add', {
            // maxlength: 6,
            RequiredDrp: true,
            messages: {
                RequiredDrp: 'DPIU is Required',

            }
        });

    });

    $("#rdSRRDA").on("click", function () {

        $("#DPIU").hide('slow');
        $('#DPIU').val(0);
        $('#DPIU').rules("remove", "messages");

        //Removes validation from input-fields
      
        $("#DPIU").removeClass("input-validation-error").addClass("input-validation-valid");
        $("#DPIU").next('span').removeClass("field-validation-error").addClass("field-validation-valid");
        $("#DPIU").parent('div').find('span:eq(1)').text("");
      

    });

    //event for view details button
    $("#btnViewDetails").click(function () {

        //$('#FUND_TYPE').rules('add', {
        //    // maxlength: 6,
        //    RequiredDrp: true,
        
        //    messages: {
        //        RequiredDrp: 'Fund Type is Required',
              
        //    }
        //});


        $('#VOUCHER_TYPE').rules('add', {
            // maxlength: 6,
            RequiredDrp: true,

            messages: {
                RequiredDrp: 'Voucher Type is Required',

            }
        });


        if ($("#selectionForm").valid())
        {
            $('#VoucherList').jqGrid('GridUnload');
            loadVoucherGrid();
       }

    })


    $(function () {
        if ($("#rdSRRDA").is(":checked")) {
            $('#ddlSRRDA').trigger('change');
            $("#DPIU").hide();
        }
    });

    //Added By Abhishek 5Jan2015
    $("#ddlSRRDA").change(function () {

        var adminNdCode = $('#ddlSRRDA option:selected').val();

        $.ajax({
            url: '/Definalization/PopulateDPIU/' + adminNdCode,
            type: 'GET',
            catche: false,
            error: function (xhr, status, error) {
                alert('An Error occured while processig your request.')
                return false;
            },
            success: function (data) {

                $('#DPIU').empty();

                $.each(data, function () {

                    $('#DPIU').append("<option value=" + this.Value + ">" + this.Text + "</option>");

                });
            }
        });

    });
});


//function to populate the grid od authorization request
function loadVoucherGrid() {


    jQuery("#VoucherList").jqGrid({

        url: '/Definalization/GetVoucherList/',
        datatype: 'json',
        mtype: 'POST',
        height: 'auto',
        rowNum: 15,
        postData: {
            //commented by Koustubh Nakate on 23/08/2013 for set fund type from session  
            //'fundType': $("#FUND_TYPE").val(),
            'level': function ()
            {
                if ($('#rdSRRDA').is(":checked")) {
                    return $('#rdSRRDA').val();
                } else { return $('#rdDPIU').val(); }
            },
            'voucherType': $('#VOUCHER_TYPE').val(),
            'months': $('#MONTH').val(),
            'year': $('#YEAR').val(),
            'DPIU': $('#DPIU').val(),
            'SRRDA': $('#ddlSRRDA').val(),
            'FundType': function () {
                if ($('#rdoFundProgramme').is(":checked")) {
                    return $('#rdoFundProgramme').val();
                }
                else if ($('#rdoFundAdmin').is(":checked")) {
                    return $('#rdoFundAdmin').val();
                }
                else {
                    return $('#rdoFundMaintenance').val();
                }
            },
        },
        rownumbers: true,
        //width: 1150,
        autowidth: true,
        pginput: false,
        //shrinkToFit: false,
        rowList: [15, 20, 30],
        colNames: ['Receipt /Voucher/ TEO Number',
                    'Date',
                    'Transaction Type',
                    'Cheque/Ref/Epayment Number ',
                    'Cheque Date',
                    'Cheque Amount (In Rs.)',
                    'Cash Amount (In Rs.)',
                    'Gross Amount (In Rs.)',
                    'Payee Name',
                    'View Details',
                    'Definalize',
                    'Delete',
                    'encashed'
                    ],
        colModel: [

            {
                name: 'Voucher_Number',
                index: 'Voucher_Number',
                width: 50,
                align: "center"
               
            },
            {
                name: 'Bill_date',
                index: 'Bill_date',
                width: 80,
                align: "center"
               

            },
             {
                 name: 'Trans_Type',
                 index: 'Trans_Type',
                 width: 150,
                 align: "left"
                

             },

              {
                  name: 'ref_NO',
                  index: 'ref_NO',
                  width: 80,
                  align: "center"

              },
            {
                name: 'Cheque_Date',
                index: 'Cheque_Date',
                width: 50,
                align: "center"

            },
             {
                 name: 'Cheque_Amount',
                 index: 'Cheque_Amount',
                 width: 80,
                 align: "right"

             },
             {
                 name: 'Cash_Amount',
                 index: 'Cash_Amount',
                 width: 80,
                 align: "right"

             },
            {
                name: 'Gross_Amount',
                index: 'Gross_Amount',
                width: 80,
                align: "right"

            },

            {
                name: 'Payee_Name',
                index: 'Payee_Name',
                width: 120,
                align: "left"

            },

              {
                  name: 'View',
                  index: 'View',
                  width: 40,
                  align: "Center"

              },
               {
                   name: 'Definalize',
                   index: 'Definalize',
                   width: 50,
                   align: "Center"

               },
                {
                    name: 'Delete',
                    index: 'Delete',
                    width: 44,
                    align: "Center"

                },

                {
                    name: 'EncashedSRRDA',
                    index: 'EncashedSRRDA',
                    width: 0,
                    align: "Center",
                    hidden:true

                }



        ],
        pager: "#Voucherpager",
        viewrecords: true,
        loadError: function (xhr, st, err) {
            unblockPage();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            return false;
        },
        loadComplete: function (xhr, st, err) {
            $("#VoucherList").parents('div.ui-jqgrid-bdiv').css("max-height", "420px");
            $("#VoucherList").jqGrid('setLabel', "rn", "Sr.</br> No");

        },
        sortname: 'Auth_Number',
        sortorder: "asc",
        caption: "Voucher Details"
    });


   
}




//function to view the details
function ViewDetails(urlparam) {
    $("#VoucherDetailsDialog").dialog("open");
    $('#TransactionList').jqGrid('GridUnload');
    loadTransactionGrid(urlparam);

}

//function to definalization of the voucher
function DefinalizeVoucher(urlparam) {

    if (confirm("Are you sure you want to definalize the selected voucher?")) {


        $.ajax({
            type: "POST",
            url: "/Definalization/CheckIfAssetPayment/" + urlparam,
            //async: false,
            cache:false,
            error: function (xhr, status, error) {
                unblockPage();
                $('#errorSpan').text(xhr.responseText);
                $('#divError').show('slow');
                return false;
            },
            success: function (data) {
                if (data != "") {
                    if (data == "1") {
                        if (confirm("Asset details has been entered against this voucher.Are you still want to definalize the selected voucher ?")) {
                            $.ajax({
                                type: "POST",
                                url: "/Definalization/DefinalizeVoucher/" + urlparam,
                                //async: false,
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

                                    if (data.result == "1") {
                                        $('#VoucherList').jqGrid('GridUnload');
                                        loadVoucherGrid();
                                        alert("Voucher Definalized Successfully.");
                                        return false;
                                    }
                                    else if (data.result == "-111") {
                                        alert("Voucher cant be Definalized as its been acknowledged.");
                                        return false;
                                    }
                                    else if (data.result == "-222") {
                                        alert("Voucher cant be Definalized as its month is closed.");
                                        return false;
                                    }
                                    else if (data.result == "-333") {
                                        alert("Opening Balance voucher cant be Definalized as Payment/Receipt/TEO details has been entered.");
                                        return false;
                                    }
                                    else if (data.result == "-444") {
                                        alert("Epayment voucher cant be Definalized  as its payment is done by bank.");
                                        return false;
                                    }
                                    else if (data.result == "-555") {
                                        alert("Voucher is not finalized");
                                        return false;
                                    }
                                        //Commented by Abhishek kamble 5-June-2014
                                    //else if (data.result == "-666") {
                                    //    alert("Receipt cant be definalized as it would result in negative authorization balance");
                                    //    return false;
                                    //}
                                    else if (data.result == "-777") {
                                        alert("Receipt cant be definalized as it would result in negative cash balance");
                                        return false;
                                    }
                                    else if (data.result == "-123") {
                                        alert("Imprest Voucher cant be definalized as it has already been settled.");
                                        return false;
                                    }
                                    else if (data.result == "-1000") {
                                        alert("Voucher cant be definalized as it is the part of cancellation entry.");
                                        return false;
                                    }
                                    else {
                                        alert("Error while deleting the voucher.");
                                        return false;
                                    }
                                }
                            });
                        }

                    }
                    else {
                        $.ajax({
                            type: "POST",
                            url: "/Definalization/DefinalizeVoucher/" + urlparam,
                            //async: false,
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

                                if (data.result == "1") {
                                    $('#VoucherList').jqGrid('GridUnload');
                                    loadVoucherGrid();
                                    alert("Voucher Definalized Successfully.");
                                    return false;
                                }
                                else if (data.result == "-111") {
                                    alert("Voucher cant be Definalized as its been acknowledged.");
                                    return false;
                                }
                                else if (data.result == "-222") {
                                    alert("Voucher cant be Definalized as its month is closed.");
                                    return false;
                                }
                                else if (data.result == "-333") {
                                    alert("Opening Balance voucher cant be Definalized as Payment/Receipt/TEO details has been entered.");
                                    return false;
                                }
                                else if (data.result == "-444") {
                                    alert("Epayment voucher cant be Definalized  as its payment is done by bank.");
                                    return false;
                                }
                                else if (data.result == "-555") {
                                    alert("Voucher is not finalized");
                                    return false;
                                }
                                else if (data.result == "-666") {
                                    alert("Receipt cant be definalized as it would result in negative authorization balance");
                                    return false;
                                }
                                else if (data.result == "-777") {
                                    alert("Receipt cant be definalized as it would result in negative cash balance");
                                    return false;
                                }
                                else if (data.result == "-123") {
                                    alert("Imprest Voucher cant be definalized as it has already been settled.");
                                    return false;
                                }
                                else if (data.result == "-999") {
                                    alert("Transfer entry order cant be definalized because associated auto entries at piu level cant be deleted , since piu has closed its month  ");
                                    return false;
                                }
                                else if (data.result == "-1000") {
                                    alert("Voucher cant be definalized as it is the part of cancellation entry.");
                                    return false;
                                }
                                else {
                                    alert("Error while deleting the voucher.");
                                    return false;
                                }
                            }
                        });

                    }

                }
                else {
                    alert("Error while getting voucher details");
                    return false;
                }


            }
        });
    }


}

//function to delete the voucher
function Delete(urlparam) {

    if (confirm("Are you sure you want to delete the selected voucher ? ")) {


        $.ajax({
            type: "POST",
            url: "/Definalization/CheckIfAssetPayment/" + urlparam,
            //async: false,
            error: function (xhr, status, error) {
                unblockPage();
                $('#errorSpan').text(xhr.responseText);
                $('#divError').show('slow');
                return false;
            },
            success: function (data) {
                if (data != "") {
                    if (data == "1")
                    {
                        alert("Asset details has been entered against this voucher.selected voucher cant be deleted")
                        return false;

                    }
                    else {
                        $.ajax({
                            type: "POST",
                            url: "/Definalization/DeleteVoucher/" + urlparam,
                            //async: false,
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

                                if (data.result == "1") {
                                    $('#VoucherList').jqGrid('GridUnload');
                                    loadVoucherGrid();
                                    alert("voucher Deleted Successfully.");
                                    return false;
                                }
                                else if (data.result == "-111") {
                                    alert("voucher cant be deleted as its been acknowledged.");
                                    return false;
                                }
                                else if (data.result == "-222") {
                                    alert("voucher cant be deleted as its month is closed.");
                                    return false;
                                }
                                else if (data.result == "-333") {
                                    alert("Opening Balance voucher cant be deleted as Payment/Receipt/TEO details has been entered.");
                                    return false;
                                }
                                else if (data.result == "-444") {
                                    alert("Epayment voucher cant be deleted its payment is finalized by bank.");
                                    return false;
                                } else if (data.result == "-555") {
                                    alert("voucher is not finalized");
                                    return false;
                                }
                                else if (data.result == "-666") {
                                    alert("Receipt cant be deleted as it would result in negative authorization balance");
                                    return false;
                                }
                                else if (data.result == "-777") {
                                    alert("Receipt cant be deleted as it would result in negative cash balance");
                                    return false;
                                }
                                else if (data.result == "-123") {
                                    alert("Imprest Voucher cant be definalized as it has already been settled.");
                                    return false;
                                }
                                else if (data.result == "-1000") {
                                    alert("Voucher cant be definalized as it is the part of cancellation entry.");
                                    return false;
                                }
                                else if (data.result == "-888") {
                                    alert("Voucher cant be definalized as its settlement is already present.");
                                    return false;
                                }//If Added BY Abhishek kamble 14-July-2014
                                else if (data.result == "-999") {
                                    alert("Voucher cant be deleted as it is reconciled by bank.");
                                    return false;
                                }//If Added BY Abhishek kamble 28-July-2014
                                else if (data.result == "-8080") {
                                    alert("An Error occured while deleting the voucher.");
                                    return false;
                                }
                                else {
                                    alert("Error while deleting the voucher.");
                                    return false;
                                }
                            }
                        });

                    }

                }
                else {
                    alert("Error while getting voucher details");
                    return false;
                }


            }
        });



       
    }

}

function loadTransactionGrid(param_bill_id) {

   
    jQuery("#TransactionList").jqGrid({

        url: '/Definalization/GetTransactionDetails/' + param_bill_id,
        datatype: 'json',
        mtype: 'POST',
        height: 'auto',
        rowNum: -1,
        pgbuttons: false,
        rownumbers: true,
        //width: 1150,
       // autowidth: true,
        pginput: false,
        //shrinkToFit: false,
       // rowList: [15, 20, 30],
        colNames: ['Head Description',
                    'Amount (In Rs.)',
                    'Narration',
                    'Contractor Name',
                    'Agreement Number',
                    'Road Name',
                   'PIU Name',
                   'Final Payment'
        ],
        colModel: [

            {
                name: 'Head Description',
                index: 'Head Description',
                width: 180,
                align: "left"

            },
            {
                name: 'Amount',
                index: 'Amount',
                width: 80,
                align: "right"


            },
             {
                 name: 'Narration',
                 index: 'Narration',
                 width: 170,
                 align: "left"
             },

              {
                  name: 'Contractor',
                  index: 'Contractor',
                  width: 120,
                  align: "left"

              },
            {
                name: 'Agreement',
                index: 'Agreement',
                width: 150,
                align: "left"

            },
             {
                 name: 'Road',
                 index: 'Road',
                 width: 150,
                 align: "left"

             },
             {
                 name: 'PIU',
                 index: 'PIU',
                 width: 100,
                 align: "left"

             },
            {
                name: 'Final',
                index: 'Final',
                width: 40,
                align: "center"

            }
        ],
        pager: "#TransactionPager",
        sortname: 'Head Description',
        viewrecords: true,
        loadError: function (xhr, st, err) {
            unblockPage();
            $('#errorSpan1').text(xhr.responseText);
            $('#divError1').show('slow');
            return false;
        },
        loadComplete: function (xhr, st, err) {
            $('#divErro1r').hide('slow');
            $('#errorSpan1').html("");
            $('#errorSpan1').hide();

            $("#TransactionList").parents('div.ui-jqgrid-bdiv').css("max-height", "420px");
            $("#TransactionList").jqGrid('setLabel', "rn", "Sr.</br> No");
            //$('#TransactionList').setGridWidth($('#VoucherDetailsDialog').width());
        },
        // sortname: '',
        sortorder: "asc",
        caption: "Transaction Details"
    });



}