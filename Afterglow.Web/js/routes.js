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
    	'/plugins',
    	{
    	    templateUrl: 'Views/PluginsView.html',
    	    controller: 'PluginsController'
    	})
    .when(
        '/profiles',
        {
            templateUrl: 'Views/ProfilesView.html',
            controller: 'ProfilesController'
        })
    .when('/plugin/:profileId/:pluginType/:id',
        {
            templateUrl: 'Views/PluginView.html',
            controller: 'PluginController'
        })
    .when('/plugin/:profileId/:pluginType/new/:type',
        {
            templateUrl: 'Views/PluginView.html',
            controller: 'PluginController'
        })
    .when(
        '/profile/:id',
        {
            templateUrl: 'Views/ProfileView.html',
            controller: 'ProfileController'
        })
        .otherwise(
        {
            redirectTo: '/home'
        });
});