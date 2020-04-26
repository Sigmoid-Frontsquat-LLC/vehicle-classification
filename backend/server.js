const express = require("express");
const app = express();
const os = require("os");
const fs = require("fs");

app.use(
    express.json({
        limit: "10mb",
    })
);

// Home or default
app.get("/", (req, res) => {
    res.setHeader("Content-Type", "application/json");

    res.end(
        JSON.stringify({
            status: "OK",
        })
    );
});

app.post("/classify", (req, res) => {
    const json = req.body; // we don't need to parse because app.use(json) is enabled.

    const base64Image = json.image;
    const extension = json.extension;

    let buffer = new Buffer(base64Image, "base64");

    const hash = require("crypto")
        .createHash("md5")
        .update(base64Image)
        .digest("hex");

    const path = "temp/" + hash + "." + extension;
    fs.writeFileSync(path, buffer);

    const spawn = require("child_process").spawn;
    const c_process = spawn("python3", ["./classification.py", "-s", path]);

    let error = "";

    c_process.stderr.on("data", (chunk) => {
        error += chunk;
    });

    let out = "";

    c_process.stdout.on("data", (chunk) => {
        out += chunk;
    });

    c_process.on("exit", (code) => {
        const result = {
            code: 200,
            error: error,
            out: out,
        };

        if (out === undefined || out === null) {
            if (error !== undefined || error !== null) {
                result.code = 400;
            }
        } else {
            out = out.replace("[", "").replace("]", "");

            const array = [];
            const e = out.split("},");

            for (let i = 0; i < e.length; ++i) {
                e[i] = e[i].replace("{", "").replace("}", "").trimLeft();

                const tuple = e[i].split(":");
                const label = tuple[0].replace("'", "").replace("'", "");
                const prob = Number.parseFloat(tuple[1]);

                const clazz = {
                    label: label,
                    prob: prob,
                };

                array.push(clazz);
            }

            result.payload = array;
        }

        res.setHeader("Content-Type", "application/json");

        return res.status(result.code).end(JSON.stringify(result));
    });
});
