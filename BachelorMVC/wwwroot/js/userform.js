    //Variabler for å holde informasjon om antall bokser
    var AntallMedlemmer = 1;
    var boxName = 0;

    //Funksjon for å slette sist lagt inn input felt
    function removeLastInput() {
        var test = document.getElementById('EmailsForSigning');
        var childNode = test.lastChild;
        test.removeChild(childNode);
        AntallMedlemmer -= 1;
    }

     //Funksjon for å legge til ny input felt for emails
 function addInput() {
    if (AntallMedlemmer == 0) {
        AntallMedlemmer = 1;
    }
    var boxName = "Email " + AntallMedlemmer;
    document.getElementById('EmailsForSigning').innerHTML += '<input class="inputFields" type="text" tag="epost" id="' + AntallMedlemmer + '"/>';
    AntallMedlemmer += 1;
}

function getSignMethod() {
    var radios = document.getElementsByClassName("sign-method-radio");
    alert(radios);
    for (let radio of radios) {
        if(radio.checked) {
            alert(radio.value);
            return radio.value;
        }
    }
}