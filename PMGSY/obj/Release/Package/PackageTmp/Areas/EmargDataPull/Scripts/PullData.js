$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmPackageWiseReport'));

    // CHANGES MADE ON 50-01-2022
    $("#btnReadBalanceWorkExpenditure").click(function () {


        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/EmargDataPull/EmargDataPull/GetRoadWiseExpenditureBalanceWork/',
            type: 'POST',
            catche: false,
            //       data: $("#frmPackageWiseReport").serialize(),
            async: false,
            success: function (response) {
                console.log(response);
                $.unblockUI();
                //   alert("Details received Successfully.")


                if (response.success) {
                    alert(response.message)
                }
                else {
                    alert(response.message)
                }


            },
            error: function () {
                $.unblockUI();
                alert("Details not received.")
                return false;
            },
        });



    });


    $("#btnReadBalanceWork").click(function () {


        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/EmargDataPull/EmargDataPull/GetRoadWiseBalanceWorkPackageDetails/',
            type: 'POST',
            catche: false,
            //       data: $("#frmPackageWiseReport").serialize(),
            async: false,
            success: function (response) {
                console.log(response);
                $.unblockUI();
                //   alert("Details received Successfully.")


                if (response.success) {
                    alert(response.message)
                }
                else {
                    alert(response.message)
                }


            },
            error: function () {
                $.unblockUI();
                alert("Details not received.")
                return false;
            },
        });



    });


    $("#btnReadLockPackage").click(function () {


        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/EmargDataPull/EmargDataPull/GetLockedPackageRecord/',
            type: 'POST',
            catche: false,
            //       data: $("#frmPackageWiseReport").serialize(),
            async: false,
            success: function (response) {
                console.log(response);
                $.unblockUI();
                //   alert("Details received Successfully.")


                if (response.success) {
                    alert(response.message)
                }
                else {
                    alert(response.message)
                }


            },
            error: function () {
                $.unblockUI();
                alert("Details not received.")
                return false;
            },
        });



    });

    // CHANGES ENDED HERE ON 05-01-2022

    $("#btnViewPackageWiseDetails").click(function () {


        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/EmargDataPull/EmargDataPull/PullEmargRoadDetails/',
            type: 'POST',
            catche: false,
            //       data: $("#frmPackageWiseReport").serialize(),
            async: false,
            success: function (response) {
                console.log(response);
                $.unblockUI();
                //   alert("Details received Successfully.")


                if (response.success) {
                    alert(response.message)
                }
                else {
                    alert(response.message)
                }


            },
            error: function () {
                $.unblockUI();
                alert("Details not received.")
                return false;
            },
        });



    });


    $("#btnViewAckDetails").click(function () {


        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/EmargDataPull/EmargDataPull/PullEmargAcknowledmentDetails/',
            type: 'POST',
            catche: false,
            //       data: $("#frmPackageWiseReport").serialize(),
            async: false,
            success: function (response) {
                console.log(response);
                $.unblockUI();
                //  alert("Acknowledment Details received Successfully.")


                if (response.success) {
                    alert(response.message)
                }
                else {
                    alert(response.message)
                }


            },
            error: function () {
                $.unblockUI();
                alert("Acknowledment Details not received.")
                return false;
            },
        });



    });


    $("#btnGetDataFromFirst").click(function () {


        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/EmargDataPull/EmargDataPull/GetDataFromFirstLevelDetails/',
            type: 'POST',
            catche: false,
            //       data: $("#frmPackageWiseReport").serialize(),
            async: false,
            success: function (response) {
                console.log(response);
                $.unblockUI();
                //  alert("Data received in table omms.EMARG_FIRST_LEVEL_ACK_SERVICE Successfully.")


                if (response.success) {
                    alert(response.message)
                }
                else {
                    alert(response.message)
                }


            },
            error: function () {
                $.unblockUI();
                alert(" Details not received.")
                return false;
            },
        });



    });
    $("#btnGetDataFromSecond").click(function () {


        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/EmargDataPull/EmargDataPull/GetDataFromSecondService/',
            type: 'POST',
            catche: false,
            //       data: $("#frmPackageWiseReport").serialize(),
            async: false,
            success: function (response) {
                console.log(response);
                $.unblockUI();
                //  alert("Data received in table omms.EMARG_ROAD_CORRECTION_SERVICEII Successfully.")


                if (response.success) {
                    alert(response.message)
                }
                else {
                    alert(response.message)
                }


            },
            error: function () {
                $.unblockUI();
                alert(" Details not received.")
                return false;
            },
        });



    });

    $("#btnGetEmargKPI").click(function () {


        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/EmargDataPull/EmargDataPull/GetKPIDetails/',
            type: 'POST',
            catche: false,
            //       data: $("#frmPackageWiseReport").serialize(),
            async: false,
            success: function (response) {
                console.log(response);
                $.unblockUI();
                if (response.success) {
                    alert(response.message)
                }
                else {
                    alert(response.message)
                }

            },
            error: function () {
                $.unblockUI();
                alert(" Details not received.")
                return false;
            },
        });



    });




    //$("#sendMailToSQC").click(function () {


    //    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
    //    $.ajax({
    //        url: '/EmargDataPull/EmargDataPull/SendEmailToSQC/',
    //        type: 'POST',
    //        catche: false,
    //        //       data: $("#frmPackageWiseReport").serialize(),
    //        async: false,
    //        success: function (response) {
    //            console.log(response);
    //            $.unblockUI();
    //            if (response.success) {
    //                alert(response.message)
    //            }
    //            else {
    //                alert(response.message)
    //            }

    //        },
    //        error: function () {
    //            $.unblockUI();
    //            alert(" Details not received.")
    //            return false;
    //        },
    //    });

    //});


    $("#sendMailToSQM1").click(function () {


        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/EmargDataPull/EmargDataPull/SendEmailToSQM1/',
            type: 'POST',
            catche: false,
            //       data: $("#frmPackageWiseReport").serialize(),
            async: false,
            success: function (response) {
                console.log(response);
                $.unblockUI();
                if (response.success) {
                    alert(response.message)
                }
                else {
                    alert(response.message)
                }

            },
            error: function () {
                $.unblockUI();
                alert(" Details not received.")
                return false;
            },
        });

    });


    $("#sendMailToSQM2").click(function () {


        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/EmargDataPull/EmargDataPull/SendEmailToSQM2/',
            type: 'POST',
            catche: false,
            //       data: $("#frmPackageWiseReport").serialize(),
            async: false,
            success: function (response) {
                console.log(response);
                $.unblockUI();
                if (response.success) {
                    alert(response.message)
                }
                else {
                    alert(response.message)
                }

            },
            error: function () {
                $.unblockUI();
                alert(" Details not received.")
                return false;
            },
        });

    });


    $("#sendMailToSQM3").click(function () {


        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/EmargDataPull/EmargDataPull/SendEmailToSQM3/',
            type: 'POST',
            catche: false,
            //       data: $("#frmPackageWiseReport").serialize(),
            async: false,
            success: function (response) {
                console.log(response);
                $.unblockUI();
                if (response.success) {
                    alert(response.message)
                }
                else {
                    alert(response.message)
                }

            },
            error: function () {
                $.unblockUI();
                alert(" Details not received.")
                return false;
            },
        });

    });


    $("#sendMailToSQM4").click(function () {


        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/EmargDataPull/EmargDataPull/SendEmailToSQM4/',
            type: 'POST',
            catche: false,
            //       data: $("#frmPackageWiseReport").serialize(),
            async: false,
            success: function (response) {
                console.log(response);
                $.unblockUI();
                if (response.success) {
                    alert(response.message)
                }
                else {
                    alert(response.message)
                }

            },
            error: function () {
                $.unblockUI();
                alert(" Details not received.")
                return false;
            },
        });

    });



    $("#sendMailToSQM5").click(function () {


        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/EmargDataPull/EmargDataPull/SendEmailToSQM5/',
            type: 'POST',
            catche: false,
            //       data: $("#frmPackageWiseReport").serialize(),
            async: false,
            success: function (response) {
                console.log(response);
                $.unblockUI();
                if (response.success) {
                    alert(response.message)
                }
                else {
                    alert(response.message)
                }

            },
            error: function () {
                $.unblockUI();
                alert(" Details not received.")
                return false;
            },
        });

    });



    $("#sendMailToSQM6").click(function () {


        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/EmargDataPull/EmargDataPull/SendEmailToSQM6/',
            type: 'POST',
            catche: false,
            //       data: $("#frmPackageWiseReport").serialize(),
            async: false,
            success: function (response) {
                console.log(response);
                $.unblockUI();
                if (response.success) {
                    alert(response.message)
                }
                else {
                    alert(response.message)
                }

            },
            error: function () {
                $.unblockUI();
                alert(" Details not received.")
                return false;
            },
        });

    });



    $("#sendMailToSQM7").click(function () {


        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/EmargDataPull/EmargDataPull/SendEmailToSQM7/',
            type: 'POST',
            catche: false,
            //       data: $("#frmPackageWiseReport").serialize(),
            async: false,
            success: function (response) {
                console.log(response);
                $.unblockUI();
                if (response.success) {
                    alert(response.message)
                }
                else {
                    alert(response.message)
                }

            },
            error: function () {
                $.unblockUI();
                alert(" Details not received.")
                return false;
            },
        });

    });




    $("#sendMailToSQM8").click(function () {


        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/EmargDataPull/EmargDataPull/SendEmailToSQM8/',
            type: 'POST',
            catche: false,
            //       data: $("#frmPackageWiseReport").serialize(),
            async: false,
            success: function (response) {
                console.log(response);
                $.unblockUI();
                if (response.success) {
                    alert(response.message)
                }
                else {
                    alert(response.message)
                }

            },
            error: function () {
                $.unblockUI();
                alert(" Details not received.")
                return false;
            },
        });

    });



    $("#sendMailToSQM9").click(function () {


        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/EmargDataPull/EmargDataPull/SendEmailToSQM9/',
            type: 'POST',
            catche: false,
            //       data: $("#frmPackageWiseReport").serialize(),
            async: false,
            success: function (response) {
                console.log(response);
                $.unblockUI();
                if (response.success) {
                    alert(response.message)
                }
                else {
                    alert(response.message)
                }

            },
            error: function () {
                $.unblockUI();
                alert(" Details not received.")
                return false;
            },
        });

    });

    // $("#btnViewPackageWiseDetails").trigger('click');

    //if ($('#Mast_State_Code').val() > 0) {
    //    $("#btnViewPackageWiseDetails").trigger('click');
    //}

    $("#btnGetEmargPayment").click(function () {


        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/EmargDataPull/EmargDataPull/PullEmargPaymentDetails/',
            type: 'POST',
            catche: false,
            //       data: $("#frmPackageWiseReport").serialize(),
            async: false,
            success: function (response) {
                console.log(response);
                $.unblockUI();
                if (response.success) {
                    alert(response.message)
                }
                else {
                    alert(response.message)
                }

            },
            error: function () {
                $.unblockUI();
                alert(" Details not received.")
                return false;
            },
        });



    });

    $("#btnGetEmargPaymentNew").click(function () {


        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
        $.ajax({
            url: '/EmargDataPull/EmargDataPull/PullEmargPaymentDetailsNew/',
            type: 'POST',
            catche: false,
            //       data: $("#frmPackageWiseReport").serialize(),
            async: false,
            success: function (response) {
                console.log(response);
                $.unblockUI();
                if (response.success) {
                    alert(response.message)
                }
                else {
                    alert(response.message)
                }

            },
            error: function () {
                $.unblockUI();
                alert(" Details not received.")
                return false;
            },
        });



    });

    $("#spCollapseIconCN").click(function () {

        $("#spCollapseIconCN").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#dvSearchParameter").toggle("slow");

    });

    //this function call  on layout.js
    closableNoteDiv("divCommonReport", "spnCommonReport");
});



//District Change Fill Block DropDown List
