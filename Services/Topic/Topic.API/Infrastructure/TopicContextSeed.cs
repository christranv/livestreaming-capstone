using System;
using global::Topic.API.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Topic.API.Models;
using System.Text.RegularExpressions;

namespace Topic.API.Infrastructure
{
    public class TopicContextSeed
    {
        public async Task SeedAsync(TopicContext context, IWebHostEnvironment env, IOptions<TopicSettings> settings,
            ILogger<TopicContextSeed> logger)
        {
            var policy = CreatePolicy(logger, nameof(TopicContextSeed));

            await policy.ExecuteAsync(async () =>
            {
                var contentRootPath = env.ContentRootPath;
                var picturePath = env.WebRootPath;
                var picApiPath = settings.Value.CategoryPicApiPath;

                if (!context.TagItems.Any())
                {
                    await context.TagItems.AddRangeAsync(GetTopicTagsFromFile(contentRootPath, logger));
                    await context.SaveChangesAsync();
                }

                if (!context.CategoryItems.Any())
                {
                    await context.CategoryItems.AddRangeAsync(GetTopicCategoriesFromFile(contentRootPath, picApiPath,
                        context, logger));
                    await context.SaveChangesAsync();
                }


                GetCatalogItemPictures(contentRootPath, picturePath);
            });
        }

        private IEnumerable<TopicCategory> GetTopicCategoriesFromFile(string contentRootPath, string picApiPath,
            TopicContext context, ILogger<TopicContextSeed> logger)
        {
            string csvFileCatalogBrands = Path.Combine(contentRootPath, "Setup", "TopicCategories.csv");

            if (!File.Exists(csvFileCatalogBrands))
            {
                return new List<TopicCategory>();
            }

            string[] csvheaders;
            try
            {
                string[] requiredHeaders = {"name", "tags"};
                csvheaders = GetHeaders(csvFileCatalogBrands, requiredHeaders);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
                return new List<TopicCategory>();
            }

            var topicTagIdLookup = context.TagItems.ToDictionary(ct => ct.Name, ct => ct);

            return File.ReadAllLines(csvFileCatalogBrands)
                .Skip(1) // skip header row
                .Select(row => Regex.Split(row, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"))
                .SelectTry(column => CreateCategory(column, csvheaders, topicTagIdLookup, picApiPath))
                .OnCaughtException(ex =>
                {
                    logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
                    return null;
                })
                .Where(x => x != null);
        }

        private TopicCategory CreateCategory(string[] column, string[] headers,
            Dictionary<string, TopicTag> topicTagIdLookup, string picApiPath)
        {
            if (column.Count() != headers.Count())
            {
                throw new Exception(
                    $"column count '{column.Count()}' not the same as headers count'{headers.Count()}'");
            }

            var name = column[Array.IndexOf(headers, "name")].Trim('"').Trim();
            var displayOrder = int.Parse(column[Array.IndexOf(headers, "display-order")].Trim('"').Trim());
            var categoryItem = new TopicCategory(
                Guid.NewGuid().ToString(),
                displayOrder,
                name,
                picApiPath.Replace("[0]", name + "-285x380.jpg")
            );

            string[] tagNames = Regex.Split(column[Array.IndexOf(headers, "tags")].Trim('"').Trim(), @"\|");
            foreach (string tagName in tagNames)
            {
                if (!topicTagIdLookup.ContainsKey(tagName))
                {
                    throw new Exception($"type={tagName} does not exist in topic tags");
                }

                categoryItem.CategoryTags.Add(new TopicCategoryTags
                    {Category = categoryItem, Tag = topicTagIdLookup[tagName]});
            }

            return categoryItem;
        }

        private IEnumerable<TopicTag> GetTopicTagsFromFile(string contentRootPath, ILogger<TopicContextSeed> logger)
        {
            string csvFileCatalogTypes = Path.Combine(contentRootPath, "Setup", "TopicTags.csv");

            if (!File.Exists(csvFileCatalogTypes))
            {
                return new List<TopicTag>();
            }

            string[] csvheaders;
            try
            {
                string[] requiredHeaders = {"name"};
                csvheaders = GetHeaders(csvFileCatalogTypes, requiredHeaders);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
                return new List<TopicTag>();
            }

            return File.ReadAllLines(csvFileCatalogTypes)
                .Skip(1) // skip header row
                .SelectTry(x => CreateTopicTag(x))
                .OnCaughtException(ex =>
                {
                    logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
                    return null;
                })
                .Where(x => x != null);
        }

        private TopicTag CreateTopicTag(string name)
        {
            name = name.Trim('"').Trim();

            if (String.IsNullOrEmpty(name))
            {
                throw new Exception("catalog Type Name is empty");
            }

            return new TopicTag(Guid.NewGuid().ToString(), name);
        }

        private string[] GetHeaders(string csvfile, string[] requiredHeaders, string[] optionalHeaders = null)
        {
            string[] csvheaders = File.ReadLines(csvfile).First().ToLowerInvariant().Split(',');

            if (csvheaders.Count() < requiredHeaders.Count())
            {
                throw new Exception(
                    $"requiredHeader count '{requiredHeaders.Count()}' is bigger then csv header count '{csvheaders.Count()}' ");
            }

            if (optionalHeaders != null)
            {
                if (csvheaders.Count() > (requiredHeaders.Count() + optionalHeaders.Count()))
                {
                    throw new Exception(
                        $"csv header count '{csvheaders.Count()}'  is larger then required '{requiredHeaders.Count()}' and optional '{optionalHeaders.Count()}' headers count");
                }
            }

            foreach (var requiredHeader in requiredHeaders)
            {
                if (!csvheaders.Contains(requiredHeader))
                {
                    throw new Exception($"does not contain required header '{requiredHeader}'");
                }
            }

            return csvheaders;
        }

        private void GetCatalogItemPictures(string contentRootPath, string picturePath)
        {
            if (picturePath != null)
            {
                DirectoryInfo directory = new DirectoryInfo(picturePath);
                if (directory.GetFiles().Count() > 0) return;
                foreach (FileInfo file in directory.GetFiles())
                {
                    file.Delete();
                }

                string zipFileCatalogItemPictures = Path.Combine(contentRootPath, "Setup", "CategoryImages.zip");
                ZipFile.ExtractToDirectory(zipFileCatalogItemPictures, picturePath);
            }
        }

        private AsyncRetryPolicy CreatePolicy(ILogger<TopicContextSeed> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<SqlException>().WaitAndRetryAsync(
                retryCount: retries,
                sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                onRetry: (exception, timeSpan, retry, ctx) =>
                {
                    logger.LogWarning(exception,
                        "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}",
                        prefix, exception.GetType().Name, exception.Message, retry, retries);
                }
            );
        }
    }
}