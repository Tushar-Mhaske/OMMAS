
$.unblockUI();

$.validator.unobtrusive.parse($("#frmRevoke"));

$(document).ready(function () {

    var dateObj = new Date();
    var currentMonth = dateObj.getMonth()+1;
    var currentYear = dateObj.getFullYear();
   
    if ($("#levelId").val() == 4) {
      //  alert('inside');
        $("#ddlSRRDA").val($("#AdminNdCode").val());
        $("#ddlSRRDA").prop("disabled", true);
        $('#ddlSRRDA').trigger('change');
       // getMonthClosedDetails();
    }


    $('#divPeriodicMessage').hide();
    //added by Abhishek to show month start/close details at srrda level start
    $(function () {

        var countOwn = 0;
        var countLower = 0;

        $("#rdOwn").click(function () {
            countLower = 0;

            if (($("#rdOwn").prop("checked"))) {
                countOwn++;
                // if (countOwn == 1) {
                    //$("#ddlDPIU").empty();
                    $("#ddlDPIU").hide("slow");
                    $("#spnDPIU").hide();  
                    getMonthClosedDetails();
                //}
            }

        });

        $("#rdDPIU").click(function () {
            if (($("#rdDPIU").prop("checked"))) {
                    $("#ddlDPIU").show("slow");
                    $("#spnDPIU").show();
                  
            }
        });
      
        $("#ddlDPIU,#ddlSRRDA").change(function () {
            if ($('#ddlSRRDA').val()!="0") {
                $("#spnMonthClosedDetailsUsingAjax").show("slow");
                getMonthClosedDetails();
            }
            else {
                $("#spnMonthClosedDetailsUsingAjax").hide();
            }
            
        });

        $("#FundType").change(function () {
          //  getMonthClosedDetails();
        });

        if ($("#rdDPIU").is(":checked"))
        {
            $("#ddlDPIU").show();
        }
       
    });
    //added by Abhishek to show month start/close details at srrda level end
    
    $("#btnView_Monthly").click(function () {
        //validation changes by Deendayal for monthly revoke of DPIU 18-Sep-2014 
     
        if (ValidateClosedMonthsForMonthly()) {
            RevokeClosing();
        }
    
    });


    //Added by Deendayal for Periodic revoke on 03/01/2017
    $('#btnView_Periodic').click(function() {
        if (ValidatePeriod() && ValidateClosedMonthsForPeriodic()) {
            $('#StartMonth').val($("#pStartMonth").val());
            $('#StartYear').val($("#pStartYear").val());
          
            $('#Remark').val($("#pRemarkId").val());
            RevokeClosing();
        }
      
    });

    //Added by Deendayal for validation of Months as per current month and closed month on 03/01/2017
    function ValidateClosedMonthsForMonthly() {
       
        var closedMonths = $("#MonthClosed").val();
        var actualMonth = $("#StartMonth :selected").text();
        var actualMonthValue = parseInt($("#StartMonth").val());
        var actualYearValue = parseInt($("#StartYear").val());
        var actualYear = $("#StartYear :selected").text();
        if (closedMonths == "") {
            if (actualMonthValue > currentMonth && actualYearValue == currentYear) {               
                alert("Month Selection is invalid");
                return false;
            }

            return true;
        }
        else {
            var res = closedMonths.split(" ");
            var closedMonthData = res[0];
            var closedYearData = res[1];
            if (closedMonthData == actualMonth && closedYearData == actualYear) {
                return true;
            }
            else {
                //alert("First Revoke Month " + closedMonthData + " of Year " + closedYearData);

                alert("Month not closed for " + $('#StartMonth option:selected').text() + "  " + $('#StartYear').val());
                return false;
            }

        }
    }

    //Added by Deendayal for validation of Period as per current month and closed month on 03/01/2017
    function ValidateClosedMonthsForPeriodic() {
        debugger;
        var closedMonths = $("#MonthClosed").val();
        var actualMonth = $("#pStartMonth :selected").text();
        var selectedStartMonthValue = $("#pStartMonth").val();
        selectedStartMonthValue = parseInt(selectedStartMonthValue);
        var selectedToMonthValue = parseInt($("#ToMonth").val());
        var actualYear = $("#pStartYear :selected").text();
        if (closedMonths == "") {
            if (selectedStartMonthValue > currentMonth && selectedToMonthValue > currentMonth) {
                alert("Month Selection is invalid");
                return false;
            }
            return true;
        }
        else {
            var res = closedMonths.split(" ");
            var closedMonthData = res[0];
            var closedYearData = res[1];
            if (closedMonthData == actualMonth && closedYearData == actualYear) {
                return true;
            }
            else {
                alert("Start Month must be " + closedMonthData + " and Start Year must be " + closedYearData);
                return false;
            }

        }
    }
    //Added by Deendayal for validation of start Months and end Months  on 03/01/2017
    function ValidatePeriod() {
        var startMonthValue = parseInt($("#pStartMonth").val());
        var ToMonthValue = parseInt($("#ToMonth").val());
        var startYearValue = parseInt($("#pStartYear").val());
        var ToYearValue = parseInt($("#ToYear").val());
        if (startYearValue > ToYearValue ) {
            return true;
        }
        else {
            if (startYearValue == ToYearValue) {

                if ( ToMonthValue <= startMonthValue ) {
                   
                    return true;
                }
                else {
                    alert("Revoking Period is invalid");
                    return false;
                }
               
            }
            else {
                alert("Revoking Period is invalid");
                return false;
            }
        }
    
    }

//Revoke Months or Period
    function RevokeClosing() {
        
        if ($("#frmRevoke").valid()) {

            $.ajax({
                url: "/RevokeClosing/RevokeClosing/",
                type: "POST",
                async: false,
                cache: false,
                data: $("#frmRevoke").serialize(),
                success: function (data) {
                    $.unblockUI();
                    if (data.success) {
                        alert(data.message);
                       
                        if (!($("#levelId").val() == 4)) {

                            $('#frmRevoke').trigger('reset');

                            $('#ddlDPIU').empty(); //pp
                            $('#ddlDPIU').append("<option value='0'>Select DPIU</option>");//pp
                            $('#ddlDPIU').hide();
                            $('#monthClosingRow').hide("slow");
                            $('#yearClosingRow').hide("slow");
                            $("#spnMonthClosedDetailsUsingAjax").hide();
                            $('#divPeriodicMessage').hide("slow");
                        }
                        else {
                            $("#ddlSRRDA").val($("#AdminNdCode").val());
                            $("#ddlDPIU").val('0');
                            $("#StartMonth").val('0');
                            $("#StartYear").val('0');
                            $("#Remark").val(' ');
                        }

                            return true;
                        }
                    else {
                            alert(data.message);
                           // $("#divRevokeError").show("slide");
                        //  $("#divRevokeError span:eq(1)").html('<strong style="color:red">Alert: ' + data.message + '</strong>');
                            
                        }
                        return false;
                    
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    $.unblockUI();
                    alert("Error while processing request");
                }
            });
        }
        $.unblockUI();

    }

    //Added by Deendayal operation on selection of Periodic radio button
    $('#periodicClosing').click(function () {
        var dpiu = $("#rdDPIU").prop("checked");
        var srrdaValue = $('#ddlSRRDA').val();
        var srrdaCheck = $("#rdOwn").prop("checked");
        var value = $("#ddlDPIU").val();
        if (($("#periodicClosing").prop("checked"))) {
            $('#monthClosingRow').hide("slow");
            $('#yearClosingRow').show("slow");
            $("#frmRevoke").trigger('reset');
            $('#ddlSRRDA').val(srrdaValue);
            $("#periodicClosing").prop("checked", true);
            $('#divPeriodicMessage').show('slow');
        }
        if (dpiu) {
            $("#rdDPIU").prop("checked", true);
          
            $("#ddlDPIU").val(value);
        }
        if (srrdaCheck) {
            $("#rdOwn").prop("checked", true);
        }
    });

    //Added by Deendayal operation on selection of Monthly radio button
    $('#monthClosing').click(function () {
        var dpiu = $("#rdDPIU").prop("checked");
        var srrdaCheck = $("#rdOwn").prop("checked");
        var srrdaValue = $('#ddlSRRDA').val();
        var value = $("#ddlDPIU").val();

        if (($("#monthClosing").prop("checked"))) {
            $('#monthClosingRow').show("slow");
            $('#yearClosingRow').hide("slow");
            $("#frmRevoke").trigger('reset');
            $('#ddlSRRDA').val(srrdaValue);
            $("#monthClosing").prop("checked", true);
            $('#divPeriodicMessage').hide('slow');
        }
        if (dpiu) {
            $("#rdDPIU").prop("checked", true);
          
            $("#ddlDPIU").val(value);
        }
        if (srrdaCheck) {
            $("#rdOwn").prop("checked", true);
        }
    });

    $(function () {
        //if ($("#rdOwn").is(":checked")) {
       // alert($("#lvlID").val());
        if ($("#lvlID").val() == 6) {
            $('#ddlSRRDA').trigger('change');
        }
          //  $("#ddlDPIU").hide();
     //   }
    });

    //Added By Abhishek 5 Jan 2015
    $("#ddlSRRDA").change(function () {

        var adminNdCode = $('#ddlSRRDA option:selected').val();
        $.ajax({
            url: '/RevokeClosing/PopulateDPIU/' + adminNdCode,
            type: 'GET',
            catche: false,
            error: function (xhr, status, error) {
                alert('An Error occured while processig your request.')
                return false;
            },
            success: function (data) {
                $('#ddlDPIU').empty();
                $.each(data, function () {
                    $('#ddlDPIU').append("<option value=" + this.Value + ">" + this.Text + "</option>");
                });
            }
        });

    });
});

//Get Months closed details
function getMonthClosedDetails()
{

    var OwnDPIUFlag;

    if($("#rdOwn").is(":checked"))
    {
        OwnDPIUFlag="O";
    }
    else if ($("#rdDPIU").is(":checked")) {
        OwnDPIUFlag="D";    
    }

    $.ajax({
        url: "/RevokeClosing/GetMonthClosedDetails/",
        type: "POST",
        async: false,
        cache: false,
        data: { adminNdCodeDPIU: $("#ddlDPIU option:selected").val(), OwnDPIUFlag: OwnDPIUFlag, FundType: $("#FundType").val(), SRRDANdCode: $("#ddlSRRDA option:selected").val() },
        success: function (data) {
            $.unblockUI();
            if (!data.status) {//fail                
                alert(data.message);
                return false;
            }
            else {//success
                
                $("#spnMonthClosedDetails").hide();

                if (data.message == "") {
                    $('#MonthClosed').val("");
                    $("#spnMonthClosedDetailsUsingAjax").html("Months not yet closed");

                } else {
                    $('#MonthClosed').val(data.message);
                    $("#spnMonthClosedDetailsUsingAjax").html("Months Closed till "+data.message);
                }
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
            alert("Error while processing request");
        }
    });   
}