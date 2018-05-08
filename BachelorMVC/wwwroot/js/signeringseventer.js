

   //Etter dokumentopplasting kan klient kjøre denne funksjonen for å opprette signeringsoppdrag hos Assently
   function OpprettSigneringsOppdrag(url) {
    var fields = document.getElementById('EmailsForSigning').childNodes;
    
    //Ta imot navn på signeringsoppdrag
    let navn = document.getElementById('case-navn').value;

    //Ta imot eposter
    var emails = "";
    for (var i = 1, len = fields.length; i < len; i++) {
            emails += fields[i].value + ",";
    }

    //Hent filnavn
    var dokumentNavn = $('input[type=file]').val().split('\\').pop()


    //Send oppdrag til backend for videre behandling
$.ajax({
    type: 'POST',
    data: { epost: emails, caseNavn: navn, dokumentNavn: dokumentNavn},
    dataType: 'json',
    url: url,
    traditional: true,
    success: function (data) {
        alert("sendt");
     },
    error: function (XMLHttpRequest, textStatus, errorThrown) {
        alert(errorThrown);
    }

    });

}

// Klient kan laste opp et dokument til backend. 
function uploadDocument() {
    let fileSelect = document.getElementById('file-upload-input');
    let uploadButton = document.getElementById('file-upload-button');


    //Hent objekt som FileList
    //Løsning for enkel filopplasting
    var form = $('file-upload-form');
    var formData = new FormData(form);
    alert(formData);


    //todo: løsning for flere filer
    $.ajax({
        type: 'POST',
        url: '@Url.Action("Upload", "Home")',
        data: formData,
        dataType: 'json',
        contentType: 'false',
        processData: 'false',
        cache: 'false',
        success: function (data) {
            alert("sendt");
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(errorThrown);
        }

    });

}