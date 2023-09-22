$(document).ready(function () {

    $(function () {
        $("#accordion").accordion({
            //fillSpace: true,
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $('#btnDropListProposal').click(function () {
        if (parseInt($('#ddlState option:selected').val()) <= 0) {
            alert('Please select a valid State');
            return false;
        }
        LoadDropProposalsListMRD();
    });

});

function LoadDropProposalsListMRD() {
    $("#tblProposalsForDroppingMRD").jqGrid('GridUnload');
    $('#tblProposalsForDroppingMRD').jqGrid({
        url: '/Proposal/ListDropppingWorks',
        datatype: "json",
        mtype: "POST",
        colNames: ['State', 'Request Letter No.', /*'Drop Letter No.',*/ 'Request Date', 'No. of Works for Dropping', 'No. of Works Approved for Dropping', /*'Approved',*/ 'Generate Request', 'View Request Letter', 'View Details', 'View Order'],
        colModel: [
                      { name: 'State', index: 'State', height: 'auto', width: 30, align: "center", sortable: false, hidden: false },
                      { name: 'RequestLetterNo', index: 'RequestLetterNo', height: 'auto', width: 30, align: "center", sortable: false, hidden: false },
                      //{ name: 'DropLetterNo', index: 'DropLetterNo', height: 'auto', width: 30, align: "center", sortable: false },
                      { name: 'RequestDate', index: 'RequestDate', height: 'auto', width: 30, align: "center", sortable: false },
                      { name: 'DroppingWorks', index: 'DroppingWorks', height: 'auto', width: 30, align: "center", sortable: false },
                      { name: 'ApprovedWorks', index: 'ApprovedWorks', height: 'auto', width: 30, align: "center", sortable: false },
                      //{ name: 'Approved', index: 'Approved', height: 'auto', width: 30, align: "center", sortable: false },
                      { name: 'GenerateRequest', index: 'GenerateRequest', height: 'auto', width: 30, align: "center", sortable: false },
                      { name: 'View', index: 'View', height: 'auto', width: 30, align: "center", sortable: false },
                      { name: 'ViewDetails', index: 'ViewDetails', height: 'auto', width: 30, align: "center", sortable: false },
                      { name: 'ViewOrder', index: 'ViewOrder', height: 'auto', width: 30, align: "center", sortable: false },
        ],
        postData: { stateCode: $('#ddlState option:selected').val(), },
        pager: jQuery('#dvPagerProposalsForDroppingMRD'),
        rowNum: 40,
        rowList: [15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'State',
        sortorder: "asc",
        caption: "Drop Order Request",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        loadComplete: function () {
            //if (parseInt($("#tblLetterGen").getGridParam("reccount")) > 0) {
            //    $('#dvPagerLetterGen').html("<input type='button' style='margin-left:27px' id='btnGenLetter' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'GenerateDropLetter();return false;' value='Generate Drop Letter'/>")
            //}
        },
    });
}


var Rejected = 0;

function ShowDroppedProposelDetails(dropCode) {
    //$('#dvDropProposalList').show();
    //  alert("Dropped Order List")
    $('#dvDetailDroppedOrderList').hide();


    $("#tblstDropProposal").jqGrid('GridUnload');

    jQuery("#tblstDropProposal").jqGrid({
        url: '/Proposal/GetDroppedProposalListByBatch?reqCode=' + dropCode,
        datatype: "json",
        mtype: "POST",
        postData: { StateCode: $("#ddlStates option:selected").val(), YearCode: $("#ddlYears option:selected").val(), StreamCode: $("#ddlStreams option:selected").val(), BatchCode: $("#ddlBatch option:selected").val(), Scheme: $("#ddlSchemes option:selected").val(), Type: $("#ddlProposalType option:selected").val(), __RequestVerificationToken: $('#frmDropFilterProposal input[name=__RequestVerificationToken]').val() },
        colNames: ['State', 'District', 'Block', 'Year', 'Batch', 'Package', 'Name of Road / Bridge', 'Road Length (in Kms) / Bridge Length (in Mtrs.)', 'Total Cost', 'Expenditure Incurred', 'Recoup Amount', 'Reason', 'Work Status', 'Select &nbsp;&nbsp;<input id="dropAll" type="checkbox" name="AllApprove" value="DropAll"/>', 'Request Code', 'Collaboration'],
        colModel: [
                            { name: 'State', index: 'State', height: 'auto', width: 150, align: "center", search: false, sortable: false },
                            { name: 'District', index: 'District', height: 'auto', width: 150, align: "center", search: false, sortable: false },
                            { name: 'Block', index: 'Block', height: 'auto', width: 150, align: "center", search: false, sortable: false },
                            { name: 'Year', index: 'Year', height: 'auto', width: 100, align: "center", search: false, sortable: false },
                            { name: 'Batch', index: 'Batch', height: 'auto', width: 100, align: "center", search: false, sortable: false },
                            { name: 'Package', index: 'Package', height: 'auto', width: 100, align: "center", search: false, sortable: false },
                            { name: 'RoadName', index: 'RoadName', height: 'auto', width: 180, align: "center", search: false, sortable: false },
                            { name: 'RoadLength', index: 'RoadLength', height: 'auto', width: 120, align: "center", search: false, sortable: false },

                            { name: 'TotalCost', index: 'TotalCost', height: 'auto', width: 100, align: "center", search: false, sortable: false },
                            { name: 'ExpenditureIncurred', index: 'ExpenditureIncurred', height: 'auto', width: 100, align: "center", search: false, sortable: false },
                            { name: 'RecoupAmount', index: 'RecoupAmount', height: 'auto', width: 100, align: "center", search: false, sortable: false },
                            { name: 'Reason', index: 'Reason', height: 'auto', width: 150, align: "center", search: false, sortable: false },

                            { name: 'WorkStatus', index: 'WorkStatus', height: 'auto', width: 100, align: "center", search: false, sortable: false },
                            { name: 'Approve', index: 'Approve', height: 'auto', width: 100, align: "center", search: false, sortable: false, formatter: countRejected },
                            { name: 'REQCODE', index: 'REQCODE', height: 'auto', width: 80, align: "center", search: false, sortable: false, hidden: true },
                            { name: 'COLLABORATION', index: 'COLLABORATION', height: 'auto', width: 80, align: "center", verticalalign: "middle", search: false, sortable: false, hidden: true }
        ],
        postData: { __RequestVerificationToken: $('#frmListProposalsForDroppingOrderMRD input[name=__RequestVerificationToken]').val() },
        pager: jQuery('#dvlstPagerDropProposal'),
        rowNum: 5000,
        rowList: [50, 100, 150, 200, 300, 400, 500, 600, 700, 800, 1000],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'ROAD_TYPE',
        sortorder: "desc",
        caption: "&nbsp;&nbsp;Work List For Dropping  ",//+ $("#ddlStates option:selected").text(),
        height: 'auro',
        hidegrid: true,
        rownumbers: true,
        // autowidth: true,
        shrinkToFit: false,
        onSelectRow: function (rowid, status, e) {
            // alert(rowid);
        },
        gridComplete: function () {

        },
        loadComplete: function (data) {
            $('#dropAll').parent().removeClass('ui-jqgrid-sortable');//to make header checkbox clickable

            $('#dropAll').change(function () {
                //alert($(this).prop('checked'));
                if ($(this).prop('checked')) {
                    $('.dropped').not(":disabled").prop('checked', true);
                }
                else {
                    $('.dropped').not(":disabled").prop('checked', false);
                }
            });

            var droppedBefore = []
            $.each($("input[name='Approve']:checked"), function (i, value) {
                droppedBefore[i] = $(this).val().trim();
            })

            var records = jQuery("#tblstDropProposal").jqGrid('getGridParam', 'records');

            if (records == droppedBefore.length + Rejected) {
                $('#dropAll').prop('checked', true);
                $('#dropAll').prop('disabled', true);
                //  $('.dropped').prop('disabled', true);
                //data.IsDOGenerated = true;
            }

            $("input[name = 'Approve']").change(function () {

                var dropped = []
                $.each($("input[name='Approve']:checked"), function (i, value) {
                    dropped[i] = $(this).val().trim();
                })

                if (records != dropped.length + Rejected) {
                    $('#dropAll').prop('checked', false);
                    $('#dropAll').prop('disabled', false);
                }
                else {
                    $('#dropAll').prop('checked', true);
                    $('#dropAll').prop('disabled', true);
                }
            });
            ///&& data.IsDOGenerated == falsehttp://localhost:49769/Web References/
            if (records != droppedBefore.length) {


                $("#dvlstPagerDropProposal_left").html("<input type='button' style='margin-left:27px' id='btnGenerateDetails' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'GenerateDropOrder();return false;' value='Approve.'/>")


                $("#dvlstPagerDropProposal_center").hide();

                if ($("#btnGenerateDetails1").length === 1) {
                    $("#pg_dvlstPagerDropProposal tr td:nth-child(1)").append("");
                    $("#btnGenerateDetails1").remove();
                }
                $("#pg_dvlstPagerDropProposal tr td:nth-child(1)").append("<input type='button' style='margin-left:40px' id='btnGenerateDetails1' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'RejectGenerateDropOrder();return false;' value='Reject'/>");
                $("#first_dvlstPagerDropProposal").find("td input").html("");


                // $("#dvlstPagerDropProposal_right").html("<input type='button' style='margin-left:40px' id='btnGenerateDetails1' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'RejectGenerateDropOrder();return false;' value='Reject'/>")

            }
            else {
                $("#dvlstPagerDropProposal_left").html('');
            }

        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }
    });
    Rejected = 0;
}


function countRejected(cellvalue, options, rowObject) {

    if (cellvalue == "Rejected") {

        Rejected++;
        //alert("Rejected" + Rejected)

    }

    return cellvalue;
}

function GetButtons() {

    $("#pg_dvlstPagerDropProposal tr td:nth-child(1)").append("<input type='button' style='margin-left:40px' id='btnGenerateDetails1' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'RejectGenerateDropOrder();return false;' value='Reject'/>");
    //pg_dvlstPagerDropProposal
    //$('#pg_dvlstPagerDropProposal')
    //.append($('<td>', { 'class': 'text-center', 'text': 'col1' }))
    //.append($('<td>', { 'class': 'text-center', 'text': 'col2' }))

}
function RejectGenerateDropOrder() {
    //$("#accordion div").html("");
    //$("#accordion h3").html(
    //        "<a href='#' style= 'font-size:.9em;' >Drop Order Details</a>" +
    //        '<a href="#" style="float: right;">' +
    //        '<img class="ui-icon ui-icon-closethick" onclick="CloseDetails();" /></a>'
    //        );

    var postData = $("#tblstDropProposal").jqGrid('getGridParam', 'postData');

    var records = $('#tblstDropProposal').jqGrid('getGridParam', 'records');

    var DropApproveArray = [];
    $.each($("input[name='Approve']:checked:not(:disabled)"), function (i, value) {
        DropApproveArray[i] = $(this).val().trim();
    })

    var DropApproveArrayAll = [];
    $.each($("input[name='Approve']:checked"), function (i, value) {
        DropApproveArrayAll[i] = $(this).val().trim();
    })


    if (DropApproveArrayAll.length > 0 && DropApproveArray.length > 0) {

        if (Rejected != 0) {
            if ((DropApproveArrayAll.length + Rejected) == records) {
                if (!confirm("Are you sure to continue?")) {
                    return;
                }
            }
        }                                                                      //changes Added 26-04-2022 

        else if (DropApproveArrayAll.length != records) {
            if (!confirm("All roads are not selected. Are you sure to continue?")) {
                return;
            }
        }
        $('#accordion').show('fold', function () {
            //blockPage();

            //$("#divAddDropOrder").load("/Proposal/DropOrderView?" + $.param({ StateCode: postData.StateCode, StreamCode: $('#tblstDropProposal').jqGrid('getCol', 'COLLABORATION')[0], YearCode: postData.YearCode, BatchCode: $('#tblstDropProposal').jqGrid('getCol', 'Batch')[0].substring(7, 8), SchemeCode: postData.Scheme, RequestCode: $('#tblstDropProposal').jqGrid('getCol', 'REQCODE')[0], ApproveRoads: DropApproveArray }), function () {
            //    $.validator.unobtrusive.parse($('#divAddDropOrder'));
            //    unblockPage();
            //});

            $.blockUI({ message: "<img src='/Content/images/ajax-loader.gif'>" });

            $.ajax({
                url: '/Proposal/RejectDropOrderView',
                method: 'GET',
                cache: false,
                async: true,
                traditional: true,
                data: { StateCode: $('#ddlState option:selected').val(), StreamCode: $('#tblstDropProposal').jqGrid('getCol', 'COLLABORATION')[0], YearCode: postData.YearCode, BatchCode: $('#tblstDropProposal').jqGrid('getCol', 'Batch')[0].substring(7, 8), SchemeCode: postData.Scheme, RequestCode: $('#tblstDropProposal').jqGrid('getCol', 'REQCODE')[0], ApproveRoads: DropApproveArray },
                dataType: 'html',
                success: function (data, status, xhr) {
                    $('#divAddDropOrder').html(data);
                    $.validator.unobtrusive.parse($('#divAddDropOrder'));
                    $.unblockUI();
                },
                error: function (xhr, status, err) {
                    alert(xhr.responseText);
                }
            });

            $('#divAddDropOrder').show('slow');
            $("#divAddDropOrder").css('height', 'auto');
            $("#tblstDropProposal").jqGrid('setGridState', 'hidden');
        });
    }
    else {
        alert('No Proposal is selected. Please select proposals.');
    }
}

function GenerateDropOrder() {
    //$("#accordion div").html("");
    //$("#accordion h3").html(
    //        "<a href='#' style= 'font-size:.9em;' >Drop Order Details</a>" +
    //        '<a href="#" style="float: right;">' +
    //        '<img class="ui-icon ui-icon-closethick" onclick="CloseDetails();" /></a>'
    //        );

    var postData = $("#tblstDropProposal").jqGrid('getGridParam', 'postData');

    var records = $('#tblstDropProposal').jqGrid('getGridParam', 'records');

    var DropApproveArray = [];
    $.each($("input[name='Approve']:checked:not(:disabled)"), function (i, value) {
        DropApproveArray[i] = $(this).val().trim();
    })

    var DropApproveArrayAll = [];
    $.each($("input[name='Approve']:checked"), function (i, value) {

        DropApproveArrayAll[i] = $(this).val().trim();
    })

    //var Rejected = [];
    //$.each($("td[title='Rejected')"), function (i, value) {

    //    Rejected[i] = $(this).val().trim();

    //})

    //alert("DropApproveArrayAll.length" + DropApproveArrayAll.length);
    //alert("DropApproveArray" + DropApproveArray.length);
    //alert("Rejected" + Rejected);

    if (DropApproveArrayAll.length > 0 && DropApproveArray.length > 0) {

        if (Rejected != 0) {
            if ((DropApproveArrayAll.length + Rejected) == records) {
                if (!confirm("Are you sure to continue?")) {
                    return;
                }
            }
        }
            //changes Added 26-04-2022 

        else if (DropApproveArrayAll.length != records) {
            if (!confirm("All roads are not selected. Are you sure to continue?")) {
                return;
            }
        }
        $('#accordion').show('fold', function () {
            //blockPage();

            //$("#divAddDropOrder").load("/Proposal/DropOrderView?" + $.param({ StateCode: postData.StateCode, StreamCode: $('#tblstDropProposal').jqGrid('getCol', 'COLLABORATION')[0], YearCode: postData.YearCode, BatchCode: $('#tblstDropProposal').jqGrid('getCol', 'Batch')[0].substring(7, 8), SchemeCode: postData.Scheme, RequestCode: $('#tblstDropProposal').jqGrid('getCol', 'REQCODE')[0], ApproveRoads: DropApproveArray }), function () {
            //    $.validator.unobtrusive.parse($('#divAddDropOrder'));
            //    unblockPage();
            //});

            $.blockUI({ message: "<img src='/Content/images/ajax-loader.gif'>" });

            $.ajax({
                url: '/Proposal/DropOrderView',
                method: 'GET',
                cache: false,
                async: true,
                traditional: true,
                data: { StateCode: $('#ddlState option:selected').val(), StreamCode: $('#tblstDropProposal').jqGrid('getCol', 'COLLABORATION')[0], YearCode: postData.YearCode, BatchCode: $('#tblstDropProposal').jqGrid('getCol', 'Batch')[0].substring(7, 8), SchemeCode: postData.Scheme, RequestCode: $('#tblstDropProposal').jqGrid('getCol', 'REQCODE')[0], ApproveRoads: DropApproveArray },
                dataType: 'html',
                success: function (data, status, xhr) {
                    $('#divAddDropOrder').html(data);
                    $.validator.unobtrusive.parse($('#divAddDropOrder'));
                    $.unblockUI();
                },
                error: function (xhr, status, err) {
                    alert(xhr.responseText);
                }
            });

            $('#divAddDropOrder').show('slow');
            $("#divAddDropOrder").css('height', 'auto');
            $("#tblstDropProposal").jqGrid('setGridState', 'hidden');
        });
    }
    else {
        alert('No Proposal is selected. Please select proposals.');
    }
}

function CloseDetails() {
    $("#tblstDropProposal").jqGrid('setGridState', 'visible');
    $('#accordion').hide('slow');
}
function ViewDropOrder() {
    var postData = $("#tblstDropProposal").jqGrid('getGridParam', 'postData');
    window.open('/Proposal/PreviewDropOrderReport?' + $.param({ StateCode: postData.StateCode, StreamCode: postData.StreamCode, YearCode: postData.YearCode, BatchCode: postData.BatchCode, SchemeCode: postData.Scheme, ProposalType: postData.Type }), '_blank');
}

function ShowDetailDroppedProposelDetails(RequestCode) {
    //alert(RequestCode);
    $('#gbox_tblstDropProposal').hide();
    $('#dvDetailDroppedOrderList').show();  //Show the current grid div first

    $("#tblstDetailDroppedOrder").jqGrid('GridUnload');
    jQuery("#tblstDetailDroppedOrder").jqGrid({
        url: '/Proposal/GetDetailDropOrderList',
        datatype: "json",
        mtype: "POST",
        //  postData: { StateCode: $("#ddlStates option:selected").val(), YearCode: $("#ddlYears option:selected").val(), StreamCode: $("#ddlStreams option:selected").val(), BatchCode: $("#ddlBatch option:selected").val(), Scheme: $("#ddlSchemes option:selected").val(), Type: $("#ddlProposalType option:selected").val(), Status: $('#ddlStatus option:selected').val() },
        postData: { RequestCode: RequestCode, Scheme: $("#ddlSchemes option:selected").val(), __RequestVerificationToken: $('#frmListProposalsForDroppingOrderMRD input[name=__RequestVerificationToken]').val() },
        colNames: ['No of Works', 'Order No', 'Order Date', 'Approve Date', 'View'],
        colModel: [
                            { name: 'NoofWork', index: 'NoofWork', height: 'auto', width: 80, align: "center", search: false, sortable: false },
                            { name: 'OrderNo', index: 'OrderNo', height: 'auto', width: 200, align: "center", search: false, sortable: false },
                            { name: 'OrderDate', index: 'OrderDate', height: 'auto', width: 180, align: "center", search: false, sortable: false },
                            { name: 'ApproveDate', index: 'ApproveDate', height: 'auto', width: 180, align: "center", search: false, sortable: false },
                            { name: 'View', index: 'View', height: 'auto', width: 80, align: "center", search: false, sortable: false },

        ],
        //postData: { __RequestVerificationToken: $('#frmListProposalsForDroppingOrderMRD input[name=__RequestVerificationToken]').val() },
        pager: jQuery('#dvlstDetailPagerDropperOrder'),
        rowNum: 5,
        rowList: [5, 10, 15],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'OrderNo',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Drop Order Request Details",
        height: 'auto',
        //width:'100%',
        hidegrid: true,
        rownumbers: true,
        // autowidth: true,
        shrinkToFit: false,
        cmTemplate: { title: false },
        loadComplete: function () { },
        loadError: function () { }
    })

}