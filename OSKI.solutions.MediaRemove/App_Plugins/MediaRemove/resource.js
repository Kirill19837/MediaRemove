angular.module("umbraco.resources")
        .factory("Our.Umbraco.MediaRemove.Resource", function ($http) {
            return {
                getRebuildStatus : function() {
                    return $http.get(Umbraco.Sys.ServerVariables.MediaRemove.RebuildApi + "GetRebuildStatus");
                },
                rebuild : function(id) {
                    return $http.get(Umbraco.Sys.ServerVariables.MediaRemove.RebuildApi + "Rebuild");
                },
                getUnusedMedia : function() {
                    return $http.get(Umbraco.Sys.ServerVariables.MediaRemove.GetUnusedMedia);
                },
                deleteUnusedMedia: function(ids) {
                    return $http.post(Umbraco.Sys.ServerVariables.MediaRemove.DeleteUnusedMedia, ids);
                },
                getUnusedMediaStatus: function() {
                    return $http.get(Umbraco.Sys.ServerVariables.MediaRemove.GetUnusedMediaStatus);
                },
                getBuiltStatus : function() {
                    return $http.get(Umbraco.Sys.ServerVariables.MediaRemove.IsBuilt);
                },
                getDeleteMediaStatus: function () {
                    return $http.get(Umbraco.Sys.ServerVariables.MediaRemove.DeleteUnusedMediaStatus);
                }
            };
        });