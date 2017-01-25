
require.config({
    waitSeconds: 200,

    map: {
        '*': {
            'css':                  'assets/bower_components/require-css/css'
        }
    },
    paths: {
        
        'jquery':                   'assets/global/plugins/jquery/jquery-migrate-1.2.1.min',
        "angularAMD":               'assets/bower_components/angularAMD/angularAMD.min',
        'angular-local-storage':    'assets/bower_components/angular-local-storage/dist/angular-local-storage.min',
        "ngload":                   "assets/bower_components/angularAMD/ngload.min",
        "angular-ui-router":        "assets/bower_components/angular-ui-router/release/angular-ui-router.min",
        "angular-ui-select":        "assets/bower_components/angular-ui-select/dist/select.min",
        "ui-router-extras":         "assets/bower_components/ui-router-extras/release/ct-ui-router-extras.min",
        'lodash':                   'assets/bower_components/lodash/dist/lodash.min',
        'restangular':              'assets/bower_components/restangular/dist/restangular.min',
        'angular-ui-grid':          'assets/bower_components/angular-ui-grid/ui-grid',
        'pdfmake':                  'assets/bower_components/pdfmake-master/build/pdfmake.min',
        'vfs_fonts':                'assets/bower_components/pdfmake-master/build/vfs_fonts',
        'jstree':                   'assets/bower_components/jstree/dist/jstree_3.2.1',
        'plupload':                 'assets/global/plugins/plupload-2.1.8/js/plupload.full.min',
        'angular-validation':       'assets/bower_components/angular-validation-master/dist/angular-validation',
        'ng-tagsinput':             'assets/bower_components/ng-tags-input/ng-tags-input',
        'moment':                   'assets/bower_components/moment/min/moment-with-locales.min',
        'angular-moment':           'assets/bower_components/angular-moment/angular-moment',
        'moment-range':             'assets/bower_components/moment-range/lib/moment-range.min',
        
        'rate':                     'assets/bower_components/angular-rateit-master/dist/ng-rateit.min',
        'angular-gantt':            'assets/bower_components/angular-gantt/assets/angular-gantt',
        'angular-gantt-plugins':    'assets/bower_components/angular-gantt/assets/angular-gantt-plugins',
        'angular-ui-tree':          'assets/bower_components/angular-ui-tree/dist/angular-ui-tree.min',
      
        'angular-sanitize':         'assets/bower_components/angular-sanitize/angular-sanitize.min',
        'angular-strap':            'assets/bower_components/angular-strap/dist/angular-strap.min',
        'angular-native-dragdrop': 'assets/bower_components/angular-native-dragdrop/draganddrop',
        'interact':                 'assets/bower_components/interact/dist/interact.min',
        
        'calendar':                 'assets/bower_components/angular-bootstrap-calendar/dist/js/angular-bootstrap-calendar-tpls',
        'timer':                    'assets/bower_components/angular-timer/dist/angular-timer',
        'humanizeDuration':         'assets/bower_components/humanize-duration/humanize-duration',
        'ckeditor':                 'assets/global/plugins/ckeditor_4.5.6/ckeditor',
        'echarts':                  'assets/global/plugins/echarts/echarts.min',
        'BigInt':                   'assets/global/js/BigInt',
        'jsBarcode':                'assets/bower_components/JsBarcode/dist/JsBarcode.all.min',
    },
    shim: {
        "ngload":                   ["angularAMD"],
        'ui-router-extras':         ['angular-ui-router'],
        'restangular':              ['lodash'],
        'angular-ui-grid':          ['pdfmake','vfs_fonts','css!assets/bower_components/angular-ui-grid/ui-grid'],
        'angular-validation':       ['assets/bower_components/angular-validation-master/dist/angular-validation-rule'],
        'ng-tagsinput':             ['css!assets/bower_components/ng-tags-input/ng-tags-input'],
        'calendar':                 ['css!assets/bower_components/angular-bootstrap-calendar/dist/css/angular-bootstrap-calendar.min'],
        'rate':                     ['css!assets/bower_components/angular-rateit-master/dist/ng-rateit'],
        'timer':                    ['moment', 'angular-moment', 'humanizeDuration'],
        'angular-moment':           ['moment'],
        'angular-gantt':            ['angular-moment', 'angular-ui-tree',  'angular-native-dragdrop'],
        'angular-gantt-plugins':    ['angular-moment', 'angular-gantt']
    },
    deps: ["apps/app"]
});
