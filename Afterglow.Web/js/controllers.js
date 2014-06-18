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

function HomeController($scope, $route, $routeParams, $location) {
    $scope.$route = $route;
    $scope.$location = $location;
    $scope.$routeParams = $routeParams;

    $scope.timer = null;
    $scope.previewRunning = false;

    $scope.$watch('previewRunning', function (newValue, oldValue) {
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

        $.post('/runtime', { Start: true },
        function (data, textStatus, jqXHR) {

            $scope.previewRunning = data.Active;

            generatePreviewGrid(data);

            if ($($("#previewGroup").children()[1]).height() != 0) {
                timer = setInterval(getPreview, 100);
            }
        }, 'json');
    }

    $scope.stopPreview = function () {
        $.post('/runtime', { Start: true },
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

        $.get("/preview", {}, function (data, status, xhr) {
            clearInterval(timer);
            timer = null;
            $(data.Lights).each(function (index, item) {
                $("#previewLight_" + item.Top + "_" + item.Left).css("background-color", "#" + item.Colour.R.toString(16) + item.Colour.G.toString(16) + item.Colour.B.toString(16));
            });
            timer = setInterval(getPreview, 100);
        });

    }

    function generatePreviewGrid(setup) {
        var cellHeight = 30;
        var cellWidth = 30;

        var numberHigh = setup.NumberOfLightsHigh;
        var numberWide = setup.NumberOfLightsWide;

        $("#previewScreen").children().remove();

        var height = cellHeight * numberHigh;
        var width = cellWidth * numberWide;

        $("#previewScreen").attr('style', 'position:relative;width:' + width + 'px; height:' + height + 'px');

        var centreDivHeight = cellHeight * (numberHigh > 2 ? numberHigh - 2 : 0);
        var centreDivWidth = cellWidth * (numberWide > 2 ? numberWide - 2 : 0);

        $("#previewScreen").append('<div style="display:inline-block;border: 1px solid transparent;border-radius: 4px;position: absolute;background-color:#6495ed;height:' + (centreDivHeight-3) + 'px; width:' + (centreDivWidth-3) + 'px; top:' + (cellHeight+1) + 'px; left:' + (cellWidth+1) + 'px; float:left;" ></div>');

        for (var topPosition = 0; topPosition < numberHigh; topPosition++) {
            for (var leftPosition = 0; leftPosition < numberWide; leftPosition++) {
                //Only add 
                if (topPosition == 0 || topPosition == numberHigh - 1 || leftPosition == 0 || leftPosition == numberWide - 1) {
                    var top = cellHeight * topPosition;
                    var left = cellWidth * leftPosition;
                    $("#previewScreen").append('<div id="previewLight_' + topPosition + '_' + leftPosition + '" style="display:inline-block;border: 1px solid transparent;border-radius: 4px;position: absolute;background-color:#b0c4de;height:' + (cellHeight-1) + 'px; width:' + (cellWidth-1) + 'px; top:' + top + 'px; left:' + left + 'px; float:left;" ></div>');
                }
            }
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

function ProfileController($scope, $location, $routeParams, $http) {
    $scope.id = $routeParams.id;

    $scope.profile = null;
    $scope.modal = null;

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
        $scope.modal = {
            title: modalTitle,
            plugins: [],
            pluginType: pluginType
        };
        $http.post('/pluginTypes', {
            pluginType: pluginType
        }).success(
        function (data, textStatus, jqXHR) {
            $scope.modal.plugins = data.plugins;
            $('#pluginTypeSelectionModal').show();
        });
    }

    $scope.confirmAdd = function (pluginType, plugin) {
        $('#pluginTypeSelectionModal').hide();
        $location.path('/plugin/'+ $scope.id + '/' + pluginType + '/new/' + plugin);
    }
}

function PluginController($scope, $route, $routeParams, $http) {
    $scope.id = $routeParams.id;

    $scope.new = false;
    $scope.allowDelete = false;

    $scope.plugin = null;

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

        var lightsProperty = $.grep($scope.plugin.properties, function (property) {
            return (property.type == 'lights');
        });
        if (lightsProperty.length == 0) {
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
        var lightsProperty = $.grep($scope.plugin.properties, function (property) {
            return (property.type == 'lights');
        });
        var widthProperty = $.grep($scope.plugin.properties, function (property) {
            return (property.name == 'NumberOfLightsWide');
        });
        var heightProperty = $.grep($scope.plugin.properties, function (property) {
            return (property.name == 'NumberOfLightsHigh');
        });

        if (lightsProperty[0] == null || widthProperty[0] == null || heightProperty[0] == null){
            return;
        }

        //todo validate values

        lightsProperty[0].value.firstRowIndex = 0;
        lightsProperty[0].value.firstColumnIndex = 0;
        lightsProperty[0].value.lightRows = [];
        
        var colourClasses = ["btn-primary", "btn-success", "btn-info", "btn-warning", "btn-danger" ];
        var colourPosition = 0;

        for (var row = 0; row <= heightProperty[0].value - 1; row++)
        {
            var lightRow = {};
            lightRow.rowIndex = row;
            lightRow.lightColumns = [];

            for (var column = 0; column <= widthProperty[0].value - 1; column++)
            {
                var lightColumn = {};
                lightColumn.columnIndex = column;
                lightColumn.id = null;
                lightColumn.index = null;
                if (column == 0 || column == widthProperty[0].value - 1
                    || row == 0 || row == heightProperty[0].value - 1)
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
            lightsProperty[0].value.lightRows.push(lightRow);
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

        var lightsProperty = $.grep($scope.plugin.properties, function (property) {
            return (property.type == 'lights');
        });
        
        lightsProperty[0].value.firstColumnIndex = columnIndex;
        lightsProperty[0].value.firstRowIndex = rowIndex;
        lightsProperty[0].value.clockwise = clockwise;

        setupLights(lightsProperty[0].value, false);
    }
    $scope.disable = function (columnIndex, rowIndex) {
        var lightsProperty = $.grep($scope.plugin.properties, function (property) {
            return (property.type == 'lights');
        });

        lightsProperty[0].value.lightRows[rowIndex].lightColumns[columnIndex].enabled = false;
        setupLights(lightsProperty[0].value, true);
    }
    $scope.enable = function (columnIndex, rowIndex) {
        var lightsProperty = $.grep($scope.plugin.properties, function (property) {
            return (property.type == 'lights');
        });

        lightsProperty[0].value.lightRows[rowIndex].lightColumns[columnIndex].enabled = true;
        setupLights(lightsProperty[0].value, false);
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