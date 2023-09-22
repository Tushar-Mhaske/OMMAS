/**
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   
        * Description   :   Handles click event.
        * Author        :   Rohit Jadhav. 
        * Creation Date :   
 **/
$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmQMPahse'));

   // if ($("#hdnRoleCode").val() == "8") {
    //    $("#StateName").val($('#ddlQMPahse option:selected').text());

      //  loadReport();
 // }

    $("#btnQMPahse").click(function () {
        if ($('#frmQMPahse').valid()) {




            $("#loadReport1").html("");
            $("#loadReport2").html("");
            $("#loadReport3").html("");
            $("#LoadQMInspDetailsReport").html("");
            $("#StateName").val($('#ddlQMPahse option:selected').text());
            $("#MPConstName").val($('#ddlMPConst option:selected').text());
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif" />' });
            loadReport();
        }
    });

    // $("#loadChart").load('/RoadList/RoadLists/MPRoadListReport/');

    $("#ddlQMPahse").change(function () {

        $.blockUI({ message: '<h4><label style="font-weight:normal">loading MP Constituency...</label> ' });
        var val = $("#ddlQMPahse").val();
        $.ajax({
            type: 'POST',
            url: "/RoadList/RoadLists/PopulateMPConstituency?id=" + val,
            async: false,
            success: function (data) {
                $.unblockUI();
                $("#ddlMPConst").empty();
                $.each(data, function () {

                    $("#ddlMPConst").append("<option value=" + this.Value + ">" +
                                                                this.Text + "</option>");
                    //if (this.Text == "All DPIU") {
                    //    $("#ADMIN_ND_CODE").append("<option value=" + "0" + ">" +
                    //                                             "Select DPIU" + "</option>");
                    //} else {
                    //    $("#ADMIN_ND_CODE").append("<option value=" + this.Value + ">" +
                    //                                            this.Text + "</option>");
                    //}
                });

                $.unblockUI();
            }
        });
    });

});

function loadReport() {
    $.ajax({
        url: '/RoadList/RoadLists/MPRoadListReport/',
        type: 'POST',
        catche: false,
        data: $("#frmQMPahse").serialize(),
        async: false,
        success: function (response) {
            $.unblockUI();
            $("#loadReport3").html(response);

        },
        error: function () {
            $.unblockUI();
            alert("An Error");
            return false;
        },
    });
}