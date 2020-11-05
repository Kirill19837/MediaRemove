angular.module('umbraco').controller('Our.Umbraco.Nexu.DashboardController',
    ['$scope', 'Our.Umbraco.Nexu.Resource', '$timeout', function ($scope, nexuResource, $timeout) {
        $scope.isLoading = true;
        $scope.RebuildStatus = {
            IsProcessing: true,
            ItemName: '',
            ItemsProcessed: 0
        };
        $scope.isBuiltRelations = false;
        $scope.unusedMedia = {
            IsProcessingMedia: false,
            Data : []
        };
        $scope.filteredMedia = [];
        $scope.autoRefresh = true;
        $scope.exceptionListSource = null;
        $scope.exceptionSources = [];

        $scope.preventDelete = Umbraco.Sys.ServerVariables.Nexu.PreventDelete;
        $scope.preventUnPublish = Umbraco.Sys.ServerVariables.Nexu.PreventUnPublish;

        $scope.getRebuildStatus = function () {
            nexuResource.getRebuildStatus()
                .then(function (result) {
                    $scope.isLoading = false;
                    $scope.RebuildStatus = result.data;

                    if ($scope.RebuildStatus.IsProcessing && $scope.autoRefresh) {
                        $timeout(function () { $scope.getRebuildStatus() }, 5000, true);
                    }
                });
        };

        $scope.rebuild = function () {
            if ($scope.isBuiltRelations) {
                return;
            }
            nexuResource.rebuild(-1)
                .then(function (result) {
                    $scope.getRebuildStatus();
                    $scope.isBuiltRelations = true;
                });
            $timeout(function () { $scope.getRebuildStatus() }, 500, true);
        };

        $scope.getBuiltStatus = function() {
            nexuResource.getBuiltStatus().then(function ({ data }) {
                $scope.isBuiltRelations = data.IsBuilt;
            });
        }

        $scope.getUnusedMedia = function () {
            if ($scope.unusedMedia.IsProcessingMedia) {
                return;
            }
            nexuResource.getUnusedMedia()
                .then(function () {
                    $scope.getUnusedMediaStatus();
                });
        };

        $scope.getUnusedMediaStatus = function() {
            nexuResource.getUnusedMediaStatus()
                .then(function ({ data }) {
                    console.log(data);
                    $scope.unusedMedia = data;
                    data.Data.forEach((x) => {
                        x.ToRemove = true;
                        if (x.Source) {
                            try {
                                x.Source = JSON.parse(x.Source).src;
                            } catch (ex) {
                                console.log(ex);
                            }
                            if ($scope.exceptionSources.indexOf(x.Source) > -1) {
                                x.ToRemove = false;
                            }
                        }
                    });
                    $scope.filteredMedia = data.Data;
                    if ($scope.unusedMedia.IsProcessingMedia) {
                        $timeout(function () { $scope.getUnusedMediaStatus() }, 5000, true);
                    }
                });
        };

        $scope.deleteUnusedMedia = function () {
            if ($scope.filteredMedia.length == 0 && $scope.unusedMedia.IsProcessingMedia) {
                return;
            }
            let forDeleting = [];
            let forLeaving = [];
            $scope.filteredMedia.forEach((x) => {
                if (x.ToRemove) {
                    forDeleting.push(x);
                } else {
                    forLeaving.push(x);
                }
            });
            let ids = forDeleting.map(x => x.Id);
            nexuResource.deleteUnusedMedia(ids)
                .then(function () {
                    $scope.filteredMedia = forLeaving;
                });
        };

        $scope.$watch('autoRefresh', function () {
            if ($scope.autoRefresh === true) {
                $scope.getRebuildStatus();
            }
        }, true);


        $scope.showContent = function ( fileContent, fileName ) {
            $scope.exceptionListSource = fileName;
            $scope.exceptionSources = fileContent.split(/\r\n|\n/);;
            $scope.unusedMedia.Data.forEach((x) => {
                if ($scope.exceptionSources.indexOf(x.Source) > -1) {
                    x.ToRemove = false;
                } else {
                    x.ToRemove = true;
                }
            });
            $scope.filteredMedia = $scope.unusedMedia.Data;
        }

        $scope.getRebuildStatus();
        $scope.getUnusedMediaStatus();
        $scope.getBuiltStatus();
    }]);