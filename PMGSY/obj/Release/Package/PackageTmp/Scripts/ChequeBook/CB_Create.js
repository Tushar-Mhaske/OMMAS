var isGridLoaded = false;

$(document).ready(function () {
   // $("#ddlDPIU").trigger("change");
    $("#tblChequeBookList").jqGrid('GridUnload');
    LoadGrid(0, 0, 0, $("#ddlDPIU").val(), $("#ddlDPIU option:selected").text());
    //alert($("#ddlDPIU").val());
    $.validator.unobtrusive.parse("#frmMasterAgency");

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $("#ISSUE_DATE").addClass("pmgsy-textbox");

    $("#ISSUE_DATE").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        showOn: 'button',
        buttonImage: '/Content/images/calendar_2.png',
        buttonImageOnly: true,
        maxDate: 0,
        minDate: $("#ACC_OPEN_DATE").val() 
    }).attr('readonly', 'readonly');
 

   // LoadGrid(0, 0, 0, 0, "");
    $("#rdoSRRDA").click(function () {
      //  alert("SRRDA");
        $(".tdDPIU").hide('slow');
        $("#tblChequeBookList").jqGrid('GridUnload');
        LoadGrid(0, 0, 0, 0,"SRRDA");
      //  jQuery("#tblChequeBookList").jqGrid("setGridParam", { postData: { 'month': 0, 'year': 0, 'cheque': 0, 'AdminNdCode': 0, 'IsSRRDADpiu': "SRRDA" } });
    //    jQuery("#tblChequeBookList").trigger("reloadGrid");
       // LoadGrid(0, 0, 0, 0, "SRRDA");
    });
    $("#rdoDPIU").click(function () {
        // alert("DPIU");
        $("#tblChequeBookList").jqGrid('GridUnload');
        $(".tdDPIU").show('slow');
       LoadGrid(0, 0, 0, $("#ddlDPIU").val(), $("#ddlDPIU option:selected").text());
    });

    $(function () {
        $("#ddlDPIU").trigger("change");
    
        $("#ddlDPIU").change(function () {
          
            $("#dvErrorMessage").hide();
         //   $("#btnReset").trigger();

            $("#spnDPIUName").html("");
            $("#spnDPIUName").html($("#ddlDPIU option:selected").text());

            $("#tblChequeBookList").jqGrid('GridUnload');
         
            LoadGrid(0, 0, 0, $("#ddlDPIU").val(), $("#ddlDPIU option:selected").text());
          
        });
    });

    $("#ddlReasonType").append("<option value='0' selected>--select--</option>");

    $('#btnSave').click(function (e) {
        if ($('#frmMasterAgency').valid()) {

            var dpiuCode = $("#ddlDPIU option:selected").val();

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/ChequeBook/AddCB/",
                type: "POST",
          
                data: $("#frmMasterAgency").serialize(),
                success: function (data) {
                    if (data.success==true) {
                        alert(data.message);
                        
                        LoadGrid(0, 0, 0, $("#ddlDPIU").val(), $("#ddlDPIU option:selected").text());
                        //$('#tblMasterAgencyList').trigger('reloadGrid');
                        //$("#dvAgencyDetails").load("/Master/AddEditMasterAgency");
                        if ($("#dvCBDetails").is(":visible")) {
                            $('#dvCBDetails').hide('slow');

                            $('#btnSearchView').hide();
                            $('#btnAddCB').show();

                        }

                        if (!$("#dvSearchAgency").is(":visible")) {
                            $("#dvSearchAgency").show('slow');
                        }
                      //  SearchAgencyCreateDetail(dpiuCode);
                    }
                    else if (data.success==false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvCBDetails").html(data);
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    //alert(xhr.responseText);
                    alert('An Error occurred while processing your request');
                    $.unblockUI();
                }

            });

        }
    });



    $('#btnUpdate').click(function (e) {

    //    alert("In the Update Function")
        if ($('#frmMasterAgency').valid()) {
            var agencyCode = $("#ddlAgencyType option:selected").val();
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/ChequeBook/EditChequeBookDetails/",
                type: "POST",           
                data: $("#frmMasterAgency").serialize(),
                success: function (data) {

                    if (data.success==true) {
                        alert("Cheque Book Details are Updated Successfully.");
                        LoadGrid(0, 0, 0, $("#ddlDPIU").val(), $("#ddlDPIU option:selected").text());
                        
                        if ($("#dvCBDetails").is(":visible")) {
                            $('#dvCBDetails').hide('slow');
                            $('#dvSearchCB').hide();
                            $('#btnAddCB').show();
                        }

                        if (!$("#dvSearchCB").is(":visible")) {
                            $("#dvSearchCB").show('slow');
                        }
                        //SearchAgencyCreateDetail(agencyCode);
                    }
                    else if (data.success == false) {
                        alert("Cheque Book Details are not Updated.");
                        if (data.message != "") {


                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvCBDetails").html(data);
                    }

                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    //alert(xhr.responseText);
                    alert('An Error occurred while processing your request');
                    $.unblockUI();
                }

            });
        }
    });

    $('#btnCancel').click(function (e) {

        //$.ajax({
        //    url: "/ChequeBook/AddEditCB",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {
        //        $("#dvAgencyDetails").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }
        //});
        if ($("#dvCBDetails").is(":visible")) {
            $('#dvCBDetails').hide('slow');

            $('#dvSearchCB').hide();
            $('#btnAddCB').show();

        }

        if (!$("#dvSearchCB").is(":visible")) {
            $("#dvSearchCB").show('slow');
        }
      
    });

    $("#btnReset").click(function () {
      
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');

    });

    //$("#dvhdCreateNewAgencyDetails").click(function () {

    //    if ($("#dvCreateNewAgencyDetails").is(":visible"))
    //    {

    //        $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

    //        $(this).next("#dvCreateNewAgencyDetails").slideToggle(300);
    //    }

    //    else
    //    {
    //        $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

    //        $(this).next("#dvCreateNewAgencyDetails").slideToggle(300);
    //    }
    //});

    //$("#MAST_AGENCY_NAME").focus(function () {
    //    $('#dvErrorMessage').hide('slow');
    //    $('#message').html('');
    //});

    //$("#MAST_AGENCY_TYPE").focus(function () {
    //    $('#dvErrorMessage').hide('slow');
    //    $('#message').html('');
    //});
});

//function SearchAgencyCreateDetail(agencyCode) {
//    $('#AgencyType').val(agencyCode);
//       $('#tblMasterAgencyList').setGridParam({
//        url: '/Master/GetMasterAgencyList', datatype: 'json'
//    });
//    $('#tblMasterAgencyList').jqGrid("setGridParam", { "postData": { AgencyType: agencyCode } });
//    $('#tblMasterAgencyList').trigger("reloadGrid", [{ page: 1 }]);
//}