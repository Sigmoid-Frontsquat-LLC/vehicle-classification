const http = require('http');



const options = {
    hostname: 'api.openweathermap.org',
    port: 80,
    path: '/data/2.5/weather/?q=Sacramento&appid=bc0a93ad114268f9add5784eb27fb0f9',
    // query: {
    //     q: 'Sacramento,us',
    //     appid: 'bc0a93ad114268f9add5784eb27fb0f9'
    // },
    method: 'GET'
}


function requestCallback(res) {
    // TODO 
    console.log(`statusCode: ${res.statusCode}`);
    res.on('data', (chunk) => {

    })
}

const request = http.request(options, (res) => {
    console.log(`statusCode: ${res.statusCode}`);


    // this is going to be my JSON response 
    let data_recieved = "";



    // when the DATA event is called ,
    // let us know and giv me the chunk of data 
    // that has been released for consumption
    res.on('data', (chunk) => {
        data_recieved += chunk;
        // same as console.log 
        // process.stdout.write(chunk);
        console.log(chunk.toString());
        console.log('\n\n');
    });



    // we have gotten everything .... 
    res.on('end', () => {
        console.log(`Done ${data_recieved}`)
    })

})


request.on('error', (err) => {
    console.log(`Error: ${err.message}`)
})


// go ahead and make the request...
request.end()






