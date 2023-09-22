
//Added By Abhishek kamble 3-dec-2013

$(document).ready(function () {

  

    //added by abhishek kamble 5-dec-2013
    //$('.linkSchedule').click(function () {
    //    alert("test");

    //    //$(this).closest('form')[0].submit();

    //    alert($("#frmShowSchedule").serialize());


    //});

    //$("#idFilterDiv").click(function () {
    //    $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");

    //    if ($("#dvShowHideBalanceSheet").is(":hidden")) {
    //        $("#dvShowHideBalanceSheet").show("slow");
    //    }
    //    else {
    //        $("#dvShowHideBalanceSheet").hide("slow");
    //    }

    //    if ($("#dvBalanceSheetDetails").is(":hidden")) {
    //        $("#dvBalanceSheetDetails").show("slow");
    //    }
    //    else {
    //        $("#dvBalanceSheetDetails").hide("slow");
    //    }

    //    if ($("#tblMaintenanceFundBalanceSheet_wrapper").is(":hidden")) {
    //        $("#tblMaintenanceFundBalanceSheet_wrapper").show("slow");
    //    }
    //    else {
    //        $("#tblMaintenanceFundBalanceSheet_wrapper").hide("slow");
    //    }

    //});

    //$("#idFilterDiv").click(function () {
    //    $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
     
    //    $("#dvShowHideBalanceSheet").slideToggle("slow");
    //    $("#dvBalanceSheetDetails").slideToggle("slow");
    //    $("#tblMaintenanceFundBalanceSheet_wrapper").slideToggle("slow");
    //});


});
//var actionName;
//var ControllerName;

function ShowBalanceSheetSchedules(actionName,controllerName,HeadName)
{
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#HeadName").val(HeadName);


    //alert($("#Month").val());
    //alert($("#frmShowSchedule").serialize());
    //alert(actionName);
    //alert(controllerName);

    //alert($("#data").val());

    //var url = '/' + controllerName + '/' + actionName + '/BalanceSheetViewModel?=' + $("#frmShowSchedule").serialize();

    $.ajax({
        type: 'POST',
        url: '/' + controllerName + '/' + actionName + '/',
        async: false,
        data: $("#frmShowSchedule").serialize(),
        success: function (data) {
            //alert("Success");

            //alert(data);

            //var w = window.open();
            //$(w.document.body).html(data);

            //var w = window.open();
            //var html = $("#dvTest").html(data);            
            //$(w.document.body).html(html);

            $("#dvShowHideBalanceSheet").hide("slow");
            $("#dvBalanceSheetDetails").hide("slow");
            $("#tblMaintenanceFundBalanceSheet_wrapper").hide("slow");
            $("#dvShowBalancesheetSchedules").html('');
            $("#dvShowBalancesheetSchedules").html(data);
            $("#dvShowBalancesheetSchedules").show("slow");
            //var w = window.open();
            //w.document.write(data);

            //window.open(
            //                //'data:application/pdf,' + encodeuricomponent(data),
            //                data,
            //                'batch print',
            //                'width=600,height=600,location=_newtab'
            //            );

            //if (data.indexOf("http://") == 0) {
            //    alert("t");
            //    window.open(data);
            //}
            //else {
            //    var win = window.open();
            //    with (win.document) {
            //        open();
            //        write(data); //-> how to execute this HTML code? The code also includes references to other js files.
            //        close();
            //    }
            //}            
            
            //return false;

            //if (data.success == true) {
            //    alert(data.message);
            //    ClearDetails();
            //    $('#constructionCategory').trigger('reloadGrid');
            //    $.unblockUI();
            },        
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
            $.unblockUI();
        }
    })//ajax end

    //window.open(url, 'location=_newtab', "toolbar=yes, scrollbars=yes, resizable=yes");

    //$.post('/' + controllerName + '/' + actionName + '/', { text1: "aaa", text2: "bbb" }, function (result) {
    //    WinId = window.open('', 'newwin', 'width=400,height=500');
    //    WinId.document.open();
    //    WinId.document.write(result);
    //    WinId.document.close();
    //});
    $.unblockUI();
}

