
jQuery.validator.addMethod("pantanfieldvalidator", function (value, element, param) {

    if ($("#radioContractor").is(":checked")) {
        ObjVal = $("#MAST_CON_PAN").val();
        var panPat = /^([a-zA-Z]{5})(\d{4})([a-zA-Z]{1})$/;
     //   var code = /([C,P,H,F,A,T,B,L,J,G])/;
		 // added on 22-12-2021 by Srishti Tyagi
        var code = /([C,P,H,F,A,T,B,L,J,G,c,p,h,f,a,t,b,l,j,g])/;
        var code_chk = ObjVal.substring(3, 4);
        $('#SearchPanNumeberOfContr').hide();
        if (ObjVal.search(panPat) == -1) {

            return false;
        }
        if (code.test(code_chk) == false) {
            return false;
        }
        if ($('#EncryptedContractorCode').val() === '') {
            $('#SearchPanNumeberOfContr').show();
        }
        else {
            $('#SearchPanNumeberOfContr').hide();
        }
        return true;
    } else if ($("#radioSupplier").is(":checked")) {
        ObjVal = $("#MAST_CON_PAN").val();
      //  var panPat = /^([a-zA-Z]{4})(\d{5})([a-zA-Z]{1})$/;
	  
		// added on 22-12-2021 by Srishti Tyagi
		var tan = /^([a-zA-Z]{4})(\d{5})([a-zA-Z]{1})$/;
		
        // for validations of PAN on supplier side
        var pan = /^([a-zA-Z]{5})(\d{4})([a-zA-Z]{1})$/;
        var code = /([C,P,H,F,A,T,B,L,J,G,c,p,h,f,a,t,b,l,j,g])/;
        var code_chk = ObjVal.substring(3, 4);
		
        $('#SearchPanNumeberOfContr').hide();
      //  if (ObjVal.search(panPat) == -1) {
      //      return false;
      //  }
		
		// added on 22-12-2021 by Srishti Tyagi
        if ((ObjVal.search(tan) == -1))    // check if TAN number is not valid
        {
            if ((ObjVal.search(pan) != -1) && (code.test(code_chk) == true))   // check if PAN number not valid
            {
                if ($('#EncryptedContractorCode').val() === '')   // condition for link of check PAN
                {
                    $("#spnSearchPanNumeberOfContr").text('Check PAN');
                    $('#SearchPanNumeberOfContr').show();
                }
                else
                {
                    $('#SearchPanNumeberOfContr').hide();
                }
                return true;
                
            }
            else if ((ObjVal.search(pan) != -1) && (code.test(code_chk) == false))   // check if PAN number is not valid
            {
                return false;
            }
            else if ((ObjVal.search(pan) == -1))      // check if PAN number is not valid
            {
                return false;
            }
            
            return false; 
        }
		
        //for Check Pan Exist or not link
        if ($('#EncryptedContractorCode').val() === '') {
			// added on 23-12-2021 by Srishti tyagi
			$("#spnSearchPanNumeberOfContr").text('Check TAN');
            $('#SearchPanNumeberOfContr').show();
        }
        else {
            $('#SearchPanNumeberOfContr').hide();
        }
        return true;
    }
});

jQuery.validator.unobtrusive.adapters.addBool("pantanfieldvalidator");

$(document).ready(function () {

   
    //alert($('#EncryptedContractorCode').val());
    $('#SearchPanNumeberOfContr').hide();
    $.validator.unobtrusive.parse('#frmAdd');

    var firstLoading = "false";
    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $("#chkAddress").change(function (e) {
        if (($("#chkAddress").is(':checked'))) {
            var text = $("#add1").val();
            $("#add2").val(text);
        }
        else {
            $("#add2").val("");
        }
    });

    $('#MAST_CON_EXPIRY_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a from date',
        buttonImageOnly: true,
        alt: "Select Date",
        title: "Expiry Date",
        onSelect: function (selectedDate) {

        }
    });


    $("#State").change(function () {


        FillInCascadeDropdown({ userType: $("#State").find(":selected").val() },
                    "#MAST_DISTRICT_CODE_ADDR", "/Master/GetDistrictsList?stateCode=" + $('#State option:selected').val());
    });


    $("#btnList").click(function () {

        $("#dvMasterDataEntry").load("/Master/ListContractor");

    });

    //$('#btnList').click(function (e) {
    //    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    //    if ($("#dvMasterDataEntry").is(":visible")) {
    //        $('#dvMasterDataEntry').hide('slow');

    //        $('#btnList').hide();
    //        $('#btnCreateNew').show();

    //    }

    //    if (!$("#searchContractor").is(":visible")) {

    //        $('#searchContractor').load('/Master/ListContractor/', function () {

    //            $('#Contractor').trigger('reloadGrid');

    //            var data = $('#Contractor').jqGrid("getGridParam", "postData");
    //            if (!(data === undefined)) {

    //                $('#State').val(data.State);
    //                $('#ContractorSupplier').val(data.ContractorSupplier);
    //                $('#Status').val(Status.AgencyType);
    //                $('#PAN').val(PAN.PAN);
    //                $('#Contractor').val(data.Contractor);
    //            }
    //            $('#searchContractor').show('slow');

    //        });
    //    }
    //    $.unblockUI();
    //});

    $("#btnSave").click(function (e) {

        //Add by vikky 11-01-2022
        $(".panNumber").val(function () {
            return this.value.toUpperCase();
        })
        //end here by vikky 11-01-2022
        if ($("#frmAdd").valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                type: 'POST',
                url: '/Master/AddContractor/',
                async: false,
                data: $("#frmAdd").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert("Contractor/Supplier details saved successfully.");
                        ClearDetails();
                        $("#dvMasterDataEntry").load("/Master/ListContractor");

                        $.unblockUI();
                    }
                    else if (data.success == false) {

                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                            $.unblockUI();
                        }

                    }
                    else {
                        $("#dvMasterDataEntry").html(data);
                        //if ($("#radioSupplier").val() == "S") {

                        //    //alert($("#radioSupplier").val());
                        //    $("#lblPan").text("TAN");
                        //    $("Pan_Tan ").val("T");
                        //}
                        //else if ($("#radioContractor").val() == "C") {
                        //    //alert($("#radioContractor").val());
                        //    $("#lblPan").text("PAN");
                        //    $("Pan_Tan ").val("P");
                        //}
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

    $("#radioContractor").click(function () {

        //$("#lblPan").text("PAN");
        $("#lblPan").text("PAN/TAN");
        $("Pan_Tan ").val("P");
        $("#MAST_CON_PAN").attr('placeholder', 'Enter PAN/TAN Number...');
        $("#MAST_CON_PAN").attr('title', 'Enter PAN/TAN Number');

        //$('#lblPan').text('PAN');
        $("#spnSearchPanNumeberOfContr").text('Check PAN');
    });

    $("#radioSupplier").click(function () {
        //$("#lblPan").text("TAN");
        $("#lblPan").text("PAN/TAN");
        $("Pan_Tan ").val("T");
        $("#MAST_CON_PAN").attr('placeholder', 'Enter PAN/TAN Number...');
        $("#MAST_CON_PAN").attr('title', 'Enter PAN/TAN Number');

        //$('#lblPan').text('TAN');
        $("#spnSearchPanNumeberOfContr").text('Check TAN');
    });

    $(function () {

        if ($("#radioSupplier").is(":checked")) {
            $("#lblPan").text("PAN/TAN");
            $("Pan_Tan ").val("T");
            $("#MAST_CON_PAN").attr('placeholder', 'Enter PAN/TAN Number...');
            $("#MAST_CON_PAN").attr('title', 'Enter PAN/TAN Number');

        }
        else {
            $("#lblPan").text("PAN/TAN");
            $("Pan_Tan ").val("P");
            $("#MAST_CON_PAN").attr('placeholder', 'Enter PAN/TAN Number...');
            $("#MAST_CON_PAN").attr('title', 'Enter PAN/TAN Number');
        }

    });

    $("#btnReset").click(function (e) {

        ClearDetails();
    });

    $("#btnCancel").click(function (e) {

        $("#dvMasterDataEntry").load("/Master/ListContractor");
    })


    $("#chkAddress").change(function () {
        if ($("#chkAddress").is(":checked")) {
            var address1 = $("#add1").val();
            if (address1 == "") {
                alert("Please enter address1");
                $("#chkAddress").attr("checked", false);
                return false;
            }
            else {
                $("#add2").val(address1);
                alert("Your Address1 and Address2 is same.");
            }
        }
        else {
            $("#add2").val();
        }
    });

    $("#btnUpdate").click(function (e) {
        //Added on 03-10-2022
        if ($('#flag_P').val() == "False" && $('#flag_OD').val() == "True") {

            $('#radioContractor').attr("disabled", false);
            $('#radioSupplier').attr("disabled", false);
            $('#MAST_CON_FNAME').attr("disabled", false);
            $('#MAST_CON_MNAME').attr("disabled", false);
            $('#MAST_CON_LNAME').attr("disabled", false);
            $('#MAST_CON_EXPIRY_DATE').attr("disabled", false);
            $('#MAST_CON_COMPANY_NAME').attr("disabled", false);
            $('#add1').attr("disabled", false);
            $('#add2').attr("disabled", false);
            //$('#MAST_DISTRICT_CODE_ADDR').attr("disabled", false);
            $('#MAST_CON_PIN').attr("disabled", false);
            $('#rdoActive').attr("disabled", false);
            $('#rdoInActive').attr("disabled", false);
            $('#rdoExpired').attr("disabled", false);
            $('#rdoBlacklisted').attr("disabled", false);

        } else if ($('#flag_P').val() == "True" && $('#flag_OD').val() == "False") {

            $('#MAST_CON_PAN').attr("disabled", false);
        }
        //Add by vikky 11-01-2022
        $(".panNumber").val(function () {
            return this.value.toUpperCase();
        })
        //end here by vikky 11-01-2022
        $("#MAST_DISTRICT_CODE_ADDR").attr("disabled", false);
        $("#State").attr("disabled", false);

        if ($("#frmAdd").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                type: 'POST',
                url: '/Master/EditContractor/',
                async: false,
                data: $("#frmAdd").serialize(),
                success: function (data) {
                    if (data.success == true) {
                        alert(data.message);
                        ClearDetails();
                        $("#dvMasterDataEntry").load("/Master/ListContractor");
                        $.unblockUI();
                    }
                    else if (data.success == false) {
                        $("#MAST_DISTRICT_CODE_ADDR").attr("disabled", true);
                        $("#State").attr("disabled", true);

                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvMasterDataEntry").html(data);
                        $("#MAST_DISTRICT_CODE_ADDR").attr("disabled", true);
                        $("#State").attr("disabled", true);

                        if ($("#radioSupplier").val() == "S") {
                            $("#lblPan").text("TAN");
                            $("Pan_Tan ").val("T");
                        }
                        else if ($("#radioContractor").val() == "C") {
                            $("#lblPan").text("PAN");
                            $("Pan_Tan ").val("P");
                        }
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }
            })
        }
        else {
            $("#MAST_DISTRICT_CODE_ADDR").attr("disabled", true);
            $("#State").attr("disabled", true);
        }
    });

    $("#MAST_CON_ADDR1").focus(function () {
        ClearMessage();
    });

    $("#MAST_CON_ADDR1").focus(function () {
        ClearMessage();
    });

    $("#MAST_CON_COMPANY_NAME").focus(function () {
        ClearMessage();
    });

    $("#MAST_CON_EMAIL").focus(function () {
        ClearMessage();
    });

    $("#MAST_CON_EXPIRY_DATE").focus(function () {
        ClearMessage();
    });

    $("#MAST_CON_FAX").focus(function () {
        ClearMessage();
    });

    $("#MAST_CON_FNAME").focus(function () {
        ClearMessage();
    });

    $("#MAST_CON_LEGAL_HEIR_FNAME").focus(function () {
        ClearMessage();
    });

    $("#MAST_CON_LEGAL_HEIR_MNAME").focus(function () {
        ClearMessage();
    });

    $("#MAST_CON_LEGAL_HEIR_LNAME").focus(function () {
        ClearMessage();
    });

    $("#MAST_CON_MNAME").focus(function () {
        ClearMessage();
    });

    $("#MAST_CON_LNAME").focus(function () {
        ClearMessage();
    });

    $("#MAST_CON_MOBILE").focus(function () {
        ClearMessage();
    });

    $("#MAST_CON_PAN").focus(function () {
        ClearMessage();
    });

    $("#MAST_CON_PHONE1").focus(function () {
        ClearMessage();
    });

    $("#MAST_CON_PHONE2").focus(function () {
        ClearMessage();
    });

    $("#MAST_CON_PIN").focus(function () {
        ClearMessage();
    });

    $("#MAST_CON_REMARKS").focus(function () {
        ClearMessage();
    });

    $("#MAST_CON_STD_FAX").focus(function () {
        ClearMessage();
    });

    $("#MAST_CON_STD1").focus(function () {
        ClearMessage();
    });

    $("#MAST_CON_STD2").focus(function () {
        ClearMessage();
    });

    $("#MAST_CON_PAN").blur(function () {

        if ($("#MAST_CON_PAN").val() === '') {
            $('#SearchPanNumeberOfContr').hide();
        }
    });

    if ($('#EncryptedContractorCode').val() != null) {
        modifyContractorDetails();
    }

});

function ClearMessage() {
    $('#dvErrorMessage').hide('slow');
    $('#message').html('');
}

function ClearDetails() {
    $('#add1').val('');
    $('#add2').val('');
    $('#MAST_CON_COMPANY_NAME').val('');
    $('#MAST_CON_EMAIL').val('');
    $('#MAST_CON_EXPIRY_DATE').val('');
    $('#MAST_CON_FAX').val('');
    $('#MAST_CON_FNAME').val('');
    $('#MAST_CON_LEGAL_HEIR_FNAME').val('');
    $('#MAST_CON_LEGAL_HEIR_MNAME').val('');
    $('#MAST_CON_LEGAL_HEIR_LNAME').val('');
    $('#MAST_CON_LNAME').val('');
    $('#MAST_CON_MNAME').val('');
    $('#MAST_CON_MOBILE').val('');
    $('#MAST_CON_PAN').val('');
    $('#MAST_CON_PHONE1').val('');
    $('#MAST_CON_PHONE2').val('');
    $('#MAST_CON_PIN').val('');
    $('#MAST_CON_REMARKS').val('');
    $('#MAST_CON_STATUS').val('');
    $('#MAST_CON_STD_FAX').val('');
    $('#MAST_CON_STD1').val('');
    $('#MAST_CON_STD2').val('');
    $('#MAST_CON_SUP_FLAG').val('');
    $('#dvErrorMessage').hide('slow');
    $('#message').html('');
}



var fixPositionsOfFrozenDivs = function () {
    if (typeof this.grid.fbDiv !== "undefined") {
        $(this.grid.fbDiv).css($(this.grid.bDiv).position());
    }
    if (typeof this.grid.fhDiv !== "undefined") {
        $(this.grid.fhDiv).css($(this.grid.hDiv).position());
    }
};


function FillInCascadeDropdown(map, dropdown, action) {

    var message = '';

    if (dropdown == '#MAST_DISTRICT_CODE_ADDR') {
        message = '<h4><label style="font-weight:normal"> Loading Districts... </label></h4>';
    }

    $(dropdown).empty();
    $.blockUI({ message: message });
    $.post(action, map, function (data) {
        $.each(data, function () {
            $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
        });
    }, "json");
    $.unblockUI();
}


function SearchPanNumeberOfContractor() {

    if (!$("#MAST_CON_PAN").val() == '') {
        var strPannumber = $("#MAST_CON_PAN").val();
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            type: 'POST',
            url: '/Master/SearchPanNumeberOfContractor/',
            dataType: 'json',
            data: { "strPannumber": strPannumber },
            async: false,
            cache: false,
            success: function (jsonData) {
                if (jsonData.success) {

                    alert(jsonData.message);

                    $.unblockUI();
                }
                else {

                    // alert(data.message);
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
        alert("Please enter PAN to check it exist or not");
    }

}

//Below function added on 03-10-2022
function modifyContractorDetails() {
    //alert($('#flag_P').val() + " " + $('#flag_OD').val());

    if ($('#flag_P').val() == "False" && $('#flag_OD').val() == "True") {

        $('#radioContractor').attr("disabled", true);
        $('#radioSupplier').attr("disabled", true);
        $('#MAST_CON_FNAME').attr("disabled", true);
        $('#MAST_CON_MNAME').attr("disabled", true);
        $('#MAST_CON_LNAME').attr("disabled", true);
        $('#MAST_CON_EXPIRY_DATE').attr("disabled", true);
        $('#MAST_CON_COMPANY_NAME').attr("disabled", true);
        $('#add1').attr("disabled", true);
        $('#add2').attr("disabled", true);
        $('#MAST_DISTRICT_CODE_ADDR').attr("disabled", true);
        $('#MAST_CON_PIN').attr("disabled", true);
        $('#rdoActive').attr("disabled", true);
        $('#rdoInActive').attr("disabled", true);
        $('#rdoExpired').attr("disabled", true);
        $('#rdoBlacklisted').attr("disabled", true);

    } else if ($('#flag_P').val() == "True" && $('#flag_OD').val() == "False") {

        $('#MAST_CON_PAN').attr("disabled", true);
    }
}
