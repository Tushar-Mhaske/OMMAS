$(document).ready(function () {
    var htmlRes;
    $.validator.unobtrusive.parse($('#frmEAuthRequestList'));
    $("#dialog").dialog({
        open: function (event, ui) {
            console.log('dialog open');
            $("#divloadReport").html(htmlRes);
            $(event.target).parent().css('background-color', 'white');
            $(event.target).parent().css('color', 'white');
        },
        
        width: 1830,
        height:500,
        autoOpen: false,
        position :{
            my: "left top",
            at: "left top",
            of: "#mainDiv"
        },

        
    });
    $("#dvEauthNoNote").hide();
    $("#divEauthDetailNote1").hide();
    LoadGrid("", "", "","Load");

    $("#ADMIN_ND_CODE").change(function () {

        var firstParent = $("#divEAuth").parent().attr('id');
        var dialogID = $("#" + firstParent).parent().attr('id');
        $("#btnSendEmail").hide();
        $('#' + dialogID).dialog('close');
        $("#dvDetailGrid").hide();
        $("#tblSave").hide();
        if ($("#ADMIN_ND_CODE").val() != "0") {
            $("#ADMIN_ND_CODE").removeClass("input-validation-error").addClass("input-validation-valid");
            $("#tdDpiu").find('span:eq(0)').text("").hide().removeClass("field-validation-error").addClass("field-validation-valid");
        }
        else {
            $("#ADMIN_ND_CODE").removeClass("input-validation-valid").addClass("input-validation-error");
            $("#tdDpiu").find('span:eq(0)').text("Please select DPIU").show().removeClass("field-validation-valid").addClass("field-validation-error");
        }
    });


    //On Change of Status/DPIU/Month/Year Detail Grid Should be Hide
    $("#ddlMonth").change(function () {

        var firstParent = $("#divEAuth").parent().attr('id');
        var dialogID = $("#" + firstParent).parent().attr('id');
        $("#btnSendEmail").hide();
        $('#' + dialogID).dialog('close');


        $("#dvDetailGrid").hide();
        $("#tblSave").hide();
    });


    $("#ddlYear").change(function () {
        var firstParent = $("#divEAuth").parent().attr('id');
        var dialogID = $("#" + firstParent).parent().attr('id');
        $("#btnSendEmail").hide();
        $('#' + dialogID).dialog('close');



        $("#dvDetailGrid").hide();
        $("#tblSave").hide();
    });

    $("#EAUTH_STATUS1").change(function () {

        var firstParent = $("#divEAuth").parent().attr('id');
        var dialogID = $("#" + firstParent).parent().attr('id');
        $("#btnSendEmail").hide();
        $('#' + dialogID).dialog('close');


        $("#dvDetailGrid").hide();
        $("#tblSave").hide();
    });

    $("#btnView").click(function () {
        if ($('#frmEAuthRequestList').valid()) {
            LoadGrid($("#ddlMonth").val(), $("#ddlYear").val(), $("#ADMIN_ND_CODE").val(), "", $("#EAUTH_STATUS1").val());
        }

    });

    //To View REPORT on Browser Not Used now
    $("#btnViewReport").click(function () {
        $.ajax({
            url: '/EAuthorization/GetSRRDAEAuthorizationReport/',
            type: 'POST',
            catche: false,
            data: { "EncryptedEAuthID": $("#hidEncId").val().toString() },
            async: false,
            success: function (response) {
               // $.unblockUI();
                $("#divloadReport").html(response);

            },
            error: function () {
                //$.unblockUI();
                alert("An Error Occur while Processing,please try Again");

                return false;
            },
        });
    });


    //When Approval/Rejected Count=0 then Show Continue Button..
    $("#btnContinue").click(function () {
        //if Status is 'Y'...then First it Should Save.....

        

        $.ajax({
            url: '/EAuthorization/CheckSendMailStatus/',
            type: 'POST',
            catche: false,
            data: { "EncryptedEAuthID": $("#hidEncId").val().toString() },
            async: false,
            success: function (data) {

                unblockPage();
                if (data.Success) {
                    
                    if (data.status == "Y") {  //if Status is Y then Save Value AND Proceed..
                        //Getting Encrypted EAuthID From Hidden Field
                        var EncryptedHiddenEAuthID = $("#hidEncId").val();
                        var ArrrovedArr = new Array();
                        var RejectArr = new Array();
                        var selectRdoBtn = "";

                        //Radio button Class
                        $(".clsRemark").each(function () {
                            var RadioID = $(this).attr('id');
                            //alert(RadioID);       //1          1          2                2            3                3              ID
                            var RadioValue = $(this).attr('value');
                            //alert(RadioValue);  //EAuth_A   EAuth_R   EAuth_A           EAuth_R       EAuth_A           EAuth_R         VALUES
                            selectRdoBtn = $('input[name=EAuthRemark_' + RadioID + ']:checked').val();
                            if (selectRdoBtn == 'EAuth_A') {

                                //var str = RadioID;
                                var txtRemark = "txtRemark_" + RadioID;
                                var txtValue = $("#" + txtRemark).val();
                                var str = RadioID + "$*_~-~_*$" + txtValue;
                                ArrrovedArr.push(str);

                            }
                            else if (selectRdoBtn == 'EAuth_R') {

                                var txtRemark = "txtRemark_" + RadioID;
                                var txtValue = $("#" + txtRemark).val();

                               
                                


                                //if (txtValue == null || txtValue == undefined) {
                                    
                                //    return;
                                //}


                                var str = RadioID + "$*_~-~_*$" + txtValue;
                                RejectArr.push(str);

                            }
                            else {
                               

                            }


                        });
                        var ArrrovedArr1 = new Array();
                        var RejectArr1 = new Array();
                        //Only Single Array
                        ArrrovedArr1 = ArrrovedArr.filter(function (item, pos) {
                            return ArrrovedArr.indexOf(item) == pos;
                        });

                        RejectArr1 = RejectArr.filter(function (item, pos) {
                            return RejectArr.indexOf(item) == pos;
                        });
                        var ArrrovedArr2 = ArrrovedArr1.join('$%*~_~*%$');
                        var RejectArr12 = RejectArr1.join('$%*~_~*%$');
                        var token = $('input[name=__RequestVerificationToken]').val();

                        if (confirm("Are you sure you want to Proceed For e-Authorization ?")) {
                            $.ajax({
                                type: "GET",
                                dataType: "json",
                                url: '/EAuthorization/ProceedForApproveRejectDetails',
                                data: { "ApproveArr": ArrrovedArr2.toString(), "RejectArr": RejectArr12.toString(), "EncryptedEAuthID": EncryptedHiddenEAuthID.toString(), "__RequestVerificationToken": token },
                                cache: false,
                                error: function (xhr, status, error) {
                                    //unBlockPage();
                                    alert("Error in processing,Please try Again");
                                    return false;

                                },
                                success: function (data) {


                                    if (data.success) {
                                        alert(data.ErrMessage);
                                        $('#tblEAuthSRRDARequestGrid1').trigger('reloadGrid');   //Reload Details Grid After Saving Approval Status
                                        $('#tblEAuthSRRDARequestGrid').trigger('reloadGrid');   //Reload Details Grid After Saving Approval Status
                                        $.ajax({
                                            url: '/EAuthorization/GetSRRDAEAuthorizationReport/',
                                            type: 'POST',
                                            catche: false,
                                            data: { "EncryptedEAuthID": $("#hidEncId").val().toString() },
                                            async: false,
                                            success: function (response) {
                                                htmlRes = response;
                                                $('#dialog').dialog('open');
                                            },
                                            error: function () {
                                                //$.unblockUI();
                                                alert("An Error Occur while Processing,please try Again");

                                                return false;
                                            },
                                        });

                                        $("#tblSave").hide();

                                    } else {
                                        alert(data.ErrMessage);
                                    }
                                }

                            });
                        }

                    }
                    else {//Show  Report...

                        $.ajax({
                            url: '/EAuthorization/GetSRRDAEAuthorizationReport/',
                            type: 'POST',
                            catche: false,
                            data: { "EncryptedEAuthID": $("#hidEncId").val().toString() },
                            async: false,
                            success: function (response) {
                                htmlRes = response;
                                $('#dialog').dialog('open');
                                CheckSendMailStatus($("#hidEncId").val().toString());  //In SRRDAReport Layout
                            },
                            error: function () {
                                //$.unblockUI();
                                alert("An Error Occur while Processing,please try Again");

                                return false;
                            },
                        });

                    }

                } else {
                    alert("Error Occur while Processing ,Please try Again ");
                    return;
                }
            },
            error: function () {
                $.unblockUI();
                alert("An Error Occur while Processing,please try Again");

                return false;
            },
        });



      
    });

    




    //Save Status  AND Show Report on Browser to Send Mail
    $("#btnProceed").click(function () {
        //Getting Encrypted EAuthID From Hidden Field
        var EncryptedHiddenEAuthID=$("#hidEncId").val();
        var ArrrovedArr = new Array();
        var RejectArr = new Array();
        var selectRdoBtn = "";
        //Radio button Class
        $(".clsRemark").each(function () {
            var RadioID = $(this).attr('id');
            //alert(RadioID);       //1          1          2                2            3                3              ID
            var RadioValue = $(this).attr('value');
            //alert(RadioValue);  //EAuth_A   EAuth_R   EAuth_A           EAuth_R       EAuth_A           EAuth_R         VALUES

            selectRdoBtn = $('input[name=EAuthRemark_' + RadioID + ']:checked').val();
            if (selectRdoBtn == 'EAuth_A') {

                //var str = RadioID;
                var txtRemark = "txtRemark_" + RadioID;
                var txtValue = $("#" + txtRemark).val();
                var str = RadioID + "$*_~-~_*$" + txtValue;
                ArrrovedArr.push(str);

            }
            else if (selectRdoBtn == 'EAuth_R') {

                var txtRemark = "txtRemark_" + RadioID;
                var txtValue = $("#" + txtRemark).val();


                //if (txtValue == null || txtValue == undefined) {
                //    //alert("Please Enter Remark");
                //    return;
                //}

                var str = RadioID + "$*_~-~_*$" + txtValue;
                RejectArr.push(str);

            }
            else {
                alert("Error in processing,Please try Again");

            }


        });


        //alert(ArrrovedArr);
        //alert(RejectArr);


        var ArrrovedArr1 = new Array();
        var RejectArr1 = new Array();

        //Only Single Array

        ArrrovedArr1 = ArrrovedArr.filter(function (item, pos) {
            return ArrrovedArr.indexOf(item) == pos;
        });

        RejectArr1 = RejectArr.filter(function (item, pos) {
            return RejectArr.indexOf(item) == pos;
        });


        var ArrrovedArr2 = ArrrovedArr1.join('$%*~_~*%$');
        var RejectArr12 = RejectArr1.join('$%*~_~*%$');
        
        ///^([a-z0-9]{5,})$/.test('abc1');   // false


        //var pattern = /[a-zA-Z\&.]/g
        //if (pattern.test(RejectArr12)) {

        //} else {
        //    alert(pattern.test(txtValue));
        //    alert("Special Symbols are not Allowed in Remark")
        //}



        





        var token = $('input[name=__RequestVerificationToken]').val(); 
        if (confirm("Are you sure you want to Proceed For e-Authorization ?")) {
            blockPage();
            $.ajax({
                type: "GET",
                dataType: "json",
                url: '/EAuthorization/ProceedForApproveRejectDetails',
                data: { "ApproveArr": ArrrovedArr2.toString(), "RejectArr": RejectArr12.toString(), "EncryptedEAuthID": EncryptedHiddenEAuthID.toString(), data: { "__RequestVerificationToken": token }, },
                cache: false,
                //contentType: "application/json; charset=utf-8",
                error: function (xhr, status, error) {
                    unBlockPage();
                    alert("Error in processing,Please try Again");
                    return false;

                },
                success: function (data) {
                    unblockPage();

                    if (data.success) {
                        alert(data.ErrMessage);
                        $('#tblEAuthSRRDARequestGrid1').trigger('reloadGrid');   //Reload Details Grid After Saving Approval Status
                        $('#tblEAuthSRRDARequestGrid').trigger('reloadGrid');   //Reload Details Grid After Saving Approval Status

                        if (data.ApprovalStatus == "R") {

                            $("#tblSave").hide();
                        } else {
                            $.ajax({
                                url: '/EAuthorization/GetSRRDAEAuthorizationReport/',
                                type: 'POST',
                                catche: false,
                                // data: $("#frmAnaProposal").serialize(),
                                data: { "EncryptedEAuthID": $("#hidEncId").val().toString() },
                                async: false,
                                success: function (response) {


                                    htmlRes = response;

                                    $('#dialog').dialog('open');



                                },
                                error: function () {
                                    //$.unblockUI();
                                    alert("An Error");
                                    return false;
                                },
                            });
                        }

                        $("#tblSave").hide();




                    } else {
                        alert(data.ErrMessage);
                    }


                }

            });
        }

    });



});






//Master Grid:-EO
function LoadGrid(month, year, districtCode,Load,Status) {

    $("#tblEAuthSRRDARequestGrid").jqGrid('GridUnload')

    blockPage();

    jQuery("#tblEAuthSRRDARequestGrid").jqGrid({
        url: '/EAuthorization/SRRDAeAuthorizationRequestListData/',
        datatype: 'json',
        mtype: 'POST',
        height: 'auto',
        rowNum: 15,
        postData: { 'months': month, 'year': year, 'AdminNDCode': districtCode, 'Load': Load, 'Status': Status },
        rownumbers: true,
        autowidth: true,
        pginput: false,
        rowList: [15, 20, 30],
        colNames: ['DPIU Name', 'e-Authorization Number', 'e-Authorization Request Date', 'Requested Amount (In Lakhs.)', 'Status', 'Action Date', 'View Details'],
        colModel: [

           {
               name: 'DPIU_Name',
               index: 'DPIU_Name',
               width: 90,
               align: "center",
               frozen: true

           },
           {
               name: 'EAuth_No',
               index: 'EAuth_No',
               width: 80,
               align: "center",
               frozen: true,

           },


             {
                 name: 'EAuth_Date',
                 index: 'EAuth_Date',
                 width: 40,
                 align: "center"

             },

           {
               name: 'EAuth_RequestAmt',
               index: 'EAuth_RequestAmt',
               width: 80,
               align: "center"

           },

             {
                 name: 'Status',
                 index: 'Status',
                 width: 80,
                 align: "Center"

             },
              {
                  name: 'Action_Date',
                  index: 'Action_Date',
                  width: 40,
                  align: "Center"

              },
               {
                   name: 'View_Details',
                   index: 'View_Details',
                   width: 40,
                   align: "Center"

               }



        ],
        pager: "#divEAuthSRRDARequestPager",
        viewrecords: true,
        loadComplete: function (xhr, st, err) {
            unblockPage();


            $("#tblEAuthSRRDARequestGrid").jqGrid('setLabel', "rn", "Sr.</br> No");

        },

        sortname: 'EAuth_No',
        sortorder: "asc",
        caption: "e-Authorization List"


    });




}


function ViewEAuthorizationDetailsForApproval(EAuthID) {
    $("#dvDetailGrid").show();
    $("#tblEAuthSRRDARequestGrid1").jqGrid('GridUnload')
    LoadSRRDATransactionGrid(EAuthID);
    $("#tblSave").show();

    var firstParent = $("#divEAuth").parent().attr('id');
    var dialogID = $("#" + firstParent).parent().attr('id');

    $("#btnSendEmail").hide();
    $('#' + dialogID).dialog('close');

 

   



}

//Details Grid:-EO
function LoadSRRDATransactionGrid(EAuthID) {
    blockPage();
    jQuery("#tblEAuthSRRDARequestGrid1").jqGrid({

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
        colNames: ['Company Name', 'Payee Name', 'Agreement No', 'Package No', 'Amount Already Authorized(In Lakhs.)','Agreement Amount(In Lakhs.)','Requested Amount (In Lakhs.)', 'Status/Action'],
        colModel: [
             {
                 name: 'Company_name',
                 index: 'Company_name',
                 width: 380,
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
                width: 280,
                align: "center",



            },

            {
                name: 'Package_no',
                index: 'Package_no',
                width: 150,
                align: "center",



            },



            {
                name: 'Amount_Already_Authorized',
                index: 'Amount_Already_Authorized',
                width: 130,
                align: "center",



            },


            {
                name: 'Àgreement_Amount',
                index: 'Àgreement_Amount',
                width: 130,
                align: "center",



            },


            {
                name: 'Authorization_Request_Amount',
                index: 'Authorization_Request_Amount',
                width: 130,
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
        pager: "#divEAuthSRRDARequestPager1",
        viewrecords: true,
        loadComplete: function (data) {
            unblockPage();
           

            $('#tblEAuthSRRDARequestGrid1').jqGrid('setGridWidth', $("#gview_tblEAuthSRRDARequestGrid1").width());

            $("#hidEncId").val(data.hidEncId);

            $("#hidStatus").val(data.Status);
            $("#hid_ApprovalRejectCount").val(data.Count);

            $('#lbleAuthNO').text(data.EAUTHNO);
            
            var firstChildClass = $("#gview_tblEAuthSRRDARequestGrid1").children().children('.ui-jqgrid-title').text("e-Authorization Request Details:" + data.EAUTHNO);

            $("#divEauthDetailNote1").hide('slow');

            
            if (data.Status == "R") {
                $("#tblSave").hide();
                $("#btnContinue").hide();
                $("#btnProceed").hide();
            }


            //Count >=1 means Approved/Rejected So Show Continue button
            if (data.Count >= 1) {
                $("#btnContinue").show();
                $("#btnProceed").hide();
            } else {
                $("#btnContinue").hide();
                $("#btnProceed").show();
            }


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



function RadioClick(EncTxNo, TxNo) {
    var id = "txtRemark_" + TxNo;
    $("#" + id).hide();

}


function RadioClick1(EncTxNo, TxNo) {
    var id = "txtRemark_" + TxNo;
    $("#" + id).show();

}





//function validateRemark(EncTxNo, TxNo) {


//    var id = "txtRemark_" + TxNo;
//    var Remarkvalue = $("#" + id).val();

//    var regex = /[a-zA-Z\.\&]/;

//    alert(regex.test(Remarkvalue));
//    if (regex.test(Remarkvalue)) {
//        alert("Special Symbols are not allowed in Remark");
//    }


//}

