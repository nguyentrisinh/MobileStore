﻿module.exports = function (callback, data) {
    var jsreport = require('jsreport-core')();
 
    jsreport.init().then(function () {
        return jsreport.render({
            template: {
                content: '{{:foo}}',
                engine: 'jsrender',
                recipe: 'phantom-pdf'
            },
            data: {
                foo: data
            }
        }).then(function (resp) {
            callback(/* error */ null, resp.content.toJSON().data);
        });
    }).catch(function (e) {
        callback(/* error */ e, null);
    })
};