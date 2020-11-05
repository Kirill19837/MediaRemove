(function () {
    'use strict';

    angular.module("umbraco.directives")
        .directive('onReadFile', function ($parse) {

            function link(scope, element, attrs) {
                var fn = $parse(attrs.onReadFile);

                element.on('change',
                    function (onChangeEvent) {
                        const file = (onChangeEvent.srcElement || onChangeEvent.target).files[0];
                        if (!file.name.endsWith(".txt")) {
                            (onChangeEvent.srcElement || onChangeEvent.target).files[0] = null;
                            return;
                        }
                        var reader = new FileReader();

                        reader.onload = function (onLoadEvent) {
                            scope.$apply(function () {
                                fn(scope, {
                                    fileContent: onLoadEvent.target.result,
                                    fileName: file.name
                                });
                            });
                        };
                        reader.readAsText(file);
                    });
            }

            var directive = {
                link: link,
                restrict: 'A',
                transclude: true
            };

            return directive;
        });

})();