$(document).ready(function () {

    //this should be added on document ready
    $.validator.unobtrusive.parse($('#frmCreateMLAConstituency'));

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });



    $('#btnSave').click(function (e) {
        //e.preventDefault();


        if ($('#frmCreateMLAConstituency').valid()) {

            var stateCode = $('#ddlStates option:selected').val();
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/LocationMasterDataEntry/CreateMLAConstituency",
                type: "POST",
               // dataType: "json",
                data: $("#frmCreateMLAConstituency").serialize(),
                success: function (data) {

                    if (data.success==true) {
                        alert(data.message);
                        //ClearDetails();
                        if ($("#dvMapMLAConstituencyBlockDetails").is(":visible")) {
                            $('#dvMapMLAConstituencyBlockDetails').hide('slow');
                        }
                        if ($("#dvMappedMLAConstituencyBlockDetails").is(":visible")) {
                            $('#dvMappedMLAConstituencyBlockDetails').hide('slow');
                        }
                        $('#dvMLAConstituencyDetails').hide('slow');
                        $('#btnSearchView').hide();
                        $('#btnCreateNew').show();
                        if (!$("#dvSearchMLAConstituency").is(":visible")) {
                            $("#dvSearchMLAConstituency").show('slow');
                        }
                        SearchMLACreateDetail(stateCode);
                    }
                    else if (data.success==false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }

                    }
                    else {
                        $("#dvMLAConstituencyDetails").html(data);
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


    $('#btnUpdate').click(function (e) {

        e.preventDefault();

        if ($('#frmCreateMLAConstituency').valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $("#ddlStates").attr("disabled", false);
            var stateCode = $('#ddlStates option:selected').val();
            $.ajax({
                url: "/LocationMasterDataEntry/EditMLAConstituency",
                type: "POST",
              //  dataType: "json",
                data: $("#frmCreateMLAConstituency").serialize(),
                success: function (data) {
                    

                    if (data.success==true) {
                        alert(data.message);
                        //SearchMLACreateDetail();
                        //$('#tbMLAConstituencyList').trigger('reloadGrid');
                        //$('#dvMLAConstituencyDetails').load("/LocationMasterDataEntry/CreateMLAConstituency");
                        if ($("#dvMapMLAConstituencyBlockDetails").is(":visible")) {
                            $('#dvMapMLAConstituencyBlockDetails').hide('slow');
                        }
                        if ($("#dvMappedMLAConstituencyBlockDetails").is(":visible")) {
                            $('#dvMappedMLAConstituencyBlockDetails').hide('slow');
                        }
                        $('#dvMLAConstituencyDetails').hide('slow');
                        $('#btnSearchView').hide();
                        $('#btnCreateNew').show();
                        if (!$("#dvSearchMLAConstituency").is(":visible")) {
                            $("#dvSearchMLAConstituency").show('slow');
                        }
                        SearchMLACreateDetail(stateCode);

                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $("#ddlStates").attr("disabled", true);
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvMLAConstituencyDetails").html(data);
                        $("#ddlStates").attr("disabled", true);
                    }

                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {

                    $("#ddlStates").attr("disabled", true);
                    alert(xhr.responseText);
                    $.unblockUI();
                }

            });
        }
    });

    $('#btnCancel').click(function (e) {

        //$.ajax({
        //    url: "/LocationMasterDataEntry/CreateMLAConstituency",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {
        //        //$("#mainDiv").html(data);
        //        $("#dvMLAConstituencyDetails").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }

        //});
        if ($("#dvMapMLAConstituencyBlockDetails").is(":visible")) {
            $('#dvMapMLAConstituencyBlockDetails').hide('slow');
        }
        if ($("#dvMappedMLAConstituencyBlockDetails").is(":visible")) {
            $('#dvMappedMLAConstituencyBlockDetails').hide('slow');
        }
        $('#dvMLAConstituencyDetails').hide('slow');
        $('#btnSearchView').hide();
        $('#btnCreateNew').show();
        if (!$("#dvSearchMLAConstituency").is(":visible")) {
            $("#dvSearchMLAConstituency").show('slow');
        }
    });


    $('#btnReset').click(function () {
        ClearDetails();
    });


    //for expand and collpase Document Details 
    $("#dvhdCreateNewMLAConstituencyDetails").click(function () {

        if ($("#dvCreateNewMLAConstituencyDetails").is(":visible")) {

            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            $(this).next("#dvCreateNewMLAConstituencyDetails").slideToggle(300);
        }

        else {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvCreateNewMLAConstituencyDetails").slideToggle(300);
        }
    });

    $("#ddlStates").change(function () {

        if ($("#dvErrorMessage").is(":visible")) {
            $('#dvErrorMessage').hide('slow');
            $('#message').html('');
        }

    });

    $('#MAST_MLA_CONST_NAME').change(function () {

        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });
});

function ClearDetails() {
    
    $('#MAST_MLA_CONST_NAME').val(''); 
    $('#ddlStates').val('0'); 
    $('#dvErrorMessage').hide('slow');
    $('#message').html('');
}

function SearchMLACreateDetail(stateCode) {
    $('#ddlSearchStates').val(stateCode);
    $('#tbMLAConstituencyList').setGridParam({
        url: '/LocationMasterDataEntry/GetMLAConstituencyDetailsList'
    });
   /* var data = $('#tbMLAConstituencyList').jqGrid("getGridParam", "postData");
    data._search = false;
    data.searchField = "";*/
    // $('#tbDistrictList').jqGrid("setGridParam", { "postData": data });

    $('#tbMLAConstituencyList').jqGrid("setGridParam", { "postData": { stateCode: $('#ddlSearchStates option:selected').val() } });
    $('#tbMLAConstituencyList').trigger("reloadGrid", [{ page: 1 }]);

}