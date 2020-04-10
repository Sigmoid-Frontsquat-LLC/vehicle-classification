"use strict";

const https = require("https");
const http = require("http");
const IncomingMessage = require("https").IncomingMessage;
const fs = require("fs");

if (fs.existsSync("dump") == false) {
    fs.mkdirSync("dump");
}

const key = "b59f4841c57440d3baaa8fbf777caf54";
const host = "api.cognitive.microsoft.com";
const path = "/bing/v7.0/images/search";
const terms = ["truck", "sedan", "coupe", "suv", "motorcycle"];

const encode_query = (query) => {
    const keys = Object.keys(query);

    if (keys.length === 0) return "";

    let str =
        "?" +
        encodeURIComponent(keys[0]) +
        "=" +
        encodeURIComponent(query[keys[0]]);

    for (let i = 1; i < keys.length; ++i) {
        str +=
            "&" +
            encodeURIComponent(keys[i]) +
            "=" +
            encodeURIComponent(query[keys[i]]);
    }

    return str;
};

const options = {
    method: "GET",
    hostname: host,
    path: path,
    headers: {
        "Ocp-Apim-Subscription-Key": key,
    },
};

const save_url_contents = function (url, path) {
    var file = fs.createWriteStream(path);
    const protocol = require("url").parse(url).protocol.replace(":", "");

    if (protocol === "http") {
        var request = http.request(url, (res) => {
            res.pipe(file);

            res.on("end", () => {
                file.close();

                const stats = fs.statSync(path);

                const kb = Math.round(stats.size / 1024);
                const mb = Math.round(kb / 1024);

                if (kb === 0) {
                    console.log(`Removing ${path}`);
                    fs.unlinkSync(path);
                }
            });
        });

        request.on("error", (err) => {
            console.log(`HTTP URL ${url} - Err: ${err.message}`);

            try {
                fs.unlinkSync(path);
                console.log(`Removed ${path}`);
            } catch {}
        });

        request.end();
    } else if (protocol === "https") {
        var request = https.request(url, (res) => {
            res.pipe(file);

            res.on("end", () => {
                file.close();

                const stats = fs.statSync(path);

                const kb = Math.round(stats.size / 1024);
                const mb = Math.round(kb / 1024);

                if (kb === 0) {
                    console.log(`Removing ${path}`);
                    fs.unlinkSync(path);
                }
            });
        });

        request.on("error", (err) => {
            console.log(`HTTPS URL ${url} - Err: ${err.message}`);

            try {
                fs.unlinkSync(path);

                console.log(`Removed ${path}`);
            } catch {}
        });

        request.end();
    } else {
        console.log(`Invalid prototol ${protocol}`);
    }
};

const webresult_handler = async function (response) {
    if ("error" in response) {
        return console.log("Error" + response.error.message);
    }

    const term = response.queryContext.originalQuery;

    const path = "dump/" + term;

    if (fs.existsSync(path) == false) {
        fs.mkdirSync(path);
    }

    const hits = response["value"];

    for (let i = 0; i < hits.length; ++i) {
        const url = hits[i].contentUrl;
        const image_path = path + "/" + term + i + "." + hits[i].encodingFormat;

        save_url_contents(url, image_path);

        await new Promise((resolver) => {
            setTimeout(resolver, 1000);
        });
    }

    console.log(`${term} finished...`);
};

/**
 *
 * @param {IncomingMessage} res
 */
const response_handler = async function (res) {
    let data = "";

    res.on("data", (chunk) => {
        data += chunk;
    });

    res.on("end", async () => {
        await webresult_handler(JSON.parse(data));
    });
};

async function main() {
    const max_images = 150; // default is 35 maximum is 150

    const query = {
        q: "",
        count: max_images,
        imageType: "Photo",
    };

    for (let i = 0; i < terms.length; ++i) {
        const term = terms[i];

        query.q = term;
        options.path = path + encode_query(query);
        console.log(`Searching for ${term} via path ${options.path}`);

        const request = https.request(options, response_handler);

        request.on("error", (err) => {
            console.log(`Term ${term} - Err: ${err.message}`);
        });

        request.end();

        await new Promise((resolver) => {
            setTimeout(resolver, 1000);
        });
    }
}

main();
