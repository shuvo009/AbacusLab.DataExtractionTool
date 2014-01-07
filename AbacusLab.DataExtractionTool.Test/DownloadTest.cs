using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbacusLab.DataExtractionTool.Implementation.Dependence;
using AbacusLab.DataExtractionTool.Implementation.Download;
using AbacusLab.DataExtractionTool.Implementation.Download.Amazon;
using AbacusLab.DataExtractionTool.Interface;
using AbacusLab.DataExtractionTool.Interface.Download;
using Xunit;
using Microsoft.Practices.Unity;

namespace AbacusLab.DataExtractionTool.UnitTest
{
    public class DownloadTest
    {
        [Fact]
        public void DownloadHtmlTest()
        {
            IStringDownload stringDownload = new DownloadHtml();
            var downloadPath = stringDownload.Download("http://google.com").Result;
            var actualFilePath = Path.Combine(Path.GetTempPath(), "AbacusLabTempTextFile.txt");
            Assert.Equal(actualFilePath, downloadPath);
        }

        [Fact]
        public void DownloadVCardTest()
        {
            IStringDownload stringDownload = new DownloadVCard();
            var downloadPath = stringDownload.Download(@"http://www.slk-law.com/webportal/perform.v?obj=ve_oid:poid:Z1tOl9NPl0LPoDtRkfJDm0JC&action=vCard").Result;
            var actualFilePath = Path.Combine(Path.GetTempPath(), "AbacusLabVcard.vcf");
            Assert.Equal(downloadPath, actualFilePath);
        }

        [Fact]
        public void AmazonLookUpTest()
        {
            var amazonProductAmazonProductInfo = DependenceResolver.Resolver.Container.Resolve<IAmazonProductInfoDownloaded>();
        //    var productsInfo = amazonProductAmazonProductInfo.DownloadProductByLookup("978-1617291340", true).Result; // Using ISBN on Book
            var productsInfo = amazonProductAmazonProductInfo.DownloadProductByLookup("B009VBX8LM", false, @"H:\TestResult\AmazonLookUpTest.xlsx").Result; // Using ASIN With Search Category on Book
            Assert.Equal(productsInfo, true);
        }


        [Fact]
        public void AmazonSearchTest()
        {
            var amazonProductAmazonProductInfo = DependenceResolver.Resolver.Container.Resolve<IAmazonProductInfoDownloaded>();
            var productsInfo = amazonProductAmazonProductInfo.DownloadProductBySearch("video games", "VideoGames", @"H:\TestResult\AmazonSearchTest.xlsx").Result; // Using ASIN With Search Category on Book
            Assert.Equal(productsInfo, true);
        }

        [Fact]
        public void GooglePlaceSearchTest()
        {
            var googlePlaceSearch = DependenceResolver.Resolver.Container.Resolve<IGooglePlaceDownload>();
            var isSuccess = googlePlaceSearch.DownloadPlaceInfo("Law firm in London", @"H:\TestResult\GooglePlaceSearchTest.xlsx").Result;
            Assert.Equal(isSuccess, true);
        }
    }
}
