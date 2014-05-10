﻿function IndexController($scope, $route, $routeParams, $location, $http) {
    $scope.$route = $route;
    $scope.$location = $location;
    $scope.$routeParams = $routeParams;

    $scope.profiles = null;
    $scope.currentProfile = 'Default Profile';
    $scope.running = false;

    $scope.$watch('running', function (newValue, oldValue) {
        if (newValue) {
            $('#onLabel').addClass('active');
            $('#offLabel').removeClass('active');
        } else {
            $('#onLabel').removeClass('active');
            $('#offLabel').addClass('active');
        }
    });

    $scope.refresh = function () {
        $http.get('/menuSetup?format=json').success(
        function (data) {
            $scope.profiles = data.profiles;
            $scope.currentProfile = data.currentProfile;
        });

        $http.get('/isRunning?format=json').success(
        function (data, textStatus, jqXHR) {
            $scope.running = data.active;
        });
    }

    $scope.refresh();

    $scope.startAfterglow = function () {
        $http.get('/start?format=json').success(
        function (data, textStatus, jqXHR) {
            $scope.running = data.active;
        });
    }

    $scope.stopAfterglow = function () {
        $http.get('/stop?format=json').success(
        function (data, textStatus, jqXHR) {
            $scope.running = data.active;
        });
    }

    $scope.selectProfile = function (profileId) {
        $http.post('/setProfile?format=json', { profileId: profileId }).success(
        function (data) {
            $scope.currentProfile = data;
        })
    }
}

function HomeController($scope, $route, $routeParams, $location) {
    $scope.$route = $route;
    $scope.$location = $location;
    $scope.$routeParams = $routeParams;

    $scope.timer = null;
    $scope.previewRunning = false;

    $scope.$watch('running', function (newValue, oldValue) {
        if (newValue) {
            $('#previewOnLabel').addClass('active');
            $('#previewOffLabel').removeClass('active');
        } else {
            $('#previewOnLabel').removeClass('active');
            $('#previewOffLabel').addClass('active');
        }
    });

    $scope.startPreview = function () {

        $('#onLabel').click();

        $.post('/runtime?format=json', { Start: true },
        function (data, textStatus, jqXHR) {

            $scope.previewRunning = data.Active;

            generatePreviewGrid(data);

            if ($($("#previewGroup").children()[1]).height() != 0) {
                timer = setInterval(getPreview, 100);
            }
        }, 'json');
    }

    $scope.stopPreview = function () {
        $.post('/runtime?format=json', { Start: true },
        function (data, textStatus, jqXHR) {
            $scope.previewRunning = data.Active;

            generatePreviewGrid(data);

            if (timer != null) {
                clearInterval(timer);
                timer = null;
            }
        }, 'json');
    }

    var firstPreview = true;
    function getPreview() {

        $.get("/preview?format=json", {}, function (data, status, xhr) {
            clearInterval(timer);
            timer = null;
            $(data.Lights).each(function (index, item) {
                $("#previewLight_" + item.Top + "_" + item.Left).css("background-color", "#" + item.Colour.R.toString(16) + item.Colour.G.toString(16) + item.Colour.B.toString(16));
            });
            timer = setInterval(getPreview, 100);
        });

    }

    function generatePreviewGrid(setup) {
        var cellHeight = 20;
        var cellWidth = 20;

        var numberHigh = setup.NumberOfLightsHigh;
        var numberWide = setup.NumberOfLightsWide;

        $("#previewScreen").children().remove();

        var height = cellHeight * numberHigh;
        var width = cellWidth * numberWide;

        $("#previewScreen").attr('style', 'position:relative;width:' + width + 'px; height:' + height + 'px;');

        var centreDivHeight = cellHeight * (numberHigh > 2 ? numberHigh - 2 : 0);
        var centreDivWidth = cellWidth * (numberWide > 2 ? numberWide - 2 : 0);

        $("#previewScreen").append('<div style="display:inline-block;position: absolute;background-color:#6495ed;height:' + centreDivHeight + 'px; width:' + centreDivWidth + 'px; top:' + cellHeight + 'px; left:' + cellWidth + 'px; float:left;" ></div>');

        for (var topPosition = 0; topPosition < numberHigh; topPosition++) {
            for (var leftPosition = 0; leftPosition < numberWide; leftPosition++) {
                //Only add 
                if (topPosition == 0 || topPosition == numberHigh - 1 || leftPosition == 0 || leftPosition == numberWide - 1) {
                    var top = cellHeight * topPosition;
                    var left = cellWidth * leftPosition;
                    $("#previewScreen").append('<div id="previewLight_' + topPosition + '_' + leftPosition + '" style="display:inline-block;position: absolute;background-color:#b0c4de;height:' + cellHeight + 'px; width:' + cellWidth + 'px; top:' + top + 'px; left:' + left + 'px; float:left;" ></div>');
                }
            }
        }
    }
}

function SettingsController($scope, $route, $routeParams, $http) {
    $scope.settings = null;

    $scope.refresh = function () {
        $http.post('/settings?format=json', {}).success(
        function (data, textStatus, jqXHR) {
            $scope.settings = data;
        });
    }

    $scope.refresh();

    $scope.update = function () {
        $http.post('/updateSettings?format=json', {
            id: $scope.id,
            port: $scope.settings.port,
            userName: $scope.settings.userName,
            password: $scope.settings.password
        }).success(
        function (data, textStatus, jqXHR) {
            $scope.settings = data;
        });
    }
}

function ProfilesController($scope, $route, $location, $http) {

    $scope.profiles = [];

    $scope.refresh = function () {
        $http.get('/profiles?format=json').success(
        function (data) {
            $scope.profiles = data.profiles;
        });
    }

    $scope.refresh();

    $scope.addProfile = function () {
        $http.get('/addProfile?format=json').success(
        function (data) {
            $location.path('/profile/' + data);
        });
    }
}

function PluginsController($scope, $route, $routeParams, $http) {

    $scope.availablePlugins = [];
    
    $scope.refresh = function () {
        $http.get('/availablePlugins?format=json').success(
        function (data) {
            $scope.availablePlugins = data;
        });
    }

    $scope.refresh();
}

function ProfileController($scope, $route, $routeParams, $http) {
    $scope.id = $routeParams.id;

    $scope.profile = null;

    $scope.refresh = function () {
        $http.post('/profile?format=json',{id : $scope.id}).success(
        function (data, textStatus, jqXHR) {
            $scope.profile = data;
        });
    }

    $scope.refresh();

    $scope.update = function (){
        $http.post('/updateProfile?format=json', {
            id: $scope.id,
            name: $scope.profile.name,
            description: $scope.profile.description,
            captureFrequency: $scope.profile.captureFrequency,
            outputFrequency: $scope.profile.outputFrequency
        }).success(
        function (data, textStatus, jqXHR) {
            $scope.profile = data;
        });
    }
}

function PluginController($scope, $route, $routeParams, $http) {
    $scope.id = $routeParams.id;

    $scope.plugin = null;

    $scope.refresh = function () {
        $http.post('/plugin?format=json', {
            id: $routeParams.id,
            profileId: $routeParams.profileId,
            pluginType: $routeParams.pluginType
        }).success(
        function (data, textStatus, jqXHR) {
            $scope.plugin = data;
        });
    }

    $scope.refresh();

    $scope.update = function () {
        $http.post('/updatePlugin?format=json', {
            id: $scope.id,
            profileId: $scope.plugin.profileId,
            pluginType: $scope.plugin.pluginType,
            properties: $scope.plugin.properties
        }).success(
        function (data, textStatus, jqXHR) {
            $scope.plugin = data;
        });
    }
}