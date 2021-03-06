var apiBaseUrl = "http://localhost:7071";

function login(form){
    var emailAddress = form.emailAddress.value;

    var request = new XMLHttpRequest();
    request.open('POST', `${apiBaseUrl}/api/GenerateJWT`, true);

    let data = {
        "EmailAddress": emailAddress
    };

    request.send(JSON.stringify(data));

    request.onload = function () {
        var resp = this.response;
        localStorage.setItem('JWT', resp);
        redirectToHome();
    };

    request.onerror = function () { };
}