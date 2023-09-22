$(document).ready(function () {

    
    $("#btnGoToListPage1").click(function () {
        
        blockPage();
        $("#mainDiv").load("/EAuthorization/GetEAuthorizationList/", function () {
            unblockPage();
            //$('#EAuthorizationList').trigger('reloadGrid');
        });

       // loadEAuthorizationGrid("view", "");

        //Commented on 04-12-2018
        //$('#EAuthorizationList').trigger('reloadGrid');


    });


  






    $("#btnGoToListPage1").hide();

    if (operation == "A") {
        

        $('#divPlaceAddEditEAuthorizationDetails').load('/EAuthorization/AddEditEAuthorizationDetails/' + month + '$' + year, function () {
            $("#masterEAuthorizationListGrid").hide();
            $("#trShowHideLinkTable").hide();
            $("#EAuthorizationMasterDataEntryDiv").show('slow');

            
            
        });
    }
    else if (operation == "E") {
        
        $("#ALREADY_AUTHORISED_AMOUNT").show();
        loadEAuthorizationGrid(Bill_ID);
        $(".innerDivHeader").hide();
        $("#trShowHideLinkTable").hide();
        $("#btnAddNewEAuthorizationDetails").show();
        loadPaymentGrid(Bill_ID);

        $("#btnGoToListPage1").show();
        if (cntEntryInDetailsForFinalizedButton == 0) {
            $("#tblFinalize").hide();
        }
        else {
            $("#tblFinalize").show();
        }
        




    }



    $("#btnAddNewEAuthorizationDetails").click(function () {
        $('#EAuthorizationTransactionFormPlaceHolder').load('/EAuthorization/GetEAuthDetailsEntryForm/' + Bill_ID, function () {

            $("#trShowHideLinkTable").show();
            $("#TransactionForm").show('slow');
            $("#EAuthorizationTransactionFormPlaceHolder").show('slow');


        });
    });


   


    $("#btnfinalize").click(function () {

        if (confirm("Are you sure you want to Finalize the details ?")) {
            //blockPage();
            var token = $('input[name=__RequestVerificationToken]').val();
            $.ajax({
                type: "POST",
                url: "/EAuthorization/FinalizeEAuthorizationDetails/" + Bill_ID,
                data: { "__RequestVerificationToken": token },
                async: false,
                //data: $("#PaymentTransactionForm").serialize(),
                error: function (xhr, status, error) {
                   // unBlockPage();
                    alert("Error in processing,Please try Again");
                    return false;

                },
                success: function (data) {
                   // unBlockPage();
                    if (data.Success) {
                        alert("e-Authorization Request Details Finalized Successfully.");

                        $("#mainDiv").load("/EAuthorization/GetEAuthorizationList/", function () {
                            //unblockPage();
                        });



                        $("#tblFinalize").hide();
                        $("#btnAddNewEAuthorizationDetails").hide();



                    } else {
                        alert(data.errMessage);
                    }

                }

            });


        }


    });

   

});



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
                  //80  20
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
            //$('#errorSpan').text(xhr.responseText);
            //$('#divError').show('slow');
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
   
}


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
        colNames: ['Company Name', 'Payee Name', 'Agreement Number', 'Package No', 'Agreement Amount', 'Amount Already Authorized(In Lakhs.)', 'Authorization Request Amount(In Lakhs.)', 'Edit', 'Delete'],
        colModel: [
             {
                 name: 'Company_name',
                 index: 'Company_name',
                 width: 500,
                 align: "center",

             },

            {
                name: 'Payee_name',
                index: 'Payee_name',
                width: 250,
                align: "center",

            },

            {
                name: 'Agreement_no',
                index: 'Agreement_no',
                width: 320,
                align: "center",



            },

            {
                name: 'Package_no',
                index: 'Package_no',
                width: 180,
                align: "center",



            },

            {
                name: 'Agreement_amount',
                index: 'Agreement_amount',
                width: 100,
                align: "center",



            },

            {
                name: 'Amount_Already_Authorized',
                index: 'Amount_Already_Authorized',
                width: 100,
                align: "center",



            },
            {
                name: 'Authorization_Request_Amount',
                index: 'Authorization_Request_Amount',
                width: 100,
                align: "center",



            },

            {
                name: 'Edit',
                index: 'Edit',
                width: 90,
                align: "Center",


            },

            {
                name: 'Delete',
                index: 'Delete',
                width: 90,
                align: "Center",


            }


        ],
        pager: "#PaymentGridDivpager",
        viewrecords: true,
        loadComplete: function () {
            unblockPage();

            $('#PaymentGridDivList').jqGrid('setGridWidth', $("#gview_PaymentGridDivList").width());

        },
        loadError: function (xhr, st, err) {
            unblockPage();
            //$('#errorSpan').text(xhr.responseText);
            //$('#divError').show('slow');
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
        caption: "e-Authorization Details"
    });



}





function EditEAuthorizationDetail(id) {
    $('#EAuthorizationTransactionFormPlaceHolder').load('/EAuthorization/EditEAuthorizationDetails/' + id, function () {

        $("#trShowHideLinkTable").show();
        $("#TransactionForm").show('slow');
        $("#EAuthorizationTransactionFormPlaceHolder").show('slow');
        $("#trAggreement").show('slow');
        $("#trPackage").show('slow');

        //Hide add button 
        $("#btnAddNewEAuthorizationDetail").hide();

        

        
    });

}




function DeleteEAuthorizationMaster(EAuth) {


    if (confirm("Are you sure you want to Delete the details ?")) {
        blockPage();
        var token = $('input[name=__RequestVerificationToken]').val();
        $.ajax({
            type: "POST",
            url: "/EAuthorization/DeleteEAuthorizationMaster/" + EAuth,
            async: false,
            //data: $("#PaymentTransactionForm").serialize(),
            data: { "__RequestVerificationToken": token },
            error: function (xhr, status, error) {
                unBlockPage();
                alert("Error in processing,Please try Again");
                return false;

            },
            success: function (data) {
                unblockPage();
                if (data.Success) {
                    
                    alert("e-Authorization Request Details Deleted Successfully.");


                    $('#EAuthorizationList').trigger('reloadGrid');
                    $('#PaymentGridDivList').trigger('reloadGrid');
                    $("#tblFinalize").hide();
                    $("#btnAddNewEAuthorizationDetails").hide();
                    $("#mainDiv").load("/EAuthorization/GetEAuthorizationList/", function () {
                        //unblockPage();
                    });
                    
                }
                else {
                    alert(data.errMessage);
                }


            }


        });

    }



}





function DeleteEAuthorizationDetail(id) {


    if (confirm("Are you sure you want to Delete the details ?")) {
        blockPage();
        var token = $('input[name=__RequestVerificationToken]').val();
        $.ajax({
            type: "POST",
            url: "/EAuthorization/DeleteEAuthorizationDetails/" + id,
            async: false,
            data: { "__RequestVerificationToken": token },
            //data: $("#PaymentTransactionForm").serialize(),
            error: function (xhr, status, error) {
                unBlockPage();
                alert("Error in processing,Please try Again");
                return false;

            },
            success: function (data) {
                unblockPage();
                if (data.Success) {
                    alert("e-Authorization Request Details Deleted Successfully.");
                    $('#EAuthorizationList').trigger('reloadGrid');
                    $("#PaymentGridDivList").jqGrid().setGridParam
                      ({ url: '/EAuthorization/GetPaymentDetailList/' + Bill_ID, datatype: "json", page: 1 }).trigger("reloadGrid");
                    if (data.Count == 0) {
                        $("#tblFinalize").hide();
                    }

                    // unblockPage();
                }
                else {
                    alert(data.errMessage);
                }


            }


        });

    }
}



function FinalizeEAuthorizationMaster(EAuth) {

    if (confirm("Are you sure you want to Finalize the details ?")) {
        //blockPage();
        var token = $('input[name=__RequestVerificationToken]').val();
        $.ajax({
            type: "POST",
            url: "/EAuthorization/FinalizeEAuthorizationDetails/" + EAuth,
            data: { "__RequestVerificationToken": token },
            async: false,
            error: function (xhr, status, error) {
                //unBlockPage();
                alert("Error in processing,Please try Again");
                return false;

            },
            success: function (data) {
               // unBlockPage();

                if (data.Success) {
                    alert("e-Authorization Request Details Finalized Successfully.");
                    $('#EAuthorizationList').trigger('reloadGrid');
                    $('#PaymentGridDivList').trigger('reloadGrid');
                    $("#tblFinalize").hide();
                    $("#btnAddNewEAuthorizationDetails").hide();
                    $("#mainDiv").load("/EAuthorization/GetEAuthorizationList/", function () {
                        //unblockPage();
                    });




                } else {
                    alert(data.errMessage);
                }

            }

        });


    }


}



