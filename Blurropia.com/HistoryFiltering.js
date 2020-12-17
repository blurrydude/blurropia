function ReadMinerHistory() {
    var firestore = getFirestore();
    var minerHistory = firestore.getDocuments('MinerHistory');
    Logger.log(minerHistory[10]);
}

function FilterHistory() {
    var firestore = getFirestore();
    var date = new Date('2/1/2020');
    var now = new Date();
    var lastHour = 0;
    var lastDay = 0;
    var lastMonth = 0;
    while(date < now) {
        date = date.addSeconds(86400);
        var epoch = Math.round(date.getTime()/1000);
        var mincap = epoch - 86400;
        var ltcPoolData = firestore.query('LtcPoolData').where('captured','>=',mincap).where('captured','<',epoch).execute();
        var updates = [];
        for(var i in ltcPoolData) {
            try {
                var fields = ltcPoolData[i].fields;
                var id = fields.captured;
                var date = fields.captured.toDate();
                var m = date.getMonth();
                var d = date.getDate()-1;
                var h = date.getHours();
                var update = false;
                if(fields['monthChange']) { fields['monthChange'] = false; update = true; }
                if(fields['dayChange']) { fields['dayChange'] = false; update = true; }
                if(fields['hourChange']) { fields['hourChange'] = false; update = true; }
                if(m != lastMonth && !fields['monthChange']) {
                    fields['monthChange'] = true;
                    lastMonth = m;
                    update = true;
                }
                if(d != lastDay && !fields['dayChange']) {
                    fields['dayChange'] = true;
                    lastDay = d;
                    update = true;
                }
                if(h != lastHour && !fields['hourChange']) {
                    fields['hourChange'] = true;
                    lastHour = h;
                    update = true;
                }
                if(update) {
                    updates.push({id:id, fields:fields});
                }
            } catch(e) {Logger.log(e);}
        }

        for(var i in updates) {
            try {
                firestore.updateDocument('LtcPoolData/'+updates[i].id, updates[i].fields);
            } catch(e) {Logger.log(e);}
        }
    }
}

function FilterTempHumHistory() {
    var firestore = getFirestore();
    var date = new Date('2/1/2020');
    var now = new Date();
    var lastHour = 0;
    var lastDay = 0;
    var lastMonth = 0;
    while(date < now) {
        date = date.addSeconds(86400);
        var epoch = Math.round(date.getTime()/1000);
        var mincap = epoch - 86400;
        var ltcPoolData = firestore.query('TempLog').where('timestamp','>=',mincap).where('timestamp','<',epoch).execute();
        var updates = [];
        for(var i in ltcPoolData) {
            try {
                var fields = ltcPoolData[i].fields;
                var id = fields.timestamp;
                var date = fields.timestamp.toDate();
                var m = date.getMonth();
                var d = date.getDate()-1;
                var h = date.getHours();
                var update = false;
                if(fields['monthChange']) { fields['monthChange'] = false; update = true; }
                if(fields['dayChange']) { fields['dayChange'] = false; update = true; }
                if(fields['hourChange']) { fields['hourChange'] = false; update = true; }
                if(m != lastMonth && !fields['monthChange']) {
                    fields['monthChange'] = true;
                    lastMonth = m;
                    update = true;
                }
                if(d != lastDay && !fields['dayChange']) {
                    fields['dayChange'] = true;
                    lastDay = d;
                    update = true;
                }
                if(h != lastHour && !fields['hourChange']) {
                    fields['hourChange'] = true;
                    lastHour = h;
                    update = true;
                }
                if(update) {
                    updates.push({id:id, fields:fields});
                }
            } catch(e) {Logger.log(e);}
        }

        for(var i in updates) {
            try {
                firestore.updateDocument('TempLog/'+updates[i].id, updates[i].fields);
            } catch(e) {Logger.log(e);}
        }
    }
}
/*
Array.prototype.getTotal = function() {
    var array = this;
    var total = 0;
    for(var i in array) {
        total = total + array[i];
    }
    return total;
}

Array.prototype.getAverage = function() {
    var array = this;
    if(this.length == 0) return 0;
    var total = array.getTotal();
    return total / array.length;
}

Array.prototype.getMax = function() {
    var array = this;
    var max = 0;
    for(var i in array) {
        if(array[i] > max) max = array[i];
    }
    return max;
}

Array.prototype.getMax = function() {
    var array = this;
    var min = 0;
    for(var i in array) {
        if(array[i] < min) min = array[i];
    }
    return min;
}
*/
Number.prototype.toDate = function() {
    return new Date(this*1000);
}

Date.prototype.addDays = function(days) {
    var date = new Date(this.valueOf());
    date.setDate(date.getDate() + days);
    return date;
}

Date.prototype.addHours = function(hours) {
    var epoch = Math.round(this.getTime()/1000);
    var newEpoch = epoch + (hours*3600);
    var date = new Date(newEpoch*1000);
    return date;
}

Date.prototype.addMinutes = function(minutes) {
    var epoch = Math.round(this.getTime()/1000);
    var newEpoch = epoch + (minutes*60);
    var date = new Date(newEpoch*1000);
    return date;
}

Date.prototype.addSeconds = function(seconds) {
    var epoch = Math.round(this.getTime()/1000);
    var newEpoch = epoch + seconds;
    var date = new Date(newEpoch*1000);
    return date;
}

Date.prototype.startOfTheHour = function() {
    var t = this;
    var epoch = Math.round(d.getTime()/1000);
    var toth = new Date(Math.floor(epoch/3600)*3600000);
    return toth;
}

Date.prototype.startOfTheDay = function() {
    var t = this;
    var totd = new Date((t.getMonth()+1)+'/'+t.getDate()+'/'+t.getFullYear());
    return totd;
}

Date.prototype.startOfTheMonth = function() {
    var t = this;
    var totm = new Date((t.getMonth()+1)+'/1/'+t.getFullYear());
    return totm;
}

Date.prototype.startOfTheYear = function() {
    var t = this;
    var totm = new Date('1/1/'+t.getFullYear());
    return totm;
}