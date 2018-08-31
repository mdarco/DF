﻿(function () {
    'use strict';

    angular
        .module('DFApp')
        .controller('MembersController', ctrlFn);

    ctrlFn.$inject = ['$rootScope', '$scope', '$location', '$uibModal', 'NgTableParams', 'AuthenticationService', 'MembersService', 'LookupsService', 'toastr', 'choreos', 'danceGroups', 'danceSelections', 'events'];

    function ctrlFn($rootScope, $scope, $location, $uibModal, NgTableParams, AuthenticationService, MembersService, LookupsService, toastr, choreos, danceGroups, danceSelections, events) {
        // set active menu item
        $("#left-panel nav ul li").removeClass("active");
        $("#menuMembers").addClass("active");

        // prevent 'Enter' to submit the form
        $('#searchForm').bind('keydown', function (e) {
            if (e.keyCode === 13) {
                e.preventDefault();
            }
        });

        var currentUser = AuthenticationService.getCurrentUser();

        var isBack = false; // determines if we're back from the dossier page
        var queryString = $location.search();
        if (queryString && queryString.back) {
            isBack = queryString.back;
        }

        $scope.choreos = choreos;
        $scope.danceGroups = danceGroups;
        $scope.danceSelections = danceSelections;
        $scope.events = events;

        $scope.statuses = [
            { ID: 'all', Name: 'Svi' },
            { ID: 'active', Name: 'Aktivni' }//,
            //{ ID: 'inactive', Name: 'Neaktivni' }
        ];

        //#region Filter members

        $scope.totalRecords = 0;
        $scope.initialDocListLoad = true;
        $scope.showGrid = false;

        $scope.membersTableParams = new NgTableParams(
            {
                page: 1,
                count: 10
            },
            {
                total: 0,
                getData: function (params) {
                    if ($scope.initialDocListLoad) {
                        $scope.initialDocListLoad = false;
                        return [];
                    }

                    $scope.filter = $scope.filter || {};

                    $scope.filter.PageNo = params.page();
                    $scope.filter.RecordsPerPage = params.count();

                    resolveStatusFilter();

                    MembersService.setSearchFilter(angular.copy($scope.filter));

                    return MembersService.getFiltered($scope.filter).then(
                        function (result) {
                            if (!result || !result.data || !result.data.Data) {
                                $scope.showGrid = false;
                                return [];
                            }

                            if (result.data.Data.length > 0) {
                                $scope.showGrid = true;
                            } else {
                                $scope.showGrid = false;
                            }

                            params.total(result.data.Total);
                            $scope.totalRecords = result.data.Total;

                            return result.data.Data;
                        },
                        function (error) {
                            toastr.error('Došlo je do greške prilikom pretraživanja.');
                            $scope.showGrid = false;
                        }
                    );
                }
            }
        );

        $scope.applyFilter = function () {
            //$scope.newSearch = true;
            $scope.membersTableParams.reload();
        };

        $scope.clearFilter = function () {
            $scope.filter = {};
            $scope.showGrid = false;
            //$scope.newSearch = true;
            $scope.totalRecords = 0;
            $scope.membersTableParams.data = {};
        };

        function resolveStatusFilter() {
            if (!$scope.filter.Status) {
                $scope.filter.ExcludeNonActive = false;
                return;
            }

            switch ($scope.filter.Status.toLowerCase()) {
                case '':
                case 'all':
                    $scope.filter.ExcludeNonActive = false;
                    break;

                case 'active':
                    $scope.filter.ExcludeNonActive = true;
                    break;

                default:
                    $scope.filter.ExcludeNonActive = false;
                    break;
            }
        }

        //#endregion

        if (isBack) {
            if (MembersService.getSearchFilter()) {
                $scope.filter = MembersService.getSearchFilter();
                $scope.applyFilter();
            }
        } else {
            if ($scope.danceGroups && $scope.danceGroups.length > 0) {
                $scope.filter = {
                    DanceGroupID: $scope.danceGroups[0].ID
                };
            }
        }

        //#region Add member

        $scope.addMember = function () {
            var dialogOpts = {
                backdrop: 'static',
                keyboard: false,
                backdropClick: false,
                templateUrl: 'pages/members/member-dialog/member-dialog.html',
                controller: 'MemberDialogController',
                resolve: {
                    ageCategories: function (LookupsService) {
                        return LookupsService.getAgeCategories().then(
                            function (result) {
                                return result.data;
                            }
                        );
                    }
                }
            };

            var dialog = $uibModal.open(dialogOpts);

            dialog.result.then(
                function () {
                    $scope.applyFilter();
                },
                function () {
                    // modal dismissed => do nothing
                }
            );
        };

        //#endregion

        //#region Member dossier

        $scope.openMemberDossier = function (member) {
            MembersService.setSearchFilter(angular.copy($scope.filter));
            $location.path('/member-file/' + member.MemberID);
        };

        //#endregion
    }
})();
