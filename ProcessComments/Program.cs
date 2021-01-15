using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using NLog.Extensions.Logging;
using ProcessComments.Contexts;
using ProcessComments.Entities;

namespace ProcessComments
{
    class Program
    {
        static void Main(string[] args)
        {
            var batchSize = 50;
            var startIndex = 31900;
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            
            
            var logger = LoggerFactory.Create(buidlder => buidlder.AddNLog())
                .CreateLogger<Program>();
            
            using var transactionsContext = new TransactionsContext(configuration.GetConnectionString("TransactionsContextProd"));
            using var dataContext = new DataStorageContext(configuration.GetConnectionString("DataStorageContextProd"));

            var transactionsCount = transactionsContext.Transactions
                .Count(t => t.Code == 3
                            || t.Code == 16
                            || t.Code == 17
                            || t.Code == 19
                            || t.Code == 18);

            var commentsAdd = 0;

            for (int i = startIndex; i < transactionsCount; i += batchSize)
            {
                var transactions = transactionsContext.Transactions
                    .Where(t => t.Code == 3
                                || t.Code == 16
                                || t.Code == 17
                                || t.Code == 19
                                || t.Code == 18)
                    .Skip(i)
                    .Take(batchSize);
                
                logger.LogInformation($"Обработка транзакций на файле i = {i}");
                
                var comments = new List<TransactionComment>();

                foreach (var transaction in transactions)
                {
                    var file = transactionsContext.Files
                        .Where(f => f.Code == 7
                                    || f.Code == 10
                                    || f.Code == 11)
                        .FirstOrDefault(f => f.TransactionId == transaction.Id);
                    
                    if (file == null)
                    {
                        continue;
                    }

                    var fileContent = transactionsContext.FileContents.Find(file.Id);

                    if (fileContent == null)
                        continue;

                    var doc = new XmlDocument();

                    var tagName = GetTagName(transaction.Code);

                    try
                    {
                        if (fileContent.DataStorageId.HasValue)
                        {
                            var data = dataContext.Data.Find(fileContent.DataStorageId).Bytes;
                            var xml = Encoding.GetEncoding(1251).GetString(data);
                            doc.LoadXml(xml);
                        }

                        if (fileContent.Content != null)
                        {
                            var xml = Encoding.GetEncoding(1251).GetString(fileContent.Content);
                            doc.LoadXml(xml);
                        }

                        var commentNodes = doc.GetElementsByTagName(tagName);

                        if (commentNodes.Count <= 0)
                            continue;

                        logger.LogInformation(transaction.Id + " " + commentNodes[0].InnerText);

                        comments.Add(new TransactionComment
                        {
                            Comment = commentNodes[0].InnerText,
                            TransactionId = transaction.Id
                        });
                        commentsAdd++;
                    }
                    catch (Exception e)
                    {
                        logger.LogCritical(transaction.Id + " " + e.StackTrace);
                        logger.LogCritical($"Остановка обработки коммертариев на файле с номером i = {i}");
                        Console.ReadLine();
                        return;
                    }
                }
                
                transactionsContext.TransactionComments.AddRange(comments);
                
                transactionsContext.SaveChanges();
            }
            
            Console.WriteLine("Найденно транзакция для сохранения: " + transactionsCount);
            Console.WriteLine("Сохранено комментариев: " + commentsAdd);
        }

        private static string GetTagName(int code)
        {
            switch (code)
            {
                case 3:
                    return "ТекстУведУточ";
                case 16:
                    return "ТекстПредАн";
                case 18:
                    return "ТекстПредАн";
                case 17:
                    return "ТекстПредАн";
                case 19:
                    return "ТекстУведУточ";
                default:
                    return "";
            }
        }
    }
}
