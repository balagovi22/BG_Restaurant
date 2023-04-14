using AutoMapper.Configuration.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Restaurant.ProductAPI.Repository;

namespace ProductAP.UnitTestsI.Mocks
{
    public class MockIProductRepository
    {
        public static Mock<IProductRepository> GetMock()
        {
            var mock= new Mock<IProductRepository>();
            return mock;
        }
    }
}
