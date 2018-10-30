
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


app.post('/TaskModify', function(req, res, next) 
{
	console.log("TaskModify req.body=" + JSON.stringify(req.body) );
	
	var updateSerial = req.body.UpdateSerial ;
	var requestSerial = req.body.RequestSerial ;
	
	if( updateSerial < gTemperalArray.length )
	{
		// need to fetch first 
		var contentObj = 
		{
			'UpdateSerial' : updateSerial
			,'RequestSerial' : requestSerial
		}

		respondObj = 
		{
			'Success':false
			,'Code':1
			,'Message':'Please fetch to the latest first'
			,'Key':'TaskModify'
			,'Content': JSON.stringify( contentObj )
		}

		res.json( respondObj ) ;
	}
	else
	{
		var projectKey = req.body.ProjectKey ;
		var taskBundle = req.body.Task ;
		
		var queryInfo = gDataBasePtr.query( 'SELECT * FROM tb_TaskBundles \
		WHERE ProjectKey = ? AND TaskIndex = ? ',
		[ projectKey , taskBundle.Data.TaskID ] , 
		function( err , rows , fields ) 
		{
			if( err )
			{
				throw err ;
			}
			
			if(0 == rows.length)
			{
				
				var contentObj = 
				{
					'UpdateSerial' : updateSerial
					,'RequestSerial' : requestSerial
				}

				respondObj = 
				{
					'Success':false
					,'Code':2
					,'Message':'TaskID does not exist.'
					,'Key':'TaskModify'
					,'Content': JSON.stringify( contentObj )
				}

				res.json( respondObj ) ;
			}
			else
			{
				gDataBasePtr.query('UPDATE tb_TaskBundles SET \
					( Title , ProjectKey , Assignee , TimeStamp \
					, ProgressInt ,ProgressFloat ,Link \
					, PositionStr , IsPin \
					, ParentID , RelativesStr, NeedFollowID ) VALUES \
					( ? , ? , ? , ? \
					, ? , ? , ? \
					, ? , ? \
					, ? , ? , ? ) WHERE ProjectKey = ? AND TaskIndex = ? ', 
					[ taskBundle.Data.Title , projectKey , taskBundle.Data.Assignee, taskBundle.Data.TimeStamp
					, taskBundle.Data.ProgressInt, taskBundle.Data.ProgressFloat, taskBundle.Data.Link 
					, taskBundle.Visual.PositionStr , taskBundle.Visual.IsPin 
					, taskBundle.Relation.ParentID , JSON.stringify(taskBundle.Relation.RelativesStr) , taskBundle.Relation.NeedFollowID 
					, projectKey , taskBundle.Data.TaskID 
					],
					function( iciErr , iciResult )
				{
					if ( iciErr ) 
					{
						throw iciErr ;
					}	
					
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
						,'Key':'TaskModify'
						,'Content': JSON.stringify( contentObj )
					}

					res.json( respondObj ) ;
					
				} ) ;
			}
		} ) ;
				
	}
	
	
	
	
} ) ;

app.post('/TaskAdd', function(req, res, next) 
{
	// console.log("req.get('Content')" + req.get('Content') );
	// console.log("req.get('Author')" + req.get('Author') );
	console.log("TaskAdd req.body=" + JSON.stringify(req.body) );
	/*	
req.body={"UpdateSerial":0,"RequestSerial":0,"Task":{"Data":{"TaskID":0,"Title":"","Assignee":"","TimeStamp":0,"ProgressInt":0,"ProgressFloat":0,"Link":""},"Visual":{"PositionStr":"","IsPin":false},"Relation":{"ParentID":0,"Relatives":[],"NeedFollowID":0},"Relative":{"ID":0,"Type":""}}}
	*/
	var updateSerial = req.body.UpdateSerial ;
	var requestSerial = req.body.RequestSerial ;
	var projectKey = req.body.ProjectKey ;
	var taskBundle = req.body.Task ;
	
	// check and insert
	gDataBasePtr.query('INSERT INTO tb_TaskBundles \
		( Title , ProjectKey , Assignee , TimeStamp \
		, ProgressInt ,ProgressFloat ,Link \
		, PositionStr , IsPin \
		, ParentID , RelativesStr, NeedFollowID ) VALUES \
		( ? , ? , ? , ? \
		, ? , ? , ? \
		, ? , ? \
		, ? , ? , ? )', 
		[ taskBundle.Data.Title , projectKey , taskBundle.Data.Assignee, taskBundle.Data.TimeStamp
		, taskBundle.Data.ProgressInt, taskBundle.Data.ProgressFloat, taskBundle.Data.Link 
		, taskBundle.Visual.PositionStr , taskBundle.Visual.IsPin 
		, taskBundle.Relation.ParentID , JSON.stringify(taskBundle.Relation.RelativesStr) , taskBundle.Relation.NeedFollowID 
		],
		function( iciErr , iciResult )
	{
		if ( iciErr ) 
		{
			throw iciErr ;
		}	
		
		taskBundle.Data.TaskID = iciResult.insertId ;
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
			,'Key':'TaskAdd'
			,'Content': JSON.stringify( contentObj )
		}

		res.json( respondObj ) ;
		
	}  ) ;
	
	
	
	
});


var gTemperalArray = [] ;

app.all('/FetchTasks', function(req, res, next) 
{
	// console.log("req.get('Content')" + req.get('Content') );
	// console.log("req.get('Author')" + req.get('Author') );
	console.log("FetchTasks req.body=" + JSON.stringify(req.body) );
	/*	
req.body={"UpdateSerial":0,"RequestSerial":0,"Task":{"Data":{"TaskID":0,"Title":"","Assignee":"","TimeStamp":0,"ProgressInt":0,"ProgressFloat":0,"Link":""},"Visual":{"PositionStr":"","IsPin":false},"Relation":{"ParentID":0,"Relatives":[],"NeedFollowID":0},"Relative":{"ID":0,"Type":""}}}
	*/
	var updateSerial = req.body.UpdateSerial ;
	var requestSerial = req.body.RequestSerial ;
	var projectKey = req.body.ProjectKey ;
	
	var contentObj = 
	{
		'UpdateSerial' : gTemperalArray.length 
		,'RequestSerial' : requestSerial
		, 'TaskVec' : [] 
	}

	if( updateSerial <= 0 )
	{
		// fetch all 
		
		var queryInfo = gDataBasePtr.query( 'SELECT * FROM tb_TaskBundles \
		WHERE ProjectKey = ?',
		[ projectKey ] , 
		function( err , rows , fields ) 
		{
			if( err )
			{
				throw err ;
			}
				
			for( var i = updateSerial ; i < rows.length ; ++i )
			{
				var taskBundleObj = 
				{
					'Data' : 
					{
						'TaskID' : rows[i].TaskIndex
						,'Title' : rows[i].Title
						,'Assignee' : rows[i].Assignee
						,'TimeStamp' : rows[i].TimeStamp
						,'ProgressInt' : rows[i].ProgressInt
						,'ProgressFloat' : rows[i].ProgressFloat
						,'Link' : rows[i].Link
					}
					,'Visual' :
					{
						'PositionStr' : rows[i].PositionStr
						,'IsPin' : rows[i].IsPin
					}
					,'Relation' :
					{
						'ParentID' : rows[i].ParentID
						,'RelativesStr' : rows[i].RelativesStr
						,'NeedFollowID' : rows[i].NeedFollowID
					}
				};
				contentObj.TaskVec.push(taskBundleObj);
			}
			
			respondObj = 
			{
				'Success':true
				,'Code':0
				,'Message':''
				,'Key':'FetchTasks'
				,'Content': JSON.stringify( contentObj )
			}
				
			res.json( respondObj ) ;
			
			console.log("rows.length=" + rows.length);
			
		} ) ;
	}
	else if( updateSerial < gTemperalArray.length )
	{
		
		for( var i = updateSerial ; i < gTemperalArray.length ; ++i )
		{
			contentObj.TaskVec.push( gTemperalArray[i] ) ;
		}
		
		console.log("gTemperalArray.length=" + gTemperalArray.length );		

		respondObj = 
		{
			'Success':true
			,'Code':0
			,'Message':''
			,'Key':'FetchTasks'
			,'Content': JSON.stringify( contentObj )
		}

		res.json( respondObj ) ;
	}

	
});

app.listen( 3102 );
console.log('Listening on port 3102...'  );



setInterval(function () 
{
	// console.log('setInterval gDataBasePtr.query...'  );
    gDataBasePtr.query('SELECT 1');
}, 5000);

