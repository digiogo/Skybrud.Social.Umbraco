angular.module("umbraco").controller("Skybrud.Social.Tumblr.OAuth.Controller", ['$scope', 'editorState', function ($scope, editorState) {

    // Define an alias for the editor (eg. used for callbacks)
    var alias = ('skybrudsocial_' + Math.random()).replace('.', '');

    // Get a reference to the current editor state
    var state = editorState.current;

    $scope.expiresDays = null;
    
    $scope.callback = function (data) {

        $scope.model.value = data;
        updateBlogsInfoListUI();
    };

    function updateBlogsInfoListUI() {

        var array = $scope.model.value.blogs;
        if (array) {
            $scope.blogs = array;
        }

        //Whether there is a selected blog, bind data
        if ($scope.model.value && $scope.model.value.selected_blog) {
            $scope.selectedBlogInfo = $scope.model.value.selected_blog;
        }

        //Register onChange event
        $scope.blogInfoSelected = function (selectedBlog) {
            $scope.selectedBlogInfo = selectedBlog;
            $scope.model.value.selected_blog = selectedBlog;//persists on Umbraco model
        };
    }

    $scope.authorize = function () {

        var url = '/App_Plugins/Skybrud.Social/Dialogs/TumblrOAuth.aspx?callback=' + alias;
        url += "&contentTypeAlias=" + state.contentTypeAlias;
        url += "&propertyAlias=" + $scope.model.alias;

        window.open(url, 'Tumblr OAuth', 'scrollbars=no,resizable=yes,menubar=no,width=480,height=720');
    };

    $scope.clear = function () {

        $scope.model.value = null;

    };
    

    //load stored data in content editor (UI)
    updateBlogsInfoListUI();

    // Register the callback function in the global scope
    window[alias] = $scope.callback;

}]);

angular.module("umbraco").controller("Skybrud.Social.Tumblr.OAuth.PreValues.Controller", ['$scope', 'assetsService', function ($scope, assetsService) {
    
    if (!$scope.model.value) {
        $scope.model.value = {
            appid: '',
            appsecret: '',
            redirecturi: ''
        };
    }

    $scope.suggestedRedirectUri = window.location.origin + '/App_Plugins/Skybrud.Social/Dialogs/TumblrOAuth.aspx';
    assetsService.loadCss("/App_Plugins/Skybrud.Social/Social.css");

}]);