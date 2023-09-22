$(document).ready(function () {
    $.validator.unobtrusive.parse("#frmCreateNews");

    $("#btnUpdateNews").click(function () {
        // alert("4");
        UpdateNews();
    });


    $('#txtNewsPublishSt').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a start date',
        buttonImageOnly: true,
        buttonText: 'Start Date',
        changeMonth: true,
        changeYear: true,
        minDate:0,
        //minDate: new Date(currentYear, currentMonth, currentDate),
        onSelect: function (selectedDate) {
            $("#txtNewsPublishEnd").datepicker("option", "minDate", selectedDate);
            $(function () {
                $('#txtNewsPublishSt').focus();
                $('#txtNewsPublishEnd').focus();
            })
        }
    });

    $('#txtNewsPublishEnd').datepicker({
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose a end date',
        buttonImageOnly: true,
        buttonText: 'End Date',
        changeMonth: true,
        changeYear: true,
        //minDate: 0,
        onSelect: function (selectedDate) {
            $("#txtNewsPublishSt").datepicker("option", "maxDate", selectedDate);
        }
    });

    $(function () {
        $("#txtNewsPublishEnd").datepicker("option", "minDate", $('#txtNewsPublishSt').val());
    });
    


    $("#btnCreateNews").click(function () {
        //$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        if ($('#frmCreateNews').valid()) {

            $.ajax({
                url: "/NewsDetails/NewsCreation/",
                cache: false,
                type: "POST",
                async: false,
                data: $("#frmCreateNews").serialize(),
                success: function (data) {


                    if (data.status == true) {
                        alert("News Created Successfully");

                        //$("#dvNewsDetails").load('/NewsDetails/CreateNews', function () {
                        //    $.validator.unobtrusive.parse($('#dvNewsDetails'));

                        //    //    unblockPage();
                        //    //});
                        //    $('#dvNewsDetails').show('slow');
                        //    $("#dvNewsDetails").css('height', 'auto');
                        //});
                        $("#btnAddNews").show();
                        $('#dvNewsDetails').hide();
                        $('#accordion').hide();
                        loadNewsDetails();
                    }
                    else {
                        alert("Error occured while saving News.");
                    }

                },
                error: function () {
                    alert("error");
                }
            })
        }
    });


});


function UpdateNews() {
    //$.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    // alert("1");
    if ($('#frmCreateNews').valid()) {
        $.ajax({
            url: "/NewsDetails/NewsUpdation/",
            cache: false,
            type: "POST",
            async: false,
            data: $("#frmCreateNews").serialize(),
            success: function (data) {
                if (data.status == "1") {
                    alert("News Updated Successfully");

                    //$("#dvNewsDetails").load('/NewsDetails/CreateNews', function () {
                    //    $.validator.unobtrusive.parse($('#dvNewsDetails'));

                    //    //    unblockPage();
                    //    //});
                    //    $('#dvNewsDetails').show('slow');
                    //    $("#dvNewsDetails").css('height', 'auto');
                    //});

                    //$('#dvNewsDetails').hide();
                    //$("#accordion h3").hide();
                    //$("#tbNewsDetailsJqGrid").trigger('reloadGrid');
                    ////$('#tbNewsDetailsJqGrid').show('slow');
                    ////$("#tbNewsDetailsJqGrid").css('height', 'auto');
                    //$("#tbNewsDetailsJqGrid").jqGrid('setGridState', 'visible');
                    //$("#btnAddNews").show('slow');

                    CloseNewsDetails();
                }
                else if (data.status == "-1") {
                    alert("Error occured while Updating News.");
                }
                else if (data.status == "0") {
                    alert("Could not Update News.");
                }
                else if (data.status == "-2") {
                    alert("News is not Approved");
                }
            },
            error: function () {
                alert("error");
            }
        })
    }
};

