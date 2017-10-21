// Load required modules
var module = {};
var http    = require("http");              // http server core module
var express = require("express");           // web framework external module
var io      = require("socket.io");         // web socket external module
var easyrtc = require("easyrtc");           // EasyRTC external module
var bodyParser  = require('body-parser');
var SerialPort = require("serialport");     // SerialPort external module
//var robot = require("create-oi");           // iRobot Create Open Interface
const Readline = SerialPort.parsers.Readline;
const parser = new Readline();
 
//Set your password here!
var password = "pass";
var loggedIn = false; 
 
//iRobot OpCodes
var Start = 128; //0x80; //128
var Full = 132; //0x84; //132
var Demo = 136; //0x88; //136
var Demo2 = 2; //0x02; //2
var Drive = 137; //0x89; //137
var DriveDirect = 145; //0x91; //145

//robot normal speed 0-500 (mm/s)
var wheelSpeed = 0x62;

var startFull = new Buffer(2);
startFull[0] = 0x80;
startFull[1] = 0x84;

//note the robot is mounted to the laptop
//backwards so we use negative speed to
//drive backwards when going forward.
var driveForward = new Buffer(5);
driveForward[0] = 0x91;
driveForward[1] = (-wheelSpeed) >> 8;
driveForward[2] = (-wheelSpeed) & 255;
driveForward[3] = (-wheelSpeed) >> 8;
driveForward[4] = (-wheelSpeed) & 255;

var driveBackward = new Buffer(5);
driveBackward[0] = 0x91;
driveBackward[1] = (wheelSpeed) >> 8;
driveBackward[2] = (wheelSpeed) & 255;
driveBackward[3] = (wheelSpeed) >> 8;
driveBackward[4] = (wheelSpeed) & 255;

var turnLeft = new Buffer(5);
turnLeft[0] = 0x91;
turnLeft[1] = (wheelSpeed) >> 8;
turnLeft[2] = (wheelSpeed) & 255;
turnLeft[3] = (-wheelSpeed) >> 8;
turnLeft[4] = (-wheelSpeed) & 255;

var turnRight = new Buffer(5);
turnRight[0] = 0x91;
turnRight[1] = (-wheelSpeed) >> 8;
turnRight[2] = (-wheelSpeed) & 255;
turnRight[3] = (wheelSpeed) >> 8;
turnRight[4] = (wheelSpeed) & 255;

var stop = new Buffer(5);
stop[0] = 0x91;
stop[1] = 0x00 >> 8;
stop[2] = 0x00 & 255;
stop[3] = 0x00 >> 8;
stop[4] = 0x00 & 255;


var demoTwo = new Buffer(2);
demoTwo[0] = 0x88;
demoTwo[1] = 0x02;

//robot serial buffers, etc.
<<<<<<< HEAD
global.messageBuffer = new Buffer(8);
messageBuffer[0] = 0x80;
messageBuffer[1] = 0x84;
messageBuffer[2] = 0x88;
messageBuffer[3] = 0x02;

global.messageIndex = 0;

=======
var messageBuffer = new Buffer(64);
var messageIndex = 0;
>>>>>>> origin/master

// Setup and configure Express http server. Expect a subfolder called "static" to be the web root.
var httpApp = express();
httpApp.use(express.static(__dirname + "/static/"));
console.log(__dirname + "/static/");
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
var port = new SerialPort('COM6',{
  baudRate: 57600,
  dataBits: 8,
  stopBits: 1,
  hupcl: false,
  rtscts: false,
  parity: 'none'
}, function (err) {
  if (err) {
    return console.log('Error: ', err.message);
  }
  // console.log("doing demo 2!");
  //DoDemo2();
  StartInFull();
  //setTimeout(StartInFull,2000);
  //setTimeout(DoDemo2,5000);
});
port.pipe(parser);
parser.on('data',console.log);


/*
port.on('data', function(data) {
	console.log('Data: ', data);
});
*/
function QueueCommand(op, param1, param2)
{
	var opByte = dec2bin(op);
	var param1Byte = dec2bin(param1);
	var param2Byte = dec2bin(param2);
	
	global.messageBuffer[global.messageIndex] = opByte;
	global.messageIndex++;
	global.messageBuffer[global.messageIndex] = param1Byte >> 8;
	global.messageIndex++;
	global.messageBuffer[global.messageIndex] = param1Byte & 255;
	global.messageIndex++;
	global.messageBuffer[global.messageIndex] = param2Byte >> 8;
	global.messageIndex++;
	global.messageBuffer[global.messageIndex] = param2Byte & 255;
	global.messageIndex++;
	console.log("messageIndex :" + global.messageIndex);
}

function QueueCommand(op, param1)
{
	var opByte = dec2bin(op);
	var param1Byte = dec2bin(param1);
	
	global.messageBuffer[global.messageIndex] = opByte;
	global.messageIndex++;
	global.messageBuffer[global.messageIndex] = param1Byte >> 8;
	global.messageIndex++;
	global.messageBuffer[global.messageIndex] = param1Byte & 255;
	global.messageIndex++;
	console.log("messageIndex :" + global.messageIndex);
}

function QueueCommand(op)
{
	var opByte = dec2bin(op)
	global.messageBuffer[global.messageIndex] = opByte;
	global.messageIndex++;
	console.log("messageIndex :" + global.messageIndex);
}

function StartInFullMode()
{
	QueueCommand(Start);
	SendCommand();
	QueueCommand(Full);
	SendCommand();
}

function StartInFull() {
	writeSerial(startFull);
}

function DoDemo2()
{
	writeSerial(demoTwo);
	/*
	StartInFullMode();
	console.log("queueing demo command.");
	QueueCommand(Demo);
	SendCommand();
	QueueCommand(Demo2);
	console.log("sending command...");
	SendCommand();
	*/
}

function DriveForward()
{
	writeSerial(driveForward);
	//QueueCommand(DriveDirect, 10, 10);
	//SendCommand();
}

function DriveBackward()
{
	writeSerial(driveBackward);
}

function TurnLeft()
{
	writeSerial(turnLeft);
}

function TurnRight()
{	
	writeSerial(turnRight);
}

function Stop()
{
	writeSerial(stop);
	//QueueCommand(DriveDirect, 0, 0);
	//SendCommand();
}

function SendCommand()
{
	try
	{
		//writeSerial(startFull);
			/*module.wait(1);*/
		//writeSerial(demoTwo);
		console.log("sending " + global.messageIndex + " bytes.");
		var sendBuffer = global.messageBuffer.slice(0, global.messageIndex);
		writeSerial(sendBuffer);
	}
	catch (err)
	{
		console.log(err.message);
	}
	global.messageIndex = 0;
}

function dec2bin(dec)
{
	return parseInt(dec,10) & 255;
}


function writeSerial(message) {
	port.write(message, function(err) {
	if (err) {
	  return console.log('Error on write: ', err.message);
	}
	console.log('message written:\n' + JSON.stringify(message));
	});
}

//////////////////////////////////////////////////////
//handle all the post events from the client web page
// for drive commands, etc.
//////////////////////////////////////////////////////

httpApp.post('/login', function(req, res) {
  console.log("Got password: " + req.body.password);
  res.contentType('json');
  res.send({ some: JSON.stringify({response:'json'}) });
  //SendUDP("control " + req.body.password);
  console.log(password + " == " + req.body.password);
  loggedIn = password == req.body.password;
});

httpApp.post('/forward', function(req, res) {
    console.log('Forward button pressed!');
	//125 -25  W
    //SendUDP("125 -25");
	DriveForward();
});

httpApp.post('/left', function(req, res) {
    console.log('Left button pressed!');
	//-25 125  A
    //SendUDP("-25 125");
	TurnLeft();
});

httpApp.post('/backward', function(req, res) {
    console.log('backward button pressed!');
	//125 275  S
    //SendUDP("125 275");
	DriveBackward();
});

httpApp.post('/right', function(req, res) {
    console.log('right button pressed!');
	//275 125  D
    //SendUDP("275 125");
	TurnRight();
});

httpApp.post('/stop', function(req, res) {
    console.log('stop button pressed!');
	//125 125  stop
    //SendUDP("125 125");
	Stop();
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
