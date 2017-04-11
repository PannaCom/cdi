(function($){
  $(function(){

    $('.button-collapse').sideNav();
    $('.parallax').parallax();
    
    $('#booking_type_vehicle').material_select();
    $('#booking_type').material_select();


  }); // end of document ready
})(jQuery); // end of jQuery name space


function checkemail(email) {
    $.ajax({
        url: '/Home/checkemailexist',
        data: { email: email },
        type: 'POST',
        dataType: 'json',
        cache: false,
        success: function (data) {
            checking(data);
        }
    })
}

function checkphone(phone) {
    $.ajax({
        url: '/Home/checkphoneexist',
        data: { phone: phone },
        type: 'POST',
        dataType: 'json',
        cache: false,
        success: function (data) {
            checking(data);
        }
    })
}

function checking(x) {
    console.log(x);
    if (x === '1') {
        return true;
    } else if(x === '0') {
        return false;
    }
}

function adderror(selector, error) {
    $('label[for="' + selector + '"]').attr('data-error', error);
    $('#' + selector).removeClass("valid").addClass("invalid");
    $('#' + selector).addClass("invalid");
}

function removeerror(selector) {
    $('label[for="' + selector + '"]').removeAttr('data-error');
    $('#' + selector).removeClass("invalid");
    $('#' + selector).addClass("valid");
}