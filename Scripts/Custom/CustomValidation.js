$(document).ready(function() {
    //Restricts the user to entering numeric input with one potential decimal in the ##.## format
    $('#taxRate').keypress(function(event) {
        if ((event.which != 46 || $(this).val().indexOf('.') != -1) &&
            (event.which < 48 || event.which > 57)) {
            event.preventDefault();
        }

        var currentinput = $(this).val();
        
        if ((currentinput.indexOf('.') != -1 && currentinput.substring(currentinput.indexOf('.')).length > 2))
        {
            event.preventDefault();
        }
       
    });

});