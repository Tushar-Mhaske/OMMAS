var hiddenScheme2Col = false;
$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmDropFilterProposal'));

    $(function () {
        $("#accordion").accordion({
            //fillSpace: true,
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $("#btnDropListProposal").click(function () {
         
        $('#dvDropProposalList').hide(); //hide below grid 
        $('#dvDetailDroppedOrderList').hide(); //hide below grid 
        
        if ($("#ddlSchemes option:selected").val() == 1) {
            hiddenScheme2Col = true;
        }
        else {
            hiddenScheme2Col = false;
        }
        if ($("#frmDropFilterProposal").valid()) {
            
            LoadDropedOrderList();
            //LoadProposalForDropOrder();
        }
    });

    $('#ddlStates').change(function () {
        $("#ddlYears").empty();
        $.ajax({
            url: '/Proposal/PopulateFinancialYearsByStateForDropping',
            type: 'POST',
            beforeSend: function () {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            },
            data: AddAntiForgeryToken({ stateCode: $("#ddlStates").val() }),
            success: function (jsonData) {
                for (var i = 0; i < jsonData.length; i++) {
                    if (jsonData[i].Selected == true) {
                        $("#ddlYears").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                    else {
                        $("#ddlYears").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                }

                $.unblockUI();
            },
            error: function (err) {
                //alert("error " + err);
                $.unblockUI();
            }
        });
    });

});
function ShowDroppedProposelDetails(roadCode)
{
    $('#dvDropProposalList').show();

    $("#tblstDropProposal").jqGrid('GridUnload');

    jQuery("#tblstDropProposal").jqGrid({
        url: '/Proposal/GetDroppedProposalListByBatch?reqCode='+roadCode,
        datatype: "json",
        mtype: "POST",
        postData: { StateCode: $("#ddlStates option:selected").val(), YearCode: $("#ddlYears option:selected").val(), StreamCode: $("#ddlStreams option:selected").val(), BatchCode: $("#ddlBatch option:selected").val(), Scheme: $("#ddlSchemes option:selected").val(), Type: $("#ddlProposalType option:selected").val(),__RequestVerificationToken : $('#frmDropFilterProposal input[name=__RequestVerificationToken]').val() },
        colNames: ['State', 'District', 'Block', 'Year', 'Batch', 'Package', 'Name of Road / Bridge', 'Road Length (in Kms) / Bridge Length (in Mtrs.)', 'Work Status', 'Approve &nbsp;&nbsp;<input id="dropAll" type="checkbox" name="AllApprove" value="DropAll"/>', 'Request Code','Collaboration'],
        colModel: [
                            { name: 'State', index: 'State', height: 'auto', width: 200, align: "center", search: false ,sortable:false},
                            { name: 'District', index: 'District', height: 'auto', width: 200, align: "center", search: false, sortable: false },
                            { name: 'Block', index: 'Block', height: 'auto', width: 200, align: "center", search: false, sortable: false },
                            { name: 'Year', index: 'Year', height: 'auto', width: 200, align: "center", search: false, sortable: false },
                            { name: 'Batch', index: 'Batch', height: 'auto', width: 100, align: "center", search: false, sortable: false },
                            { name: 'Package', index: 'Package', height: 'auto', width: 120, align: "center", search: false, sortable: false },
                            { name: 'RoadName', index: 'RoadName', height: 'auto', width: 200, align: "center", search: false, sortable: false },
                            { name: 'RoadLength', index: 'RoadLength', height: 'auto', width: 170, align: "center", search: false, sortable: false },
                            { name: 'WorkStatus', index: 'WorkStatus', height: 'auto', width: 100, align: "center", search: false, sortable: false },
                            { name: 'Approve', index: 'Approve', height: 'auto', width: 100, align: "center", search: false, sortable: false },
                            { name: 'REQCODE', index: 'REQCODE', height: 'auto', width: 80, align: "center", search: false, sortable: false, hidden: true },
                            { name: 'COLLABORATION', index: 'COLLABORATION', height: 'auto', width: 80, align: "center", search: false, sortable: false, hidden: true }
        ],
        pager: jQuery('#dvlstPagerDropProposal'),
        rowNum: 10,
        //rowList: [10, 20, 30],
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
        onSelectRow:function(rowid,status,e)
        {
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

            if (records == droppedBefore.length) {
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
                 
                if (records != dropped.length) {
                    $('#dropAll').prop('checked', false);
                }
                else {
                    $('#dropAll').prop('checked', true);
                }
            });
            ///&& data.IsDOGenerated == false
            if (records != droppedBefore.length) {
                // $("#tblstDropProposal #dvlstPagerDropProposal").css({ height: '50px' });
              
                $("#dvlstPagerDropProposal_left").html("<input type='button' style='margin-left:27px' id='btnGenerateDetails' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'GenerateDropOrder();return false;' value='Approve'/>")
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

    if (DropApproveArrayAll.length > 0) {

        if (DropApproveArrayAll.length != records) {
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
          traditional:true,
                 data: { StateCode: postData.StateCode, StreamCode: $('#tblstDropProposal').jqGrid('getCol', 'COLLABORATION')[0], YearCode: postData.YearCode, BatchCode: $('#tblstDropProposal').jqGrid('getCol', 'Batch')[0].substring(7, 8), SchemeCode: postData.Scheme, RequestCode: $('#tblstDropProposal').jqGrid('getCol', 'REQCODE')[0], ApproveRoads: DropApproveArray },
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

    function ViewDistrictAbstract() {
        var postData = $("#tblstDropProposal").jqGrid('getGridParam', 'postData');
        window.open('/Proposal/PreviewDistrictAbstractDropReport?' + $.param({ StateCode: postData.StateCode, StreamCode: postData.StreamCode, YearCode: postData.YearCode, BatchCode: postData.BatchCode, SchemeCode: postData.Scheme, ProposalType: postData.Type }), '_blank');
    }

 function LoadDropedOrderList() {

        $("#tblstDroppedOrder").jqGrid('GridUnload');

        jQuery("#tblstDroppedOrder").jqGrid({
            url: '/Proposal/GetDropOrderList',
            datatype: "json",
            mtype: "POST",
            postData: { StateCode: $("#ddlStates option:selected").val(), YearCode: $("#ddlYears option:selected").val(), StreamCode: $("#ddlStreams option:selected").val(), BatchCode: $("#ddlBatch option:selected").val(), Scheme: $("#ddlSchemes option:selected").val(), Type: $("#ddlProposalType option:selected").val(), Status: $('#ddlStatus option:selected').val(), __RequestVerificationToken: $('#frmDropFilterProposal input[name=__RequestVerificationToken]').val() },
            colNames: ['State', 'Year', 'Batch', 'Collaboration', 'Scheme', 'No. of works for dropping', 'Request Date', /*'Is Approved', 'Approve Date',*/ 'View Details', 'View Orders'],
            colModel: [
                                { name: 'STATE', index: 'STATE', height: 'auto', width: 200, align: "center", search: false, sortable: false },
                                { name: 'YEAR', index: 'YEAR', height: 'auto', width: 200, align: "center", search: false, sortable: false },
                                { name: 'BATCH', index: 'BATCH', height: 'auto', width: 200, align: "center", search: false, sortable: false },
                                { name: 'COLLABORATION', index: 'COLLABORATION', height: 'auto', width: 180, align: "center", search: false, sortable: false },
                                { name: 'SCHEME', index: 'SCHEME', height: 'auto', width: 150, align: "center", search: false, sortable: false },
                                { name: 'COUNT', index: 'COUNT', height: 'auto', width: 190, align: "center", search: false, sortable: false },
                                { name: 'REQDATE', index: 'REQCODE', height: 'auto', width: 190, align: "center", search: false, sortable: true },
                               // { name: 'ISAPPROVE', index: 'ISAPPROVE', height: 'auto', width: 80, align: "center", search: false, sortable: false },
                                //{ name: 'APPDATE', index: 'APPDATE', height: 'auto', width: 80, align: "center", search: false, sortable: false },
                                { name: 'VIEW', index: 'VIEW', height: 'auto', width: 80, align: "center", search: false, sortable: false },
                                { name: 'ORDERDETAILS', index: 'ORDERDETAILS', height: 'auto', width: 80, align: "center", search: false, sortable: false },
            ],
            pager: jQuery('#dvlstPagerDropperOrder'),
            rowNum: 5,
            rowList: [5, 10, 15],
            viewrecords: true,
            recordtext: '{2} records found',
            sortname: 'REQDATE',
            sortorder: "asc",
            caption: "&nbsp;&nbsp; Drop Order Request",
            height: 'auto',
            //width:'100%',
            hidegrid: true,
            rownumbers: true,
            // autowidth: true,
            shrinkToFit: false,
            cmTemplate: { title: false },
            loadComplete: function () { },
            loadError: function () { }
        });
    }


 function ShowDetailDroppedProposelDetails(RequestCode)
 {
     $('#dvDetailDroppedOrderList').show();  //Show the current grid div first

     $("#tblstDetailDroppedOrder").jqGrid('GridUnload');
        jQuery("#tblstDetailDroppedOrder").jqGrid({
            url: '/Proposal/GetDetailDropOrderList',
            datatype: "json",
            mtype: "POST",
          //  postData: { StateCode: $("#ddlStates option:selected").val(), YearCode: $("#ddlYears option:selected").val(), StreamCode: $("#ddlStreams option:selected").val(), BatchCode: $("#ddlBatch option:selected").val(), Scheme: $("#ddlSchemes option:selected").val(), Type: $("#ddlProposalType option:selected").val(), Status: $('#ddlStatus option:selected').val() },
            postData: { RequestCode: RequestCode, Scheme: $("#ddlSchemes option:selected").val(), __RequestVerificationToken: $('#frmDropFilterProposal input[name=__RequestVerificationToken]').val() },
            colNames: ['No of Work', 'Order No', 'Order Date','Approve Date', 'View'],
            colModel: [
                                { name: 'NoofWork', index: 'NoofWork', height: 'auto', width: 80, align: "center", search: false, sortable: false },
                                { name: 'OrderNo', index: 'OrderNo', height: 'auto', width: 200, align: "center", search: false, sortable: false },
                                { name: 'OrderDate', index: 'OrderDate', height: 'auto', width: 180, align: "center", search: false, sortable: false },
                                { name: 'ApproveDate', index: 'ApproveDate', height: 'auto', width: 180, align: "center", search: false, sortable: false },
                                { name: 'View', index: 'View', height: 'auto', width: 80, align: "center", search: false, sortable: false },
                                 
            ],
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
 
 AddAntiForgeryToken = function (data) {
     debugger;
     data.__RequestVerificationToken = $('#frmDropFilterProposal input[name=__RequestVerificationToken]').val();
     return data;
 };