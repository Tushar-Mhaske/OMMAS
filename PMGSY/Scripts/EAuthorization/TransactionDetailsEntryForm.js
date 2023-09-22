$(document).ready(function () {


    $("#dialogAddAuthorizationDetails").dialog({
        open: function (event, ui) {

            $("#divAddAuthorizationDetails").html(htmlRes);
        },
        width: 822,
        height: 320,
        autoOpen: false,
        position: {
            my: "left+45% top+40%",
            at: "left top",
            of: "#mainDiv"
        },
    });

    //populate roads based on the contractor aggrement
    $('#IMS_AGREEMENT_CODE_C').change(function () {
        if ($('#IMS_AGREEMENT_CODE_C option:selected').val() != 0) {
            $("#IMS_PR_ROAD_CODE").empty();
            FillInCascadeDropdown(null, "#IMS_SANCTION_PACKAGE", "/EAuthorization/PopulatePackage/" + $("#MAST_CON_ID_C").val() + "$" + $("#IMS_AGREEMENT_CODE_C option:selected").val());
            $("#trPackage").show();
        }
        else {

            //$("#trRoad").hide();
            $("#IMS_PR_ROAD_CODE").empty();

            $("#trPackage").hide();
            $("#IMS_SANCTION_PACKAGE").empty();

        }

    });

    $('#MAST_CON_ID_C').change(function () {
        setPayeeName($('#MAST_CON_ID_C').val());
        getContratorBankDetails($("#MAST_CON_ID_C").val());
        $('#IMS_AGREEMENT_CODE_C').val("");
        $("#IMS_AGREEMENT_CODE_C").empty();
        if ($('#MAST_CON_ID_C').val() != 0) {
            autoFillAgreementList();
            $("#trAggreement").show();

        }
        else {
            $('#IMS_AGREEMENT_CODE_C').val("");
            $("#IMS_AGREEMENT_CODE_C").empty();
            $("#IMS_PR_ROAD_CODE").empty();
            $("#trAggreement").hide();
            $("#trRoad").hide();



        }

    });


    //On Package Change..Check Entry in new Table:ACC_EAUTH_AUTHAMT_DETAILS
    $('#IMS_SANCTION_PACKAGE').change(function () {
        CheckAlreadyAuthorisedAmount($("#MAST_CON_ID_C").val() + "$" + $("#IMS_AGREEMENT_CODE_C option:selected").val() + "$" + $("#IMS_SANCTION_PACKAGE").val());

     

        //Populate Agreement AND Expenditure Amount
        PopulateAgreementExpenditureAmount($("#MAST_CON_ID_C").val() + "$" + $("#IMS_AGREEMENT_CODE_C option:selected").val() + "$" + $("#IMS_SANCTION_PACKAGE").val());


    });



    $("#btnCancel").click(function () {
        $("#TransactionForm").hide('slow');
    });

    //Reseting All the fields on Reset Button click
    $("#btnPaymentReset").click(function (e) {

        $("#lblAgreementAmount").text('');
        $("#spnBankAccNumber").text('');
        $("#spnBankName").text('');
        $("#spnIFSCCode").text('');
        $("#lblExpenditureAmount").text('');
        $("#lnkAlreadyAutAmount").hide();
    });



    $("#btnPaymentUpdate").click(function () {
        if ($("#PaymentTransactionForm").valid()) {

            if (confirm("Are you sure you want to update the details ?")) {
                blockPage();
                $.ajax({
                    type: "POST",
                    url: "/EAuthorization/UpdateEAuthorizationDetails/" + Bill_ID,
                    async: false,
                    data: $("#PaymentTransactionForm").serialize(),
                    error: function (xhr, status, error) {
                        unBlockPage();
                        alert("Error in processing,Please try Again");
                        return false;

                    },
                    success: function (data) {

                        unblockPage();
                        if (data.Success) {


                            alert("e-Authorization Request Details Updated Successfully.");

                            $("#PaymentGridDivList").jqGrid().setGridParam
                              ({ url: '/EAuthorization/GetPaymentDetailList/' + Bill_ID, datatype: "json", page: 1 }).trigger("reloadGrid");

                            $('#EAuthorizationList').trigger('reloadGrid');

                            $("#tblFinalize").show();

                            //$("#TransactionForm").hide('slow');
                            $("#EAuthorizationTransactionFormPlaceHolder").hide('slow');
                            $("#trShowHideLinkTable").hide('slow');


                            // unblockPage();
                        }
                        else {
                            alert(data.errMessage);
                        }



                    }


                });

            }
        }

    });



    $("#lnkAddAuthDetails").click(function () {
        GetAddEAuthorizationLinkView($("#MAST_CON_ID_C").val() + "$" + $("#IMS_AGREEMENT_CODE_C option:selected").val() + "$" + $("#IMS_SANCTION_PACKAGE").val());

        $.validator.unobtrusive.parse($("#frmlnkAddEAuthorizationDetails"));



    });












    //Adding Transacion Details
    $("#btnPaymentSubmit").click(function (e) {
        $.validator.unobtrusive.parse($("#PaymentTransactionForm"));


        if ($("#MAST_CON_ID_C").val() == "0") {
            alert("Please Select Contractor");
            return false;
        }



       

        if ($("#IMS_AGREEMENT_CODE_C").val() == "0") {
            alert("Please Select Agreement Number");
            return false;
        }

        if ($("#IMS_SANCTION_PACKAGE").val() == "0") {
            alert("Please Select Package Number");
            return false;
        }

        //Package NIL Check
        //if ($("#IMS_SANCTION_PACKAGE").val() == "NIL") {
        //    alert("Invalid Package,Please Select Another Package");
        //    return false;
        //}


        //Do not Allow to Save Unless Bank Details are Available
        if ($("#spnBankAccNumber").text() == "-" || $("#spnIFSCCode").text() == "-" || $("#spnBankName").text() == "-") {
            alert("Contractor bank Details are Not Available");
            return false;
        }


        //Check if Already Authorised Amount is Entered or Not...If Amount is Not Entered..Than show alert
        if ($("#ALREADY_AUTHORISED_AMOUNT").val() == null || $("#ALREADY_AUTHORISED_AMOUNT").val() == "" || $("#ALREADY_AUTHORISED_AMOUNT").val() == undefined) {
            alert("Please Enter Amount already Authorized");
            return false;
        }

        if ($("#PaymentTransactionForm").valid()) {
            if (confirm("Are you sure you want to Save the details ?")) {
                blockPage();
                $.ajax({
                    type: "POST",
                    url: "/EAuthorization/AddPaymentTransactionDetails/" + Bill_ID,
                    //async: false,
                    data: $("#PaymentTransactionForm").serialize(),

                    error: function (xhr, status, error) {
                        unblockPage();
                        alert("Error while Processing");

                        return false;
                    },
                    success: function (data) {
                        unblockPage();
                        if (data.Success) {

                            alert("e-Authorization Transaction Details Added Successfully");

                            $("#PaymentGridDivList").jqGrid().setGridParam
                            ({ url: '/EAuthorization/GetPaymentDetailList/' + Bill_ID, datatype: "json", page: 1 }).trigger("reloadGrid");

                            $('#EAuthorizationList').trigger('reloadGrid');

                            $("#tblFinalize").show();


                            //Show Transaction Form After Ever Save...so Add button is Hide
                            $('#EAuthorizationTransactionFormPlaceHolder').load('/EAuthorization/GetEAuthDetailsEntryForm/' + Bill_ID, function () {

                                $("#trShowHideLinkTable").show('slow');
                                $("#TransactionForm").show('slow');
                                $("#EAuthorizationTransactionFormPlaceHolder").show('slow');
                            });

                            $("#btnAddNewEAuthorizationDetails").hide();


                        }
                        else if (data.Success == false && data.status == "-6") {
                            alert("Error Occur While Adding EAuthorization Details");
                        }
                        else if (data.Success == false && data.status == "-1") {
                            alert("This transaction cant be added ,master payment is already finalized");
                            return false;
                        }
                        else if (data.Success == false && data.status == "-2") {
                            alert("Please Select Contractor");
                            return false;
                        }
                        else if (data.Success == false && data.status == "-20") {
                            alert("Please Select Package");
                            return false;
                        }
                        else if (data.Success == false && data.status == "-45") {
                            alert("Invalid Package,Please Select Another Package");
                            return false;
                        }


                        else if (data.Success == false && data.status == "-3") {
                            alert("Please Select Agreement Number");
                            return false;
                        }
                        else if (data.Success == false && data.status == "-4") {
                            alert("Please Enter Authorization Request Amount");
                            return false;
                        }
                        else if (data.Success == false && data.status == "-5") {
                            alert("e-Authorization Request Amount must be greater than 0");
                            return false;

                        }
                        else if (data.Success == false && data.status == "-66") {
                            alert("Please Enter Amount already Authorized");
                            return false;

                        }

                        else if (data.Success == false && data.status == "-88") {
                            alert("Details already entered against selected Package,Please select Another Package");
                            return false;

                        }

                        else if ((data.Success == false && data.status == "-7")) {
                            alert("Sum of Authorization Request Amount and Amount Already Authorized Should be less than Agreement Amount");
                            return false;
                        }
                        else if ((data.Success == false && data.status == "-77")) {
                            alert("Contractor bank details not present.");
                            return false;
                        }

                        else {
                            alert("Error in Adding Payment Detials,Please try Again");
                            return false;

                        }

                    }



                });
            }
        }


    });








});



function autoFillAgreementList() {


    if ($('#MAST_CON_ID_C').val() > 0) {

        $.ajax({
            type: 'POST',
            url: '/EAuthorization/PopulateAgreement?MastConID=' + $('#MAST_CON_ID_C option:selected').val(),
            async: false,
            cache: false,
            dataType: 'json',
            success: function (data) {

                $('#IMS_AGREEMENT_CODE_C').empty();
                $.each(data, function (i, office) {
                    $("#IMS_AGREEMENT_CODE_C").append('<option value="' + office.Value + '">' +
                        office.Text + '</option>');

                });

            },
            error: function () {
                alert("Error in Processing");


            }
        });
    }

}


function setPayeeName(ConID) {
    $.ajax({
        type: "POST",
        url: "/EAuthorization/SetPayeeName/" + ConID,
        async: false,

        error: function (xhr, status, error) {
            alert("Error Occur while Processing");
            return false;

        },
        success: function (data) {
            $("#PAYEE_NAME").val(data);
            $("#PAYEE_NAME").attr('readonly', 'readonly');
        }
    });
}

function FillInCascadeDropdown(map, dropdown, action) {
    $(dropdown).empty()
    $.post(action, map, function (data) {
        $.each(data, function () {
            if (this.Selected == true)
            { $(dropdown).append("<option value='" + this.Value + "' selected =" + this.Selected + ">" + this.Text + "</option>"); }
            else { $(dropdown).append("<option value='" + this.Value + "'>" + this.Text + "</option>"); }
        });
    }, "json");
}

function PopulateAgreementExpenditureAmount(strAmtValue) {
    $.ajax({
        url: '/EAuthorization/PopulateAgreementExpenditureAmount/',
        type: 'GET',
        catche: false,
        data: { "strAmtValue": strAmtValue.toString() },
        async: false,
        success: function (data) {
            if (data.Success) {
                $("#trAgreementAmount").show();
                $("#trExpenditureAmount").show();

                //$("#lblAgreementAmount").text(data.tend_Aggrement_Amount);
                $("#lblAgreementAmount").text(data.tend_Aggrement_Amount);
                $("#lblExpenditureAmount").text(data.Expenditure_Amount);
                //$("#lblExpenditureAmount").text(data.Expenditure_Amount);
                $("#AGREEMENT_AMOUNT").val(data.tend_Aggrement_Amount);
                $("#EXPENDITURE_AMOUNT").val(data.Expenditure_Amount);
            } else {
                alert(data.errMessage);
            }
        },
        error: function () {
            alert("An Error while Processing,Please try Again");
            return false;
        },
    });

}


//ANS..on 
function CheckAlreadyAuthorisedAmount(strAmtValue) {
    $.ajax({
        url: '/EAuthorization/CheckAlreadyAuthorisedAmount/',
        type: 'GET',
        catche: false,
        data: { "PackageValue": strAmtValue.toString() },
        async: false,
        success: function (data) {
            if (data.Success) {
                if (data.AuthorisedAmount == 0) {

                    //Package NIL Check

                    //if ($('#IMS_SANCTION_PACKAGE').val() == "NIL") {
                    //    $("#trAlreadyAuthAmount").show();
                    //    $("#lnkAlreadyAutAmount").hide();

                    //    return;

                    //}


                    //If not Already Entry ..show link
                    $("#trAlreadyAuthAmount").show();
                    $("#lnkAlreadyAutAmount").show();
                    $("#txtAlreadyAutAmount").hide();
                    $("#trAlreadyAuthAmount").show();
                    $("#ALREADY_AUTHORISED_AMOUNT").val("");
                    $("#AMOUNT_Q").val("");
                } else {

                    //If Already Entry ..show ReadOnly
                    $("#trAlreadyAuthAmount").show();
                    $("#txtAlreadyAutAmount").show();
                    $("#ALREADY_AUTHORISED_AMOUNT").val(data.AuthorisedAmount);
                    $("#ALREADY_AUTHORISED_AMOUNT").attr('readonly', 'readonly');
                    $("#lnkAlreadyAutAmount").hide();
                }
            } else {
                alert(data.errMessage);
            }
        },
        error: function () {
            alert("An Error while Processing,Please try Again");
            return false;
        },
    });

}
function getContratorBankDetails(mastConID) {
    $.ajax({
        type: "POST",
        url: "/payment/GetContratorBankNameAccNoAndIFSCcode/" + mastConID + '$' + $("#fundType").val() + '$' + "0",
        async: false,
        error: function (xhr, status, error) {
            unblockPage();
            return false;
        },
        success: function (data) {
            unblockPage();
            if (data.Success == true) {
               
                $("#trContractorBankDetails").show();
                $("#spnBankAccNumber").html(data.BankAccNumber);
                $("#spnIFSCCode").html(data.BankIFSCCode);
                $("#spnBankName").html(data.BankName);
                $('#conAccountId').val(data.BankAccountId);
            }
            else if (data.Success == false) {
                if (data.message != undefined) {
                    alert(data.message);
                }
                else {
                    alert("Contractor bank details not present.");
                }
                $("#spnBankAccNumber").html("-");
                $("#spnIFSCCode").html("-");
                $("#spnBankName").html("-");
            }
            else {
                alert("An error ocured while proccessing your request.");
                $("#spnBankAccNumber").html("-");
                $("#spnIFSCCode").html("-");
                $("#spnBankName").html("-");
            }
        }
    });
}
function GetAddEAuthorizationLinkView(ConIDAggrementPackageValue) {
    $.ajax({
        type: "GET",
        url: '/EAuthorization/GetAddEAuthorizationLinkView/',
        data: { "ConIDAggrementPackageValue": ConIDAggrementPackageValue.toString() },
        async: false,
        error: function (xhr, status, error) {
            unblockPage();
            return false;
        },
        success: function (response) {
            unblockPage();
            //Show Dialog UI
            htmlRes = response;
            $('#dialogAddAuthorizationDetails').dialog('open');

            $("span.ui-dialog-title").text('e-Authorization Payment Details');
        }

    });

}