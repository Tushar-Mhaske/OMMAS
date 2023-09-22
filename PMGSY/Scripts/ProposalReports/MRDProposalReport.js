
$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmMRDProposal");

    $("#idFilterDiv").click(function () {

        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#divFilterForm").toggle("slow");

        if ($("#loadReportParent").height() == 560) {
            $("#loadReportParent").height('650');
        }
        else {
            $("#loadReportParent").height('560');
        }

    });


    $('#ddlState').change(function () {

        $('#ddlDistrict').empty();
        FillInCascadeDropdown({ userType: $("#ddlDistrict").find(":selected").val() },
                    "#ddlDistrict", "/ProposalReports/GetDistrictbyState?stateCode=" + $('#ddlState option:selected').val());

        $('#ddlAgency').empty();
        FillInCascadeDropdown({ userType: $("#ddlAgency").find(":selected").val() },
                    "#ddlAgency", "/ProposalReports/PopulateAgencies?stateCode=" + $('#ddlState option:selected').val());
    }); //end function District change

    //$('#ddlState').change(function () {
    //    $('#ddlPackage').empty();
    //    FillInCascadeDropdown({ userType: $("#ddlPackage").find(":selected").val() },
    //                "#ddlPackage", "/ProposalReports/PopulatePackages?param=" + $('#ddlState option:selected').val());

    //}); //end function State change

    $('#ddlDistrict').change(function () {
        $('#ddlPackage').empty();
        if (($('#ddlState').is(":visible"))) {

            FillInCascadeDropdown({ userType: $("#ddlDistrict").find(":selected").val() },
                        "#ddlPackage", "/ProposalReports/GetPackages?param=" + $('#ddlState option:selected').val() + '$' + $('#ddlDistrict option:selected').val());
        }
        else {
            FillInCascadeDropdown({ userType: $("#ddlDistrict").find(":selected").val() },
                        "#ddlPackage", "/ProposalReports/GetPackages?param=" + $('#StateCode').val() + '$' + $('#ddlDistrict option:selected').val());
        }
        $('#ddlBlock').empty();
        FillInCascadeDropdown({ userType: $("#ddlDistrict").find(":selected").val() },
                    "#ddlBlock", "/ProposalReports/GetBlocksbyDistrict?distCode=" + $('#ddlDistrict option:selected').val());

    }); //end function District change

    $('#ddlYear').change(function () {
        $('#ddlPackage').empty();
        FillInCascadeDropdown({ userType: $("#ddlYear").find(":selected").val() },
                    "#ddlPackage", "/ProposalReports/GetPackages?param=" + $('#ddlState option:selected').val() + '$' + $('#ddlDistrict option:selected').val() + '$' + $('#ddlYear option:selected').val());

    }); //end function District change

    $('#ddlBatch').change(function () {
        $('#ddlPackage').empty();
        FillInCascadeDropdown({ userType: $("#ddlBatch").find(":selected").val() },
                    "#ddlPackage", "/ProposalReports/GetPackages?param=" + $('#ddlState option:selected').val() + '$' + $('#ddlDistrict option:selected').val() + '$' + $('#ddlYear option:selected').val() + '$' + $('#ddlBatch option:selected').val());

    }); //end function District change

    $('#ddlCollaboration').change(function () {
        $('#ddlPackage').empty();
        FillInCascadeDropdown({ userType: $("#ddlCollaboration").find(":selected").val() },
                    "#ddlPackage", "/ProposalReports/GetPackages?param=" + $('#ddlState option:selected').val() + '$' + $('#ddlDistrict option:selected').val() + '$' + $('#ddlYear option:selected').val() + '$' + $('#ddlBatch option:selected').val() + '$' + $('#ddlCollaboration option:selected').val());

    }); //end function District change

    function FillInCascadeDropdown(map, dropdown, action) {

        var message = '';

        //if (dropdown == '#ddlDistrict') {

        //    message = '<h4><label style="font-weight:normal"> Loading Districts... </label></h4>';
        //}
        //else if (dropdown == '#ddlBlocks') {
        //    message = '<h4><label style="font-weight:normal"> Loading Blocks... </label></h4>';
        //}

        $(dropdown).empty();
        //$(dropdown).append("<option value=0>--Select--</option>");

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        //$.blockUI({ message: message });

        $.post(action, map, function (data) {
            $.each(data, function () {
                $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
        }, "json");
        $.unblockUI();

    } //end FillInCascadeDropdown()



    $("#btnViewMRDProposalReport").click(function () {
        

        if ($("#frmMRDProposal").valid()) {

            $("#divFilterForm").toggle("slow");


            if ($("#loadReportParent").height() == 570) {
                $("#loadReportParent").height('650');
            }
            else {
                $("#loadReportParent").height('570');
            }

            $("#loadReport").html("");
            $("#loadReport1").html('');

            if (($('#ddlState').is(":visible"))) {
                $("#StateName").val($("#ddlState option:selected").text());
            }
            if (($('#ddlDistrict').is(":visible"))) {
                $("#DistrictName").val($("#ddlDistrict option:selected").text());
            }
            $("#BlockName").val($("#ddlBlock option:selected").text());
            $("#YearName").val($("#ddlYear option:selected").text());
            $("#BatchName").val($("#ddlBatch option:selected").text());
            $("#CollabName").val($("#ddlCollaboration option:selected").text());
            $("#PackageName").val($("#ddlPackage option:selected").text());
            $("#StatusName").val($("#ddlStatus option:selected").text());
            $("#STAStatusName").val($("#ddlSTAStatus option:selected").text());
            $("#ProposalName").val($("#ddlProposal option:selected").text());
            $("#CategoryName").val($("#ddlCategory option:selected").text());
            if (($('#ddlState').is(":visible"))) {
                $("#AgencyName").val($("#ddlAgency option:selected").text());
            }
            if ($("#ddlProposal").val() == "L") {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });


                $.ajax({
                    url: "/ProposalReports/MRDProposalDataReport/",
                    cache: false,
                    type: "POST",
                    async: false,
                    data: $("#frmMRDProposal").serialize(),
                    success: function (response) {

                        $("#loadReport").html('');
                        $("#loadReport").html(response);
                    },
                    error: function () {
                        alert("error");
                    }
                })
                $.unblockUI();
            }
            else if ($("#ddlProposal").val() == "P") {

                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

                $.ajax({
                    url: "/ProposalReports/MRDProposalDataReportRoad/",
                    cache: false,
                    type: "POST",
                    async: false,
                    data: $("#frmMRDProposal").serialize(),
                    success: function (response) {

                        $("#loadReport").html('');
                        $("#loadReport").html(response);
                    },
                    error: function () {
                        alert("error");
                    }
                })
                $.unblockUI();
            }
            else {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });


                $.ajax({
                    url: "/ProposalReports/MRDProposalDataReportRoad/",
                    cache: false,
                    type: "POST",
                    async: false,
                    data: $("#frmMRDProposal").serialize(),
                    success: function (response) {

                        $("#loadReport").html('');
                        $("#loadReport").html(response);
                    },
                    error: function () {
                        alert("error");
                    }
                })

                $.ajax({
                    url: "/ProposalReports/MRDProposalDataReport/",
                    cache: false,
                    type: "POST",
                    async: false,
                    data: $("#frmMRDProposal").serialize(),
                    success: function (response) {

                        $("#loadReport1").html('');
                        $("#loadReport1").html(response);
                    },
                    error: function () {
                        alert("error");
                    }
                })
                $.unblockUI();
            }
        }
    });

    //closableNoteDiv("divCompRoads", "spnCompRoads");
});