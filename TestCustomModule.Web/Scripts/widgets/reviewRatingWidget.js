angular.module('CustomerReviews.Web')
	.controller('CustomerReviews.Web.reviewRatingWidgetController', ['$scope', 'CustomerReviews.WebApi', '$translate', function ($scope, reviewsApi, $translate) {
		var blade = $scope.blade;

		function refresh() {
			$scope.loading = true;
			reviewsApi.getProductRating({ productId: blade.itemId }, function (data) {
				$scope.loading = false;
				$scope.ratingValue = data.ratingValue;
				$scope.noRatings = typeof $scope.ratingValue === 'undefined';
			}, function (error) {
				bladeNavigationService.setError('Error ' + error.status, blade);
				blade.isLoading = false;
			});
		}

		$scope.$watch("blade.itemId", function (id) {
			if (id) refresh();
		});
	}]);
