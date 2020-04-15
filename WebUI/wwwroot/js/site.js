// Write your Javascript code.
function touch() {
    $.ajax({
        url: "/cards/touch",
        timeout: 20000,
        global: false,
        dataType: "text"
    });
}