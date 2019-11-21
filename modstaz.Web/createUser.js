function createUser(form) {
    var emailAddress = form.emailAddress.value;

    console.log("Creating User!");
    var request = new XMLHttpRequest();
    request.open('POST', `${apiBaseUrl}/api/CreateUser`, true);

    let data = {
        "EmailAddress": emailAddress
    };

    request.send(JSON.stringify(data));

    request.onload = function () {
        var resp = this.response;
        console.log(resp);
    };

    request.onerror = function () { };
}