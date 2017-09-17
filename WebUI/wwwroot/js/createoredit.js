$(document).ready(function () {
    $('#DocumentIssueDate').datepicker();
    $('#ExpirationDate').datepicker();

    $('[data-val-required]').parent().parent().children('label').append('<span style="color:red">*</span>');
    //$('#ExpirationDate').datepicker({dateFormat: 'dd.MM.yyyy'});
    $('button.absent').on("click",
        function () {
            var id = $(this).attr("data-for");
            var input = $("#" + id).get(0);
            input.value = "";
            input.disabled = !input.disabled;
            input.placeholder = input.disabled ? "ОТСУТСТВУЕТ" : "";
        });
    $('[data-toggle="tooltip"]').tooltip();
    $("#mainForm").on('submit',
        function () {
            $("#mainForm input[type='text']").each(function () {
                this.value = this.value.toUpperCase();
            });
        });
    getVin();
    $("#VIN").on("blur", getVin);
});

function getVin() {
    $("#lastRunning").html('<i class="fa fa-spinner" aria-hidden="true"></i>');
    var vin = $("#VIN").val();
    if (!vin) {
        $("#lastRunning").text("введите VIN");
        return;
    }

    $.getJSON("/cards/lastrunning/" + vin)
        .done(function (result) {
            console.log(result);
            if (result.error === "session expired") {
                $("#lastRunning").text("не удалось получить информацию");
                return;
            }
            if (result.error === "no info") {
                $("#lastRunning").text("нет информации");
                return;
            }

            if (result.error === "unknown error") {
                $("#lastRunning").text("не удалось получить информацию");
                return;
            }

            $("#lastRunning").text(`${result.running} км (${result.date.split('T')[0]})`);
        })
        .fail(function () {
            $("#lastRunning").text("не удалось получить информацию");
        });
}