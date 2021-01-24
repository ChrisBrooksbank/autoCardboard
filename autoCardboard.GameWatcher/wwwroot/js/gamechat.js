"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/gamehub").build();

connection.on("ReceiveGameStatusMessage", function (statusMessage) {
    console.log("Got game status message");
});

connection.start().then(function () {
}).catch(function (err) {
    return console.error(err.toString());
});