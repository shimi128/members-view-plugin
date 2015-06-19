angular.module('umbraco').controller("NgSoft.MembersController", function ($scope, $http, $window) {


    var init = function () {
        $scope.members = {};
        $scope.pageIndex = 0;
        $scope.pageSize = 30;
        $scope.totalRecords=0;
        $scope.currentPage=0;
        $scope.range = [];
        $scope.itemsPerPage = [
    { name: 'Show 10 per page', value: 10 },
    { name: 'Show 20 per page', value: 20 },
    { name: 'Show 25 per page', value: 25 },
    { name: 'Show 30 per page', value: 30 },
        ];
        $scope.selectedOption = $scope.itemsPerPage[3];
    };


    $scope.getMembers = function () {
       $http.get('/umbraco/BackOffice/Api/Members/Get/?pageIndex=' + $scope.pageIndex + '&pageSize=' + $scope.pageSize).success(function (result) {
            $scope.members = result.Members;
            $scope.totalRecords = result.TotlalRecords;
            $scope.currentPage = result.PageIndex;
            getRange(($scope.totalRecords / $scope.pageSize));
        });
    };

    $scope.showPager=function() {
        return $scope.totalRecords > $scope.pageSize;
    }

    $scope.showDDl=function() {
        return $scope.totalRecords > 5;
    }

    $scope.prevPage = function () {
        if ($scope.pageIndex > 0 ) {
            $scope.pageIndex = $scope.pageIndex - 1;
            $scope.getMembers();
        }
    };

    $scope.nextPage = function () {
        if ($scope.range.length > 1 && $scope.range.length -1 > $scope.pageIndex) {
            $scope.pageIndex = $scope.pageIndex + 1;
            $scope.getMembers();
        }
    };

    $scope.setPage = function (number) {
        $scope.pageIndex = number!==undefined ? number  : this.n;
        $scope.getMembers();
    };

    $scope.setChange = function () {
        if ($scope.pageSize === this.selectedOption.value)
            return;
        $scope.pageSize = this.selectedOption.value;
        $scope.setPage(0);
        
       
    };

    $scope.excel = function () {
        $window.location.href = '/umbraco/BackOffice/Api/Members/GetExcel/';
    };

    $scope.editMember = function () {
        $window.location.href = '/umbraco/#/member/member/edit/' + this.member.Key;
    };

    function getRange(total) {
        var range = [];
        for (var i = 0; i < total; i++) {
            range.push(i);
        }
        $scope.range = range;
    };

    init();

    $scope.getMembers($scope.pageIndex, $scope.pageSize);
});