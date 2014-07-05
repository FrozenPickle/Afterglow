function IndexController($scope, $route, $routeParams, $location, $http) {
    $scope.$route = $route;
    $scope.$location = $location;
    $scope.$routeParams = $routeParams;

    $scope.profiles = null;
    $scope.currentProfile = { name: 'Loading Settings' };
    $scope.running = false;

    $scope.refresh = function () {
        $http.get('/menuSetup').success(
        function (data) {
            $scope.profiles = data.profiles;
            $scope.currentProfile = data.currentProfile;
        });

        $http.get('/isRunning').success(
        function (data, textStatus, jqXHR) {
            $scope.running = data.active;
        });
    }

    $scope.refresh();

    $scope.startAfterglow = function () {
        $http.get('/start').success(
        function (data, textStatus, jqXHR) {
            $scope.running = data.active;
        });
    }

    $scope.stopAfterglow = function () {
        $http.get('/stop').success(
        function (data, textStatus, jqXHR) {
            $scope.running = data.active;
        });
    }

    $scope.selectProfile = function (profileId) {
        $http.post('/setProfile', { profileId: profileId }).success(
        function (data) {
            $scope.currentProfile = data;
        });
    }
}

function HomeController($scope, $interval, $routeParams, $http) {
    
    $scope.previewRunning = false;
    $scope.promise = null;

    var PREVIEW_INTERVAL = 100;

    $scope.lightSetup = {};

    $scope.refresh = function () {
        $http.get('/previewSetup').success(
        function (data) {
            $scope.lightSetup = data.lightSetup;
        });
    }

    $scope.refresh();

    $scope.startPreview = function () {

        $scope.startAfterglow();
        $scope.runPreview();
    }
    $scope.runPreview = function () {

        if ($scope.promise != null) {
            $interval.cancel($scope.promise);
        }
        $scope.promise = $interval(function () {
            $http.get('/previewLights').success(function (data) {

                $scope.lightSetup.captureFPS = data.captureFPS;
                $scope.lightSetup.captureFrameTime = data.captureFrameTime;
                $scope.lightSetup.outputFPS = data.outputFPS;
                $scope.lightSetup.outputFrameTime = data.outputFrameTime;

                for (var lightIndex = 0; lightIndex < data.lights.length; lightIndex++) {
                    var light = data.lights[lightIndex];

                    if ($scope.lightSetup.lightRows.length > light.top &&
                        $scope.lightSetup.lightRows[light.top].lightColumns.length > light.left) {
                        $scope.lightSetup.lightRows[light.top].lightColumns[light.left].colour = light.colour;
                    }
                }
            }).error(function () {
                if ($scope.promise != null) {
                    $interval.cancel($scope.promise);
                }
            });
        }, PREVIEW_INTERVAL);
    }

    $scope.stopPreview = function () {
        if ($scope.promise != null) {
            $interval.cancel($scope.promise);
        }
    }
}

function SettingsController($scope, $route, $routeParams, $http) {
    $scope.settings = null;

    $scope.refresh = function () {
        $http.get('/settings', {}).success(
        function (data, textStatus, jqXHR) {
            $scope.settings = data;
        });
    }

    $scope.refresh();

    $scope.update = function () {
        $http.post('/updateSettings', {
            runOnWindowsStartup: $scope.settings.runOnWindowsStartup,
            logLevel: $scope.settings.logLevel,
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
        $http.get('/profiles').success(
        function (data) {
            $scope.profiles = data.profiles;
        });
    }

    $scope.refresh();

    $scope.addProfile = function () {
        $http.get('/addProfile').success(
        function (data) {
            $location.path('/profile/' + data);
        });
    }
}

function PluginsController($scope, $route, $routeParams, $http) {

    $scope.availablePlugins = [];
    
    $scope.refresh = function () {
        $http.get('/availablePlugins').success(
        function (data) {
            $scope.availablePlugins = data;
        });
    }

    $scope.refresh();
}

function ProfileController($scope, $location, $routeParams, $http, $modal) {
    $scope.id = $routeParams.id;

    $scope.profile = null;

    $scope.refresh = function () {
        $http.post('/profile',{id : $scope.id}).success(
        function (data, textStatus, jqXHR) {
            $scope.profile = data;
        });
    }

    $scope.refresh();

    $scope.update = function (){
        $http.post('/updateProfile', {
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

    $scope.add = function (pluginType, modalTitle) {
        $http.post('/pluginTypes', {
            pluginType: pluginType
        }).success(
        function (data, textStatus, jqXHR) {
            var modalInstance = $modal.open({
                templateUrl: 'Views/PluginTypeSelectionView.html',
                controller: PluginTypeSelectionController,
                resolve: {
                    title: function() {
                        return modalTitle;
                    },
                    pluginType: function(){
                        return pluginType;
                    },
                    plugins: function () {
                        return data.plugins;
                    }
                }
            });

            modalInstance.result.then(function (selectedItem) {
                $location.path('/plugin/' + $scope.id + '/' + pluginType + '/new/' + selectedItem);
            });
        });
    }
}

function PluginTypeSelectionController($scope, $modalInstance, title, pluginType, plugins) {

    $scope.title = title;
    $scope.pluginType = pluginType;
    $scope.plugins = plugins;
    
    $scope.confirmAdd = function (plugin) {
        $modalInstance.close(plugin);
    }

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};

function PluginController($scope, $route, $routeParams, $http) {
    $scope.id = $routeParams.id;

    $scope.new = false;
    $scope.allowDelete = false;

    $scope.plugin = null;

    function findItem(items, callback) {
        if (items == null)
        {
            return null;
        }
        var len = items.length;
        var i = 0;
        for (i; i < len; i++) {
            var item = items[i];
            var cond = callback(item);
            if (cond) {
                return item;
            }
        }

        return null;
    };

    $scope.refresh = function () {
        $scope.new = ($scope.id == undefined);

        if ($routeParams.pluginType > 3) {
            $scope.allowDelete = true;
        }

        $http.post('/plugin', {
            id: $routeParams.id,
            profileId: $routeParams.profileId,
            pluginType: $routeParams.pluginType,
            type: $routeParams.type
        }).success(
        function (data, textStatus, jqXHR) {
            $scope.plugin = data;
            watchLightProperties();
        });
    }

    $scope.refresh();

    function watchLightProperties() {

        var lightsProperty = findItem($scope.plugin.properties, function (property) {
            return (property.type == 'lights');
        });
        if (lightsProperty == null) {
            return;
        }

        for (var propertyIndex = 0; propertyIndex < $scope.plugin.properties.length; propertyIndex++) {
            var property = $scope.plugin.properties[propertyIndex];
            if (property.name == 'NumberOfLightsWide' || property.name == 'NumberOfLightsHigh') {
                $scope.$watch('plugin.properties[' + propertyIndex + '].value', function (newValue, oldValue) {
                    if (newValue != null && oldValue != null && newValue != oldValue) {
                        rebuildLights();
                    }
                });
            }
        }
    }
    function rebuildLights() {
        var lightsProperty = findItem($scope.plugin.properties, function (property) {
            return (property.type == 'lights');
        });
        var widthProperty = findItem($scope.plugin.properties, function (property) {
            return (property.name == 'NumberOfLightsWide');
        });
        var heightProperty = findItem($scope.plugin.properties, function (property) {
            return (property.name == 'NumberOfLightsHigh');
        });

        if (lightsProperty == null || widthProperty == null || heightProperty == null){
            return;
        }

        //todo validate values

        lightsProperty.value.firstRowIndex = 0;
        lightsProperty.value.firstColumnIndex = 0;
        lightsProperty.value.lightRows = [];
        
        var colourClasses = ["btn-primary", "btn-success", "btn-info", "btn-warning", "btn-danger" ];
        var colourPosition = 0;

        for (var row = 0; row <= heightProperty.value - 1; row++)
        {
            var lightRow = {};
            lightRow.rowIndex = row;
            lightRow.lightColumns = [];

            for (var column = 0; column <= widthProperty.value - 1; column++)
            {
                var lightColumn = {};
                lightColumn.columnIndex = column;
                lightColumn.id = null;
                lightColumn.index = null;
                if (column == 0 || column == widthProperty.value - 1
                    || row == 0 || row == heightProperty.value - 1)
                    {
                    lightColumn.colourClass = colourClasses[colourPosition++];
                    
                    lightColumn.enabled = true;
                    
                    if (colourPosition >= colourClasses.length)
                    {
                        colourPosition = 0;
                    }
                } else {
                    lightColumn.colourClass = "disabled";
                }
                lightRow.lightColumns.push(lightColumn);
            }
            lightsProperty.value.lightRows.push(lightRow);
        }
    }

    $scope.update = function () {
        $http.post('/updatePlugin', {
            id: $scope.id,
            profileId: $scope.plugin.profileId,
            pluginType: $scope.plugin.pluginType,
            properties: $scope.plugin.properties,
            type: $routeParams.type
        }).success(
        function (data, textStatus, jqXHR) {
            $scope.plugin = data;
            $scope.id = data.id;
            $scope.new = false;
        });
    }

    $scope.delete = function () {
        $http.post('/deletePlugin', {
            id: $scope.id,
            profileId: $scope.plugin.profileId,
            pluginType: $scope.plugin.pluginType,
            type: $routeParams.type
        }).success(
        function (data, textStatus, jqXHR) {
            window.history.back();
        });
    }

    //Light Setup Specific funtions
    $scope.first = function(columnIndex, rowIndex, clockwise){

        var lightsProperty = findItem($scope.plugin.properties, function (property) {
            return (property.type == 'lights');
        });
        
        lightsProperty.value.firstColumnIndex = columnIndex;
        lightsProperty.value.firstRowIndex = rowIndex;
        lightsProperty.value.clockwise = clockwise;

        setupLights(lightsProperty.value, false);
    }
    $scope.disable = function (columnIndex, rowIndex) {
        var lightsProperty = findItem($scope.plugin.properties, function (property) {
            return (property.type == 'lights');
        });

        lightsProperty.value.lightRows[rowIndex].lightColumns[columnIndex].enabled = false;
        setupLights(lightsProperty.value, true);
    }
    $scope.enable = function (columnIndex, rowIndex) {
        var lightsProperty = findItem($scope.plugin.properties, function (property) {
            return (property.type == 'lights');
        });

        lightsProperty[0].value.lightRows[rowIndex].lightColumns[columnIndex].enabled = true;
        setupLights(lightsProperty.value, false);
    }

    function setupLights(lightSetup, disable) {

        var completed = false;
        var numberOfRows = 0;
        var numberOfColumns = 0;
        var currentRowIndex = lightSetup.firstRowIndex;
        var currentColumnIndex = lightSetup.firstColumnIndex;
        var currentId = 1;

        //ensure the lights array is populated
        if (lightSetup == null || lightSetup.lightRows.length == 0) {
            completed = true;
        } else if (lightSetup.lightRows[0] == null || lightSetup.lightRows[0].length == 0) {
            completed = true;
        } else {
            numberOfRows = lightSetup.lightRows.length - 1;
            numberOfColumns = lightSetup.lightRows[0].lightColumns.length - 1;
        }

        while (!completed) {
            //Each iteration of the while sets one light index

            //setting first light
            if (currentRowIndex == lightSetup.firstRowIndex && currentColumnIndex == lightSetup.firstColumnIndex &&
                currentId != 1) {
                completed = true;
            }
            else if (currentId == 1) {
                if (disable && !lightSetup.lightRows[currentRowIndex].lightColumns[currentColumnIndex].enabled) {
                    //do nothing
                } else {
                    if (disable){
                        lightSetup.firstRowIndex = currentRowIndex;
                        lightSetup.firstColumnIndex = currentColumnIndex;
                    }
                    lightSetup.lightRows[currentRowIndex].lightColumns[currentColumnIndex].id = currentId++;
                    lightSetup.lightRows[currentRowIndex].lightColumns[currentColumnIndex].index = currentId - 2;
                    lightSetup.lightRows[currentRowIndex].lightColumns[currentColumnIndex].enabled = true;
                }
            } else if (lightSetup.lightRows[currentRowIndex].lightColumns[currentColumnIndex].enabled) {
                lightSetup.lightRows[currentRowIndex].lightColumns[currentColumnIndex].id = currentId++;
                lightSetup.lightRows[currentRowIndex].lightColumns[currentColumnIndex].index = currentId - 2;
            } else {
                lightSetup.lightRows[currentRowIndex].lightColumns[currentColumnIndex].id = null;
                lightSetup.lightRows[currentRowIndex].lightColumns[currentColumnIndex].index = null;
            }


            if (completed) {
                break;
            } else if (lightSetup.clockwise) {
                //Clockwise
                if (currentRowIndex == 0 && currentColumnIndex == 0) {
                    currentColumnIndex++; // move right along top
                } else if (currentRowIndex == 0 && currentColumnIndex < numberOfColumns) {
                    currentColumnIndex++; // move right along top
                } else if (currentRowIndex == 0 && currentColumnIndex == numberOfColumns) {
                    currentRowIndex++; //move down along right
                } else if (currentRowIndex < numberOfRows && currentColumnIndex == numberOfColumns) {
                    currentRowIndex++; //move down along right
                } else if (currentRowIndex == numberOfRows && currentColumnIndex == numberOfColumns) {
                    currentColumnIndex--; //move left along bottom
                } else if (currentRowIndex == numberOfRows && currentColumnIndex > 0) {
                    currentColumnIndex--; //move left along bottom
                } else if (currentRowIndex == numberOfRows && currentColumnIndex == 0) {
                    currentRowIndex--; //move up along left
                } else if (currentRowIndex < numberOfRows && currentColumnIndex == 0) {
                    currentRowIndex--; //move up along left
                }

            } else if (!lightSetup.clockwise) {

                //Anit-Clockwise
                if (currentRowIndex == 0 && currentColumnIndex == 0) {
                    currentRowIndex++; //move Down along left
                } else if (currentRowIndex < numberOfRows && currentColumnIndex == 0) {
                    currentRowIndex++; //move Down along Left
                } else if (currentRowIndex == numberOfRows && currentColumnIndex == 0) {
                    currentColumnIndex++; //move Right along bottom
                } else if (currentRowIndex == numberOfRows && currentColumnIndex < numberOfColumns) {
                    currentColumnIndex++; //move right along bottom
                } else if (currentRowIndex == numberOfRows && currentColumnIndex == numberOfColumns) {
                    currentRowIndex--; //move up along right
                } else if (currentRowIndex > 0 && currentColumnIndex == numberOfColumns) {
                    currentRowIndex--; //move up along right
                } else if (currentRowIndex == 0 && currentColumnIndex == numberOfColumns) {
                    currentColumnIndex--; //move left along top
                } else if (currentRowIndex == 0 && currentColumnIndex < numberOfColumns) {
                    currentColumnIndex--; //move left along top
                }
            }
        }
        return lightSetup;
    }
    
}