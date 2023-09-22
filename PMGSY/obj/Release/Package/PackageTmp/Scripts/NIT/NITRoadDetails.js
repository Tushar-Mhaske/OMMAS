
$.validator.unobtrusive.adapters.add('datecomparefieldvalidator', ['date'], function (options) {
    options.rules['datecomparefieldvalidator'] = options.params;
    options.messages['datecomparefieldvalidator'] = options.message;
});


$.validator.addMethod("datecomparefieldvalidator", function (value, element, params) {


    var compareDate = $("#" + params.date).val();

    if (params.date == "TEND_RECEVING_DATE") {
        if (new Date(compareDate.split('/')[2], compareDate.split('/')[1], compareDate.split('/')[0]) > new Date(value.split('/')[2], value.split('/')[1], value.split('/')[0]))
            return false;
        else
            return true;
    }


    if (params.date == "TEND_OPENING_DATE") {
        if (new Date(compareDate.split('/')[2], compareDate.split('/')[1], compareDate.split('/')[0]) > new Date(value.split('/')[2], value.split('/')[1], value.split('/')[0]))
            return false;
        else
            return true;
    }


    if (params.date == "TEND_DATE_OF_TECHNICAL_OPENING") {
        if (new Date(compareDate.split('/')[2], compareDate.split('/')[1], compareDate.split('/')[0]) > new Date(value.split('/')[2], value.split('/')[1], value.split('/')[0]))
            return false;
        else
            return true;
    }


    if (params.date == "TEND_DATE_OF_FINANCIAL_OPENING") {
        if (new Date(compareDate.split('/')[2], compareDate.split('/')[1], compareDate.split('/')[0]) > new Date(value.split('/')[2], value.split('/')[1], value.split('/')[0]))
            return false;
        else
            return true;
    }


    if (params.date == "CurrentDate") {


        element = this.validationTargetFor(this.clean(element));

        compareDate = new Date();

        if ($('#TEND_RECEVING_DATE').val() != '' && element.name == 'TEND_RECEVING_DATE') {

            if (new Date(compareDate.getFullYear(), compareDate.getMonth() + 1, compareDate.getDate()) > new Date(value.split('/')[2], value.split('/')[1], value.split('/')[0]))
                return false;

            else
                return true;

        }

        //if ($('#TEND_ISSUE_START_DATE').val() != '' && element.name == 'TEND_ISSUE_START_DATE') {

        //    if (new Date(compareDate.getFullYear(), compareDate.getMonth() + 1, compareDate.getDate()) > new Date(value.split('/')[2], value.split('/')[1], value.split('/')[0]))
        //        return false;
        //    else
        //        return true;

        //}
    }



    return true;
});



jQuery.validator.addMethod("comparefieldvalidator", function (value, element, param) {

    var earnestMoney = parseFloat($('#TEND_EARNEST_MONEY').val());
    var estCost = parseFloat($('#TEND_EST_COST').val());
    if (earnestMoney >= estCost)
        return false;
    else
        return true;
});

jQuery.validator.unobtrusive.adapters.addBool("comparefieldvalidator");


$.validator.unobtrusive.adapters.add('maintenancedatevalidator', ['date'], function (options) {
    options.rules['maintenancedatevalidator'] = options.params;
    options.messages['maintenancedatevalidator'] = options.message;
});

jQuery.validator.addMethod("maintenancedatevalidator", function (value, element, param) {


    element = this.validationTargetFor(this.clean(element));

 
    if (element.name == 'TEND_DATE_OF_PREBID') {
        if ($('#TenderIssueStartDate').val() != '' && $('#TEND_DATE_OF_PREBID').val() != '') {


            if (new Date($('#TenderIssueStartDate').val().split('/')[2], $('#TenderIssueStartDate').val().split('/')[1], $('#TenderIssueStartDate').val().split('/')[0]) > new Date(value.split('/')[2], value.split('/')[1], value.split('/')[0])) {

                return false;
            }
            else {

                return true;
            }
        }

    }

    return true;

});

$(document).ready(function () {

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $.validator.unobtrusive.parse($('#frmNITRoadDetails'));

    if ($('#ddlWorks').length > 1) {
        $('#tdddlRoad').attr('colspan', 1);
        $('#tdlblWorks').show('slow');
        $('#tdddlWorks').show('slow');
    }

    LoadNITRoadDetails();



    $('#TEND_RECEVING_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        buttonText: 'Deadline for Receiving Bids Date',
        minDate:new Date(),
        onSelect: function (selectedDate) {
            $("#TEND_OPENING_DATE").datepicker("option", "minDate", selectedDate);
         //   $("#TEND_OPENING_DATE").datepicker("option", "maxDate", selectedDate);
        },
        onClose: function () {

            $(this).focus().blur();
        }
    });

    $('#TEND_OPENING_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        buttonText: 'Tender Opening Date',
        onSelect: function (selectedDate) {
            $("#TEND_DATE_OF_TECHNICAL_OPENING").datepicker("option", "minDate", selectedDate);
            $("#TEND_RECEVING_DATE").datepicker("option", "maxDate", selectedDate);
        },
        onClose: function () {

            $(this).focus().blur();
        }
    });



    $('#TEND_DATE_OF_TECHNICAL_OPENING').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        buttonText: 'Opening of Technical Bid Date',
        onSelect: function (selectedDate) {
            $("#TEND_DATE_OF_FINANCIAL_OPENING").datepicker("option", "minDate", selectedDate);
            $("#TEND_OPENING_DATE").datepicker("option", "maxDate", selectedDate);
            //$("#TEND_DATE_OF_COMMENCEMENT").datepicker("option", "minDate", selectedDate);
        },
        onClose: function () {

            $(this).focus().blur();
        }
    });

    $('#TEND_DATE_OF_FINANCIAL_OPENING').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        buttonText: 'Opening of Financial Bid Date',
        onSelect: function (selectedDate) {
            $("#TEND_DATE_OF_TECHNICAL_OPENING").datepicker("option", "maxDate", selectedDate);
            $("#TEND_DATE_OF_BID_VALIDITY").datepicker("option", "minDate", selectedDate);
        },
        onClose: function () {

            $(this).focus().blur();
        }
    });

    $('#TEND_DATE_OF_PREBID').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        buttonText: 'Pre Bid Meeting Date',
        onSelect: function (selectedDate) {
            $("#TEND_INSP_END_DATE").datepicker("option", "minDate", selectedDate);
            $("#TEND_ISSUE_END_DATE").datepicker("option", "maxDate", selectedDate);
            //$("#TEND_DATE_OF_AGREEMENT").datepicker("option", "maxDate", selectedDate);
            //$("#TEND_DATE_OF_COMMENCEMENT").datepicker("option", "minDate", selectedDate);
        },
        onClose: function () {

            $(this).focus().blur();
        }
    });

    $('#TEND_DATE_OF_BID_VALIDITY').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        buttonText: 'Bid Validity Date',
        onSelect: function (selectedDate) {
            $("#TEND_DATE_OF_FINANCIAL_OPENING").datepicker("option", "maxDate", selectedDate);
        },
        onClose: function () {

            $(this).focus().blur();
        }
    });


    $('#TEND_RECEVING_TIME').timepicker({
       
        showLeadingZero: false,
        //onHourShow: tpStartOnHourShowCallback,
        //onMinuteShow: tpStartOnMinuteShowCallback,       
        showCloseButton: true,
        showDeselectButton: true,
        showOn: 'button',
        button: "#tmReceivingBid",
        minutes: {
            starts: 0,
            ends: 59,
            interval: 1
        },
        onClose: function () {

            $(this).focus().blur();
        }
    });

    $('#TEND_OPENING_TIME').timepicker({

        showLeadingZero: false,
        //onHourShow: tpStartOnHourShowCallback,
        //onMinuteShow: tpStartOnMinuteShowCallback,
        showCloseButton: true,
        showDeselectButton: true,
        showOn: 'button',
        button: "#tmTenderOpening",
        minutes: {
            starts: 0,
            ends: 59,
            interval: 1
        },
        onClose: function () {

            $(this).focus().blur();
        }
    });

    $('#TEND_TIME_OF_TECHNICAL_OPENING').timepicker({

        showLeadingZero: false,
        //onHourShow: tpStartOnHourShowCallback,
        //onMinuteShow: tpStartOnMinuteShowCallback,
        showCloseButton: true,
        showDeselectButton: true,
        showOn: 'button',
        button: "#tmOpeningofTechicalBid",
        minutes: {
            starts: 0,
            ends: 59,
            interval: 1
        },
        onClose: function () {

            $(this).focus().blur();
        }
    });

    $('#TEND_TIME_OF_FINANCIAL_OPENING').timepicker({

        showLeadingZero: false,
        //onHourShow: tpStartOnHourShowCallback,
        //onMinuteShow: tpStartOnMinuteShowCallback,
        showCloseButton: true,
        showDeselectButton: true,
        showOn: 'button',
        button: "#tmOpeningofFinancialBid",
        minutes: {
            starts: 0,
            ends: 59,
            interval: 1
        },
        onClose: function () {

            $(this).focus().blur();
        }
    });

    $('#TEND_TIME_OF_PREBID').timepicker({

        showLeadingZero: false,
        //onHourShow: tpStartOnHourShowCallback,
        //onMinuteShow: tpStartOnMinuteShowCallback,
        showCloseButton: true,
        showDeselectButton: true,
        showOn: 'button',
        button: "#tmPreBidMeetingBid",
        minutes: {
            starts: 0,
            ends: 59,
            interval: 1
        },
        onClose: function () {

            $(this).focus().blur();
        }
    });

    $('#TEND_TIME_OF_BID_VALIDITY').timepicker({

        showLeadingZero: false,
        //onHourShow: tpStartOnHourShowCallback,
        //onMinuteShow: tpStartOnMinuteShowCallback,
        showCloseButton: true,
        showDeselectButton: true,
        showOn: 'button',
        button: "#tmBidValidity",
        minutes: {
            starts: 0,
            ends: 59,
            interval: 1
        },
        onClose: function () {

            $(this).focus().blur();
        }
    });


    $("#ddlSanctionedYears").change(function () {

        if ($("#dvErrorMessage").is(":visible")) {
            $('#dvErrorMessage').hide('slow');
            $('#message').html('');
        }


        FillInCascadeDropdown({ userType: $("#ddlSanctionedYears").find(":selected").val() },
                    "#ddlPackages", "/NIT/GetPackagesByYear?sanctionYear=" + $('#ddlSanctionedYears option:selected').val() );

        $('#ddlRoads').empty();
        $('#ddlRoads').append("<option value=0>Select Road</option>");

        $('#ddlWorks').empty();
        $('#ddlWorks').append("<option value=0>Select Work</option>");

    }); //end function year change

    $("#ddlPackages").change(function () {

     
        FillInCascadeDropdown({ userType: $("#ddlPackages").find(":selected").val() },
                    "#ddlRoads", "/NIT/GetRoadsByPackag?sanctionYear=" + $('#ddlSanctionedYears option:selected').val() + "&packageID=" + $('#ddlPackages option:selected').val());

     
        $('#ddlWorks').empty();
        $('#ddlWorks').append("<option value=0>Select Work</option>");

    }); //end function package change

    $("#ddlRoads").change(function () {


        //FillInCascadeDropdown({ userType: $("#ddlRoads").find(":selected").val() },
        //            "#ddlWorks", "/NIT/GetWorksByRoad?roadCode=" + $('#ddlRoads option:selected').val());

       
       
        var itemCount = 0;
        $.ajax({
            type: 'POST',
            url: '/NIT/GetWorksByRoad?roadCode=' + $("#ddlRoads option:selected").val() ,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {

                itemCount = data.length;
                $('#ddlWorks').empty();
                $.each(data, function () {
                    $('#ddlWorks').append("<option value=" + this.Value + ">" + this.Text + "</option>");
                });

            },
            error: function (xhr, ajaxOptions, thrownError) {
                // alert(xhr.responseText);
                $.unblockUI();
            }
        });



     
        if (itemCount > 1) {
            
            $('#tdddlRoad').attr('colspan', 1);
            $('#tdlblWorks').show('slow');
            $('#tdddlWorks').show('slow');
        }
        else {
            $('#tdlblWorks').hide();
            $('#tdddlWorks').hide();
            $('#tdddlRoad').attr('colspan', 3);
          

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                type: 'GET',
                url: '/NIT/GetEstimatedCostMaintenanceCost?roadCode=' + $("#ddlRoads option:selected").val()+ "&workCode=0",
                dataType: 'json',
                async: false,
                cache: false,
                success: function (data) {
                   
                    var totalEstimatedCost = data.totalEstimatedCost;
                    var totalMaintenanceCost = data.totalMaintenanceCost;

                    $('#TEND_EST_COST').val(totalEstimatedCost);
                    $('#TEND_MAINT_COST').val(totalMaintenanceCost);

                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                   // alert(xhr.responseText);
                    $.unblockUI();
                }
            });
        }
    }); //end function road change


    $('#ddlWorks').change(function () {


        if ($("#ddlWorks option:selected").val() > 0) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                type: 'GET',
                url: '/NIT/GetEstimatedCostMaintenanceCost?roadCode=' + $("#ddlRoads option:selected").val() + "&workCode=" + $("#ddlWorks option:selected").val(),
                dataType: 'json',
                async: false,
                cache: false,
                success: function (data) {

                    var totalEstimatedCost = data.totalEstimatedCost;
                    var totalMaintenanceCost = data.totalMaintenanceCost;

                    $('#TEND_EST_COST').val(totalEstimatedCost);
                    $('#TEND_MAINT_COST').val(totalMaintenanceCost);

                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    // alert(xhr.responseText);
                    $.unblockUI();
                }
            });
        }
        else {

            $('#TEND_EST_COST').val('');
            $('#TEND_MAINT_COST').val('');
        }


    });//end function work change


    $('#btnSaveNITRoadDetails').click(function (e) {


        if ($('#frmNITRoadDetails').valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/NIT/AddNITRoadDetails",
                type: "POST",
                data: $("#frmNITRoadDetails").serialize(),
                success: function (data) {

                    if (data.success == true) {

                        alert(data.message);
                        $('#btnResetNITRoadDetails').trigger('click');

                        $('#ddlRoads').empty();
                        $('#ddlRoads').append("<option value=0>Select Road</option>");

                        $('#ddlWorks').empty();
                        $('#ddlWorks').append("<option value=0>Select Work</option>");

                        $('#tbNITRoadList').jqGrid("setGridParam", { "postData": { TendNITCode: $('#EncryptedTendNITCode').val() } });
                        $('#tbNITRoadList').trigger('reloadGrid', [{ page: 1 }]);

                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $('#dvNITRoadDetails').html(data);
                        $('#dvNewNITRoadDetails').show('slow');
                        $('#dvNITRoadDetails').show('slow');
                    }


                    $.unblockUI();

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }

            });

        }
    });

    $('#btnResetNITRoadDetails').click(function (e) {

        if ($("#dvErrorMessage").is(':visible')) {
            $('#dvErrorMessage').hide('slow');
            $('#message').html('');
        }
     
    });

    $("#btnUpdateNITRoadDetails").click(function (e) {


        if ($("#frmNITRoadDetails").valid()) {

            var encryptedTendNITCode = $('#EncryptedTendNITCode').val();

            $('#ddlRoads').attr('disabled', false);
            $('#ddlWorks').attr('disabled', false);
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                type: 'POST',
                url: '/NIT/EditNITRoadDetails',
                async: false,
                data: $("#frmNITRoadDetails").serialize(),
                success: function (data) {

                    if (data.success == true) {
                        alert(data.message);
                        $('#dvNITRoadDetails').load('/NIT/NITRoadDetails/' + encryptedTendNITCode, function () {
                           // $('#dvNewSplitWorkDetails').show('slow');
                            $('#EncryptedTendNITCode').val(encryptedTendNITCode);
                            $('#dvNITRoadDetails').hide();
                            $.unblockUI();
                        });

                        $('#tbNITRoadList').trigger('reloadGrid');
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                        $('#ddlRoads').attr('disabled', true);
                        $('#ddlWorks').attr('disabled', true);
                    }
                    else {

                        $("#dvNITRoadDetails").html(data);
                        $('#dvNewNITRoadDetails').show('slow');
                        $("#dvNITRoadDetails").show('slow');
                        $("#TEND_COST_FORM").focus();
                        $('#ddlRoads').attr('disabled', true);
                        $('#ddlWorks').attr('disabled', true);
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }
            })
        }
    });

    $("#btnCancelNITRoadDetails").click(function (e) {

        var encryptedTendNITCode = $('#EncryptedTendNITCode').val();
        $.ajax({
            url: "/NIT/NITRoadDetails/" + encryptedTendNITCode,
            type: "GET",
            dataType: "html",
            success: function (data) {

                $('#dvNITRoadDetails').html(data);
               // $('#dvNewSplitWorkDetails').show('slow');
                $('#dvNITRoadDetails').hide();
                $('#EncryptedTendNITCode').val(encryptedTendNITCode);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
            }

        });

    })

    //new change done by Vikram on 3 Feb 2014

    $('#imgCloseProgressDetails').click(function () {

        $("#dvNewNITRoadDetails").hide("slow");
        $("#divError").hide("slow");

    });

    //end of change


});

function FillInCascadeDropdown(map, dropdown, action) {

    var message = '';

    if (dropdown == '#ddlPackages') {
        message = '<h4><label style="font-weight:normal"> Loading Packages... </label></h4>';
    }
    else if (dropdown == '#ddlRoads') {
        message = '<h4><label style="font-weight:normal"> Loading Roads... </label></h4>';
    }

    $(dropdown).empty();
    //$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.blockUI({ message: message });
    $.post(action, map, function (data) {
        $.each(data, function () {
            $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
        });
    }, "json");
    $.unblockUI();
} //end FillInCascadeDropdown()

function LoadNITRoadDetails() {


    jQuery("#tbNITRoadList").jqGrid({
        url: '/NIT/GetNITRoadDetailsList',
        datatype: "json",
        mtype: "POST",
        postData: { TendNITCode: $('#EncryptedTendNITCode').val() },
        colNames: ['Road Name', 'Work Name', 'Receiving Bids Date',  'Tender Opening Date', 'Technical Bid Opening Date', 'Financial Bid Opening Date', 'Tender Form Cost', 'Total Estimated Cost', 'Total Maintenance Cost', 'View', 'Edit', 'Delete'],
        colModel: [
                           { name: 'RoadName', index: 'RoadName', width: 120, sortable: true },
                           { name: 'WorkName', index: 'WorkName', width: 80, sortable: true },
                           { name: 'ReceivingBidsDate', index: 'ReceivingBidsDate', height: 'auto', width: 100, sortable: true, },
                           { name: 'TenderOpeningDate', index: 'TenderOpeningDate', width: 100, sortable: true },
                           { name: 'TechnicalBidOpeningDate', index: 'TechnicalBidOpeningDate', width: 120, sortable: true },
                           { name: 'FinancialBidOpeningDate', index: 'FinancialBidOpeningDate', width: 120, sortable: true },
                           { name: 'TenderFormCost', index: 'TenderFormCost', height: 'auto', width: 100, sortable: true, align: "right" },
                           { name: 'TotalEstimatedCost', index: 'TotalEstimatedCost', height: 'auto', width: 100, sortable: true, align: "right" },
                           { name: 'TotalMaintenanceCost', index: 'TotalMaintenanceCost', height: 'auto', width: 110, sortable: true, align: "right" },
                            { name: 'View', index: 'View', width: 40, sortable: false, formatter: FormatColumnView, align: "center", resizable: false, hidden:true },
                           { name: 'Edit', index: 'Edit', width: 40, sortable: false, formatter: FormatColumnEdit, align: "center" },
                           { name: 'Delete', index: 'Edit', width: 40, sortable: false, align: "center", formatter: FormatColumnDelete }

        ],
        pager: jQuery('#dvNITRoadListPager'),
        rowNum: 5,
        rowList: [5, 10],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "NIT Road Details List",
        height: 'auto',
        //width: 1135,
        autowidth: true,
        rownumbers: true,
        sortname: 'RoadName',
        sortorder: "asc",
        hidegrid: false,
        loadComplete: function () {

            var reccount = $('#tbNITRoadList').getGridParam('reccount');
            if (reccount > 0) {
                $('#dvNITRoadListPager_left').html('[<b> Note</b>: 1.All Amounts are in Lakhs 2.All Lengths are in Kms ]');
            }
        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                // alert(xhr.responseText);
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }


    }); //end of grid
}


function FormatColumnEdit(cellvalue, options, rowObject) {
   //style='border-color:white'
    if (cellvalue != '') {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit Road Details' onClick ='EditRoadDetails(\"" + cellvalue.toString() + "\");'></span></td> </tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }

}

function FormatColumnView(cellvalue, options, rowObject) {

    if (cellvalue != '') {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-zoomin' title='View Road Details' onClick ='ViewRoadDetails(\"" + cellvalue.toString() + "\");'></span></td> </tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }

}


function FormatColumnDelete(cellvalue, options, rowObject) {

    if (cellvalue != '') {
        return "<center><table><tr><td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete Road Details' onClick ='DeleteRoadDetails(\"" + cellvalue.toString() + "\");'></span></td> </tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }

}

function EditRoadDetails(urlparameter) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        type: 'GET',
        url: '/NIT/EditNITRoadDetails/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {

            $("#dvNITRoadDetails").html(data);

            $('#dvNewNITRoadDetails').show('slow');
            $('#dvNITRoadDetails').show('slow');
            $('#TEND_COST_FORM').focus();
 
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }
    })
}

function ViewRoadDetails(urlparameter) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        type: 'GET',
        url: '/NIT/ViewNITRoadDetails/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {

            $("#dvViewNITRoadDetails").html(data);

            $("#dvViewNITRoadDetails").dialog('open');

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }
    })
}

function DeleteRoadDetails(urlparameter) {
    if (confirm("Are you sure you want to delete road details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'POST',
            url: '/NIT/DeleteNITRoadDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert(data.message);

                    $('#tbNITRoadList').trigger('reloadGrid');

                    if ($('#dvNewNITRoadDetails').is(':visible') && $('#EncryptedTendNITCode').val() != '') {
                        var encryptedTendNITCode = $('#EncryptedTendNITCode').val();
                        $("#dvNITRoadDetails").load("/NIT/NITRoadDetails/" + encryptedTendNITCode, function () {

                            $('#dvNewNITRoadDetails').show('slow');
                            $('#dvNITRoadDetails').show('slow');
                            $('#EncryptedTendNITCode').val(encryptedTendNITCode);

                        });
                    }

                    $.unblockUI();
                }
                else {
                    alert(data.message);
                    $.unblockUI();
                }

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }
        });
    }
    else {
        return false;
    }
}