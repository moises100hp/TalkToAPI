function TesteCors() {
    var tokenJWT = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6Im1hcmlhQGdtYWlsLmNvbSIsInN1YiI6Ijk0OTQxZDdiLWIzOTctNDMzYS05ODM2LTkzOWI4NzcyMDA4OCIsImV4cCI6MTYzNjU5NDcxMX0.jMMb230vLBq9dSKjEXgoZCA_irVgAfUskDrp37E3wGo";
    var servico = "https://localhost:44371/api/mensagem/918da4f5-fd6d-4c6d-9c82-3a290a063860/94941d7b-b397-433a-9836-939b87720088";
    $("#resultado").html("--Solicitando--");
    $.ajax({
        url: servico,
        method: "GET",
        crossDomain: true,
        headers: { "Accept": "application/json" },
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Authorization", "Bearer" + tokenJWT);
        },
        success: function (data, status, xhr) {
            $("#resultado").html(data);
        }
    });
}