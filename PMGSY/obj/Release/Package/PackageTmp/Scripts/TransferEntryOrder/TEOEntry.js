var isMasterGridLoaded = false;
var isDetailsGridLoaded = false;
var amountValC = 0;
var amountValD = 0;

var amountValC_Credit = 0;
var amountValD_Credit = 0;
var amountValC_Debit = 0;
var amountValD_Debit = 0;



var isImprestEntry = "False";
$(document).ready(function () {
    //var to store if imprest entry 
    isImprestEntry = $("#ImprestEntry").val();

    if (isImprestEntry == "True") {

        $("#teoHeader").text("Settlement of imprest");

    }

    $(document).unbind('keydown').bind('keydown', function (event) {
        var doPrevent = false;
        if (event.keyCode === 8) {
            var d = event.srcElement || event.target;
            if ((d.tagName.toUpperCase() === 'INPUT' && (d.type.toUpperCase() === 'TEXT' || d.type.toUpperCase() === 'PASSWORD'))
                 || d.tagName.toUpperCase() === 'TEXTAREA') {
                doPrevent = d.readOnly || d.disabled;
            }
            else {
                doPrevent = true;
            }
        }

        if (doPrevent) {
            event.preventDefault();
        }
    })

    if (billId == 0) {
        //$("#loadTEOMaster").load("/TEO/TEOMaster/");
        blockPage();
        $.ajax({
            url: "/TEO/TEOMaster/",
            type: "GET",
            async: false,
            cache: false,
            data:
                {
                    "Month": month,
                    "Year": year,
                    //Avinash_Start
                    "EncryptedEAuthID": $('#EncryptedEAuthID').val()
                    //Avinash_End
                },
            success: function (data) {
                unblockPage();
                $("#loadTEOMaster").html(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                unblockPage();
                alert(xhr.responseText);
            }
        });
    }
    else {
        LoadTEOMasterGrid(billId);
        LoadTEODetailsGrid(billId);
    }
    //else if (isFinalize == 'N') {
    //    LoadTEOMasterGrid(billId);
    //    LoadTEODetailsGrid(billId);
    //}
    //else if (isFinalize == 'Y') {
    //    LoadTEOMasterGrid(billId);
    //    LoadTEODetailsGrid(billId);
    //}

    $("#btnFinalizeTEO").click(function () {

        if (confirm("Are you sure you want to finalize TEO?")) {
            blockPage();
            $.ajax({
                url: "/TEO/FinalizeTEO/" + billId,
                type: "POST",
                async: false,
                cache: false,
                error: function () {

                    unblockPage();
                },
                success: function (data) {
                    unblockPage();

                    if (data.success) {
                        alert("TEO has been finalized");

                        if (isImprestEntry == "False") {
                            strUrl = "/TEO/TEOList/"
                        }
                        else {
                            strUrl = "/TEO/TEOImprest/"
                        }

                        $.ajax({
                            url: strUrl,
                            type: "GET",
                            async: false,
                            cache: false,
                            data:
                                {
                                    "Month": month,
                                    "Year": year
                                },
                            success: function (data) {
                                unblockPage();
                                $("#mainDiv").html(data);
                            },
                            error: function (xhr, ajaxOptions, thrownError) {
                                unblockPage();
                                alert(xhr.responseText);
                            }
                        });

                        //new change done by Abhishek kamble on 16-Oct-2014                        
                        //Master TXN 1530- Transfer of DPR Expenses between two DPIUs 
                        if (levelId == 4 && parseInt(_MasterTxnID) == 1530) {
                            $.ajax({
                                url: '/TEO/AddAutoEntryTOB/' + billId,
                                type: "POST",
                                async: false,
                                cache: false,
                                //data:
                                //    {
                                //        "Month": month,
                                //        "Year": year
                                //    },
                                success: function () {
                                    unblockPage();
                                    //$("#mainDiv").html(data);
                                    return true;
                                },
                                error: function (xhr, ajaxOptions, thrownError) {
                                    unblockPage();
                                    alert(xhr.responseText);
                                }
                            });
                        }

                        //end of change

                        return false;
                    }
                    else {
                        alert(data.message);
                        return false;
                    }
                }
            });
            return false;
        }
        else {
            return false;
        }
    });

    $("#lblBackToList").click(function () {
        //$("#mainDiv").load("/TEO/TEOList/");
        var strUrl = "";

        if (isImprestEntry == "False") {
            strUrl = "/TEO/TEOList/"
        }
        else {
            strUrl = "/TEO/TEOImprest/"
        }

        blockPage();

        $.ajax({
            url: strUrl,
            type: "GET",
            async: false,
            cache: false,
            data:
                {
                    "Month": month,
                    "Year": year
                },
            success: function (data) {
                unblockPage();

                $("#mainDiv").html(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                unblockPage();

                alert(xhr.responseText);
            }
        });


    });

    $("#lblBackToImprest").click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/TEO/TEOImprest/",
            type: "GET",
            async: false,
            cache: false,
            data:
                {
                    "Month": month,
                    "Year": year
                },
            success: function (data) {
                $.unblockUI();

                $("#mainDiv").html(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                $.unblockUI();

                alert(xhr.responseText);
            }
        });
    });
});

function LoadTEOMasterGrid(MasterId) {
    if (isMasterGridLoaded) {
        $("#tblTEOMasterGrid").GridUnload();
        isMasterGridLoaded = false;
    }

    jQuery("#tblTEOMasterGrid").jqGrid({
        url: '/TEO/TEOMasterList',
        datatype: "json",
        mtype: "POST",
        colNames: ['TEO Number', 'TEO Date', 'Transaction Name', 'Gross Amount', 'Edit', 'Delete'],
        colModel: [
                            { name: 'TEONumber', index: 'TEONumber', width: 100, align: 'center', sortable: true },
                            { name: 'TEODate', index: 'TEODate', width: 100, align: 'center', sortable: true },
                            { name: 'TransactionName', index: 'TransactionName', width: 200, align: 'center', sortable: true },
                            { name: 'GrossAmount', index: 'GrossAmount', width: 100, align: 'right', sortable: true },
                            { name: 'Edit', index: 'Edit', width: 50, align: 'center', sortable: false },
                            { name: 'Delete', index: 'Delete', width: 50, align: 'center', sortable: false }
        ],
        postData: {
            'masterId': MasterId
        },
        viewrecords: true,
        caption: "TEO Master",
        height: 'auto',
        autowidth: true,
        sortname: 'TEONumber',
        rownumbers: true,
        loadComplete: function () {
            isMasterGridLoaded = true;
            //Added By Abhishek Kamble 11-nov-2013
            $('#tblTEOMasterGrid_rn').html('Sr.<br/>No.');
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
            }
        }

    }); //end of documents grid
}

function LoadTEODetailsGrid(MasterId) {
    if (isDetailsGridLoaded) {
        $("#tblTEODetailsGrid").GridUnload();
        isDetailsGridLoaded = false;
    }
    jQuery("#tblTEODetailsGrid").jqGrid({
        url: '/TEO/TEODetailsList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Transaction Number', 'Type', 'Head code', 'Head Name', 'Contractor Name', 'Agreement', 'Road Name', 'DPIU', 'Credit Amount', 'Debit Amount', 'Narration', 'Edit', 'Delete', 'Status'],
        colModel: [
                            { name: 'TransactionNumber', index: 'TransactionNumber', width: 0, align: 'center', sortable: true, hidden: true },
                            { name: 'CreditDebit', index: 'CreditDebit', width: 60, align: 'center'/*, sortable: true */ },
                            { name: 'AccHeadcode', index: 'AccHeadcode', width: 0, align: 'center', sortable: true, hidden: true },
                            { name: 'HeadName', index: 'HeadName', width: 200, align: 'left', sortable: true },
                            { name: 'Contractor', index: 'Contractor', width: 125, align: 'left', sortable: true, hidden: fundType == 'A' ? true : false },//, cellattr: function (rowId, tv, rawObject, cm, rdata) { return 'style="white-space: normal;' } 
                            { name: 'Agreement', index: 'Agreement', width: 125, align: 'left', sortable: true, hidden: fundType == 'A' ? true : false },
                            { name: 'Road', index: 'Road', width: 100, align: 'left', sortable: true, hidden: fundType == 'A' ? true : false },//, cellattr: function (rowId, tv, rawObject, cm, rdata) { return 'style="white-space: normal;' } 
                            { name: 'DPIU', index: 'DPIU', width: 100, align: 'left', sortable: true, cellattr: function (rowId, tv, rawObject, cm, rdata) { return 'style="white-space: normal;' } },
                            { name: 'CAmount', index: 'CAmount', width: 70, align: 'right', sortable: true, cellattr: function (rowId, tv, rawObject, cm, rdata) { return 'style="white-space: normal;' } },
                            { name: 'DAmount', index: 'DAmount', width: 70, align: 'right', sortable: true, cellattr: function (rowId, tv, rawObject, cm, rdata) { return 'style="white-space: normal;' } },
                            { name: 'Narration', index: 'Narration', width: 100, align: 'left', sortable: false },
                            { name: 'Edit', index: 'Edit', width: 30, align: 'center', sortable: false },
                            { name: 'Delete', index: 'Delete', width: 35, align: 'center', sortable: false },
                            { name: 'Status', index: 'Status', width: 50, align: 'center', sortable: false }

        ],
        pager: jQuery('#divTEODetailsPager'),
        rowNum: 10,
        postData: {
            'masterId': MasterId
        },
        altRows: true,
        rowList: [10, 20, 50],
        viewrecords: true,
        recordtext: '{2} records found',
        emptyrecords: 'No records to view',
        sortname: 'CreditDebit',
        sortorder: "asc",
        caption: "TEO Details",
        height: 'auto',
        autowidth: true,//'770px',
        rownumbers: true,
        footerrow: true,
        userDataOnFooter: true,
        loadComplete: function () {
            isDetailsGridLoaded = true;
            $("#tblTEODetailsGrid").find('a').click(function () {
                var selRowId = $(this).parents('tr').attr('id');
                var data = $("#tblTEODetailsGrid").getRowData(selRowId);
                amountValC = data['CAmount'];
                amountValD = data['DAmount'];

                if (data['CreditDebit'] == 'Credit') {
                    amountValC_Credit = data['CAmount'];
                    amountValD_Credit = data['DAmount'];
                }
                else {
                    amountValC_Debit = data['CAmount'];
                    amountValD_Debit = data['DAmount'];
                }



            });
            //// This code is to show Finalize button
            var userdata = $("#tblTEODetailsGrid").getGridParam('userData');
            // userData.isFinalize checked for current data entry and isFinalize is checked for whether TEO already finalized

            if (userdata.isFinalize == "Y" && isFinalize != 'Y') {
                $("#loadTEOCreditDetails").html('');
                $("#loadTEODebitDetails").html('');
                $("#divFinalizeTEO").show('slow');
                $("#btnFinalizeTEO").show('slow');

            }
            else if (isFinalize == 'Y') {
                $("#loadTEOCreditDetails").html('');
                $("#loadTEODebitDetails").html('');
            }
            else {
                $("#divFinalizeTEO").hide('slow');
                $("#btnFinalizeTEO").hide('slow');

                isMulTxn = userdata.multipleTrans;

                if (isMulTxn == "Y" && $('#tblTEODetailsGrid').jqGrid('getGridParam', 'reccount') == 0) {
                    $("#loadTEOCreditDetails").load("/TEO/TEODetails/" + billId + "/C", function () {
                        $("#loadTEODebitDetails").load("/TEO/TEODetails/" + billId + "/D", function () {
                            $.each($("select"), function () {
                                if ($(this).find('option').length >= 1) {
                                    $('#tr' + $(this).attr('id')).show();
                                }
                            });
                        });
                        $.each($("select"), function () {
                            if ($(this).find('option').length >= 1) {
                                $('#tr' + $(this).attr('id')).show();
                            }
                        });
                    });
                }
                else if (isMulTxn == "Y" && $('#tblTEODetailsGrid').jqGrid('getGridParam', 'reccount') == 1) {
                    var rowdata = $("#tblTEODetailsGrid").getRowData($("#tblTEODetailsGrid").getDataIDs()[0]);
                    var vpCHead = 0;
                    var vpDHead = 0;
                    var vpDistRepeat = "N";
                    var vpDPIURepeat = "N";
                    var vpContRepeat = "N";
                    var vpSupRepeat = "N";
                    var vpAggRepeat = "N";
                    var vpRoadRepeat = "N";
                    var vpHeadRepeat = "N";

                    $.ajax({
                        url: "/TEO/GetTEOValidationParams/" + billId,
                        type: "POST",
                        async: false,
                        cache: false,
                        success: function (data) {
                            vpCHead = data.CHead;
                            vpDHead = data.DHead;
                            vpDistRepeat = data.District;
                            vpDPIURepeat = data.DPIU;
                            vpContRepeat = data.Contractor;
                            vpSupRepeat = data.Supplier;
                            vpAggRepeat = data.Agreement;
                            vpRoadRepeat = data.Road;
                            vpHeadRepeat = data.Head;


                            if (rowdata["CreditDebit"] == "Credit") {
                                $("#loadTEODebitDetails").load("/TEO/TEODetails/" + billId + "/D", function (data) {

                                    $.each($("select"), function () {
                                        if ($(this).find('option').length >= 1) {
                                            $('#tr' + $(this).attr('id')).show();
                                        }
                                    });

                                    var contID = 0;

                                    if ($("#ddlHeadContractorD").find('option').length >= 1) {
                                        contID = $("#ddlHeadContractorD").val();


                                    }
                                    else if ($("#ddlContractorD").find('option').length >= 1) {
                                        contID = $("#ddlContractorD").val();

                                    }

                                    $.ajax({
                                        url: "/Receipt/GetContractorName/" + contID,
                                        type: "POST",
                                        async: false,
                                        cache: false,
                                        success: function (data) {
                                            if ($("#ddlHeadContractorD").find('option').length >= 1) {
                                                $("#HeadConSupNameD").text(data);
                                                $("#trHeadConSupNameD").show('slow');
                                            }
                                            else if ($("#ddlContractorD").find('option').length >= 1) {
                                                $("#ConSupNameD").text(data);
                                                $("#trConSupNameD").show('slow');
                                            }

                                        },
                                        error: function (xhr, ajaxOptions, thrownError) {
                                            alert(xhr.responseText);
                                        }
                                    });

                                    if (vpDistRepeat == "Y") {
                                        $("#ddlDistrictD").attr('disabled', 'disabled');
                                    }
                                    if (vpDPIURepeat == "Y") {
                                        $("#ddlDPIUD").attr('disabled', 'disabled');
                                    }
                                    if (vpContRepeat == "Y" || vpSupRepeat == "Y") {
                                        $("#ddlContractorD").attr('disabled', 'disabled');
                                        $("#ddlHeadContractorD").attr('disabled', 'disabled');
                                    }
                                    if (vpAggRepeat == "Y") {
                                        $("#ddlAgreementD").attr('disabled', 'disabled');
                                        $("#ddlHeadAgreementD").attr('disabled', 'disabled');
                                    }
                                    if (vpRoadRepeat == "Y") {
                                        $("#ddlRoadD").attr('disabled', 'disabled');
                                        $("#ddlHeadRoadD").attr('disabled', 'disabled');
                                    }
                                    if (vpHeadRepeat == "Y") {
                                        $("#ddlHeadD").attr('disabled', 'disabled');
                                    }

                                    $("#loadTEOCreditDetails").load("/TEO/TEODetails/" + billId + "/C", function () {
                                        $.each($("select"), function () {
                                            if ($(this).find('option').length >= 1) {
                                                $('#tr' + $(this).attr('id')).show();
                                            }
                                        });


                                        //set credit side contractor and agreement also selected
                                        if ($("#ddlHeadContractorC").is(":visible")) {
                                            $("#ddlHeadContractorC").val(contID);
                                        }
                                        else {
                                            $("#ddlContractorC").val(contID);
                                        }


                                        if (vpContRepeat == "Y" || vpSupRepeat == "Y") {
                                            //set disable
                                            $("#ddlHeadContractorC").attr('disabled', 'disabled');
                                            $("#ddlContractorC").attr('disabled', 'disabled');

                                            // set the contractor name

                                            if ($("#ddlHeadContractorC").find('option').length >= 1) {
                                                $("#HeadConSupNameC").text($("#HeadConSupNameD").text());


                                                $("#trHeadConSupNameC").show('slow');

                                            }
                                            else if ($("#ddlContractorC").find('option').length >= 1) {
                                                $("#ConSupNameC").text($("#ConSupNameD").text());

                                                $("#trConSupNameC").show('slow');
                                            }

                                        }
                                        if (vpAggRepeat == "Y") {

                                            if ($("#ddlHeadAgreementC").find('option').length >= 1) {
                                                $('#ddlHeadAgreementC').html($('#ddlHeadAgreementD').html());
                                                $("#ddlHeadRoadC").html($('#ddlHeadRoadD').html());
                                            }
                                            else if ($("#ddlAgreementC").find('option').length >= 1) {
                                                $('#ddlAgreementC').html($('#ddlAgreementD').html());
                                                $("#ddlRoadC").html($('#ddlRoadD').html());
                                            }

                                            $("#ddlAgreementC").attr('disabled', 'disabled');
                                            $("#ddlHeadAgreementC").attr('disabled', 'disabled');

                                        }

                                    });//end load loadTEOCreditDetails credit

                                });
                            }
                            else {

                                $("#loadTEOCreditDetails").load("/TEO/TEODetails/" + billId + "/C", function () {
                                    $.each($("select"), function () {
                                        if ($(this).find('option').length >= 1) {
                                            $('#tr' + $(this).attr('id')).show();
                                        }
                                    });

                                    var contID = 0;

                                    if ($("#ddlHeadContractorC").find('option').length >= 1) {
                                        contID = $("#ddlHeadContractorC").val();
                                    }
                                    else if ($("#ddlContractorC").find('option').length >= 1) {
                                        contID = $("#ddlContractorC").val();
                                    }

                                    $.ajax({
                                        url: "/Receipt/GetContractorName/" + contID,
                                        type: "POST",
                                        async: false,
                                        cache: false,
                                        success: function (data) {
                                            if ($("#ddlHeadContractorC").find('option').length >= 1) {
                                                $("#HeadConSupNameC").text(data);
                                                $("#trHeadConSupNameC").show('slow');
                                            }
                                            else if ($("#ddlContractorC").find('option').length >= 1) {
                                                $("#ConSupNameC").text(data);
                                                $("#trConSupNameC").show('slow');
                                            }
                                        },
                                        error: function (xhr, ajaxOptions, thrownError) {
                                            alert(xhr.responseText);
                                        }
                                    });
                                    if (vpDistRepeat == "Y") {
                                        $("#ddlDistrictC").attr('disabled', 'disabled');
                                    }
                                    if (vpDPIURepeat == "Y") {
                                        $("#ddlDPIUC").attr('disabled', 'disabled');
                                    }
                                    if (vpContRepeat == "Y" || vpSupRepeat == "Y") {
                                        $("#ddlContractorC").attr('disabled', 'disabled');
                                        $("#ddlHeadContractorC").attr('disabled', 'disabled');
                                    }
                                    if (vpAggRepeat == "Y") {
                                        $("#ddlAgreementC").attr('disabled', 'disabled');
                                        $("#ddlHeadAgreementC").attr('disabled', 'disabled');
                                    }
                                    if (vpRoadRepeat == "Y") {
                                        $("#ddlRoadC").attr('disabled', 'disabled');
                                        $("#ddlHeadRoadC").attr('disabled', 'disabled');
                                    }
                                    if (vpHeadRepeat == "Y") {
                                        $("#ddlHeadC").attr('disabled', 'disabled');
                                    }

                                    $("#loadTEODebitDetails").load("/TEO/TEODetails/" + billId + "/D", function () {
                                        $.each($("select"), function () {
                                            if ($(this).find('option').length >= 1) {
                                                $('#tr' + $(this).attr('id')).show();
                                            }
                                        });

                                        //set credit side contractor and agreement also selected
                                        if ($("#ddlHeadContractorD").is(":visible")) {
                                            $("#ddlHeadContractorD").val(contID);
                                        }
                                        else {
                                            $("#ddlContractorD").val(contID);
                                        }

                                        if (vpContRepeat == "Y" || vpSupRepeat == "Y") {
                                            //set disable
                                            $("#ddlHeadContractorD").attr('disabled', 'disabled');
                                            $("#ddlContractorD").attr('disabled', 'disabled');

                                            // set the contractor name

                                            if ($("#ddlHeadContractorD").find('option').length >= 1) {
                                                $("#HeadConSupNameD").text($("#HeadConSupNameC").text());
                                                $("#trHeadConSupNameD").show('slow');

                                            }
                                            else if ($("#ddlContractorD").find('option').length >= 1) {
                                                $("#ConSupNameD").text($("#ConSupNameC").text());
                                                $("#trConSupNameD").show('slow');
                                            }

                                        }
                                        if (vpAggRepeat == "Y") {

                                            if ($("#ddlHeadAgreementD").find('option').length >= 1) {

                                                $('#ddlHeadAgreementD').html($('#ddlHeadAgreementC').html());
                                                $("#ddlHeadRoadD").html($('#ddlHeadRoadC').html());

                                            }
                                            else if ($("#ddlAgreementD").find('option').length >= 1) {

                                                $('#ddlAgreementD').html($('#ddlAgreementC').html());
                                                $("#ddlRoadD").html($('#ddlRoadC').html());
                                            }

                                            $("#ddlAgreementD").attr('disabled', 'disabled');
                                            $("#ddlHeadAgreementD").attr('disabled', 'disabled');
                                        }

                                    });
                                });
                            }
                        },
                        error: function (xhr, ajaxOptions, thrownError) {

                        }
                    });
                }
                else if (isMulTxn == "Y") {
                    var vpCHead = 0;
                    var vpDHead = 0;
                    var vpDistRepeat = "N";
                    var vpDPIURepeat = "N";
                    var vpContRepeat = "N";
                    var vpSupRepeat = "N";
                    var vpAggRepeat = "N";
                    var vpRoadRepeat = "N";
                    var vpHeadRepeat = "N";

                    $.ajax({
                        url: "/TEO/GetTEOValidationParams/" + billId,
                        type: "POST",
                        async: false,
                        cache: false,
                        success: function (data) {
                            vpCHead = data.CHead;
                            vpDHead = data.DHead;
                            vpDistRepeat = data.District;
                            vpDPIURepeat = data.DPIU;
                            vpContRepeat = data.Contractor;
                            vpSupRepeat = data.Supplier;
                            vpAggRepeat = data.Agreement;
                            vpRoadRepeat = data.Road;
                            vpHeadRepeat = data.Head;


                            $("#loadTEODebitDetails").load("/TEO/TEODetails/" + billId + "/D", function () {
                                $.each($("select"), function () {
                                    if ($(this).find('option').length >= 1) {
                                        $('#tr' + $(this).attr('id')).show();
                                    }
                                });

                                var contID = 0;

                                if ($("#ddlHeadContractorD").find('option').length >= 1) {
                                    contID = $("#ddlHeadContractorD").val();
                                }
                                else if ($("#ddlContractorD").find('option').length >= 1) {
                                    contID = $("#ddlContractorD").val();
                                }

                                // alert('In MutTXN=Yes and debitContractor=' + contID);

                                $.ajax({
                                    url: "/Receipt/GetContractorName/" + contID,
                                    type: "POST",
                                    async: false,
                                    cache: false,
                                    success: function (data) {
                                        if ($("#ddlHeadContractorD").find('option').length >= 1) {
                                            $("#HeadConSupNameD").text(data);
                                            $("#trHeadConSupNameD").show('slow');
                                        }
                                        else if ($("#ddlContractorD").find('option').length >= 1) {
                                            $("#ConSupNameD").text(data);
                                            $("#trConSupNameD").show('slow');
                                        }

                                    },
                                    error: function (xhr, ajaxOptions, thrownError) {
                                        alert(xhr.responseText);
                                    }
                                });

                                if (vpDistRepeat == "Y") {
                                    $("#ddlDistrictD").attr('disabled', 'disabled');
                                }
                                if (vpDPIURepeat == "Y") {
                                    $("#ddlDPIUD").attr('disabled', 'disabled');
                                }
                                if (vpContRepeat == "Y" || vpSupRepeat == "Y") {
                                    $("#ddlContractorD").attr('disabled', 'disabled');
                                    $("#ddlHeadContractorD").attr('disabled', 'disabled');
                                }
                                if (vpAggRepeat == "Y") {
                                    $("#ddlAgreementD").attr('disabled', 'disabled');
                                    $("#ddlHeadAgreementD").attr('disabled', 'disabled');
                                }
                                if (vpRoadRepeat == "Y") {
                                    $("#ddlRoadD").attr('disabled', 'disabled');
                                    $("#ddlHeadRoadD").attr('disabled', 'disabled');
                                }
                                if (vpHeadRepeat == "Y") {
                                    $("#ddlHeadD").attr('disabled', 'disabled');
                                }




                                $("#loadTEOCreditDetails").load("/TEO/TEODetails/" + billId + "/C", function () {
                                    $.each($("select"), function () {
                                        if ($(this).find('option').length >= 1) {
                                            $('#tr' + $(this).attr('id')).show();
                                        }
                                    });


                                    //    alert('In MutTXN=Yes and creditContractor=' + $("#ddlContractorC").val());

                                    /*commented by Koustubh Nakate on 30/09/2013 */
                                    ///  var contID = 0;

                                    //if ($("#ddlHeadContractorC").find('option').length >= 1) {
                                    //    contID = $("#ddlHeadContractorC").val();
                                    //}
                                    //else if ($("#ddlContractorC").find('option').length >= 1) {
                                    //    //  contID = $("#ddlContractorC").val();

                                    //    //added by Koustubh Nakate on 30/09/2013 
                                    //    contID = $("#ddlContractorD").val();

                                    //}

                                    //set credit side contractor and agreement also selected
                                    if ($("#ddlHeadContractorC").is(":visible")) {
                                        $("#ddlHeadContractorC").val(contID);
                                    }
                                    else {
                                        $("#ddlContractorC").val(contID);
                                    }



                                    //alert(contID);

                                    $.ajax({
                                        url: "/Receipt/GetContractorName/" + contID,
                                        type: "POST",
                                        async: false,
                                        cache: false,
                                        success: function (data) {
                                            if ($("#ddlHeadContractorC").find('option').length >= 1) {
                                                $("#HeadConSupNameC").text(data);
                                                $("#trHeadConSupNameC").show('slow');
                                            }
                                            else if ($("#ddlContractorC").find('option').length >= 1) {
                                                $("#ConSupNameC").text(data);
                                                $("#trConSupNameC").show('slow');
                                            }
                                        },
                                        error: function (xhr, ajaxOptions, thrownError) {
                                            alert(xhr.responseText);
                                        }
                                    });
                                    if (vpDistRepeat == "Y") {
                                        $("#ddlDistrictC").attr('disabled', 'disabled');
                                    }
                                    if (vpDPIURepeat == "Y") {
                                        $("#ddlDPIUC").attr('disabled', 'disabled');
                                    }
                                    if (vpContRepeat == "Y" || vpSupRepeat == "Y") {
                                        $("#ddlContractorC").attr('disabled', 'disabled');
                                        $("#ddlHeadContractorC").attr('disabled', 'disabled');


                                        /*added by Koustubh Nakate on 30/09/2013 */
                                        // set the contractor name

                                        if ($("#ddlHeadContractorC").find('option').length >= 1) {
                                            $("#HeadConSupNameC").text($("#HeadConSupNameD").text());
                                            $("#trHeadConSupNameC").show('slow');
                                            $("#ddlHeadContractorC").val(contID);

                                        }
                                        else if ($("#ddlContractorC").find('option').length >= 1) {
                                            $("#ConSupNameC").text($("#ConSupNameD").text());

                                            $("#trConSupNameC").show('slow');

                                            $("#ddlContractorC").val(contID);
                                        }

                                    }
                                    if (vpAggRepeat == "Y") {

                                        /*added by Koustubh Nakate on 30/09/2013 */
                                        if ($("#ddlHeadAgreementC").find('option').length >= 1) {
                                            $('#ddlHeadAgreementC').html($('#ddlHeadAgreementD').html());
                                            $("#ddlHeadRoadC").html($('#ddlHeadRoadD').html());
                                        }
                                        else if ($("#ddlAgreementC").find('option').length >= 1) {
                                            $('#ddlAgreementC').html($('#ddlAgreementD').html());
                                            $("#ddlRoadC").html($('#ddlRoadD').html());
                                        }

                                        $("#ddlAgreementC").attr('disabled', 'disabled');
                                        $("#ddlHeadAgreementC").attr('disabled', 'disabled');


                                    }
                                    if (vpRoadRepeat == "Y") {
                                        $("#ddlRoadC").attr('disabled', 'disabled');
                                        $("#ddlHeadRoadC").attr('disabled', 'disabled');
                                    }
                                    if (vpHeadRepeat == "Y") {
                                        $("#ddlHeadC").attr('disabled', 'disabled');
                                    }
                                });

                            });





                        },
                        error: function (xhr, ajaxOptions, thrownError) {

                        }
                    });
                }
                else {


                    if ($('#tblTEODetailsGrid').jqGrid('getGridParam', 'reccount') == 1) {


                        var rowdata = $("#tblTEODetailsGrid").getRowData($("#tblTEODetailsGrid").getDataIDs()[0]);
                        var vpCHead = 0;
                        var vpDHead = 0;
                        var vpDistRepeat = "N";
                        var vpDPIURepeat = "N";
                        var vpContRepeat = "N";
                        var vpSupRepeat = "N";
                        var vpAggRepeat = "N";
                        var vpRoadRepeat = "N";
                        var vpHeadRepeat = "N";

                        $.ajax({
                            url: "/TEO/GetTEOValidationParams/" + billId,
                            type: "POST",
                            async: false,
                            cache: false,
                            success: function (data) {
                                vpCHead = data.CHead;
                                vpDHead = data.DHead;
                                vpDistRepeat = data.District;
                                vpDPIURepeat = data.DPIU;
                                vpContRepeat = data.Contractor;
                                vpSupRepeat = data.Supplier;
                                vpAggRepeat = data.Agreement;
                                vpRoadRepeat = data.Road;
                                vpHeadRepeat = data.Head;

                                if (rowdata["CreditDebit"] == "Credit") {

                                    $("#loadTEODebitDetails").load("/TEO/TEODetails/" + billId + "/D", function () {
                                        $.each($("select"), function () {
                                            if ($(this).find('option').length >= 1) {
                                                $('#tr' + $(this).attr('id')).show();
                                            }
                                        });

                                        var contID = 0;

                                        if ($("#ddlHeadContractorD").find('option').length >= 1) {
                                            contID = $("#ddlHeadContractorD").val();
                                        }
                                        else if ($("#ddlContractorD").find('option').length >= 1) {
                                            contID = $("#ddlContractorD").val();
                                        }

                                        $.ajax({
                                            url: "/Receipt/GetContractorName/" + contID,
                                            type: "POST",
                                            async: false,
                                            cache: false,
                                            success: function (data) {
                                                if ($("#ddlHeadContractorD").find('option').length >= 1) {
                                                    $("#HeadConSupNameD").text(data);
                                                    $("#trHeadConSupNameD").show('slow');
                                                }
                                                else if ($("#ddlContractorD").find('option').length >= 1) {
                                                    $("#ConSupNameD").text(data);
                                                    $("#trConSupNameD").show('slow');
                                                }

                                            },
                                            error: function (xhr, ajaxOptions, thrownError) {
                                                alert(xhr.responseText);
                                            }
                                        });

                                        if (vpDistRepeat == "Y") {
                                            $("#ddlDistrictD").attr('disabled', 'disabled');
                                        }
                                        if (vpDPIURepeat == "Y") {
                                            $("#ddlDPIUD").attr('disabled', 'disabled');
                                        }
                                        if (vpContRepeat == "Y" || vpSupRepeat == "Y") {
                                            $("#ddlContractorD").attr('disabled', 'disabled');
                                            $("#ddlHeadContractorD").attr('disabled', 'disabled');
                                        }
                                        else {
                                            if ($("#trddlContractorC").is(':visible')) {
                                                $("#trddlContractorC").hide();
                                            }
                                        }
                                        if (vpAggRepeat == "Y") {
                                            $("#ddlAgreementD").attr('disabled', 'disabled');
                                            $("#ddlHeadAgreementD").attr('disabled', 'disabled');
                                        }
                                        if (vpRoadRepeat == "Y") {
                                            $("#ddlRoadD").attr('disabled', 'disabled');
                                            $("#ddlHeadRoadD").attr('disabled', 'disabled');
                                        }
                                        if (vpHeadRepeat == "Y") {
                                            $("#ddlHeadD").attr('disabled', 'disabled');
                                        }
                                    });
                                }
                                else {
                                    $("#loadTEOCreditDetails").load("/TEO/TEODetails/" + billId + "/C", function () {
                                        $.each($("select"), function () {
                                            if ($(this).find('option').length >= 1) {
                                                $('#tr' + $(this).attr('id')).show();
                                            }
                                        });

                                        var contID = 0;

                                        if ($("#ddlHeadContractorC").find('option').length >= 1) {
                                            contID = $("#ddlHeadContractorC").val();
                                        }
                                        else if ($("#ddlContractorC").find('option').length >= 1) {
                                            contID = $("#ddlContractorC").val();
                                        }

                                        $.ajax({
                                            url: "/Receipt/GetContractorName/" + contID,
                                            type: "POST",
                                            async: false,
                                            cache: false,
                                            success: function (data) {
                                                if ($("#ddlHeadContractorC").find('option').length >= 1) {
                                                    $("#HeadConSupNameC").text(data);
                                                    $("#trHeadConSupNameC").show('slow');
                                                }
                                                else if ($("#ddlContractorC").find('option').length >= 1) {
                                                    $("#ConSupNameC").text(data);
                                                    $("#trConSupNameC").show('slow');
                                                }
                                            },
                                            error: function (xhr, ajaxOptions, thrownError) {
                                                alert(xhr.responseText);
                                            }
                                        });
                                        if (vpDistRepeat == "Y") {
                                            $("#ddlDistrictC").attr('disabled', 'disabled');
                                        }
                                        if (vpDPIURepeat == "Y") {
                                            $("#ddlDPIUC").attr('disabled', 'disabled');
                                        }
                                        if (vpContRepeat == "Y" || vpSupRepeat == "Y") {
                                            $("#ddlContractorC").attr('disabled', 'disabled');
                                            $("#ddlHeadContractorC").attr('disabled', 'disabled');
                                        }
                                        else {
                                            if ($("#trddlContractorD").is(':visible')) {
                                                $("#trddlContractorD").hide();
                                            }
                                        }
                                        if (vpAggRepeat == "Y") {
                                            $("#ddlAgreementC").attr('disabled', 'disabled');
                                            $("#ddlHeadAgreementC").attr('disabled', 'disabled');
                                        }
                                        if (vpRoadRepeat == "Y") {
                                            $("#ddlRoadC").attr('disabled', 'disabled');
                                            $("#ddlHeadRoadC").attr('disabled', 'disabled');
                                        }
                                        if (vpHeadRepeat == "Y") {
                                            $("#ddlHeadC").attr('disabled', 'disabled');
                                        }
                                    });
                                }
                            },
                            error: function (xhr, ajaxOptions, thrownError) {

                            }
                        });
                    }
                    else if ($('#tblTEODetailsGrid').jqGrid('getGridParam', 'reccount') == 2) {
                        $("#loadTEOCreditDetails").html("");
                        $("#loadTEODebitDetails").html("");
                    }
                    else {
                        $("#loadTEOCreditDetails").load("/TEO/TEODetails/" + billId + "/C", function () {
                            $("#loadTEODebitDetails").load("/TEO/TEODetails/" + billId + "/D", function () {
                                $.each($("select"), function () {
                                    if ($(this).find('option').length >= 1) {
                                        $('#tr' + $(this).attr('id')).show();
                                    }
                                });
                            });
                            $.each($("select"), function () {
                                if ($(this).find('option').length >= 1) {
                                    $('#tr' + $(this).attr('id')).show();
                                }
                            });
                        });
                    }
                }
            }

            //Added By Abhishek Kamble 11-nov-2013
            $('#tblTEODetailsGrid_rn').html('Sr.<br/>No.');

            $("#divTEODetailsPager_left").html("<table><tr><td style='width:20%'><span style='float:left;' class='ui-icon ui-icon-info'></span>Rs.  <font color='#4eb305'>  " + userdata.creditRemainingAmt + " </font>Credit Amount Remaining </td><td style='width:20%'><span style='float:left;display:inline' class='ui-icon ui-icon-info'></span>Rs. <font color='#4eb305'>" + userdata.debitRemainingAmt + "</font> Debit Amount Remaining</td></tr></table>");

        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
            }
        }

    }); //end of documents grid
}


function FillInCascadeDropdownWithSelection(map, dropdown, action) {

    $(dropdown).empty()

    $.post(action, map, function (data) {
        ddvalues = data;
        $.each(data, function () {
            $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
        });

    }, "json");
}

function EditTEOMaster(urlParam) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: "/TEO/TEOMaster/" + urlParam,
        type: "GET",
        async: false,
        cache: false,
        data:
            {
                "Month": month,
                "Year": year
            },
        success: function (data) {
            $.unblockUI();

            $("#loadTEOMaster").html(data);
            // new change done by Vikram on 28-09-2013
            if ($("#ddlSubTrans").val() > 0) {
                $("#trddlSubTrans").show();
                $("#ddlSubTrans").trigger('change');
            }
            //end of change
            $("#BILL_NO").attr("readonly", "readonly");
            if (isDetailsGridLoaded && jQuery('#tblTEODetailsGrid').jqGrid('getGridParam', 'reccount') > 0) {
                $("#ddlTransMaster").attr("disabled", "disabled");
                $("#ddlSubTrans").attr("disabled", "disabled");
                $(".ui-datepicker-trigger").hide();
                $("#BILL_DATE").attr("readonly", "readonly");
            }
            $("#loadTEOMaster").show('slow');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();

            alert(xhr.responseText);
        }
    });
}

function DeleteTEOMaster(urlParam) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    var varMsg = null;
    var IsDetails = $('#tblTEODetailsGrid').jqGrid('getGridParam', 'reccount');
    if (IsDetails != "undefined" && IsDetails > 0) {
        varMsg = "Are you sure you want to delete TEO Master and its Details?";
    }
    else {
        varMsg = "Are you sure you want to delete TEO Master?";
    }
    if (confirm(varMsg)) {
        $.ajax({
            url: "/TEO/DeleteTEOMaster/" + urlParam,
            type: "POST",
            async: false,
            cache: false,
            success: function (data) {
                $.unblockUI();

                if (data.success) {
                    $("#divTEOMasterError").hide("slide");
                    $("#divTEOMasterError span:eq(1)").html('');
                    alert("TEO data Deleted");
                    if (isImprestEntry == "False") {
                        strUrl = "/TEO/TEOList/"
                    }
                    else {
                        strUrl = "/TEO/TEOImprest/"
                    }
                    $("#mainDiv").load(strUrl);
                    return false;
                }
                else {
                    $("#divTEOMasterError").show("slide");
                    $("#divTEOMasterError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                    return false;
                }
            }
        });
        $.unblockUI();

    }
    else {
        $.unblockUI();

        return false;
    }
}

function EditTEODetails(urlParam, creditDebit) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    var vpCHead = 0;
    var vpDHead = 0;
    var vpDistRepeat = "N";
    var vpDPIURepeat = "N";
    var vpContRepeat = "N";
    var vpSupRepeat = "N";
    var vpAggRepeat = "N";
    var vpRoadRepeat = "N";
    var vpHeadRepeat = "N";

    $.ajax({
        url: "/TEO/GetTEOValidationParams/" + billId,
        type: "POST",
        async: false,
        cache: false,
        success: function (data) {
            $.unblockUI();

            vpCHead = data.CHead;
            vpDHead = data.DHead;
            vpDistRepeat = data.District;
            vpDPIURepeat = data.DPIU;
            vpContRepeat = data.Contractor;
            vpSupRepeat = data.Supplier;
            vpAggRepeat = data.Agreement;
            vpRoadRepeat = data.Road;
            vpHeadRepeat = data.Head;

            if (creditDebit == "D") {
                $("#loadTEODebitDetails").show();
                $("#loadTEODebitDetails").load("/TEO/TEODetails/" + urlParam + "/" + creditDebit, function () {
                    $.each($("select"), function () {
                        if ($(this).find('option').length > 1) {
                            if ($(this).attr('id') === undefined) {
                            }
                            else {
                                if ($(this).attr('id').toString().toLowerCase() == "ddlroadd") {
                                    // $("#trHeadIsFinalPayD").show();
                                }
                                if ($(this).attr('id').toString().toLowerCase() == "ddlheadroadd") {
                                    // $("#trHeadIsFinalPayD").show();
                                }
                                if ($(this).attr('id').toString().toLowerCase() == "ddlcontractord") {
                                    $("#ddlContractorD").attr('disabled', 'disabled');
                                }
                                if ($(this).attr('id').toString().toLowerCase() == "ddlheadcontractord") {
                                    $("#ddlHeadContractorD").attr('disabled', 'disabled');
                                }
                                if ($(this).attr('id').toString().toLowerCase() == "ddlagreementd") {
                                    $("#ddlAgreementD").attr('disabled', 'disabled');
                                }
                                if ($(this).attr('id').toString().toLowerCase() == "ddlheadagreementd") {
                                    $("#ddlHeadAgreementD").attr('disabled', 'disabled');
                                }
                                $('#tr' + $(this).attr('id')).show();
                            }
                        }
                    });

                    var contID = 0;

                    if ($("#ddlHeadContractorD").find('option').length >= 1) {
                        contID = $("#ddlHeadContractorD").val();
                        $.ajax({
                            url: "/Receipt/GetContractorName/" + contID,
                            type: "POST",
                            async: false,
                            cache: false,
                            success: function (data) {
                                $("#HeadConSupNameD").text(data);
                                $("#trHeadConSupNameD").show('slow');
                                //$("#trHeadIsFinalPayD").show(); //by amol
                            },
                            error: function (xhr, ajaxOptions, thrownError) {
                                alert(xhr.responseText);
                            }
                        });
                    }
                    else if ($("#ddlContractorD").find('option').length >= 1) {
                        contID = $("#ddlContractorD").val();
                        $.ajax({
                            url: "/Receipt/GetContractorName/" + contID,
                            type: "POST",
                            async: false,
                            cache: false,
                            success: function (data) {
                                $("#ConSupNameD").text(data);
                                $("#trConSupNameD").show('slow');
                                //$("#trHeadIsFinalPayD").show();//by amol
                            },
                            error: function (xhr, ajaxOptions, thrownError) {
                                alert(xhr.responseText);
                            }
                        });
                    }



                    if (vpDistRepeat == "Y") {
                        $("#ddlDistrictD").attr('disabled', 'disabled');
                    }
                    if (vpDPIURepeat == "Y") {
                        $("#ddlDPIUD").attr('disabled', 'disabled');
                    }
                    if (vpContRepeat == "Y" || vpSupRepeat == "Y") {
                        $("#ddlContractorD").attr('disabled', 'disabled');
                        $("#ddlHeadContractorD").attr('disabled', 'disabled');
                    }
                    if (vpAggRepeat == "Y") {
                        $("#ddlAgreementD").attr('disabled', 'disabled');
                        $("#ddlHeadAgreementD").attr('disabled', 'disabled');
                    }
                    if (vpRoadRepeat == "Y") {
                        $("#ddlRoadD").attr('disabled', 'disabled');
                        $("#ddlHeadRoadD").attr('disabled', 'disabled');
                    }
                    if (vpHeadRepeat == "Y") {
                        $("#ddlHeadD").attr('disabled', 'disabled');
                    }


                    if ($("#ddlHeadRoadD").is(":visible") || $("#ddlRoadD").is(":visible")) {
                        var roadCode = $("#ddlHeadRoadD").is(":visible") ? $("#ddlHeadRoadD").val() : $("#ddlRoadD").val()

                        if (($("#ddlRoadD").val != "" && $("#ddlRoadD").val != null) || ($("#ddlHeadRoadD").val != "" && $("#ddlHeadRoadD").val != null))
                            $.ajax({
                                // url: "/TEO/IsFinalPayment/" + $("#ddlContractorD").val() + "$" + $("#ddlAgreementD").val() + "$" + roadCode,
                                url: "/TEO/IsFinalPayment/" + 0 + "$" + 0 + "$" + roadCode,
                                type: "POST",
                                async: false,
                                cache: false,
                                success: function (data) {
                                    if (data == "1") {
                                        $("#trHeadIsFinalPayD").show('slow');
                                        $("#HeadIsFinalPayD").attr('checked', 'checked');
                                        $("#HeadIsFinalPayD").attr('disabled', 'disabled');
                                    }
                                    else {
                                        $("#trHeadIsFinalPayD").show('slow');
                                        $("#HeadIsFinalPayD").attr('checked', false);
                                        //  $("#HeadIsFinalPayD").attr('disabled', false);
                                        $("#HeadIsFinalPayD").attr('disabled', true);
                                    }
                                },
                                error: function (xhr, ajaxOptions, thrownError) {
                                    alert(xhr.responseText);
                                }
                            });
                    }

                });
            }
            else {
                $("#loadTEOCreditDetails").show();
                $("#loadTEOCreditDetails").load("/TEO/TEODetails/" + urlParam + "/" + creditDebit, function () {
                    $.each($("select"), function () {
                        if ($(this).find('option').length > 1) {
                            if ($(this).attr('id') === undefined) {
                            }
                            else {
                                if ($(this).attr('id').toString().toLowerCase() == "ddlroadc") {
                                    // $("#trIsFinalPayC").show();
                                }
                                if ($(this).attr('id').toString().toLowerCase() == "ddlheadroadc") {
                                    // $("#trHeadIsFinalPayC").show();
                                }
                                if ($(this).attr('id').toString().toLowerCase() == "ddlcontractorc") {
                                    $("#ddlContractorC").attr('disabled', 'disabled');
                                }
                                if ($(this).attr('id').toString().toLowerCase() == "ddlheadcontractorc") {
                                    $("#ddlHeadContractorC").attr('disabled', 'disabled');
                                }
                                if ($(this).attr('id').toString().toLowerCase() == "ddlagreementc") {
                                    $("#ddlAgreementC").attr('disabled', 'disabled');
                                }
                                if ($(this).attr('id').toString().toLowerCase() == "ddlheadagreementc") {
                                    $("#ddlHeadAgreementC").attr('disabled', 'disabled');
                                }
                                $('#tr' + $(this).attr('id')).show();
                            }
                        }
                    });


                    var contID = 0;

                    if ($("#ddlHeadContractorC").find('option').length >= 1) {
                        contID = $("#ddlHeadContractorC").val();
                        $.ajax({
                            url: "/Receipt/GetContractorName/" + contID,
                            type: "POST",
                            async: false,
                            cache: false,
                            success: function (data) {
                                $("#HeadConSupNameC").text(data);
                                $("#trHeadConSupNameC").show('slow');
                                $("#trIsFinalPayC").show();//by amol 
                            },
                            error: function (xhr, ajaxOptions, thrownError) {
                                alert(xhr.responseText);
                            }
                        });
                    }
                    else if ($("#ddlContractorC").find('option').length >= 1) {
                        contID = $("#ddlContractorC").val();
                        $.ajax({
                            url: "/Receipt/GetContractorName/" + contID,
                            type: "POST",
                            async: false,
                            cache: false,
                            success: function (data) {
                                $("#ConSupNameC").text(data);
                                $("#trConSupNameC").show('slow');
                                $("#trIsFinalPayC").show();//by amol 
                            },
                            error: function (xhr, ajaxOptions, thrownError) {
                                alert(xhr.responseText);
                            }
                        });
                    }


                    if (vpDistRepeat == "Y") {
                        $("#ddlDistrictC").attr('disabled', 'disabled');
                    }
                    if (vpDPIURepeat == "Y") {
                        $("#ddlDPIUC").attr('disabled', 'disabled');
                    }
                    if (vpContRepeat == "Y" || vpSupRepeat == "Y") {
                        $("#ddlContractorC").attr('disabled', 'disabled');
                        $("#ddlHeadContractorC").attr('disabled', 'disabled');
                    }
                    if (vpAggRepeat == "Y") {
                        $("#ddlAgreementC").attr('disabled', 'disabled');
                        $("#ddlHeadAgreementC").attr('disabled', 'disabled');
                    }
                    if (vpRoadRepeat == "Y") {
                        $("#ddlRoadC").attr('disabled', 'disabled');
                        $("#ddlHeadRoadC").attr('disabled', 'disabled');
                    }
                    if (vpHeadRepeat == "Y") {
                        $("#ddlHeadC").attr('disabled', 'disabled');
                    }

                    if ($("#ddlHeadRoadC").is(":visible") || $("#ddlRoadC").is(":visible")) {
                        var roadCode = $("#ddlHeadRoadC").is(":visible") ? $("#ddlHeadRoadC").val() : $("#ddlRoadC").val();

                        if (($("#ddlRoadC").val != "" && $("#ddlRoadC").val != null) || ($("#ddlHeadRoadC").val != "" && $("#ddlHeadRoadC").val != null)) {
                            $.ajax({
                                // url: "/TEO/IsFinalPayment/" + $("#ddlContractorC").val() + "$" + $("#ddlAgreementC").val() + "$" + $("#ddlRoadC").val(),// contractor and agreement is not required
                                url: "/TEO/IsFinalPayment/" + 0 + "$" + 0 + "$" + roadCode,
                                type: "POST",
                                async: false,
                                cache: false,
                                success: function (data) {
                                    if (data == "1") {
                                        $("#trHeadIsFinalPayC").show('slow');
                                        $("#HeadIsFinalPayC").attr('checked', 'checked');
                                        $("#HeadIsFinalPayC").attr('disabled', 'disabled');
                                    }
                                    else {

                                        $("#trHeadIsFinalPayC").show('slow');
                                        $("#HeadIsFinalPayC").attr('checked', false);
                                        // $("#HeadIsFinalPayC").attr('disabled', false);
                                        $("#HeadIsFinalPayC").attr('disabled', true);
                                    }
                                },
                                error: function (xhr, ajaxOptions, thrownError) {
                                    alert(xhr.responseText);
                                }
                            });
                        }
                    }
                });
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();

        }
    });
}

function DeleteTEODetails(urlParam, creditDebit) {

    if (confirm("Are you sure you want to delete TEO details?")) {

        $.ajax({
            url: "/TEO/DeleteTEODetails/" + urlParam,
            type: "POST",
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert("TEO Details Deleted");
                    $.ajax({
                        url: "/TEO/TEODetails/" + $("#tblTEOMasterGrid").getDataIDs()[0] + "/" + creditDebit,
                        type: "POST",
                        async: false,
                        cache: false,
                        success: function (data) {
                            $("#loadTEOMaster").hide();
                            $("#loadTEOMaster").html('');
                            if (creditDebit == "C") {
                                $("#loadTEOCreditDetails").show();
                                $("#loadTEOCreditDetails").html(data);
                            }
                            else {
                                $("#loadTEODebitDetails").show();
                                $("#loadTEODebitDetails").html(data);
                            }
                            LoadTEODetailsGrid(billId);
                            $.each($("select"), function () {
                                if ($(this).find('option').length >= 1) {
                                    $('#tr' + $(this).attr('id')).show();
                                }
                            });
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            alert(xhr.responseText);
                            unblockPage();
                        }
                    });

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


