/*global jasmine, __karma__, window*/
Error.stackTraceLimit = Infinity;


jasmine.DEFAULT_TIMEOUT_INTERVAL = 3000;

// Cancel Karma's synchronous start,
// we will call `__karma__.start()` later, once all the specs are loaded.
__karma__.loaded = function () { };


function isJsFile(path) {
    return path.slice(-3) === '.js';
}

function isSpecFile(path) {
    return path.slice(-8) === '.spec.js';
}

function isBuiltFile(path) {
    var builtPath = '/base/app/';
    return isJsFile(path) && (path.substr(0, builtPath.length) === builtPath);
}

var allSpecFiles = Object.keys(window.__karma__.files)
  .filter(isSpecFile)
  .filter(isBuiltFile);

// Load our SystemJS configuration.
System.config({
    baseURL: '/base'
});

System.config({
    packages: {
        'app': {
            main: 'main.js',
            defaultExtension: 'js'
        },
        'rxjs': {
            defaultExtension: 'js'
        },
        '@angular/core': {
            main: 'index.js',
            defaultExtension: 'js'
        },
        '@angular/compiler': {
            main: 'index.js',
            defaultExtension: 'js'
        },
        '@angular/common': {
            main: 'index.js',
            defaultExtension: 'js'
        },
        '@angular/platform-browser': {
            main: 'index.js',
            defaultExtension: 'js'
        },
        '@angular/platform-browser-dynamic': {
            main: 'index.js',
            defaultExtension: 'js'
        },
        '@angular/http': {
            main: 'index.js',
            defaultExtension: 'js'
        },
        '@angular/router-deprecated': {
            main: 'index.js',
            defaultExtension: 'js'
        },
        '@angular/router': {
            main: 'index.js',
            defaultExtension: 'js'
        },
        'jspdf': {
            defaultExtension: 'js'
        },
        'jspdf-autotable': {
            defaultExtension: 'js'
        },

    },

    map: {
        'rxjs': 'node_modules/rxjs',
        '@angular': 'node_modules/@angular',
        'jspdf': 'node_modules/jspdf',
        'jspdf-autotable': 'node_modules/jspdf-autotable',
        'app': 'app',

    },
    //paths: {
    //   // 'dragula': 'node_modules/dragula/dist/dragula.js'
    //},
});

Promise.all([
  System.import('@angular/core/testing'),
  System.import('@angular/platform-browser-dynamic/testing')
]).then(function (providers) {
    var testing = providers[0];
    var testingBrowser = providers[1];

    testing.setBaseTestProviders(testingBrowser.TEST_BROWSER_DYNAMIC_PLATFORM_PROVIDERS,
      testingBrowser.TEST_BROWSER_DYNAMIC_APPLICATION_PROVIDERS);

}).then(function () {
    // Finally, load all spec files.
    // This will run the tests directly.
    return Promise.all(
      allSpecFiles.map(function (moduleName) {
          return System.import(moduleName);
      }));
}).then(__karma__.start, __karma__.error);


