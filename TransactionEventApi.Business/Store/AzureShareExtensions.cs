using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Azure.Storage.Files.Shares;
using Newtonsoft.Json;

namespace Glasswall.Administration.K8.TransactionEventApi.Business.Store
{
    public static class AzureShareExtensions
    {
        public static Task<TDownload> ReadJsonAsync<TDownload>(this ShareFileClient file)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            return InternalReadJsonAsync<TDownload>(file);
        }

        public static Task<TDownload> ReadXmlAsync<TDownload>(this ShareFileClient file)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            return InternalReadXmlAsync<TDownload>(file);
        }
        
        public static Task<string> ReadAsync(this ShareFileClient file)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            return InternalReadAsync(file);
        }

        private static async Task<TDownload> InternalReadJsonAsync<TDownload>(ShareFileClient file)
        {
            var fileContents = await InternalReadAsync(file);
            return JsonConvert.DeserializeObject<TDownload>(fileContents);
        }

        private static async Task<TDownload> InternalReadXmlAsync<TDownload>(ShareFileClient file)
        {
            var fileContents = await file.DownloadAsync();
            var serializer = new XmlSerializer(typeof(TDownload));
            var reader = new StreamReader(fileContents.Value.Content);
            var obj = (TDownload)serializer.Deserialize(reader);
            reader.Close();
            return obj;
        }

        private static async Task<string> InternalReadAsync(this ShareFileClient file)
        {
            var fileContents = await file.DownloadAsync();
            using var ms = new MemoryStream();
            await fileContents.Value.Content.CopyToAsync(ms);
            var fileBytes = ms.ToArray();
            return Encoding.UTF8.GetString(fileBytes);
        }
    }
}
