var isGridLoaded = false;

//var AdminCode
$(document).ready(function () {

    LoadGrid(0, 0, 0, 0, "");

    $("#rdoSearchSRRDA").click(function () {
        $("#trSearchDPIU").hide("slow");
     //   LoadGrid(0, 0, 0, 0, "SRRDA");
    });
    $("#rdoSearchDPIU").click(function () {
        $("#trSearchDPIU").show("slow");
      //  LoadGrid(0, 0, 0, $("#ddlDPIU").val(), $("#ddlDPIU option:selected").text());
    });
    
    //$("#ddlSearchDPIU").change(function () {
        
    //    LoadGrid($("#ddlMonth").val(), $("#ddlYear").val(), $("#txtChequeNo").val(), $("#ddlSearchDPIU").val(), $("#ddlSearchDPIU option:selected").text());

    //});

    $("#AddChequeBook").click(function () {
        blockPage();
        $("#divChequeBookError").hide("slide");
        $("#divChequeBookError span:eq(1)").html("");
        $('#ddlSearchDPIU option:nth(0)').attr("selected", "selected");

        //$("#loadPage").load('/ChequeBook/AddEditChequeBook', function () {
        //    alert("test");
        //    $("#AddChequeBook").hide('slow');
        //    $("#tblSearch").hide('slow');
        //    $("#loadPage").show('slow');
        //    $("#searchChequeBook").show('slow');
        //}); 
        
        $.ajax({
            url: "/ChequeBook/AddEditChequeBook/",
            type: "GET",
            cache: false,
            data:{AdminNdCode: $("#ddlDPIU").val()},
            success: function (data) {                              
                $("#loadPage").html(data);
                $("#AddChequeBook").hide('slow');
                $("#tblSearch").hide('slow');
                $("#loadPage").show('slow');
                $("#searchChequeBook").show('slow')

                $("#ddlDPIU").trigger("change");

                unblockPage();        
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                unblockPage();          
            }
        });


        //$.ajax({
        //    url: "/ChequeBook/CheckBankDetailsExist/",
        //    type: "GET",
        //    //dataType: "html",
        //    async: false,
        //    cache: false,
        //    success: function (data) {
        //        var isBankDetailsExist = data.exist;

        //        if (isBankDetailsExist == true) {
        //            $.ajax({
        //                url: "/ChequeBook/AddEditChequeBook/",
        //                type: "GET",
        //                //dataType: "html",
        //                async: false,
        //                cache: false,
        //                success: function (data) {
        //                    $("#loadPage").html(data);
        //                    $("#AddChequeBook").hide('slow');
        //                    $("#tblSearch").hide('slow');
        //                    $("#loadPage").show('slow');
        //                    $("#searchChequeBook").show('slow')
        //                },
        //                error: function (xhr, ajaxOptions, thrownError) {
        //                    alert(xhr.responseText);
        //                }

        //            });
        //        }
        //        else {
        //            alert('Bank details does not exist.');
        //        }
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }

        //});


       

    });

    $("#searchChequeBook").click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $("#divChequeBookError").hide("slide");
        $("#divChequeBookError span:eq(1)").html("");
        $("#loadPage").hide('slow');
        $("#tblSearch").show('slow');
        $("#AddChequeBook").show("slow");
        $("#searchChequeBook").hide('slow');

        LoadGrid(0, 0, 0, $("#ddlSearchDPIU").val(), $("#ddlSearchDPIU option:selected").text());

        $.unblockUI();

       // AdminCode = $("#ddlSearchDPIU").val();
    });    

    $("#btnSearch").click(function () {

        if (isGridLoaded) {
            $("#tblChequeBookList").GridUnload();
            if ($("#rdoSearchSRRDA").is(":checked")) {
                LoadGrid($("#ddlMonth").val(), $("#ddlYear").val(), $("#txtChequeNo").val(), $("#ddlSearchDPIU").val(), "SRRDA");
            } else {
                LoadGrid($("#ddlMonth").val(), $("#ddlYear").val(), $("#txtChequeNo").val(), $("#ddlSearchDPIU").val(), $("#ddlSearchDPIU option:selected").text());
            }
        }
        else {
            
            if ($("#rdoSearchSRRDA").is(":checked")) {
                LoadGrid($("#ddlMonth").val(), $("#ddlYear").val(), $("#txtChequeNo").val(), $("#ddlSearchDPIU").val(), "SRRDA");
            } else {
                LoadGrid($("#ddlMonth").val(), $("#ddlYear").val(), $("#txtChequeNo").val(), $("#ddlSearchDPIU").val(), $("#ddlSearchDPIU option:selected").text());
            }
        }
        //AdminCode = $("#ddlSearchDPIU").val();
    });

    $("#btnSearchCancel").click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $("#tblSearch").hide('slow');
        $("#searchChequeBook").show('slow');

        $('#ddlSearchDPIU option:nth(0)').attr("selected", "selected");


        $.unblockUI();
    });

    $("#iconClose").click(function () {
        $("#btnSearchCancel").trigger("click");
    });

}); // end of DOM


function EditChequeBook(urlparamater) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    if ($("#tblSearch").is(":visible")) {
       // AdminCode = $("#ddlSearchDPIU").val();
        $("#iconClose").trigger("click");
    } else if ($("#loadPage").is(":visible")) {
       // AdminCode = $("#ddlDPIU").val();
    }


    $("#AddChequeBook").hide("slow");
    if (!$("#divAddNew").is(":visible")) {
        $("#divAddNew").slideToggle(300);
    }
    $("#aAddNew span:first").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

    $.ajax({
        url: "/ChequeBook/Edit/" + urlparamater,
        type: "GET",
        //data: { AdminNdCode: AdminCode },
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {

            $("#loadPage").html(data);
            $("#loadPage").show();

            if ($("#rdoSRRDA").is(":checked"))
            {
                $(".tdDPIU").hide();
            }
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();

        }

    });
}


function DeleteChequeBook(urlparamater) {
    

    if (confirm("Are you sure to delete Cheque Book Details?")) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        //var AdminNdCode = 0;
        //if ($("#tblSearch").is(":visible")) {
        //   // alert("test vi");
        //    AdminNdCode = $("#ddlSearchDPIU").val();
        //} else if ($("#frmChequeBookDetails").is(":visible")) {
        //    AdminNdCode = $("#ddlDPIU").val();
        //    //alert("test :" + $("#frmChequeBookDetails").is(":visible"));
        //}


        $.ajax({
            url: "/ChequeBook/Delete/" + urlparamater,
            type: "POST",
            async:false,
            //dataType: "json",            
           // data: { AdminNdCode: AdminNdCode },
            success: function (data) {
                $.unblockUI();

                if (data.success) {
                    alert("Cheque Book Details Deleted");
                    $("#divChequeBookError").hide("slide");
                    $("#divChequeBookError span:eq(1)").html("");
                    //$("#mainDiv").html(data);
                    //LoadGrid(0, 0, 0,0,"");
                    $("#tblChequeBookList").trigger("reloadGrid");
                }
                else {
                    $("#divChequeBookError").show("slide");
                    $("#divChequeBookError span:eq(1)").html('<strong>Alert: </strong>' + data.message);
                    return false;
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


function LoadGrid(month, year, cheque,adminNdCode,dpiuName)
{
    blockPage();
    //alert($("#ddlDPIU").val());

    if (cheque == "")
    {
        cheque = 0;
    }
    if (isGridLoaded == true)
    {
        $("#tblChequeBookList").GridUnload();
    }
    var IsSRRDADpiu;

    if ($("#tblSearch").is(":visible")) {
        IsSRRDADpiu=($("#rdoSearchDPIU").is(":checked") ? $("#rdoSearchDPIU").val() : $("#rdoSearchSRRDA").val())
    }
    else if ($("#frmChequeBookDetails").is(":visible")) {
        IsSRRDADpiu=($("#rdoDPIU").is(":checked") ? $("#rdoDPIU").val() : $("#rdoSRRDA").val())
    }

    jQuery("#tblChequeBookList").jqGrid({
        url: '/ChequeBook/GetChequeBookList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Cheque Book Received Date', 'Start Number', 'End Number', 'Edit', 'Delete'],
        colModel: [
                            { name: 'ChqRecDate', index: 'ChqRecDate', width: 180, align: 'center', sortable: true },
                            { name: 'LeafStart', index: 'LeafStart', width: 100, align: 'center', sortable: true },
                            { name: 'LeafEnd', index: 'LeafEnd', width: 100, align: 'center', sortable: true },
                            { name: 'Edit', index: 'Edit', width: 100, align: 'center', sortable: false ,hidden:false},
                            { name: 'Delete', index: 'Delete', width: 100, align: 'center', sortable: false, hidden: false}
        ],
        pager: jQuery('#divChequeBookListPager'),
        rowNum: 10,
        postData: {
            'month':month, 
            'year': year,
            'cheque': cheque,
            'AdminNdCode': adminNdCode,
            'IsSRRDADpiu':IsSRRDADpiu
        },
        altRows: true,
        rowList: [10, 20, 50],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'ChqRecDate',
        sortorder: "desc",
        caption: "Cheque Book Details - " + dpiuName,
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        //hidegrid: false,
        loadComplete: function () {
            isGridLoaded = true;
            $('#tblChequeBookList_rn').html('Sr.<br/> No.');
            unblockPage();

        },
        loadError: function (xhr, ststus, error) {

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
        //.navGrid('#divChequeBookListPager', { edit: false, add: false, del: false, search: false, refresh: false })
        //.navButtonAdd('#divChequeBookListPager', {
        //    caption: "Reset Cheque Book Details",
        //    buttonicon: "ui-icon-refresh",
        //    onClickButton: function () {
        //        $("#tblChequeBookList").GridUnload();
        //        $("#ddlMonth").val("0"); $("#ddlYear").val("0"); $("#txtChequeNo").val("");
        //        LoadGrid(0, 0, 0,0);
        //    },
        //    position: "first"
        //}); //end of documents grid
}
