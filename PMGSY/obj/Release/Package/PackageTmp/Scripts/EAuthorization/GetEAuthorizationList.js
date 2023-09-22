
$(document).ready(function () {


    $("#divEauthDetailNote").hide();
    
    var htmlRes;

    $("#maindialog").dialog({
        open: function (event, ui) {
            console.log('dialog open');
            $("#dvViewDetails").html(htmlRes);
        },
        width: 823,
        autoOpen: false,
      
    });


    GetClosedMonthAndYear();

    $("#btnAddNewAuthorizationDetails").click(function () {

        $("#mainDiv").load('/EAuthorization/MasterEAuthorizationDetails/' + $('#months').val() + '$' + $('#year').val() + '$' + "A");
    });

    loadEAuthorizationGrid("view", "");

    $("#btnViewFirstListing").click(function () {
        if ($('#months').val() == 0) {
            alert("please select month");
            return false;
        }

        if ($('#year').val() == 0) {
            alert("please select year");
            return false;
        }

        $('#EAuthList').jqGrid('GridUnload');
        loadEAuthorizationGrid("view", "");

    });

});

//Master Grid on GetEAuthorization list Page
function loadEAuthorizationGrid(mode, paramAuthStatus) {
    blockPage();
    
    $("#EAuthList").jqGrid('GridUnload');

    jQuery("#EAuthList").jqGrid({

        url: '/EAuthorization/EAuthorizationRequestListView/',
        datatype: 'json',
        mtype: 'POST',
        height: 'auto',
        rowNum: 15,
        postData: { 'mode': mode, 'months': $('#months').val(), 'year': $('#year').val(), 'fromDate': $('#fromDate').val(), 'toDate': $('#toDate').val(), 'EAUTH_STATUS': paramAuthStatus },

        rownumbers: true,
        //width: 1150,
        autowidth: true,
        pginput: false,
        //shrinkToFit: false,
        rowList: [15, 20, 30],
        //colNames: ['Authorization Number ', 'Authorization Date', 'Transaction Type', 'Bank Authorization</br>Request Amount (In Rs.)', 'Cash Amount (In Rs.)', 'Gross Amount (In Rs.)', 'Status', 'Edit', 'Delete', 'Finalize'],
        colNames: ['EAuthorization Number ', 'EAuthorization Date', 'EAuthorization Status', 'Requested Amount (In Lakhs.)','Approved Amount(In Lakhs.)','Approval/Rejected Date', 'Edit', 'Delete', 'Finalize'],
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
                  //80
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

        //For highlighting selected Row
        onSelectRow: function (id) {

            
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




function EditEAuthorizationMaster(EAuth) {


    //After Edit button redirect to master page and Display grid and Transaction form

    $('#mainDiv').load('/EAuthorization/MasterEAuthorizationDetails/' + EAuth, function () {

        $('#EAuthorizationTransactionFormPlaceHolder').load('/EAuthorization/GetEAuthDetailsEntryForm/' + EAuth, function () {

            $("#trShowHideLinkTable").show();
            $("#TransactionForm").show('slow');
            $("#EAuthorizationTransactionFormPlaceHolder").show('slow');

            $("#btnAddNewEAuthorizationDetails").hide();


        });



    });




}



function DeleteEAuthorizationMaster(EAuth) {


    if (confirm("Are you sure you want to Delete the details ?")) {
        blockPage();
        var token = $('input[name=__RequestVerificationToken]').val();

        $.ajax({
            type: "POST",
            url: "/EAuthorization/DeleteEAuthorizationMaster/" + EAuth,
            data: { "__RequestVerificationToken": token },
            async: false,
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
                    $('#EAuthList').trigger('reloadGrid');
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
               // unBlockPage();
                alert("Error in processing,Please try Again");
                return false;

            },
            success: function (data) {
                //unBlockPage();
                if (data.Success) {
                    alert("EAuthorization Request Details Finalized Successfully.");
                    $('#EAuthList').trigger('reloadGrid');

                } else {
                    alert(data.errMessage);
                }

            }

        });


    }


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



function GetEAuthorizationDetailsViewAfterFinalize(EauthID) {
    loadTransactionGrid(EauthID)
}

function loadTransactionGrid(EAuthID) {

    blockPage();

    $("#TransactionList").jqGrid('GridUnload')

    jQuery("#TransactionList").jqGrid({

        url: '/EAuthorization/GetSRRDAeAuthDetailListForApproval/' + EAuthID,
        datatype: 'json',
        mtype: 'POST',
        height: 'auto',
        rowNum: 15,
        rownumbers: true,
        //width:$("#gview_PaymentMasterList").width(),
        autowidth: true,
        pginput: false,
        pgbuttons: false,
        //shrinkToFit: true,
        //rowList: [15, 20, 30],
        colNames: ['Company Name', 'Payee Name', 'Agreement No', 'Package No', 'Amount Already Authorized(In Lakhs.)', 'Agreement Amount(In Lakhs.)', 'Requested Amount<br/>(In Lakhs.)','Status/Action'],
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
                width: 250,
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
                width: 180,
                align: "center",



            },



            {
                name: 'Amount_Already_Authorized',
                index: 'Amount_Already_Authorized',
                width: 100,
                align: "center",



            },


            {
                name: 'Àgreement_Amount',
                index: 'Àgreement_Amount',
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
                name: 'Status',
                index: 'Status',
                width: 210,
                align: "Center",
                //formatter: 'currencyFmatter'


            }


        ],
        pager: "#Transactionpager",
        viewrecords: true,
        loadComplete: function (data) {
            unblockPage();

            $('#TransactionList').jqGrid('setGridWidth', $("#gview_TransactionList").width());
            //$("#hidEncId").val(data.hidEncId);

            $("#hiddenAUTH_NO").val(data.EAUTHNO);
            $('#lblEAuthNO').text(data.EAUTHNO);

            $("#divEauthDetailNote").hide('slow');
            $("#gview_TransactionList").children().children('.ui-jqgrid-title').text("e-Authorization Request Details:" + data.EAUTHNO);

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
        caption: "e-Authorization Request Details" 
    });

}
