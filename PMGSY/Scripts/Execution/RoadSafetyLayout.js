/// <reference path="../jquery-1.9.1-vsdoc.js" />
$(document).ready(function () {
    $.validator.unobtrusive.parse("#frmRoadSafetyLayout");
    debugger;
    var agrdate = $('#Agreementdate').text().split('/');;
    var Agdate = new Date(agrdate[2], (parseInt(agrdate[1]) - 1), agrdate[0]);
  //  alert(Agdate)
    var end = new Date();
    //var diff = new Date(end - Agdate);
    var start = new Date(2000,0, 1);

    var diff = new Date(end - start);
    var days = diff/1000/60/60/24;
   // alert(Math.floor(days))
    $('#txtAuditDate').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a Audit date',
        maxDate: "0D",
        minDate: "-" + Math.floor(days)+ "D",
        buttonImageOnly: true,
        buttonText: 'Audit Date',
        changeMonth: true,
        changeYear: true,
        onSelect: function (selectedDate) {
            $('#txtAuditDate').trigger('blur');
        }
    });

    $('#chkTSC,#chkPIC,#chkPIURRNMU').click(function () {
        var TSC = $('#chkTSC').is(":checked");

        var PIC = $('#chkPIC').is(":checked");
        var PIURRNMU = $('#chkPIURRNMU').is(":checked");

        if (TSC == true || PIC == true || PIURRNMU == true) {
            $('#errspn').hide();
            $('#errspn').text("");
        }
    });
    $('#btnSave').click(function () {

        if ($('#frmRoadSafetyLayout').valid())
        {
            var TSC = $('#chkTSC').is(":checked");
            var PIC = $('#chkPIC').is(":checked");
            var PIURRNMU = $('#chkPIURRNMU').is(":checked");

            if (TSC == false && PIC == false && PIURRNMU == false) {
                $('#errspn').show();
                $('#errspn').text("please select at least one Road safety.");
                $('#errspn').css("color", "#E80C4D");
                return;
            }
            else {
                $('#errspn').hide();
                $('#errspn').text("");
                SaveRoadSafety();
            }
            
        }
    });

    var ProposalCode = $('#prRoadCode').val();
   // alert("alert"+ProposalCode)

    LoadRoadSafetyDetailsGrid(ProposalCode);
});
 
function SaveRoadSafety()
{ 
   

    $.blockUI({ message: "<img src='/Content/images/ajax-loader.gif'>" });

    $.ajax({
        url    : '/Execution/AddRoadSafety',
        method : 'POST',
        cache  : false,
        async  : true,
        data   : $('#frmRoadSafetyLayout').serialize(),
        dataType:'json',
        success: function (data, status, xhr)
        {
            alert(data.message)
            if (data.success) {
                $('#accordion').hide();  //close the form
                $("#tbExecutionList").jqGrid('setGridState', 'visible'); //make the upper grid open
            }
            $.unblockUI();
        },
        error: function (xhr, status, err)
        {
            alert(xhr.responseText);
            $.unblockUI();
        }
    });

}


//Road Safety Listing Starts   [by Pradip Patil on 05/05/2017]
function LoadRoadSafetyDetailsGrid(ProposalCode) {

    jQuery("#tbRoadSafetyList").jqGrid('GridUnload');
    jQuery("#tbRoadSafetyList").jqGrid({
        url: '/Execution/GetRoadSafetyList',
        datatype: "json",
        mtype: "POST",
        postData: { prRoadCode: ProposalCode },
        colNames: ['Stage', 'Road Safety', 'Audit Date'],
        colModel: [
                            { name: 'StageView', index: 'StageView', width: 250, align: "left", sortable: false },
                            { name: 'CondtuctBy', index: 'CondtuctBy', width: 200, sortable: false, align: "center" },
                            { name: 'ConductedDate', index: 'ConductedDate', width: 200, sortable: false, align: "center" },
        ],
        pager: jQuery('#pagerHabitationRoadList'),
        rowNum: 5,
        rowList: [5, 10, 15],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'ConductedDate',
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Road Safety List",
        height: 'auto',
        //autowidth: true,
        hidegrid: true,
        rownumbers: true,
        loadComplete: function (data) {
            //make the road safety grid in center

            $("#gbox_tbRoadSafetyList").css("margin-left", "500px");
        },
        loadError: function (xhr, status, error) {

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
