 // Funktion för att radera en post och uppdatera sidan
 function deleteTraining(id) {
    if (confirm('Är du säker på att du vill radera denna post?')) {
        // Gör en AJAX-begäran för att radera posten
        fetch('/Home/Delete/' + id, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'X-Requested-With': 'XMLHttpRequest'
            }
        })
        .then(response => {
            if (!response.ok) {
                throw new Error('Något gick fel vid radering av posten.');
            }
            // Uppdatera sidan efter att posten har raderats
            location.reload();
        })
        .catch(error => {
            console.error(error.message);
        });
    }
}
// Funktion för att läsa in en post i formuläret för uppdatering
function ReadForm(i) {
    $.ajax({
        url: '/Home/ReadForm',
        type: 'GET',
        data: {
            id: i
        },
        dataType: 'json',
        success: function (response) {
            // Töm fälten
            $("#Training_Type").val('');
            $("#Training_Id").val('');
            $("#Training_Duration").val('');
            $("#Training_Distance").val('');
            $("#Training_Comment").val('');
            
            // Fyll i värden från responsen
            $("#Training_Type").val(response.type);
            $("#Training_Id").val(response.id);
            $("#Training_Duration").val(response.duration);
            $("#Training_Distance").val(response.distance);
            $("#Training_Comment").val(response.comment);

            // Uppdatera formulärets handling och knapptext
            $("#form-button").val("Spara");
            $("#form-action").attr("action", "/Home/Update");
        },
    });
}
