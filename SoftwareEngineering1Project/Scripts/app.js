//options for toastr
toastr.options = {
    progressBar: true,
    "closeButton": false,
    "debug": false,
    "newestOnTop": true,
    "progressBar": true,
    "positionClass": "toast-top-right",
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "5000",
    "extendedTimeOut": "1000",
    "showEasing": "linear",
    "hideEasing": "swing",
    "showMethod": "slideDown",
    "hideMethod": "fadeOut"
};

//initialize assets
$(document).ready(function () {
    $('.datepicker').datepicker({
        dateFormat: 'mm/dd/yy',
        showAnim: 'slideDown'
    });
});