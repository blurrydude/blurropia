function testBinanceRequest() {
    Logger.log(getBinancePrice('LTC'));
}

function getBinanceAccount() {
    return binanceRequest('/api/v3/account','GET','',true);
}

function binanceRequest(endpoint, method, body, auth) {
    var secretKey = "rkjxROwRls1fyqVGI5l0GiUsKAt68Eo8NsXJ4lS2fV6XZALnEdhFjrBWtaSzn8xm";
    var apiKey = "5rogZOCtCIWXBrJk2KVxkDwRcatmXmA10PeCKSLuMluIPXwupVUzfOTpiX79uYCu";
    var baseUrl = "https://api.binance.com";
    var bodyQuery = toQueryString(body, auth);
    var options = {
        method : method,
        contentType: 'application/json',
        headers: {},
        muteHttpExceptions: true
    };
    if(method!="GET") options["payload"] = body;
    if(auth) {
        var byteSignature = Utilities.computeHmacSha256Signature(bodyQuery, secretKey);
        // convert byte array to hex string
        var signature = byteSignature.reduce(function(str,chr){
            chr = (chr < 0 ? chr + 256 : chr).toString(16);
            return str + (chr.length==1?'0':'') + chr;
        },'');
        options.headers["X-MBX-APIKEY"] = apiKey;
        //options.payload["signature"] = signature;
        bodyQuery = bodyQuery + '&signature=' + signature; 
    }

    var finalURL = baseUrl+endpoint+'?'+bodyQuery;
    Logger.log('ENDPOINT:');
    Logger.log(finalURL);
    Logger.log('OPTIONS:');
    Logger.log(options);
    var response = UrlFetchApp.fetch(finalURL, options);
    var content = response.getContentText();
    Logger.log('CONTENT:');
    Logger.log(content);
    var data = JSON.parse(content);
    Logger.log('DATA:');
    Logger.log(data);
    return data;
}

function getBinancePrice(symbol) {
    var priceEndpoint = "/api/v3/ticker/price";
    return binanceRequest(priceEndpoint,"GET",{symbol:symbol+'USDT'});
}

function toQueryString(obj, auth) {
    var out = auth ? 'timestamp='+(new Date).getTime() : '';
    var i = 0;
    for (var key in obj) {
        if (obj.hasOwnProperty(key)) {
            out = out + (!auth&&i==0?'':'&') + key + '=' + obj[key];
        }
        i++;
    }
    
    return out;
}