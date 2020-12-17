function testCoinBaseRequest() {
    Logger.log(placeUsdcSellOrder(0.08,false));
}

function syncAccountData() {
    var firestore = getFirestore();
    var accounts = coinBaseRequest('GET','/v2/accounts','');
    firestore.updateDocument('MCP/CBAccounts',accounts);
}

function syncPaymentData() {
    var firestore = getFirestore();
    var accounts = coinBaseRequest('GET','/v2/payment-methods','');
    firestore.updateDocument('MCP/CBPayments',accounts);
}

function getCoinbaseAccount() {
    var accounts = coinBaseRequest('GET','/v2/accounts','');
    return accounts;
}

function getLtcBalance() {
    var account = coinBaseRequest('GET','/v2/accounts/c1717448-ffbf-5a53-bfb7-78fecfa07be4','');
    var balance = account.data.balance.amount;
    return balance;
}

function getUsdBalance() {
    var account = coinBaseRequest('GET','/v2/accounts/2e8df5b4-7342-5b3a-bc2e-efb73125d65f','');
    var balance = account.data.balance.amount;
    return balance;
}

function getUsdcBalance() {
    var account = coinBaseRequest('GET','/v2/accounts/d8980ae5-8f23-5ce4-9d0e-f3f6225aed8e','');
    var balance = account.data.balance.amount;
    return balance;
}

function getLtcQuote(buyOrSell) {
    var buy = buyOrSell=='buy';
    var balance = buy?getUsdBalance()*0.98:getLtcBalance();
    var response = buy?placeLtcBuyOrder(balance, false):placeLtcSellOrder(balance, false);
    var total = response.data.total.amount;
    var subtotal = response.data.subtotal.amount;
    return total;
}

function placeLtcBuyOrder(amount, commit) {
    var requestPath = "/v2/accounts/c1717448-ffbf-5a53-bfb7-78fecfa07be4/buys";
    var orderData = {
        amount: amount, 
        currency: "LTC", 
        payment_method: "56e08bfa-d073-53bf-bde0-f2541edd0d6b",
        commit: commit||false
    };
    var response = coinBaseRequest('POST', requestPath, JSON.stringify(orderData));
    return response;
}

function placeLtcSellOrder(amount, commit) {
    var requestPath = "/v2/accounts/c1717448-ffbf-5a53-bfb7-78fecfa07be4/sells";
    var orderData = {
        amount: amount, 
        currency: "LTC", 
        payment_method: "56e08bfa-d073-53bf-bde0-f2541edd0d6b",
        commit: commit||false
    };
    var response = coinBaseRequest('POST', requestPath, JSON.stringify(orderData));
    return response;
}

function placeUsdcBuyOrder(amount, commit) {
    var requestPath = "/v2/accounts/d8980ae5-8f23-5ce4-9d0e-f3f6225aed8e/buys";
    var orderData = {
        amount: amount, 
        currency: "USDC", 
        payment_method: "56e08bfa-d073-53bf-bde0-f2541edd0d6b",
        commit: commit||false
    };
    var response = coinBaseRequest('POST', requestPath, JSON.stringify(orderData));
    return response;
}

function placeUsdcSellOrder(amount, commit) {
    var requestPath = "/v2/accounts/d8980ae5-8f23-5ce4-9d0e-f3f6225aed8e/sells";
    var orderData = {
        amount: amount, 
        currency: "USDC", 
        payment_method: "56e08bfa-d073-53bf-bde0-f2541edd0d6b",
        commit: commit||false
    };
    var response = coinBaseRequest('POST', requestPath, JSON.stringify(orderData));
    return response;
}

function coinBaseRequest(method, requestPath, body) {
    //var tr = UrlFetchApp.fetch("https://api.coinbase.com/v2/time");
    //var epochNow = JSON.parse(tr.getContentText()).data.epoch;
    var date = new Date();
    var epochNow = Math.floor((date.getTime()/1000)).toString();

    method = method.toUpperCase();
    var apiKey = "vQO5kEP2fieQdJNy";
    var apiSecret = "Win1Rp5WHTHmCIyvJI5eOjp8DD5Jnrd1";
    var message = epochNow + method + requestPath + body;
    Logger.log(message);
    var byteSignature = Utilities.computeHmacSha256Signature(message, apiSecret);
    // convert byte array to hex string
    var signature = byteSignature.reduce(function(str,chr){
        chr = (chr < 0 ? chr + 256 : chr).toString(16);
        return str + (chr.length==1?'0':'') + chr;
    },'');

    var options = {
        method : method,
        contentType: 'application/json',
        headers: {
        "CB-ACCESS-KEY":apiKey,
        "CB-ACCESS-SIGN":signature,
        "CB-ACCESS-TIMESTAMP": epochNow,
        "CB-VERSION": "2020-02-19"
        },
        payload: body,
        muteHttpExceptions: true
    };

    Logger.log('ENDPOINT:');
    Logger.log("https://api.coinbase.com"+requestPath);
    Logger.log('OPTIONS:');
    Logger.log(options);
    var response = UrlFetchApp.fetch("https://api.coinbase.com"+requestPath, options);
    var content = response.getContentText();
    Logger.log('CONTENT:');
    Logger.log(content);
    var data = JSON.parse(content);
    Logger.log('DATA:');
    Logger.log(data);
    return data;
}

function coinBaseFee(amount) {
    var percentageFee = amount * 0.0149;
    var flatFee = 0;
    if(amount <= 10) flatFee = 0.99;
    if(amount > 10 && amount <= 25) flatFee = 1.49;
    if(amount > 25 && amount <= 50) flatFee = 1.99;
    if(amount > 50 && amount <= 200) flatFee = 2.99;
    return flatFee > percentageFee ? flatFee : percentageFee;
}