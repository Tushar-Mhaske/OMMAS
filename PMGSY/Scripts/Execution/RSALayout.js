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
     
        SaveRoadSafety();

    });

});
 
function SaveRoadSafety()
{
    if ($('#frmRoadSafetyLayout').valid()) {

        if (confirm("Do you want to save Details ? RSA Stage & Inspection Date can not be modified again once the details are saved. ")) {

            $.blockUI({ message: "<img src='/Content/images/ajax-loader.gif'>" });
            $.ajax({
                url: '/Execution/AddRSA',
                method: 'POST',
                cache: false,
                async: true,
                data: $('#frmRoadSafetyLayout').serialize(),
                dataType: 'json',
                success: function (data, status, xhr) {
                    alert(data.message)
                    if (data.success) {
                        $('#accordion').hide();  //close the form
                        $("#tbExecutionList").jqGrid('setGridState', 'visible'); //make the upper grid open
                    }
                    $.unblockUI();
                },
                error: function (xhr, status, err) {
                    alert("Error Occured.");
                    //   alert(xhr.responseText);
                    $.unblockUI();
                }
            });
        }
    }
    else {

        return false;
    }

}
