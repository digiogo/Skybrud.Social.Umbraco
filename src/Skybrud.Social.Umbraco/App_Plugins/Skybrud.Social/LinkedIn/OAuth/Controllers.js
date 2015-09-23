angular.module("umbraco").controller("Skybrud.Social.LinkedIn.OAuth.Controller", ['$scope', 'editorState', function ($scope, editorState) {

    // Define an alias for the editor (eg. used for callbacks)
    var alias = ('skybrudsocial_' + Math.random()).replace('.', '');

    // Get a reference to the current editor state
    var state = editorState.current;

    $scope.expiresDays = null;
    
    $scope.callback = function (data) {

        $scope.model.value = data;
        updateUI();

    };

    $scope.authorize = function () {

        var url = '/App_Plugins/Skybrud.Social/Dialogs/LinkedInOAuth.aspx?callback=' + alias;
        url += "&contentTypeAlias=" + state.contentTypeAlias;
        url += "&propertyAlias=" + $scope.model.alias;

        window.open(url, 'LinkedIn OAuth', 'scrollbars=no,resizable=yes,menubar=no,width=480,height=720');
    };

    $scope.clear = function () {

        $scope.model.value = null;

    };
    
    function format(number, decimals) {
        var text = Math.round(number * 10 * decimals);
        return text / 10 / decimals;
    }

    function updateUI() {

        if ($scope.model.value && $scope.model.value.expires_at) {

            var seconds = (new Date($scope.model.value.expires_at) - new Date()) / 1000;
            $scope.expiresDays = format(seconds / 60 / 60 / 24, 2);

        } else {

            $scope.expiresDays = NaN;

        }

    }

    //load stored data in content editor (UI)
    updateUI();

    // Register the callback function in the global scope
    window[alias] = $scope.callback;

}]);

angular.module("umbraco").controller("Skybrud.Social.LinkedIn.OAuth.PreValues.Controller", ['$scope', 'assetsService', function ($scope, assetsService) {
    
    if (!$scope.model.value) {
        $scope.model.value = {
            appid: '',
            appsecret: '',
            redirecturi: '',
            permissions: []
        };
    }

    $scope.allPermissions = ["r_basicprofile", "r_emailaddress", "rw_company_admin", "w_share"];//TODO: remove all elements after UI loads and then rebuild array only with current checked permissions
    $scope.suggestedRedirectUri = window.location.origin + '/App_Plugins/Skybrud.Social/Dialogs/LinkedInOAuth.aspx';

    assetsService.loadCss("/App_Plugins/Skybrud.Social/Social.css");

    $scope.toggleSelection = function toggleSelection(permName) {
        var idx = $scope.model.value.permissions.indexOf(permName);

        if (idx > -1) {// is currently selected
            $scope.model.value.permissions.splice(idx, 1);
        }
        else {// is newly selected
            $scope.model.value.permissions.push(permName);
        }
    };
}]);