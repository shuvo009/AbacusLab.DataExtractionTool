using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using AbacusLab.DataExtractionTool.Entitys.Amazon;
using AbacusLab.DataExtractionTool.Implementation.AmazoneService;
using AbacusLab.DataExtractionTool.Implementation.Base;
using AbacusLab.DataExtractionTool.Implementation.Dependence;
using AbacusLab.DataExtractionTool.Interface;
using AbacusLab.DataExtractionTool.Interface.Download;
using Microsoft.Practices.Unity;

namespace AbacusLab.DataExtractionTool.Implementation.Download.Amazon
{
    public class AmazonProductInfoDownloaded : DownloadBase, IAmazonProductInfoDownloaded
    {
        private readonly string _associateTag;
        private readonly string _accessKeyId;
        private readonly AWSECommerceServicePortTypeClient _amazonClient;

        public AmazonProductInfoDownloaded()
        {
            _accessKeyId = Properties.Settings.Default.AmazonAccessKey;
            var secretAccessKey = Properties.Settings.Default.AmazonSecretAccessKey;
            _associateTag = Properties.Settings.Default.AmazonAssociateTag;

            var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport) { MaxReceivedMessageSize = int.MaxValue };

            _amazonClient = new AWSECommerceServicePortTypeClient(
                        binding,
                        new EndpointAddress("https://webservices.amazon.co.uk/onca/soap?Service=AWSECommerceService"));

            _amazonClient.ChannelFactory.Endpoint.Behaviors.Add(new AmazonSigningEndpointBehavior(_accessKeyId, secretAccessKey));
        }

        public async Task<bool> DownloadProductByLookup(string isbnOrAsin, bool searchTypeIsIsbn, string saveFilePath)
        {
            AddTitles(saveFilePath);
            ProgressComplete = 0;
            return await Task.Run(() =>
            {
                isbnOrAsin = isbnOrAsin.Replace("-", "");
                var lookup = new ItemLookup();
                var request = new ItemLookupRequest
                {
                    IdType = searchTypeIsIsbn ? ItemLookupRequestIdType.ISBN : ItemLookupRequestIdType.ASIN,
                    ItemId = new[] { isbnOrAsin },
                    ResponseGroup = new[]
                                                    {
                                                      "Large"
                                                     },
                    SearchIndex = searchTypeIsIsbn ? AmazonSearchIndex.Books.ToString() : null,
                    IdTypeSpecified = true
                };

                lookup.Request = new[] { request };
                lookup.AWSAccessKeyId = _accessKeyId;
                lookup.AssociateTag = _associateTag;
                var response = _amazonClient.ItemLookup(lookup);
                return ReadResponse(response.Items, saveFilePath);
            });
        }

        public async Task<bool> DownloadProductBySearch(string searchText, string searchCategory, string saveFilePath)
        {
            AddTitles(saveFilePath);
            ProgressComplete = 0;
            return await Task.Run(() =>
            {
                Int64 currentPageNumber = 1;
                Int64 totalPage = 20;
                do
                {
                    IsIndeterminate = true;
                    var itemSearch = new ItemSearch();
                    var itemSearchRequest = new ItemSearchRequest
                    {
                        Keywords = searchText,
                        SearchIndex = searchCategory,
                        ResponseGroup = new[] { "Large" },
                        ItemPage = currentPageNumber.ToString(CultureInfo.InvariantCulture)
                    };

                    itemSearch.Request = new[] { itemSearchRequest };
                    itemSearch.AWSAccessKeyId = _accessKeyId;
                    itemSearch.AssociateTag = _associateTag;
                    var response = _amazonClient.ItemSearch(itemSearch);
                    currentPageNumber++;
                    IsIndeterminate = false;
                    var isSuccess = ReadResponse(response.Items, saveFilePath).Result;
                    if (!isSuccess)
                        return false;
                    var pages = Convert.ToInt64(response.Items.First().TotalPages);
                    if (pages < 21)
                        totalPage = pages;
                    ProgressMaxValue = 100;
                } while (totalPage > currentPageNumber);
                return true;
            });
        }

        #region Private Methods
        private async Task<bool> ReadResponse(IEnumerable<Items> items, string saveFilePath)
        {
            try
            {
                foreach (var product in items)
                {
                    foreach (var bookItemWithItem in product.Item)
                    { 
                        var productInformation = new ProductInformation();

                        productInformation.URL = bookItemWithItem.DetailPageURL ?? String.Empty;
                        if (bookItemWithItem.ItemAttributes != null)
                        {
                            productInformation.Name = bookItemWithItem.ItemAttributes.Title ?? String.Empty;
                            productInformation.IsbnName = bookItemWithItem.ItemAttributes.ISBN ?? String.Empty;
                        }

                        if (bookItemWithItem.OfferSummary != null)
                        {
                            productInformation.NewPrice = bookItemWithItem.OfferSummary.LowestNewPrice != null ? bookItemWithItem.OfferSummary.LowestNewPrice.FormattedPrice ?? string.Empty : string.Empty;
                            productInformation.NewTotal = bookItemWithItem.OfferSummary.TotalNew ?? string.Empty;
                            productInformation.UsedTotal = bookItemWithItem.OfferSummary.TotalUsed ?? string.Empty;
                            productInformation.UsedPrice = bookItemWithItem.OfferSummary.LowestUsedPrice != null ? bookItemWithItem.OfferSummary.LowestUsedPrice.FormattedPrice ?? String.Empty : String.Empty;
                        }

                        if (bookItemWithItem.LargeImage != null)
                        {
                            productInformation.ImageName = Path.GetFileName(bookItemWithItem.LargeImage.URL);
                            productInformation.ImageUrl = bookItemWithItem.LargeImage.URL;
                        }

                        try
                        {
                            productInformation.Price = bookItemWithItem.Offers.Offer[0].OfferListing[0].Price.FormattedPrice;
                        }
                        catch (Exception)
                        {
                            productInformation.Price = string.Empty;
                        }
                        if (String.IsNullOrEmpty(productInformation.IsbnThirteen))
                            productInformation.IsbnThirteen = " ";

                        productInformation.StockInOrOut = await GetInOrOutStock(bookItemWithItem.DetailPageURL);
                        productInformation.Asin = bookItemWithItem.ASIN ?? String.Empty;
                        var isSuccess = await ExcelWriterService.SaveInFile(productInformation, saveFilePath);
                        if (!isSuccess)
                            return false;
                        ProgressComplete++;
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<string> GetInOrOutStock(string url)
        {
            IStringDownload download = new DownloadHtml();
            using (var sr = new StreamReader(await download.Download(url)))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.IndexOf("In Stock", StringComparison.OrdinalIgnoreCase) > -1)
                        return "In Stock";
                }
            }
            return string.Empty;
        }

        private void AddTitles(string saveFilePath)
        {
            var productInformation = new ProductInformation
            {
                Name = "Name",
                Asin = "Asin",
                ImageName = "ImageName",
                ImageUrl = "ImageUrl",
                IsbnName = "ISBN",
                IsbnThirteen = "ISBN 13",
                NewPrice = "NewPrice",
                NewTotal = "NewTotal",
                Price = "Price",
                StockInOrOut = "StockInOrOut",
                URL = "URL",
                UsedPrice = "UsedPrice",
                UsedTotal = "UsedTotal"
            };
            ExcelWriterService.SaveInFile(productInformation, saveFilePath);
        }
        #endregion
    }
}
//  request.ResponseGroup = new[] { "Small","Accessories","AlternateVersion","BrowseNodes","Collections","EditorialReview","Images","ItemAttributes","ItemIds","Large","ListmaniaLists","Medium","MerchantltemAttributes","OfferFull","OfferListing","Offers","OfferSummary","PromotionalTag","PromotionDetails","PromotionSummary","RelatedItems","Reviews","SalesRank","SearchInside","ShippingCharges","Similarities","Subjects","Tags","TagsSummary","Tracks","VariationImages","VariationMatrix","VariationMinimum","VariationOffers","Variations","VaritionSummary" };
