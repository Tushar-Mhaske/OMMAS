$(document).ready(function () {
    //Make EAuthorization Read Only
    $('#EAUTHORIZATION_DATE').attr("readonly", "readonly");

    //Get Last Closing Month
    GetClosedMonthAndYear();

    //Get Bank Eauthorization Available In Order to Display in e-Auth MAster Form
    GetBankAuthorizationAvailable($("#BILL_MONTH").val(), $("#BILL_YEAR").val());

    $("#btnGoToListPage").click(function () {
        $("#mainDiv").load("/EAuthorization/GetEAuthorizationList/", function () {
            //unblockPage();
        });
        //Commented on 04-12-2018
        //$('#EAuthorizationList').trigger('reloadGrid');
    });

    $("#EAUTHORIZATION_DATE").datepicker("enable");

    //Master Form EAuth DatePicker Configuration
    $("#EAUTHORIZATION_DATE").datepicker({
        showOn: 'button',
        buttonImage: '/Content/images/calendar_2.png',
        buttonImageOnly: true,
        changeMonth: false,
        changeYear: false,
        dateOnly: false,
        dateFormat: "dd/mm/yy",
        maxDate: new Date(),
        buttonText: 'EAuthorization Date',
        minDate: new Date(),
        maxDate: new Date(),
        onClose: function () {
            $(this).focus().blur();
        }
    });

    $("#EAUTHORIZATION_DATE").attr('readonly', 'readonly');

    if ($("#EAUTHORIZATION_DATE").val() == '' || $("#EAUTHORIZATION_DATE").val() == 0) {
        var selectedDate = $("#CURRENT_DATE").val().split('/');
        var day = selectedDate[0];
        var month = selectedDate[1];
        var year = selectedDate[2];
        ModifiedCurrentDate = ModifiedDate(day, month, year);
        $("#EAUTHORIZATION_DATE").datepicker('setDate', process(ModifiedCurrentDate));
    }
    var currentDate = $("#CURRENT_DATE").val().split("/");
    var currentDay = currentDate[0];
    var ModifiedCurrentDate = ModifiedDate(currentDate[0], $("#BILL_MONTH").val(), $("#BILL_YEAR").val());

    //Master Month On Change
    $("#BILL_MONTH").change(function () {

        

        GetBankAuthorizationAvailable($("#BILL_MONTH").val(), $("#BILL_YEAR").val());

        
        
        if (!($("#EAUTHORIZATION_DATE").is(":disabled"))) {

            if ($("#EAUTHORIZATION_DATE").val() == 0) {
                $("#EAUTHORIZATION_DATE").datepicker('setDate', process($("#CURRENT_DATE").val()));

            } else {
                if ($("#EAUTHORIZATION_DATE").val() != '') {
                    var selectedDate = $("#EAUTHORIZATION_DATE").val().split('/');
                    var day = selectedDate[0];
                    ModifiedCurrentDate = ModifiedDate(day, $("#BILL_MONTH").val(), $("#BILL_YEAR").val());

                    //Not allow to change date 
                    //$("#EAUTHORIZATION_DATE").datepicker('setDate', process(ModifiedCurrentDate)).attr("readonly", "readonly");;

                    //$("#EAUTHORIZATION_DATE").datepicker('setDate', process(ModifiedCurrentDate));


                }
                else {
                    ModifiedCurrentDate = ModifiedDate(currentDate[0], $("#BILL_MONTH").val(), $("#BILL_YEAR").val());

                    //Not allow to change date 
                    //$("#EAUTHORIZATION_DATE").datepicker('setDate', process(ModifiedCurrentDate)).attr("readonly", "readonly");;

                    //$("#EAUTHORIZATION_DATE").datepicker('setDate', process(ModifiedCurrentDate));

                }
            }
        }

        



        if ($("#BILL_MONTH").val() != 0 && $("#BILL_YEAR").val() != 0) {
            $.ajax({
                type: "GET",
                url: "/EAuthorization/GetAuthorizationNumber/" + $("#BILL_MONTH").val() + "$" + $("#BILL_YEAR").val(),
                async: false,
                error: function (xhr, status, error) {
                    unblockPage();
                    return false;
                },
                success: function (data) {
                    unblockPage();
                    $('#EAUTHORIZATION_NO').val(data).attr("readonly", "readonly");
                    

                }
            });
        } else {


            return false;
        }




    });

    //Master Year on Change
    $("#BILL_YEAR").change(function () {

        GetBankAuthorizationAvailable($("#BILL_MONTH").val(), $("#BILL_YEAR").val());

        if (!($("#EAUTHORIZATION_DATE").is(":disabled"))) {
            if ($("#EAUTHORIZATION_DATE").val() == 0) {
                $("#EAUTHORIZATION_DATE").datepicker('setDate', process($("#CURRENT_DATE").val()));

            } else {
                if ($("#EAUTHORIZATION_DATE").val() != '') {
                    var selectedDate = $("#EAUTHORIZATION_DATE").val().split('/');
                    var day = selectedDate[0];
                    ModifiedCurrentDate = ModifiedDate(day, $("#BILL_MONTH").val(), $("#BILL_YEAR").val());

                    //Not allow to change date 
                    //$("#EAUTHORIZATION_DATE").datepicker('setDate', process(ModifiedCurrentDate)).attr("readonly", "readonly");;
                    //$("#EAUTHORIZATION_DATE").datepicker('setDate', process(ModifiedCurrentDate));

                }
                else {
                    ModifiedCurrentDate = ModifiedDate(currentDate[0], $("#BILL_MONTH").val(), $("#BILL_YEAR").val());

                    //Not allow to change date 
                    //$("#EAUTHORIZATION_DATE").datepicker('setDate', process(ModifiedCurrentDate));

                    //$("#EAUTHORIZATION_DATE").datepicker('setDate', process(ModifiedCurrentDate)).attr("readonly", "readonly");;

                }
            }
        }


        //Getting Authorization Number:Master Form
        if ($("#BILL_YEAR").val() != 0 && $("#BILL_MONTH").val() != 0) {

            $.ajax({
                type: "GET",
                url: "/EAuthorization/GetAuthorizationNumber/" + $("#BILL_MONTH").val() + "$" + $("#BILL_YEAR").val(),
                async: false,
                error: function (xhr, status, error) {
                    unblockPage();
                    return false;
                },
                success: function (data) {
                    unblockPage();
                    $('#EAUTHORIZATION_NO').val(data).attr("readonly", "readonly");

                }
            });
        } else {


            return false;
        }





    });

    $('#btnGoToListPage1').click(function () {

        $("#mainDiv").load("/EAuthorization/GetEAuthorizationList/", function () {
            //unblockPage();
        });

        //$('#EAuthorizationList').trigger('reloadGrid');

    });


    $("#BILL_MONTH").change(function () {
        $("#lblMonth1").hide();
    });

    $("#BILL_YEAR").change(function () {
        $("#lblMonth1").hide();
    });

    $("#btnMasterReset").click(function () {
        $("#lblMonth1").hide();
    });

    $('#HideShowTransaction').click(function () {

        $("#TransactionForm").toggle('slow', function () { });

        $('#iconSpan').toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
    });


    //Save Master e-Authorization Details
    $("#btnSaveAuthorizationDetails").click(function (e) {
        var currentDate = $("#EAUTHORIZATION_DATE").val().split("/");
        var currentDay = parseInt(currentDate[0]);
        var currentMonth = parseInt(currentDate[1]);
        var currentYear = currentDate[2];
        var billYear = $("#BILL_YEAR").val();
        var billMonth = $("#BILL_MONTH").val();

        var d = new Date();
        var day = parseInt(d.getDate());
        var year = parseInt(d.getFullYear());
        var month = parseInt(d.getMonth() + 1);

        if (currentDay != day) {
            alert("e-Authorization Date Should be Todays Date");
            return;
        }
        else if (currentMonth != month) {
            alert("e-Authorization Date Should be Todays Date");
            return;
        }
        else if (currentYear != year) {
            alert("e-Authorization Date Should be Todays Date");
            return;
        }


        if (currentMonth == billMonth) {
        } else {
            alert("e-Authorization Date must be within selected month and year");
            return;
        }

        if (currentYear == billYear) {


        } else {
            alert("e-Authorization Date must be within selected month and year");
            return;
        }

        e.preventDefault();

        $.validator.unobtrusive.parse($("#masterEAuthorizationForm"));
        if ($("#masterEAuthorizationForm").valid()) {
            if (confirm("Are you sure you want to save the details ?")) {

                blockPage();
                $.ajax({
                    type: "POST",
                    url: "/EAuthorization/AddEAuthorizationMasterDetails",
                    async: false,
                    data: $("#masterEAuthorizationForm").serialize(),

                    error: function (xhr, status, error) {
                        unblockPage();
                        return false;
                    },
                    success: function (data) {
                        unblockPage();

                        if (data.success) {
                            //blockPage();
                            alert("e-Authorization Master Details added.");
                            $("#lblMonth1").hide();

                            loadEAuthorizationGrid(data.Auth_Id);

                            $("#btnGoToListPage1").show();

                            $('#EAuthorizationTransactionFormPlaceHolder').load('/EAuthorization/GetEAuthDetailsEntryForm/' + data.Auth_Id, function () {

                                $("#trShowHideLinkTable").show();
                                $("#TransactionForm").show('slow');
                                $("#EAuthorizationTransactionFormPlaceHolder").show('slow');
                                $("#divPlaceAddEditEAuthorizationDetails").hide('slow');

                                loadPaymentGrid(data.Auth_Id); 
                            });

                        }
                        else {
                            if (data.Bill_ID == "-1") {
                                alert("Error in Adding e-Authorization Mater Details,Please try Again");
                            }
                            else if (data.Bill_ID == "-11") {
                                alert("e-Authorization Date Should be Todays Date");
                            }
                            else if (data.Bill_ID == "-111") {
                                alert("e-Authorization request Entry can only be made in Programme fund");
                            }
                            else if (data.Bill_ID == "-2") {
                                alert("Error in Processing,please try again");

                            }
                            else if (data.Bill_ID == "-26") {
                                alert("Error in Processing,Please try Again");

                            }
                            else if (data.Bill_ID == "-23") {  
                                alert(data.StatusMessage);  //Invalid EAuthorization Number(Tampered)

                            }
                            else {
                                //alert("Error in Processing");
                                $("#lblMonth1").show();
                                $("#lblMonth1").text(data.ErrMessage);
                                //alert(data.ErrMessage);

                            }

                        }




                    }
                });
            }

        }


    });


});


function process(date) {

    var parts = date.split("/");

    return new Date(parts[2], parts[1] - 1, parts[0]);
}

function ModifiedDate(day, month, year) {
    return day + "/" + month + "/" + year;
}




//Load Grid After adding Master Grid
function loadEAuthorizationGrid(Auth_Id) {
    blockPage();


    jQuery("#EAuthorizationList").jqGrid({

        url: '/EAuthorization/ListEAuthorizationRequestForDataEntry/ ',
        datatype: 'json',
        mtype: 'POST',
        height: 'auto',
        rowNum: 15,

        postData: { 'EAUTH_ID': Auth_Id },

        rownumbers: true,
        //width: 1150,
        autowidth: true,
        pginput: false,
        //shrinkToFit: false,
        rowList: [15, 20, 30],
        //colNames: ['Authorization Number ', 'Authorization Date', 'Transaction Type', 'Bank Authorization</br>Request Amount (In Rs.)', 'Cash Amount (In Rs.)', 'Gross Amount (In Rs.)', 'Status', 'Edit', 'Delete', 'Finalize'],
        colNames: ['EAuthorization Number ', 'EAuthorization Date', 'EAuthorization Status', 'Requested Amount (In Lakhs.)', 'Approved Amount(In Lakhs.)', 'Approval/Rejected Date', 'Edit', 'Delete', 'Finalize'],
        colModel: [

            {
                name: 'EAuth_Number',
                index: 'EAuth_Number',
                width: 90,
                align: "center",
                frozen: true

            },
            {
                name: 'EAuth_date',
                index: 'EAuth_date',
                width: 40,
                //80  40
                align: "center",
                frozen: true,

            },

             {
                 name: 'EAuth_Status',
                 index: 'EAuth_Status',
                 width: 80,
                 align: "center"

             },



              {
                  name: 'RequestAmount',
                  index: 'RequestAmount',
                  width: 80,
                  //80 20
                  align: "center"

              },


                {
                    name: 'ApprovedAmount',
                    index: 'ApprovedAmount',
                    width: 80,
                    align: "center"

                },

              {
                  name: 'Approved_RjctDate',
                  index: 'Approved_RjctDate',
                  width: 40,
                  align: "center"

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
                   width: 40,
                   align: "Center"

               },
                {
                    name: 'Finalize',
                    index: 'Finalize',
                    width: 40,
                    align: "Center"

                }



        ],
        pager: "#EAuthpager",
        viewrecords: true,
        loadError: function (xhr, st, err) {
            unblockPage();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            return false;
        },
        loadComplete: function (xhr, st, err) {
            unblockPage();


            $("#EAuthList").jqGrid('setLabel', "rn", "Sr.</br> No");

        },
        sortname: 'EAuth_Number',
        sortorder: "asc",
        caption: "e-Authorization Request Details"
    });



    //jQuery("#EAuthList").jqGrid('setGroupHeaders', {
    //    useColSpanStyle: true,
    //    groupHeaders: [
    //      { startColumnName: 'Edit', numberOfColumns: 2, titleText: 'Action' }

    //    ]
    //});
}


//Detils Grid:
function loadPaymentGrid(param_Bill_ID) {

    blockPage();
    jQuery("#PaymentGridDivList").jqGrid({

        url: '/EAuthorization/GetPaymentDetailList/' + param_Bill_ID,
        datatype: 'json',
        mtype: 'GET',
        height: 'auto',
        rowNum: 15,
        rownumbers: true,
        //width:$("#gview_PaymentMasterList").width(),
        autowidth: true,
        pginput: false,
        pgbuttons: false,
        //shrinkToFit: true,
        //rowList: [15, 20, 30],
        colNames: ['Company Name', 'Payee Name', 'Agreement Number', 'Package No', 'Agreement Amount(In lakhs.)', 'Amount Already Authorized(In lakhs.)', 'Authorization Request Amount(In Lakhs.)', 'Edit', 'Delete'],
        colModel: [
             {
                 name: 'Company_name',
                 index: 'Company_name',
                 width: 400,
                 align: "center",

             },

            {
                name: 'Payee_name',
                index: 'Payee_name',
                width: 200,
                align: "center",

            },

            {
                name: 'Agreement_no',
                index: 'Agreement_no',
                width: 300,
                align: "center",



            },

            {
                name: 'Package_no',
                index: 'Package_no',
                width: 100,
                align: "center",



            },

            {
                name: 'Agreement_amount',
                index: 'Agreement_amount',
                width: 120,
                align: "center",



            },

            {
                name: 'Amount_Already_Authorized',
                index: 'Amount_Already_Authorized',
                width: 120,
                align: "center",



            },
            {
                name: 'Authorization_Request_Amount',
                index: 'Authorization_Request_Amount',
                width: 120,
                align: "center",



            },

            {
                name: 'Edit',
                index: 'Edit',
                width: 50,
                align: "Center",


            },

            {
                name: 'Delete',
                index: 'Delete',
                width: 50,
                align: "Center",


            }


        ],
        pager: "#PaymentGridDivpager",
        viewrecords: true,
        loadComplete: function () {
            unblockPage();
            // var myGrid = $("#PaymentGridDivList"),
            //width = myGrid.jqGrid('getGridParam', 'width'); // get current width
            // myGrid.jqGrid('setGridWidth', width, true);

            $('#PaymentGridDivList').jqGrid('setGridWidth', $("#gview_PaymentGridDivList").width());

        },
        loadError: function (xhr, st, err) {
            unblockPage();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');
            return false;
        },
        sortname: 'Company_name',
        grouping: true,
        groupingView: {
            //groupField: ['Company_name'],
            groupColumnShow: [false],
            groupText: ['<b>{0}</b>']
            //,groupCollapse: true
        },
        sortorder: "asc",
        caption: "Payment Details"
    });



}




function GetClosedMonthAndYear() {
    blockPage();

    $.ajax({
        type: "POST",
        url: "/MonthlyClosing/GetClosedMonthandYear/",
        // async: false,

        error: function (xhr, status, error) {
            unblockPage();
            $('#errorSpan').text(xhr.responseText);
            $('#divError').show('slow');

            return false;

        },
        success: function (data) {
            unblockPage();

            if (data.monthClosed) {
                $("#lblMonth").text(data.month);
                $("#lblYear").text(data.year);

                $("#TrMonthlyClosing").show('Slow');
                $("#AccountNotClosedTr").hide('Slow');
                return false;
            }
            else if (data.monthClosed == false) {
                $("#AccountNotClosedTr").show('Slow');
                $("#TrMonthlyClosing").hide('Slow');
                return false;
            }
            else {

                alert("Error While getting Monthly Closing Details");
                return false;
            }

        }
    });


}


//Function to Display Bank Authorization Available:Master Form
function GetBankAuthorizationAvailable(month, year) {

    $.ajax({
        type: "POST",
        url: "/EAuthorization/GetBankAuthorizationAvailable/" + month + "$" + year,
        async: false,
        error: function (xhr, status, error) {
            //unblockPage();

            alert("Error");
            return false;
        },
        success: function (data) {
            // unblockPage();

            if (data != "") {
                balanceCashAmount = data.Cash;
                balanceChequeAmount = data.BankAuth;
                $("#lblbankAuthAvailable").text(parseFloat(data.BankAuth).toFixed(2));
            }

            
        }
    });


}