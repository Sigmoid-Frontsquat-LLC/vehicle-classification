const express = require('express');
const app = express();
const Joi = require('joi');
const os = require('os');
const fs = require('fs');
const https = require('https');
const sqlite3 = require('sqlite3').verbose();





// get 
app.get('/', (req,res) => {
    res.setHeader("content-type","application/json");
    res.setHeader("server", os.hostname());

    const response = {
        message: "Hell wrld!",
        code: 200
    };

    res.status(response.code).send(JSON.stringify(response));
});



// creating a new database
let db = new sqlite3.Database('./db/test.db', (err) => {
    if(err) {
        return console.log(err.message);
    }
    console.log('connected to the in-memory database ')
});


// closing the database 
db.close((err) => {
    if(err) {
        console.log('err.messsage');
    }
    console.log("great success: i like db closed ... ... .. ...")
})