"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("https://localhost:44387/gamehub").build();

connection.on("ReceiveGameState", function (gameState) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = "Got GameState";
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.start().then(function () {
}).catch(function (err) {
    return console.error(err.toString());
});