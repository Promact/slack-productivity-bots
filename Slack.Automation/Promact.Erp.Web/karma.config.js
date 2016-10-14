module.exports = function (config) {
    config.set({

        // base path that will be used to resolve all patterns (eg. files, exclude)
        basePath: '',


        // frameworks to use
        // available frameworks: https://npmjs.org/browse/keyword/karma-adapter
        frameworks: ['jasmine'],


        // list of files / patterns to load in the browser
        files: [
        'node_modules/jspdf/dist/jspdf.min.js',
        'node_modules/jspdf-autotable/dist/jspdf.plugin.autotable.umd.js',
        'node_modules/core-js/client/shim.min.js',
        'node_modules/reflect-metadata/Reflect.js',

        // System.js for module loading
        'node_modules/systemjs/dist/system-polyfills.js',
        'node_modules/systemjs/dist/system.src.js',

        // Zone.js dependencies
        'node_modules/zone.js/dist/zone.js',
        'node_modules/zone.js/dist/zone.js',
        //'node_modules/zone.js/dist/jasmine-patch.js',
        'node_modules/zone.js/dist/async-test.js',
        'node_modules/zone.js/dist/fake-async-test.js',

        // RxJs.
        { pattern: 'node_modules/rxjs/**/*.js', included: false, watched: false },
        { pattern: 'node_modules/rxjs/**/*.js.map', included: false, watched: false },


        'karma-test-shim.js',

         // paths loaded via module imports

         // Angular itself
        { pattern: 'node_modules/@angular/**/*.js', included: false, watched: true },
        { pattern: 'node_modules/@angular/**/*.js.map', included: false, watched: true },


        // Our built application code
        { pattern: 'app/**/*.ts', included: false, watched: true },
        { pattern: 'app/**/*.js', included: false, watched: true },
        { pattern: 'app/**/*.js.map', included: false, watched: true },
        { pattern: 'app/**/*.html', included: false, watched: true },
        //{ pattern: 'app/leaveReport/**/*.js', included: false, watched: true },
        //{ pattern: 'app/leaveReport/**/*.js.map', included: false, watched: true },
        //{ pattern: 'app/leaveReport/**/*.html', included: false, watched: true },
        ],



        // list of files to exclude
        exclude: [
        ],


        // preprocess matching files before serving them to the browser
        // available preprocessors: https://npmjs.org/browse/keyword/karma-preprocessor
        //preprocessors: {
        //    "**/app/*spec.js": "coverage"
        //},

        // test results reporter to use
        // possible values: 'dots', 'progress'
        // available reporters: https://npmjs.org/browse/keyword/karma-reporter
        reporters: ['progress', 'coverage', 'coveralls'],

        coverageReporter: {
            type: 'lcov', // lcov or lcovonly are required for generating lcov.info files
            dir: 'coverage/'
        },


        // web server port
        port: 9876,


        // enable / disable colors in the output (reporters and logs)
        colors: true,


        // level of logging
        // possible values: config.LOG_DISABLE || config.LOG_ERROR || config.LOG_WARN || config.LOG_INFO || config.LOG_DEBUG
        logLevel: config.LOG_INFO,


        // enable / disable watching file and executing tests whenever any file changes
        autoWatch: true,


        // start these browsers
        // available browser launchers: https://npmjs.org/browse/keyword/karma-launcher
        // browsers: ['Chrome', 'Firefox', 'IE'],
        browsers: ['Chrome'],


        // Continuous Integration mode
        // if true, Karma captures browsers, runs the tests and exits
        singleRun: true,

        // Concurrency level
        // how many browser should be started simultaneous
        concurrency: Infinity,

        // Karma plugins loaded
        plugins: [
            'karma-jasmine',
            'karma-chrome-launcher',
            'karma-coverage',
            'karma-coveralls'
        ],

    })
}