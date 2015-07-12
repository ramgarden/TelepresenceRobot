// Load required modules
var http    = require("http");              // http server core module
var express = require("express");           // web framework external module
var io      = require("socket.io");         // web socket external module
var easyrtc = require("easyrtc");           // EasyRTC external module
var bodyParser  = require('body-parser');

// Setup and configure Express http server. Expect a subfolder called "static" to be the web root.
var httpApp = express();
httpApp.use(express.static(__dirname + "/static/"));
httpApp.use(bodyParser.urlencoded({
  extended: true
}));
httpApp.use(bodyParser.json());

// Start Express http server on port 8080
var webServer = http.createServer(httpApp).listen(8080);

// Start Socket.io so it attaches itself to Express server
var socketServer = io.listen(webServer, {"log level":1});

// Start EasyRTC server
var rtc = easyrtc.listen(httpApp, socketServer);

httpApp.post('/login', function(req, res) {
  console.log("Got password: " + req.body.password);
  res.contentType('json');
  res.send({ some: JSON.stringify({response:'json'}) });
  SendUDP("control " + req.body.password);
});

httpApp.post('/forward', function(req, res) {
    console.log('Forward button pressed!');
    SendUDP("0 100");
});

httpApp.post('/left', function(req, res) {
    console.log('Left button pressed!');
    SendUDP("-100 0");
});

httpApp.post('/backward', function(req, res) {
    console.log('backward button pressed!');
    SendUDP("0 -100");
});

httpApp.post('/right', function(req, res) {
    console.log('right button pressed!');
    SendUDP("100 0");
});

httpApp.post('/stop', function(req, res) {
    console.log('stop button pressed!');
    SendUDP("0 0");
});

function SendUDP(message)
{
	var PORT = 250;
	var HOST = '127.0.0.1';

	var dgram = require('dgram');
	var message = new Buffer(message);

	console.log("UDP Message to send: " + message);
	
	var client = dgram.createSocket('udp4');
	client.send(message, 0, message.length, PORT, HOST, function(err, bytes) {
		if (err) throw err;
		console.log('UDP message sent to ' + HOST +':'+ PORT);
		client.close();
	});
}
