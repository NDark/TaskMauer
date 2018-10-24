﻿
var gMySQLPtr = require('mysql');
var express = require('express');
var passport = require('passport'),
	LocalStrategy = require('passport-local').Strategy;
var dateFormat = require('dateformat');
var app = express();


app.configure(function() 
{
	app.use(express.cookieParser());
	app.use(express.bodyParser());
	app.use(express.session({ secret: 'alpha' }));
	app.use(passport.initialize());
	app.use(passport.session());
	app.use(app.router);
});

var gDataBasePtr = null ;

function gHandleMySQLDisconnect() 
{
	console.log('gHandleMySQLDisconnect() start');
	
	gDataBasePtr = gMySQLPtr.createConnection(
		{
			host:'localhost',
			user: 'root',
			password: 'kk2015',
			database: 'taskmauerserver',
			port:3306
		}
	);

  gDataBasePtr.connect(function(err) {              // The server is either down
    if(err) {                                     // or restarting (takes a while sometimes).
      console.log('error when connecting to db:', err);
      setTimeout( gHandleMySQLDisconnect , 2000); // We introduce a delay before attempting to reconnect,
    }                                     // to avoid a hot loop, and to allow our node script to
  });                                     // process asynchronous requests in the meantime.
                                          // If you're also serving http, display a 503 error.
  gDataBasePtr.on('error', function(err) 
  {
    console.log('db error: ', err);
	
	if(err.code == 'PROTOCOL_CONNECTION_LOST') 
	{
		console.log('err.code == PROTOCOL_CONNECTION_LOST');
	}

    if(err.code === 'PROTOCOL_CONNECTION_LOST') 
	{ 
		// Connection to the MySQL server is usually
		// lost due to either server restart, or a
		// connnection idle timeout (the wait_timeout
		// server variable configures this)
		console.log('restart gHandleMySQLDisconnect()');
		gHandleMySQLDisconnect();                     
    } 
	else 
	{                                      
		console.log('throw err');
      throw err;                                  
    }
  });
}


gHandleMySQLDisconnect() ;


app.all('/test', function(req, res, next) 
{
    res.send(401, "this is test.txt");
    // console.log("testtxt connected");
	console.log( "url=" + req.url ) ;
});

app.post('/TaskAdd', function(req, res, next) 
{
	// console.log("req.get('Content')" + req.get('Content') );
	// console.log("req.get('Author')" + req.get('Author') );
	console.log("req.body=" + JSON.stringify(req.body) );
	/*	
req.body={"UpdateSerial":0,"RequestSerial":0,"Task":{"Data":{"TaskID":0,"Title":"","Assignee":"","TimeStamp":0,"ProgressInt":0,"ProgressFloat":0,"Link":""},"Visual":{"PositionStr":"","IsPin":false},"Relation":{"ParentID":0,"Relatives":[],"NeedFollowID":0},"Relative":{"ID":0,"Type":""}}}
	*/
	var updateSerial = req.body.UpdateSerial ;
	var requestSerial = req.body.RequestSerial ;
	var taskBundle = req.body.Task ;
	// check and insert
	// generate a new update serial
	if( gMaxTaskID )
	{
		// find the max task id in the table 
		gMaxTaskID = 1;
	}
	
	++gMaxTaskID;
	
	taskBundle.Data.TaskID = gMaxTaskID ;
	gTemperalArray.push(taskBundle) ;
	
	var contentObj = 
	{
		'UpdateSerial' : updateSerial
		,'RequestSerial' : requestSerial
	}

	respondObj = 
	{
		'Success':true
		,'Code':0
		,'Message':''
		,'Key':''
		,'Content': JSON.stringify( contentObj )
	}

	res.json( respondObj ) ;
	
});

var gMaxTaskID = 0 ;
var gTemperalArray = [] ;

app.all('/FetchTasks', function(req, res, next) 
{
	// console.log("req.get('Content')" + req.get('Content') );
	// console.log("req.get('Author')" + req.get('Author') );
	console.log("req.body=" + JSON.stringify(req.body) );
	/*	
req.body={"UpdateSerial":0,"RequestSerial":0,"Task":{"Data":{"TaskID":0,"Title":"","Assignee":"","TimeStamp":0,"ProgressInt":0,"ProgressFloat":0,"Link":""},"Visual":{"PositionStr":"","IsPin":false},"Relation":{"ParentID":0,"Relatives":[],"NeedFollowID":0},"Relative":{"ID":0,"Type":""}}}
	*/
	var updateSerial = req.body.UpdateSerial ;
	var requestSerial = req.body.RequestSerial ;
	
	var contentObj = 
	{
		'UpdateSerial' : updateSerial
		,'RequestSerial' : requestSerial
		, 'TaskVec' : [] 
	}
	
	if( updateSerial == -1 )
	{
		// fetch all 
	}
	else if( updateSerial < gTemperalArray.length )
	{
		
		for( var i = updateSerial ; i < gTemperalArray.length ; ++i )
		{
			contentObj.TaskVec.push( gTemperalArray[i] ) ;
		}
		
		contentObj.UpdateSerial = gTemperalArray.length ;
	}
	// check and insert
	// generate a new update serial

	respondObj = 
	{
		'Success':true
		,'Code':0
		,'Message':''
		,'Key':''
		,'Content': JSON.stringify( contentObj )
	}

	res.json( respondObj ) ;
	
});

app.listen( 3102 );
console.log('Listening on port 3102...'  );



setInterval(function () 
{
	// console.log('setInterval gDataBasePtr.query...'  );
    gDataBasePtr.query('SELECT 1');
}, 5000);

