// Load required modules
var module = {};
var SerialPort = require("serialport");     // SerialPort external module
//var robot = require("create-oi");           // iRobot Create Open Interface
const Readline = SerialPort.parsers.Readline;
const parser = new Readline();

var startFull = new Buffer(2);
startFull[0] = 0x80;
startFull[1] = 0x84;

var demoTwo = new Buffer(2);
demoTwo[0] = 0x88;
demoTwo[1] = 0x09;

//robot serial buffers, etc.
var messageBuffer = new Buffer(4);
messageBuffer[0] = 0x80;
messageBuffer[1] = 0x84;
messageBuffer[2] = 0x88;
messageBuffer[3] = 0x02;

// Start up the Serial Port for talking directly to the iRobot Create (TM)
var SerialPort = require('serialport');
var port = new SerialPort('COM4',{
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
  setTimeout(StartInFull,1000);
  setTimeout(DoDemo2,5000);
});
port.pipe(parser);
parser.on('data',console.log);


/*
port.on('data', function(data) {
	console.log('Data: ', data);
});
*/

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


function writeSerial(message) {
	port.write(message, function(err) {
	if (err) {
	  return console.log('Error on write: ', err.message);
	}
	console.log('message written:\n' + JSON.stringify(message));
	});
}

//handle all the post events from the client web page
// for drive commands, etc.
