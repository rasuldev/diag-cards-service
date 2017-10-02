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

    processExpDate();
    $("#CardType").on("change", () => {
        processExpDate();
        processNotes(); 
    });
    $("#IssueYear").on("blur", processExpDate);
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
            result = $.parseJSON(result);
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
        }).error(function(e) {
            console.log(e);
        });
    
}

function processNotes() {
    var taxiNotes = "ИСПОЛЬЗУЕТСЯ В КАЧЕСТВЕ ТАКСИ";
    var cardType = $("#CardType").val();
    var note = $("#Note");
    if (cardType === "11") {
        // if Taxi
        if (note.val().trim() === "") {
            note.val(taxiNotes);
        }
    } else {
        if (note.val() === taxiNotes) {
            note.val("");
        }
    }
}

function processExpDate() {
    // Current date
    var regDate = strToDate($("#RegisteredDate").val());
    var cardType = $("#CardType").val();
    var monthsToAdd = 12;
    if (cardType === "11") {
        // if Taxi
        monthsToAdd = 6;
    } else {
        var carAge = regDate.getYear() - $("#IssueYear").val();
        monthsToAdd = (carAge > 7) ? 12 : 24;
    }

    regDate.setMonth(regDate.getMonth() + monthsToAdd);
    regDate.setDate(regDate.getDate() - 1);
    $("#ExpirationDate").val(dateToStr(regDate));
}

function strToDate(dateStr) {
    var chunks = dateStr.split(".");
    return new Date(chunks[2], chunks[1]-1, parseInt(chunks[0]));
}

function dateToStr(date) {
    var day = date.getDate().toString().padStart(2, "0");
    var month = (date.getMonth() + 1).toString().padStart(2, "0");
    var year = date.getFullYear();
    return `${day}.${month}.${year}`;
}