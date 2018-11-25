angular.module('CustomerReviews.Web')
	.factory('CustomerReviews.WebApi', ['$resource', function ($resource) {
		return $resource('api/customerReviews', {}, {
			search: { method: 'POST', url: 'api/customerReviews/search' },
			update: { method: 'PUT' },
			delete: { method: 'DELETE' },
			getProductRating: { method: 'GET', url: 'api/customerReviews/productRating', params: { productId: '@productId' } }
		});
	}]);
