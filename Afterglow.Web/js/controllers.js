function IndexController($scope, $route, $routeParams, $location, $http) {
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
        $http.get('/setup?format=json').success(
        function (data) {
            $scope.profiles = [];
            $.each(data.Setup.Profiles, function (index, object) {
                $scope.profiles.push({name:object.Name, description: object.Description})
            });
            var currentProfile = $.grep(data.Setup.Profiles, function (object, index) {
                return object.Id == data.Setup.CurrentProfileId;
            });
            $scope.currentProfile = currentProfile[0];
        });

        $.get('/runtime?format=json', {},
        function (data, textStatus, jqXHR) {
            $scope.running = data.Active;
        }, 'json');
    }

    $scope.refresh();

    $scope.startAfterglow = function () {
        $.post('/runtime?format=json', { Start: true },
        function (data, textStatus, jqXHR) {
            $scope.running = data.Active;
        }, 'json');
    }

    $scope.stopAfterglow = function () {
        $.post('/runtime?format=json', { Start: false },
        function (data, textStatus, jqXHR) {
            $scope.running = data.Active;
        }, 'json');
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

function SettingsController($scope, $route, $routeParams, $location) {
    $scope.$route = $route;
    $scope.$routeParams = $routeParams;
}

function ProfilesController($scope, $route, $routeParams, $http) {
    $scope.$route = $route;
    $scope.$routeParams = $routeParams;
}

function PluginsController($scope, $route, $routeParams, $http) {

    $scope.availablePlugins = null;
    
    $scope.refresh = function () {
        $http.get('/availablePlugins?format=json').success(
        function (data) {
            $scope.availablePlugins = data;
        });
    }

    $scope.refresh();
}

function PluginController($scope, $route, $routeParams, $location) {
    $scope.$route = $route;
    $scope.$location = $location;
    $scope.$routeParams = $routeParams;
}