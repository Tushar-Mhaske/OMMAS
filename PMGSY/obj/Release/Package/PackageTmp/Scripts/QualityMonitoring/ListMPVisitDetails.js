$(document).ready(function () {
    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    // Calling The Load List Function 
    LoadDetailsList($("#PrRoadCode").val());
    

});

// Format Columns Function

function FormatColumn(cellvalue, options, rowObject) {

    if (cellvalue != '') {
        return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit MP Visit Details' onClick ='EditMPVisitDetailsDetails(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete MP Visit Details ' onClick ='DeleteMPVisitDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }

}


// Load List Function

function LoadDetailsList(PrRoadCode) {
    $('#tbList').jqGrid('GridUnload');
    $('#tbList').jqGrid({
        url: '/QualityMonitoring/GetMPVisitList',
        datatype: "json",
        mtype: "POST",
        colNames: ['MP Name','Date of Visit','MP House','Remarks','Action','PDF / Image Upload',],
        colModel: [
                      { name: 'MP_NAME', index: 'MP_NAME', height: 'auto', width: 225, align: "left", sortable: true },
                     // { name: 'PIU_NAME', index: 'PIU_NAME', height: 'auto', width: 150, align: "left", sortable: true },
                       { name: 'DATE_OF_VISIT', index: 'DATE_OF_VISIT', height: 'auto', width: 225, align: "left", sortable: true },
                      { name: 'MP_HOUSE', index: 'MP_HOUSE', height: 'auto', width: 200, align: "left", sortable: true },
                    
                      { name: 'REMARKS', index: 'REMARKS', height: 'auto', width: 300, align: "left", sortable: true },
                      { name: 'a', width: 95, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false },
                      { name: 'FileUpload', index: 'FileUpload', height: 'auto', width: 80, align: "left", sortable: true },
        ],
        pager: jQuery('#dvPager'),
        rowNum: 4,
        postData: { PrRoadCode: PrRoadCode },
        rowList: [4,8],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'NoName',
        sortorder: "asc",
        caption: "MP Visit Details List",
        height: 'auto',
        autowidth: '100%',
        rownumbers: true,
        hidegrid: false,
    });

}


function EditMPVisitDetailsDetails(urlparameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        type: 'GET',
        url: '/QualityMonitoring/EditMPVisitDetails/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {

            $("#divfillMPVisitForm").html(data);

            //$("#divfillMPVisitForm").show();
           

            if (data.success == false) {
                alert('Error occurred while processing your request.');
            }
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }

    })
}


function DeleteMPVisitDetails(urlparameter) {
    if (confirm("Are you sure you want to delete MP Visit details?")) {
        $.ajax({
            type: 'POST',
            url: '/QualityMonitoring/DeleteMPVisitDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {
                    alert('MP Visit details deleted successfully.');
                    $('#tbList').trigger('reloadGrid');
                   // $("#divfillMPVisitForm").load("/QualityMonitoring/FillMPVisitDetails"+ $("#PrRoadCode").val());
                    
                }
                else if (data.success == false) {
                    alert('MP Visit details are in use and can not be deleted.');
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                //alert(xhr.responseText);
            }
        });
    }
    else {
        return false;
    }
}





function CloseMP() {
    $('#accordion').hide('slow');
    $('#divfillMPVisitForm').hide('slow');
    $("#tbList").jqGrid('setGridState', 'visible');
    //LoadDetailsList($("#PrRoadCode").val());
    showFilter();
}

function UploadMPFiles(urlParameter) {
   
  
    jQuery('#tbList').jqGrid('setSelection', urlParameter);

    //  $("#tbList").jqGrid('setGridState', 'hidden');
    //$("#divfillMPVisitDetailsForm").hide();
   // $("#divList").hide();

   $("#divfillMPVisitForm").jqGrid('setGridState', 'hidden');

    $("#divfillMPVisitForm").load('/QualityMonitoring/FileUploadMPVisit/' + urlParameter, function () {

    });

    //$("#accordion div").html("");
    //$("#accordion h3").html(
    //        "<a href='#' style= 'font-size:.9em;' >Upload C Proforma</a>" +
    //        '<a href="#" style="float: right;">' +
    //        '<img  class="ui-icon ui-icon-closethick" onclick="CloseMP();" /></a>'
    //        );

    //$('#accordion').show('fold', function () {

    
    //    $('#divfillMPVisitForm').show('slow');
    //    $("#divfillMPVisitForm").css('height', 'auto');
    //});

    //$("#tbProposalList").jqGrid('setGridState', 'hidden');
    //$('#idFilterDiv').trigger('click');
    //$("#tbLSBProposalList").jqGrid('setGridState', 'hidden');
    //$("#tbSRRDALSBProposalList").jqGrid('setGridState', 'hidden');
}