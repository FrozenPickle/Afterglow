'use strict';

afterglowApp.config(function ($routeProvider) {
    $routeProvider.when(
        '/home',
        {
            templateUrl: 'Views/HomeView.html',
            controller: 'HomeController'
        })
    .when(
        '/preview',
        {
            templateUrl: 'Views/PreviewView.html',
            controller: 'PreviewController'
        })
    .when(
        '/settings',
        {
            templateUrl: 'Views/SettingsView.html',
            controller: 'SettingsController'
        })
    .when(
        '/profiles',
        {
            templateUrl: 'Views/ProfilesView.html',
            controller: 'ProfilesController'
        })
    .when(
    	'/plugins',
    	{
    	    templateUrl: 'Views/PluginsView.html',
    	    controller: 'PluginsController'
    	})
        .otherwise(
        {
            redirectTo: '/home'
        });
});