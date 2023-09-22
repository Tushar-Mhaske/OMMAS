/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QMMPVisit.js
        * Description   :   Handles events for Filters in MP Visit Screen
        * Author        :   Shyam Yadav 
        * Creation Date :   08/Apr/2015
 **/

$(document).ready(function () {
    $.validator.unobtrusive.parse($('#mpVisitFilterForm'));

    $(function () {
        $("#fillMPVisitDetailsAccordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $("#DistrictCode").change(function () {
       
        if ($("#DistrictCode").length > 0) {
            $("#BlockCode").empty();

            $.ajax({
                url: '/QualityMonitoring/PopulateBlocks',
                type: 'POST',
                data: { selectedDistrict: $("#DistrictCode").val(), value: Math.random() },
                success: function (jsonData) {
                    for (var i = 0; i < jsonData.length; i++) {
                        $("#BlockCode").append("<option value='" + jsonData[i].Value + "'>" + jsonData[i].Text + "</option>");
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    }); //--DistCode Change

    loadRoadListForMPVisit($("#StateCode").val(), $("#DistrictCode").val(), $("#BlockCode").val());

    //button in QualityFilters.cshtml
    $('#btnViewWorks').click(function () {

        loadRoadListForMPVisit($("#StateCode").val(), $("#DistrictCode").val(), $("#BlockCode").val());

    });//btn3TierListDetails ends here
    






});//doc.ready ends here


function loadRoadListForMPVisit(stateCode, districtCode, blockCode) {
    
    $("#tbMPVisitRoadList").jqGrid('GridUnload');

    jQuery("#tbMPVisitRoadList").jqGrid({
        url: '/QualityMonitoring/GetRoadListForMPVisit?' + Math.random(),
        datatype: "json",
        mtype: "POST",
        colNames: ["State", "District", "Block", "Package", "Sanction Year", "Road / LSB Name", "Type",
                    "Length (Road in Km / LSB in Mtrs)", "Sanctioned Cost", "MP Visit Details"],
        colModel: [
                            { name: 'State', index: 'State', width: 70, sortable: false, align: "center", search: false },
                            { name: 'District', index: 'District', width: 40, sortable: false, align: "left", search: false },
                            { name: 'Block', index: 'Block', width: 40, sortable: false, align: "left", search: false },
                            { name: 'Package', index: 'Package', width: 40, sortable: false, align: "left", search: false },
                            { name: 'SanctionYear', index: 'SanctionYear', width: 45, sortable: false, align: "center", search: false },
                            { name: 'RoadName', index: 'RoadName', width: 140, sortable: false, align: "left", search: false },
                            { name: 'PropType', index: 'PropType', width: 40, sortable: false, align: "left", search: false },
                            { name: 'Length', index: 'Length', width: 35, sortable: false, align: "center", search: false },
                            { name: 'Cost', index: 'Cost', width: 60, sortable: false, align: "center", search: false },
                            { name: 'EnterData', index: 'EnterData', width: 30, sortable: false, align: "center", search: false }
        ],
        postData: { "stateCode": stateCode, "districtCode": districtCode, "blockCode": blockCode},
        pager: jQuery('#dvMPVisitRoadListPager'),
        rowNum: 20000,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;Sanctioned Works",
        pgbuttons: false,
        pgtext: null,
        height: '300',
        autowidth: true,
        //sortname: 'Monitor',
        //rowList: [5, 10, 15],
        grouping: true,
        groupingView: {
            groupField: ['State', 'District'],
            groupText: ['<b>{0}</b>', '<b>{0}</b>'],
            groupColumnShow: [false, false],
            groupCollapse: false
        },
        loadComplete: function () {
            //$('#tbMPVisitRoadList').setGridWidth(($('#div3TierInspectionList').width() - 10), true);
            unblockPage();
        },
        loadError: function (xhr, status, error) {
            if (xhr.responseText == "session expired") {
                window.location.href = "/Login/SessionExpire";
            }
            else {
                window.location.href = "/Login/SessionExpire";
            }
        }
    }); //end of grid
}

function CloseMPVisitDetails()
{

    $("#fillMPVisitDetailsAccordion").hide('slow');
    $("#divList").hide('slow');
    loadRoadListForMPVisit($("#StateCode").val(), $("#DistrictCode").val(), $("#BlockCode").val());


}
function qmFillMPVisitDetails(prRoadCode)
{
    jQuery('#tbMPVisitRoadList').jqGrid('setSelection', prRoadCode);
   // $('#divList').show();
    //
    $("#divfillMPVisitDetailsForm").show();
    $("#divList").show();
    //

    $("#fillMPVisitDetailsAccordion div").html("");
    $("#fillMPVisitDetailsAccordion h3").html(
                    "<a href='#' style= 'font-size:.9em;' >Fill MP Visit Details</a>" +
                    '<a href="#" style="float: right;">' +
                    '<img class="ui-icon ui-icon-closethick" onclick="CloseMPVisitDetails();" /></a>'
                    );

    $('#fillMPVisitDetailsAccordion').show('fast', function ()
    {
       

        $("#divfillMPVisitDetailsForm").load("/QualityMonitoring/FillMPVisitDetails/" + prRoadCode , function ()
        {
            //$.validator.unobtrusive.parse($('#divProposalForm'));
        });

        $("#divfillMPVisitDetailsForm").css('height', 'auto');
        $('#divfillMPVisitDetailsForm').show('slow');
    });

    $("#tbMPVisitRoadList").jqGrid('setGridState', 'hidden');
}