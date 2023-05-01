// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    $("#show-image-fields-btn").click(function () {
        $("#image-fields").toggle();
        $(this).text(function (i, text) {
            return text === "Add Images" ? "Hide Fields" : "Add Images";
        });
    });
});

$(document).ready(function () {
    $('#rentalHistoryTable').DataTable();
});
$(document).ready(function () {
    $('#customerTable').DataTable();
});
$(document).ready(function () {
    $('#staffTable').DataTable();
});
$(document).ready(function () {
    $('#offerTable').DataTable();
});
$(document).ready(function () {
    $('#cusRequestTable').DataTable();
});
$(document).ready(function () {
    $('#staffRequestTable').DataTable();
});
$(document).ready(function () {
    $('#damageTable').DataTable();
});