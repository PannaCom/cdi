(function($){
  $(function(){

    $('.button-collapse').sideNav();
    $('.parallax').parallax();
    
    $('#booking_type_vehicle').material_select();
    $('#booking_type').material_select();


  }); // end of document ready
})(jQuery); // end of jQuery name space

//function myCallback(result) {
//    // Code that depends on 'result'
//}

//foo(myCallback);

function checkemail(email) {       
    var jqXHR = $.ajax({
        url: '/Home/checkemailexist',
        data: { email: email },
        type: 'POST',
        dataType: 'json',
        cache: false,
        async: false
    })
    return checking(jqXHR.responseText);
}

function checkphone(phone) {
    var jqXHR = $.ajax({
        url: '/Home/checkphoneexist',
        data: { phone: phone },
        type: 'POST',
        dataType: 'json',
        cache: false,
        async: false
    })
    return checking(jqXHR.responseText);
}

function checking(x) {
    if (x === '1') {
        return true;
    } else if(x === '0') {
        return false;
    }
}

function adderror(selector, error) {
    $('label[for="' + selector + '"]').attr('data-error', error);
    $('#' + selector).addClass("invalid").removeClass("valid");
}

function removeerror(selector) {
    $('label[for="' + selector + '"]').removeAttr('data-error');
    $('#' + selector).addClass("valid").removeClass("invalid");
}

function timediff(startTime, endTime) {
    //var startTime = "05:01:20";
    //var endTime = "09:00:00";
    //var regExp = /(\d{1,2})\:(\d{1,2})\:(\d{1,2})/;
    var regExp = /(\d{1,2})\:(\d{1,2})\//;
    return parseInt(endTime.replace(regExp, "$1$2")) - parseInt(startTime.replace(regExp, "$1$2"));
}

function timediff3(startTime, endTime) {
    //var startTime = "05:01:20";
    //var endTime = "09:00:00";
    var regExp = /(\d{1,2})\:(\d{1,2})\:(\d{1,2})/;
    return parseInt(endTime.replace(regExp, "$1$2$3")) - parseInt(startTime.replace(regExp, "$1$2$3"));
}