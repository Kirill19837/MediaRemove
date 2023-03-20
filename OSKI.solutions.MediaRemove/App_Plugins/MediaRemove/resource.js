angular.module("umbraco.resources")
        .factory("Our.Umbraco.MediaRemove.Resource", function ($http) {
            return {
                getRebuildStatus : function() {
                    return $http.get("/umbraco/api/mediaRemove/" + "GetRebuildStatus");
                },
                rebuild : function(id) {
                    return $http.get("/umbraco/api/mediaRemove/" + "Rebuild");
                },
                getUnusedMedia : function() {
                    return $http.get("/umbraco/api/mediaRemove/" + "GetUnusedMedia");
                },
                deleteUnusedMedia: function(ids) {
                    return $http.post("/umbraco/api/mediaRemove/" + "DeleteUnusedMedia", ids);
                },
                getUnusedMediaStatus: function() {
                    return $http.get("/umbraco/api/mediaRemove/" + "GetUnusedMediaStatus");
                },
                getBuiltStatus : function() {
                    return $http.get("/umbraco/api/mediaRemove/" + "IsBuilt");
                },
                getDeleteMediaStatus: function () {
                    return $http.get("/umbraco/api/mediaRemove/" + "DeleteUnusedMediaStatus");
                }
            };
        });