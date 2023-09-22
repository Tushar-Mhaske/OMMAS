
$(document).ready(function () {
    StatePriorityInitialization();

});

function StatePriorityInitialization() {
    $("#btnPriority1Submit").click(function () {
        var progMin = $("#txtPriority1Prog").val();
        var CompMin = $("#txtPriority1Comp").val();
        if (!isInteger(progMin)) {
            alert("Enter Valid Progress Value.");
        }
        else if (!isInteger(CompMin)) {
            alert("Enter Valid Completed Value.");
        }
        else {
            var data = { priority: 1, ProgressMin: progMin, ProgressMax: 0, CompletedMin: CompMin, CompletedMax: 0 };
            LoadStateReport($('#div1StatePriorityList'), data);
            
        }
    });

    $("#btnPriority2Submit").click(function () {

        var progMin = $("#txtPriority2ProgMin").val();
        var CompMin = $("#txtPriority2CompMin").val();
        var progMax = $("#txtPriority2ProgMax").val();
        var CompMax = $("#txtPriority2CompMax").val();

        if (!isInteger(progMin)) {
            alert("Enter Valid Progress Min Value.");
        }
        else if (!isInteger(CompMin)) {
            alert("Enter Valid Completed Min Value.");
        }
        else if (!isInteger(progMax)) {
            alert("Enter Valid Progress max Value.");
        }
        else if (!isInteger(CompMax)) {
            alert("Enter Valid Completed max Value.");
        }
        else {

            var data = { priority: 2, ProgressMin: progMin, ProgressMax: progMax, CompletedMin: CompMin, CompletedMax: CompMax };
            LoadStateReport($('#div2StatePriorityList'), data);

        }
    });

    $("#btnPriority3Submit").click(function () {
        var progMin = $("#txtPriority3Prog").val();
        var CompMin = $("#txtPriority3Comp").val();
        if (!isInteger(progMin)) {
            alert("Enter Valid Progress Min Value.");
        }
        else if (!isInteger(CompMin)) {
            alert("Enter Valid Completed Min Value.");
        }
        else {
            var data = { priority: 3, ProgressMin: progMin, ProgressMax: 0, CompletedMin: CompMin, CompletedMax: 0 };
            LoadStateReport($('#div3StatePriorityList'), data);

        }
    });
    $("#txtPriority1Prog").focusout(function () {
        if (isInteger($("#txtPriority1Prog").val())) {
            $("#txtPriority2ProgMax").val($("#txtPriority1Prog").val());
            $("#txtPriority2ProgMin").val($("#txtPriority1Prog").val());
        }
        else {
            $("#txtPriority2ProgMax").val(0);
            $("#txtPriority2ProgMin").val(0);
        }
    });

    $("#txtPriority1Comp").focusout(function () {
        if (isInteger($("#txtPriority1Comp").val())) {
            $("#txtPriority2CompMax").val($("#txtPriority1Comp").val());
            $("#txtPriority2CompMin").val($("#txtPriority1Comp").val());
        }
        else {
            $("#txtPriority2CompMax").val(0);
            $("#txtPriority2CompMin").val(0);
        }
    });

    $("#txtPriority2ProgMin").focusout(function () {
        if (isInteger($("#txtPriority2ProgMin").val()))
            $("#txtPriority3Prog").val($("#txtPriority2ProgMin").val());
        else
            $("#txtPriority3Prog").val(0);
    });
    $("#txtPriority2CompMin").focusout(function () {
        if (isInteger($("#txtPriority2CompMin").val()))
            $("#txtPriority3Comp").val($("#txtPriority2CompMin").val());
        else
            $("#txtPriority3Comp").val(0);
    });

}

function LoadStateReport(grid, data) {
    grid.load("/QMSSRSReports/QMSSRSReports/StateSchedulePriorityReport/", data);
}


