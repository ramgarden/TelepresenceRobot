// Load required modules
var http    = require("http");              // http server core module
var express = require("express");           // web framework external module
var io      = require("socket.io");         // web socket external module
var easyrtc = require("easyrtc");           // EasyRTC external module
var bodyParser  = require('body-parser');
var SerialPort = require("serialport");     // SerialPort external module
//var robot = require("create-oi");           // iRobot Create Open Interface
 
 
//iRobot OpCodes
var Start = 128;
var Full = 132;
var Demo = 136;
var Demo2 = 2;

//robot serial buffers, etc.
var messageBuffer = new ArrayBuffer(64);
var messageIndex = 0;

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



// Start up the Serial Port for talking directly to the iRobot Create (TM)
var SerialPort = require('serialport');
var port = new SerialPort('COM4', function (err) {
  if (err) {
    return console.log('Error: ', err.message);
  }
  var opByte = parseInt(Start) & 255;
  port.write(opByte, function(err) {
    if (err) {
      return console.log('Error on write: ', err.message);
    }
    console.log('message written');
  });
  console.log("doing demo 2!");
  DoDemo2();
});


function QueueCommand(op, param1)
{
	var opByte = parseInt(op) & 255;
	var param1Byte = parseInt(param1) & 255;
	
	messageBuffer[messageIndex] = opByte;
	messageIndex++;
	messageBuffer[messageIndex] = param1Byte;
	messageIndex++;
}

function QueueCommand(op)
{
	var opByte = parseInt(op) & 255;
	messageBuffer[messageIndex] = opByte;
	messageIndex++;
}

function StartInFullMode()
{
	QueueCommand(Start);
	QueueCommand(Full);
	SendCommand();
}

function DoDemo2()
{
	console.log("queueing demo command.");
	QueueCommand(Demo, Demo2);
	console.log("sending command...");
	SendCommand();
}

function SendCommand()
{
	try
	{
		console.log("writing to serial...");
		writeSerial(messageBuffer);
	}
	catch (err)
	{
		console.log(err.message);
	}
	messageIndex = 0;
}


function writeSerial(message) {
	port.write(message, function(err) {
	if (err) {
	  return console.log('Error on write: ', err.message);
	}
	console.log('message written:\n' + message);
	});
}

//handle all the post events from the client web page
// for drive commands, etc.

httpApp.post('/login', function(req, res) {
  console.log("Got password: " + req.body.password);
  res.contentType('json');
  res.send({ some: JSON.stringify({response:'json'}) });
  SendUDP("control " + req.body.password);
});

httpApp.post('/forward', function(req, res) {
    console.log('Forward button pressed!');
	//125 -25  W
    SendUDP("125 -25");
	robot.on('ready', function() {
		this.drive(100,0);
	});
});

httpApp.post('/left', function(req, res) {
    console.log('Left button pressed!');
	//-25 125  A
    SendUDP("-25 125");
});

httpApp.post('/backward', function(req, res) {
    console.log('backward button pressed!');
	//125 275  S
    SendUDP("125 275");
});

httpApp.post('/right', function(req, res) {
    console.log('right button pressed!');
	//275 125  D
    SendUDP("275 125");
});

httpApp.post('/stop', function(req, res) {
    console.log('stop button pressed!');
	//125 125  stop
    SendUDP("125 125");
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
