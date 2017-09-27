// Write your Javascript code.
function touch() {
    $.ajax({
        url: "/cards/touch",
        timeout: 10000,
        global: false,
        dataType: "text"
    });
}