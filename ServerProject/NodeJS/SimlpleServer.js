
var gMySQLPtr = require('mysql');
var express = require('express');
var passport = require('passport'),
	LocalStrategy = require('passport-local').Strategy;
var dateFormat = require('dateformat');
var app = express();


Date.prototype.getWeek = function() {
  var date = new Date(this.getTime());
   date.setHours(0, 0, 0, 0);

  date.setDate(date.getDate() + 3 - (date.getDay() + 6) % 7);

  var week1 = new Date(date.getFullYear(), 0, 4);
  return 1 + Math.round(((date.getTime() - week1.getTime()) / 86400000
                        - 3 + (week1.getDay() + 6) % 7) / 7);
}

var activiyDateTable = 
{
'20180913': 'AnniversarySale'
,'20181006': 'KpopSinger'
,'20181017': 'HikingActivity'
,'20181221': 'FilmPremiere'
,'20180211': 'SemesterBegin'
};

var activityKeyArray = [ 
'SemesterBegin' 
,'FilmPremiere' 
,'KpopSinger' 
,'AnniversarySale' 
,'HikingActivity' ];

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
			database: 'reallyserver',
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


/**
取得系統資訊
*/
app.all('/ServerInfo', 
function(req, res, next) 
{
	respondObj = 
	{
		'Success':true,
		'Code':0,
		'Message':'',
		'Key':'ServerInfo',
		'Content':''
		,'ServerVersion':'20180912'
		,'NeedToUpdateClientVersion':'1.0.84'
		,'RecommendToUpdateClientVersion':'1.1'
		,'StrategyNum':'280'
	}
	
	res.json( respondObj ) ;
} ) ;


/**
版本資訊
*/
app.all('/VersionTable', 
function(req, res, next) 
{
	respondObj = 
	{
		'Success':true
		,'Code':0
		,'Message':''
		,'Key':'VersionTable'
		,'Content':'{"List":[{"Key":"ab_localization","Version":4},{"Key":"ab_customerresponses","Version":2},{"Key":"ab_strategylist","Version":2},{"Key":"Android","Version":2},{"Key":"iOS","Version":2}]}'
	}
	
	res.json( respondObj ) ;
} ) ;

/**
日曆資訊
*/
app.all('/CalanderInfo', 
function(req, res, next) 
{
	var today = new Date(); // Today!
	var yesterday = new Date(); 
	yesterday.setDate(today.getDate() - 1); // Yesterday!

    var todayStr = dateFormat(today, "yyyymmdd");

	var dataObj = 
	{
		Valid : true 
		, DateKey : todayStr
		,Days : []
	};
	
	for( var i = 0 ; i < 4 ; ++i)
	{
		var day = new Date();
		day.setDate(yesterday.getDate() + i );
		
		var dateStr=dateFormat(day, "yyyymmdd");
		var weeknum = day.getWeek();
		
		if(activiyDateTable[dateStr])
		{
			dataObj.Days[i]={Valid:true ,ActivityKey:activiyDateTable[dateStr]};
		}
		else if( 3 == day.getDay() )// check wed
		{
			var index = weeknum*2+1 ;
			index= index % activityKeyArray.length;
			dataObj.Days[i]={Valid:true ,ActivityKey:activityKeyArray[index]};
		}
		else if( 0 == day.getDay() )// check Sun
		{
			var index = weeknum*2 ;
			index= index % activityKeyArray.length;
			dataObj.Days[i]={Valid:true ,ActivityKey:activityKeyArray[index]};
		}
		else
		{
			dataObj.Days[i]={Valid:false ,ActivityKey:''};
		}
	}
	
	respondObj = 
	{
		'Success':true,
		'Code':0,
		'Message':'',
		'Key':'CalanderInfo',
		'Content':JSON.stringify(dataObj)
	}
	
	res.json( respondObj ) ;
} ) ;


app.all('/test', function(req, res, next) 
{
    res.send(401, "this is test.txt");
    // console.log("testtxt connected");
	console.log( "url=" + req.url ) ;
});


app.all('/UploadContentTest', function(req, res, next) 
{
	gDataBasePtr.query('INSERT INTO tb_UploadStrategies \
		( Content , \
			Author, \
		UploadTime ) VALUES \
		( ?, ?, ? )', 
		[ 'content' ,
		'author' ,
		new Date() ],
		function( iciErr , iciResult )
	{
		if ( iciErr ) 
		{
			throw iciErr ;
		}	
		
		var queryInfo = gDataBasePtr.query( 'SELECT * FROM tb_UploadStrategies',
		[ ] , 
		function( err , rows , fields ) 
		{
			if( err )
			{
				throw err ;
			}
			
			respondObj = 
			{
				'Success':true
				,'Code':0
				,'Message':''
				,'Key':''
				,'Content':JSON.stringify( rows.length )
			}
			
			res.json( respondObj ) ;
			
			console.log("UploadContent rows.length=" + rows.length);
			
		} ) ;
		
	} ) ;
		
	
   
});



app.post('/UploadContent', function(req, res, next) 
{
	// console.log("req.get('Content')" + req.get('Content') );
	// console.log("req.get('Author')" + req.get('Author') );
	
	// console.log("req.body=" + JSON.stringify(req.body) );
	
	
	var contnet = req.body.Content ;
	var author = req.body.Author ;
	if( 'undefined'==typeof(author) )
	{
		author = '';
	}
	
	if( 'undefined'==typeof(contnet)
		|| ''==contnet
		 )   
	{
		
		respondObj = 
		{
			'Success':false
			,'Code':0
			,'Message':''
			,'Key':''
			,'Content':''
		}

		res.json( respondObj ) ;
			
		console.log("parameter is missing" );
		
		return ;
	}

	gDataBasePtr.query('INSERT INTO tb_UploadStrategies \
		( Content , \
			Author, \
		UploadTime ) VALUES \
		( ?, ?, ? )', 
		[ contnet ,
		author ,
		new Date() ],
		function( iciErr , iciResult )
	{
		if ( iciErr ) 
		{
			throw iciErr ;
		}	
		
		var queryInfo = gDataBasePtr.query( 'SELECT * FROM tb_UploadStrategies',
		[ ] , 
		function( err , rows , fields ) 
		{
			if( err )
			{
				throw err ;
			}
			
			respondObj = 
			{
				'Success':true
				,'Code':0
				,'Message':''
				,'Key':''
				,'Content':JSON.stringify( rows.length )
			}
			
			res.json( respondObj ) ;
			
			console.log("UploadContent rows.length=" + rows.length);
			
		} ) ;
		
	} ) ;
		
	
   
});
app.listen( 3101 );
console.log('Listening on port 3101...'  );



setInterval(function () 
{
	// console.log('setInterval gDataBasePtr.query...'  );
    gDataBasePtr.query('SELECT 1');
}, 5000);

