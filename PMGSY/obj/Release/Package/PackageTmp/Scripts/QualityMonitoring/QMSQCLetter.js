/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QMSQCLetter.js
        * Description   :   Handles events for SQC Letter
        * Author        :   Shyam Yadav 
        * Creation Date :   08/Jul/2014
 **/

$(document).ready(function () {

    SQCLetterListGrid();

    $("#btnViewSqcList").click(function () {
        SQCLetterListGrid();
    });

});


function closeDivError() {
    $("#divError").hide('slow');
}


function SQCLetterListGrid() {

    $("#tbSQCLetterList").jqGrid('GridUnload');

    jQuery("#tbSQCLetterList").jqGrid({
        url: '/QualityMonitoring/QMSQCLetterList',
        datatype: "json",
        mtype: "POST",
        colNames: ["State", "SQC Name", "Generate / View", "Send"],
        colModel: [
                    { name: 'State', index: 'State', width: 150, sortable: false, align: "left", search: false },
                    { name: 'Sqc', index: 'Sqc', width: 250, sortable: false, align: "left", search: false },
                    { name: 'View', index: 'View', width: 100, sortable: false, align: "left", search: false },
                    { name: 'Send', index: 'Send', width: 100, sortable: false, align: "left", search: false }
        ],
        postData: { "inspMonth": $("#ddlInspMonthSQCLetter").val(), "inspYear": $("#ddlInspYearSQCLetter").val() },
        pager: jQuery('#dvSQCLetterListPager'),
        rowNum: 100,
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "&nbsp;&nbsp;SQC List",
        sortname: 'State',
        height: '200',
        autowidth: true,
        loadComplete: function () {
            $('#tbSQCLetterList').setGridWidth(($('#divSQCLetterList').width() - 10), true);
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



function QMGenerateSQCLetter(sqcId) {
    blockPage();
    if ($("#frmSQCLetter").valid()) {
        $.ajax({
            type: 'POST',
            url: '/QualityMonitoring/AddLetterDetails',
            async: false,
            data: { id: sqcId, userType: 'S', inspMonth: $("#ddlInspMonthSQCLetter").val(), inspYear: $("#ddlInspYearSQCLetter").val() },
            beforeSend: function () {
                blockPage();
            },
            success: function (data) {
                if (data.success) {
                    alert('SQC Letter Generated Successfully.');
                    $("#tbSQCLetterList").trigger('reloadGrid');
                    QMOpenGeneratedSQCLetter(sqcId);
                    unblockPage();
                }
                else {
                    $("#divError").show("slow");
                    $("#divError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                    $.validator.unobtrusive.parse($('#mainDiv'));
                    unblockPage();
                }
                unblockPage();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                unblockPage();
            }
        })
    }
    else {
        return false;
        unblockPage();
    }

}


function QMViewSQCLetter(letterId) {

    window.open('/QualityMonitoring/DownloadLetter?' + $.param({ id: letterId, isLettterId: true, userType: 'S' }), '_blank');

}

function QMOpenGeneratedSQCLetter(sqcId) {
   
    window.open('/QualityMonitoring/DownloadLetter?' + $.param({ id: sqcId, isLettterId: false, userType: 'S', inspMonth: $("#ddlInspMonthSQCLetter").val(), inspYear: $("#ddlInspYearSQCLetter").val() }), '_blank');

}

function QMSendMailToSQC(sqcId, letterId)
{
    $.ajax({
        type: 'POST',
        url: '/QualityMonitoring/SendLetter',
        async: false,
        data: { userId: sqcId, userType: 'S', letterId: letterId },
        beforeSend: function () {
            blockPage();
        },
        success: function (data) {
            if (data.Success) {
                alert(data.Message);
                $("#tbSQCLetterList").trigger('reloadGrid');
                unblockPage();
            }
            else {
                $("#divError").show("slow");
                $("#divError span:eq(1)").html('<strong>Alert: </strong>' + data.Message);
                $.validator.unobtrusive.parse($('#mainDiv'));
                unblockPage();
            }
            unblockPage();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            unblockPage();
        }
    })
}