jQuery.validator.addMethod("comparefieldvalidator", function (value, element, param) {

    var startChainage = parseFloat($('#IMS_START_CHAINAGE').val());
    var endChainage = parseFloat($('#IMS_END_CHAINAGE').val());
    if (startChainage >= endChainage)
        return false;
    else
        return true;
});

jQuery.validator.unobtrusive.adapters.addBool("comparefieldvalidator");


jQuery.validator.addMethod("customrequired", function (value, element, param) {

    if ($("#PMGSYScheme").val() == 2 || $("#PMGSYScheme").val() == 3)
    {
        
        if (($("#"+element.id).val() == ""))  {
            return false;
        }
        else {
            return true;
        }
    }
    else
        return true;
});

jQuery.validator.unobtrusive.adapters.addBool("customrequired");

$(document).ready(function () {


  //  $('#tblWorkCostDetails td').attr('style', 'text-align:center');

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $.validator.unobtrusive.parse($('#frmAddSplitWorkDetails'));


    LoadSplitWorkDetails();

    $('#btnSaveSplitWorkDetails').click(function (e) {


        if ($('#frmAddSplitWorkDetails').valid()) {

            var encryptedIMSPRCode = $('#EncryptedIMSPRRoadCode').val();

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                url: "/SplitWork/AddSplitWorkDetails",
                type: "POST",
                data: $("#frmAddSplitWorkDetails").serialize(),
                success: function (data) {

                    if (data.success == true) {

                        alert(data.message);
                       // $('#btnResetSplitWorkDetails').trigger('click');

                        $('#dvSplitWorkDetails').load('/SplitWork/SplitWorkDetails/' + encryptedIMSPRCode, function () {
                            $('#dvNewSplitWorkDetails').show('slow');
                            $('#EncryptedIMSPRRoadCode').val(encryptedIMSPRCode);                                          
                            //$('#dvSplitWorkDetails').hide();
                            $.unblockUI();
                        });

                        $('#tbSplitWorkList').jqGrid("setGridParam", { "postData": { IMSPRRoadCode: $('#EncryptedIMSPRRoadCode').val() } });
                        $('#tbSplitWorkList').trigger('reloadGrid', [{ page: 1 }]);

                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#ermessage').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $('#dvSplitWorkDetails').html(data);
                        $('#dvNewSplitWorkDetails').show('slow');
                        $('#dvSplitWorkDetails').show('slow');
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


    $('#btnResetSplitWorkDetails').click(function (e) {

        if ($("#dvErrorMessage").is(":visible")) {
            $('#dvErrorMessage').hide('slow');
            $('#ermessage').html('');
        }
        $('#tdPavLength').text('-');

    });


    $("#btnUpdateSplitWorkDetails").click(function (e) {


        if ($("#frmAddSplitWorkDetails").valid()) {
         
            var encryptedIMSPRCode = $('#EncryptedIMSPRRoadCode').val();

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                type: 'POST',
                url: '/SplitWork/EditSplitWorkDetails',
                async: false,
                data: $("#frmAddSplitWorkDetails").serialize(),
                success: function (data) {

                    if (data.success == true) {
                        alert(data.message);
                        $('#dvSplitWorkDetails').load('/SplitWork/SplitWorkDetails/' + encryptedIMSPRCode, function () {
                            $('#dvNewSplitWorkDetails').show('slow');                    
                            $('#EncryptedIMSPRRoadCode').val(encryptedIMSPRCode);
                            $('#dvSplitWorkDetails').hide();
                            $.unblockUI();
                        });

                        $('#tbSplitWorkList').trigger('reloadGrid');
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#ermessage').html(data.message);
                            $('#dvErrorMessage').show('slow');                       
                        }
                    }
                    else {

                        $("#dvSplitWorkDetails").html(data);
                        $('#dvNewSplitWorkDetails').show('slow');
                        $("#dvSplitWorkDetails").show('slow');
                        $("#IMS_WORK_DESC").focus();  
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

    $("#btnCancelSplitWorkDetails").click(function (e) {

        var encryptedIMSPRCode = $('#EncryptedIMSPRRoadCode').val();
        $.ajax({
            url: "/SplitWork/SplitWorkDetails/" + encryptedIMSPRCode,
            type: "GET",
            dataType: "html",
            success: function (data) {

                $('#dvSplitWorkDetails').html(data);
                $('#dvNewSplitWorkDetails').show('slow');
                $('#dvSplitWorkDetails').hide();
                $('#EncryptedIMSPRRoadCode').val(encryptedIMSPRCode);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
            }

        });

    })

    $('#IMS_END_CHAINAGE').change(function (e) {

        if ($('#IMS_START_CHAINAGE').val() != '' && $('#IMS_END_CHAINAGE').val() != '') {
            var startChainage = parseFloat($('#IMS_START_CHAINAGE').val());
            var endChainage = parseFloat($('#IMS_END_CHAINAGE').val());
            var pavLength = endChainage - startChainage;
            if (pavLength >= 0) {
                $('#tdPavLength').text(parseFloat(endChainage - startChainage).toFixed(3));
            }
            else {
                $('#tdPavLength').text('-');
            }
        }
        else {
            $('#tdPavLength').text('-');

        }
    });

    $('#IMS_START_CHAINAGE').change(function (e) {

        if ($('#IMS_END_CHAINAGE').val() != '') {
            var startChainage = parseFloat($('#IMS_START_CHAINAGE').val());
            var endChainage = parseFloat($('#IMS_END_CHAINAGE').val());
            var pavLength = endChainage - startChainage;
            if (pavLength >= 0) {
                $('#tdPavLength').text(endChainage - startChainage);
            }
            else {
                $('#tdPavLength').text('-');
            }
        }
        else {
            $('#tdPavLength').text('-');

        }
    });

    $("#IMS_FURNITURE_COST,#IMS_PAV_EST_COST,#IMS_CD_WORKS_EST_COST,#IMS_PROTECTION_WORKS,#IMS_OTHER_WORK_COST,#IMS_HIGHER_SPECIFICATION_COST").blur(function () {

        var pavCost = 0;
        var cdCost = 0;
        var otherworkCost = 0;
        var protectionCost = 0;
        var hsCost = 0;
        var fcCost = 0;
        var totalCost = 0;
        var totalCostInHS = 0;

        if(Number($("#IMS_PAV_EST_COST").val()) != NaN)
        {
            pavCost = Number($("#IMS_PAV_EST_COST").val());
        }

        if (Number($("#IMS_CD_WORKS_EST_COST").val()) != NaN) {
            cdCost = Number($("#IMS_CD_WORKS_EST_COST").val());
        }

        if (Number($("#IMS_PROTECTION_WORKS").val()) != NaN) {
            protectionCost = Number($("#IMS_PROTECTION_WORKS").val());
        }

        if (Number($("#IMS_OTHER_WORK_COST").val()) != NaN) {
            otherworkCost = Number($("#IMS_OTHER_WORK_COST").val());
        }

        if (Number($("#IMS_HIGHER_SPECIFICATION_COST").val()) != NaN) {
            hsCost = Number($("#IMS_HIGHER_SPECIFICATION_COST").val());
        }

        if (Number($("#IMS_FURNITURE_COST").val()) != NaN) {
            fcCost = Number($("#IMS_FURNITURE_COST").val());
        }
         
        if(Number(pavCost) != NaN && Number(cdCost) != NaN && Number(protectionCost) != NaN && Number(otherworkCost) != NaN && Number(fcCost) != NaN)
        {
            totalCost = pavCost + cdCost + protectionCost + otherworkCost + fcCost;
            //totalCost = Number(pavCost).toFixed(2) + Number(cdCost).toFixed(2) + Number(protectionCost).toFixed(2) + Number(otherworkCost).toFixed(2) + Number(fcCost).toFixed(2);
        }

        if (Number(pavCost) != NaN && Number(cdCost) != NaN && Number(protectionCost) != NaN && Number(otherworkCost) != NaN && Number(fcCost) != NaN && Number(hsCost) != NaN) {
            totalCostInHS = pavCost + cdCost + protectionCost + otherworkCost + fcCost + hsCost;
            //totalCostInHS = Number(pavCost).toFixed(2) + Number(cdCost).toFixed(2) + Number(protectionCost).toFixed(2) + Number(otherworkCost).toFixed(2) + Number(fcCost).toFixed(2) + Number(hsCost).toFixed(2);
        }
        
        //if ($("#SharePercent").val() == 1)
        //{
        //    $("#lblStateShare").html(Number(totalCost * 0.10).toFixed(2));
        //    $("#lblMoRDShare").html(Number(totalCost * 0.90).toFixed(2));
        //}
        //else if ($("#SharePercent").val() == 2)
        //{
        //    $("#lblStateShare").html(Number(totalCost * 0.25).toFixed(2));
        //    $("#lblMoRDShare").html(Number(totalCost * 0.75).toFixed(2));
        //}

        ///Changed by SAMMED A. PATIL on 14JUNE2017 to display Total State Share Cost
        if ($("#SharePercent").val() == 1) {
            $("#lblStateShare").html(Number(totalCost * 0.25).toFixed(2));
            $("#lblMoRDShare").html(Number(totalCost * 0.75).toFixed(2));
        }
        else if ($("#SharePercent").val() == 2) {
            $("#lblStateShare").html(Number(totalCost * 0.10).toFixed(2));
            $("#lblMoRDShare").html(Number(totalCost * 0.90).toFixed(2));
        }
        else if ($("#SharePercent").val() == 3) {
            $("#lblStateShare").html(Number(totalCost * 0.40).toFixed(2));
            $("#lblMoRDShare").html(Number(totalCost * 0.60).toFixed(2));
        }
        else if ($("#SharePercent").val() == 4) {
            $("#lblStateShare").html(Number(totalCost * 0.0).toFixed(2));
            $("#lblMoRDShare").html(Number(totalCost * 1.0).toFixed(2));
        }

        $("#lblTotalCost").html(Number(totalCostInHS).toFixed(2));
        $("#lblTotalCostExHS").html(Number(totalCost).toFixed(2));

    });

});

function LoadSplitWorkDetails() {


    jQuery("#tbSplitWorkList").jqGrid({
        url: '/SplitWork/GetSplitWorkDetailsList',
        datatype: "json",
        mtype: "POST",
        postData: { IMSPRRoadCode: $('#EncryptedIMSPRRoadCode').val() },
        colNames: ['Work Name', 'Start Chainage', 'End Chainage', 'Pavement Length', 'Pavement Cost', 'CD Works Cost', 'Protection Cost', 'Other Works Cost', 'State Share', 'Maintenance Cost', 'Edit', 'Delete'],
        colModel: [
                           { name: 'WorkName', index: 'WorkName', width: 130, sortable: true },
                           { name: 'StartChainage', index: 'StartChainage', height: 'auto', width: 100, sortable: true, },
                           { name: 'EndChainage', index: 'EndChainage', width: 100, sortable: true },
                           { name: 'PevementLength', index: 'PevementLength', width: 100, sortable: true },
                           { name: 'PevementCost', index: 'PevementCost', width: 100, sortable: true, align: "right" },
                           { name: 'CDWorksCost', index: 'CDWorksCost', height: 'auto', width: 100, sortable: true, align: "right" },
                           { name: 'ProtectionCost', index: 'ProtectionCost', height: 'auto', width: 100, sortable: true, align: "right" },
                           { name: 'OtherWorksCost', index: 'OtherWorksCost', height: 'auto', width: 100, sortable: true, align: "right" },
                           { name: 'StateShare', index: 'StateShare', height: 'auto', width: 100, sortable: true, align: "right" },
                           { name: 'MaintenanceCost', index: 'MaintenanceCost', height: 'auto', width: 100, sortable: false, align: "right" },
                            { name: 'Edit', index: 'Edit', width: 50, sortable: false, formatter: FormatColumnEdit, align: "center" },
                            { name: 'Delete', index: 'Edit', width: 50, sortable: false, align: "center", formatter: FormatColumnDelete }
                           
        ],
        pager: jQuery('#dvSplitWorkListPager'),
        rowNum: 5,
        rowList: [5, 10],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "Split Work Details List",
        height: 'auto',
        width: 1135,
        //autowidth: true,
        rownumbers: true,
        sortname: 'WorkName',
        sortorder: "asc",
        hidegrid: false,
        //footerrow: true,
        userDataOnFooter: true,
        loadComplete: function (data) {

            //var reccount = $('#tbSplitWorkList').getGridParam('reccount');
            //if (reccount > 0) {
            //    $('#dvSplitWorkListPager_left').html('[<b> Note</b>: 1.All Amounts are in Lakhs 2.All Lengths are in Kms ]');
            //}
            
            var reccount = $('#tbSplitWorkList').getGridParam('reccount');
        
            if (reccount > 1 && data.userdata.status == 'N') {

                var button = '<input type="button" id="btnFinalizeSplitWork" name="btnFinalizeSplitWork" value="Finalize" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only" title="Finalize Split Works" tabindex="100" style="font-size:1em; margin-left:25px" onclick="FinalizeSplitWork()" />'

                $('#dvSplitWorkListPager_left').html(button);
                $('#SplitCount').val(data.userdata.splitCount);
            }
            else if (reccount > 0 && data.userdata.status == 'Y') {
                $('#dvSplitWorkListPager_left').html('[<b> Note</b>: 1.All Amounts are in Lakhs 2.All Lengths are in Kms ]');
                $('#tblCreateNewSplitWork').hide();
                $('#tblCreateNewSplitWork').empty();
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

    if (cellvalue != '') {
        return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-pencil' title='Edit Split Work Details' onClick ='EditSplitWorkDetails(\"" + cellvalue.toString() + "\");'></span></td> </tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }

}

function FormatColumnDelete(cellvalue, options, rowObject) {

    if (cellvalue != '') {
        return "<center><table><tr><td style='border-color:white'><span class='ui-icon ui-icon-trash' title='Delete Split Work Details' onClick ='DeleteSplitWorkDetails(\"" + cellvalue.toString() + "\");'></span></td> </tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>";
    }

}


function EditSplitWorkDetails(urlparameter) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        type: 'GET',
        url: '/SplitWork/EditSplitWorkDetails/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {

            $("#dvSplitWorkDetails").html(data);

            $('#dvNewSplitWorkDetails').show('slow');
            $('#dvSplitWorkDetails').show('slow');
            $('#IMS_WORK_DESC').focus();
            $("#IMS_FURNITURE_COST,#IMS_PAV_EST_COST,#IMS_CD_WORKS_EST_COST,#IMS_PROTECTION_WORKS,#IMS_OTHER_WORK_COST,#IMS_HIGHER_SPECIFICATION_COST").trigger('blur');
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }
    })
}

function DeleteSplitWorkDetails(urlparameter) {
    if (confirm("Are you sure you want to delete split work details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'POST',
            url: '/SplitWork/DeleteSplitWorkDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert(data.message);

                    $('#tbSplitWorkList').trigger('reloadGrid');

                    if ($('#dvNewSplitWorkDetails').is(':visible') && $('#EncryptedIMSWorkCode').val() != '') {
                        var encryptedIMSPRCode = $('#EncryptedIMSPRRoadCode').val();
                        $("#dvSplitWorkDetails").load("/SplitWork/SplitWorkDetails/" + encryptedIMSPRCode, function () {

                            $('#dvNewSplitWorkDetails').show('slow');
                            $('#dvSplitWorkDetails').show('slow');
                            $('#EncryptedIMSPRRoadCode').val(encryptedIMSPRCode);

                        });
                    }

                    if (jQuery("#tbSplitWorkList").getGridParam('reccount') <= 1) {

                        $('#imgCloseAgreementDetails').trigger('click');
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

function FinalizeSplitWork() {
    var message = '';
    var recCount = $('#tbSplitWorkList').getGridParam('reccount');

    if (parseInt(recCount) == parseInt($('#SplitCount').val())) {

        message = "Are you sure you want to finalize split works?";

    }
    else {
        message = "The total split count does not match, Are you sure you want to finalize split works? ";
    }

    var urlparameter = $('#EncryptedIMSPRRoadCode').val();
    if (confirm(message)) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'POST',
            url: '/SplitWork/FinalizedSplitWorkDetails/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success==true) {
                    alert(data.message);

                    $('#tbSplitWorkList').trigger('reloadGrid');

                    if ($('#dvNewSplitWorkDetails').is(':visible') && $('#EncryptedIMSWorkCode').val() != '') {
                        var encryptedIMSPRCode = $('#EncryptedIMSPRRoadCode').val();
                        $("#dvSplitWorkDetails").load("/SplitWork/SplitWorkDetails/" + encryptedIMSPRCode, function () {

                            $('#dvNewSplitWorkDetails').show('slow');
                            $('#dvSplitWorkDetails').show('slow');
                            $('#EncryptedIMSPRRoadCode').val(encryptedIMSPRCode);

                        });
                    }
                    else {
                        $('#dvSplitWorkDetails').hide('slow');
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