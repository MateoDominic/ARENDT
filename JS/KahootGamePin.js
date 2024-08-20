document.addEventListener('DOMContentLoaded', function(){

    
const socket = new WebSocket('ws://localhost:8181');

//otvaranje veze
socket.onopen = function(event){
    console.log('Connected to ws server');
    requestSessionCode();
}


//primajnje poruke od servera
socket.onmessage = function(event){
    const data = JSON.parse(event.data);
    if(data.sessionCode){
        console.log('Recive session code:'. data.sessionCode);
        displaySessionCode(data.sessionCode); //prikazivanje koda na stranici
    }
};

//zatvaranje veze
socket.onclose = function(event){
    console.log('ws connection closed');
}

//greska u vezi
socket.onerror = function(error){
    console.log('ws error:', error);
}


function requestSessionCode(){
    const request ={
        action: "getSessionCode"
    };

    socket.send(JSON.stringify(request));
}

function displaySessionCode(code){
    const codeElement = document.getElementById('session-code');
    codeElement.textContent = code;
}









})




