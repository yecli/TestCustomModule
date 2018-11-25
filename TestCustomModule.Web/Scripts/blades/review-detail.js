angular.module('CustomerReviews.Web')
	.controller('CustomerReviews.Web.reviewsDetailController', ['$scope', 'CustomerReviews.WebApi', 'platformWebApp.bladeNavigationService', function ($scope, reviewsApi, bladeNavigationService) {
		var blade = $scope.blade;
		blade.updatePermission = 'customerReview:update';

		$scope.$watch('blade.currentEntity', function () {
			$scope.isValid = blade.currentEntity && (blade.currentEntity.rating >= 1 && blade.currentEntity.rating <= 5);
		}, true);

		function initialize(item) {
			blade.currentEntity = angular.copy(item);
			blade.origEntity = angular.copy(item);
			blade.isLoading = false;
		};

		blade.toolbarCommands = [
			{
				name: 'customerReviews.blades.item-detail.commands.save',
				icon: 'fa fa-save',
				executeMethod: function () { saveChanges(); },
				canExecuteMethod: canSave,
				permission: blade.updatePermission
			},
			{
				name: 'customerReviews.blades.item-detail.commands.cancel',
				icon: 'fa fa-undo',
				executeMethod: function () { cancelChanges(); },
				canExecuteMethod: isDirty
			},
		];

		function isDirty() {
			return !angular.equals(blade.currentEntity, blade.origEntity) && blade.hasUpdatePermission();
		}

		function canSave() {
			return isDirty() && $scope.isValid;
		}

		function cancelChanges() {
			angular.copy(blade.origEntity, blade.currentEntity);
			$scope.bladeClose();
		};

		function saveChanges() {
			blade.isLoading = true;

			reviewsApi.update({}, [blade.currentEntity], function () {
				if (blade.parentBlade && blade.parentBlade.refresh) {
					blade.parentBlade.refresh();
				}
				blade.isLoading = false;
				bladeNavigationService.closeBlade(blade);
			}, function (error) {
				bladeNavigationService.setError('Error ' + error.status, blade);
				blade.isLoading = false;
			});
		};

		initialize(blade.currentEntity);
	}]);
