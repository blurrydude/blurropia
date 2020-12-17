function getFirestore() {
    var email = "blurropia-uo@appspot.gserviceaccount.com";
    var projectId = "blurropia-uo";
    var key = "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQC4NcgsoSandklk\n7AMd/qEvzgL3//gGowA40FzOVL2EVU91Z2Up87Lt2feUZfuu/EJvnbrCqCJlAtae\n4IN0hLP3zgBl4W05d09rPiqySCBeWZVw/7nclkr24PTfIP/j/KtIHLIN9P78nCxm\nbQOpdLKBT3Rge9DQQb46CcymaMzCRxyqCuXDLWKIH5Bf0PkPL5z7vNtrTkeJCE6z\neutArM6kU7dOihOKXH3JRfuZL4k+fZFJFwtNlh/sy9Wym7JNuaI5Im10Dap0ovxW\nnx7VqyHG65ZCVsm/ysEPLFea/z5Gv7bU1Q5GUiv8inQqUdJulWhbm54IGyqgLghU\nYXO3Uru3AgMBAAECggEAScPDnleD1pBP8Jlv4B18Z1u8JZo7NFEmgBmz6C3X3Dub\nP3zBbmy3jHZ5pVO3VsmOQwrq5BMW/tMFpYZrLYgMVyGHWETOi7ICvAOfblfimlWO\nObs1rqcCat0HQaiBv8aIwi43gM5II1jYkMV7r2pbJtvt0ssSAasp5SzSZhrWT78j\nPU77tyWgvne3L2CFPhlAsUj3CGFh9LYcS0dWa74IBTqBoJkm90uLPAaRcdxVuXnh\nzOjk8cD8KCp6iB24bFLyjFbqqr/fomcl31gPeBouHTQEuRV9P5qQks8hFAli03BB\nVjW0tOVy07L1Z9PWL6x14iJY6uBIVVCR+A1ENOghgQKBgQDeEKdOVbBmijo8jZyp\nhYzqChfSmhzGw/s6mzSzOIG/0RsTxYBdp81b7lZUq55vKVOxhCfa1/AegTaAqIc4\nZs7lBVf64SxblbYPImGsueOifA73iCB1FvDbT9/50LYbv1hrFlkMcHY03KX3TgJz\n2EeElqONOS+jH97vo3R7UnqXNwKBgQDUXDbrUBL8Ix2Z8otbQII98IzzndrN0Su3\n38IzTNArfyx024qQ2zax6dLqKyLn72MX0Ubx4s9pIHwVuFZTV0OiwlaGv/sj0ffn\nJXtOvuwDH/B8hC2nyz5XQpsLdoRHkJE9IvkX/05Mkz1+jhz8XvCBqhyfCnEdRAF5\nilkYRVo/gQKBgHX2psmoH2L5PuJ336b1+NtCrVEE6RB1f3mshRxHgjVhYV7pZb1a\nrG6Pd3DXEfy/GWSNbZaFa1rpDQjffaVqh7GFWy08fJHSP7pea0CUJuZjvJelyogo\nvZ3jGnncc0DeLpwEbR18iG6gX9G1sPvVAnjehNc6b6HaBlWKoQ/5yoGVAoGAJtae\nrLa+YiLB/S/g0NWhfFaMUNnj9JR23HAuAlfgErR1mwGz+/47qJpNGsq83ZJomyiJ\nQx/qMx3f09Ec6K0jD5dfX9GoyspR/pJaVPKSE5g50WwTNYb0Zd0mohgQ5U3JYlcN\nG5hz54leS9EN+BIdBnnoNiYgvet8IZBt7wRwFwECgYEAsUMn+SAO0B1lbTCCFGM1\nATx430jIamz0j9VaklMREjBYScnyrHO5t5Wdz9xLpENNYRI/pBDCmeFaIKM2aGKd\nsfLN4Pe+/HC3XxlriDzPHtm1bPwezTN5N+8lkzNbeF2we076pzk42MUCrUUftZkW\niIbrM2JSV0+ZrTDq28gP9SA=\n-----END PRIVATE KEY-----\n";
    return FirestoreApp.getFirestore(email, key, projectId);
  }
    
  function doGet(request) {
    var viewTemplate = (request.parameters && request.parameter.view) ? request.parameter.view.toLowerCase() + '.view' : 'index';
    var template = HtmlService.createTemplateFromFile(viewTemplate);
    if(request.params && request.params.view) return template.evaluate().getContent();
    return template.evaluate().setXFrameOptionsMode(HtmlService.XFrameOptionsMode.ALLOWALL);
  }
  
  function include(filename) {
    return HtmlService.createHtmlOutputFromFile(filename).getContent();
    /*var template = HtmlService.createTemplateFromFile(filename)
    template.data = {
      scriptUrl: ScriptApp.getService().getUrl(),
    };
    return template.evaluate().getContent();*/
    //return template.evaluate().setXFrameOptionsMode(HtmlService.XFrameOptionsMode.ALLOWALL).getContent();
  }

  function getFirestoreCredentials() {
    return {
    apiKey: "AIzaSyBS5gkmi1wy13hU2KoZ1gnA5le8srcOp_8",
    authDomain: "blurropia-uo.firebaseapp.com",
    projectId: "blurropia-uo",
    storageBucket: "blurropia-uo.appspot.com",
    messagingSenderId: "525894614869",
    appId: "1:525894614869:web:cea647e817d49c5cd0b9dc"
  };
    /*{
      apiKey: PropertiesService.getScriptProperties().getProperty('apiKey'),
      authDomain: PropertiesService.getScriptProperties().getProperty('authDomain'),
      databaseURL: PropertiesService.getScriptProperties().getProperty('databaseURL'),
      projectId: PropertiesService.getScriptProperties().getProperty('projectId'),
      storageBucket: PropertiesService.getScriptProperties().getProperty('storageBucket'),
      messagingSenderId: PropertiesService.getScriptProperties().getProperty('messagingSenderId'),
      appId: PropertiesService.getScriptProperties().getProperty('appId'),
      //measurementId: ''//PropertiesService.getScriptProperties().getProperty('measurementId')
    };*/
  }
  
  function getTemplates(files) {
    var templates = [];
    for(var i = 0; i < files.length; i++) {
      /*var temp = HtmlService.createTemplateFromFile(files[i])
      temp.data = {
        scriptUrl: ScriptApp.getService().getUrl(),
      };
      var template = temp.evaluate().getContent();*/
      var template = HtmlService.createHtmlOutputFromFile(files[i]).getContent();
      //var template = temp.evaluate().setXFrameOptionsMode(HtmlService.XFrameOptionsMode.ALLOWALL).getContent();
      templates.push({key:files[i], value:template});
    }
    return templates;
  }
  
  function getFirestoreDocument(path) {
    //console.info({message: 'getFirestoreDocument() - BEGIN', parameters: {path: path}});
    var executionTimeLabel = 'getFirestoreDocument() - END [Execution Duration]'; 
    //console.time(executionTimeLabel);
    
    var firestore = getFirestore();
    try {
      var data = firestore.getDocument(path);
    }
    catch(e) {
      //console.info({message: 'getFirestoreDocument() - ERROR', parameters: {path: path, exception: e}});
      data = {empty:true,fields:{}};
    }
    
    //console.log({message: 'Data retrieved from getFirestoreDocument()', data: data});
    //console.timeEnd(executionTimeLabel);
    
    return data;
  }
  
  function getFirestoreDocuments(path) {
    //console.info({message: 'getFirestoreDocuments() - BEGIN', parameters: {path: path}});
    var executionTimeLabel = 'getFirestoreDocuments() - END [Execution Duration]'; 
    //console.time(executionTimeLabel);
    
    var firestore = getFirestore();
    var data = firestore.getDocuments(path);
    
    //console.log({message: 'Data retrieved from getFirestoreDocuments()', data: data});
    //console.timeEnd(executionTimeLabel);
    
    return data;
  }
  
  function updateFirestoreDocument(path, dat) {
    try {
      var firestore = getFirestore();
      var data = firestore.updateDocument(path, dat);
    return data;
    } catch(e) {
      return {path:path, data:dat, error:e};
    }
  }
  
  function deleteFirestoreDocument(path) {
    
    var firestore = getFirestore();
    var data = firestore.deleteDocument(path);
    return data;
  }
  
  function queryFirestore(collection, field, comparitor, value) {
    //console.info({message: 'queryFirestore() - BEGIN', parameters: {collection: collection, field: field, comparitor: comparitor, value: value}});
    var executionTimeLabel = 'queryFirestore() - END [Execution Duration]'; 
    //console.time(executionTimeLabel);
    
    try {
      //var cache = CacheService.getScriptCache();
      /*var cachedReqeusts = cache.get(collection + '-' + field + '-' + comparitor + '-' + value);
      
      if (cachedReqeusts) {
        //console.log({message: 'Cached data retrieved from queryFirestore()', data: cachedReqeusts});
        //console.timeEnd(executionTimeLabel);
        
        return cachedReqeusts;
      }*/  
      
      var firestore = getFirestore();
      var data = firestore.query(collection).where(field, comparitor, value).execute();
      
      //cache.put(collection + '-' + field + '-' + comparitor + '-' + value, JSON.stringify(data), 1500);
      
      //console.log({message: 'New data retrieved from queryFirestore()', data: data});
      //console.timeEnd(executionTimeLabel);
      
      return data;
    }
    catch (e) 
    {
      //console.error(e);
      //console.timeEnd(executionTimeLabel);
    }
  }
  
  function MD5( input, isShortMode )
  {
      var txtHash = '';
      var rawHash = Utilities.computeDigest(
                        Utilities.DigestAlgorithm.MD5,
                        input,
                        Utilities.Charset.UTF_8 );
  
      var isShortMode = ( isShortMode == true ) ? true : false;
   
      if ( ! isShortMode ) {
          for ( i = 0; i < rawHash.length; i++ ) {
  
              var hashVal = rawHash[i];
  
              if ( hashVal < 0 ) {
                  hashVal += 256;
              };
              if ( hashVal.toString( 16 ).length == 1 ) {
                  txtHash += '0';
              };
              txtHash += hashVal.toString( 16 );
          };
      } else {
          for ( j = 0; j < 16; j += 8 ) {
  
              hashVal = ( rawHash[j]   + rawHash[j+1] + rawHash[j+2] + rawHash[j+3] )
                      ^ ( rawHash[j+4] + rawHash[j+5] + rawHash[j+6] + rawHash[j+7] );
  
              if ( hashVal < 0 ) {
                  hashVal += 1024;
              };
              if ( hashVal.toString( 36 ).length == 1 ) {
                  txtHash += "0";
              };
  
              txtHash += hashVal.toString( 36 );
          };
      };
  
      // change below to "txtHash.toLowerCase()" for lower case result.
      return txtHash.toUpperCase();
  
  }
  
  function getTemplateHtml(docId) {
    return UrlFetchApp.fetch("https://docs.google.com/feeds/download/documents/export/Export?id="+docId+"&exportFormat=html").getContentText();
    //return DocumentApp.openById(docId).getBody().getText();//1Rb7tX8QvzwTNLBBUaBqrcwIk0j6Q3gkwNHEwORyJeX4
    //return doc.getBody();
    //return HtmlService.createHtmlOutputFromFile(templateFile).getContent();
    //return ScriptApp.getOAuthToken();
  }
  
  /*
  function doc_to_html()
  {
   var id = document_Id;
   var url = "https://docs.google.com/feeds/download/documents/export/Export?id="+id+"&exportFormat=html";
   var param = 
          {
            method      : "get",
            headers     : {"Authorization": "Bearer " + ScriptApp.getOAuthToken()},
            muteHttpExceptions:true,
          };
   var html = UrlFetchApp.fetch(url,param).getContentText();
  }
  */