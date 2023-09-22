var isGridLoaded = false;
$(document).ready(function () {
  
    $("input[type=text]").bind("keypress", function (e)
    {
        if (e.keyCode == 13) {
            return false;
        }
    });

    LoadGrid(0, 0, 0, 0, "");

    $("#rdoSearchSRRDA").click(function () {
        $("#trSearchDPIU").hide("slow");
        
    });
    $("#rdoSearchDPIU").click(function () {
        $("#trSearchDPIU").show("slow");
      
    });

    $('#btnAddCB').click(function (e) {
    if ($("#dvSearchCB").is(":visible"))
        {
            $('#dvSearchCB').hide('slow');
        }

        if (!$("#dvCBDetails").is(":visible")) {

            $("#dvCBDetails").load("/ChequeBook/AddEditCB/");

            $('#dvCBDetails').show('slow');

            $('#btnAddCB').hide();
            $('#btnSearchView').show();
     
        }

    });


    // Button Click on Search
    $("#btnSearch").click(function ()
    {
        if (isGridLoaded)
        {
            $("#tblChequeBookList").GridUnload();

            if ($("#rdoSearchSRRDA").is(":checked"))
            {
              //  LoadGrid($("#ddlMonth").val(), $("#ddlYear").val(), $("#txtChequeNo").val(), $("#ddlSearchDPIU").val(), "SRRDA");
                LoadGrid($("#ddlMonth").val(), $("#ddlYear").val(), $("#txtChequeNo").val(), $("#ddlSearchDPIU").val(), "S");

            } else
            {
               // LoadGrid($("#ddlMonth").val(), $("#ddlYear").val(), $("#txtChequeNo").val(), $("#ddlSearchDPIU").val(), $("#ddlSearchDPIU option:selected").text());
                LoadGrid($("#ddlMonth").val(), $("#ddlYear").val(), $("#txtChequeNo").val(), $("#ddlSearchDPIU").val(), "D");
            }
        }
        else
        {
            if ($("#rdoSearchSRRDA").is(":checked"))
            {
              //  LoadGrid($("#ddlMonth").val(), $("#ddlYear").val(), $("#txtChequeNo").val(), $("#ddlSearchDPIU").val(), "SRRDA");
                LoadGrid($("#ddlMonth").val(), $("#ddlYear").val(), $("#txtChequeNo").val(), $("#ddlSearchDPIU").val(), "S");

            } else
            {
               // LoadGrid($("#ddlMonth").val(), $("#ddlYear").val(), $("#txtChequeNo").val(), $("#ddlSearchDPIU").val(), $("#ddlSearchDPIU option:selected").text());
                LoadGrid($("#ddlMonth").val(), $("#ddlYear").val(), $("#txtChequeNo").val(), $("#ddlSearchDPIU").val(), "D");
            }
        }
      
    });

    $("#btnCancel").click(function ()
    {
        $('#dvSearchCB').hide('slow');
       // $('#dvCBDetails').show('slow');
        $('#btnSearchView').show();
    });



    // Search 
    $('#btnSearchView').click(function (e) {
    
          $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

          if ($("#dvCBDetails").is(":visible"))
          {
           
              $('#dvCBDetails').hide('slow');
              $('#btnSearchView').hide();
              $('#btnAddCB').show();
          }

          if (!$("#dvSearchCB").is(":visible")) {
                $('#dvSearchCB').load('/ChequeBook/SearchCB/', function () {
                 $('#tblMasterAgencyList').trigger('reloadGrid');

                var data = $('#tblCBList').jqGrid("getGridParam", "postData");
                 if (!(data === undefined)) {

                    $('#AgencyType').val(data.AgencyType);
                }
                 $('#dvSearchCB').show('slow');

            });
        }
$.unblockUI();
   });

});

function FormatColumn(cellvalue, options, rowObject) {
    if (cellvalue.toString() == "") {
 
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked');'></span></td><td style='border:none'><span class='ui-icon ui-icon-locked');'></span></td></tr></table></center>";
    }
    return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit Cheque Book Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete Cheque Book Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}

function EditChequeBook(id) {
   // alert("In Edit of master cheque book details")
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        url: "/ChequeBook/EditChequeBookDetails/" + id,
        type: "GET",
        async: false,
        dataType: "html",
        catche: false,
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if ($("#dvSearchCB").is(":visible")) {
                $('#dvSearchCB').hide('slow');
            }
            $('#btnAddCB').hide();
            $('#btnSearchView').show();
 
            $("#dvCBDetails").html(data);
            $("#dvCBDetails").show();
            $("#MAST_AGENCY_NAME").focus(); //////////////////////////////////////////////////
        },
        error: function (xht, ajaxOptions, throwError) {
            //alert(xht.responseText);
            alert('An Error occurred while processing your request');
          
        }
        
    });
    $.unblockUI();
}

function DeleteChequeBook(urlParam) {
    if (confirm("Are you sure you want to delete Cheque Book details?")) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: "/ChequeBook/DeleteCB/" + urlParam,
            type: "POST",
            dataType: "json",
            //beforeSend: function (xhr) {   //to be change for AjaxSetup
            //    blockPage();
            //    xhr.setRequestHeader(CSRFHeader, CSRFToken);
            //},
            success: function (data) {
                $.unblockUI();
                if (data.success) {
                   // alert(data.message);
                    alert("Check Book Details deleted Successfully.")
                    $('#tblCBList').trigger('reloadGrid');
                }
                else {
                    // alert(data.message);
                    alert("Cannot Delete. Cheques are Issued from the Cheque Book")
                    $.unblockUI();
                }
                $("#dvCBDetails").load("/ChequeBook/AddEditCB/");
            },
            error: function (xht, ajaxOptions, throwError)
            {
                //alert(xht.responseText); $.unblockUI();
                alert('An Error occurred while processing your request');
            }

        });
    }
    else {
        return false;
    }
}

function LoadGrid(month, year, cheque, adminNdCode, dpiuName) {
    if (cheque == "") {
        cheque = 0;
    }
    if (isGridLoaded == true) {
        $("#tblChequeBookList").GridUnload();
        $("#tblChequeBookList").jqGrid('GridUnload'); // from pradip
    }
    var IsSRRDADpiu;
     if ($("#tblSearch").is(":visible")) {
        IsSRRDADpiu = ($("#rdoSearchDPIU").is(":checked") ? $("#rdoSearchDPIU").val() : $("#rdoSearchSRRDA").val())
    }
    else if ($("#frmChequeBookDetails").is(":visible")) {
        IsSRRDADpiu = ($("#rdoDPIU").is(":checked") ? $("#rdoDPIU").val() : $("#rdoSRRDA").val())
    }
    $("#tblChequeBookList").jqGrid('GridUnload'); // from pradip
   // alert('Go in Grid')
   jQuery("#tblChequeBookList").jqGrid({
        url: '/ChequeBook/GetCBList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Cheque Book Received Date', 'Start Number', 'End Number', 'Edit', 'Delete'],
        colModel: [
                            { name: 'ISSUE_DATE', index: 'ISSUE_DATE', width: 180, align: 'center', sortable: true },
                            { name: 'LEAF_START', index: 'LEAF_START', width: 100, align: 'center', sortable: true },
                            { name: 'LEAF_END', index: 'LEAF_END', width: 100, align: 'center', sortable: true },
                            { name: 'Edit', index: 'Edit', width: 100, align: 'center', sortable: false, hidden: false },
                            { name: 'Delete', index: 'Delete', width: 100, align: 'center', sortable: false, hidden: false }
                  
        ],
         pager: jQuery('#divChequeBookListPager'),
         rowNum: 15,
        postData: {
            'month': month,
            'year': year,
            'cheque': cheque,
            'AdminNdCode': adminNdCode,
            'IsSRRDADpiu': dpiuName
        },
        altRows: true,
        rowList: [10, 20, 50],
        viewrecords: true,
        recordtext: '{2} records found',
        //sortname: 'ISSUE_DATE',
        //sortorder: "desc",
        caption: "Cheque Book Details - " + dpiuName,
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        loadComplete: function () {
            isGridLoaded = true;
            $('#tblChequeBookList_rn').html('Sr.<br/> No.');
            unblockPage();

        },
        loadError: function (xhr, ststus, error)
        {
           if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
            }
            unblockPage();
        }
    });
}


