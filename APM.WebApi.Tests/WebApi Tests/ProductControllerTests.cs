using APM.WebApi.Controllers;
using APM.WebApi.Models;
using APM.WebApi.Repositories;
using APM.WebApi.Services;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Xunit;

namespace APM.WebApi.Tests
{
    public class ProductControllerTests : IDisposable
    {
        private IProductRepository ProductRepository;
        private ICurrencyConversionService CurrencyConversionService;
        private ProductsController ProductsController;
        private List<Product> MockedProducts = new List<Product>();
        private Product MockedProduct;
        public ProductControllerTests()
        {
            ProductRepository = A.Fake<IProductRepository>();
            CurrencyConversionService = A.Fake<ICurrencyConversionService>();
            ProductsController = new ProductsController(ProductRepository, CurrencyConversionService);

            SetupProductMocks();
            SetupHttpRequestMocks();
            SetupFakes();
        }

        #region Mocks and Fakes
        private void SetupProductMocks()
        {
            //arrange
            MockedProducts.Add(
                new Product()
                {
                    ProductId = 1,
                    ProductName = "FirstProduct",
                    ProductCode = "ABC-1111",
                    Price = 1.99M,
                    Description = "My nice first product",
                    ReleaseDate = DateTime.Now.AddDays(-10)
                });

            MockedProducts.Add(
                new Product()
                {
                    ProductId = 2,
                    ProductName = "MySecondProduct",
                    ProductCode = "ZZZ-2222",
                    Price = 100.99M,
                    Description = "My superb second product",
                    ReleaseDate = DateTime.Now
                });

            MockedProduct = new Product()
            {
                ProductId = 3,
                ProductName = "Product",
                ProductCode = "XXX-9999",
                Price = 1000M,
                Description = "My awesome third mocked product",
                ReleaseDate = DateTime.Now.AddDays(10)
            };

        }

        /// <summary>
        /// This must be done in order to test ModelState validations
        /// </summary>
        private void SetupHttpRequestMocks()
        {
            //Mocking HttpRequest
            HttpConfiguration configuration = new HttpConfiguration();
            HttpRequestMessage request = new HttpRequestMessage();
            ProductsController.Request = request;
            ProductsController.Request.Properties["MS_HttpConfiguration"] = configuration;
        }

        private void SetupFakes()
        {
            //arrange
            A.CallTo(() => ProductRepository.Retrieve()).Returns(MockedProducts);
        }

        #endregion

        [Fact]
        public void GetAll_ReturnsSuccess()
        {
            //act
            var result = ProductsController.Get() as OkNegotiatedContentResult<IQueryable<Product>>;

            //assert
            A.CallTo(() => ProductRepository.Retrieve()).MustHaveHappened();
            Assert.NotNull(result);
            Assert.Equal(result.Content, MockedProducts);
        }

        [Fact]
        public void GetById_ExistentId_ReturnsSuccess()
        {
            //act
            var result = ProductsController.Get(MockedProducts[0].ProductId) as OkNegotiatedContentResult<Product>;

            //assert
            A.CallTo(() => ProductRepository.Retrieve()).MustHaveHappened();
            Assert.NotNull(result);
            Assert.Equal(result.Content, MockedProducts[0]);
        }

        [Theory]
        [InlineData(9999999)]
        [InlineData(999)]
        public void GetById_PositiveNonExistentId_ReturnsNotFound(int id)
        {
            //arrange
            MockedProduct.ProductId = id;

            //act
            var result = ProductsController.Get(id);

            //assert
            A.CallTo(() => ProductRepository.Retrieve()).MustHaveHappened();
            Assert.NotNull(result);
            Assert.True(result is NotFoundResult);
        }


        [Theory]
        [InlineData(-9)]
        [InlineData(-9999999)]
        public void GetById_NegativeId_ReturnsNotFound(int id)
        {
            //arrange

            MockedProduct.ProductId = id;
            //act
            var result = ProductsController.Get(id);

            //assert
            A.CallTo(() => ProductRepository.Retrieve()).MustNotHaveHappened();
            Assert.NotNull(result);
            Assert.True(result is NotFoundResult);
        }

        [Fact]
        public void PostProduct_NullProduct_ReturnsBadRequest()
        {
            //arrange
            Product product = null;
            A.CallTo(() => ProductRepository.Save(A<Product>.Ignored)).Returns(product);

            //act
            var result = ProductsController.Post(product);

            //assert
            A.CallTo(() => ProductRepository.Save(A<Product>.Ignored)).MustNotHaveHappened();
            Assert.NotNull(result);
            Assert.True(result is BadRequestErrorMessageResult || result is BadRequestResult);
        }

        [Theory]
        [InlineData(null)]//"Product Code is required"
        [InlineData("123")]//"Product Code min length is 6 characters"
        public void PostProduct_InvalidProductCode_ReturnsInvalidModelState(string productCode)
        {
            //Arrange
            MockedProduct.ProductCode = productCode;

            //Act
            ProductsController.Validate(MockedProduct);//raises the ValidateModel validation
            A.CallTo(() => ProductRepository.Save(A<Product>.Ignored)).Returns(MockedProduct);
            var result = ProductsController.Post(MockedProduct);

            //Assert
            A.CallTo(() => ProductRepository.Save(A<Product>.Ignored)).MustNotHaveHappened();
            Assert.NotNull(result);
            Assert.True(result is InvalidModelStateResult);
        }


        [Theory]
        [InlineData("")]//"Product Name is required"
        [InlineData("123")]//"Product Name min length is 4 characters"
        [InlineData("MyproductName1234567890")]//"Product Name max length is 12 characters"
        public void PostProduct_InvalidProductName_ReturnsInvalidModelState(string productName)
        {
            //Arrange
            MockedProduct.ProductName = productName;

            //Act
            ProductsController.Validate(MockedProduct);//raises the ValidateModel validation
            A.CallTo(() => ProductRepository.Save(A<Product>.Ignored)).Returns(MockedProduct);
            var result = ProductsController.Post(MockedProduct);

            //Assert
            A.CallTo(() => ProductRepository.Save(A<Product>.Ignored)).MustNotHaveHappened();
            Assert.NotNull(result);
            Assert.True(result is InvalidModelStateResult);
        }

        [Fact]
        public void PostProduct_ValidProduct_ReturnsCreated()
        {
            //Act
            ProductsController.Validate(MockedProduct);//raises the ValidateModel validation
            A.CallTo(() => ProductRepository.Save(A<Product>.Ignored)).Returns(MockedProduct);
            var result = ProductsController.Post(MockedProduct) as CreatedNegotiatedContentResult<Product>;

            //Assert
            A.CallTo(() => ProductRepository.Save(A<Product>.Ignored)).MustHaveHappened();
            Assert.NotNull(result);
            Assert.Equal(result.Content, MockedProduct);
        }

        [Fact]
        public void PutProduct_NullProduct_ReturnsBadRequest()
        {
            //arrange
            Product product = null;
            A.CallTo(() => ProductRepository.Save(A<Product>.Ignored)).Returns(product);

            //act
            var result = ProductsController.Put(0, product);

            //assert
            A.CallTo(() => ProductRepository.Save(A<Product>.Ignored)).MustNotHaveHappened();
            Assert.NotNull(result);
            Assert.True(result is BadRequestErrorMessageResult || result is BadRequestResult);
        }

        [Theory]
        [InlineData(null)]//"Product Code is required"
        [InlineData("123")]//"Product Code min length is 6 characters"
        public void PutProduct_InvalidProductCode_ReturnsInvalidModelState(string productCode)
        {
            //Arrange
            MockedProduct.ProductCode = productCode;

            //Act
            ProductsController.Validate(MockedProduct);//raises the ValidateModel validation
            A.CallTo(() => ProductRepository.Save(A<Product>.Ignored)).Returns(MockedProduct);
            var result = ProductsController.Put(MockedProduct.ProductId, MockedProduct);

            //Assert
            A.CallTo(() => ProductRepository.Save(A<Product>.Ignored)).MustNotHaveHappened();
            Assert.NotNull(result);
            Assert.True(result is InvalidModelStateResult);
        }


        [Theory]
        [InlineData("")]//"Product Name is required"
        [InlineData("123")]//"Product Name min length is 4 characters"
        [InlineData("MyproductName1234567890")]//"Product Name max length is 12 characters"
        public void PutProduct_InvalidProductName_ReturnsInvalidModelState(string productName)
        {
            //Arrange
            MockedProduct.ProductName = productName;

            //Act
            ProductsController.Validate(MockedProduct);//raises the ValidateModel validation
            A.CallTo(() => ProductRepository.Save(A<Product>.Ignored)).Returns(MockedProduct);
            var result = ProductsController.Put(MockedProduct.ProductId,MockedProduct);

            //Assert
            A.CallTo(() => ProductRepository.Save(A<Product>.Ignored)).MustNotHaveHappened();
            Assert.NotNull(result);
            Assert.True(result is InvalidModelStateResult);
        }

        [Fact]
        public void PutProduct_NotFoundProduct_ReturnsNotFound()
        {
            //Arrange
            MockedProduct.ProductId = 4;

            //Act
            ProductsController.Validate(MockedProduct);//raises the ValidateModel validation
            A.CallTo(() => ProductRepository.Save(A<Product>.Ignored)).Returns(null);
            var result = ProductsController.Put(MockedProduct.ProductId, MockedProduct);

            //Assert
            A.CallTo(() => ProductRepository.Save(A<Product>.Ignored)).MustHaveHappened();
            Assert.NotNull(result);
            Assert.True(result is NotFoundResult);
        }

        [Fact]
        public void PutProduct_ValidProduct_ReturnsSuccess()
        {
            //Arrange
            MockedProduct.ProductId = 2;

            //Act
            ProductsController.Validate(MockedProduct);//raises the ValidateModel validation
            A.CallTo(() => ProductRepository.Save(A<Product>.Ignored)).Returns(MockedProduct);
            var result = ProductsController.Put(MockedProduct.ProductId, MockedProduct);

            //Assert
            A.CallTo(() => ProductRepository.Save(A<Product>.Ignored)).MustHaveHappened();
            Assert.NotNull(result);
            Assert.True(result is OkResult);

        }


        public void Dispose()
        {
            ProductRepository = null;
            ProductsController = null;
            MockedProducts = null;
        }
    }
}
