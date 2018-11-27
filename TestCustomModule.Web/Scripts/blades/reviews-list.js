angular.module('CustomerReviews.Web')
    .controller('CustomerReviews.Web.reviewsListController', ['$scope', 'CustomerReviews.WebApi', 'platformWebApp.bladeUtils', 'uiGridConstants', 'platformWebApp.uiGridHelper',
        function ($scope, reviewsApi, bladeUtils, uiGridConstants, uiGridHelper) {
            $scope.uiGridConstants = uiGridConstants;

            var blade = $scope.blade;
            var bladeNavigationService = bladeUtils.bladeNavigationService;

            blade.refresh = function () {
                blade.isLoading = true;
                reviewsApi.search(angular.extend(filter, {
                    searchPhrase: filter.keyword ? filter.keyword : undefined,
                    sort: uiGridHelper.getSortExpression($scope),
                    skip: ($scope.pageSettings.currentPage - 1) * $scope.pageSettings.itemsPerPageCount,
                    take: $scope.pageSettings.itemsPerPageCount
                }), function (data) {
                    blade.isLoading = false;
                    $scope.pageSettings.totalItems = data.totalCount;
                    blade.currentEntities = data.results;
                });
            }

            blade.selectNode = function (data) {
				openReviewDetails(data);
			}

			function openReviewDetails(nodeData) {
				$scope.selectedNodeId = nodeData ? nodeData.id : null;

				var newBlade = {
					id: 'reviewDetails',
					currentEntityId: $scope.selectedNodeId,
					currentEntity: nodeData || {},
					isNew: nodeData == null,
					title: 'customerReviews.widgets.item-detail.title',
					subtitle: 'customerReviews.widgets.item-detail.subtitle',
					controller: 'CustomerReviews.Web.reviewDetailController',
					template: 'Modules/$(TestCustomModule.Web)/Scripts/blades/review-detail.tpl.html'
				};
				bladeNavigationService.showBlade(newBlade, blade);
			}

            function openBladeNew() {
				openReviewDetails(null);
            }

            blade.headIcon = 'fa-comments';

            blade.toolbarCommands = [
                {
                    name: "platform.commands.refresh", icon: 'fa fa-refresh',
                    executeMethod: blade.refresh,
                    canExecuteMethod: function () {
                        return true;
                    },
					permission: 'customerReview:update'
                },
     //           {
     //               name: "platform.commands.add", icon: 'fa fa-plus',
     //               executeMethod: openBladeNew,
     //               canExecuteMethod: function () {
     //                   return true;
     //               },
					//permission: 'customerReview:update'
     //           }
			];

            // simple and advanced filtering
            var filter = $scope.filter = blade.filter || {};

            filter.criteriaChanged = function () {
                if ($scope.pageSettings.currentPage > 1) {
                    $scope.pageSettings.currentPage = 1;
                } else {
                    blade.refresh();
                }
            };

            // ui-grid
            $scope.setGridOptions = function (gridOptions) {
                uiGridHelper.initialize($scope, gridOptions, function (gridApi) {
                    uiGridHelper.bindRefreshOnSortChanged($scope);
                });
                bladeUtils.initializePagination($scope);
            };

        }]);
