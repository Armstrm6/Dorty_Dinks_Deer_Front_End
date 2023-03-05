// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

/* Fetch the GetAllDinksInformation API */
async function getAPI() {
    const response = await fetch("https://localhost:5001/api/Dinks/GetAllDinksInformation");
    return await response.json();
}

/* Initialize map */
async function initMap() {
    const dinks = await getAPI();
    const imageLink = "https://localhost:5001/api/Dinks/GetPicture?pictureId=";
    const mapBoundry = { lat: dinks[0].geoX, lng: dinks[0].geoY };
    const map = new google.maps.Map(document.getElementById("map"), {
        zoom: 5,
        center: mapBoundry,
    });

    /* Display each marker and update zoom */
    let bounds = new google.maps.LatLngBounds();
    console.log(dinks);
    for (let i = 0; i < dinks.length; i++) {
        let data = dinks[i];
        const latLng = new google.maps.LatLng(data.geoX, data.geoY);

        bounds.extend(latLng);

        let newMarker = new google.maps.Marker({
            position: latLng,
            map,
        });

        console.log(data.deerId);

        /* adding an info window */
        let content = ""
        for (let i = 0; i < data.pictureId.length; i++) {
            content+="<img style='width: 150px; height: 175px;'id='marker-image' src='" + imageLink + data.pictureId[i] + "'></img><br/>";
            console.log(content)
        }
        let newInfoWindow = new google.maps.InfoWindow({
            content: content
        });
        console.log(newInfoWindow.content);

        

        newMarker.addListener("click", () => {
            newInfoWindow.open({
                anchor: newMarker,
                map,
                shouldFocus: false,
            });
        });
    }
    map.fitBounds(bounds);
}

async function displayDinks() {
    let dinks = await getAPI();
    let ul = document.getElementById("list-group");
    console.log(dinks.length);
    for (let i = 0; i < dinks.length; i++) {
        let li = document.createElement("li");
        li.setAttribute('id', dinks[i].deerId);

        li.appendChild(document.createTextNode("Dinks hit on: " + dinks[i].deerDate.split("T")[0]));
        // pass deer id to edit action
        // in action, fetch get single dink
        // populate the modal
        li.innerHTML += `<div style="float: right;"><button class="btn btn-primary" id="button-edit" data-id="${dinks[i].deerId}">Edit</button><button class="btn btn-danger" id="button-delete" data-id="${dinks[i].deerId}">Delete</button></div>`;

        li.classList.add("list-group-item");

        ul.appendChild(li);
    }
}



/* Reset the modal on window close */ 
$('#myModal').on('hidden.bs.modal', function () {
    $(this).find('form').trigger('reset');
});

/* set the input fields to the corresponding dink */
$(document).on('click', '#button-edit', async function () {
    let dinkId = $(this).data('id');
    let response = await fetch(`https://localhost:5001/api/Dinks/GetSingleDink/${dinkId}?DeerID=${dinkId}`);
    let result = await response.json();
    const imageLink = "https://localhost:5001/api/Dinks/GetPicture?pictureId=";

    console.log($("#edit-lat").val())
    $("#dink-id").val(dinkId);
    $("#edit-lat").val(result.geoX);
    $("#edit-lng").val(result.geoY);
    $("#edit-date").val(result.deerDate.split("T")[0]);
    $('#editModal').modal('show');
    let content = document.createElement("li");

    for (let i = 0; i < result.pictureId.length; i++) {
        content.innerHTML += `<div id="picture${result.pictureId[i]}" style='position: relative;'><img  style='width: 150px; height: 175px;' src=${imageLink + result.pictureId[i]}></img><button id='dink-pic-btn' data-id="${result.pictureId[i]}" style='position: absolute; top: 0px; right: 0px;';>X</button></div>`;
    }

    content.style.listStyleType = "none";

    let div = document.getElementById("Edit-Images");
    div.textContent = "";
    div.appendChild(content);

});

$(document).on('click', '#dink-pic-btn', async function deleteDinkPicture() {
    let pictureId = $(this).data('id');
    let element = document.getElementById("picture"+pictureId);
    element.remove();

    await fetch(`https://localhost:5001/api/Dinks/DeletePicture/?pictureId=${pictureId}`,
        {
            method: "DELETE",
            headers: { 'Content-type': 'application/json; charset=UTF-8' }
        });

});


$(document).on('click', '#button-delete', async function deleteDink() {
    let dinkId = $(this).data('id');

    let element = document.getElementById(dinkId);
    element.remove();

    await fetch(`https://localhost:5001/api/Dinks/DeleteDinks/${dinkId}?deerId=${dinkId}`,
        {
            method: "DELETE",
            headers: { 'Content-type': 'application/json; charset=UTF-8' }
        });

});


displayDinks();
window.initMap = initMap;