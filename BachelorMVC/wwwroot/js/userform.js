//Variabler for å holde informasjon om antall bokser
var AntallMedlemmer = 1;
var boxName = 0;
var emailInputs = [];

//Funksjon for å slette sist lagt inn input felt
function removeLastInput() {
    emailInputs.pop();
    populateEmails();
}

//Fjern spesifikk epost-input
function removeInputAt(index) {
    if (index === emailInputs.length) {
        emailInputs.pop();
    } else  {
        emailInputs.splice(index, 1);
    }
    populateEmails();
}

//Funksjon for å legge til ny input felt for emails
function addInput() {
    if (AntallMedlemmer == 0) {
        AntallMedlemmer = 1;
    }
    let input = document.createElement("INPUT");
    input.setAttribute("placeholder", "Epost");
    input.setAttribute("type", "email");
    input.setAttribute("class", "textfield inputFields");

    /* knapper for å slette spesifikke inputbokser, planlagt feature
    let del = document.createElement("DIV");
    del.setAttribute("id", "delete-email-input");
    del.setAttribute("name", emailInputs.length);
    del.setAttribute("onclick", "removeInputAt(" + emailInputs.length + ")");

    let container = document.createElement("DIV");
    container.setAttribute("id", "container-email-input");
    container.appendChild(input);
    container.appendChild(del);
    */emailInputs.push(input);

    populateEmails();
}


function populateEmails() {
    var emailContainer = document.getElementById("EmailsForSigning");
    emailContainer.innerHTML = "";
    for (let input of emailInputs) {
        emailContainer.appendChild(input);
    }
}

function getSignMethod() {
    var radios = document.getElementsByClassName("sign-method-radio");
    for (let radio of radios) {
        if(radio.checked) {
            return radio.value;
        }
    }
}

//Validerer skjema før info sendes til backend
function validated() {
    //fildocument.getElementById("input-upload").files[0].type
    let userFile = document.getElementById("input-upload");
    let selfsignChk = document.getElementById("checkbox-self-sign");
    let emails = document.getElementsByClassName("inputFields");

    //sjekk om fil er lastet opp
    if(userFile.files.length === 0) {
        alert("Du må laste opp en fil");
        return false;
    }

    //sjekk MIME type
    if(!(userFile.files[0].type === "application/pdf")) {
        alert("feil filtype");
        return false;
    }

    //sjekk størrelse
    if((userFile.size > 5000000)) {
        alert("filen er for stor");
        return false;
    }

    //eposter
    //sjekk for minst 1 epost
    if(emails.length === 0 && !(selfsignChk.checked)) {
        alert("Minst 1 epost kreves");
        return false;
    }

    return true;
}

function ShowModalLearnMore() {
    document.getElementById("learn-more").hidden = false;
}

